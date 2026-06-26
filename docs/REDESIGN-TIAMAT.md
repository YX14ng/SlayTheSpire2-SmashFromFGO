# REDESIGN-TIAMAT — dos pozas (Lily mazo principal + ventana Bestia)

Rediseño 2026-06-19 (workflow `tiamat-redesign-twopool`: 3 propuestas + síntesis, sobre los
baselines de `sts2-mechanics-design` + `docs/METHODOLOGY.md`). **Supersede** el single-pool de
[DESIGN-TIAMAT.md](DESIGN-TIAMAT.md) (cuyo kit Madre/enjambre pasa a ser el contenido de la
**ventana Bestia**). NO implementado — pendiente de aprobación + playtest.

## Concepto
**Tempo-controladora de marea.** Tiamat-Larva (Alter Ego) NO corre por daño turno a turno:
bloquea, regenera, niega (Sello de Habilidad / Bloqueo de Curación / −crit / −Fuerza), **siembra
Maldición** y **cría Laḫmu** mientras llena el medidor NP, sobreviviendo cualquier turno. La
**Bestia II** no es un buff de tasa: es una **EJECUCIÓN temporal de 1-3 turnos** (entrada SOLO vía
el NP `Nammu Dur-an-ki` a 100+) cuya magnitud la decide cuánta Maldición/Crianza/cría montó en
Lily. **La Maldición es la divisa que Lily acuña y la Bestia gasta.** Decisión de firma:
**abrir a 100** (ventanas frecuentes de 1 turno) **vs banquear a 300** (una ventana total de 3).

**HP = 70.** ~80% del combate es Lily (tempo-control), ~20% es la ventana Bestia.

## La mecánica de las dos pozas
- **Lily = mazo principal PERSISTENTE** (drafteado, se conserva todo el run). Arrancás acá.
- **Bestia = ventana temporal de 1-3 turnos**, solo por el NP. Al abrirla, se **manifiestan a la
  mano** las 7 cartas Bestia (Special, no drafteables, flag `BeastEphemeral`). Al cerrarse la
  ventana (Counter→0, reversión de forma, o fin de combate/muerte) **se exhaustan/desvanecen** —
  NUNCA entran al mazo persistente. Lily queda intacta para el próximo ciclo.
- **Duración = clamp(tier/100, 1, 3):** 100-199→1 turno, 200-299→2, 300→3. La Sobrecarga sube la
  VENTANA en que el setup ya construido rinde — NO la tasa de daño plano.

## Mazo inicial (HP 70)
| Carta | ⚡ | Efecto |
|---|---|---|
| **Marea de Caos** ×4 | 1 | 5 daño + 2 Maldición (básica híbrida: golpe a tasa reducida que siembra el puente desde el turno 1) |
| **Caparazón Larval** ×3 | 1 | 6 Baluarte; +1 por cada Laḫmu (máx +3) |
| **Engendrar** ×1 *(firma)* | 1 | Parí 1 Laḫmu + 10 NP (motor de arranque) |
| **Ojo de la Estrella Azul** ×1 *(firma)* | 1 | +14 NP; Bloqueo de Curación al más maldito + −25% crit enemigo, 1 turno |

## Pool Lily (drafteable, ~27 — actualizado tras DESIGN-REVIEW-2 §2/§3, antes 15)
> El pool original era de 15 drafteables (3 comunes / 9 PC / 3 raras), demasiado chico vs el
> baseline vanilla ~82 (feast-or-famine por escasez de opciones). DESIGN-REVIEW-2 lo subió a **27**
> agregando cartas SOBRE los motores existentes (Maldición / Crianza-Laḫmu): sembradores de Maldición
> **con cuerpo** (que pegan), daño Lily decente (la fase Lily necesita PEGAR, no solo cargar), un 2º
> canje Maldición→recurso, motores de población/enjambre y 2 raras nuevas. Sin mecánica nueva.

