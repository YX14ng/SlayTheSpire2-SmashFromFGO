# ANIM-TODO — estado de animaciones

Actualizado 2026-06-14 (tras el render). Los 5 personajes nuevos/recientes (Mordred, Gilgamesh,
Okita, Oberon, Siegfried) ya tienen **animaciones renderizadas, empaquetadas y publicadas**.

## ✅ HECHO (export → render → frames → publish)

Por personaje: FBX exportado (AssetStudioGUI) → render a webp (`tools/render_all_new.ps1`,
auto-escala+auto-crop de `render_project/render.gd`, ventanas `SELECT` por id) → `<char>_frames.tres`
(`tools/make_char_frames_tres.ps1`, anims idle/attack/cast/hurt/die) → la `<char>_visuals.tscn`
(scaffold) ya lo referencia → publicado a F:\Games y Steam.

| Personaje | Bundle / FBX | Frames (idle/atk/cast/hurt) | .tres |
|---|---|---|---|
| Mordred | 100900 (Animator/model) | 77/77/78/17 | mordred_frames.tres |
| Gilgamesh | 200200 (Animator/**chr**) | 77/77/78/17 | gilgamesh_frames.tres |
| Okita | 102700 (Animator/model) | 77/77/77/15 | okita_frames.tres |
| Oberon | 2800100 (Animator/**chr**) | 77/77/77/15 | oberon_frames.tres |
| Siegfried | 100800 (Animator/model) | 101/75/101/17 | siegfried_frames.tres |

> Gotcha: Gilgamesh y Oberon exportaron como `Animator/chr/chr.fbx` (no `model/model.fbx`);
> `render_all_new.ps1` auto-detecta ambos. Los FBX exportados viven en `assets/reference/extracted/<id>_anim/`.

## ⏳ PENDIENTE — fine-tune de scale/position en juego (paso interactivo)

Las `<char>_visuals.tscn` usan el **scale/position por defecto del scaffold** (`scale 0.6`, `position (0,-217)`,
`flip_h=true`). Hay que ajustarlos por personaje viéndolos en combate (que pisen el piso, tamaño correcto),
igual que se hizo con Morgan/Artoria. Casos a vigilar:
- **Oberon**: crop alto (las alas de mariposa suben mucho) → probablemente flota; bajar `position.y` o ajustar.
- **Siegfried**: crop alto (espada Balmung en alto en algunos clips).
- Re-render de un clip / re-ventaneo: `tools/render_all_new.ps1 -Mode probe/debug/save -Only <id>`,
  ventanas en `render_project/render.gd` (`SELECT`). Tras re-render: `make_char_frames_tres.ps1` + publish.

## Optimización opcional (VRAM)
Si hay presión de VRAM (Siegfried tiene frames de ~1905px): `tools/patch_webp_imports.ps1 -Dir <Char> -SizeLimit 1024`
entre dos publishes (publish → patch → publish), como Morgan/Artoria. Hoy se publicó sin patch (import .ctex
mode=1 VRAM-comprimido).

## Animaciones ya completas de antes (no tocar)
Mash (800100/800150/800200), Morgan (505320/704020/704030), Artoria (504520/704710/704720), Tiamat (9935400/9935410).
