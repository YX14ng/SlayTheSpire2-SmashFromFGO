# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

A custom character mod for **Slay the Spire 2** (Early Access, Godot 4 / C#). The character is **Mash Kyrielight** (the Shielder from Fate/Grand Order — "Smash" in the repo name is a pun on her name; no FGO character is literally named Smash). The mod targets the **MAIN branch** of the game (v0.103.x), not the beta branch — BaseLib ≥ 3.1.8 is the main-compatible line.

- [docs/HANDOFF.md](docs/HANDOFF.md) — **pending tasks + cross-machine handoff** (NP cap 300, dupe mechanic, Holy Grail). If you're picking up development on a new machine, read this FIRST.
- [docs/WORKFLOW-FGO.md](docs/WORKFLOW-FGO.md) — **the playbook**: end-to-end process for building FGO characters (asset pipeline, renderer, art matching, the gotcha table). Read this FIRST for any new character or when touching the pipeline.
- The repo holds TWO mod projects: `FGOCore/` (shared mechanics library — build/publish first) and `MashShielder/` (the character, depends on FGOCore). On Linux, MegaDot lives at `MegaDotLinux/` in the repo.
- [docs/MODDING.md](docs/MODDING.md) — toolchain, mod format, BaseLib API, resource links.
- [docs/DESIGN.md](docs/DESIGN.md) — full character design: mechanics (Carga NP, Baluarte, Formas Shielder/Ortinax/Paladín, Intercepción, Black Barrel), complete card pool, relics, and the 2D model/animation plan.
- [docs/DESIGN-MORGAN.md](docs/DESIGN-MORGAN.md) — second character (Morgan, Berserker→Caster form-switching via cards): complete design, not yet implemented. Designed with the [.claude/skills/sts2-mechanics-design](.claude/skills/sts2-mechanics-design/SKILL.md) skill (real vanilla balance baselines — use it for ANY new card/character/relic design).
- `tools/*.ps1` + `tools/render_project/` — reusable pipeline scripts (render, art, icons, tres generation). `.claude/workflows/match-ce-art.js` — parameterized CE art matching workflow.
- `assets/reference/` — official art and battle-sprite part atlases downloaded from Atlas Academy (servant 800100, costumes 800150 Ortinax / 800200 Paladin). FGO battle models are Unity 2D puppets, NOT frame spritesheets — animations must be re-rigged in Godot (see DESIGN.md §7).

The user communicates in Spanish — respond in Spanish. Card/mechanic names in the design are Spanish with English localization planned.

## Key facts

- StS2 runs on **MegaDot** (Mega Crit's Godot 4 fork, https://megadot.megacrit.com/) with game logic in C#. Mods are distributed as `<Id>.json` (manifest) + `<Id>.dll` (code) + `<Id>.pck` (assets) in the game's `mods/` folder.
- Game install: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2` (v0.103.3 as of June 2026). **BaseLib** and **ModConfig** are already installed there; dozens of character mods in that folder serve as structural references (e.g. `JeanneAlter`, another FGO character).
- The standard framework is **BaseLib** (NuGet `Alchyr.Sts2.BaseLib`): provides `CustomCharacterModel`, `CustomCardModel`, `CustomRelicModel`, custom keywords/enums, localization, and automatic ID prefixing. API docs: https://alchyr.github.io/BaseLib-Wiki/
- Project scaffolding comes from `dotnet new install Alchyr.Sts2.Templates` → "Slay the Spire 2 Character" template. Project name must have no spaces; "Put solution and project in same directory" must be enabled.
- The mod's manifest `id` must never change once chosen — it determines the filenames the game loads.

## Build & deploy

- Requires the **.NET 9 SDK** (not yet installed on this machine as of June 2026 — only the runtime is present) and a MegaDot build matching the game version. Paths are configured in `Directory.Build.props`.
- **Build** compiles code only. **Publish** (to local folder) compiles the `.dll`, packs assets into the `.pck`, and copies all three files into the game's `mods/` folder. Any non-code change (localization JSON, images, scenes) requires Publish, not just Build.
- Localization errors on first build of the character template are expected — resolve via the `Alchyr.Sts2.ModAnalyzers` "Generate localization" quick-fix and move strings into the localization JSON files.
