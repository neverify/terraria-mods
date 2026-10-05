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
from collections.abc import Iterable, Sequence
from pathlib import Path

import click
from markdown_it import MarkdownIt
from markdown_it.token import Token
from rich.console import Console
from rich.progress import BarColumn, MofNCompleteColumn, Progress, TextColumn

ROOT = Path(__file__).resolve().parent.parent
SRC_DIR = ROOT / "src"
DEFAULT_TEMPLATE = ROOT / "templates" / "nexus-description.txt"
DEFAULT_OUTPUT_DIR = ROOT / "nexus-descriptions"
console = Console()


class DescriptionError(Exception):
    """An invalid mod README or manifest."""


def load_manifest(path: Path) -> dict[str, str]:
    try:
        manifest = json.loads(path.read_text(encoding="utf-8"))
        for field in ("id", "name", "description", "homepage"):
            if not isinstance(manifest.get(field), str) or not manifest[field].strip():
                raise ValueError(f"missing non-empty {field!r}")
        return manifest
    except (OSError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise DescriptionError(f"invalid manifest {path}: {exc}") from exc


def heading_level(token: Token) -> int:
    return int(token.tag.removeprefix("h"))


def heading_text(token: Token) -> str:
    return token.content.strip()


def get_sections(tokens: Sequence[Token]) -> list[list[Token]]:
    sections: list[list[Token]] = []
    current: list[Token] | None = None

    for index, token in enumerate(tokens):
        if token.type == "heading_open" and heading_level(token) == 2:
            if heading_text(tokens[index + 1]) == "Development":
                break
            current = [token]
            sections.append(current)
            continue
        if current is not None:
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


def render_blocks(tokens: Sequence[Token]) -> str:
    result: list[tuple[str, str]] = []
    list_stack: list[str] = []

    def append_block(content: str, separator: str = "\n\n") -> None:
        result.append((content, separator))

    index = 0
    while index < len(tokens):
        token = tokens[index]
        if token.type == "heading_open":
            level = heading_level(token)
            if level not in (2, 3):
                raise DescriptionError(f"unsupported heading level: {level}")
            content = tokens[index + 1]
            size = 5 if level == 2 else 4
            append_block(
                f"{'[line]\n' if level == 2 else ''}[size={size}][b]"
                f"{render_inline(content.children or [])}[/b][/size]",
                "\n" if level == 3 else "\n\n",
            )
            index += 3
        elif token.type == "paragraph_open":
            content = tokens[index + 1]
            append_block(render_inline(content.children or []))
            index += 3
        elif token.type == "bullet_list_open":
            list_stack.append("bullet")
            append_block("[list]")
            index += 1
        elif token.type == "ordered_list_open":
            list_stack.append("ordered")
            append_block("[list=1]")
            index += 1
        elif token.type == "list_item_open":
            append_block("[*]")
            index += 1
        elif token.type == "list_item_close":
            append_block("[/*]")
            index += 1
        elif token.type == "bullet_list_close" or token.type == "ordered_list_close":
            append_block("[/list]")
            list_stack.pop()
            index += 1
        elif token.type == "fence" or token.type == "code_block":
            append_block(token.content.rstrip("\n"))
            index += 1
        elif token.type in {"inline", "html_block", "blank_line"}:
            index += 1
        else:
            raise DescriptionError(f"unsupported Markdown token: {token.type}")

    if list_stack:
        raise DescriptionError("unclosed Markdown list")
    return "".join(
        content + separator for content, separator in result if content
    ).strip()


def parse_readme(path: Path) -> tuple[str, str, str]:
    try:
        markdown = path.read_text(encoding="utf-8")
    except OSError as exc:
        raise DescriptionError(f"unable to read {path}: {exc}") from exc

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

    sections = get_sections(tokens)
    configuration = render_blocks([token for section in sections for token in section])
    return title, overview, configuration


def render_description(manifest: dict[str, str], readme: Path, template: str) -> str:
    title, overview, sections = parse_readme(readme)
    if title != manifest["name"]:
        raise DescriptionError(
            f"README title {title!r} does not match manifest name {manifest['name']!r}"
        )

    description = template.format(
        name=manifest["name"],
        overview=overview,
        sections=sections,
        files_url=f"{manifest['homepage']}?tab=files",
        bugs_url=f"{manifest['homepage']}?tab=bugs",
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
        or json.loads((path / "manifest.json").read_text(encoding="utf-8"))[
            "id"
        ].casefold()
        in selected_ids
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
        mods = find_mods(mod_names)
        if mod_names and not mods:
            raise DescriptionError(f"no matching mods found: {', '.join(mod_names)}")
    except (OSError, KeyError, TypeError, ValueError, json.JSONDecodeError) as exc:
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
                manifest = load_manifest(mod / "manifest.json")
                content = render_description(manifest, mod / "README.md", template)

                destination = output_dir / f"{manifest['id']}.txt"
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
