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


def load_nexus_ids(path: Path) -> dict[str, int]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
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
            if not isinstance(page_id, int):
                raise TypeError(
                    f"invalid 'mod_page_id' for {mod_id!r}: must be a number"
                )
            page_ids[mod_id] = page_id
        return page_ids
    except (OSError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise UpdateError(f"invalid Nexus configuration {path}: {exc}") from exc


def resolve_mod_ids(selected: tuple[str, ...], configured_ids: set[str]) -> list[str]:
    if not selected:
        return sorted(configured_ids)

    configured_by_name = {mod_id.casefold(): mod_id for mod_id in configured_ids}
    mod_ids: list[str] = []
    unknown: list[str] = []
    for value in selected:
        mod_id = configured_by_name.get(value.casefold())
        if mod_id is None:
            unknown.append(value)
        else:
            mod_ids.append(mod_id)
    if unknown:
        raise UpdateError(
            f"no Nexus configuration found for mod(s): {', '.join(unknown)}"
        )
    return mod_ids


def load_available_descriptions(mod_ids: list[str]) -> list[str]:
    available: list[str] = []
    for mod_id in mod_ids:
        description_path = DESCRIPTION_DIR / f"{mod_id}.txt"
        if description_path.is_file():
            available.append(mod_id)
        else:
            console.print(
                f"[yellow]Skipping {mod_id}: description not found at "
                f"{description_path}[/yellow]"
            )

    if not available:
        raise UpdateError(f"no descriptions available in {DESCRIPTION_DIR}")
    return available


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
        page_ids = load_nexus_ids(NEXUS_CONFIG)
        selected_ids = resolve_mod_ids(mod_names, set(page_ids))
        available_ids = load_available_descriptions(selected_ids)
        update_descriptions(available_ids, page_ids)
    except (OSError, UpdateError) as exc:
        raise click.ClickException(str(exc)) from exc


if __name__ == "__main__":
    main()
