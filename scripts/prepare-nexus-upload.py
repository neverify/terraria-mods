import argparse
import os
import re
import sys
import uuid
from pathlib import Path
from typing import TextIO

from common.config import load_nexus_config
from common.json import load_json
from common.mods import find_mod_directory
from common.mods import load_manifest as read_manifest
from common.paths import NEXUS_CONFIG as CONFIG
from common.versions import parse_tag

HEADING_RE = re.compile(r"^###\s+(?P<category>\S.*?)\s*$")
BULLET_RE = re.compile(r"^\s*[-*+]\s+(?P<entry>\S.*?)\s*$")


class NexusUploadError(Exception):
    """An invalid release or Nexus configuration."""


def load_manifest(mod_id: str) -> dict:
    return read_manifest(find_mod_directory(mod_id) / "manifest.json")


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

    try:
        mod_id, version = parse_tag(tag)
    except ValueError:
        raise NexusUploadError(
            f"release tag {tag!r} does not match '<mod-id>-v<version>'"
        )
    manifest = load_manifest(mod_id)

    config = load_nexus_config(CONFIG)
    if mod_id not in config:
        raise NexusUploadError(f"no Nexus configuration found for {mod_id!r}")
    nexus_ids = config[mod_id]
    nexus_mod_id = nexus_ids.mod_id
    nexus_file_id = nexus_ids.file_id

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
