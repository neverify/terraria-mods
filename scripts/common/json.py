import json
from pathlib import Path
from typing import Any


class JsonLoadError(Exception):
    """Raised when the loaded file can't be accessed or contains invalid JSON."""


def load_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except (OSError, UnicodeError) as exc:
        raise JsonLoadError(f"unable to read {path}: {exc}") from exc
    except json.JSONDecodeError as exc:
        raise JsonLoadError(f"invalid JSON in {path}: {exc}") from exc
