import re
from dataclasses import dataclass
from enum import IntEnum
from functools import total_ordering

VERSION_RE = re.compile(r"^(\d+)\.(\d+)\.(\d+)(?:-(alpha|beta|rc)\.(\d+))?$")
TAG_RE = re.compile(
    r"^(?P<mod_id>.+)-v(?P<version>\d+\.\d+\.\d+(?:-(?:alpha|beta|rc)\.\d+)?)$"
)


class PreReleaseType(IntEnum):
    ALPHA = 0
    BETA = 1
    RC = 2


@dataclass(frozen=True, order=True)
class PreRelease:
    type: PreReleaseType
    number: int

    def __str__(self) -> str:
        return f"{self.type.name.lower()}.{self.number}"


@total_ordering
@dataclass(frozen=True)
class Version:
    major: int
    minor: int
    patch: int
    prerelease: PreRelease | None = None

    def __str__(self) -> str:
        suffix = f"-{self.prerelease}" if self.prerelease else ""
        return f"{self.major}.{self.minor}.{self.patch}{suffix}"

    def __lt__(self, other: object) -> bool:
        if not isinstance(other, Version):
            return NotImplemented
        base = (self.major, self.minor, self.patch)
        other_base = (other.major, other.minor, other.patch)
        if base != other_base:
            return base < other_base
        if not self.prerelease or not other.prerelease:
            return bool(self.prerelease) and not bool(other.prerelease)
        return self.prerelease < other.prerelease

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Version):
            return NotImplemented
        return (
            self.major,
            self.minor,
            self.patch,
            self.prerelease,
        ) == (
            other.major,
            other.minor,
            other.patch,
            other.prerelease,
        )


def format_tag(mod_id: str, version: Version) -> str:
    return f"{mod_id}-v{version}"


def parse_tag(tag: str) -> tuple[str, Version]:
    match = TAG_RE.fullmatch(tag)
    if not match:
        raise ValueError(f"invalid release tag: {tag}")
    return match.group("mod_id"), parse_version(match.group("version"))


def parse_version(version: str) -> Version:
    match = VERSION_RE.fullmatch(version)
    if not match:
        raise ValueError(f"invalid semantic version: {version}")
    prerelease_type = match.group(4)
    prerelease = (
        PreRelease(
            PreReleaseType[prerelease_type.upper()],
            int(match.group(5)),
        )
        if prerelease_type
        else None
    )
    return Version(
        int(match.group(1)), int(match.group(2)), int(match.group(3)), prerelease
    )
