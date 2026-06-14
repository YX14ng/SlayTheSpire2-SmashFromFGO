# ANIM-TODO — animaciones pendientes (paso MANUAL, para mañana)

Escrito 2026-06-14. Claude dejó **todo menos animaciones** de los personajes nuevos. Acá está,
por personaje, el **bundle** (ya descargado) y la **carpeta destino** donde van los frames +
la escena de visuales. El export de AssetStudioGUI + el render son pasos manuales (la GUI es
obligatoria; el CLI no exporta clips — ver [WORKFLOW-FGO.md](WORKFLOW-FGO.md) §3 y [ANIMATIONS.md](ANIMATIONS.md)).

## Procedimiento por personaje (resumen — detalle en WORKFLOW-FGO §3)

1. **Export GUI (manual)**: `tools/AssetStudioGUI/AssetStudioModGUI_net9_win64/AssetStudioModGUI.exe`
   → Load `assets/reference/bundles/<id>.bundle` → seleccionar el **Animator** + **TODOS los
   AnimationClips** → click derecho → **"Export Animator + selected AnimationClips"** →
   `assets/reference/extracted/<id>_anim/`. El FBX debe pesar MUCHO (con clips: 8–80MB; sin
   clips significa que no seleccionaste los clips).
2. **Render**: crear/ajustar `tools/render_all_<char>.ps1` (clonar el de Morgan/Artoria/Tiamat),
   fijar las ventanas de clip en `tools/render_project/render.gd` (modo `probe`/`measure` para
   hallar el crop y los rangos), modos list/probe/debug/measure/save.
3. **Frames .tres**: `tools/make_frames_tres.ps1` (o el específico del char) → genera
   `<char>_frames.tres` (SpriteFrames) apuntando a los .webp renderizados.
4. **Escena**: el `<char>_visuals.tscn` ya está scaffoldeado con un AnimatedSprite2D placeholder;
   apuntar su `sprite_frames` al .tres y ajustar `position`/`scale`/`flip_h` (probar en juego).
5. **Publish** → `tools/patch_webp_imports.ps1 -Dir <Char> -SizeLimit 1024` → **publish DE NUEVO**
   (los `.webp.import` nacen en el primer publish; el scale de los .tscn asume size_limit 1024).

## Estado por personaje

| Personaje | Servant id | Bundle | Textura | Carpeta destino (frames + visuals.tscn) |
|---|---|---|---|---|
| **Siegfried** | 100800 | ✅ `assets/reference/bundles/100800.bundle` | ✅ | `SiegfriedSaber/SiegfriedSaber/character/` (ya scaffoldeada; `siegfried_frames.tres` es placeholder) |
| **Mordred** | 100900 | ✅ `assets/reference/bundles/100900.bundle` (5.7MB) | ✅ `100900.png` | `MordredSaber/MordredSaber/character/` *(se crea al scaffoldear)* |
| **Gilgamesh** | 200200 | ✅ `assets/reference/bundles/200200.bundle` (6.2MB) | ✅ `200200.png` | `GilgameshArcher/GilgameshArcher/character/` *(se crea al scaffoldear)* |
| **Okita** | 102700 | ✅ `assets/reference/bundles/102700.bundle` (6.5MB) | ✅ `102700.png` | `OkitaSaber/OkitaSaber/character/` *(se crea al scaffoldear)* |
| **Oberon** | 2800100 | ✅ `assets/reference/bundles/2800100.bundle` (9.1MB) | ✅ `2800100.png` | `OberonPretender/OberonPretender/character/` *(se crea al scaffoldear)* |

> Nombres de proyecto provisionales (MordredSaber, etc.) — se confirman al scaffoldear según el
> doc de diseño finalizado (`docs/DESIGN-<CHAR>.md`). El `id` del manifest NO se cambia una vez elegido.

## Notas
- Siegfried: el bundle 100800 ya estaba; falta el export GUI (extracted/100800_anim sólo tiene la
  textura, sin FBX) → re-exportar con clips.
- Oberon (2800100) es Pretender: puede tener huesos/props raros (revisar HIDE_BONES/CLIP_OVERRIDE
  como en Artoria/Morgan). Mordred lleva casco — ojo con el clip de "casco quitado" si el modelo
  lo trae como animación o como malla aparte.
- Las bundles están **gitignoradas** (binarios on-demand); re-descargar con
  `tools/fetch_fgo_bundle.ps1 -Ids <id> -Texture` si hace falta.
