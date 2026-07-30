#!/usr/bin/env python3
"""Apply/check the animation windows declared in animation_manifest.json."""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path
from typing import Any


ANIMATION_ORDER = ("idle", "attack", "cast", "hurt", "die")
ENTRY_RE = re.compile(
    r'\{\s*"frames":\s*\[(?P<frames>.*?)\],\s*'
    r'"loop":\s*(?P<loop>true|false),\s*'
    r'"name":\s*&"(?P<name>[^"]+)",\s*'
    r'"speed":\s*(?P<speed>[0-9.]+)\s*\}',
    re.DOTALL,
)
EXT_RE = re.compile(
    r'\[ext_resource\s+[^\]]*path="(?P<path>[^"]+)"[^\]]*id="(?P<id>[^"]+)"[^\]]*\]'
)
EXT_ID_RE = re.compile(r'ExtResource\("([^"]+)"\)')


def parse_args() -> argparse.Namespace:
    script = Path(__file__).resolve()
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=script.parent.parent)
    parser.add_argument("--manifest", type=Path, default=script.parent / "animation_manifest.json")
    parser.add_argument("--apply", action="store_true", help="Write the declared profiles; default is check-only")
    parser.add_argument("--only", action="append", default=[], help="Model id; repeat as needed")
    return parser.parse_args()


def parse_resource(path: Path) -> dict[str, dict[str, Any]]:
    text = path.read_text(encoding="utf-8-sig")
    resources = {match.group("id"): match.group("path") for match in EXT_RE.finditer(text)}
    animations: dict[str, dict[str, Any]] = {}
    for match in ENTRY_RE.finditer(text):
        ids = EXT_ID_RE.findall(match.group("frames"))
        missing = [resource_id for resource_id in ids if resource_id not in resources]
        if missing:
            raise ValueError(f"{path}: texture ids not declared: {', '.join(missing)}")
        animations[match.group("name")] = {
            "paths": [resources[resource_id] for resource_id in ids],
            "loop": match.group("loop") == "true",
            "speed": float(match.group("speed")),
        }
    missing_animations = [name for name in ANIMATION_ORDER if name not in animations]
    if missing_animations:
        raise ValueError(f"{path}: missing animations: {', '.join(missing_animations)}")
    return animations


def project_container(repo: Path, form: dict[str, Any]) -> Path:
    first_component = Path(form["resource"]).parts[0]
    return repo / first_component


def physical_path(container: Path, godot_path: str) -> Path:
    if not godot_path.startswith("res://"):
        raise ValueError(f"unsupported resource path: {godot_path}")
    return container / godot_path.removeprefix("res://")


def godot_path(container: Path, frame: Path) -> str:
    return "res://" + frame.relative_to(container).as_posix()


def selected_paths(repo: Path, form: dict[str, Any], animation: str, profile: dict[str, Any]) -> list[str]:
    frames_root_value = form.get("frames")
    if not frames_root_value:
        raise ValueError(f"{form['id']} {animation}: a procedural form cannot declare frame overrides")
    source = str(profile.get("source", animation))
    source_dir = repo / frames_root_value / source
    files = sorted(source_dir.glob("*.webp")) if source_dir.is_dir() else []
    if not files:
        raise ValueError(f"{form['id']} {animation}: no WebP frames in {source_dir}")
    first = int(profile.get("first", 0))
    last = int(profile.get("last", len(files) - 1))
    step = int(profile.get("step", 1))
    if first < 0 or last < first or last >= len(files) or step < 1:
        raise ValueError(
            f"{form['id']} {animation}: invalid range {first}..{last}/{step} for {len(files)} frames"
        )
    container = project_container(repo, form)
    return [godot_path(container, frame) for frame in files[first : last + 1 : step]]


def render_resource(repo: Path, form: dict[str, Any]) -> str:
    resource = repo / form["resource"]
    current = parse_resource(resource)
    overrides = form.get("overrides", {})
    animations: dict[str, dict[str, Any]] = {}
    container = project_container(repo, form)

    for name in ANIMATION_ORDER:
        old = current[name]
        profile = overrides.get(name)
        paths = selected_paths(repo, form, name, profile) if profile else list(old["paths"])
        speed = float(profile.get("speed", old["speed"])) if profile else float(old["speed"])
        loop = bool(profile.get("loop", old["loop"])) if profile else bool(old["loop"])
        if speed <= 0:
            raise ValueError(f"{form['id']} {name}: speed must be positive")
        missing = [path for path in paths if not physical_path(container, path).is_file()]
        if missing:
            raise ValueError(f"{form['id']} {name}: missing files: {', '.join(missing[:5])}")
        animations[name] = {"paths": paths, "speed": speed, "loop": loop}

    ordered_paths: list[str] = []
    for name in ANIMATION_ORDER:
        for path in animations[name]["paths"]:
            if path not in ordered_paths:
                ordered_paths.append(path)
    resource_ids = {path: f"tex_{index}" for index, path in enumerate(ordered_paths, start=1)}

    lines = [f'[gd_resource type="SpriteFrames" load_steps={len(ordered_paths) + 1} format=3]', ""]
    lines.extend(
        f'[ext_resource type="Texture2D" path="{path}" id="{resource_ids[path]}"]'
        for path in ordered_paths
    )
    lines.extend(["", "[resource]", "animations = ["])
    for animation_index, name in enumerate(ANIMATION_ORDER):
        animation = animations[name]
        lines.append("{")
        lines.append('"frames": [')
        for frame_index, path in enumerate(animation["paths"]):
            comma = "," if frame_index + 1 < len(animation["paths"]) else ""
            lines.append(
                '{"duration": 1.0, "texture": ExtResource("'
                + resource_ids[path]
                + '")}'
                + comma
            )
        lines.append("],")
        lines.append(f'"loop": {str(animation["loop"]).lower()},')
        lines.append(f'"name": &"{name}",')
        lines.append(f'"speed": {animation["speed"]:g}')
        lines.append("}" + ("," if animation_index + 1 < len(ANIMATION_ORDER) else ""))
    lines.append("]")
    return "\n".join(lines) + "\n"


def main() -> int:
    args = parse_args()
    repo = args.repo.resolve()
    manifest = json.loads(args.manifest.read_text(encoding="utf-8"))
    selected = set(args.only)
    forms = [
        form
        for form in manifest["forms"]
        if form.get("overrides") and (not selected or form["id"] in selected)
    ]
    unknown = selected - {form["id"] for form in manifest["forms"]}
    if unknown:
        raise SystemExit(f"Unknown model ids: {', '.join(sorted(unknown))}")

    changed: list[str] = []
    for form in forms:
        resource = repo / form["resource"]
        expected = render_resource(repo, form)
        current = resource.read_text(encoding="utf-8-sig").replace("\r\n", "\n")
        if current == expected:
            continue
        changed.append(form["id"])
        if args.apply:
            resource.write_text(expected, encoding="utf-8", newline="\n")

    if changed and not args.apply:
        print("Animation profiles out of date: " + ", ".join(changed))
        return 1
    action = "Applied" if args.apply else "Verified"
    print(f"{action} animation profiles: {len(forms)} forms ({len(changed)} changed)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
