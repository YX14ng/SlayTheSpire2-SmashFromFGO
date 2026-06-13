# DESIGN-TIAMAT — diseño del personaje (Beast II, FGO) para StS2

Diseñada con el panel de jueces (workflow `tiamat-fgo-design`, 2026-06-13). Ganó la propuesta
"invocadora de Laḫmu" (A) con la legibilidad de la "marea" (B) injertada y la forma Bestia de
(C) como variante. Fuente de fidelidad: Atlas Academy servant NA #149 / ID 9935400 + TYPE-MOON
Wiki + transcripción de Babilonia. Sigue [docs/METHODOLOGY.md](METHODOLOGY.md) y la skill
sts2-mechanics-design. NO implementada todavía.

## Identidad en una frase
**La Madre que puebla el campo.** Tiamat no pelea: *cría*. Esparce Marea de Caos (黑泥) sobre los
enemigos y, del Mar de Vida, pare Laḫmu que bloquean y muerden solos en el turno enemigo. Cada
turno decide **PARIR** (más bocas), **ALIMENTAR** (dientes más grandes) o **DEVORAR** (pico
inmediato sacrificando cría) — el doble filo del amor de madre como decisión mecánica. Fiel: su
poder canónico NO es un NP nombrado; es el Mar de Vida + Laḫmu que se multiplican y regeneran
desde restos mínimos, con la inmortalidad de Bestia ("carece del concepto de muerte") de fondo.

**Eje propio (distinción del roster):** ÚNICO arquetipo de INVOCACIÓN/gestión de tablero.
Mash=muralla, Morgan=Buster crit, Artoria=soporte crit-caster. Tiamat=enjambre + corrupción.

## Motor central — la Cría (ÚNICO sistema nuevo) + Marea (FGOCore)
**`LahmuSwarmPower` (Counter 0–6)** = nº de Laḫmu en campo, con un **nivel de Crianza global
`Nurture`** (NO lista por-larva — un solo número, decisión clara y barata de implementar).
- **Tic que NO es gratis** (números chicos del baseline):
  - Inicio del turno enemigo / fin de tu turno: el swarm **muerde** al enemigo más cursado por
    `nº Laḫmu × (1 + Nurture)`, y da a Tiamat **bloque** `nº Laḫmu × (2 + Nurture)`.
  - Base (6 larvas, Nurture 0) = ~12 bloque + 6 daño: relevante, NO gana. El poder está en Nurture.
- **Las 3 decisiones por turno** (corazón):
  1. **PARIR** +1 Laḫmu (sube el techo).
  2. **ALIMENTAR** +Nurture (escala lineal permanente a TODOS).
  3. **DEVORAR** sacrificar N Laḫmu → burst inmediato escalado por Nurture (pico gateado;
     NUNCA rinde más que parir+alimentar acumulado — regla METHODOLOGY).
- **Marea de Caos = `CursePower` de FGOCore tal cual** (DoT al inicio del turno enemigo, cap 25,
  decae 1/turno, `Curses.Apply/Consume/MostCursed`, amplificadores/preservers). El swarm muerde
  "al más cursado" → la Marea DIRIGE a la Cría (sinergia). Cartas convierten Maldición → Nurture.

## Formas (FormPower/FormSwitch — humanoide ↔ Bestia)
Cambian QUÉ decisión es buena, no los números planos:
| | **Femme Fatale** (humanoide, criadora) | **Forma Dracónica** (Bestia II, devoradora) |
|---|---|---|
| Pasiva | Inicio de turno **+1 Nurture**; cartas de cría −1⚡ (mín 0) tras jugar 1 carta de cría | El swarm muerde **dos veces**; **Devorar +50%**; esparce sola +1 Maldición/turno (`ICurseAmplifier`) |
| Rol | construir (setup/sustain) | cosechar (clímax) |
NO `IsPermanent` (switch reversible = más decisiones; `IsPermanent` queda como knob de playtest
si la danza degenera). Entrar a Bestia también ocurre **gratis al abrir la ventana de NP**.

## Ventana de NP a 100 — "Génesis" (modelo nuevo, NO ulti gratis)
1 turno: **+1⚡, robás 2**; las cartas de cría cuentan **doble**; aplica `OverchargeBlessingPower`;
si estás en humanoide te volvés **Bestia gratis**. Las cartas-NP drafteadas son el clímax.

## HP, mazo inicial (4 básicas + 2 firmas)
**HP = 73.** Básicas: Marea/Chaos Tide (1⚡, 6 daño) ×4; Caparazón (1⚡, 5 bloqueo) ×2-3; firmas:
**Engendrar** (1⚡: parí 1 Laḫmu + 8 NP; 0⚡ en Femme Fatale tras carta de cría) y **Lodo Negro**
(1⚡: 3 Maldición + 1 Nurture). Gana acto 1 sin motor (golpes/defensas a tasa); plan B = Marea+Maldición.

