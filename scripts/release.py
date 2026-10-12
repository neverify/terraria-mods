# /// script
# requires-python = ">=3.11"
# dependencies = [
#     "click>=8.5.0",
#     "rich",
# ]
# ///

import re
import shlex
import subprocess
import time
import zipfile
from collections.abc import Callable, Iterable, Sequence
from dataclasses import dataclass
from pathlib import Path
from typing import Any

import click
from common.mods import Mod as SourceMod
from common.mods import find_mods
from common.paths import BUILD_DIR, ROOT, ZIPS_DIR
from common.versions import parse_tag, parse_version
from rich.console import Console
from rich.progress import BarColumn, MofNCompleteColumn, Progress, TextColumn
from rich.prompt import Confirm
from rich.table import Table

CHANGELOG_RE = re.compile(r"^##\s+\[(?P<version>[^\]]+)\](?:\s+-.*)?\s*$", re.MULTILINE)

console = Console()


@dataclass(frozen=True)
class Mod(SourceMod):
    releases: list[str] | None
    release_notes: str | None = None

    @property
    def last_release(self) -> str | None:
        return self.releases[-1] if self.releases else None

    @property
    def updated(self) -> bool:
        return not self.last_release or parse_version(self.version) > parse_version(
            self.last_release
        )

    @property
    def tag(self) -> str:
        return f"{self.mod_id}-v{self.version}"


def fail(message: str) -> None:
    console.print(f"{message}")
    raise SystemExit(1)


def succeed(message: str) -> None:
    console.print(f"{message}")
    raise SystemExit(0)


def run(command: list[str]) -> subprocess.CompletedProcess[str]:
    try:
        return subprocess.run(
            command,
            cwd=ROOT,
            text=True,
            capture_output=True,
            check=True,
        )
    except (OSError, subprocess.CalledProcessError) as exc:
        details = getattr(exc, "stderr", None) or str(exc)
        raise RuntimeError(
            f"command failed ({shlex.join(command)}): {details.strip()}"
        ) from exc


def extract_changelog(changelog: Path, version: str) -> str:
    text = changelog.read_text(encoding="utf-8")
    matches = list(CHANGELOG_RE.finditer(text))

    for index, match in enumerate(matches):
        if match.group("version") != version:
            continue

        start = match.end()
        end = matches[index + 1].start() if index + 1 < len(matches) else len(text)
        body = text[start:end].strip()
        if not body:
            raise ValueError(f"changelog section [{version}] is empty in {changelog}")
        return body

    raise ValueError(f"changelog section [{version}] not found in {changelog}")


def load_mod(source_mod: SourceMod) -> Mod:
    path = source_mod.directory
    manifest = source_mod.manifest
    try:
        release_notes = extract_changelog(path / "CHANGELOG.md", source_mod.version)
    except (OSError, ValueError):
        release_notes = None

    releases = []
    for tag in run(
        ["git", "tag", "--list", f"{source_mod.mod_id}-v*"]
    ).stdout.splitlines():
        try:
            releases.append(parse_tag(tag)[1])
        except ValueError:
            continue

    return Mod(path, manifest, releases, release_notes)


