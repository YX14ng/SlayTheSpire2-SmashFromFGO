# Pipeline: animaciones originales de FGO → Slay the Spire 2

Investigado y validado el 2026-06-10. Estado: **pipeline confirmado de punta a punta**; queda un único paso manual (GUI) para producir el FBX animado.

## Hallazgos clave

1. **Los bundles de Atlas Academy son UnityFS estándar SIN cifrar** (Unity 2022.3). Descargados en `assets/reference/bundles/` (base `800100`, Ortinax `800150`, Paladín `800200`).
2. **Las animaciones originales están adentro**: el bundle base contiene **19 AnimationClips**, el rig completo (296 huesos/Transforms), 8 SkinnedMeshRenderers y el atlas de texturas. No es un spritesheet: es un puppet 3D skinneado que se ve 2D por cámara ortográfica.
3. **El destino en StS2 es directo**: BaseLib conecta automáticamente un `AnimationPlayer` de Godot cuyo nombre de animaciones sea `idle`, `attack`, `cast`, `hurt`, `die` (wiki de BaseLib, "Creature Visuals"). Godot 4.5 (= MegaDot) **importa FBX nativamente** con su AnimationPlayer y clips incluidos.

## Mapeo de clips FGO → estados StS2

Clips reales del bundle de Mash (nombres canónicos de FGO):

| Clip FGO | Estado StS2 | Nota |
|---|---|---|
| `wait` | `idle` | Loop de espera |
| `attack_b` (Buster) | `attack` | Alternativas: `attack_a` (Arts), `attack_q` (Quick), `attack_gen` |
| `spell` + `spell_loop` | `cast` | Casteo de skill |
| `treasureArms1_a` … `treasureArms7_a` | (cast especial / NP) | **La secuencia completa de Lord Camelot** — usable para las cartas NP en v3 |
| `damage_01` | `hurt` | Reacción de daño |
| — | `die` | FGO no tiene clip de muerte (usa pose de daño + fade); reusar `damage_01` |
| `step_front`, `step_back`, `eye_open`, `eye_close` | (extras) | Movimiento y parpadeo |

## El pipeline (3 pasos)

### Paso 1 — Exportar FBX con animaciones (único paso manual, ~2 minutos)

El CLI de AssetStudio no vincula AnimationClips sueltos al FBX (limitación confirmada de la v0.19.0); la **GUI sí**. Ya está descargada en `tools/AssetStudioGUI/`:

1. Abrir `AssetStudioModGUI.exe` → File → Load File → `assets/reference/bundles/800100.bundle`.
2. Pestaña **Asset List** → filtrar por tipo, seleccionar el **Animator `chr`** y los **19 AnimationClips** (Ctrl+click o filtrar `AnimationClip` y Ctrl+A).
3. Click derecho → **"Export Animator + selected AnimationClips"** → elegir carpeta `assets/reference/extracted/800100_anim/`.
4. Resultado: `chr.fbx` con 19 takes de animación + textura. Repetir para `800150` (Ortinax) y `800200` (Paladín) si se quiere visual por forma.

> Si la GUI requiere .NET 9: ya tenemos .NET 10; ejecutarla con `$env:DOTNET_ROLL_FORWARD="LatestMajor"` desde la terminal si se queja.

### Paso 2 — Importar en Godot/MegaDot y armar la escena

1. Copiar `chr.fbx` + textura a `MashShielder/MashShielder/character/` y abrir el proyecto en MegaDot: el import genera la escena con `Skeleton3D`, meshes y un **AnimationPlayer con los 19 clips**.
2. Crear `mash_visuals.tscn` con la estructura que exige BaseLib (wiki "Creature Visuals"):
   - Raíz `Control` con hijos (unique names): **`Visuals`** (Node2D que contiene el modelo — usar un `SubViewport`/`Node3D` embebido o el truco estándar de mesh 2D), **`Bounds`** (Control, hitbox), `IntentPosition`, `CenterPos` (Marker2D).
