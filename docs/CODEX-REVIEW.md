# CODEX-REVIEW — bug-hunt handoff brief

Self-contained brief for **Codex** (or any agent) taking over **code review to find and fix hidden
/ latent bugs** in this repo. Read this first, then `docs/FINDINGS.md` and the
`docs/WORKFLOW-FGO.md` gotcha table for depth.

---

## 1. Mission

Hunt **latent correctness bugs** — not style, not formatting. In scope:

- Logic errors and wrong edge-case behavior in card/power/relic effects.
- **Hook-contract violations** (the #1 source of real bugs here — see §4).
- **Multiplayer desync** (RNG misuse, non-deterministic per-player logic).
- **Save/load** breakage (`SavedProperty`/`PowerVar` misuse, ID renames).
- **Resource / VRAM / netcode** issues (synchronous loads, unbounded caches).

Rules of engagement:
- **Verify every finding against the decompiled game in `decompiled/`** before reporting — confirm
  the real hook signature/semantics and game behavior. Don't trust comments or memory.
- **Fix only High-confidence findings** with a minimal diff. Flag Medium/Low as items, don't guess.
- Respect the immutability + publish-together rules in §3/§4 — a "fix" that renames an ID or changes
  an FGOCore signature can break saves or silently unload every mod.

---

## 2. What this repo is NOW

> If `AGENTS.md` says "a single Mash mod for v0.103.x", that's stale. This is the current truth
> (also in `CLAUDE.md`).

- **Multi-mod monorepo**: one shared mechanics library **`FGOCore/`** (~88 `.cs`) + **12 character
  mods** (MashShielder, MorganBerserker, ArtoriaCaster, MordredSaber, GilgameshArcher, OkitaSaber,
  OberonPretender, SiegfriedSaber, Tiamat→`TiamatBeast`, KagetoraLancer, ShutenDouji y
  AstolfoRider). ~1,068 `.cs` across the character mods.
- Each top-level folder with a `*.csproj` is an **independent mod** with its own manifest, assets,
  localization. All depend on FGOCore + BaseLib.
- Targets **MAIN v0.107.1 and BETA v0.109.0** with one artifact set, compiled against **BaseLib
  3.3.6** and runtime-verified with 3.3.7.
- **`decompiled/`** = the decompiled game (incl. a full BaseLib decompile under
  `decompiled/_baselib_full/`). This is the **ground truth** for hook signatures, VFX paths, and
  base-class behavior.
- **There is NO test suite.** No `*.Tests`, no xunit/nunit. Correctness = reasoning + decompiled
  cross-reference. (The `decompiled/` test stubs are vanilla game code, not ours.)

---

## 3. Build / verify

- **Build** (code-only changes): `dotnet build <Mod>/<Mod>.csproj -c Release` → copies dll/json to
  `dist/<Mod>/`.
- **Publish** (any non-code change — localization JSON, images, scenes): `dotnet publish
  <Mod>/<Mod>.csproj -c Release` → dll **+** MegaDot `--export-pack` `.pck` to `dist/<Mod>/`. The
  export prints benign warnings (`no solution file`, `MSB3077 ExitCode -1`) but still produces a
  valid `.pck` — **verify by content, not exit code**.
- Machine-local paths (`GodotPath`, `Sts2Path`) live in each project's `Directory.Build.props`
  (**gitignored**); `Sts2PathDiscovery.props` autodetects when possible. There is no `.sln` — build
  each project from its own dir. **.NET 9 SDK** + a **MegaDot 4.5.1** export binary are required.
- **Compile-green is not the only automated gate.** Build a changed character csproj (and `FGOCore`
  first), then run the relevant localization, asset, VFX, animation and context audits under `tools/`.
- For context-sensitive changes, run
  `dotnet run --project tools/choice_context_audit/ChoiceContextAudit.csproj -- .`; it must report
  zero calls that discard an available `PlayerChoiceContext`.
- **Publish-all-together (CRITICAL)**: if you change FGOCore's public API (a method signature like
  `NpCharge.CanPay`, or `BondRelic`), **every dependent character dll must be rebuilt in the same
  batch** — an old character dll against a new FGOCore throws `MissingMethodException` /
  `ReflectionTypeLoadException` and the mod silently fails to load. Prefer fixes that don't touch
  FGOCore's public surface.

---

## 4. Bug-class checklist (the core — where bugs actually hide)

Distilled from `docs/FINDINGS.md` and the `docs/WORKFLOW-FGO.md` gotcha table. For each: the rule,
why it bites, where to look.

