#!/usr/bin/env python3
"""Audit FGO character frame sequences before they are packed into a mod."""

from __future__ import annotations

import argparse
import json
import math
import re
import statistics
import sys
from collections import deque
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

try:
    from PIL import Image, ImageChops, ImageDraw, ImageFont, ImageOps, ImageStat
except ImportError as exc:  # pragma: no cover - actionable CLI failure
    raise SystemExit("Pillow is required: python -m pip install Pillow") from exc


ANIMATIONS = ("idle", "attack", "cast", "hurt")
LIMITS = {
    "attack": {"min": 0.35, "max": 1.20, "max_static_tail": 0.45},
    "cast": {"min": 0.35, "max": 1.50, "max_static_tail": 0.55},
    "hurt": {"min": 0.20, "max": 0.60, "max_static_tail": 0.25},
}
FALLBACK_SPEED = {"idle": 15.0, "attack": 30.0, "cast": 15.0, "hurt": 30.0}
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


@dataclass
class FrameMetric:
    name: str
    width: int
    height: int
    bbox: tuple[int, int, int, int] | None
    alpha_pixels: int
    second_component_share: float
    signature: Image.Image

    @property
    def center(self) -> tuple[float, float] | None:
        if self.bbox is None:
            return None
        x0, y0, x1, y1 = self.bbox
        return ((x0 + x1) / 2.0, (y0 + y1) / 2.0)

    @property
    def bbox_width(self) -> int:
        return 0 if self.bbox is None else self.bbox[2] - self.bbox[0]

    @property
    def bbox_height(self) -> int:
        return 0 if self.bbox is None else self.bbox[3] - self.bbox[1]


def parse_args() -> argparse.Namespace:
    script = Path(__file__).resolve()
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=script.parent.parent)
    parser.add_argument("--manifest", type=Path, default=script.parent / "animation_manifest.json")
    parser.add_argument("--output", type=Path)
    parser.add_argument("--only", action="append", default=[], help="Model id; repeat as needed")
    parser.add_argument("--no-contact-sheets", action="store_true")
    parser.add_argument("--contact-max", type=int, default=48)
    return parser.parse_args()


def parse_sprite_frames(path: Path) -> dict[str, dict[str, Any]]:
    text = path.read_text(encoding="utf-8-sig")
    resources = {match.group("id"): match.group("path") for match in EXT_RE.finditer(text)}
    result: dict[str, dict[str, Any]] = {}
    for match in ENTRY_RE.finditer(text):
        name = match.group("name")
        resource_ids = EXT_ID_RE.findall(match.group("frames"))
        result[name] = {
            "speed": float(match.group("speed")),
            "loop": match.group("loop") == "true",
            "paths": [resources[resource_id] for resource_id in resource_ids if resource_id in resources],
            "missing_resource_ids": [resource_id for resource_id in resource_ids if resource_id not in resources],
        }
    return result


def second_component_share(alpha: Image.Image) -> float:
    mask = alpha.resize((96, 96), Image.Resampling.NEAREST)
    pixels = mask.load()
    seen = bytearray(96 * 96)
    sizes: list[int] = []
    for y in range(96):
        for x in range(96):
            start = y * 96 + x
            if seen[start] or pixels[x, y] < 32:
                continue
            seen[start] = 1
            queue = deque([(x, y)])
            size = 0
            while queue:
                cx, cy = queue.popleft()
                size += 1
                for nx, ny in ((cx - 1, cy), (cx + 1, cy), (cx, cy - 1), (cx, cy + 1)):
                    if nx < 0 or ny < 0 or nx >= 96 or ny >= 96:
                        continue
                    idx = ny * 96 + nx
                    if seen[idx] or pixels[nx, ny] < 32:
                        continue
                    seen[idx] = 1
                    queue.append((nx, ny))
            if size >= 4:
                sizes.append(size)
    if len(sizes) < 2:
        return 0.0
    sizes.sort(reverse=True)
    return sizes[1] / max(1, sum(sizes))