### Comunes (6)
| Carta | ⚡ | Efecto |
|---|---|---|
| Lodo Negro | 0 | 3 Maldición (sembradora de coste 0) |
| Marea Creciente | 1 | 4 Maldición a TODOS (siembra AoE) |
| Amamantar | 1 | +2 Crianza; si ≥3 Laḫmu, robá 1 |
| **Latigazo Salobre** *(nueva)* | 1 | 8 daño + 2 Maldición (sembradora CON cuerpo) |
| **Resaca** *(nueva)* | 1 | 7 daño, +4 si la presa está maldita (daño Lily que premia el campo) |
| **Charco de Marea** *(nueva)* | 1 | 5 Baluarte + 2 Maldición (sembradora defensiva) |

### Poco comunes (16)
| Carta | ⚡ | Efecto |
|---|---|---|
| Ojo de la Estrella Roja | 1 | 4 Maldición + −2 Fuerza; si ya tenía ≥6, +3 (total 7). **El puente explícito** |
| Mar de la Estrella Azur | 2 | Cura 8 HP + 12 NP (a aliados en co-op también). **Único soporte de mar del roster** |
| Núcleo de la Diosa | 1 | Resist. a Debuffs (anula los próximos 2) + 5 NP al inicio de tus turnos |
| Diluvio Negro | 2 | 4 Maldición a TODOS; +1 Crianza por enemigo ya maldito |
| Sello de las Mareas | 1 | Sello de Habilidad a 1 + 3 Maldición (eco del NP) |
| Llamado del Mar de Vida | 1 | +18 NP + robá 1 (acelerador de Sobrecarga) |
| Marea Estancada | 1 | 5 Maldición; tus Maldiciones NO decaen este turno (ICursePreserver) |
| Sobremarea | 1 | Consume 6 Maldición del más maldito → +18 NP + 1 Crianza (canje a Sobrecarga) |
| Mitosis | 1 | Devorá 1 Laḫmu: 4 + 3×Crianza; parí 1 (×1.5 en Bestia) |
| **Marea Voraz** *(nueva — loop ofensivo §3)* | 1 | Concede `TidalSwarmPower`: el enjambre muerde TAMBIÉN al final de cada turno tuyo |
| **Garra Ahogadora** *(nueva)* | 1 | 6 daño + 2 Maldición + parí 1 Laḫmu (golpe que engendra) |
| **Tributo Abisal** *(nueva — 2º canje)* | 1 | Consume ≤6 Maldición del más maldito → 5 daño + lo consumido a esa presa (Maldición→daño) |
| **Poza de Desove** *(nueva)* | 1 | Parí 2 Laḫmu + 1 Crianza (motor de población intermedio) |
| **Embate de Marea Negra** *(nueva)* | 2 | 6 daño + 2 Maldición a TODOS (daño AoE que siembra) |
| **Nodriza Venenosa** *(nueva)* | 1 | 5 daño + 2 Crianza (golpe que alimenta) |
| **Trampa de Resaca** *(nueva)* | 1 | 6 daño + Sello de Habilidad a 1 (control CON cuerpo) |

### Raras (5)
| Carta | ⚡ | Efecto |
|---|---|---|
| Once Bel Laḫmu | 2 | Parí hasta 6 + 1 Crianza por cada faltante. Exhaust |
| Sobrecarga de la Larva | 2 | +40 NP; si cruza 100, Bendición de Sobrecarga (+1 turno a la ventana) |
| Cuerno Roto | 0 | Devorá TODA la cría: AoE = nº×(4+2×Crianza); +NP = nº×10. Exhaust |
| **Espira de Leviatán** *(nueva)* | 2 | 12 daño + 4 Maldición; si la presa tenía ≥8 Maldición, golpe DOBLE (pico de daño Lily) |
| **Nido de Cría** *(nueva)* | 1 | Concede `BroodMotherPower`: parí 1 Laḫmu al inicio de cada turno tuyo. Exhaust |

