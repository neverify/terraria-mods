import re
from dataclasses import dataclass
from functools import total_ordering

VERSION_RE = re.compile(r"^(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?$")
TAG_RE = re.compile(r"^(?P<mod_id>.+)-v(?P<version>\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?)$")


@total_ordering
@dataclass(frozen=True)
class Version:
    major: int
    minor: int
    patch: int
    prerelease: str = ""

    def __str__(self) -> str:
        return f"{self.major}.{self.minor}.{self.patch}{('-' + self.prerelease) if self.prerelease else ''}"

    def __lt__(self, other: object) -> bool:
        if not isinstance(other, Version):
            return NotImplemented
        base = (self.major, self.minor, self.patch)
        other_base = (other.major, other.minor, other.patch)
        if base != other_base:
            return base < other_base
        if not self.prerelease or not other.prerelease:
            return bool(self.prerelease) and not bool(other.prerelease)
        for left, right in zip(self.prerelease.split("."), other.prerelease.split(".")):
            if left == right:
                continue
            left_numeric = left.isdigit()
            right_numeric = right.isdigit()
            if left_numeric and right_numeric:
                return int(left) < int(right)
            if left_numeric != right_numeric:
                return left_numeric
            return left < right
        return len(self.prerelease.split(".")) < len(other.prerelease.split("."))

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


def parse_tag(tag: str) -> tuple[str, Version]:
    match = TAG_RE.fullmatch(tag)
    if not match:
        raise ValueError(f"invalid release tag: {tag}")
    return match.group("mod_id"), _parse_version(match.group("version"))


def _parse_version(version: str) -> Version:
    match = VERSION_RE.fullmatch(version)
    if not match:
        raise ValueError(f"invalid semantic version: {version}")
    return Version(
        int(match.group(1)),
        int(match.group(2)),
        int(match.group(3)),
        match.group(4) or "",
    )
