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

## 3. Animaciones del modelo original (el pipeline estrella)

1. **Export GUI (manual, ~2 min por forma)**: AssetStudioGUI → Load `<id>.bundle` → seleccionar Animator `chr` + TODOS los AnimationClips → click derecho → **"Export Animator + selected AnimationClips"** → `assets/reference/extracted/<id>_anim/`. El FBX debe pesar MUCHO más que sin clips (Mash: 8-80MB). El CLI NO puede hacer este paso (v0.19).
2. **Renderizar** con `tools/render_project/` (proyecto Godot que corre MegaDot):
   - Editar `render.gd`: dict `SELECT` con las ventanas de frames por modelo (medirlas tras un primer render explorando), `CLIP_FOR` (wait→idle, attack_b→attack, spell→cast, damage_01→hurt).
   - `tools/render_all.ps1` orquesta: measure ×N formas (acumula crop común) → save ×N → copia al mod.
   - Conocimiento embebido en render.gd (NO retocar sin entender):
     - Cámara mira por el eje **X** (los puppets FGO son planos mirando de costado).
     - **Cara**: FGO muestra/oculta ojos/boca/cejas escalando huesos (`joint_open_eye`=1, `joint_close_eye`=0, `joint_close_mouth`=1, `joint_open_mouth`=0, cejas) — algunos modelos usan nombres alternativos (`joint_eyeA-D`) cuya pose de reposo ya está bien.
     - **Escala normalizada por el hueso de la cabeza** (15.0/head_raw.y) — los AABB de mesh mienten (capas/mascotas los inflan).
     - **Root motion**: solo se cancela el desplazamiento horizontal (anclar Z de la cabeza al frame 0); el vertical es animación real.
     - **Lienzo 2048 + crop común** medido con `get_used_rect()` → sin clipping y todas las formas alineadas (mismo offset de sprite).
     - Salida **WebP lossy 0.9** (¡PNG a 2× pesa 4-6×!).
     - Frames en blanco durante dashes = normal (ventanas con hash para detectar la acción real).
3. **SpriteFrames**: `tools/make_frames_tres.ps1` genera los `.tres` (idle loop **30fps**, attack 30, cast 15, hurt/die 30; die = hurt). ⚠️ El idle a 15fps (step 2) se ve ENTRECORTADO en modelos con pelo/capa fluidos (feedback de playtest con Morgan) — renderizar idle con step 1 y speed 30.
4. **Escena** `<char>_visuals.tscn`: Control + `Visuals`(Node2D) + AnimatedSprite2D (`scale 0.5`, `flip_h = true` ¡el juego espeja al jugador!, posición calculada de GROUND_ROW/CAM_CENTER impresos por el renderer) + script `mash_sprite.gd` (volver a idle al terminar) + Bounds + markers. BaseLib detecta el AnimatedSprite2D y rutea las señales con animaciones llamadas **idle/attack/cast/hurt/die**.
5. **Cambio de forma en combate**: un `.tres` por forma + swap de `sprite.SpriteFrames` vía `NCombatRoom.Instance.GetCreatureNode(creature).FindChild("Sprite")` (ver `FormVisuals.cs`). ⚠️ `ResourceLoader.Load` sincrónico de un `.tres` con ~150 webp (~35 MB) congela el juego varios segundos en el primer swap — precargar TODAS las formas con `ResourceLoader.LoadThreadedRequest(path, "SpriteFrames", useSubThreads: true)` y pinnearlas en un caché estático. ⚠️⚠️ **El disparador importa** (bug de Morgan v2): `FormVisuals.PreloadAll` corre recién en el primer `Apply()` = el primer cambio REAL igual se trababa. Resolución: la **starter relic** hace `FormSwitch.Enter<FormaInicial>(null, creature, null)` en `BeforeCombatStartLate` — eso (a) fija la forma inicial (sin esto el personaje pelea SIN forma activa hasta el primer cambio: pasiva muerta, bug silencioso), (b) dispara la precarga al ARRANCAR el primer combate, y como `source == null` no cuenta como "cambio de forma" para los bonus de primer cambio.
6. ⚠️ Tras el publish, **parchear los `.webp.import`** con `tools/patch_webp_imports.ps1`: `compress/mode=1`, `lossy_quality=0.85`, `mipmaps/generate=true` **y `process/size_limit=1024`** — sin comprimir el pck infla (Mash: 206MB→122MB), y sin size_limit las texturas a resolución completa (~1900px) comen **~1.5 GB de VRAM por personaje** (3 formas pinneadas) y producen micro-trabas en el playback. Con size_limit la textura se achica (factor `1024/cropW`) → **ajustar `scale` del sprite en los .tscn**: `scale = 0.5 / (1024/cropW)` (Morgan: 0.9326); las POSICIONES no cambian (el offset en mundo se conserva). Luego publicar de nuevo.

## 4. Arte de cartas (CEs oficiales)

