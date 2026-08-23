# PANEL U-OLGA — dictamen del juez de IMPLEMENTABILIDAD (2026-08-23)

Juez 2 de 3 (corrió después del de [balance](PANEL-UOLGA-VEREDICTO-BALANCE.md)). **Sus parches
MANDAN.** Falta el juez de fidelidad y legibilidad (nombres, complejidad en mano, refritos).

Base juzgada: propuesta [B](PANEL-UOLGA-B-CAZA.md) con los 17 parches del juez de balance.

## Veredicto

**La base parcheada se construye en ~85% con piezas ya probadas del repo. NADA obliga a tocar
FGOCore → cero republish de los 13.** La deuda real está en cuatro lugares: la **pareja de cartas
manifestadas** (inédito: todos los mods manifiestan UNA sola), el **cap +15 del Decreto** (no se
puede imponer limpio por hooks del motor), los **reusos cross-mod que el acta prometía y NO existen**
(el Anti-Purga vive en ArtoriaCaster, el Bloqueo de Curación en Tiamat; no hay registro compartido en
FGOCore) y la **metaprogresión** (viable, con bordes en `OnEnded`).

## Hallazgos por pieza

**Token/Decreto.** Controlador = un power `StackType.Single`; el precedente de «reaplicar
REEMPLAZA» es el propio `GutsPower.cs:24` → **el parche 5 del balance (re-convertir reemplaza la
ventana) sale gratis**. Entrega 1/turno con `BeforeTurnStart` → `ManifestCards.ManifestToHand<T>`
(`FGOCore/FGOCoreCode/Combat/ManifestCards.cs:30`, mismo canal que el arsenal de Gilgamesh en
`GilgameshArcher/GilgameshArcherCode/Powers/TreasureDeck.cs:24-31`). Estado en tres `PowerVar` con
nombre explícito (gotcha §5): cargas, magnitud del tier, turnos restantes.
**Los tres candados del acta se resuelven por TIPO de carta, no por parches**: si el Decreto es
`CardType.Skill` que pega daño en `OnPlay`, entonces no implementa `ICommandTyped` y
`CommandBonusPower.cs:56` no lo ve; no es Ataque, así que ningún rider de «jugaste un Ataque» ni el
auto-gasto de Crítico Listo (patrón Mordred) lo tocan — **el parche 4 del balance (el token no
critica) también sale gratis**; y no genera NP porque no llama `NpCharge.Gain`. `IsFirstInSeries` NO
hace falta: filtra replays, y el Decreto es un play normal.
**Fin de combate / save / co-op**: el combate no se serializa → powers y cartas generadas mueren con
el `CombatState`, no hay nada que guardar; en co-op todo viaja por comandos sincronizados, mismo
canal que Gilgamesh y Morgan.

**Manifestación doble a 100.** Patrón Mash literal (`MashShielder/MashShielderCode/MainFile.cs:52-86`
+ `FGOCore/FGOCoreCode/Combat/NpWindow.cs`): marcador por pico con `GaugeFilledWithContext` /
`GaugeDroppedWithContext` — ese ES el fix del brickeo histórico de Tiamat, ya factorizado.
`GaugeFilled` sólo dispara en el cruce real del umbral → sin doble manifestación. La conversión pasa
por `NpCharge.ConsumeAllForNpCard` (`FGOCore/FGOCoreCode/Np/NpCharge.cs:167`): ⚠️ las preparaciones
de Overcharge **suman tier → suman Decretos**; se acepta (tier es tier) pero queda documentado.
**Lo inédito y obligatorio**: al `GaugeDropped` hay que **exhaustar a la hermana no jugada**, o se
duplica en el próximo pico y ensucia la mano.

**Guts condicional + Forma 3 — sí, sin tocar FGOCore.** `GutsPower` recibe `dealer` en
`ModifyHpLostAfterOstyLate` (`FGOCore/FGOCoreCode/GutsPower.cs:47`): la subclase gatea por
`dealer?.HasPower<ThreatPower>()` y delega en `base`. Es seguro contra el falso disparo porque
`Hook.ModifyHpLost` sólo anota como modifier a quien CAMBIÓ el monto
(`decompiled/MegaCrit.Sts2.Core.Hooks/Hook.cs:1727`). Morir por veneno o quemadura (dealer null) no
dispara = canon exacto («やられた時» por una Amenaza). La transición usa `OnTriggered`
(`GutsPower.cs:76`) → `FormSwitch.Enter<...>` con `IsPermanent` (`Forms/FormSwitch.cs:18`,
irreversible por construcción) + `CreatureCmd.LoseMaxHp` (precedente `PaperCutsPower.cs:20`) +
remoción de los Mal por interfaz marcadora. ⚠️ **Los powers de forma NO persisten entre combates**:
la reliquia re-instala F3 al inicio de cada combate con flag run-scoped, patrón
`KagetoraLancer/KagetoraLancerCode/Relics/IdentityRelics.cs:52,86`.

