from collections.abc import Iterable
from dataclasses import dataclass
from pathlib import Path
from typing import Any

from .json import JsonLoadError, load_json
from .paths import SRC_DIR


@dataclass(frozen=True)
class Mod:
    directory: Path
    manifest: dict[str, Any]

    @property
    def mod_id(self) -> str:
        return self.manifest["id"]

    @property
    def normalized_id(self) -> str:
        return self.mod_id.replace("-", "").casefold()

    @property
    def name(self) -> str:
        return self.manifest["name"]

    @property
    def internal_name(self) -> str:
        return str(self.manifest["name"]).replace(" ", "")

    @property
    def version(self) -> str:
        return self.manifest["version"]


def load_manifest(path: Path) -> dict[str, Any]:
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


def find_mod_directory(mod_id: str) -> Path:
    for directory in SRC_DIR.iterdir():
        if directory.is_dir() and _normalized(directory.name) == _normalized(mod_id):
            return directory
    raise ValueError(f"no {mod_id!r} found in {SRC_DIR}")


def find_mods(selected: Iterable[str] | None = None) -> list[Mod]:
    if selected:
        selected = tuple(_normalized(mod_id) for mod_id in selected)
    mods: list[Mod] = []
    for directory in sorted(path for path in SRC_DIR.iterdir() if path.is_dir()):
        if selected and not (_normalized(directory.name) in selected):
            continue
        manifest_path = directory / "manifest.json"
        if not manifest_path.is_file():
            continue
        try:
            manifest = load_manifest(manifest_path)
        except ValueError:
            if selected:
                raise
            continue

        mods.append(Mod(directory, manifest))
    if selected and not mods:
        raise ValueError(f"no matching mods found: {', '.join(sorted(selected))}")
    return mods


def _normalized(mod_id: str) -> str:
    return mod_id.replace(" ", "").replace("-", "").casefold()
