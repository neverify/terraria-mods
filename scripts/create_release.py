import json
import re
import sys
import os
from pathlib import Path

TAG_RE = re.compile(r"^(?P<mod>.+)-v(?P<version>\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?)$")
CHANGELOG_RE = re.compile(
    r"^##\s+\[(?P<version>[^\]]+)\](?:\s+-.*)?\s*$",
    re.MULTILINE,
)


def fail(message: str) -> None:
    print(f"error: {message}", file=sys.stderr)
    sys.exit(1)


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
            raise ValueError(f"changelog section [{version}] is empty: {changelog}")

        return body

    raise ValueError(f"changelog section [{version}] not found: {changelog}")


def main() -> None:
    if len(sys.argv) != 2:
        fail("usage: create_release.py <tag>")

    tag = sys.argv[1]

    try:
        mod, version = parse_tag(tag)
    except ValueError as exc:
        fail(f"Failed to parse tag: {exc}")

    mod_dir = Path("src") / mod
    manifest_file = mod_dir / "manifest.json"
    changelog_file = mod_dir / "CHANGELOG.md"

    if not mod_dir.is_dir():
        fail(f"mod directory does not exist: {mod_dir}")

    if not manifest_file.is_file():
        fail(f"manifest.json does not exist: {manifest_file}")

    if not changelog_file.is_file():
        fail(f"changelog does not exist: {changelog_file}")

    try:
        metadata = json.loads(manifest_file.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        fail(f"invalid JSON in {manifest_file}: {exc}")

    title = metadata.get("name")
    if not isinstance(title, str) or not title.strip():
        fail(f"{manifest_file} does not contain a non-empty 'name'")

    try:
        notes = extract_changelog(changelog_file, version)
    except ValueError as exc:
        fail(f"Failed to extract changelog: {exc}")

    release_notes_file = Path("release-notes.md")
    release_notes_file.write_text(notes + "\n", encoding="utf-8")

    output_file = Path(os.environ["GITHUB_OUTPUT"])

    with output_file.open("a", encoding="utf-8") as output:
        # output.write(f"mod={mod}\n")
        # output.write(f"version={version}\n")
        output.write(f"title={title}\n")


if __name__ == "__main__":
    main()
