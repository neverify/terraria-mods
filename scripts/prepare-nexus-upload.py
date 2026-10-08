import argparse
import json
import os
import re
import sys
import uuid
from pathlib import Path
from typing import TextIO

ROOT = Path(__file__).resolve().parent.parent
CONFIG = Path(ROOT / "nexus.json")
TAG_RE = re.compile(r"^(?P<mod_id>.+)-v(?P<version>\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?)$")
HEADING_RE = re.compile(r"^###\s+(?P<category>\S.*?)\s*$")
BULLET_RE = re.compile(r"^\s*[-*+]\s+(?P<entry>\S.*?)\s*$")


class NexusUploadError(Exception):
    """An invalid release or Nexus configuration."""


def load_json(path: Path) -> dict:
    try:
        value = json.loads(path.read_text(encoding="utf-8"))
    except OSError as exc:
        raise ValueError(f"Unable to read {path}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise ValueError(f"Invalid JSON in {path}: {exc}") from exc
    return value


def load_manifest(mod_id: str) -> dict:
    try:
        return next(
            load_json(mod_folder / "manifest.json")
            for mod_folder in (ROOT / "src").glob("*")
            if mod_folder.is_dir()
            and mod_folder.name.casefold() == mod_id.replace("-", "").casefold()
        )
    except StopIteration:
        raise ValueError(f"no manifest found for release mod ID {mod_id!r}")


def format_changelog(body: str) -> str:
    entries: list[str] = []
    category: str | None = None

    def add_entry(entry: str) -> None:
        if category is None:
            raise ValueError(
                "release description contains a changelog entry before a category heading"
            )
        entries.append(f"{category}: {entry}")

    for line in body.splitlines():
        heading = HEADING_RE.match(line)
        if heading:
            category = heading.group("category").strip()
            continue

        bullet = BULLET_RE.match(line)
        if bullet:
            add_entry(bullet.group("entry").strip())
            continue

        if not line.strip():
            continue

        raise ValueError(f"unsupported line in release description: {line!r}")

    if not entries:
        raise ValueError("release description contains no changelog entries")
    return "\n".join(entries)


def write_output(output: TextIO, name: str, value: str) -> None:
    delimiter = f"nexus-output-{uuid.uuid4()}"
    output.write(f"{name}<<{delimiter}\n{value}\n{delimiter}\n")


def prepare(event_path: Path, asset_dir: Path, output: TextIO) -> None:
    event = load_json(event_path)
    release = event.get("release")
    if not isinstance(release, dict):
        raise NexusUploadError("GitHub event does not contain a release")

    tag = release.get("tag_name")
    body = release.get("body")
    if not isinstance(tag, str) or not tag:
        raise NexusUploadError("release tag is missing")
    if not isinstance(body, str) or not body.strip():
        raise NexusUploadError("release description is empty")

    changelog = format_changelog(body)

    tag_match = TAG_RE.fullmatch(tag)
    if tag_match is None:
        raise NexusUploadError(
            f"release tag {tag!r} does not match '<mod-id>-v<version>'"
        )
    mod_id = tag_match.group("mod_id")
    version = tag_match.group("version")
    manifest = load_manifest(mod_id)

    config = load_json(CONFIG).get("mods")
    if not isinstance(config, dict) or mod_id not in config:
        raise NexusUploadError(f"no Nexus configuration found for {mod_id!r}")
    nexus_ids = config[mod_id]
    if not isinstance(nexus_ids, dict):
        raise NexusUploadError(f"Nexus configuration for {mod_id!r} must be an object")

    nexus_mod_id = nexus_ids.get("mod_id")
    nexus_file_id = nexus_ids.get("file_id")
    if not isinstance(nexus_mod_id, int) or nexus_mod_id <= 0:
        raise NexusUploadError(
            f"Nexus mod_id for {mod_id!r} must be a positive integer"
        )
    if not isinstance(nexus_file_id, int) or nexus_file_id <= 0:
        raise NexusUploadError(
            f"Nexus file_id for {mod_id!r} must be a positive integer"
        )

    assets = list(asset_dir.glob("*.zip"))
    if len(assets) != 1:
        raise NexusUploadError(
            f"expected exactly one ZIP in {asset_dir}, found {len(assets)}"
        )

    write_output(output, "file_id", str(nexus_file_id))
    write_output(output, "mod_id", str(nexus_mod_id))
    write_output(output, "version", version)
    write_output(output, "display_name", str(manifest["name"]))
    write_output(output, "changelog", changelog)
    write_output(output, "filename", str(assets[0]))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--asset-dir", type=Path, required=True)
    args = parser.parse_args()

    event = Path(os.environ["GITHUB_EVENT_PATH"])
    output = Path(os.environ["GITHUB_OUTPUT"])

    try:
        with output.open("a", encoding="utf-8", newline="") as output:
            prepare(event, args.asset_dir, output)
    except (NexusUploadError, KeyError, OSError, TypeError, ValueError) as exc:
        print(f"Error: {exc}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