**Marcador de Amenaza.** Todo desde el mod: `BeforeCombatStartLate`
(`decompiled/.../AbstractModel.cs:496`) aplica el power visible; `FgoAttributes.RegisterOverride` /
`RemoveOverride` son públicos (`FGOCore/FGOCoreCode/Attributes/FgoAttributes.cs:40-44`). El
diccionario es estático de proceso → hay que **desregistrar al cerrar combate** con un set propio de
ModelIds, o los otros personajes FGO ven Estrellas fantasma en runs posteriores.

**Metaprogresión.** `RunManager.OnEnded(bool)` es el punto (`decompiled/.../RunManager.cs:1517`;
victoria `:1246`, muerte `CreatureCmd.cs:461`, abandono por el mismo camino). Riesgos: (a) el
early-return `_runHistoryWasUploaded` **no corta un postfix** → guard de idempotencia por run-id,
obligatorio; (b) respetar el gate `ShouldSave` para no facturar Daily/Custom; (c) en co-op corre en
cada cliente → chequear que el personaje local sea U-Olga; (d) el «llegué al Acto 3» se lee del
`RunState` en el postfix. Store `SimpleModConfig` sin objeción.

## Parches obligatorios

1. **El Decreto es `CardType.Skill` con daño en `OnPlay`, no Ataque.** Resuelve por construcción los
   tres candados del acta y el parche 4 del balance. En el texto se lee como ataque en flavor, no en
   reglas.
2. **El cap +15 se computa DENTRO del Decreto** (base + `min(15, suma de sus bonos)`), pegando con
   props no-`IsPoweredAttack` para que el motor no re-sume Fuerza (`decompiled/.../StrengthPower.cs:22`).
   Nunca por hook global — y de paso el gotcha de «`ModifyDamage*` corre en preview» deja de aplicar.
3. **`GaugeDropped` exhausta a la hermana no jugada** (NP o conversión).
4. **Anti-Purga (逆光) y Bloqueo de Curación: reimplementación LOCAL en el mod.** El acta prometía
   reuso, pero viven en ArtoriaCaster y Tiamat y no hay registro cross-mod en FGOCore; una
   dependencia entre mods hermanos está prohibida. Son ~30 líneas cada uno (el negate de 1 golpe se
   apoya en el patrón vanilla `IntangiblePower`).
5. **Guts condicional = subclase local de `GutsPower`** con gate por dealer, aplicada en
   `BeforeCombatStartLate` sólo en combates con Amenaza.
6. **La reliquia reinstala la Forma 3 al inicio de cada combate** (flag run-scoped). Sin esto, la
   transformación «irreversible» dura un solo combate.
7. **Postfix de `OnEnded` con guard por run-id + gate `ShouldSave` + check de personaje local.**
8. **Limpieza de los overrides de `FgoAttributes`** al cerrar combate.
9. **Parche 11 del balance: verificado y cerrado.** No existe `ServantDamageMultiplier` en FGOCore y
   `BondRelic.cs:26` documenta «NP stays flat», lifts sólo de HP. No hay recorte que hacer.
10. **`空前絶後` «robás 2 menos»**: `ModifyHandDraw` es ABSOLUTO → `return max(0, input-2)`, nunca 0 fijo.
11. **`驚天動地` «no factura en ningún lector»**: viable porque todos los lectores son propios — flag
    «pago en curso» que consultan el starter y las condiciones «perdiste Vida». Si algún lector vive
    fuera del mod, el candado se rompe.

## FGOCore

**Nada obligatorio, cero republish.** Todo lo necesario ya es público: `NpCharge` (eventos +
`ConsumeAllForNpCard`), `ManifestCards`, `FgoAttributes`, `FormSwitch`, `GutsPower` (subclasificable:
`OnTriggered`/`Floor` son `protected virtual`). Promover Anti-Purga y heal-block a FGOCore sería
«lo correcto» pero cuesta republicar 13 mods por ~60 líneas: **versión local**. Usar siempre los
overloads `*WithContext`.

## Orden de implementación por lotes

| # | Lote | Riesgo |
|---|---|---|
| 1 | Scaffold + starter + básicas + pool sin Amenaza | **BAJO** — todo precedente directo; cuidar el cap 3/turno con reset en `BeforeSideTurnStart` (patrón Siegfried) |
| 2 | NP + conversión + Decreto + controlador | **MEDIO** — la pareja manifestada es lo inédito; testear pico→gasto→re-pico y la re-conversión con ventana activa |
| 3 | Amenaza + Guts + Forma 3 + 逆光 local | **MEDIO-ALTO** — verificar en juego el no-disparo con dealer no-Amenaza y la reinstalación de forma entre combates |
| 4 | Reliquia 驚天動地 + candado de lectores | **MEDIO** — el flag «pago en curso» atraviesa `CreatureCmd` async |
| 5 | Metaprogresión | **ALTO y aislable** — si el postfix da problemas, el personaje funciona sin appends (el acta lo balancea sin ellos). Es el único lote recortable del primer release |
