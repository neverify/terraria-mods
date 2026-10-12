# /// script
# requires-python = ">=3.12"
# dependencies = [
#     "click>=8.5.0",
#     "pyperclip>=1.11.0",
#     "rich>=15.0.0",
# ]
# ///

import webbrowser
from collections.abc import Collection

import click
import pyperclip
from common.nexus_config import load_nexus_configs
from common.paths import DESCRIPTION_DIR, NEXUS_CONFIG
from rich.console import Console

NEXUS_MOD_URL = "https://www.nexusmods.com/games/terraria/mods/{page_id}/edit/general"

console = Console()


class UpdateError(Exception):
    """An error preparing the manual description update workflow."""


def load_page_ids() -> dict[str, int]:
    try:
        return {
            mod_id: nexus_mod.mod_page_id
            for mod_id, nexus_mod in load_nexus_configs(NEXUS_CONFIG).items()
        }
    except ValueError as exc:
        raise UpdateError(f"invalid Nexus configuration {NEXUS_CONFIG}: {exc}") from exc


def find_descriptions_to_update(
    mod_names: Collection[str], page_ids: dict[str, int]
) -> tuple[list[str], list[str], list[str]]:
    mod_names = tuple(mod_names) if mod_names else tuple(sorted(page_ids))
    available_names: list[str] = []
    unknown_names: list[str] = []
    unavailable_names: list[str] = []

    for name in mod_names:
        if name not in page_ids:
            unknown_names.append(name)
        elif (DESCRIPTION_DIR / f"{name}.txt").is_file():
            available_names.append(name)
        else:
            unavailable_names.append(name)

    return available_names, unknown_names, unavailable_names


def update_descriptions(mod_names: list[str], page_ids: dict[str, int]) -> None:
    for index, name in enumerate(mod_names, start=1):
        description_path = DESCRIPTION_DIR / f"{name}.txt"
        content = description_path.read_text(encoding="utf-8")
        pyperclip.copy(content)

        url = NEXUS_MOD_URL.format(page_id=page_ids[name])
        if not webbrowser.open_new_tab(url):
            console.print(f"[yellow]Could not open browser tab for {name}[/yellow]")
        click.confirm(
            f"[{index:>{len(str(len(mod_names)))}}/{len(mod_names)}] {name}",
            default=True,
            abort=True,
        )


@click.command()
@click.argument("mod_names", nargs=-1)
def main(mod_names: tuple[str, ...]) -> None:
    try:
        page_ids = load_page_ids()

        available_names, unknown_names, unavailable_names = find_descriptions_to_update(
            mod_names, page_ids
        )
        if unknown_names:
            console.print(
                f"[yellow]No Nexus page IDs found for mod(s): {', '.join(unknown_names)}.[/yellow]"
            )
        if unavailable_names:
            console.print(
                f"[yellow]No descriptions found for mod(s): {', '.join(unavailable_names)}.[/yellow]"
            )
        if not available_names:
            console.print("[red]No descriptions available.[/red]")
            return

        update_descriptions(available_names, page_ids)
    except (OSError, UpdateError) as exc:
        raise click.ClickException(str(exc)) from exc


if __name__ == "__main__":
    main()