## Mazo especial Bestia (7 fijas, manifestadas en la ventana, Exhaust al cerrar)
| Carta | ⚡ | Efecto |
|---|---|---|
| Marea de Caos: Diluvio | 0 | 2 Maldición a TODOS (reabastece el campo para la mordida doble) |
| Engendrar Lahmu | 0 | Parí 2 Laḫmu (si al tope, +1 Crianza) |
| Amamantar Lahmu | 1 | +3 Crianza; si ≥4 Laḫmu, robá 1 |
| Devorar a los Hijos | 1 | Devorá ≤3 Laḫmu: daño = devorados×(5+2×Crianza) +1/3 Maldición del obj., ×1.5 Bestia. Parí 1 |
| Marea de Lahmu | 2 | Parí hasta 6 + 1 Crianza por Laḫmu en campo. Exhaust |
| Cuerno Roto | 0 | Devorá TODA la cría: AoE = nº×(4+2×Crianza)×1.5; +NP = nº×10 |
| **Pluma de la Bestia** *(NP-cierre)* | ≥1 ConsumeAll | Limpia debuffs; muerde a todos por (nº×Crianza×3)+(Maldición del más maldito×4). Cap ~180-220 |

## NP — `Nammu Dur-an-ki` (ナンム・ドゥルアンキ)
- **Apertura** (a 100, `GaugeFilled` manifiesta la carta-Evento): limpia TODOS tus debuffs; daño AoE
  **FIJO** = 14 + tier/10 + Sello de Habilidad a todos; `FormSwitch.Enter` a Bestia II;
  `NpWindow.OpenWindow` (+1⚡, robar 2). **Apertura = daño fijo** (no ConsumeAll) para NO apilarse con el cierre.
- **Duración** (`ConsumeAllForNpCard` devuelve el tier): clamp(tier/100, 1, 3).
- **Cierre** separado = la carta-NP Bestia **Pluma de la Bestia** (ConsumeAll del NP recargado dentro
  de la ventana). Apertura y cierre en cartas distintas para no sumar pico el mismo turno.

## Reliquias (sin ×daño global)
1. **STARTER — Útero del Mar de Vida**: al iniciar combate +10 NP + parí 1 Laḫmu; la 1ª vez que cada
   enemigo se cursa, parí 1 Laḫmu (Maldición→cría literal; ata el puente desde el turno 1).
2. **Lágrimas de la Madre** (`ILahmuDevourListener`): al Devorar un Laḫmu, curás 2 HP.
3. **Cuerno de King Hassan** (`IGutsFloorBooster`): tu Guts revive a 1 HP pariendo 3 Laḫmu. 1/combate.

## El puente de Maldición (4 puntos, todo cableado en FGOCore)
1. **Siembra** en Lily (Estrella Roja, Lodo Negro, Mareas, Sello; Marea Estancada congela el decaimiento).
2. **Dirección** pasiva: el enjambre ya muerde a `Curses.MostCursed`; el Útero pare por enemigo recién cursado.
3. **Cosecha** en Bestia: muerde DOS veces al más maldito; esparce +1 Maldición/turno; Devorar +1/3 Maldición; Pluma suma Maldición×4.
4. **Canje** a Sobrecarga: Sobremarea consume Maldición → NP + Crianza.
> Ejemplo: 1 enemigo a 12 Maldición, enjambre 6 / Crianza 3 → mordida Bestia ×2 = 48/turno + DoT 12;
> Pluma cierra con 6×3×3 + 12×4 = ~102 base ×tier — gateado por lo construido, jamás por un ×plano.

## Distinción del roster
Único arquetipo que **ALTERNA dos identidades asimétricas en el tiempo** (≈80% tempo-control / ≈20%
enjambre-Bestia). Mash=muralla; Morgan=Buster-crit que usa Maldición como rider propio;
Artoria=soporte-crit. Tiamat usa la Maldición como **campo acumulable** (DoT + imán del enjambre +
combustible de la ventana). Frente a los servants-NP (manifiestan UNA carta-ulti), Tiamat manifiesta
**un MAZO temporal entero Y cambia de forma+reglas**. La decisión "abrir a 100 vs banquear a 300" es única.

