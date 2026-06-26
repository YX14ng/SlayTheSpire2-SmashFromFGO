# FINDINGS — hallazgos técnicos verificados

Conclusiones de alta densidad (no historial). **Verificado** = visto en código/log/decompilado;
lo no verificado se marca *(probable)* / *(a confirmar)*. Decisiones cerradas → [DECISIONS.md](DECISIONS.md).

## v0.107.1 — cambios de API (verificados con ilspycmd sobre sts2.dll)
- Hooks de inicio de turno: `AfterSideTurnStart(CombatSide, IReadOnlyList<Creature> participants, ICombatState)`; `BeforeSideTurnStart` igual +participants.
- `AfterPowerAmountChanged` gana `PlayerChoiceContext choiceContext` **primero**.
- `AfterTurnEnd`/`BeforeTurnEnd` **eliminados** → `AfterSideTurnEnd`/`BeforeSideTurnEnd(PlayerChoiceContext, CombatSide, IEnumerable<Creature>)`.
- `PowerCmd.Apply<T>` / `ModifyAmount` toman `PlayerChoiceContext` primero.
- `CardPileCmd.AddGeneratedCardToCombat(card, PileType, Player? creator, pos)` (antes `addedByPlayer`; `CardModel.Owner` ES el Player).
- `AttackCommand.Results` → `IEnumerable<List<DamageResult>>`; `CombatState : ICombatState`.
- **Regla de contexto**: si el hook da `choiceContext`, pasalo; si no, `new BlockingPlayerChoiceContext()`; nullable → `choiceContext ?? new BlockingPlayerChoiceContext()`.

## Gotchas de build (Tiamat, v0.107.1)
- `PileType` válido: `None, Draw, Hand, Discard, Exhaust, Play, Deck` (NO existe `DrawPile`/`DiscardPile`).
- `Curses.MostCursed` (y similares de FGOCore) toman `CombatState` concreto; `Creature.CombatState` es `ICombatState` → castear `(CombatState)Owner.Creature.CombatState`. Mismo patrón en `LahmuSwarmPower`.
- Cada `PowerModel` necesita loc `title/description/smartDescription` o el build tira `STS001`.

## Deploy / entorno (CRÍTICO)
- Install en uso: `G:\SteamLibrary\steamapps\common\Slay the Spire 2` (v0.107.1). **El juego se movió de biblioteca Steam (C:→G:) el 2026-06-25**; el viejo `C:\Program Files (x86)\Steam\...` quedó con restos (solo `mods/` + `window_state.json`, sin `data_sts2_windows_x86_64`). El build apunta a G: vía `Sts2Path` en los `Directory.Build.props` (machine-local, gitignored); `tools/install-mod.ps1` usa G: por default. Workshop ahora en `G:\SteamLibrary\steamapps\workshop\content\2868840\`.
- El tool **Bash corre SANDBOXEADO** con overlay de FS → para tocar el FS real usar `dangerouslyDisableSandbox:true` (Bash) o la tool PowerShell.
- dotnet **10.0.301** (`/c/Program Files/dotnet/`); MegaDot 4.5.1 en el repo; `ilspycmd` en `/c/Users/YX14n/.dotnet/tools/`.
- **godot.log**: `C:\Users\YX14n\AppData\Roaming\SlayTheSpire2\logs\godot.log` — PRIMER lugar a diagnosticar un mod que no carga.

## VRAM / preload (base del mod de optimización)
- BaseLib carga assets vía `PreloadManager.Cache` (`GetTexture2D`/`GetScene`/`GetAsset`); los modelos declaran qué necesitan con **`GetAssetPaths(IRunState runState)`** (virtual del juego que BaseLib overridea).
- El parámetro `IRunState` ⇒ la carga está pensada para ser **por-run** → palanca para "cargar solo el personaje elegido" (la idea del mod de optimización). *(A confirmar: si hoy precarga TODOS en el menú (eager) o ya por-run.)*
- Síntoma medido: **~120 mods → VRAM 4.7GB en el menú, 320s de arranque**.
- **CONFIRMADO (2026-06-26, reportes de players + código)**: `FormVisuals.Apply` (FGOCore) llamaba `PreloadAll()` que cargaba en VRAM **TODAS las formas de TODOS los mods FGO instalados** (`Registered` es un flat-list de todos los `RegisterFrames` de cada char) a un `Cache` **estático nunca liberado** — aunque jugaras un solo char. Con N chars FGO instalados ⇒ N×(cientos de frames) pinneados. Síntomas de players: en GPU débil las texturas de cartas/intención/NP **no se alocan → "solo barras de vida"** (injugable); con 3 chars instalados → **crash** (掉帧→闪退); multi = "massive performance issues". El usuario (RTX 4080/16GB) NO lo reproduce. **Fix**: `FormVisuals` agrupa por-char (`RegisterFrames` = 1 grupo) y `Apply` solo precarga el grupo del char que pelea (`PreloadGroup`), no todos.
- **Textura de frames**: `compress/mode=1` es *lossy de DISCO* pero **descomprime a RGBA8 en VRAM** (`"vram_texture": false`) → VRAM = nº_frames × (lado² × 4 × 1.33 mipmaps). A 1024 ≈ 5.6MB/frame; **Mash tenía `size_limit=0` (sin cap, ~1900px ≈ 19MB/frame ≈ 6GB solo Mash)**. **Fix**: capear los frames de los 4 animados a `process/size_limit=768` (~44% menos VRAM vs 1024; ~6× para Mash). Cambiar el `.import` re-importa solo (no hay que borrar `.godot/imported`); el `.pck` baja como verificación.
- **NO usar `compress/mode=2` (VRAM Compressed)**: guarda BC7 en DISCO → infla el `.pck`/descarga de Workshop ~10×. `size_limit` baja VRAM **y** disco. (`mode=4` Basis = chico en ambos pero con pérdida; no probado.)
- ⚠️ Un **re-render** regenera los `.import` con el default (sin cap) → hay que **re-capear los frames a 768** (cf. Mash, que quedó sin cap).

## Mods de OTROS autores rotos (desuscribir) — 2026-06-25
- **Crash al iniciar run**: NRE en `HsrSimulatedUniverseCurios` (`CarnivalsTailPatch.OnRelicObtained`) disparado por `ReAstralPartyMod` (reliquia inicial). Workshop `3747553484` / `3747579249`.
- **Downfall/Automaton** (`3747508091`): `HarmonyException` al cargar — no puede patchear `OnPlay` en v0.107.1.
- **LittleWizard** (`3747560296`): arte de carta roto (`element_burst.png`).

## Pipeline de arte CE
- `.claude/workflows/match-ce-art.js`: batches `[file, themeEn]` → CE `collectionNo` (catálogo `assets/reference/ce/ce_names.tsv`, 2611 CEs, formato `collectionNo<TAB>assetId<TAB>name`).
- Aplicar: mapear `collectionNo`→`assetId` (col 2) → `tools/make_card_art.ps1 -MappingCsv <csv> -OutDir <mod>/images/card_portraits` baja de Atlas Academy y recorta a 500×380 + `big/` 1000×760.
