# Bug report — figure_Saya (沙耶之歌) breaks other BaseLib custom characters

> Para enviar al autor de figure_Saya (Workshop `3747508952`). Bilingüe EN + 简体中文.
> Diagnóstico: log de un player + decompilado de BaseLib 3.3.x. Ver también `docs/FINDINGS.md`.

---

## English

**Summary**
`figure_Saya` bundles its own copy of BaseLib's node-factory / scene-conversion system and
re-registers the **global** node factories (`Control`, `NCreatureVisuals`, `NEnergyCounter`).
Because it loads *after* other character mods, this clobbers the real BaseLib's scene
auto-conversion, and **every other BaseLib custom character that uses `CustomVisualPath`
crashes when entering combat**.

**Environment**
- StS2 v0.107.1, BaseLib v3.3.2
- `figure_Saya` subscribed alongside other BaseLib characters (in the player's log: Mash /
  Artoria / Morgan from the FGO set, plus Acheron, Kafka, Evanescia, Kars).

**Symptom** — when entering combat as another BaseLib character:
```
System.InvalidCastException: Unable to cast object of type 'Godot.Control'
        to type 'MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals'.
   at MegaCrit.Sts2.Core.Models.CharacterModel.CreateVisuals_Patch5
   at MegaCrit.Sts2.Core.Nodes.Combat.NCreature.Create
   at MegaCrit.Sts2.Core.Nodes.Rooms.NCombatRoom.AddCreature
   at MegaCrit.Sts2.Core.Nodes.Rooms.NCombatRoom.CreateAllyNodes_Patch1
```
The game loads to the main menu fine; it crashes when the combat room builds the character.

**Root cause**
Vanilla `CharacterModel.CreateVisuals()` does
`GetScene(VisualsPath).Instantiate<NCreatureVisuals>()`, relying on BaseLib's global
scene-conversion patch (`NodeFactory.TryAutoConvert`, backed by the static `_registeredScenes`
and `_factories` dictionaries) to have converted the mod scene's root `Control` into an
`NCreatureVisuals`. The player's log shows figure_Saya creating a **second, bundled** copy of
that system:
```
[figure_Saya.ModSupport] Created node factory for Control.
[figure_Saya.ModSupport] Created node factory for NCreatureVisuals.
[figure_Saya.ModSupport] Created node factory for NEnergyCounter.
```
(`"Created node factory for <T>"` is BaseLib's own `NodeFactory<T>` constructor log line — so
`figure_Saya.ModSupport` is shipping BaseLib's node-factory code.) Loading after the other
characters, this bundled system competes for the same conversion path; for scenes registered
with the *real* BaseLib (other characters' visuals), the conversion to `NCreatureVisuals` no
longer runs → the scene root stays a plain `Control` → the `Instantiate<NCreatureVisuals>()`
cast throws.

**How to confirm**
Subscribe `figure_Saya` together with any other BaseLib custom character → enter combat as that
other character → crash. Unsubscribe `figure_Saya` → it loads fine.

**Suggested fix (figure_Saya side)**
- Don't bundle/fork BaseLib's `NodeFactory` / scene-conversion system. Depend on the installed
  BaseLib (the one the game already loads) instead of shipping your own copy that re-registers
  the **global** factories for engine types (`Control` / `NCreatureVisuals` / `NEnergyCounter`).
- If you must keep helpers, scope them so they only handle figure_Saya's own scenes and never
  overwrite the global factory registrations for types you don't own.

**Note**
We've already worked around this in our own characters (overriding `CreateCustomVisuals()` to
build the visuals directly from the factory instance), so our mods should survive the combo
now. But the underlying clobber still affects any BaseLib character relying on the standard
auto-conversion, and very likely the rest-site / merchant / energy-counter scenes too.

---

## 简体中文

**问题概述**
`figure_Saya` 自带了一份 BaseLib 的 node-factory / 场景转换系统，并**重新注册了全局**的
node factory（`Control`、`NCreatureVisuals`、`NEnergyCounter`）。由于它在其他角色 Mod **之后**
加载，会覆盖掉真正 BaseLib 的场景自动转换，导致**所有使用 `CustomVisualPath` 的其他 BaseLib
自定义角色在进入战斗时崩溃**。

**环境**
- StS2 v0.107.1，BaseLib v3.3.2
- `figure_Saya` 与其他 BaseLib 角色同时订阅（玩家日志中：FGO 系列的 玛修/Artoria/摩根，
  以及 Acheron、Kafka、Evanescia、Kars）。

**症状** —— 以另一个 BaseLib 角色进入战斗时：
```
System.InvalidCastException: 无法将 'Godot.Control' 转换为
        'MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals'。
   at CharacterModel.CreateVisuals_Patch5
   at NCreature.Create
   at NCombatRoom.AddCreature
   at NCombatRoom.CreateAllyNodes_Patch1
```
游戏能正常进入主菜单；在战斗房间创建角色时崩溃。

**根本原因**
原版 `CharacterModel.CreateVisuals()` 执行
`GetScene(VisualsPath).Instantiate<NCreatureVisuals>()`，依赖 BaseLib 的全局场景转换补丁
（`NodeFactory.TryAutoConvert`，底层是静态字典 `_registeredScenes` / `_factories`）把 Mod 场景的
根节点 `Control` 转换成 `NCreatureVisuals`。玩家日志显示 figure_Saya 创建了**第二份自带的**该系统：
```
[figure_Saya.ModSupport] Created node factory for Control.
[figure_Saya.ModSupport] Created node factory for NCreatureVisuals.
[figure_Saya.ModSupport] Created node factory for NEnergyCounter.
```
（`"Created node factory for <T>"` 正是 BaseLib 自身 `NodeFactory<T>` 构造函数的日志 —— 说明
`figure_Saya.ModSupport` 携带了 BaseLib 的 node-factory 代码。）它在其他角色之后加载，争夺同一条
转换路径；对于注册在**真正** BaseLib 上的场景（其他角色的立绘），到 `NCreatureVisuals` 的转换不再
执行 → 场景根节点仍是 `Control` → `Instantiate<NCreatureVisuals>()` 的强制转换抛异常。

**复现/确认**
把 `figure_Saya` 和任意其他 BaseLib 自定义角色一起订阅 → 用那个角色进战斗 → 崩溃。
取消订阅 `figure_Saya` → 一切正常。

**建议修复（figure_Saya 侧）**
- 不要自带/分叉 BaseLib 的 `NodeFactory` / 场景转换系统。请直接依赖游戏已加载的 BaseLib，
  而不是自带一份去**重新注册引擎类型（`Control` / `NCreatureVisuals` / `NEnergyCounter`）的全局
  factory**。
- 如果一定要保留自己的辅助代码，请限定作用域：只处理 figure_Saya 自己的场景，绝不覆盖你并不拥有
  的类型的全局 factory 注册。

**备注**
我们已经在自己的角色里做了规避（重写 `CreateCustomVisuals()`，直接从 factory 实例构建立绘），
所以我们的 Mod 现在应该能在该组合下正常运行。但底层的覆盖问题仍会影响任何依赖标准自动转换的
BaseLib 角色，并且很可能同样波及 休息点 / 商人 / 能量计数器 的场景。
