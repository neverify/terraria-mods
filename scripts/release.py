# /// script
# requires-python = ">=3.11"
# dependencies = [
#     "rich",
# ]
# ///

import argparse
import json
import re
import subprocess
import zipfile
from collections.abc import Callable, Sequence
from dataclasses import dataclass
from pathlib import Path
from typing import Any

from rich.console import Console
from rich.progress import BarColumn, MofNCompleteColumn, Progress, TextColumn
from rich.prompt import Confirm
from rich.table import Table

ROOT = Path(__file__).resolve().parent.parent
SRC_DIR = ROOT / "src"
BUILD_DIR = ROOT / "build"
ZIPS_DIR = ROOT / "zips"
TAG_RE = re.compile(r"^(?P<mod>.+)-v(?P<version>\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?)$")
CHANGELOG_RE = re.compile(r"^##\s+\[(?P<version>[^\]]+)\](?:\s+-.*)?\s*$", re.MULTILINE)
VERSION_RE = re.compile(r"^(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?$")
console = Console()


class ModReleaseError(Exception):
    """An error that should skip one mod without stopping the release run."""


@dataclass
class Mod:
    directory: Path
    manifest: dict
    updated: bool = False
    release_notes: str | None = None

    @property
    def mod_id(self) -> str:
        return self.manifest["id"]

    @property
    def version(self) -> str:
        return self.manifest["version"]

    @property
    def tag(self) -> str:
        return f"{self.mod_id}-v{self.version}"

    @property
    def name(self) -> str:
        return self.manifest["name"]


def fail(message: str) -> None:
    console.print(f"[red]Error:[/red] {message}")
    raise SystemExit(1)


def run(command: list[str]) -> subprocess.CompletedProcess[str]:
    return subprocess.run(command, cwd=ROOT, text=True, capture_output=True, check=True)


def parse_version(version: str) -> tuple[int, int, int, str]:
    match = VERSION_RE.fullmatch(version)
    if not match:
        raise ValueError(f"invalid semantic version: {version}")
    return (
        int(match.group(1)),
        int(match.group(2)),
        int(match.group(3)),
        match.group(4) or "",
    )


def parse_tag(tag: str) -> tuple[str, str]:
    match = TAG_RE.fullmatch(tag)
    if not match:
        raise ValueError(f"Invalid tag: {tag}")
    return match.group("mod"), match.group("version")


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


def load_manifest(manifest_file: Path) -> dict:
    try:
        manifest = json.loads(manifest_file.read_text(encoding="utf-8"))
        mod_id = manifest["id"]
        name = manifest["name"]
        version = manifest["version"]
        parse_version(version)
        if not all(
            isinstance(value, str) and value.strip()
            for value in (mod_id, name, version)
        ):
            raise ValueError("id, name, and version must be non-empty strings")
    except (OSError, KeyError, TypeError, ValueError, json.JSONDecodeError) as exc:
        raise ValueError(f"invalid manifest {manifest_file}: {exc}") from exc
    return manifest


def load_mod(path: Path) -> Mod | None:
    manifest_file = path / "manifest.json"
    if not manifest_file.is_file():
        return None

    try:
        manifest = load_manifest(manifest_file)
    except ValueError as exc:
        console.print(f"[red]Error:[/red] {exc}")
        return None

    mod_id = manifest["id"]
    version = manifest["version"]
    try:
        release_notes = extract_changelog(path / "CHANGELOG.md", version)
    except (OSError, ValueError):
        release_notes = None

    previous_versions = []
    for tag in run(["git", "tag", "--list", f"{mod_id}-v*"]).stdout.splitlines():
        try:
            previous_versions.append(parse_tag(tag)[1])
        except ValueError:
            continue

    updated = not previous_versions or version != max(
        previous_versions, key=parse_version
    )
    return Mod(path, manifest, updated, release_notes)


def load_mods(selected: set[str] | None) -> list[Mod]:
    mods = []
    directories = sorted(path for path in SRC_DIR.iterdir() if path.is_dir())

    with Progress(
        TextColumn("[progress.description]{task.description}"),
        BarColumn(),
        MofNCompleteColumn(),
        console=console,
    ) as progress:
        task = progress.add_task("Loading mods", total=len(directories))
        for directory in directories:
            mod = load_mod(directory)
            progress.advance(task)
            if mod is None:
                continue
            if (
                selected
                and mod.mod_id.casefold() not in selected
                and directory.name.casefold() not in selected
            ):
                continue
            mods.append(mod)

    if selected and not mods:
        fail(f"no matching mods found: {', '.join(sorted(selected))}")
    return mods


def show_status(mods: list[Mod]) -> None:
    table = Table(title="Release Status")
    table.add_column("Mod")
    table.add_column("Version")
    table.add_column("Updated")
    table.add_column("Release Notes")
    table.add_column("Release")
    for mod in mods:
        ready = mod.updated and mod.release_notes is not None
        table.add_row(
            mod.name,
            mod.version,
            "[green]yes[/green]" if mod.updated else "[yellow]no[/yellow]",
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


def run_steps(steps: Sequence[Step]) -> dict[str, Any]:
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


def release_mod(mod: Mod, publish: bool) -> None:
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

    run_steps(steps)


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Build and publish Terraria mod releases."
    )
    parser.add_argument(
        "--mods", action="append", help="space-separated list of mod names or IDs"
    )
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="show release status without making changes",
    )
    parser.add_argument(
        "--publish",
        action="store_true",
        help="publish releases instead of creating drafts",
    )
    parser.add_argument(
        "--yes", action="store_true", help="skip the release confirmation prompt"
    )
    args = parser.parse_args()

    # Check if the working tree is clean
    status = run(["git", "status", "--porcelain"]).stdout.strip()
    if status and not args.dry_run:
        fail("working tree is not clean; commit changes before releasing")

    # Determine status of mods
    selected = {value.casefold() for value in args.mods} if args.mods else None
    mods = load_mods(selected)

    # Show status of mods
    show_status(mods)
    if args.dry_run:
        return

    # Confirm release
    candidates = [mod for mod in mods if mod.updated and mod.release_notes]
    if not candidates:
        fail("no mods are ready for release")
    if not args.yes and not Confirm.ask(f"Release {len(candidates)} mod(s)?"):
        console.print("Release cancelled.")
        return

    # Release mods
    for mod in candidates:
        try:
            release_mod(mod, args.publish)
        except ValueError as exc:
            console.print(f"[red]Error:[/red] {mod.name}: {exc}")


if __name__ == "__main__":
    main()
