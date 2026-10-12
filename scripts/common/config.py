from dataclasses import dataclass
from pathlib import Path

from .json import load_json


@dataclass(frozen=True)
class NexusMod:
    mod_page_id: int
    mod_id: int
    file_id: int


def load_nexus_config(path: Path) -> dict[str, NexusMod]:
    try:
        data = load_json(path)
        mods = data["mods"]
        if not isinstance(mods, dict):
            raise TypeError("'mods' must be an object")

        result: dict[str, NexusMod] = {}
        for mod_id, mod_data in mods.items():
            if not isinstance(mod_id, str) or not isinstance(mod_data, dict):
                raise TypeError("each mod entry must be an object")
            result[mod_id] = NexusMod(
                mod_page_id=_positive_int(mod_data, "mod_page_id", mod_id),
                mod_id=_positive_int(mod_data, "mod_id", mod_id),
                file_id=_positive_int(mod_data, "file_id", mod_id),
            )
        return result
    except (KeyError, TypeError, ValueError) as exc:
        raise ValueError(f"invalid Nexus configuration {path}: {exc}") from exc


def _positive_int(data: dict, field: str, mod_id: str) -> int:
    value = data.get(field)
    if isinstance(value, bool) or not isinstance(value, int) or value <= 0:
        raise TypeError(f"{field} for {mod_id!r} must be a positive integer")
    return value
