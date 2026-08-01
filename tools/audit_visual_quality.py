#!/usr/bin/env python3
"""Validate balanced/high FGO combat-frame mirrors and their Godot imports."""

from __future__ import annotations

import argparse
import hashlib
import json
import re
from pathlib import Path


WEBP_RE = re.compile(
    r'path="res://[^/"]+/character/quality_high/(?P<path>[^"\r\n]+\.webp)"'
)


def digest(path: Path) -> bytes:
    with path.open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").digest()


def check_import(path: Path, expected_limit: int, failures: list[str]) -> None:
    if not path.is_file():
        failures.append(f"falta import: {path}")
        return
    text = path.read_text(encoding="utf-8-sig")
    required = (
        "compress/mode=1",
        "compress/lossy_quality=0.85",
        "mipmaps/generate=true",
        f"process/size_limit={expected_limit}",
    )
    for setting in required:
        if setting not in text:
            failures.append(f"{path}: falta {setting}")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parent.parent)
    args = parser.parse_args()
    repo = args.repo.resolve()

    failures: list[str] = []
    rows: list[tuple[str, int, int]] = []
    total_resources = 0
    total_frames = 0

    high_dirs = sorted(repo.glob("*/*/character/quality_high"))
    for high_dir in high_dirs:
        character_dir = high_dir.parent
        project_dir = character_dir.parent.parent
        mod_name = project_dir.name
        main_file = next(iter(project_dir.glob("*Code/MainFile.cs")), None)
        main_text = main_file.read_text(encoding="utf-8-sig") if main_file else ""
        referenced_images: set[Path] = set()
        resource_count = 0

        for high_resource in sorted(high_dir.glob("*frames*.tres")):
            base_resource = character_dir / high_resource.name
            if not base_resource.is_file():
                failures.append(f"{mod_name}: falta recurso equilibrado para {high_resource.name}")
                continue

            high_text = high_resource.read_text(encoding="utf-8-sig")
            base_text = base_resource.read_text(encoding="utf-8-sig")
            normalized = high_text.replace("/character/quality_high/", "/character/")
            if normalized != base_text:
                failures.append(f"{mod_name}: {high_resource.name} no refleja exactamente el recurso base")

            if high_resource.name not in main_text:
                failures.append(f"{mod_name}: {high_resource.name} no está registrado en MainFile.cs")

            relative_images = {Path(match.group("path")) for match in WEBP_RE.finditer(high_text)}
            if not relative_images:
                failures.append(f"{mod_name}: {high_resource.name} no referencia WebP HD")
                continue

            resource_count += 1
            for relative in relative_images:
                high_image = high_dir / relative
                base_image = character_dir / relative
                referenced_images.add(high_image)
                if not high_image.is_file() or not base_image.is_file():
                    failures.append(f"{mod_name}: falta el par de fotogramas {relative.as_posix()}")
                    continue
                if high_image.stat().st_size != base_image.stat().st_size or digest(high_image) != digest(base_image):
                    failures.append(f"{mod_name}: el fotograma HD no conserva la fuente {relative.as_posix()}")
                check_import(Path(f"{high_image}.import"), 1024, failures)
                check_import(Path(f"{base_image}.import"), 768, failures)

        actual_images = set(high_dir.rglob("*.webp"))
        if actual_images != referenced_images:
            missing = referenced_images - actual_images
            orphaned = actual_images - referenced_images
            if missing:
                failures.append(f"{mod_name}: {len(missing)} fotogramas HD referenciados ausentes")
            if orphaned:
                failures.append(f"{mod_name}: {len(orphaned)} fotogramas HD huérfanos")

        rows.append((mod_name, resource_count, len(actual_images)))
        total_resources += resource_count
        total_frames += len(actual_images)

    for project_dir in sorted(path.parent for path in repo.glob("*/project.godot")):
        manifest_paths = list(project_dir.glob("*.json"))
        if not manifest_paths or project_dir.name == "FGOCore":
            continue
        manifest_id = json.loads(manifest_paths[0].read_text(encoding="utf-8-sig"))["id"]
        character_dir = project_dir / manifest_id / "character"
        for base_resource in character_dir.glob("*frames*.tres"):
            base_text = base_resource.read_text(encoding="utf-8-sig")
            if ".webp" in base_text and not (character_dir / "quality_high" / base_resource.name).is_file():
                failures.append(f"{project_dir.name}: falta variante HD de {base_resource.name}")

    for mod_name, resources, frames in rows:
        print(f"{mod_name}: {resources} recursos HD, {frames} fotogramas")
    print(f"TOTAL: {len(rows)} mods, {total_resources} recursos HD, {total_frames} fotogramas")

    if failures:
        print(f"Auditoría visual: {len(failures)} hallazgo(s)")
        for failure in failures:
            print(f" - {failure}")
        return 1

    print("Auditoría visual: OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
