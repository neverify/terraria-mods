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
from dataclasses import dataclass

import click
import pyperclip
from common.mods import Mod, find_mods
from common.nexus_config import NexusConfig, load_nexus_configs
from common.paths import DESCRIPTION_DIR
from rich.console import Console

NEXUS_MOD_URL = "https://www.nexusmods.com/games/terraria/mods/{page_id}/edit/general"

console = Console()


@dataclass(frozen=True)
class DescriptionUpdate:
    mod: Mod
    nexus_config: NexusConfig


def find_descriptions_to_update(
    mods: Collection[Mod], nexus_configs: dict[str, NexusConfig]
) -> tuple[list[DescriptionUpdate], list[Mod], list[Mod]]:
    available: list[DescriptionUpdate] = []
    missing_nexus_configs: list[Mod] = []
    missing_descriptions: list[Mod] = []

    for mod in mods:
        nexus_config = nexus_configs.get(mod.mod_id)
        if nexus_config is None:
            missing_nexus_configs.append(mod)

        description_exists = (DESCRIPTION_DIR / f"{mod.mod_id}.txt").is_file()
        if not description_exists:
            missing_descriptions.append(mod)

        if nexus_config is not None and description_exists:
            available.append(DescriptionUpdate(mod, nexus_config))

    return available, missing_nexus_configs, missing_descriptions


def update_descriptions(updates: list[DescriptionUpdate]) -> None:
    for index, update in enumerate(updates, start=1):
        mod = update.mod
        description_path = DESCRIPTION_DIR / f"{mod.mod_id}.txt"
        content = description_path.read_text(encoding="utf-8")
        try:
            pyperclip.copy(content)
        except pyperclip.PyperclipException as exc:
            raise OSError(f"unable to copy description for {mod.name}") from exc

        url = NEXUS_MOD_URL.format(page_id=update.nexus_config.mod_page_id)
        try:
            opened = webbrowser.open_new_tab(url)
        except webbrowser.Error as exc:
            raise OSError(f"unable to open Nexus page for {mod.name}") from exc
        if not opened:
            console.print(f"[yellow]Could not open browser tab for {mod.name}[/yellow]")
        click.confirm(
            f"[{index:>{len(str(len(updates)))}}/{len(updates)}] {mod.name}",
            default=True,
            abort=True,
        )


@click.command()
@click.argument("mod_names", nargs=-1)
def main(mod_names: tuple[str, ...]) -> None:
    try:
        mods = find_mods(mod_names)
        nexus_configs = load_nexus_configs()
        available, missing_nexus_configs, missing_descriptions = (
            find_descriptions_to_update(mods, nexus_configs)
        )

        if missing_nexus_configs:
            console.print(
                f"[yellow]No Nexus configurations found for mod(s): {', '.join(mod.name for mod in missing_nexus_configs)}.[/yellow]"
            )
        if missing_descriptions:
            console.print(
                f"[yellow]No descriptions found for mod(s): {', '.join(mod.name for mod in missing_descriptions)}.[/yellow]"
            )
        if not available:
            console.print("[red]No descriptions available.[/red]")
            return

        update_descriptions(available)
    except (OSError, TypeError, ValueError) as exc:
        raise click.ClickException(str(exc)) from exc


if __name__ == "__main__":
    main()
