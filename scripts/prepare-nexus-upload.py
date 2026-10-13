import argparse
import os
import re
import sys
import uuid
from pathlib import Path
from typing import TextIO

from common.json import JsonLoadError, load_json
from common.mods import find_mods
from common.nexus_config import load_nexus_configs
from common.versions import parse_tag

HEADING_RE = re.compile(r"^###\s+(?P<category>\S.*?)\s*$")
BULLET_RE = re.compile(r"^\s*[-*+]\s+(?P<entry>\S.*?)\s*$")


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
    try:
        event = load_json(event_path)
    except JsonLoadError as exc:
        raise ValueError(f"invalid GitHub event {event_path}: {exc}") from exc

    if not isinstance(event, dict):
        raise TypeError("GitHub event must contain an object")

    release = event.get("release")
    if not isinstance(release, dict):
        raise TypeError("GitHub event does not contain a release object")

    tag = release.get("tag_name")
    body = release.get("body")
    if not isinstance(tag, str) or not tag:
        raise ValueError("release tag is missing")
    if not isinstance(body, str) or not body.strip():
        raise ValueError("release description is empty")

    changelog = format_changelog(body)

    try:
        mod_id, version = parse_tag(tag)
    except ValueError as exc:
        raise ValueError(
            f"release tag {tag!r} does not match '<mod-id>-v<version>'"
        ) from exc
    try:
        mod = find_mods(mod_id)[0]
    except (IndexError, ValueError) as exc:
        raise ValueError(f"no mod found for ID {mod_id!r}") from exc

    configs = load_nexus_configs()
    if mod_id not in configs:
        raise ValueError(f"no Nexus configuration found for {mod_id!r}")
    nexus_ids = configs[mod_id]
    nexus_mod_id = nexus_ids.mod_id
    nexus_file_id = nexus_ids.file_id

    assets = list(asset_dir.glob("*.zip"))
    if len(assets) != 1:
        raise ValueError(
            f"expected exactly one ZIP in {asset_dir}, found {len(assets)}"
        )

    write_output(output, "file_id", str(nexus_file_id))
    write_output(output, "mod_id", str(nexus_mod_id))
    write_output(output, "version", str(version))
    write_output(output, "display_name", mod.name)
    write_output(output, "changelog", changelog)
    write_output(output, "filename", str(assets[0]))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--asset-dir", type=Path, required=True)
    args = parser.parse_args()

    try:
        event_path = os.environ["GITHUB_EVENT_PATH"]
        output_path = os.environ["GITHUB_OUTPUT"]
        with Path(output_path).open("a", encoding="utf-8", newline="") as output:
            prepare(Path(event_path), args.asset_dir, output)
    except KeyError as exc:
        print(
            f"Error: missing GitHub environment variable {exc.args[0]}", file=sys.stderr
        )
        return 1
    except (OSError, ValueError) as exc:
        print(f"Error: {exc}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
