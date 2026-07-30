# Playbook: cómo crear un personaje de FGO para Slay the Spire 2

Proceso completo destilado del desarrollo de Mash Kyrielight (junio 2026). Seguir en orden.
Los scripts referenciados viven en `tools/` y son reutilizables tal cual o con cambios mínimos.

---

## 0. Prerequisitos (una sola vez por máquina)

- **.NET 9+ SDK** (`dotnet --list-sdks`).
- **MegaDot** (el Godot de Mega Crit, https://megadot.megacrit.com/) — la versión DEBE coincidir con la del juego (ver `release_info.json` en la carpeta del juego). En Windows ubicarlo en `MegaDot/` del repo (gitignorado). **En Linux: la versión de MegaDot está en la carpeta `MegaDotLinux/` del repositorio** — apuntar `GodotPath` de `Directory.Build.props` (en CADA proyecto: MashShielder y FGOCore) al binario de esa carpeta.
- **BaseLib + ModConfig** instalados en `mods/` del juego. ⚠️ La versión de BaseLib instalada debe ser EXACTAMENTE la del NuGet del proyecto (pinneada en el csproj) — si no coinciden: `ReflectionTypeLoadException` y el mod no carga.
- Templates: `dotnet new install Alchyr.Sts2.Templates`.
- **AssetStudioMod GUI + CLI** (aelurum/AssetStudio) en `tools/` — la GUI es obligatoria para un paso.
- Decompilar el juego una vez como referencia de API: `ilspycmd -p -o decompiled "...\data_sts2_windows_x86_64\sts2.dll"`.

## 1. Crear el proyecto

```
dotnet new alchyrsts2charmod -n <NombreMod> -o <repo>\<NombreMod> -M <Autor>
```
- Editar `Directory.Build.props`: ruta de MegaDot.
- Pinnear BaseLib en el csproj: `Version="X.Y.Z"` (la instalada en el juego).
- El `id` del manifest NO se cambia nunca.
- `dotnet build` debe fallar SOLO con errores STS001 de localización = setup correcto.

## 2. Conseguir los assets del servant (Atlas Academy, sin login)

ID del servant: buscar en https://apps.atlasacademy.io/db (Mash=800100; trajes tienen ID propio).

| Asset | URL |
|---|---|
| Catálogo del servant | `api.atlasacademy.io/nice/JP/servant/<collectionNo>?lore=true` (skills, NPs, trajes, charagraphs) |
| Bundle del modelo de batalla | `static.atlasacademy.io/JP/Servants/<id>/<id>` (UnityFS sin cifrar) |
| Atlas de texturas | `static.atlasacademy.io/JP/Servants/<id>/textures/<id>.png` |
| Charagraphs | `static.atlasacademy.io/JP/CharaGraph/<id>/<id>a.png` |
| Arte de CE (cartas) | `static.atlasacademy.io/JP/CharaGraph/<ceAssetId>/<ceAssetId>a.png` (512×875) |
| Iconos de skill | `static.atlasacademy.io/JP/SkillIcons/skill_XXXXX.png` |
| Iconos de item | `static.atlasacademy.io/JP/Items/<id>.png` |
| Catálogo de CEs | `api.atlasacademy.io/export/JP/basic_equip_lang_en.json` → extraer TSV con `tools/` |

⚠️ La API rate-limitea: `Start-Sleep -Milliseconds 800` entre requests en loops.

**Regla de LORE (obligatoria):** al investigar el lore/voz/kit de un personaje, buscá en fuentes
**japonesas Y chino simplificado a la vez para corroborar** (no una sola): JP = Wikipedia日本語,
TYPE-MOON Wiki/typemoon.wiki, ニコニコ大百科, pixiv百科, 円谷プロ (tokusatsu); 中文 = Mooncell `fgo.wiki`,
Moegirl `mzh.moegirl.org.cn`, Baidu百科, zh.wikipedia. El **japonés es la línea base** del que derivan
diseño y traducciones. Las **frases/voces se escriben ORIGINALES** (inspiradas en la personalidad
canónica, NO transcripciones del juego). Patrón reutilizable: el workflow por-personaje de
`fgo-voice-redesign` (1 agente JP+中文 por personaje → frases en JP/ES/EN/中文). El japonés se registra
en `docs/VOICE-LINES.md`.

**Descargar bundles**: `tools/fetch_fgo_bundle.ps1 -Ids <id>[,<id>...] [-Texture]` → baja a
`assets/reference/bundles/<id>.bundle`, verifica el magic `UnityFS` y rate-limitea (las bundles
están **gitignoradas** — son binarios descargables on-demand). Para hallar/verificar IDs:
`.claude/workflows` o la query `api.atlasacademy.io/basic/JP/svt/search?name=<nombre>`.

**Gotchas de assets (aprendidos con Tiamat, 2026-06-13):**
- **`manifest.json` nuevo**: AA ahora sirve `…/Servants/<id>/manifest.json` listando los paths, pero
  el patrón viejo `…/Servants/<id>/<id>` (sin extensión) SIGUE devolviendo el UnityFS — el fetch script vale.
- **Texturas en `/textures/`** (no la raíz) y un modelo puede tener **VARIOS atlas** (`<id>_01/_02/_03.png`).
  ⚠️ `render.gd:_setup_meshes` aplica UN atlas a todas las superficies → un modelo multi-atlas NO
  renderiza bien sin agregar mapeo surface→atlas (pendiente; bloquea p.ej. la forma Bestia de Tiamat).
- **Enemies/jefes**: una entidad puede tener DOS svtIds — uno `enemyCollectionDetail` (la "ficha",
  puppet a veces INCOMPLETO: solo wait/spell/damage) y otro `type=enemy` (la instancia de combate, clips
  completos). Verificar los AnimationClips reales (UnityPy `pip install unitypy`, o el pass `list`) antes
  de elegir. Caso Tiamat: 9935400=ficha (3 clips, sin attack) vs 9935410=combate (8 clips).
- **Jefes gigantes (`superGiant`)**: pueden SÍ tener puppet extraíble (Tiamat Beast 9935410 lo tiene);
  no asumir que no. La escala gigante la normaliza el renderer por `joint_head`.

## 3. Animaciones del modelo original (el pipeline estrella)

1. **Export GUI (manual, ~2 min por forma)**: AssetStudioGUI → Load `<id>.bundle` → seleccionar Animator `chr` + TODOS los AnimationClips → click derecho → **"Export Animator + selected AnimationClips"** → `assets/reference/extracted/<id>_anim/`. El FBX debe pesar MUCHO más que sin clips (Mash: 8-80MB). El CLI NO puede hacer este paso (v0.19).
2. **Renderizar** con `tools/render_project/` (proyecto Godot que corre MegaDot):
   - Editar `render.gd`: dict `SELECT` con las ventanas de frames por modelo (medirlas tras un primer render explorando), `CLIP_FOR` (wait→idle, attack_b→attack, spell→cast, damage_01→hurt).
   - `tools/render_all.ps1` orquesta: measure ×N formas (acumula crop común) → save ×N → copia al mod.
   - Conocimiento embebido en render.gd (NO retocar sin entender):
     - Cámara mira por el eje **X** (los puppets FGO son planos mirando de costado).
     - **Cara**: FGO muestra/oculta ojos/boca/cejas escalando huesos (`joint_open_eye`=1, `joint_close_eye`=0, `joint_close_mouth`=1, `joint_open_mouth`=0, cejas) — algunos modelos usan nombres alternativos (`joint_eyeA-D`, o un SELECTOR de expresión `joint_eye_normal/close/re/smile` + `joint_mouth_*` en los costumes de verano) cuya pose de reposo ya está bien. ⚠️ La visibilidad del ojo NO siempre es escala de hueso: si la cara muestra cejas/boca pero NO ojos, casi seguro es la POSE de la cabeza (ver gotcha del idle cabizbajo), no la cara — verificá renderizando el frame 0 antes de tocar `FACE_POSE`.
     - **Escala normalizada por el hueso de la cabeza** (15.0/head_raw.y) — los AABB de mesh mienten (capas/mascotas los inflan).
     - **Root motion**: solo se cancela el desplazamiento horizontal (anclar Z de la cabeza al frame 0); el vertical es animación real.
     - **Lienzo 2048 + crop común** medido con `get_used_rect()` → sin clipping y todas las formas alineadas (mismo offset de sprite).
     - Salida **WebP lossy 0.9** (¡PNG a 2× pesa 4-6×!).
     - Frames en blanco durante dashes = normal (ventanas con hash para detectar la acción real).
2.bis. **Gotchas de modelos nuevos** (aprendidos con Artoria 2026-06-12):
   - Algunos modelos nombran los clips con sufijo de ascensión (`wait_level_3`, no `wait`) — `_find_animation` de render.gd ya matchea por prefijo `clip + "_level"`.
   - **Props del NP dentro de la malla `weapon`** (no como malla separada): no se pueden ocultar por nodo — colapsarlos por HUESO con `HIDE_BONES` (escala 0.0001, mismo truco que FACE_POSE). Castoria: las 4 espadas teal cuelgan de `joint_weaponA-D` (`joint_sword` es el báculo). El pass `list` imprime CLIP/MESH/BONE para identificarlos.
   - **Elegir el clip de ataque mirando los snaps, no solo el probe**: attack_b/attack_a de la Berserker de verano son surfs aéreos con root motion salvaje (la figura sale del canvas); attack_q era el único a nivel de piso. `CLIP_OVERRIDE` por modelo.
   - Si un clip llena el canvas entero (espadas telequinéticas, piano de hielo del spell de 704720), la unión queda 2048×2048 = recorte muerto → `MEASURE_SKIP` esa anim (los props se cortan en el borde, la figura queda) o recortar la ventana ANTES de que aparezca el prop (precedente: espejo de Morgan).
   - **Idle (wait) que agacha la cabeza ⇒ "sin ojos la mayor parte del tiempo"** (aprendido con la Berserker de verano 704710/704720, 2026-06-13): su `wait` es un cabeceo que baja la cabeza ~94% del ciclo (el flequillo tapa los ojos); la cabeza solo está arriba en la costura del loop (`154→0`). Los ojos renderizan PERFECTO (verificar con frame 0/154) — no es bug de cara. Fix: `FRAMES_OVERRIDE` (lista explícita de frames por modelo/anim, gana sobre `SELECT`) re-ventanea el idle al tramo cabeza-arriba CRUZANDO `154→0` (p.ej. `[150,151,152,153,154,0,1,2,3,4]`): el save numera 000,001… en ese orden y loopea sin salto porque cruza el punto de loop natural del clip. El idle queda corto (10 frames) ⇒ bajarle la velocidad en el `.tres` (≈14fps) para un cabeceo calmo. El pass `list` ahora vuelca CLIP/MESH/**BONE**/EYETRACK/BLENDSHAPE (reproduce wait antes de volcar, si no el esqueleto da 0 huesos); el pass `debug` vuelca `RBONE`; existe un pass `faceexp` + `crop_face.gd` para barrer/ampliar la cara cuando el rig es raro. ⚠️ El regex de `Set-Pass` en `render_all_artoria.ps1` debe incluir TODOS los pases (incl. `faceexp`) o queda pegado en el último.
3. **SpriteFrames**: `tools/make_frames_tres.ps1` genera los `.tres` (idle loop **30fps**, attack 30, cast 15, hurt/die 30; die = hurt). ⚠️ El idle a 15fps (step 2) se ve ENTRECORTADO en modelos con pelo/capa fluidos (feedback de playtest con Morgan) — renderizar idle con step 1 y speed 30.
4. **Escena** `<char>_visuals.tscn`: Control + `Visuals`(Node2D) + AnimatedSprite2D (`flip_h = true`, el juego espeja al jugador) + script `mash_sprite.gd` (volver a idle al terminar) + Bounds + markers. BaseLib detecta el AnimatedSprite2D y rutea las señales con animaciones llamadas **idle/attack/cast/hurt/die**. Para centrar X no alcanza con `CAM_CENTER`: medir el centro alfa de todos los frames `idle`, aplicar el factor real de `process/size_limit` y la escala del sprite, e invertir la compensación por `flip_h`. Para apoyar los pies, usar `SpriteY = (sourceHeight/2 - alphaBottom) * min(1, sizeLimit/max(sourceSize)) * spriteScale`; recalcular también `Bounds`, `IntentPosition` y `CenterPos` con el alto importado. Cada forma con un lienzo distinto necesita su propio `SpriteX/SpriteY`.
5. **Cambio de forma en combate**: un `.tres` por forma + swap de `sprite.SpriteFrames` vía `NCombatRoom.Instance.GetCreatureNode(creature).FindChild("Sprite")` (ver `FormVisuals.cs`). ⚠️ `ResourceLoader.Load` sincrónico de un `.tres` con cientos de WebP congela el hilo de simulación y puede cortar el heartbeat multijugador. Usar siempre `ResourceLoader.LoadThreadedRequest(path, "SpriteFrames", useSubThreads: true)`. En solitario, `FormVisuals` precarga el grupo completo del personaje activo para que los cambios sean inmediatos; en co-op carga sólo la forma actual de cada jugador y deja cualquier forma alternativa bajo demanda para no agotar VRAM. La **starter relic** debe ejecutar `FormSwitch.Enter<FormaInicial>(null, creature, null)` en `BeforeCombatStartLate`: fija la pasiva inicial y dispara la carga sin contar como cambio de forma.
6. ⚠️ Tras el publish, **parchear los `.webp.import`** con `tools/patch_webp_imports.ps1`: `compress/mode=1`, `lossy_quality=0.85`, `mipmaps/generate=true`, límite general de 1024 y **`process/size_limit=768` para `character/frames*`**. Los frames RGBA8 con mipmaps ocupan memoria según sus dimensiones descomprimidas, no según el tamaño del PCK. Al cambiar el límite, multiplicar la escala previa por `oldEffectiveMax / newEffectiveMax` para conservar exactamente el tamaño visible; `oldEffectiveMax = min(sourceMax, oldLimit)`. Recalcular además el pivote X/Y cuando cambie el lienzo importado. Luego publicar de nuevo.

## 4. Arte de cartas (CEs oficiales)

1. TSV del catálogo: regex sobre `basic_equip_lang_en.json` → `collectionNo \t assetId \t nombre`.
2. **Matcheo temático**: workflow guardado `.claude/workflows/match-ce-art.js` (agentes en paralelo buscan CE por tema de carta, con dedup). O a mano con Grep sobre el TSV.
3. `tools/make_card_art.ps1 -MappingCsv <csv>`: descarga y recorta (franja superior, ratio carta) a `card_portraits/<id>.png` (500×380) + `big/` (1000×760).
4. Iconos de powers = iconos de skill FGO; reliquias = iconos de item (con `_outline` = silueta blanca del alpha): `tools/make_icons.ps1`. ⚠️ Para powers que son ESTADOS del juego original (Maldición, quemadura, veneno…) usar el icono de **estado** real (`static.atlasacademy.io/JP/BuffIcons/bufficon_XXX.png` — Curse = `bufficon_521`; sacarlo del campo `buffs[].icon` del JSON de un servant que lo aplique), NO un icono de skill: los jugadores de FGO reconocen el estado (feedback de playtest).
5. Pantalla de selección: bg = escena Control con arte atenuado; `char_select_*.png` = charagraph (+ gris para locked); icono/marker = recorte de cara con `tools/make_face_icons.ps1` (calibrar -FaceX/-FaceY mirando el resultado: el centro de la cara NUNCA es donde uno cree).
6. **Tienda y fogata** (si no se hace, sale el Ironclad placeholder): override `CustomMerchantAnimPath` y `CustomRestSiteAnimPath`. Mercader = Node2D + AnimatedSprite2D con anim `idle` (BaseLib la encuentra recursivamente; también acepta un `.png` directo). Fogata = igual PERO necesita `ControlRoot` (Control hijo directo) + `%Hitbox` (unique name); reticle y thought-bubbles se autogeneran del Hitbox. ⚠️ Estas escenas NO se espejan como el combate, y el mercader/fogata quedan a la DERECHA del personaje: usar `flip_h = true` (mirar a la derecha) y **negar el offset X** del sprite respecto al de combate (verificado con feedback de playtest: sin flip el personaje le da la espalda al mercader).
7. **Sólo arte oficial en el paquete**: no usar fanart de Pixiv, Danbooru ni redes sociales. Para
   cartas y pantallas elegir `CharaGraph`, CE, Command Card, iconos o capturas del cliente oficial de
   FGO servidos por Atlas Academy. Registrar `assetId`, nombre y foco de recorte en un CSV bajo
   `assets/reference/ce/`; `tools/make_card_art.ps1` acepta `cropX`/`cropY` opcionales para conservar
   la cara o el objeto principal sin recurrir a otra fuente.

## 4.5 Arquitectura: cómo encarar el próximo personaje

**Un mod por personaje + FGOCore como mod-librería compartido.** NUNCA un mod general con varios personajes:
- El pck por personaje pesa ~120 MB (frames de animación) — un mod general cargaría todo siempre.
- Un personaje con una excepción al registrarse tira abajo el mod entero (radio de explosión).
- Multijugador sincroniza mods por versión: mods chicos y estables = menos fricción.
- El `id` del manifest es inmutable: no hay vuelta atrás de un mod general.

**FGOCore** (`FGOCore/` en este repo) contiene lo compartido: Carga NP y niveles de NP,
**Críticos v2** (banco 0–100, reserva 50, ×1,5 por carta, `CritReady` y recompensa Quick
posresolución), formas, retención de Bloqueo, Maldición, Alzarse, Overcharge, atributos FGO,
Evasión, Sello de Habilidad, Certero/Sure Hit, vínculo por run y las cartas meme incoloras.
`FgoCombatState` guarda flags/contadores efímeros de turno o combate en powers ocultos sincronizados;
usar eso en vez de campos privados cuando guardar/cargar deba conservar el uso. Los hooks de preview
(`ModifyDamage*`, `ModifyBlock*`) sólo calculan: nunca consumen cargas.

⚠️ **PUBLICAR SIEMPRE JUNTOS**: FGOCore y los doce mods de personaje en la misma pasada. Un DLL
viejo contra una API nueva produce `MissingMethodException`/`ReflectionTypeLoadException` y el mod
puede omitirse silenciosamente. Los iconos core viven en su PCK; los modelos propios que extienden
clases core re-overridean sus rutas. Los manifests usan dependencias con `min_version`, los proyectos
referencian `FGOCore.dll` con `Private=false` y el orden es FGOCore primero. Migrar un modelo entre
mods cambia su ID y rompe runs activas que lo contengan.

Checklist del personaje nuevo: copiar `MashShielder/` como plantilla → cambiar id/nombres → borrar contenido Mash-específico → seguir §2-§4 para assets → mecánicas nuevas sobre las bases de FGOCore.

## 4.6 Diseño de pools (estilo JeanneAlter — estándar desde el rediseño v2, 2026-06-11)

Análisis fuente: `assets/reference/jeanne_anatomy.json` (anatomía del mod JeanneAlter, la
referencia del usuario) y `pools_audit.json`. Reglas que TODO pool nuevo cumple:

1. **Básicas de comando Buster/Arts/Quick** en cada personaje (1⚡: 10 daño / 6 + 30 NP /
   6 + 30 estrellas; arte = retrato `card_servant_1.png` de Atlas con 3 bandas de crop).
   Mazo inicial estilo QAABB sesgado a la identidad (Mash más Defender, Morgan más Buster).
2. **Conectividad ≥90% en comunes**: cada común lee o escribe ≥1 recurso propio. Las
   comunes son ENGRANAJES DE CONVERSIÓN, no vanilla-with-numbers. Pares espejo a 0⚡
   (50 NP ↔ 50 estrellas) garantizan que ningún medidor se estanque.
3. **Denominaciones fijas** 10/20/30/50/100 (básica=30, gate=50, umbral/payoff=100).
4. **Starter relic = motor**: convierte eventos universales en recursos del kit
   (golpe-totalmente-bloqueado→estrellas en Mash; perder-Vida→estrellas en Morgan),
   SIEMPRE con cap de 3 procs/turno (reset en `BeforeSideTurnStart` y sólo si
   `participants.Contains(Owner)`). Los riders del pool se calibran contra ese flujo garantizado.
5. **Glow dorado** (`ShouldGlowGoldInternal`) en TODA carta condicional — hace visibles
   los hilos en la mano.
6. **Los poderes engordan hilos existentes**, no abren nuevos.
7. **Pipeline**: diseño por panel (2+ propuestas con lentes distintas + jueces
   adversariales; los parches del juez MANDAN) → implementación por lotes de rareza →
   loc sync ("el código manda") → `tools/audit_simpleloc.ps1` → publish conjunto.
   Sacar una carta del pool sin romper saves: `CardRarity.Event` + comentario (borrar
   en la versión siguiente).

## 5. Gotchas de código (LA LISTA QUE DUELE — leer antes de escribir cartas)

| Gotcha | Detalle |
|---|---|
| **Semántica de hooks** | `ModifyDamageAdditive`/`ModifyBlockAdditive` = **DELTA** (default 0). `ModifyHpLost*`/`ModifyHandDraw`/`ModifyCardPlayCount` = **ABSOLUTO** (default devuelve el input). Devolver 0 "para no cambiar nada" en los absolutos ANULA TODO EL DAÑO del combate. Verificar SIEMPRE el default en `AbstractModel.cs`. |
| **PowerVar names** | `PowerVar<XPower>(n)` se llama `"XPower"` (typeof completo). Usar SIEMPRE el ctor con nombre: `new PowerVar<XPower>("X", n)` para que `!X!` en localización y `DynamicVars["X"]` funcionen. Los accessors azucarados (`.Vulnerable`) buscan `"VulnerablePower"`. |
| **BOM** | PowerShell 5.1 `Set-Content -Encoding utf8` escribe BOM → Godot no parsea `.tscn`/`.tres` EN RUNTIME ("Parse Error: Expected '['"; el import headless NO lo detecta, el log del juego SÍ). Escribir siempre con `[IO.File]::WriteAllText(..., UTF8Encoding($false))`. |
| **IDs con mayúsculas seguidas** | El splitter parte `QP`→`Q_P`, `IV`→`I_V`. Nombrar clases `InsufficientQp`, `FouBeastIv`. |
| **Localización de powers** | Necesitan `description` Y `smartDescription`. Diálogos del Architect van en `ancients.json`. Formato: claves planas con puntos. |
| **SimpleLoc** | `#texto` activa; `!Var!`=diff; `*Palabra` dorado termina en `[\s*.,|}]` ASCII → **en chino cerrar explícito** `*词*`; `-quitar-+agregar+` al mejorar; UN `(s)` plural por variable y por frase. ⚠️ **ESCAPES OBLIGATORIOS**: un `+`/`-` LITERAL se escribe `/+` `/-` (los PARES se interpretan como upgrade-swap y COMEN texto — bug real del vínculo: `（+2，精英+3）`→`{IfUpgraded...}`); un paréntesis tras `!var!` se escapa `/(` (dispara el pluralize). Correr `tools/audit_simpleloc.ps1` ANTES de cada publish (corre los regex reales del decompilado; los hits de pares de upgrade y `carta(s)` intencionales son la baseline). |
| **Bloqueo retenido** | El juego elige UN solo preventer de limpieza de Bloqueo → todos los preventers propios deben delegar en un helper de tope compartido (ver `BlockRetention`). |
| **GDScript** | `var x := DICT[key] + ...` no infiere tipo → tipar explícito. No pipear la corrida del renderer por `Select-Object -First` (mata el proceso a mitad de render). |
| **PS 5.1 + exes** | stderr de exes + `$ErrorActionPreference=Stop` + redirecciones = aborto espurio. Leer archivos UTF-8 con `-Encoding UTF8`. |
| **Enums del juego** | `CharacterGender`: Masculine/Feminine/Neutral. `RelicRarity`: sin "Boss" — los de jefe son `Ancient`. |
| **Cartas incoloras** | `[Pool(typeof(ColorlessCardPool))]` sobre una subclase de `CustomCardModel` → aparecen para cualquier personaje. |
| **VFX inexistente = carta congelada** | `WithHitFx("vfx/...")` con un path que no existe → NRE en `VfxCmd.PlayVfx` → la PlayCardAction aborta y la carta queda flotando en pantalla sin terminar de resolverse. Validar SIEMPRE contra el catálogo real: `grep '"vfx/' decompiled/` (no existe `vfx_attack_pierce`; para perforante usar `vfx_dramatic_stab`). |
| **Log del juego** | `%APPDATA%\SlayTheSpire2\logs\godot.log` — SIEMPRE el primer lugar para diagnosticar. Si una carta "se cuelga", buscar `completed with exception`. |
| **PowerShell aplana arrays** | `@(@("a","b"))` con UN solo par interior SE APLANA → `foreach ($pair in ...)` itera strings y `$pair[0]` es el PRIMER CARÁCTER (corrupción real: `额`→`外` ×13). Coma unaria obligatoria: `@(, @("a","b"))`. Y los .ps1 con CJK necesitan BOM para PS 5.1. |
| **Iconos de ESTADOS** | Powers que son estados del juego original (Maldición, quemadura…) usan `static.atlasacademy.io/JP/BuffIcons/bufficon_XXX.png` (sacarlo de `buffs[].icon` del JSON de un servant que lo aplique; Curse=521, estrellas=320, crit=325), NO SkillIcons — los jugadores de FGO reconocen el estado. |
| **Agentes y límite de sesión** | Los subagentes pueden morir por session-limit A MITAD de escritura → SIEMPRE inventariar archivos existentes antes de relanzar/reescribir; los workflows se retoman con `resumeFromRunId` (lo completado vuelve de caché). |

## 6. Patrones de implementación probados (copiar de este repo)

- **Recurso estilo Carga NP**: power contador + helper estático con tope (`NpCharge.cs`); cartas NP con `IsPlayable`/`ShouldGlowGoldInternal` + consumo total con escalado por tier (`ConsumeAllForNpCard`).
- **Generación de carta al llegar al máximo** (ulti): marcador re-armable removido al gastar (`CamelotManifestedPower` + `TryManifestUlt`).
- **Formas/stances**: power base con pasivas como flags + helper `Forms.Enter<T>` + swap de visual.
- **Intercepción** (counter al bloquear): `AfterDamageReceived` + `result.WasFullyBlocked`.
- **Cobertura multijugador**: `ModifyHpLostBeforeOsty` (anular HP del aliado) + `AfterDamageReceivedLate` (re-infligir a la tanque).
- **X-cost**: `HasEnergyCostX` + `ResolveEnergyXValue()`. Coste al mejorar: `EnergyCost.UpgradeBy(-1)`. Exhaust: `CanonicalKeywords => [CardKeyword.Exhaust]`.
- **Generar cartas**: `CombatState.CreateCard<T>(Owner)` + `UpgradeInternal()` + `AddGeneratedCardToCombat`. Descarte con selección: `CardSelectCmd.FromHandForDiscard`.
- **Sistema de 好感度/vínculo por run** (`MashBond.cs` + `BondPower.cs`): contador en una RELIQUIA starter (las reliquias viven toda la run; los powers mueren al fin del combate).
  - **Persistencia**: propiedad con `[SavedProperty]` (ns `MegaCrit.Sts2.Core.Saves.Runs`) + setter con `AssertMutable()` — sobrevive guardar/continuar. Patrón copiado de `BookOfFiveRings` del juego base.
  - **Contador visible en el icono**: `ShowCounter => true` + `DisplayAmount` (mostrar el NIVEL, no los puntos) + `InvokeDisplayAmountChanged()` en el setter.
  - **Fuentes de puntos**: `AfterCombatVictory(CombatRoom room)` con `room.RoomType` (Monster/Elite/Boss → +2/+3/+5) y `AfterRoomEntered(AbstractRoom room)` filtrando `RoomType.Event/Shop/RestSite` (+1). Victoria y entrada son hooks distintos → sin doble conteo.
  - **Bonos al subir de nivel** (inmediatos, p. ej. +Vida máx): en el mismo `AddPoints` comparando nivel antes/después; `CreatureCmd.GainMaxHp` funciona fuera de combate.
  - **Bonos de inicio de combate** (NP/Bloqueo inicial, capstone): `BeforeCombatStartLate()` leyendo el nivel actual; un power de display (`StackType.Counter`, stacks = nivel, sin hooks) para verlo en combate.
  - Umbrales calibrados a ~100 pts por run de 3 actos: `[5,12,20,30,40,52,64,76,88,100]` → Nv10 se alcanza recién al final.
  - **Multijugador**: el juego escala los monstruos a HP ×jugadores×1.1–1.3 (`MultiplayerScalingModel`, también bloqueo/powers enemigos) — los bonos DEFENSIVOS personales de un tanque deben escalar también: ×(1 + 0.5×(jugadores−1)) vía `Owner.RunState.Players.Count`. Los bonos ofensivos/de economía NO (la fuerza del equipo ya escala con la cantidad de jugadores).
- **REGLA — icono de la reliquia starter de mecánica**: SIEMPRE el icono de la clase del servant (Saber/Lancer/Shielder/…), en la variante que corresponda a sus estrellas de rareza en FGO: **1–3★ = bronce, 4★ = plata, 5★ = oro**. (Mash usa `Shieldergold.png`.)
- **Iconos del wiki fandom**: `static.wikia.nocookie.net` sirve **WebP aunque la URL diga .png** (GDI+ explota con "Parameter is not valid") — agregar `&format=original` a la URL. Los iconos de clase se llaman `<Clase><variante>.png` (p. ej. `Shieldergold.png`); buscarlos con la API: `fategrandorder.fandom.com/api.php?action=query&list=allimages&aiprefix=<Clase>`.

## 7. Localización en cinco idiomas

Carpetas obligatorias: `eng` (base), `esp` (español latino, distinto de `spa`=España), `zhs`
(chino simplificado), `kor` y `rus`. Mapa completo en `LocManager.cs` del decompilado. Ejecutar
`tools/audit_localization_parity.ps1` para paridad de archivos, claves y `!Variables!`, y después
`tools/audit_simpleloc.ps1`. Terminología CN: usar Mooncell (玛修, 宝具值, 格挡, 黑桶...).

## 8. Ciclo de trabajo

`dotnet build` = código y manifiesto a `dist/<Id>/`. `dotnet publish` = código + assets al PCK en
el mismo staging (necesario para CUALQUIER cambio no-código); no instala en el juego salvo el opt-in
explícito de deploy. Validar escenas con import/export headless, auditar el PCK y revisar
`godot.log` después del playtest. El warning MSB3077 del export de MegaDot puede ser benigno: manda
la presencia y contenido final del paquete.
