# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Custom character mods for **Slay the Spire 2** (Early Access, Godot 4 / C#), building **Fate/Grand Order Servants** as playable characters. ("Smash" in the repo name is a pun on Mash, the first character — no FGO Servant is literally named Smash.) Mods target the **MAIN branch** of the game (v0.103.x), not the beta — BaseLib ≥ 3.1.8 is the main-compatible line (currently pinned to **3.2.1**).

The repo is a **multi-mod monorepo**: one shared mechanics library (`FGOCore/`) and many character mods that depend on it. Each top-level folder with a `*.csproj` is an independent mod with its own manifest, assets, and localization.

| Folder | Mod id (manifest) | Servant | Status |
|---|---|---|---|
| `FGOCore/` | `FGOCore` | shared library (no character) | required by all; **build/publish FIRST** |
| `MashShielder/` | `MashShielder` | Mash Kyrielight (Shielder) | complete + animated |
| `MorganBerserker/` | `MorganBerserker` | Morgan (Berserker→Caster) | complete + animated |
| `ArtoriaCaster/` | `ArtoriaCaster` | Artoria (Caster) | complete + animated |
| `MordredSaber/` | `MordredSaber` | Mordred (Saber of Red) | implemented |
| `GilgameshArcher/` | `GilgameshArcher` | Gilgamesh (Archer) | implemented |
| `OkitaSaber/` | `OkitaSaber` | Okita Souji (Saber) | implemented |
| `OberonPretender/` | `OberonPretender` | Oberon (Pretender) | implemented |
| `SiegfriedSaber/` | `SiegfriedSaber` | Siegfried (Saber) | implemented |
| `Tiamat/` | `TiamatBeast` | Tiamat (Beast) | in progress (folder ≠ project name) |

