# Pipeline: animaciones originales de FGO → Slay the Spire 2

Investigado y validado el 2026-06-10; automatizado el 2026-07-28. Estado: **pipeline confirmado de punta a punta y reproducible por CLI**, incluido el FBX animado.

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

### Paso 1 — Exportar FBX con animaciones

El modo Animator del CLI oficial de AssetStudioMod 0.19 omite los `AnimationClip` aunque se soliciten. El helper `tools/AssetStudioAnimatorExport/` carga explícitamente `Animator`, `AnimationClip`, `Mesh` y `Texture2D`, y exporta el mismo resultado que la selección manual de la GUI:

1. Compilar una vez: `dotnet build tools/AssetStudioAnimatorExport/AssetStudioAnimatorExport.csproj -c Release`.
2. Ejecutar `AssetStudioAnimatorExport <bundle> <salida> [animator=chr]` desde su carpeta `bin/Release/net9.0`.
3. Resultado: `Animator/chr/chr.fbx` con todos los takes de animación del bundle. Para Mash base, por ejemplo, son 19 clips; Kagetora y Kenshin contienen 22 y 26 respectivamente.

La GUI sigue siendo útil para inspección, pero ya no forma parte obligatoria del pipeline.

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

Para las otras formas (Ortinax `800150`, Paladín `800200`): repetir el exportador CLI → copiar FBX a `tools/render_project/` → ajustar el script → renderizar → nuevas SpriteFrames. El NP (`treasureArms1-7`) puede renderizarse igual para una animación especial de las cartas NP.

## Capa de presentación fluida (2026-07-30)

`FGOCore/FGOCoreCode/Animation/FgoAnimationSmoothing.cs` se instala automáticamente sobre cualquier
`AnimatedSprite2D` cuyo `SpriteFrames.resource_path` pertenezca a uno de los 12 mods FGO. Conserva
los clips raster oficiales y añade dos tratamientos baratos y deterministas:

- persistencia muy tenue del fotograma anterior durante 55 ms (10% en reposo, 16% en acción), para
  reducir el parpadeo entre poses sin crear frames ni texturas nuevas;
- interpolación subpíxel en `Offset`: respiración suave en `idle` y anticipación mínima en
  `attack`/`cast`/`hurt`.

No cambia `speed`, cantidad de cuadros, `Position`, `Scale` ni los delays del modelo. Por eso no
alarga cartas/VFX y no compite con los pivotes de cambio de forma. La misma capa se adjunta desde el
factory endurecido a tienda/fogata; sus escenas reproducen únicamente `idle`. Ejecutar
`tools/audit_animations.ps1` valida perfiles, las 15 formas y la presentación 12/12.

## Kagetora/Kenshin — ✅ DOS FORMAS INTEGRADAS (2026-07-30)

- Fuentes oficiales de Atlas Academy: Nagao Kagetora `303800` (colección 252) y Uesugi Kenshin `901820` (colección 400, tercera ascensión). Hashes y transformaciones están en `assets/reference/kagetora_animation_sources.csv`.
- Clips comunes: `wait` → `idle`, `spell` → `cast` y `damage_01` → `hurt`; `die` reutiliza la reacción de daño. Kagetora usa `attack_q` (cuadros 12–47) y Kenshin una composición segura de `attack_a` (0–20 + 56–70), omitiendo el tramo intermedio donde el rig oficial se separa.
- Recorte compartido `2036×1712` y mismo plano de suelo. Pivotes finales: Kagetora `(-70,7; -316,1)` y Kenshin `(37,4; -303,6)` tras el límite de importación a 768 px.
- Cada forma tiene 153 cuadros: `idle` 78, `attack` 36, `cast` 22 y `hurt/die` 17. El auditor acepta ambos perfiles sin errores; en Kenshin quedan cuatro avisos esperados porque sus armas largas rozan el borde en algunas poses.
- La forma inicial se instala al comenzar combate y el primer NP cambia de manera permanente a Kenshin mediante `FGOCore.FormSwitch`; el cambio sustituye también el `SpriteFrames` y el pivote.
- Los 306 cuadros usan compresión VRAM, mipmaps y `size_limit=768`. El PCK publicado localmente ocupa 41.885.012 bytes.

## Astolfo Rider — ✅ INTEGRADO (2026-07-29)

- Fuente oficial de Atlas Academy: Rider `400400`, colección 094. El exportador recupera 17 clips
  del Animator `chr` y el atlas oficial.
- Clips usados: `wait` → `idle`, `attack_q` → `attack`, `spell` → `cast`, `damage_01` → `hurt`;
  `die` reutiliza la reacción de daño.
- Secuencias finales: idle 78 cuadros a 30 FPS, ataque 55 a 48 FPS, cast 30 a 20 FPS y hurt/die 17
  a 30 FPS. El ataque acelerado conserva anticipación, estocada y recuperación en 1,15 segundos.
- Recorte `1513×1010`, pivote de escena `(-102, -229)` y límite de importación de 768 px. La lanza
  oficial sale del lienzo en ocho cuadros del barrido; el cuerpo permanece completo y el auditor lo
  registra como un único aviso esperado, con 0 errores.
- Los 180 cuadros y las escenas de combate, selector, mercader y descanso están dentro del PCK final
  optimizado de 30.868.616 bytes. El perfil reproducible es `400400` en
  `tools/animation_manifest.json` y el wrapper es `tools/render_all_astolfo.ps1`.

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
