# /// script
# requires-python = ">=3.12"
# dependencies = [
#     "click>=8.5.0",
#     "markdown-it-py>=3.0.0",
#     "rich",
# ]
# ///

import json
import re
import string
from collections.abc import Iterable, Iterator, Sequence
from dataclasses import dataclass
from pathlib import Path

import click
from markdown_it import MarkdownIt
from markdown_it.token import Token
from rich.console import Console
from rich.progress import BarColumn, MofNCompleteColumn, Progress, TextColumn

ROOT = Path(__file__).resolve().parent.parent
SRC_DIR = ROOT / "src"
DEFAULT_TEMPLATE = ROOT / "templates" / "nexus-description.txt"
NEXUS_CONFIG = ROOT / "nexus.json"
DEFAULT_OUTPUT_DIR = ROOT / "nexus-descriptions"
TEMPLATE_FIELDS = frozenset({"name", "overview", "sections", "files_url", "bugs_url"})

console = Console()


class DescriptionError(Exception):
    """An invalid mod README or manifest."""


@dataclass(frozen=True)
class Manifest:
    id: str
    name: str
    description: str


@dataclass(frozen=True)
class Readme:
    title: str
    overview: str
    sections: str


def read_nexus_mod_page_ids(path: Path) -> dict[str, int]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
        mods = data.get("mods")
        if not isinstance(mods, dict):
            raise TypeError("'mods' must be an object")
        page_ids: dict[str, int] = {}
        for mod_id, mod_data in mods.items():
            if not isinstance(mod_id, str) or not isinstance(mod_data, dict):
                raise TypeError("each mod entry must be an object")
            page_id = mod_data["mod_page_id"]
            if isinstance(page_id, bool) or not isinstance(page_id, int):
                raise TypeError(f"{mod_id!r} mod_page_id must be a number")
            page_ids[mod_id] = page_id
        return page_ids
    except (OSError, KeyError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise DescriptionError(f"invalid Nexus configuration {path}: {exc}") from exc


def parse_manifest(content: str, path: Path) -> Manifest:
    try:
        manifest = json.loads(content)
        for field in ("id", "name", "description"):
            if not isinstance(manifest.get(field), str) or not manifest[field].strip():
                raise ValueError(f"missing non-empty {field!r}")
        return Manifest(
            id=manifest["id"],
            name=manifest["name"],
            description=manifest["description"],
        )
    except (OSError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise DescriptionError(f"invalid manifest {path}: {exc}") from exc


def read_manifest(path: Path) -> Manifest:
    try:
        content = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise DescriptionError(f"unable to read manifest {path}: {exc}") from exc
    return parse_manifest(content, path)


def validate_template(template: str) -> None:
    try:
        fields = {
            field_name
            for _, field_name, _, _ in string.Formatter().parse(template)
            if field_name is not None
        }
    except ValueError as exc:
        raise DescriptionError(f"invalid description template: {exc}") from exc

    unknown_fields = fields - TEMPLATE_FIELDS
    if unknown_fields:
        names = ", ".join(sorted(unknown_fields))
        raise DescriptionError(f"unknown template field(s): {names}")


def heading_level(token: Token) -> int:
    return int(token.tag.removeprefix("h"))


def heading_text(token: Token) -> str:
    return token.content.strip()


def get_description_sections(tokens: Sequence[Token]) -> list[list[Token]]:
    sections: list[list[Token]] = []
    current: list[Token] | None = None

    for index, token in enumerate(tokens):
        if token.type == "heading_open" and heading_level(token) == 2:
            if heading_text(tokens[index + 1]) == "Development":
                break
            current = [token]
            sections.append(current)
        elif current is not None:
            current.append(token)

    return sections


def render_inline(tokens: Iterable[Token]) -> str:
    result: list[str] = []
    for token in tokens:
        match token.type:
            case "text" | "code_inline":
                result.append(token.content)
            case "softbreak" | "hardbreak":
                result.append("\n")
            case "strong_open":
                result.append("[b]")
            case "strong_close":
                result.append("[/b]")
            case "em_open":
                result.append("[i]")
            case "em_close":
                result.append("[/i]")
            case "s_open":
                result.append("[s]")
            case "s_close":
                result.append("[/s]")
            case "link_open":
                result.append(f"[url={token.attrs['href']}]")
            case "link_close":
                result.append("[/url]")
            case "image":
                result.append(token.content or str(token.attrs.get("alt", "")))
            case _:
                raise DescriptionError(
                    f"unsupported inline Markdown token: {token.type}"
                )
    return "".join(result)


def next_token(tokens: Iterator[Token], expected_type: str) -> Token:
    try:
        token = next(tokens)
    except StopIteration as exc:
        raise DescriptionError(f"expected {expected_type} token") from exc
    if token.type != expected_type:
        raise DescriptionError(f"expected {expected_type} token, got {token.type}")
    return token


def render_blocks(tokens: Sequence[Token]) -> str:
    result: list[tuple[str, str]] = []
    list_depth = 0

    def append_block(content: str, separator: str = "\n\n") -> None:
        if content:
            result.append((content, separator))

    token_iterator = iter(tokens)
    while token := next(token_iterator, None):
        match token.type:
            case "heading_open":
                level = heading_level(token)
                if level not in (2, 3):
                    raise DescriptionError(f"unsupported heading level: {level}")
                content = next_token(token_iterator, "inline")
                next_token(token_iterator, "heading_close")
                size = 5 if level == 2 else 4
                append_block(
                    f"{'[line]\n' if level == 2 else ''}[size={size}][b]"
                    f"{render_inline(content.children or [])}[/b][/size]",
                    "\n" if level == 3 else "\n\n",
                )
            case "paragraph_open":
                content = next_token(token_iterator, "inline")
                next_token(token_iterator, "paragraph_close")
                append_block(render_inline(content.children or []))
            case "bullet_list_open":
                list_depth += 1
                append_block("[list]")
            case "ordered_list_open":
                list_depth += 1
                append_block("[list=1]")
            case "list_item_open":
                append_block("[*]")
            case "list_item_close":
                append_block("[/*]")
            case "bullet_list_close" | "ordered_list_close":
                if list_depth == 0:
                    raise DescriptionError("unexpected Markdown list close")
                append_block("[/list]")
                list_depth -= 1
            case "fence" | "code_block":
                append_block(token.content.rstrip("\n"))
            case "inline" | "html_block" | "blank_line":
                continue
            case _:
                raise DescriptionError(f"unsupported Markdown token: {token.type}")

    if list_depth:
        raise DescriptionError("unclosed Markdown list")
    return "".join(
        content + separator for content, separator in result if content
    ).strip()


def parse_readme(markdown: str, path: Path) -> Readme:
    tokens = MarkdownIt().parse(markdown)

    try:
        title_end = next(
            index
            for index, token in enumerate(tokens)
            if token.type == "heading_close" and heading_level(token) == 1
        )
    except StopIteration as exc:
        raise DescriptionError(f"README has no level-one title: {path}") from exc

    title = render_inline(tokens[title_end - 1].children or [])

    try:
        first_paragraph_end = next(
            (
                index
                for index, token in enumerate(tokens)
                if index > title_end and token.type == "paragraph_close"
            ),
        )
    except StopIteration as exc:
        raise DescriptionError(f"README overview is empty in {path}") from exc

    first_section = next(
        (
            index
            for index, token in enumerate(tokens)
            if index > title_end
            and token.type == "heading_open"
            and heading_level(token) == 2
        ),
        len(tokens),
    )

    overview = render_blocks(tokens[first_paragraph_end + 1 : first_section])

    description_sections = get_description_sections(tokens)
    sections = render_blocks(
        [token for section in description_sections for token in section]
    )
    return Readme(title=title, overview=overview, sections=sections)


def read_readme(path: Path) -> Readme:
    try:
        markdown = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise DescriptionError(f"unable to read {path}: {exc}") from exc
    return parse_readme(markdown, path)


def render_description(
    manifest: Manifest, readme: Readme, template: str, mod_page_id: int
) -> str:
    if readme.title != manifest.name:
        raise DescriptionError(
            f"README title {readme.title!r} does not match manifest name {manifest.name!r}"
        )

    description = template.format(
        name=manifest.name,
        overview=readme.overview,
        sections=readme.sections,
        files_url=f"https://www.nexusmods.com/terraria/mods/{mod_page_id}?tab=files",
        bugs_url=f"https://www.nexusmods.com/terraria/mods/{mod_page_id}?tab=bugs",
    )
    return re.sub(r"\n{3,}", "\n\n", description).rstrip() + "\n"


def find_mods(selected: tuple[str, ...]) -> list[Path]:
    selected_ids = {value.casefold() for value in selected}
    paths = sorted(path for path in SRC_DIR.iterdir() if path.is_dir())
    if not selected_ids:
        return paths
    return [
        path
        for path in paths
        if path.name.casefold() in selected_ids
        or read_manifest(path / "manifest.json").id.casefold() in selected_ids
    ]


@click.command()
@click.argument("mod_names", nargs=-1)
@click.option(
    "--output-dir",
    type=click.Path(path_type=Path, file_okay=False),
    default=DEFAULT_OUTPUT_DIR,
    show_default=True,
)
@click.option(
    "--template",
    "template_path",
    type=click.Path(path_type=Path, dir_okay=False, exists=True),
    default=DEFAULT_TEMPLATE,
    show_default=True,
)
def main(
    mod_names: tuple[str, ...],
    output_dir: Path,
    template_path: Path,
) -> None:
    try:
        template = template_path.read_text(encoding="utf-8")
        validate_template(template)
        nexus_mod_page_ids = read_nexus_mod_page_ids(NEXUS_CONFIG)
        mods = find_mods(mod_names)
        if mod_names and not mods:
            raise DescriptionError(f"no matching mods found: {', '.join(mod_names)}")
    except (
        DescriptionError,
        OSError,
        TypeError,
        ValueError,
        json.JSONDecodeError,
    ) as exc:
        raise click.ClickException(str(exc)) from exc

    failed_mods: dict[str, str] = {}
    with Progress(
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        task = progress.add_task("Generating descriptions", total=len(mods))
        for mod in mods:
            progress.update(task, description=f"Generating {mod.name}")
            try:
                manifest = read_manifest(mod / "manifest.json")
                readme = read_readme(mod / "README.md")

                mod_page_id = nexus_mod_page_ids.get(manifest.id)
                if not mod_page_id:
                    raise DescriptionError(
                        f"no Nexus configuration found for mod {manifest.id!r}"
                    )
                content = render_description(manifest, readme, template, mod_page_id)

                destination = output_dir / f"{manifest.id}.txt"
                output_dir.mkdir(parents=True, exist_ok=True)

                destination.write_text(content, encoding="utf-8", newline="\n")
            except (DescriptionError, OSError) as exc:
                failed_mods[mod.name] = str(exc)
            finally:
                progress.advance(task)

    success_count = len(mods) - len(failed_mods)
    console.print(
        f"Finished generating descriptions: [green]{success_count}[/green] successful, [red]{len(failed_mods)}[/red] failed."
    )
    for mod_name, error in failed_mods.items():
        console.print(f"[red]- {mod_name}: {error}[/red]")


if __name__ == "__main__":
    main()