## ~6 cartas clave (calibradas)
| Carta | Rareza | ⚡ | Efecto |
|---|---|---|---|
| Amamantar / Suckle | Común | 1 | +2 Nurture; si ≥3 Laḫmu, robá 1 |
| Mitosis / Unit Mitosis | Poco común | 1 | Devorá 1 Laḫmu: daño 4 + 3×Nurture. Parí 1 nuevo |
| Diluvio Negro | Poco común | 2 | 4 Maldición a TODOS; +1 Nurture por enemigo cursado |
| Once Bel Laḫmu | Rara | 2 | Parí hasta 6 Laḫmu; +Nurture = Laḫmu en campo. Exhaust |
| Cuerno Roto | Rara | 0 | Devorá TODA la cría: daño AoE = Laḫmu × (4 + 2×Nurture); NP = Laḫmu×10. Exhaust |
| Enūma Eliš (carta-NP) | NP | ≥1 | `ConsumeAllForNpCard`. Revive cría caída + muerde a todos por Laḫmu×Nurture (~180-220, nunca 300+). *Guiño: es el arma que la mató (Kingu), NO su NP* |

## Reliquias (SIN multiplicador global)
1. **STARTER — Útero del Mar de Vida** (evento→recurso): al iniciar combate parís 1 Laḫmu; la
   1ª vez por combate que cada enemigo recibe Maldición, parís 1 Laḫmu (tope 1/enemigo).
2. **Lágrimas de la Madre**: al Devorar un Laḫmu, curás 2 HP.
3. **Cuerno de King Hassan** (`IGutsFloorBooster`): tu Guts revive a 1 HP **pariendo 3 Laḫmu**
   (la inmortalidad girada a favor; guiño a que solo "otorgándole la muerte" cae). 1/combate.

## FGOCore: reusa vs NUEVO
- **Reusa tal cual:** `CursePower`+`Curses` (Marea), `NpChargePower`+`NpCharge` (`GaugeFilled`/
  `ConsumeAllForNpCard`), `OverchargeBlessingPower`, `FormPower`/`FormSwitch`/`FormVisuals`,
  `GutsPower` (subclase con `OnTriggered` → parir), `IGutsFloorBooster`, `BondRelic` (sin ×global).
- **NUEVO (acotado):** `LahmuSwarmPower` (Counter + campo Nurture, espejo de `CursePower`) +
  helper estático `Lahmu` (Spawn/Feed/Devour/Count). `ICurseAmplifier` en el FormPower de Bestia.

## Checklist de implementación
**FGOCore (build/publish primero):**
- [ ] `LahmuSwarmPower : FGOCorePower` (Counter, Nurture). Hook de turno: dar **bloque** a Owner
  (verificar API `BlockCmd.Add`/equivalente — `CursePower` solo DAÑA al Owner, acá hay que dar
  bloque) + **morder** al `Curses.MostCursed`. ⚠️ Confirmar el hook exacto de "fin del turno del
  jugador" (CursePower usa `AfterSideTurnStart`; revisar `AfterSideTurnEnd`/equivalente).
- [ ] Helper `Lahmu` (Spawn/Feed/Devour→burst base/Count).
**Mod Tiamat (depende de FGOCore):**
- [ ] `TiamatFemmeFatalePower`/`TiamatBeastPower` (`: FormPower`; Bestia `: ICurseAmplifier`).
- [ ] `TiamatGutsPower : GutsPower` con `OnTriggered` → parir (verificado: `OnTriggered` es virtual).
- [ ] `NpCharge.GaugeFilled` → ventana "Génesis".
- [ ] Cartas (básicas+firmas+6 clave+pool) + carta-NP "Enūma Eliš" (`ConsumeAllForNpCard`).
- [ ] Reliquias (Útero starter, Lágrimas, Cuerno de King Hassan) + BondRelic sin ×global.
- [ ] Loc eng/esp/zhs + íconos por power.
**Assets a verificar (HTTP 200 antes de codear):**
- [ ] Servant NA #149 / ID 9935400 (charagraph humanoide Femme Fatale).
- [ ] Forma Dragonoid (¿sprite de batalla o riggear desde charagraph? FGO usa puppets — WORKFLOW-FGO §3/§7).
- [ ] Sprites de Laḫmu / Bel Laḫmu (enemigos de Singularidad 7, war 107). ⚠️ Riesgo: puede no
  haber sprite limpio → fallback = ícono estilizado del contador del swarm.

## Riesgos / knobs de playtest
- Carga cognitiva del swarm → mostrar "Laḫmu: N / Crianza: M" + previsualizar bloque+mordida (o el agregado).
- Bloque del swarm demasiado alto → invade el nicho de Mash (knob: coef. bloque 2→1).
- Devorar > criar (loop roto) → bajar multiplicador de Nurture en Devorar.
- Forma reversible degenerada → subir costo del switch o `IsPermanent` la Bestia.
- Bestia + ventana NP = pico doble → calibrar Enūma Eliš ~180-220.
- Fidelidad: Enūma Eliš es de Kingu, NO de Tiamat — usar como guiño, no afirmar que es su NP.
  Larva/Tiamat (#376) es OTRA unidad: no mezclar skills.