1. TSV del catálogo: regex sobre `basic_equip_lang_en.json` → `collectionNo \t assetId \t nombre`.
2. **Matcheo temático**: workflow guardado `.claude/workflows/match-ce-art.js` (agentes en paralelo buscan CE por tema de carta, con dedup). O a mano con Grep sobre el TSV.
3. `tools/make_card_art.ps1 -MappingCsv <csv>`: descarga y recorta (franja superior, ratio carta) a `card_portraits/<id>.png` (500×380) + `big/` (1000×760).
4. Iconos de powers = iconos de skill FGO; reliquias = iconos de item (con `_outline` = silueta blanca del alpha): `tools/make_icons.ps1`. ⚠️ Para powers que son ESTADOS del juego original (Maldición, quemadura, veneno…) usar el icono de **estado** real (`static.atlasacademy.io/JP/BuffIcons/bufficon_XXX.png` — Curse = `bufficon_521`; sacarlo del campo `buffs[].icon` del JSON de un servant que lo aplique), NO un icono de skill: los jugadores de FGO reconocen el estado (feedback de playtest).
5. Pantalla de selección: bg = escena Control con arte atenuado; `char_select_*.png` = charagraph (+ gris para locked); icono/marker = recorte de cara con `tools/make_face_icons.ps1` (calibrar -FaceX/-FaceY mirando el resultado: el centro de la cara NUNCA es donde uno cree).
6. **Tienda y fogata** (si no se hace, sale el Ironclad placeholder): override `CustomMerchantAnimPath` y `CustomRestSiteAnimPath`. Mercader = Node2D + AnimatedSprite2D con anim `idle` (BaseLib la encuentra recursivamente; también acepta un `.png` directo). Fogata = igual PERO necesita `ControlRoot` (Control hijo directo) + `%Hitbox` (unique name); reticle y thought-bubbles se autogeneran del Hitbox. ⚠️ Estas escenas NO se espejan como el combate, y el mercader/fogata quedan a la DERECHA del personaje: usar `flip_h = true` (mirar a la derecha) y **negar el offset X** del sprite respecto al de combate (verificado con feedback de playtest: sin flip el personaje le da la espalda al mercader).
7. **Fanart (Danbooru, sin login)**: `posts.json?tags=<personaje>+rating:g+order:score&limit=200&page=N` (máx 2 tags reales; `solo` ya no entra → filtrar client-side con `tag_string -match '\bsolo\b'` y `tag_string_character`). ⚠️ Paginar la query ordenada por score, NO la default (default = más recientes). Acreditar al artista en `assets/reference/fanart/CREDITS.txt`.

## 4.5 Arquitectura: cómo encarar el próximo personaje

**Un mod por personaje + FGOCore como mod-librería compartido.** NUNCA un mod general con varios personajes:
- El pck por personaje pesa ~120 MB (frames de animación) — un mod general cargaría todo siempre.
- Un personaje con una excepción al registrarse tira abajo el mod entero (radio de explosión).
- Multijugador sincroniza mods por versión: mods chicos y estables = menos fricción.
- El `id` del manifest es inmutable: no hay vuelta atrás de un mod general.

**FGOCore** (`FGOCore/` en este repo) contiene lo compartido: sistema de Carga NP (eventos `NpCharge.GaugeFilled`/`GaugeDropped` para que cada personaje enganche su ulti; `INpCostWaiver` para "primer NP gratis" — los waivers EXCLUYEN cartas Event y resuelven a tier mínimo), **Estrellas de Crítico** (`CritStarsPower`: a 100 se auto-pagan y dan 1 `CritReadyPower` = próximo Ataque ×2, un stack por carta), sistema de formas (`FormPower` base con `FramesPath`/`IsPermanent`, `FormSwitch.Enter<T>`, `IFormChangeListener`, `FormVisuals.RegisterFrames` + precarga), retención de Bloqueo (`BulwarkPower` + `BlockRetention` + `IBlockRetentionSource`), Maldición (`CursePower` cap 25 + `Curses` + `ICurseAmplifier`/`ICursePreserver`), Alzarse (`GutsPower` + `IGutsFloorBooster`), dupes/NP level (`INpLevelStore` + `NpLevels.Scale` +15%/nivel + botón Invocar), `OverchargeBlessingPower`, sistema de 好感度 (`BondRelic` abstracto con curvas/capstone virtuales + `BondPower` **+ `ServantDamage/BlockMultiplier` ×1.25 global** — el techo del entorno modded, skill §1.bis), y las cartas meme incoloras (viven SOLO acá — no copiarlas a mods de personaje). ⚠️ **PUBLICAR SIEMPRE JUNTOS**: FGOCore y TODOS los mods de personaje en la misma pasada — las firmas de FGOCore cambian (ej. `NpCharge.CanPay` ganó un parámetro) y un dll de personaje viejo contra un FGOCore nuevo tira `MissingMethodException` en runtime. Los iconos de los powers core (NP, Baluarte, vínculo, FormShifted) y el arte de memes viven en el pck de FGOCore; los powers/reliquias del personaje que extienden clases core deben re-overridear las rutas de imagen a su propio mod (ver `MashFormPower`/`MashBond`). Los mods de personaje declaran `"dependencies": ["BaseLib", "FGOCore"]` y referencian `FGOCore.dll` con `Private=false` (el dll lo distribuye solo FGOCore; el loader del juego resuelve por nombre). **Orden de build: FGOCore primero** (Mash referencia el dll publicado en `mods/`). ⚠️ Migrar modelos entre mods cambia su ID (prefijo) — los saves de runs A MEDIO TERMINAR que contengan esas cartas/powers se rompen; terminar la run antes de actualizar.

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
   SIEMPRE con cap de 3 procs/turno (reset en `AfterSideTurnStart` lado jugador = cubre
   la ronda entera). Los riders del pool se calibran contra ese flujo garantizado.
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

## 7. Localización trilingüe

Carpetas: `eng` (base), `esp` (¡español **latino**! distinto de `spa`=España), `zhs` (chino simplificado). Mapa completo en `LocManager.cs` del decompilado. Verificar paridad de claves contra eng al terminar (script de comparación con `ConvertFrom-Json`). Terminología CN: usar el wiki Mooncell (玛修, 宝具值, 格挡, 黑桶...).

## 8. Ciclo de trabajo

`dotnet build` = solo código (rápido, el analizador valida localización). `dotnet publish` = código + assets al pck + copia a `mods/` (necesario para CUALQUIER cambio no-código). Validar escenas: import headless del proyecto del mod + revisar el log del juego tras probar. El warning MSB3077 del export de MegaDot es benigno.