1. **`ModifyDamage*` hooks MUST be PURE (no state mutation).** They run in **preview** too (to show
   the number), and the per-power hook is **not** given the preview flag — `Hook.ModifyDamage` has a
   `previewMode` param but does **not** forward it to `item.ModifyDamageAdditive(...)`
   (`decompiled/.../Hook.cs` ~line 2519). If you mutate there (e.g. `_pending = 0`, `_card ??= x`),
   a preview that runs *after* you cache consumes the bonus and the real hit gets 0. Pattern: cache
   in `BeforeCardPlayed` (real only), return the bonus **pure** in `ModifyDamageAdditive`, clear in
   `AfterDamageGiven` (real only y también corre si el golpe mata). `AfterDamageReceived` se omite
   en golpes letales. Grep: `ModifyDamageAdditive|ModifyDamageMultiplier` and check for assignments
   to fields inside.
2. **`ModifyHpLost*` / `ModifyHandDraw` / `ModifyCardPlayCount` are ABSOLUTE** (default return =
   input). Returning `0` **annuls all damage that combat**. Always return the input / call `base`
   when not changing it. (Contrast: `ModifyDamageAdditive`/`ModifyBlock` are DELTAs, default 0.)
3. **Multiplayer RNG split.** `RunState.Rng.*` (incl. `CombatCardGeneration`) is a **shared lockstep
   stream** consumed inside the synchronized combat sim. Consuming it in a **local-only** flow
   (card-reward, dupe roll, per-player event) desyncs its counter on one client → divergent states.
   Local/per-player rolls must use **`player.PlayerRng.Rewards`** (seed `^ NetId`, not part of the
   sim). Canonical correct example: `FGOCore/FGOCoreCode/Np/NpLevels.cs`.
   Grep: `RunState.Rng`, `CombatCardGeneration`, `PlayerRng`.
4. **Synchronous resource load freezes the netcode.** `ResourceLoader.Load<T>()` of a heavy `.tres`
   blocks the simulation thread → breaks the network heartbeat → timeout/disconnect (reported as a
   "crash"). Async-only: `LoadThreadedRequest` + poll `LoadThreadedGetStatus`; apply deferred via a
   `process_frame` signal. Reference impl: `FGOCore/FGOCoreCode/Forms/FormVisuals.cs`. Grep:
   `ResourceLoader.Load(` (sync) anywhere in combat paths.
5. **Static caches / VRAM.** Never preload all forms / all mods into a process-static cache (it pins
   VRAM forever → "only health bars" on weak GPUs, or crash). Group by character; preload only the
   fighting character's group. Frame textures capped `process/size_limit=768` in the `.import`.
   Check any `static` mutable collection for unbounded growth / missing cleanup.
6. **Multicast delegate await.** `NpCharge.GaugeFilled/GaugeDropped.Invoke()` over N subscribers
   only returns the **last** one's `Task`; earlier async handlers are fire-and-forget. If order /
   completion matters, iterate `GetInvocationList()` and `await` each.
7. **BaseLib node-factory clobber.** A character relying on BaseLib's scene auto-conversion crashes
   with `InvalidCastException 'Godot.Control' -> 'NCreatureVisuals'` on combat entry when another
   installed mod **forks BaseLib** and re-registers the global node factory. All 9 chars now
   override `CreateCustomVisuals()` → `NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath)`
   to bypass it — **verify none regressed**. Full write-up:
   `docs/REPORT-figure_Saya-baselib-conflict.md`.
8. **Smaller traps** (from the WORKFLOW-FGO gotcha table):
   - `PowerVar<T>` always constructed **with an explicit string name** (else `!X!` loc + DynamicVars
     break).
   - `CardRarity.Special` does **not** exist → use `CardRarity.Event` for manifested NP cards.
   - ID splitter mis-splits consecutive capitals (`QP`→`Q_P`, `IV`→`I_V`) → name classes
     `InsufficientQp`, `FouBeastIv`.
   - **VFX paths must exist** or the card hangs (NRE in `VfxCmd.PlayVfx`). Validate against
     `grep '"vfx/' decompiled/`.
   - **IDs are immutable with active saves** (mod id, model id, power id — the mod prefix is part of
     the id). Never rename; never migrate a mechanic between mods while saves exist.
   - Manifest `dependencies` use the **object form** `[{"id":"BaseLib","min_version":"v3.3.6"}, ...]`.
   - **One** Block-retention preventer per game — all custom retention must delegate to
     `FGOCore/FGOCoreCode/Block/BlockRetention.cs` (`IBlockRetentionSource`, MAX wins).
   - Write `.tscn`/`.tres` as UTF-8 **no BOM** (BOM → Godot rejects at runtime).