## Riesgos / knobs de playtest (palancas primarias **en negrita**)
1. Ventanas encadenadas (Lily desaparece) → KNOB: cooldown 1 turno tras revertir, o capar recarga intra-ventana.
2. Devorar > parir+alimentar (regla METHODOLOGY) → KNOB: **coef. Crianza** en Devorar/Cuerno 2×→1.5×.
3. Pico de cierre > ~220 → KNOB primario: **coef. Pluma de la Bestia** (Crianza×3→×2, Maldición×4→×3) o cap duro.
4. Fase Lily frágil en acto 1 / "sala de espera" (DESIGN-REVIEW-2 §3) → **RESUELTO sin tocar FGOCore**:
   la carta PC «Marea Voraz» concede `TidalSwarmPower` (mordida del enjambre TAMBIÉN al final de tu
   turno en Lily), más daño Lily nuevo en el pool. El knob global `BitePerLahmu 1→2` queda como palanca
   de RESERVA (afecta a todo el roster del enjambre — FLAGEAR si se toca); preferida la vía Tiamat-local.
5. Bloque del enjambre invade a Mash → KNOB: BlockPerLahmu 2→1.
6. Sobrecarga demasiado fácil → KNOB: bajar carga base de generadores Lily (14/18→12/16).
7. Carga cognitiva → HUD agregado (Laḫmu/Crianza/Maldición-campo/Ventana) + previsualizar agregados.
8. Mazo Bestia efímero DEBE limpiarse en los 3 caminos (Counter→0, reversión, fin/muerte) — testear `BeastEphemeral`.

## FGOCore: reusa vs NUEVO
- **Reusa:** `NpCharge`/Overcharge (`GaugeFilled`/`ConsumeAllForNpCard`/`OverchargeBlessingPower`),
  `FormPower`/`FormSwitch`/`FormVisuals`, `CursePower`/`Curses` (+`ICursePreserver`), `LahmuSwarmPower`/`Lahmu`
  (+`ISwarmBiteAmplifier`/`IDevourAmplifier`/`ICurseAmplifier`/`ILahmuDevourListener`), `Bulwark`,
  `GutsPower`/`IGutsFloorBooster`, `ManifestCards`/`NpWindow`, `Cleanse`, `BondRelic`.
- **NUEVO (acotado):** `TiamatLilyPower`/`TiamatBeastPower` (`: FormPower`), `TiamatBeastWindowPower`
  (Counter=duración, manifiesta + limpia el mazo efímero por flag `BeastEphemeral`), `TiamatGutsPower`,
  la carta-NP de cierre `Pluma de la Bestia`, y el flag `BeastEphemeral` en las cartas Special.
- **NUEVO post DESIGN-REVIEW-2 (todo Tiamat-local, FGOCore intacto):**
  - **`SkillSealPower` REAL** (antes placeholder no-op). Espeja el `SleepPower` de Oberon: el helper
    `Powers/Seal/Sello.cs` aplica el power y, si la intención YA roleada del enemigo es una HABILIDAD
    (`!Monster.IntendsToAttack`), la **cancela con `CreatureCmd.Stun`** (reemplaza el move por STUNNED;
    re-encola el original). El power, en `BeforeSideTurnStart` del enemigo sellado, re-chequea y cancela
    la habilidad de los turnos siguientes mientras dure, y decae 1/turno. Los ataques NO se cancelan
    (es Sello de HABILIDAD). `TidalSeal` y `NammuDuranki` ahora pasan por `Sello.Apply` → el control
    ocurre de verdad. *(El motor rola la intención al inicio de TU turno — `PrepareForNextTurn`/`RollMove`
    en `CombatManager` — así que al sellar la intención ya es visible, igual que el Sueño de Oberon.)*
  - **`TidalSwarmPower`** (loop ofensivo Lily, §3): mordida del enjambre al final de tu turno.
  - **`BroodMotherPower`** (rara «Nido de Cría»): parí 1 Laḫmu al inicio de tus turnos.
  - Pool Lily subido de 15 → 27 (cartas nuevas sobre los motores existentes; ver arriba).

## Pendientes antes de implementar
- Confirmar el NP `Nammu Dur-an-ki` (otra fuente dio "Nammu Marine Heart" — verificar Mooncell/JP).
- Aprobar números base; decidir dónde entra **RitsuLib** (estado por-carta del mazo efímero + telemetría para el balance de la ventana).
- Assets de la forma Lily (Larva/Tiamat Alter Ego) — el DESIGN-TIAMAT solo tiene Femme Fatale 9935400 + Bestia 9935410.