> **Estado + orden de lectura** (método adoptado de `iryuko/sts2-mod-dev`): al retomar una sesión, leé PRIMERO [docs/STATUS.md](docs/STATUS.md) (estado vivo, sobreescribe la tabla de arriba), [docs/DECISIONS.md](docs/DECISIONS.md) (reglas cerradas, no re-discutir) y [docs/FINDINGS.md](docs/FINDINGS.md) (hallazgos verificados). Las conclusiones van a esos docs de alta densidad, **no al chat**; marcá lo no verificado como *(probable)*/*(a confirmar)* y no escribas especulación como hecho. [docs/HANDOFF.md](docs/HANDOFF.md) queda como handoff histórico/cross-máquina.

The user communicates in Spanish — **respond in Spanish**. Card/mechanic names are authored in Spanish with English (and zhs) localization.

**Lore de personajes (regla):** investigá siempre en fuentes **japonesas Y chino simplificado a la vez** (Wikipedia日本語/TYPE-MOON Wiki + Mooncell `fgo.wiki`/Moegirl) para corroborar; el **japonés es la línea base** de diseño y traducción. Las frases/voces se escriben **ORIGINALES** (no transcripciones del juego); el JP queda en [docs/VOICE-LINES.md](docs/VOICE-LINES.md). Detalle en WORKFLOW-FGO §2.

## Architecture

### FGOCore — the shared mechanics library
`FGOCore/FGOCoreCode/` holds engine-level mechanics that every character reuses by **subclassing base powers/relics** and overriding image paths and tuning (see `MashFormPower`, `MashBond`). Core icons live in the FGOCore `.pck`; character subclasses re-override the image routes. Major subsystems (one folder each): `Np` (Carga NP / Overcharge gauge with `GaugeFilled`/`GaugeDropped` events, `Max=300`, `ManifestThreshold=100`), `Forms` (`FormPower`/`FormSwitch`/`FormVisuals` stance switching with preload), `Block` (Baluarte retention via `IBlockRetentionSource`), `Bond` (好感度 `BondRelic` abstract — `ServantDamageMultiplier`/`ServantBlockMultiplier`/`ServantRegenPerTurn` lifts inherited by every Servant), `Stars` (crit stars → `CritReadyPower`), `Curses`, `DragonScales`, `Lahmu`, `Memes` (colorless FGO meme cards), `Extensions`. Cross-cutting interfaces let relics/cards hook the engine: `ILimitBreaker` (Holy Grail — raise level caps), `IGutsFloorBooster`, `IFormChangeListener`, `INpLevelStore`/`INpCard` (dupe mechanic). `MainFile.cs` is the `[ModInitializer]` and runs `Harmony.PatchAll()`.

### Character mod anatomy
Each character is a Godot mod project: `<Name>.csproj` + `<Name>.json` (manifest) + `project.godot` + an inner `<Name>/` asset folder (`character/`, `images/{card_portraits,powers,relics}/` with `big/` variants, `localization/<lang>/*.json`, `mod_image.png`) + `<Name>Code/` C# source. The csproj wires the dependency two ways that must agree: a `<Reference Include="FGOCore">` with **`Private=false`** (compile against the published `mods/FGOCore/FGOCore.dll`, don't redistribute it) and `"dependencies": ["BaseLib","FGOCore"]` in the manifest (loader resolves it by name). The game install's other character mods (e.g. `JeanneAlter`) are structural references; `decompiled/` is the decompiled game used to find/verify base classes and VFX paths.

### CRITICAL: publish all mods together
When FGOCore's public API changes (e.g. a method signature like `NpCharge.CanPay` or `BondRelic`), **every dependent character dll must be republished in the same batch** — an old character dll against a new FGOCore throws `MissingMethodException` / `ReflectionTypeLoadException` and the mod silently fails to load. Never ship FGOCore alone.

## Build & deploy

Requires **.NET 9 SDK** and a **MegaDot 4.5.1** export binary (the game won't load a `.pck` exported by a newer Godot). Machine-local paths (`GodotPath`, `Sts2Path`) live in each project's `Directory.Build.props`, which is **gitignored** — `Sts2PathDiscovery.props` autodetects when possible. There is no `.sln`; build/publish each project from its own directory.

- **Staging (separación workspace/juego, estilo `iryuko/sts2-mod-dev`):** build/publish copian a la **staging del repo `dist/<Name>/`**, NUNCA a la carpeta del juego. Las mods de personaje referencian FGOCore desde `dist/FGOCore/FGOCore.dll` (`$(StagingPath)`), así el build NO depende de tener FGOCore en el juego (solo de buildear FGOCore primero). `dist/` está gitignoreado.
- **Build** (`dotnet build`) compila código y copia `.dll/.pdb/.json` a `dist/<Name>/` (target `CopyToModsFolderOnBuild`). Usar cuando cambiaste *solo* C#.
- **Publish** (`dotnet publish -c Release`) compila el dll **y** corre MegaDot headless `--export-pack` para el `.pck`, todo a `dist/<Name>/`. **Cualquier cambio no-código (loc JSON, imágenes, escenas) requiere Publish, no Build.**
- **Instalar al juego:** `tools/install-mod.ps1 -Mod <Id>` (o `-All`) copia `dist/<Id>/` → la `mods/` del juego; `-Clean` saca todos los mods FGO (restaura Workshop-only). Es la ÚNICA vía a la carpeta del juego. Atajo sin script: build/publish con `/p:DeployToGame=true`.
- **Orden:** buildear/publicar `FGOCore` primero (a `dist/`), después los personajes. El build ya NO necesita el juego cerrado (va a `dist/`); solo **instalar** al juego pide el juego cerrado si ese mod está cargado.
- **Game log is the first place to diagnose** a non-loading mod (`user://logs/godot.log`). A "mod X won't load" symptom can actually be a *different* mod crashing — read the log before assuming.
- First build of a new character template emits localization errors — fix via the `Alchyr.Sts2.ModAnalyzers` "Generate localization" quick-fix and move strings into the localization JSON.

## Asset pipeline (`tools/`)
FGO battle models are **Unity 2D puppets / 3D FBX rigs, NOT frame spritesheets** — animations are re-rigged and rendered in Godot. `tools/render_project/render.gd` defines per-Servant render windows (the gotcha-prone part). Reusable PowerShell scripts: `render_all*.ps1` (render frames), `make_*_frames_tres.ps1` (Godot SpriteFrames), `make_*_icons.ps1` / `make_card_art.ps1` (icons + CE art crops), `scaffold_fgo_character.ps1` (new character skeleton). `assets/reference/` holds Atlas Academy art/atlases and balance baselines. CE-art matching: `.claude/workflows/match-ce-art.js` + the `match-ce-art` skill.

## Where to read more
- [docs/WORKFLOW-FGO.md](docs/WORKFLOW-FGO.md) — **the playbook**: end-to-end process for building an FGO character (pipeline, renderer, art matching, the gotcha table). Read FIRST for any new character or pipeline change.
- [docs/HANDOFF.md](docs/HANDOFF.md) — live pending tasks + cross-machine handoff (NP cap 300, dupe mechanic, Holy Grail, balance passes).
- [docs/MODDING.md](docs/MODDING.md) — toolchain, mod format, BaseLib API, resource links.
- [docs/ANIMATIONS.md](docs/ANIMATIONS.md) / [docs/ANIM-TODO.md](docs/ANIM-TODO.md) — animation pipeline + remaining fine-tuning.
- `docs/DESIGN.md` (Mash) and `docs/DESIGN-<NAME>.md` / `docs/REDESIGN-<NAME>.md` — per-character mechanics, card pools, relics. Design any new card/character/relic with the [.claude/skills/sts2-mechanics-design](.claude/skills/sts2-mechanics-design/SKILL.md) skill (it has the real vanilla balance baselines).

## Key facts & gotchas
- StS2 runs on **MegaDot** (Mega Crit's Godot 4 fork). Mods = `<Id>.json` + `<Id>.dll` + `<Id>.pck` in the game's `mods/` folder. The standard framework is **BaseLib** (NuGet `Alchyr.Sts2.BaseLib`, docs https://alchyr.github.io/BaseLib-Wiki/): `CustomCharacterModel`/`CustomCardModel`/`CustomRelicModel`, custom keywords/enums, localization, automatic ID prefixing.
- Game install: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`. BaseLib + ModConfig are installed there; the BaseLib version in `mods/` must EXACTLY match the csproj pin (3.2.1) or the mod won't load.
- A mod's manifest `id` **must never change** once chosen — it determines the loaded filenames. Model/power IDs must never be renamed while saves are active (the mod prefix is part of the ID); migrating a mechanic between mods changes its ID and breaks in-progress runs.
- `PowerVar<T>` always with an explicit name. `ModifyHpLost*` hooks are ABSOLUTE. Validate VFX paths against `grep '"vfx/' decompiled/`. Full gotcha table in WORKFLOW-FGO.
- Scaffolding: `dotnet new install Alchyr.Sts2.Templates` → "Slay the Spire 2 Character". Project name has no spaces; "Put solution and project in same directory" must be enabled.
