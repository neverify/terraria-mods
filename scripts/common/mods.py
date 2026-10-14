from collections.abc import Iterable
from dataclasses import dataclass
from pathlib import Path
from typing import Any

from .json import JsonLoadError, load_json
from .paths import SRC_DIR
from .versions import Version, parse_version


@dataclass(frozen=True)
class Mod:
    directory: Path
    mod_id: str
    name: str
    version: Version


def find_mods(mod_names: Iterable[str] | None = None) -> list[Mod]:
    normalized_mod_names = (
        tuple(_normalized(mod_id) for mod_id in mod_names)
        if mod_names is not None
        else None
    )
    mods: list[Mod] = []
    for directory in sorted(path for path in SRC_DIR.iterdir() if path.is_dir()):
        if normalized_mod_names and not (
            _normalized(directory.name) in normalized_mod_names
        ):
            continue
        mod = _load_mod(directory, strict=bool(mod_names))
        if mod is not None:
            mods.append(mod)
    if mod_names and not mods:
        raise ValueError(f"no matching mods found: {', '.join(sorted(mod_names))}")
    return mods


def find_mod(mod_name: str) -> Mod:
    mods = find_mods((mod_name,))
    if len(mods) != 1:
        raise ValueError(f"expected exactly one mod for {mod_name!r}")
    return mods[0]


def _load_mod(directory: Path, strict: bool) -> Mod | None:
    manifest_path = directory / "manifest.json"
    if not manifest_path.is_file():
        return None
    try:
        manifest = _load_manifest(manifest_path)
        version = parse_version(manifest["version"])
    except (ValueError, TypeError):
        if strict:
            raise
        return None

    return Mod(
        directory,
        manifest["id"],
        manifest["name"],
        version,
    )


def _load_manifest(path: Path) -> dict[str, Any]:
    try:
        manifest = load_json(path)
    except JsonLoadError as exc:
        raise ValueError(f"invalid manifest {path}: {exc}") from exc
    if not isinstance(manifest, dict):
        raise TypeError(f"manifest {path} must contain an object")

    required = ("id", "name", "version")
    if not all(
        isinstance(manifest.get(field), str) and manifest[field].strip()
        for field in required
    ):
        raise ValueError(
            f"manifest {path} must contain non-empty id, name, and version"
        )
    return manifest


def _normalized(mod_id: str) -> str:
    return mod_id.replace(" ", "").replace("-", "").casefold()