def metric_for(path: Path) -> FrameMetric:
    with Image.open(path) as source:
        rgba = source.convert("RGBA")
        alpha = rgba.getchannel("A")
        histogram = alpha.histogram()
        composite = Image.new("RGB", rgba.size, (12, 12, 18))
        composite.paste(rgba, mask=alpha)
        signature = composite.resize((48, 48), Image.Resampling.LANCZOS)
        return FrameMetric(
            name=path.name,
            width=rgba.width,
            height=rgba.height,
            bbox=alpha.getbbox(),
            alpha_pixels=sum(histogram[1:]),
            second_component_share=second_component_share(alpha),
            signature=signature,
        )


def image_delta(left: Image.Image, right: Image.Image) -> float:
    rms = ImageStat.Stat(ImageChops.difference(left, right)).rms
    return math.sqrt(sum(channel * channel for channel in rms) / len(rms)) / 255.0


def sampled_indices(count: int, maximum: int) -> list[int]:
    if count <= maximum:
        return list(range(count))
    return sorted({round(index * (count - 1) / (maximum - 1)) for index in range(maximum)})


def make_contact_sheet(files: list[Path], output: Path, maximum: int) -> None:
    indices = sampled_indices(len(files), maximum)
    columns = 8
    thumb = (180, 150)
    label_height = 22
    rows = math.ceil(len(indices) / columns)
    sheet = Image.new("RGB", (columns * thumb[0], rows * (thumb[1] + label_height)), (18, 18, 28))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.load_default()
    for slot, frame_index in enumerate(indices):
        with Image.open(files[frame_index]) as source:
            frame = source.convert("RGBA")
            background = Image.new("RGBA", frame.size, (18, 18, 28, 255))
            background.alpha_composite(frame)
            preview = ImageOps.contain(background.convert("RGB"), thumb, Image.Resampling.LANCZOS)
        x = (slot % columns) * thumb[0] + (thumb[0] - preview.width) // 2
        y = (slot // columns) * (thumb[1] + label_height) + (thumb[1] - preview.height) // 2
        sheet.paste(preview, (x, y))
        draw.text((slot % columns * thumb[0] + 5, y + thumb[1] + 3), files[frame_index].stem, fill="white", font=font)
    output.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(output, quality=88, optimize=True)


def audit_animation(files: list[Path], anim: str, speed: float, allow_static: bool = False) -> dict[str, Any]:
    errors: list[str] = []
    warnings: list[str] = []
    metrics = [metric_for(path) for path in files]
    duration = len(files) / speed if speed > 0 else math.inf

    if speed <= 0:
        errors.append("velocidad inválida")

    blank = [metric.name for metric in metrics if metric.bbox is None]
    if blank:
        errors.append(f"{len(blank)} fotogramas transparentes: {', '.join(blank[:8])}")

    nonblank = [metric for metric in metrics if metric.bbox is not None]
    if nonblank:
        median_width = statistics.median(metric.bbox_width for metric in nonblank)
        median_height = statistics.median(metric.bbox_height for metric in nonblank)
        median_alpha = statistics.median(metric.alpha_pixels for metric in nonblank)
        extreme: list[str] = []
        disconnected: list[str] = []
        edge: list[str] = []
        for metric in nonblank:
            width_ratio = metric.bbox_width / max(1, median_width)
            height_ratio = metric.bbox_height / max(1, median_height)
            alpha_ratio = metric.alpha_pixels / max(1, median_alpha)
            if not 0.35 <= width_ratio <= 2.50 or not 0.35 <= height_ratio <= 2.50 or not 0.25 <= alpha_ratio <= 3.0:
                extreme.append(metric.name)
            if metric.second_component_share >= 0.22:
                disconnected.append(metric.name)
            x0, y0, x1, y1 = metric.bbox or (0, 0, 0, 0)
            if x0 <= 1 or y0 <= 1 or x1 >= metric.width - 1 or y1 >= metric.height - 1:
                edge.append(metric.name)
        if extreme:
            errors.append(f"silueta/tamaño extremo en {', '.join(extreme[:8])}")
        if disconnected:
            warnings.append(f"componentes grandes separadas en {', '.join(disconnected[:8])}")
        if edge:
            warnings.append(f"contenido tocando el borde en {', '.join(edge[:8])}")

    deltas = [image_delta(metrics[index - 1].signature, metrics[index].signature) for index in range(1, len(metrics))]
    jumps = [metrics[index].name for index, delta in enumerate(deltas, start=1) if delta >= 0.38]
    if jumps:
        errors.append(f"saltos visuales extremos en {', '.join(jumps[:8])}")

    center_jumps: list[str] = []
    for previous, current in zip(metrics, metrics[1:]):
        if previous.center is None or current.center is None:
            continue
        dx = (current.center[0] - previous.center[0]) / max(1, current.width)
        dy = (current.center[1] - previous.center[1]) / max(1, current.height)
        if math.hypot(dx, dy) >= 0.32:
            center_jumps.append(current.name)
    if center_jumps:
        errors.append(f"saltos de posición extremos en {', '.join(center_jumps[:8])}")

    static_tail = 1 if metrics else 0
    for delta in reversed(deltas):
        if delta > 0.003:
            break
        static_tail += 1
    static_tail_seconds = static_tail / speed if speed > 0 else 0.0

    limits = LIMITS.get(anim)
    if limits:
        frame_tolerance = 1.0 / speed if speed > 0 else 0.0
        if duration > limits["max"] + frame_tolerance:
            errors.append(f"duración {duration:.2f}s supera {limits['max']:.2f}s")
        elif duration < limits["min"]:
            warnings.append(f"duración corta: {duration:.2f}s")
        if (
            not allow_static
            and static_tail_seconds > limits["max_static_tail"]
            and static_tail >= max(4, len(metrics) // 4)
        ):
            errors.append(f"cola casi estática de {static_tail_seconds:.2f}s ({static_tail} frames)")

    return {
        "animation": anim,
        "frames": len(files),
        "speed": speed,
        "duration_seconds": round(duration, 3),
        "static_tail_frames": static_tail,
        "max_frame_delta": round(max(deltas, default=0.0), 4),
        "errors": errors,
        "warnings": warnings,
    }


def audit_form(repo: Path, form: dict[str, Any], output: Path, contact_max: int, contacts: bool) -> dict[str, Any]:
    result: dict[str, Any] = {key: form[key] for key in ("id", "character", "form")}
    result["animations"] = []
    result["errors"] = []
    result["warnings"] = []

    resource = repo / form["resource"]
    if not resource.is_file():
        result["errors"].append(f"falta el recurso {form['resource']}")
        return result
    parsed = parse_sprite_frames(resource)

    procedural = form.get("procedural")
    frames_root = repo / form["frames"] if form["frames"] is not None else None
    if procedural:
        procedural_path = repo / procedural
        if not procedural_path.is_file():
            result["errors"].append(f"falta el controlador procedural {procedural}")
            return result
        result["warnings"].append("usa fallback procedural porque el rig oficial no admite retarget seguro")
    elif frames_root is None:
        result["errors"].append("la forma usa un visual estático; faltan idle/attack/cast/hurt")
        return result
    elif not frames_root.is_dir():
        result["errors"].append(f"falta la carpeta {form['frames']}")
        return result

    project_container = repo / Path(form["resource"]).parts[0]

    for anim in ANIMATIONS:
        resource_anim = parsed.get(anim, {})
        speed = float(resource_anim.get("speed", FALLBACK_SPEED[anim]))
        if not resource_anim:
            result["errors"].append(f"el recurso no declara {anim}")
            continue
        if resource_anim["missing_resource_ids"]:
            result["errors"].append(
                f"{anim} usa ids de textura inexistentes: {', '.join(resource_anim['missing_resource_ids'][:8])}"
            )
        files: list[Path] = []
        for resource_path in resource_anim["paths"]:
            if not resource_path.startswith("res://"):
                result["errors"].append(f"{anim} usa una ruta no soportada: {resource_path}")
                continue
            files.append(project_container / resource_path.removeprefix("res://"))
        missing_files = [str(path.relative_to(repo)) for path in files if not path.is_file()]
        if missing_files:
            result["errors"].append(f"{anim} referencia archivos ausentes: {', '.join(missing_files[:8])}")
            continue
        if not files:
            result["errors"].append(f"{anim} no referencia fotogramas")
            continue
        animation_result = audit_animation(files, anim, speed, allow_static=bool(procedural))
        if frames_root is not None:
            directory = frames_root / anim
            disk_files = sorted(directory.glob("*.webp")) if directory.is_dir() else []
            if not disk_files:
                result["errors"].append(f"faltan fotogramas de {anim}")
                continue
            if all(path.parent == directory for path in files):
                disk_names = {path.name for path in disk_files}
                unused = sorted(disk_names - {path.name for path in files})
                if unused:
                    animation_result["warnings"].append(f"{len(unused)} fotogramas en disco no utilizados")
            else:
                animation_result["warnings"].append("usa una secuencia alternativa validada")
        result["animations"].append(animation_result)
        if contacts:
            name = f"{form['id']}_{form['character']}_{form['form']}_{anim}".replace(" ", "_")
            make_contact_sheet(files, output / "contact-sheets" / f"{name}.jpg", contact_max)
    return result


def markdown_report(report: dict[str, Any]) -> str:
    lines = [
        "# Auditoría de animaciones FGO",
        "",
        f"Generada: {report['generated_at']}",
        "",
        f"Resultado: **{'APROBADO' if report['ok'] else 'FALLÓ'}** — "
        f"{report['error_count']} errores, {report['warning_count']} advertencias.",
        "",
        "| Modelo | Personaje / forma | Animación | Frames | FPS | Duración | Errores | Advertencias |",
        "|---|---|---:|---:|---:|---:|---:|---:|",
    ]
    for form in report["forms"]:
        if not form["animations"]:
            lines.append(
                f"| {form['id']} | {form['character']} / {form['form']} | — | — | — | — | "
                f"{len(form['errors'])} | {len(form['warnings'])} |"
            )
        for animation in form["animations"]:
            lines.append(
                f"| {form['id']} | {form['character']} / {form['form']} | {animation['animation']} | "
                f"{animation['frames']} | {animation['speed']:g} | {animation['duration_seconds']:.2f}s | "
                f"{len(animation['errors'])} | {len(animation['warnings'])} |"
            )
    lines.extend(["", "## Hallazgos", ""])
    for form in report["forms"]:
        prefix = f"{form['id']} {form['character']} / {form['form']}"
        for error in form["errors"]:
            lines.append(f"- **ERROR — {prefix}:** {error}")
        for warning in form["warnings"]:
            lines.append(f"- Advertencia — {prefix}: {warning}")
        for animation in form["animations"]:
            for error in animation["errors"]:
                lines.append(f"- **ERROR — {prefix} / {animation['animation']}:** {error}")
            for warning in animation["warnings"]:
                lines.append(f"- Advertencia — {prefix} / {animation['animation']}: {warning}")
    if report["error_count"] == 0 and report["warning_count"] == 0:
        lines.append("- Sin hallazgos.")
    return "\n".join(lines) + "\n"


def main() -> int:
    args = parse_args()
    repo = args.repo.resolve()
    output = (args.output or repo / "dist" / "animation-audit").resolve()
    manifest = json.loads(args.manifest.read_text(encoding="utf-8"))
    forms = manifest["forms"]
    if args.only:
        selected = set(args.only)
        forms = [form for form in forms if form["id"] in selected]
        missing = selected - {form["id"] for form in forms}
        if missing:
            raise SystemExit(f"Unknown model ids: {', '.join(sorted(missing))}")

    output.mkdir(parents=True, exist_ok=True)
    results = [
        audit_form(repo, form, output, args.contact_max, not args.no_contact_sheets)
        for form in forms
    ]
    error_count = sum(
        len(form["errors"]) + sum(len(animation["errors"]) for animation in form["animations"])
        for form in results
    )
    warning_count = sum(
        len(form["warnings"]) + sum(len(animation["warnings"]) for animation in form["animations"])
        for form in results
    )
    report = {
        "generated_at": datetime.now(timezone.utc).isoformat(),
        "ok": error_count == 0,
        "error_count": error_count,
        "warning_count": warning_count,
        "forms": results,
    }
    (output / "animation-audit.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    (output / "animation-audit.md").write_text(markdown_report(report), encoding="utf-8")
    print(f"Animation audit: {'PASS' if report['ok'] else 'FAIL'} - {error_count} errors, {warning_count} warnings")
    print(output / "animation-audit.md")
    return 0 if report["ok"] else 1


if __name__ == "__main__":
    sys.exit(main())
