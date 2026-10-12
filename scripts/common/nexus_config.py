from dataclasses import dataclass
from pathlib import Path

from .json import JsonLoadError, load_json


@dataclass(frozen=True)
class NexusConfig:
    mod_page_id: int
    mod_id: int
    file_id: int


def load_nexus_configs(path: Path) -> dict[str, NexusConfig]:
    try:
        data = load_json(path)
    except JsonLoadError as exc:
        raise ValueError(f"invalid Nexus configuration {path}: {exc}") from exc
    try:
        mods = data["mods"]
        if not isinstance(mods, dict):
            raise TypeError("'mods' must be an object")

        configs: dict[str, NexusConfig] = {}
        for mod_id, mod_data in mods.items():
            if not isinstance(mod_id, str) or not isinstance(mod_data, dict):
                raise TypeError("each mod entry must be an object")
            try:
                configs[mod_id] = NexusConfig(
                    mod_page_id=_positive_int(mod_data, "mod_page_id"),
                    mod_id=_positive_int(mod_data, "mod_id"),
                    file_id=_positive_int(mod_data, "file_id"),
                )
            except TypeError as exc:
                raise TypeError(f"invalid data for mod {mod_id}: {exc}") from exc
        return configs
    except (KeyError, TypeError, ValueError) as exc:
        raise ValueError(f"invalid Nexus configuration {path}: {exc}") from exc


def _positive_int(data: dict, field: str) -> int:
    value = data.get(field)
    if isinstance(value, bool) or not isinstance(value, int) or value <= 0:
        raise TypeError(f"{field} must be a positive integer")
    return value
