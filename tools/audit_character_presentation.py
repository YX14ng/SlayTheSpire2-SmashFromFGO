#!/usr/bin/env python3
"""Audit custom merchant/rest scenes and the shared FGO sprite presentation layer."""

from __future__ import annotations

import argparse
import re
import sys
from dataclasses import dataclass
from pathlib import Path


MODEL_RE = re.compile(r"class\s+(?P<name>\w+)\s*:\s*PlaceholderCharacterModel")
PATH_RE = {
    "merchant": re.compile(
        r'CustomMerchantAnimPath\s*=>\s*\$"\{MainFile\.ResPath\}/character/(?P<file>[^"]+)"'
    ),
    "rest": re.compile(
        r'CustomRestSiteAnimPath\s*=>\s*\$"\{MainFile\.ResPath\}/character/(?P<file>[^"]+)"'
    ),
}
FRAMES_RE = re.compile(r'\[ext_resource\s+type="SpriteFrames"\s+path="(?P<path>res://[^"]+)"')


@dataclass
class Result:
    character: str
    merchant: str = "-"
    rest: str = "-"
    errors: list[str] | None = None

    def __post_init__(self) -> None:
        if self.errors is None:
            self.errors = []


def parse_args() -> argparse.Namespace:
    script = Path(__file__).resolve()
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=script.parent.parent)
    return parser.parse_args()


def model_files(repo: Path) -> list[tuple[Path, re.Match[str]]]:
    found: list[tuple[Path, re.Match[str]]] = []
    for path in sorted(repo.glob("*/*Code/Character/*.cs")):
        match = MODEL_RE.search(path.read_text(encoding="utf-8-sig"))
        if match:
            found.append((path, match))
    return found


def resolve_scene(project: Path, filename: str) -> Path | None:
    matches = list(project.glob(f"*/character/{filename}"))
    return matches[0] if len(matches) == 1 else None


def physical_resource(project: Path, resource_path: str) -> Path:
    return project / resource_path.removeprefix("res://")


def audit_scene(project: Path, kind: str, path: Path, smoother: str) -> list[str]:
    text = path.read_text(encoding="utf-8-sig")
    errors: list[str] = []
    if not re.search(r'^\[node name="[^"]+" type="Node2D"\]$', text, re.MULTILINE):
        errors.append(f"{path.name}: la raiz no es Node2D")
    if 'type="AnimatedSprite2D"' not in text:
        errors.append(f"{path.name}: no usa AnimatedSprite2D")
    if 'animation = &"idle"' not in text or 'autoplay = "idle"' not in text:
        errors.append(f"{path.name}: no inicia idle automaticamente")
    if "flip_h = true" not in text:
        errors.append(f"{path.name}: no mira hacia el mercader/fogata")
    if kind == "rest":
        if 'name="ControlRoot" type="Control" parent="."' not in text:
            errors.append(f"{path.name}: falta ControlRoot")
        if 'name="Hitbox" type="Control" parent="ControlRoot"' not in text:
            errors.append(f"{path.name}: falta Hitbox bajo ControlRoot")
        if "unique_name_in_owner = true" not in text:
            errors.append(f"{path.name}: Hitbox no es unique_name_in_owner")

    frames = FRAMES_RE.search(text)
    if frames is None:
        errors.append(f"{path.name}: falta un recurso SpriteFrames")
    else:
        resource_path = frames.group("path")
        if not physical_resource(project, resource_path).is_file():
            errors.append(f"{path.name}: no existe {resource_path}")
        prefix = resource_path.split("/", 3)[:3]
        resource_prefix = "/".join(prefix) + "/"
        if f'"{resource_prefix}"' not in smoother:
            errors.append(f"{path.name}: {resource_prefix} no esta cubierto por el suavizado compartido")
    return errors


def main() -> int:
    args = parse_args()
    repo = args.repo.resolve()
    smoother_path = repo / "FGOCore/FGOCoreCode/Animation/FgoAnimationSmoothing.cs"
    if not smoother_path.is_file():
        print(f"ERROR: falta {smoother_path.relative_to(repo)}")
        return 1
    smoother = smoother_path.read_text(encoding="utf-8-sig")

    models = model_files(repo)
    results: list[Result] = []
    for model_path, model_match in models:
        project = model_path.parents[2]
        source = model_path.read_text(encoding="utf-8-sig")
        result = Result(model_match.group("name"))
        for kind in ("merchant", "rest"):
            path_match = PATH_RE[kind].search(source)
            if path_match is None:
                result.errors.append(f"falta Custom{kind.title()}AnimPath")
                continue
            scene = resolve_scene(project, path_match.group("file"))
            if scene is None:
                result.errors.append(f"no se encontro una unica escena {path_match.group('file')}")
                continue
            scene_errors = audit_scene(project, kind, scene, smoother)
            setattr(result, kind, "OK" if not scene_errors else "ERROR")
            result.errors.extend(scene_errors)
        results.append(result)

    if len(results) != 12:
        print(f"ERROR: se esperaban 12 personajes y se encontraron {len(results)}")
        return 1

    print("Personaje             Tienda  Descanso")
    print("--------------------  ------  --------")
    for result in results:
        print(f"{result.character:<20}  {result.merchant:<6}  {result.rest:<8}")
    errors = [(result.character, error) for result in results for error in result.errors]
    if errors:
        print("")
        for character, error in errors:
            print(f"ERROR - {character}: {error}")
        print(f"Presentacion FGO: FAIL - {len(errors)} errores")
        return 1

    print("Presentacion FGO: PASS - 12/12 con tienda y descanso animados")
    return 0


if __name__ == "__main__":
    sys.exit(main())
