# AGENTS.md

Guidance for Codex (and other agents) working in this repository.

> **Reviewing code / hunting bugs?** Read **[docs/CODEX-REVIEW.md](docs/CODEX-REVIEW.md)** first —
> it's the self-contained bug-hunt brief (architecture, build, the recurring bug-classes, hotspots,
> method, output format).
>
> For the full project guide see **[CLAUDE.md](CLAUDE.md)** and the high-density docs in `docs/`
> (read `docs/STATUS.md` → `docs/DECISIONS.md` → `docs/FINDINGS.md` first).

## Project

A **multi-mod monorepo** of Fate/Grand Order character mods for **Slay the Spire 2** (Early Access,
Godot 4 / C#): one shared mechanics library **`FGOCore/`** + **9 character mods** (Mash, Morgan,
Artoria Caster, Mordred, Gilgamesh, Okita, Oberon, Siegfried, Tiamat) + `CoopDeterminismPatch/`.
("Smash" in the repo name is a pun on Mash, the first character — no FGO Servant is named Smash.)
Each top-level folder with a `*.csproj` is an independent mod depending on FGOCore + BaseLib.

- Targets the **MAIN public branch v0.107.1**, **BaseLib pinned 3.3.0** (the pin must match the
  BaseLib in the game or the mod won't load). All 10 mods compile green and are on Steam Workshop.
- `decompiled/` is the decompiled game (ground truth for hooks/VFX/base classes);
  `decompiled/_baselib_full/` is the BaseLib decompile.
- The user communicates in Spanish — **respond in Spanish**. Card/mechanic names are authored in
  Spanish with English + Simplified-Chinese localization.

See [docs/MODDING.md](docs/MODDING.md) (toolchain, mod format, BaseLib API),
[docs/WORKFLOW-FGO.md](docs/WORKFLOW-FGO.md) (the playbook + gotcha table), and
`docs/DESIGN-<NAME>.md` (per-character mechanics).

## Key facts

- StS2 runs on **MegaDot** (Mega Crit's Godot 4 fork). A mod = `<Id>.json` (manifest) + `<Id>.dll` +
  `<Id>.pck` in the game's `mods/`. Framework = **BaseLib** (`CustomCharacterModel`/`CustomCardModel`/
  `CustomRelicModel`, custom keywords, localization, automatic ID prefixing —
  https://alchyr.github.io/BaseLib-Wiki/).
- Game install: `G:\SteamLibrary\steamapps\common\Slay the Spire 2` (the old `C:\Program Files
  (x86)\Steam\...` path is stale). BaseLib + ModConfig are installed there.
- A manifest **`id` never changes** once chosen — it determines loaded filenames and the model-id
  prefix. Model/power IDs must never be renamed while saves are active.

## Build & deploy

- Requires **.NET 9 SDK** and a **MegaDot 4.5.1** export binary. Machine-local paths live in each
  project's `Directory.Build.props` (gitignored); `Sts2PathDiscovery.props` autodetects. No `.sln`.
- **Build/publish go to the repo staging `dist/<Id>/`, never the game folder.** `dotnet build`
  copies dll/json; `dotnet publish -c Release` also runs MegaDot `--export-pack` for the `.pck`
  (required for any non-code change). Build `FGOCore` first.
- **Publish-all-together**: an FGOCore API change requires rebuilding every character dll in the same
  batch, or the game throws `MissingMethodException`/`ReflectionTypeLoadException` and silently skips
  the mod.
- Real deploy = **Steam Workshop** (`tools/workshop_upload.ps1`). Install locally only for playtest
  (`tools/install-mod.ps1`); never have the same id installed locally AND subscribed (duplicate id →
  crash). Diagnose a non-loading mod from `%APPDATA%/SlayTheSpire2/logs/godot.log` first.