---

## 5. High-risk hotspots (scrutinize first)

**FGOCore engine** — `FGOCore/FGOCoreCode/`:
- `Np/NpCharge.cs` (gauge math, `GaugeFilled/Dropped` events, `AmplifyingCreatures` re-entrancy
  guard), `Np/NpLevels.cs` (dupe RNG — MP), `Np/NpChargePower.cs`.
- `Forms/FormVisuals.cs` (async load + static cache + per-char grouping).
- `GutsPower.cs` and `DragonScales/DragonScalesPower.cs` (**absolute** `ModifyHpLost*` hooks).
- `Stars/CriticalBank.cs`, `CritStarsPower.cs` and `CriticalResolver.cs` (reservation at 50,
  `CritReady` priority, one payment/event per card and Quick post-resolution reward).
- `FgoCombatState.cs` (hidden synchronized bitfields, turn reset ordering and participant filter).
- `Block/BlockRetention.cs` + `Block/IBlockRetentionSource.cs`, `Curses/CursePower.cs` (cap 25,
  decay, amplifiers), `Bond/BondRelic.cs` (`Points` `SavedProperty`, MP gift scaling).
- Cross-cutting interfaces to trace implementers of: `IGutsFloorBooster`, `ILimitBreaker`,
  `Np/INpLevelStore`, `Forms/IFormChangeListener`, `Block/IBlockRetentionSource`, `ICommandTyped`,
  the `DragonScales/IDragonScale*` family, `Lahmu/ILahmu*`, `Stars` `IBanksCritStars`.

**Per character** — `<Name>/<Name>Code/`:
- Everything under `Powers/` (this is where hook overrides — and thus hook-contract bugs — live).
- `Cards/**/*.cs` that touch damage, RNG, form-switching, or NP spend.
- Layout is consistent: `Cards/{Basic,Common,Uncommon,Rare,Special}/`, `Powers/`, `Relics/`,
  `Character/`. Approx surface: Mash 137 files, Artoria 126, Mordred 129, Oberon 124, Morgan 124,
  Okita 121, Gilgamesh 90, Siegfried 72, Tiamat 80, Kagetora 21, Shuten 22 y Astolfo 22.

---

## 6. Review method

- Work **per subsystem** (FGOCore) and **per character**; don't try to hold it all at once.
- For each suspect hook/effect: open the matching **decompiled** hook in `decompiled/` (and
  `decompiled/_baselib_full/` for BaseLib behavior) to confirm the real signature, whether it's
  DELTA vs ABSOLUTE, and whether it runs in preview. Reproduce the logic mentally with a concrete
  example (multi-hit card, preview-then-play, two players).
- Classify each finding **High / Medium / Low** by player impact + confidence.
- Precedent / bar: `docs/AUDIT-2026-06-15.md` — a 13-reviewer parallel audit with adversarial
  verification that found 37 verified bugs (5 High). Match that rigor; many of those classes recur.

---

## 7. Output format

Produce a findings list. For each finding:

```
<file>:<line> · [High|Med|Low] · <one-line bug>
  evidence: <decompiled path / repro steps that prove it>
  fix: <minimal proposed change>
```

- **Apply** only High-confidence fixes (minimal diff; no ID/signature renames without honoring the
  publish-together rule). Rebuild the affected mod(s) green; if FGOCore changed, rebuild all.
- **Flag** Medium/Low for the user to decide.
- After verifying, append confirmed findings (with the rule learned) to `docs/FINDINGS.md` in its
  existing high-density style.

---

## 8. Canonical references

- `docs/FINDINGS.md` — verified bug-classes + the engine-mechanics deep notes (RNG, netcode,
  VRAM, hooks). **Read after this file.**
- `docs/WORKFLOW-FGO.md` — the playbook + the full gotcha table (§ code gotchas).
- `docs/AUDIT-2026-06-15.md` — prior audit (method + 37 findings with code paths).
- `docs/DECISIONS.md` — closed rules (don't re-litigate).
- `CLAUDE.md` — current project overview + build/deploy + key facts.
- `decompiled/` — ground truth for hooks/VFX/signatures; `decompiled/_baselib_full/` for BaseLib.
