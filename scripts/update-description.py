# /// script
# requires-python = ">=3.12"
# dependencies = [
#     "click>=8.5.0",
#     "pyperclip>=1.11.0",
#     "rich>=15.0.0",
# ]
# ///

import json
import webbrowser
from pathlib import Path

import click
import pyperclip
from rich.console import Console
from rich.progress import BarColumn, MofNCompleteColumn, Progress, TextColumn

ROOT = Path(__file__).resolve().parent.parent
DESCRIPTION_DIR = ROOT / "nexus-descriptions"
NEXUS_CONFIG = ROOT / "nexus.json"
NEXUS_MOD_URL = "https://www.nexusmods.com/games/terraria/mods/{page_id}/edit/general"

console = Console()


class UpdateError(Exception):
    """An error preparing the manual description update workflow."""


def load_page_ids(path: Path) -> dict[str, int]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
        if not isinstance(data, dict):
            raise TypeError("invalid Nexus configuration: root must be an object")
        mods = data.get("mods")
        if not isinstance(mods, dict):
            raise TypeError("invalid Nexus configuration: 'mods' must be an object")
        page_ids: dict[str, int] = {}
        for mod_id, mod_data in mods.items():
            if not isinstance(mod_id, str):
                raise TypeError(f"invalid mod ID: {mod_id!r}")
            if not isinstance(mod_data, dict):
                raise TypeError(f"invalid mod data for {mod_id!r}: must be an object")
            page_id = mod_data.get("mod_page_id")
            if isinstance(page_id, bool) or not isinstance(page_id, int):
                raise TypeError(
                    f"invalid 'mod_page_id' for {mod_id!r}: must be a number"
                )
            page_ids[mod_id] = page_id
        return page_ids
    except (OSError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise UpdateError(f"invalid Nexus configuration {path}: {exc}") from exc


def find_descriptions_to_update(
    selected_ids: tuple[str, ...], page_ids: dict[str, int]
) -> tuple[list[str], list[str], list[str]]:
    mod_ids = tuple(selected_ids) if selected_ids else tuple(sorted(page_ids))
    available_ids: list[str] = []
    unknown_ids: list[str] = []
    unavailable_ids: list[str] = []

    for mod_id in mod_ids:
        if mod_id not in page_ids:
            unknown_ids.append(mod_id)
        elif (DESCRIPTION_DIR / f"{mod_id}.txt").is_file():
            available_ids.append(mod_id)
        else:
            unavailable_ids.append(mod_id)

    return available_ids, unknown_ids, unavailable_ids


def update_descriptions(mod_ids: list[str], page_ids: dict[str, int]) -> None:
    with Progress(
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        task = progress.add_task("Preparing descriptions", total=len(mod_ids))
        for mod_id in mod_ids:
            description_path = DESCRIPTION_DIR / f"{mod_id}.txt"
            content = description_path.read_text(encoding="utf-8")
            pyperclip.copy(content)

            progress.update(task, description=f"Ready: {mod_id}")
            url = NEXUS_MOD_URL.format(page_id=page_ids[mod_id])
            if not webbrowser.open_new_tab(url):
                console.print(
                    f"[yellow]Could not open browser tab for {mod_id}[/yellow]"
                )
            click.confirm(
                f"Paste and save {mod_id}, then continue",
                default=True,
                abort=True,
            )
            progress.advance(task)


@click.command()
@click.argument("mod_names", nargs=-1)
def main(mod_names: tuple[str, ...]) -> None:
    try:
        page_ids = load_page_ids(NEXUS_CONFIG)

        available_ids, unknown_ids, unavailable_ids = find_descriptions_to_update(
            mod_names, page_ids
        )
        if unknown_ids:
            console.print(
                f"[yellow]No Nexus page IDs found for mod(s): {', '.join(unknown_ids)}.[/yellow]"
            )
        if unavailable_ids:
            console.print(
                f"[yellow]No descriptions found for mod(s): {', '.join(unavailable_ids)}.[/yellow]"
            )
        if not available_ids:
            console.print("[red]No descriptions available.[/red]")
            return

        update_descriptions(available_ids, page_ids)
    except (OSError, UpdateError) as exc:
        raise click.ClickException(str(exc)) from exc


if __name__ == "__main__":
    main()