def load_mods(selected: Iterable[str] | None) -> list[Mod]:
    console.print("Fetching remote tags...", end=" ")
    run(["git", "fetch", "--all"])
    console.print("[green]done[/green]")

    source_mods = find_mods(selected)
    mods = []

    with Progress(
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        task = progress.add_task("Loading mods", total=len(source_mods))
        for source_mod in source_mods:
            mod = load_mod(source_mod)
            progress.advance(task)
            mods.append(mod)
    return mods


def show_status(mods: list[Mod]) -> None:
    table = Table(title="Release Status")
    table.add_column("Mod")
    table.add_column("Latest Release")
    table.add_column("Current Version")
    table.add_column("Release Notes")
    table.add_column("Release")
    for mod in mods:
        ready = mod.updated and mod.release_notes is not None
        table.add_row(
            mod.name,
            mod.last_release or "[dim]N/A[/dim]",
            f"[green]{mod.version}[/green]"
            if mod.updated
            else f"[yellow]{mod.version}[/yellow]",
            "[green]yes[/green]" if mod.release_notes is not None else "[red]no[/red]",
            "[green]ready[/green]" if ready else "[dim]skipped[/dim]",
        )
    console.print(table)


def build_mod(mod: Mod) -> None:
    run(["dotnet", "build", str(mod.directory), "-c", "Release"])


def create_zip(mod: Mod) -> Path:
    source = BUILD_DIR / mod.mod_id
    destination = ZIPS_DIR / f"{mod.mod_id}-{mod.version}.zip"
    if not source.is_dir():
        raise ValueError(f"build output does not exist: {source}")
    ZIPS_DIR.mkdir(exist_ok=True)
    if destination.exists():
        destination.unlink()

    with zipfile.ZipFile(destination, "w", zipfile.ZIP_DEFLATED) as archive:
        for path in source.rglob("*"):
            if path.is_file():
                archive.write(path, path.relative_to(BUILD_DIR))
    return destination


@dataclass(frozen=True)
class Step:
    description: str
    action: Callable[[dict[str, Any]], Any]
    result: str | None = None


def run_steps(steps: Sequence[Step], delay: float = 0.0) -> dict[str, Any]:
    if not steps:
        raise ValueError("at least one progress step is required")

    results: dict[str, Any] = {}
    progress = Progress(
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    )

    with progress:
        task = progress.add_task(steps[0].description, total=len(steps))
        for step in steps:
            progress.update(task, description=step.description)
            result = step.action(results)
            if step.result is not None:
                results[step.result] = result
            progress.advance(task)
            if delay > 0:
                time.sleep(delay)

    return results


def check_tag(mod: Mod) -> None:
    existing = run(["git", "tag", "--list", mod.tag]).stdout.strip()
    if existing:
        raise ValueError(f"tag already exists: {mod.tag}")


def create_tag(mod: Mod) -> None:
    run(["git", "tag", "-a", mod.tag, "-m", f"{mod.name} {mod.version}"])


def push_tag(mod: Mod) -> None:
    run(["git", "push", "origin", mod.tag])


def create_github_release(mod: Mod, archive: Path, publish: bool) -> None:
    command = [
        "gh",
        "release",
        "create",
        mod.tag,
        str(archive),
        "--verify-tag",
        "--title",
        f"{mod.name} {mod.version}",
        "--notes",
        mod.release_notes or "",
    ]
    if not publish:
        command.append("--draft")
    run(command)


def release_mod(mod: Mod, publish: bool, slow: bool = False) -> None:
    steps = [
        Step(
            f"Building {mod.name}",
            lambda _: build_mod(mod),
        ),
        Step(
            f"Creating archive for {mod.name}",
            lambda _: create_zip(mod),
            result="archive",
        ),
        Step(
            f"Checking tag {mod.tag}",
            lambda _: check_tag(mod),
        ),
        Step(
            f"Creating tag {mod.tag}",
            lambda _: create_tag(mod),
        ),
        Step(
            f"Pushing tag {mod.tag}",
            lambda _: push_tag(mod),
        ),
        Step(
            f"Creating GitHub release for {mod.name}",
            lambda results: create_github_release(
                mod,
                results["archive"],
                publish,
            ),
        ),
    ]

    run_steps(steps, delay=1.0 if slow else 0.0)


@click.command()
@click.argument("mod_names", nargs=-1)
@click.option(
    "--dry-run",
    is_flag=True,
    help="Show release status without making changes.",
)
@click.option(
    "--publish",
    is_flag=True,
    help="Publish releases instead of creating drafts.",
)
@click.option(
    "--yes",
    "skip_confirmation",
    is_flag=True,
    help="Skip the release confirmation prompt.",
)
@click.option(
    "--slow",
    "add_delay",
    is_flag=True,
    help="Add a fixed 1s delay between operations.",
)
def main(
    mod_names: tuple[str, ...],
    dry_run: bool,
    publish: bool,
    skip_confirmation: bool,
    add_delay: bool,
) -> None:
    # Check if the working tree is clean
    status = run(["git", "status", "--porcelain"]).stdout.strip()
    if status and not dry_run:
        fail(
            "[yellow]Working tree is not clean; commit changes before releasing or use --dry-run to view release status.[/yellow]"
        )

    # Load status of mods
    try:
        mods = load_mods(mod_names)
    except RuntimeError as exc:
        fail(f"[red]Unable to load release status: [/red]{exc}")

    # Show status of mods
    show_status(mods)

    candidates = [mod for mod in mods if mod.updated and mod.release_notes]
    if not candidates:
        succeed("[yellow]No mods ready for release.[/yellow]")
    console.print(f"[green]{len(candidates)} mod(s) ready for release.[/green]")

    if dry_run:
        return

    # Confirm release
    if not skip_confirmation and not Confirm.ask(f"Release {len(candidates)} mod(s)?"):
        console.print("[yellow]Release cancelled.[/yellow]")
        return

    # Release mods
    success_count = 0
    fail_count = 0
    for mod in candidates:
        if add_delay:
            time.sleep(1)
        try:
            release_mod(mod, publish, add_delay)
            success_count += 1
        except ValueError as exc:
            console.print(f"[red]Failed to release {mod.name}: [/red]{exc}")
            fail_count += 1

    console.print(f"Finished: {success_count} successful, {fail_count} failed.")


if __name__ == "__main__":
    main()