3. En el AnimationPlayer, **renombrar/duplicar** los clips al contrato de BaseLib: `wait`→`idle` (loop), `attack_b`→`attack`, `spell`→`cast`, `damage_01`→`hurt` y `die`. Configurar las transiciones de vuelta a `idle` ("At End") o usar un `AnimationTree` con `AnimationNodeStateMachine`.
4. En `MashShielder.cs`, cambiar `CreateCustomVisuals()` por `CustomVisualPath => "res://MashShielder/character/mash_visuals.tscn"` (BaseLib convierte la escena a `NCreatureVisuals` automáticamente).

### Paso 3 — Publish

`dotnet publish` empaqueta la escena y el modelo en el `.pck`. Las señales de animación del juego (ataque/casteo/daño/muerte) ya las enruta BaseLib — sin código extra.

## Estado actual en el mod — ✅ ANIMACIONES ORIGINALES INTEGRADAS (2026-06-10)

El pipeline completo se ejecutó con éxito. **Mash usa sus animaciones reales de FGO en el mod**:

1. FBX animado exportado con la GUI (Animator `chr` + 19 clips → `assets/reference/extracted/800100_anim/`).
2. **Renderizador propio** (`tools/render_project/`): proyecto Godot que MegaDot ejecuta para renderizar cada clip a secuencias PNG con fondo transparente. Resolvió en el camino:
   - Cámara en el eje X (los puppets de FGO son planos mirando de costado).
   - **Cara en blanco**: FGO muestra/oculta ojos/boca/cejas escalando los huesos `joint_open_eye`, `joint_close_mouth`, etc. — el renderer los posa manualmente cada frame.
   - **Z-fighting facial**: el modelo mide 0.02 unidades; se escala ×1000 antes de renderizar.
   - **Root motion**: los ataques desplazan al personaje (dash); se cancela anclando el AABB poseado al del frame 0.
   - Ventanas de acción detectadas por hash de frames: attack útil = frames 27-53, hurt = 0-16.
3. Frames seleccionados en `MashShielder/MashShielder/character/frames/` (152 PNGs a 512px, ~12 MB): idle 78f@15fps loop, attack 27f@30fps, cast 30f@15fps, hurt 17f@30fps, die = hurt.
4. `mash_visuals.tscn` generado programáticamente: estructura NCreatureVisuals (Visuals/Bounds/markers) + `AnimatedSprite2D` con SpriteFrames + `mash_sprite.gd` (vuelve a idle al terminar cada animación, salvo die). BaseLib detecta el AnimatedSprite2D y enruta las señales del juego automáticamente.
5. `CustomVisualPath` apunta a la escena; publicado (pck 12.2 MB).

Para las otras formas (Ortinax `800150`, Paladín `800200`): repetir GUI export → copiar FBX a `tools/render_project/` → ajustar el script → renderizar → nuevas SpriteFrames. El NP (`treasureArms1-7`) puede renderizarse igual para una animación especial de las cartas NP.

### Ajustes pendientes de la fase de pruebas
- Escala/offset del sprite y `Bounds` (hitbox) — probablemente requieran retoque al verlo en el juego.
- La charagraph estática quedó en `images/character/mash_battle.png` como fallback.

## Plan B (si el FBX diera problemas en Godot)

1. **Frames renderizados**: [FateViewer](https://katboi01.github.io/FateViewer/) reproduce los modelos/animaciones de FGO en el navegador — grabar cada clip a secuencia de frames y armar un `AnimatedSprite2D` con `idle/attack/cast/hurt/die`. Menos fiel (raster), 100% confiable.
2. **Convertidor propio**: con el dump de curvas (`assets/reference/extracted/dump100/`) generar recursos `.anim`/glTF programáticamente. Máximo control, máximo esfuerzo.

## Créditos / fuentes

- [Atlas Academy](https://atlasacademy.io/) — hosting de los assets (`static.atlasacademy.io/JP/Servants/{id}/{id}`).
- [AssetStudioMod (aelurum)](https://github.com/aelurum/AssetStudio) — extracción/exportación.
- [FateViewer (katboi01)](https://github.com/katboi01/FateViewer) — visor de referencia de animaciones.
- [BaseLib Wiki — Creature Visuals](https://alchyr.github.io/BaseLib-Wiki/docs/scenes/creature-visuals.html) — contrato de animaciones de StS2.
