# DESIGN-OBERON — diseño del personaje (Pretender, FGO) para StS2

Consolidado de las dos propuestas de diseño (`oberon_design_a.json` "el banco" + `oberon_design_b.json` "Deuda con interés/límite de crédito + Sueño con Insomnio"). **Decisión: FUSIÓN con design_a como esqueleto y los tres inventos más jugables de design_b injertados** — porque A tiene el pool más rico y la economía-banco más limpia, pero B resuelve mejor tres cosas que A dejaba flojas (motivación detallada en §0). Kit/assets en `oberon_kit.json` / `oberon_assets.json`. Diseñado con `sts2-mechanics-design`. Manifest id **`OberonPretender`** (inmutable), dependencies `["BaseLib","FGOCore"]`. Lore investigado JP+中文 (regla WORKFLOW-FGO §2: TYPE-MOON Wiki日本語 + Mooncell `fgo.wiki`).

---

## 0. Veredicto de fusión A vs B (qué gana cada uno y por qué)

| Eje | Gana | Motivo |
|---|---|---|
| **Esqueleto del pool** (cartas, raras, especiales) | **A** | A tiene el pool completo y mejor distribuido (20 com / 28 PC / 20 raras con arquetipos drafteables claros: Banca al Día, Largo Invierno, Insecto del Fin, Sueño del Mundo). B repite la mitad. |
| **Regla de Deuda** | **B (modificada)** | A cobra "10 NP por Deuda; impago = 3 HP". B agrega **interés +10/turno sobre lo impago** y **límite de crédito** (no podés endeudarte sin fin). El interés es el lore exacto ("el préstamo se cobra al despertar") y es el candado anti-degeneración que A confiaba sólo al HP. **Adopto B**: la Deuda se mide en **puntos de NP** (1 Deuda = 10 NP al cobro), con interés. |
| **Sueño / anti-stunlock** | **B** | A deja "Dormido" sin candado de re-aplicación → stunlock potencial. B agrega **Insomnio 2** (no re-dormible 2 turnos). **Adopto Insomnio** — es la perilla de riesgo #1 de ambos docs ya resuelta por código. |
| **Mecánica de forma Invierno** | **A** | A: "en Invierno la Deuda no se cobra **pero gana +1 de interés/turno**" — la bola de nieve es decisión real. B la congela del todo (menos tensión). **Adopto A** (con la unidad de B). |
| **Starter** | **A** | "cada Deuda pagada con NP → +N Estrellas" convierte el impuesto del kit en la 2ª economía. Limpio, calibra todos los riders. |
| **Número de mecánicas nuevas** | empate | Ambos: **2 grandes** (Deuda + Préstamo-tag) **+ 1 chica** (Sueño). Presupuesto §1.3 cumplido (≤2 propias + FGOCore + keyword). |

**Resultado:** Deuda **con interés y límite de crédito** (de B), unidad en puntos-de-NP, cobro→HP imparable, forma Invierno con **interés acumulativo** (de A), Sueño **con Insomnio** (de B), pool y reliquias **de A**.

---

## 1. Identidad en una frase

**El prestamista de sueños: el Rey Hada te regala la mejor noche de tu vida AHORA — baterías de NP enormes, estrellas, golpes sobre-tasa — y SIEMPRE la cobra al amanecer; su clímax es la mentira revelada: Vortigern, el insecto del fin, que deja de pagar y hace que el mundo pague la cuenta.**
Soporte/burst Buster de "préstamo con interés": cada buff es deuda diferida; cruza el umbral del ulti antes que nadie a cambio de sangrar mañana.

Trilingüe núcleo (Mooncell): **Oberon / Oberon / 奥伯龙** · **Vortigern / Vortigern / 沃提庚** · **Deuda / Debt / 借贷** · **Préstamo / Loan / 预支** (keyword) · **Dormido / Sleep / 沉睡** · **Insomnio / Insomnia / 失眠** · **El Rey del Cuento / The Storybook King / 童话之王** · **El Príncipe del Invierno / The Prince of Winter / 冬之王子**.

---

## 2. Inventario del kit FGO (mapeo 1:1 — la fidelidad manda, §4.1)

Servant 316, Pretender 5★, deck **QQABB** (Q4/A4/B3/Ex6; NP Buster 4 hits), star gen 20,5%, NP gain 0,59%/hit. Stats lv90 ATK 11.810 / HP 13.266 (estatuilla de soporte). **NO tiene Rank-Up Quests ni Interludes** (verificado Atlas Academy 2026-06) → no hay "rank-up-as-upgrade" estilo Siegfried; los upgrades de carta son tuning normal.

| Elemento FGO real | Traducción en el mod | Tipo |
|---|---|---|
| **S1 Evening Shroud EX** (+NP dmg party 3t + carga 20% party; el ÚNICO skill sin demérito) | **«Velo de la Noche»** poco común, sin Exhaust ni demérito | carta KIT |
| **S2 Morning Lark EX** (carga +50% a un aliado, demerit: drain 20% al final del turno; +C.Stars; +crit propio) | **FIRMA «Alondra de la Mañana»** — el préstamo modelo: +50 NP, +10★, **Deuda** | básica firma |
| **S3 Ending of Dreams EX** (apoteosis 1 turno → pierde TODOS sus buffs → Sueño Eterno permanente) | **«El Fin de los Sueños»** rara clímax → turno-dios → pierde todos los Poderes → forma **Vortigern** | rara KIT |
| **NP jugable Rye Rhyme Goodfellow** (AoE Buster, remueve buffs de ataque, Sleep a todos, demerit Invencible a todos; OC escala anti-[Lawful]) | **cartas-NP «Rye Rhyme Goodfellow»** + ulti auto Desatado: strip de Fuerza + AoE + Dormir a todos; OC doble vs Élite/Jefe (la Ley del Spire) | NP / ulti |
| **NP verdadero Lie Like Vortigern** (solo-lore: el dragón se traga el mundo) | **rara «Lie Like Vortigern»** (transformación permanente) + ulti Desatado exclusivo de Vortigern | rara / ulti |
| **Pasiva ??? (id 900250)**: +10% éxito de debuffs al party; demerit anti-Merlin | reliquia **«???»** + chiste anti-Merlin de flavor (no hay Merlin en el roster) | reliquia |
| **Territory Creation E−** (el rango más bajo del juego) | carta meme **«Creación de Territorio E−»** (2 de Bloqueo) | común meme |
| **Item Construction A+** (rocío tricolor con que maldijo a Titania) | carta **«Rocío Tricolor»** + poder **«Construcción de Ítems A+»** | PC |
| **Riding A** (monta una polilla halcón a 130 km/h) | reliquia **«Polilla Halcón»** + cartas de Quick/robo | reliquia |
| **Midsummer Night's Dream EX** (inmunidad permanente a debuffs mentales y Maldición) | reliquia **«Sueño de una Noche de Verano»** (inmune a Maldición + anula 1er debuff/combate) | reliquia |
| **Suerte EX / star gen 20,5%** | poder **«Suerte EX»** + sesgo a Estrellas del mazo inicial (doble Quick) | PC/poder |
| **Threat to Humanity** (rasgo Beast) | poder **«Amenaza para la Humanidad»** | PC |

---

## 3. Recurso / motor del personaje — EL BANCO

Cuatro economías. Dos universales FGO + dos propias + un estado chico.

### 3.a — Carga NP (FGOCore, universal)
0–300; a **100** se auto-manifiesta GRATIS la **Desatada** de la forma activa (`GaugeFilled`), queda hasta caer bajo 100 (`GaugeDropped`); al jugarse consume **TODA** la carga (`NpCharge.ConsumeAllForNpCard(creature, minCost, source)`, verificado `NpCharge.cs:79`) y escala por Sobrecarga (PerTen). +15%/nivel con `NpLevels` (dupes).

### 3.b — Estrellas de Crítico (FGOCore `CritStarsPower`/`CritReadyPower`, compartida)
Umbral **100 → 1 CRÍTICO LISTO** (próximo Ataque ×2), auto-payoff (el de Morgan/Jeanne, NO el gateado de Artoria). Oberon la alimenta con el **doble Quick** del mazo inicial y con los **pagos de Deuda** vía la starter.

### 3.c — DEUDA (mecánica propia, power contador `DebtPower`)
La identidad. **Regla determinista en una frase:**

> **«Cada carta de PRÉSTAMO te da recurso AHORA y suma DEUDA. Al FINAL de tu turno, tu Carga NP paga tu Deuda (10 NP por punto de Deuda); la Deuda que no puedas pagar PERSISTE, gana +10 de interés (+1 punto) al turno siguiente, y por cada punto impago perdés 3 de Vida (imparable, `ValueProp.Unblockable|Unpowered`).»**

- **Unidad:** 1 punto de Deuda = **10 NP** al cobro = **3 HP** si impago. (Préstamo chico = Deuda 1 / +30 NP neto; estándar = Deuda 2 / +50; hipoteca = Deuda 4 / +100.)
- **Límite de crédito (de B, candado estructural):** con Deuda actual **≥5** (50 NP-equivalente) no podés jugar cartas de Préstamo — **el glow se apaga** (crédito cortado, feedback visible). Sube a 10 con la reliquia de jefe / Trono del Invierno.
- **La gracia:** la Deuda sólo grava el NP que **ahorrás**. Si gastás el préstamo en cartas-NP ese mismo turno, el cobrador encuentra la billetera vacía → la Deuda impaga sangra HP **e interés**. La decisión de cada turno: **pagar puntual** (retener NP, ulti más lento, pero la starter convierte cada pago en estrellas) **vs disparar el ulti YA** (consume todo → impago → sangrás + interés).
- **La matemática honesta** (techo §1.bis, regla §1.1: número sobre-tasa SÓLO embudado por mecánica propia): `«0⚡: +50 NP. Deuda 2»` rinde **NETO +30** (= una Arts básica; el flat de Jeanne Moli1 0⚡+50-sin-costo es el techo declarado). El préstamo **no da más TOTAL, da más AHORA**: cruzás los 100 antes, el ulti gratis llega antes y más seguido. El riesgo de impago (HP + interés) es el costo real.

### 3.d — SUEÑO (estado chico, `SleepPower` + `InsomniaPower`)
> **«Dormido: el enemigo no actúa en su próximo turno y TODO el daño que recibiría se anula (intocable, en el mundo de los sueños); despierta al final de ese turno y gana Insomnio 2 (no re-dormible 2 turnos).»**

El demérito de Invencible del NP real fundido en un solo estado: dormís al que pega fuerte mientras armás el motor, **nunca al que querés matar este turno** (perdés tu propia ventana de daño = tradeoff temático, tempo-neutral no tempo-positivo). **Insomnio 2** = imposible el stunlock. **Vortigern lo invierte**: sus Ataques golpean a los Dormidos SIN despertarlos (el abismo los devora en el sueño).

### Loop esperado
Pedir prestado en **Rey del Cuento** (Alondra/Favor del Rey/Anticipo) → cruzar 100 → **decidir: pago puntual** (→ estrellas vía starter → camino al Crítico Listo) **o disparo el ulti YA** (impago + interés) → **ventanas de Invierno** para acumular sin pagar (con interés acechando) → **dormir** la amenaza grande para comprar turnos → **clímax**: transformarse en **Vortigern** y declarar default contra el mundo (Deuda → daño AoE), o guardar 300 para **El Fin de los Sueños** (el mejor último sueño, turno-dios). Arquetipos drafteables: **(1) Banca al Día** (préstamos + pago puntual → estrellas → críticos), **(2) El Largo Invierno** (diferir + Deuda como activo: Negociación Feérica, Pagaré del Apocalipsis), **(3) El Insecto del Fin** (Vortigern default engine), **(4) Sueño del Mundo** (Dormido + saqueo de recursos a los dormidos).

---

## 4. Mapeo a FGOCore (reusa vs NUEVO)

### Reusa SIN cambios estructurales
- `NpCharge` 0-300 + `GaugeFilled`/`GaugeDropped` (la reposesión del ulti por drain es comportamiento emergente del evento existente) + `ConsumeAllForNpCard` + Sobrecarga PerTen (`NpCharge.cs:79` verificado).
- `OverchargeBlessingPower` (mapeo directo del "+30% NP damage" de Velo / Fin de los Sueños).
- `INpCostWaiver` (excluye Event ✓).
- `CritStarsPower`/`CritReadyPower` + helper `CritStars` (economía compartida; `CritStarsPower.cs:50` aplica `CritReadyPower` a 100).
- `FormPower`/`FormSwitch`/`FormVisuals`/`FormShiftedPower`/`IFormChangeListener` (3 formas; manifestación de ulti por forma vía handler `GaugeFilled`, patrón Morgan).
- `BondRelic` (×1.25 daño/bloqueo global — el techo §1.bis va por la palanca central, SIN ×global por carta).
- `INpLevelStore`/`NpLevels` (+15%/nivel, dupes).
- `GoldenRulePower` (no aplica directo — Oberon no tiene Golden Rule; ignorar).
- `CursePower`/`ICursePreserver` (SÓLO para la inmunidad de la reliquia «Sueño de una Noche de Verano»).
- `ILimitBreaker` (Santo Grial).

### NUEVO EN FGOCORE (compartible — futuros servants duermen gente: Merlin, Euryale, Semiramis, Aśvatthāman)
- **`SleepPower`** («Dormido»: skip de intención + **anulación total del daño recibido** — implementar la anulación **ANTES de Bloqueo** vía el preventer correcto; **gotcha de hooks ABSOLUTOS** `ModifyHpLost*` igual que `GutsPower` y el orden de la Hoja de Tilo de Siegfried; cuenta el despertar con evento `AfterAwakened` para los lectores).
- **`InsomniaPower`** (contador 2, bloquea re-aplicación de Sleep) + helper estático `Sleep.TryApply`/`IsAsleep`.
- Loc trilingüe en core + iconos: **icono de ESTADO real** sacado de `buffs[].icon` del JSON del NP 2800101 en Atlas (regla del workflow §5 — no inventar paths).
- **Bases de cartas de comando `BusterCardBase`/`ArtsCardBase`/`QuickCardBase`** (PENDIENTE compartido ya especificado para el rediseño Morgan — verificado por grep que NO existen aún; crearlas en `FGOCore/Cards/Command`, Oberon las subclasea con `[Pool]` propio y arte de `card_servant_1.png` en 3 bandas).
- Helper **`CritReady.TryConsume` con flag de supresión** (lo exige «Avalancha de Escamas» — parche P8 de Morgan).

### LOCAL al mod (NO contamina core)
- **`DebtPower`** (contador cap 15; cobro en el hook de **fin de turno del lado jugador**; NP primero a 10/punto, fallback 3 HP `Unblockable|Unpowered`; interés +1/turno impago; límite de crédito 5; evento `AfterDebtPaid(amount)` para starter/Euforia Nocturna/Telar; pasiva Vortigern reusa el evento estilo `IFormChangeListener`).
- **Keyword de tribu `PRÉSTAMO`** (CustomEnum CardKeyword, tag VISIBLE no-gate sobre toda carta que genera Deuda, glow integrado).
- **3 FormPowers de Oberon** (Rey del Cuento / Príncipe del Invierno / Vortigern) con `FramesPath` re-overrideando rutas al pck propio (regla `MashFormPower`).
- **Handler de ulti-por-forma** (qué Desatada manifiesta `GaugeFilled` según la forma activa).

**Recordatorio de publicación:** FGOCore + TODOS los personajes se publican JUNTOS (la firma de `NpCharge` ya cambió una vez — regla §4.5).

---

## 5. Formas (FGOCore — el kit las pide: 3 modelos de batalla = dramaturgia pura)

Modelos verificados HTTP 200 (`oberon_assets.json`). La starter hace `FormSwitch.Enter<RaeyDelCuento>(...)` en `BeforeCombatStartLate` (fija forma inicial + dispara precarga `FormVisuals`, gotcha Morgan v2). Cada forma cambia las **DECISIONES**, no sólo los números (§5).

| Forma | Modelo | Pasiva (flag en el FormPower) | Decisión que cambia |
|---|---|---|---|
| **EL REY DEL CUENTO** (inicial) | **2800100** (alas de mariposa radiantes) | «La **primera carta de Préstamo** que jugás cada turno: **+1 punto de Deuda → +10 NP extra** (el cuento endulza el trato).» | Ordenar los préstamos primero; jugar limpio, pagar puntual. |
| **EL PRÍNCIPE DEL INVIERNO** | **2800110** (capa blanca de plumas) | «Al final de tu turno tu Deuda **NO se cobra**; en su lugar gana **+1 de interés** (bola de nieve). Tus payoffs dan **Bloqueo**.» | La estafa larga: retener TODO el medidor 1-2 turnos para reventar 100/200 sin impuesto, sabiendo que la bola te espera a la salida. Forma defensiva. |
| **VORTIGERN** (clímax PERMANENTE `IsPermanent`) | **2800120** (alas venosas de libélula; visual asc 3 siniestro) | «Tus Ataques hacen **+3** y golpean a los Dormidos **SIN despertarlos**. Al final de tu turno, hasta **5 puntos de Deuda impaga** infligen **2 de daño a TODOS** por punto **en vez de quitarte Vida**; el resto se cobra normal (NP primero). **Pierde el endulzante del Rey.**» | INVERTIDA: ahora querés estar en cero — fundir el medidor cada turno para declarar **default** y que el mundo pague. El Sueño deja de ser defensa y se vuelve setup ofensivo. |

**Manifestación de ulti por forma** (handler `GaugeFilled`, patrón Morgan): Rey/Invierno → **«Rye Rhyme Goodfellow: Desatado»**; Vortigern → **«Lie Like Vortigern: Desatado»**.
**Cambios de forma esperados por combate: 2-4** (firma-toggle «Caer la Noche», común «Atajo del Bosque»/«El Regreso del Príncipe», PC «Truco de Robin Goodfellow», y **una sola vez** la transformación a Vortigern). Acceso barato ida-y-vuelta entre Rey↔Invierno (regla §5: 0-1⚡ + efecto inmediato chico).

---

## 6. Pool COMPLETO de cartas

**Gramática de denominaciones (regla Jeanne 10/20/30/50/100):** NP — 10 rider · 20 paquete · 30 básica Arts · 50 préstamo estándar/espejo · 100 jackpot/umbral. Estrellas — idéntico, 100 = umbral auto-crit. **DEUDA** — 1 punto = 10 NP = 3 HP; préstamo chico 1, estándar 2, hipoteca 4; cap 15, límite de crédito 5, interés +1/turno. **GLOW DORADO** (`ShouldGlowGoldInternal`) en TODA condicional: riders de Deuda, condicionales de forma, espejos pagables, lectores de Dormido, NPs jugables, y **crédito cortado** (préstamos dejan de brillar a Deuda ≥5). Básicas de comando con arte = `card_servant_1.png` en 3 bandas.

### 6.1 — Básicas (mazo inicial QQABB fiel) — 8 cartas
| Carta (eng / esp / zhs) | Coste | Tipo | Efecto | up |
|---|---|---|---|---|
| **Buster / Buster / Buster** | 1⚡ | Ataque | 10 de daño. | +3 |
| **Arts / Arts / Arts** | 1⚡ | Ataque | 6 de daño; +30 Carga NP. | +3 / +20 |
| **Quick / Quick / Quick** | 1⚡ | Ataque | 6 de daño; +30 Estrellas. **(×2 en el mazo inicial — único doble-Quick del roster, star gen 20,5% real)** | +3 / +20 |
| **Strike / Golpe / 打击** | 1⚡ | Ataque | 6 de daño. (mantiene vivo el tag Strike, lección P6 Morgan) | +3 |
| **Defend / Defender / 守护** | 1⚡ | Habilidad | 5 de Bloqueo. | +3 |
| **FIRMA — King Fae's Word / «Palabra del Rey Hada» / 妖精王之言** | 1⚡ | Habilidad · **PRÉSTAMO** | +30 Carga NP; robá 1. **Deuda 1.** | +40 NP |
| **FIRMA — Morning Lark / «Alondra de la Mañana»** (S2) / 朝之云雀 | 1⚡ | Habilidad · **PRÉSTAMO** | +50 Carga NP; +10 Estrellas. **Deuda 2.** (la aritmética EXACTA de Morning Lark: +50 ya, −20 al final = neto +30 si ahorrás) | +20★ / Deuda 2→1 |
| **FIRMA — Nightfall / «Caer la Noche» / 夜幕降临** | 1⚡ | Habilidad | 4 de Bloqueo; **alterná** entre Rey del Cuento ↔ Príncipe del Invierno. (toggle, patrón Truco del Clan del Espejo: la danza día/noche desde el combate 1, evita atascarse en Invierno) | 7 |

**Mazo inicial (10):** 2× Buster + 1× Arts + **2× Quick** + 1× Golpe + 2× Defender + 1× «Palabra del Rey Hada» + 1× «Caer la Noche». Las 6 cartas de comando + 2 firmas codifican las 4 economías (daño/NP/estrellas/Deuda+formas) desde el turno 1. **Gana el acto 1 sin motor ✓** (26 de daño en comandos + firma que acelera el primer ulti a turno ~3-4). *(Nota: «Alondra» y «Caer la Noche» son las dos firmas; «Palabra del Rey Hada» entra como la 2ª firma de batería para el doble-préstamo desde el turno 1 — total 2 firmas + 1 toggle, dentro de plantilla.)*

### 6.2 — Comunes (20)
| Carta | ⚡ | Tipo | Efecto | up | engranaje |
|---|---|---|---|---|---|
| **Scale Dust / «Polvo de Escamas» / 鳞粉** | 1 | At | 6 de daño; +10 Estrellas. | +3/+10 | estrellas |
| **White Lie / «Mentira Piadosa» / 善意的谎言** | 1 | At · PRÉSTAMO | 12 de daño. **Deuda 1.** (sobre-tasa común 9-10 pagada por Deuda 1 ≈ ½⚡ diferido) | 16 | préstamo-daño |
| **Dawn's Advance / «Anticipo del Alba» / 黎明预支** | 0 | Hab · PRÉSTAMO | +50 Carga NP. **Deuda 2.** (el Moli1 con interés: neto +30, ráfaga 50) | +60 | préstamo |
| **Empty Promise / «Promesa Vacía» / 空头承诺** | 0 | Hab · PRÉSTAMO | +50 Estrellas. **Deuda 2.** (préstamo de estrellas, cobrado en NP: interés cruzado) | +60 | préstamo-★ |
| **Dreamlike Mist / «Niebla del Ensueño» / 梦境之雾** (espejo A) | 0 | Hab | si ≥50 NP: −50 Carga → +50 Estrellas. Glow. | consume 40 | conversión (P5: up consume 40) |
| **Fae Tribute / «Tributo de los Fae» / 妖精的贡品** (espejo B) | 0 | Hab | si ≥50 Estrellas: −50 → +50 Carga NP. Glow. | consume 40 | conversión |
| **Moth Flight / «Vuelo de la Polilla» / 飞蛾之翼** | 1 | At | 5 de daño ×2; +10 Estrellas. | 7×2 | multi+★ |
| **Iron Eyes / «Ojos Férricos» / 铁色之眼** (Fae Eyes) | 0 | Hab | 1 Vulnerable; +5 NP. | +10 NP | debuff (alimenta «???») |
| **Early Collection / «Cobro Adelantado» / 提前催收** | 1 | Hab | pagá YA hasta 3 puntos de tu Deuda (con NP, 10 c/u); robá 1. Glow si tenés Deuda. (pagar en TU horario → dispara las estrellas de la starter cuando querés + desactiva el cobro de fin de turno) | robá 2 | pago activo |
| **Sweet Words / «Palabras Dulces» / 甜言蜜语** | 0 | Hab · Exhaust | remové 1 punto de Deuda. (condonación; Exhaust paga el perdón §3) | 2 | anti-Deuda |
| **Dusk Song / «Canto del Atardecer» / 黄昏之歌** | 1 | Hab | 6 de Bloqueo; +10 NP. | +3/+5 | bloqueo+NP |
| **Feather Coat / «Abrigo de Plumas» / 羽衣** | 1 | Hab | 5 de Bloqueo; en Invierno: +4. Glow. | 8/+5 | payoff Invierno |
| **Bedtime Story / «Cuento Antes de Dormir» / 睡前故事** | 1 | Hab | robá 2; +10 NP. | +20 NP | robo+NP |
| **Dragonfly Wings / «Alas de Libélula» / 蜻蜓之翼** | 1 | At | 8 de daño; en Invierno: +10 Estrellas. Glow. | 11 | payoff Invierno |
| **King's Favor / «Favor del Rey» / 国王的恩宠** | 1 | At | 8 de daño; si tenés Deuda: +4. Glow. (rider calibrado: la firma siempre lo enciende) | 11/+5 | Deuda-lector |
| **Forget-me-not Petals / «Pétalos de Nomeolvides» / 勿忘草花瓣** | 1 | Hab | 4 de Bloqueo; +10 Estrellas. | 6/+10 | bloqueo+★ |
| **Prince's Smile / «Sonrisa del Príncipe» / 王子的微笑** | 0 | Hab · Exhaust | +20 NP. | +30 | burst NP |
| **March of the Insect-Fae / «Marcha de las Hadas-Insecto» / 虫之妖精的行军** | 1 | At | 4 de daño a TODOS; +10 NP. (AoE −30% + rider) | 6 | AoE+NP |
| **Territory Creation E− / «Creación de Territorio E−» / 阵地作成E−** | 0 | Hab | 2 de Bloqueo. *«El rango más bajo del juego: rey solo de nombre.»* | 4 y +5 NP | meme (semi-neutral permitido) |
| **Ride the Hawk Moth / «Cabalgar la Polilla Halcón» / 骑乘鹰蛾** | 1 | Hab | robá 1; +20 Estrellas (130 km/h cuando nadie mira). | robá 2 | ★+robo |

**Conectividad: 19/20 ≥ 95% ✓** (sólo Territory E− está fuera, a propósito — el meme permitido).

### 6.3 — Poco comunes (28)
| Carta | ⚡ | Tipo | Efecto | up |
|---|---|---|---|---|
| **KIT Evening Shroud EX / «Velo de la Noche»** (S1) / 夜之帷幕 | 1 | Hab · **sin demérito** | +20 NP; 3 turnos tus cartas NP hacen +30% de daño. (Co-op: aliados +10 NP.) **El ÚNICO skill limpio del kit real** | +30/+40% |
| **KIT Morning Lark EX (alt) / «Alondra de la Mañana+»** (variante draft, S2) | 1 | Hab · Exhaust · PRÉSTAMO | +50 NP; +20 Estrellas. **Deuda 2.** | +60/+30 |
| **Royal Loan / «Préstamo Real» / 王室借贷** | 1 | Hab · PRÉSTAMO | +50 NP; robá 1. **Deuda 2.** | robá 2 |
| **False Starfall / «Lluvia de Estrellas Falsas» / 伪星之雨** | 1 | Hab · PRÉSTAMO | +50 Estrellas; robá 1. **Deuda 2.** | robá 2 |
| **PODER Nocturnal Euphoria / «Euforia Nocturna» / 夜之欢愉** | 1 | Poder | al final del turno, si no te queda Deuda impaga: +10 NP y +10 Estrellas. (premia solvencia; lore S1) | +15/+15 |
| **PODER Interest in My Favor / «Interés a Mi Favor» / 利息归我** | 2 | Poder | cuando jugás una carta de Préstamo: +10 NP. | +10 NP y +10★ |
| **The Piper of Autumn Wood / «El Flautista del Bosque de Otoño» / 秋之森的吹笛人** | 2 | At | 10 de daño a TODOS; si tenés Deuda: +3. Glow. | 14/+4 |
| **Sting of Resentment / «Aguijón del Resentimiento» / 怨恨之刺** | 1 | At | 12 de daño; +5 NP. | 16/+10 |
| **Fortune EX / «Fortuna EX» / 幸运EX** | 1 | Hab | +30 Estrellas; robá 1. (Suerte EX) | +40 |
| **Midnight Assault / «Asalto de Medianoche» / 午夜突袭** | 1 | At | 7 de daño; si tenés CRÍTICO LISTO: +20 NP. Glow. (cose estrellas→NP) | 10/+30 |
| **Robin Goodfellow's Trick / «Truco de Robin Goodfellow» / 好人罗宾的恶作剧** | 0 | Hab | alterná Rey ↔ Invierno; robá 1. (switch premium, patrón Liebre Blanca) | robá 2 |
| **Tricolor Dew / «Rocío Tricolor» / 三色之露** (love-in-idleness) | 1 | Hab · Exhaust | 2 Débil y 2 Vulnerable a un enemigo; +10 NP (el rocío que maldijo a Titania). | 3 y 3 |
| **PODER KIT Item Construction A+ / «Construcción de Ítems A+» / 道具作成A+** | 1 | Poder | tus Débil y Vulnerable aplican +1 stack. | además +5 NP cada vez |
| **PODER Insect's Vigil / «Vigilia del Insecto» / 虫之守望** | 1 | Poder | al inicio de tu turno: +10 NP. | +10 NP y +5★ |
| **Light Sleep / «Sueño Ligero» / 浅眠** | 1 | Hab | 8 de Bloqueo; si hay un enemigo Dormido: robá 2. Glow. | 11 |
| **While the World Sleeps / «Mientras el Mundo Duerme» / 当世界沉睡** | 1 | Hab | +20 NP; +20 más por cada enemigo Dormido. Glow. (el turno de sueño es para armar los libros) | +30 base |
| **A Winter's Tale / «Cuento del Invierno» / 冬天的故事** | 2 | Hab | 13 de Bloqueo; si tenés Deuda: +4. Glow. | 17/+5 |
| **Black Butterflies / «Mariposas Negras» / 黑蝶** | 1 | At | 3 de daño ×3 a enemigos aleatorios; +5 NP por golpe que dañe Vida. | 4×3 |
| **The Swallowed Storm / «La Tormenta Tragada» / 吞下的风暴** | 2 | Hab · Exhaust | 20 de Bloqueo; +10 NP (se traga la Tormenta entera). | 26/+20 |
| **PODER Threat to Humanity / «Amenaza para la Humanidad» / 人类之威胁** | 2 | Poder | cuando un enemigo muere: +20 NP y +20 Estrellas. | +30/+30 |
| **Two-Faced / «Doble Cara» / 两面三刀** | 1 | At | 6 de daño; si cambiaste de forma este combate: +6. Glow. | 8/+8 |
| **But a Dream / «Solo un Sueño» / 不过是个梦** (no more yielding but a dream) | 1 | Hab · Exhaust | remové tus debuffs; +10 NP. | además 1 Artefacto |
| **Autumn Wood Harvest / «Cosecha del Bosque de Otoño» / 秋之森的收获** | 1 | Hab | +20 NP y +20 Estrellas. | +30/+30 |
| **Mortgage Tomorrow / «Hipoteca del Mañana» / 抵押明天** | 2 | Hab · Exhaust · PRÉSTAMO | +100 Carga NP. **Deuda 4.** (el jackpot: ulti instantáneo; neto +60 pagado con 2⚡+Exhaust §3) | Deuda 3 |
| **PODER Night Reading / «Lectura Nocturna» / 夜读** | 2 | Poder | al inicio de tu turno: robá 1 adicional. (la velocidad de mazo es el combustible del banco, P10 Morgan) | 1⚡ |
| **Thorn of the Abyss / «Espina del Abismo» / 深渊之棘** | 2 | At · PRÉSTAMO | 18 de daño. **Deuda 2.** (18 a 2⚡ = baseline 'con downside'; el downside es el pagaré) | 24 |
| **Faerie Negotiation / «Negociación Feérica» / 妖精交涉** | 0 | Hab | +10 Estrellas por cada Deuda que tengas (máx 3); tu Deuda NO se reduce. Glow. (la deuda como ACTIVO: sinergia Invierno) | máx 5 |
| **Absolute Disguise / «Disfraz Absoluto» / 绝对伪装** | 1 | Hab · Exhaust | 1 Intangible (ni la Clarividencia de Merlin lo percibe). (precedente 幻术 de Jeanne) | además +20 NP |

*(28 PC; cubren los 4 arquetipos: Banca, Invierno, Insecto/Vortigern, Sueño del Mundo.)*

### 6.4 — Raras (20)
| Carta | ⚡ | Tipo | Efecto | up |
|---|---|---|---|---|
| **NP Rye Rhyme Goodfellow / «Rye Rhyme Goodfellow» / 彼方にかざす夢の噺・向彼方高举的梦之话** | 2 (mín 70, consume TODA) · Exhaust | Ataque NP | 22 de daño a TODOS; removés la Fuerza positiva de todos; **todos se DUERMEN**. SOBRECARGA: +3 por cada 10 sobre el mínimo (**doble vs Élites/Jefes** — anti-[Orden]). Glow al gate. | 28 |
| **KIT Ending of Dreams EX / «El Fin de los Sueños»** (S3, CLÍMAX) / 梦之终结EX | 2 · Exhaust | Hab | este turno tus Ataques hacen +50% y tu próxima carta NP hace **×2**; al final del turno: perdés TODOS tus Poderes positivos y NO robás cartas en tu próximo turno (caés en el Sueño). Glow si ≥70 NP. (NO carga NP — corrección de fama; demérito irrenunciable, tasa 500%; co-op: apuntable a aliado) | 1⚡ |
| **Lie Like Vortigern / «Lie Like Vortigern» / 谎言如沃提庚** | 2 · Exhaust | Poder | consumí TODA tu Deuda: 3 de daño a TODOS por punto consumido; entrás en **VORTIGERN** (permanente). (el NP verdadero solo-lore como carta-clímax, patrón Coronación de Invierno) | 1⚡ |
| **Lullaby of the End / «Canción de Cuna del Fin» / 终焉的摇篮曲** | 2 · Exhaust | Hab | un enemigo se DUERME. (el stun disfrazado; precedente 神明裁决) | 1⚡ |
| **Abyssal Fairy Dust / «Polvo de Hadas del Abismo» / 深渊妖精尘** | 1 · Exhaust | Hab | +50 NP y +50 Estrellas. | +60/+60 |
| **PODER Luck EX / «Suerte EX» / 幸运EX** | 1 | Poder | al inicio de tu turno: +20 Estrellas. | +30 |
| **PODER Wings of Reverie / «Alas del Ensueño» / 梦想之翼** | 2 | Poder | cuando jugás una carta NP: +20 Estrellas y robá 1. (los finishers realimentan la economía) | +30 |
| **Scale Avalanche / «Avalancha de Escamas» / 鳞雪崩** | 2 | At | 7 de daño ×3 a un enemigo; si tenés CRÍTICO LISTO: lo consume y **LOS TRES golpes critican**. Glow. (requiere `CritReady.TryConsume` con supresión — P8 Morgan) | 9×3 |
| **Impossible Confession / «Confesión Imposible» / 无法说出口的告白** | 1 · Exhaust | Hab | curás 8; removés tus debuffs (*«No tengo nada que me guste» — también mentira*). | 12 |
| **Sovereign Debt / «Deuda Soberana» / 主权债务** | 1 · PRÉSTAMO | At | +100 Estrellas (dispara el umbral → CRÍTICO LISTO), LUEGO 12 de daño (sale ×2). **Deuda 4.** (la carta-combo modelo, patrón 黑闪) | 16 |
| **The Worm of the Abyss / «El Gusano del Abismo» / 深渊之虫** | 2 | At | 10 de daño a TODOS; en VORTIGERN: +8. Glow. (1.440 km de longitud) | 13/+10 |
| **Dragon's Blender-Maw / «Boca-Licuadora del Dragón» / 龙之绞肉口** | 3 | At | 30 de daño; si Deuda ≥3: +10 (la deuda alimenta al gusano). Glow. | 38/+12 |
| **PODER Happy Ending Plan / «Plan del Final Feliz» / 幸福结局计划** | 2 | Poder | al jugarla +2 de Bendición de Sobrecarga; al inicio de cada turno: +1. (`OverchargeBlessingPower`: empuja los ultis a ≥150 → el sueño masivo) | +3 al jugar |
| **The Ending Where Britain Has a Future / «El Final donde Britania Tiene un Futuro» / 不列颠拥有未来的结局** | 2 · Exhaust | Hab | 25 de Bloqueo; remové hasta 3 puntos de Deuda. (el reset: aún traicionando, ejecuta el final bueno) | 32 / hasta 5 |
| **Collecting Names / «Acumular Nombres» / 收集名号** | 2 | Hab | robá 3; +20 NP (Robin Goodfellow, Príncipe del Invierno…). | robá 4 |
| **PODER Court of Insect-Fae / «Corte de las Hadas-Insecto» / 虫之妖精的宫廷** | 2 | Poder | al inicio de tu turno: añadí una «Palabra del Rey Hada» a tu mano (cuesta su 1⚡ normal). (el pool se cita a sí mismo; costo real para no regalar préstamos gratis) | 1⚡ |
| **End of the Tale / «El Final del Cuento» / 故事的结局** | 2 | At | 16 de daño; si mata: +50 NP y +50 Estrellas. | 22 |
| **Apocalypse IOU / «Pagaré del Apocalipsis» / 末日借据** | 1 | At | 8 de daño; +4 por cada Deuda que tengas (máx +20). Glow. (Deuda como activo, payoff escalar) | 10 / +5 por Deuda |
| **PODER Vespers of the End / «Vísperas del Fin» / 终焉的晚祷** | 1 | Poder | en VORTIGERN: tus Ataques hacen +3. Glow. | +5 |
| **A Dream Raised to the Beyond / «Cuento de un Sueño Alzado hacia el Más Allá» / 向彼方高举的梦之话** | 1 | Hab | +30 NP; si hay un enemigo Dormido: +30 más y robá 1. Glow. (el nombre real del NP) | +40 base |

### 6.5 — Tokens / especiales (generadas, no drafteables directo)
| Carta | Disparo | Efecto |
|---|---|---|
| **«Rye Rhyme Goodfellow: Desatado» / 解放** | auto a 100 NP en Rey/Invierno (`GaugeFilled`) | Ataque NP **0⚡**, Exhaust: 30 de daño a TODOS; removés la Fuerza positiva de todos; consume TODA la carga. **SOBRECARGA: +3 por cada 10 sobre 100 (DOBLE contra Élites/Jefes — la Ley del Spire)**; a sobrecarga **≥150: todos los enemigos se DUERMEN** (el sueño masivo vive en la sobrecarga adrede — lección P1 Morgan: los riders planos de un ulti pagan por FRECUENCIA). +15%/nivel `NpLevels`. |
| **«Lie Like Vortigern: Desatado» / 解放** | auto a 100 NP en Vortigern (`GaugeFilled`) | Ataque NP **0⚡**, Exhaust: 35 de daño a TODOS; **consume TODA tu Deuda: +3 de daño por punto consumido** (ignora Dormido sin despertar). SOBRECARGA: +4 por cada 10 sobre 100. Sin sueño — el dragón no arrulla, devora. +15%/nivel. |
| **Power «Deuda» / Debt / 借贷** | sistema | contador cap 15; cobro al final del turno propio (NP primero a 10/punto, impago 3 HP `Unblockable|Unpowered` + interés +1); límite de crédito 5; icono de estado real (bufficon del NP-drain del NP 2800101). |
| **Power «Dormido» / Sleep / 沉睡** + **«Insomnio» / Insomnia / 失眠** | sistema | Dormido: pierde su próxima acción + daño recibido → 0 mientras duerme; despierta al final de esa acción → Insomnio 2 (no re-dormible). Iconos = bufficon real de `buffs[].icon` del NP 2800101. |
| **Keyword «PRÉSTAMO» / Loan / 预支** | sistema | CustomEnum CardKeyword, tag visible (no gate) + glow integrado sobre toda carta que genera Deuda. |
| **3 FormPowers** (Rey del Cuento / Príncipe del Invierno / Vortigern) | sistema | `FramesPath` a los modelos 2800100/2800110/2800120. |

---

## 7. Reliquias

| # | Rareza | Nombre (eng / esp / zhs) | Efecto |
|---|---|---|---|
| 1 | **STARTER (motor)** | The Dream Contract / **«El Contrato de Sueños»** / 梦境契约 | (1) Al iniciar cada combate entrás en **El Rey del Cuento** (`FormSwitch.Enter` en `BeforeCombatStartLate` → fija forma + dispara precarga `FormVisuals`, gotcha Morgan v2). (2) Cada punto de Deuda que **PAGÁS con NP** al final del turno: **+10 Estrellas** (máx 3 procs/turno, reset en `AfterSideTurnStart`). Convierte el impuesto del kit en la 2ª economía y calibra el flujo: pagar 2-3 = 20-30★ ≈ una Quick gratis. Icono: clase **Pretender ORO 5★** (`Pretendergold.png` con `&format=original` / Mooncell 金卡Pretender). |
| 2 | **STARTER (BondRelic)** | Chronicle of Avalon le Fae / **«Crónica de Avalon le Fae»** / 妖精国阿瓦隆编年史 | vínculo estándar (+2/+3/+5 por victoria, +1 por sala) heredando `ServantDamage`/`BlockMultiplier` **×1.25 global** (el techo §1.bis — palanca central, NO ×global por carta). Overrides: Nv4 empezás cada combate con +20 NP; Nv7 +20 NP y +20 Estrellas; capstone Nv10 «El Final Feliz»: al inicio de cada combate +2 de Bendición de Sobrecarga. |
| 3 | **STARTER OCULTA (`INpLevelStore`)** | Forget-me-not of Autumn Wood / **«Nomeolvides del Bosque de Otoño»** / 秋之森的勿忘草 | dupes/NP level 1-5, pity 50%+25%, botón Invocar; **+15%/nivel a todas las cartas NP** (`NpLevels.Scale`). |
| 4 | **ANCIENT/JEFE** (reemplaza la starter de motor) | The Book of Dream's End / **«El Libro del Fin de los Sueños»** / 梦终之书 | cada Deuda pagada: **+10 Estrellas (máx 5/turno)** Y la primera Deuda impaga de cada turno NO te quita Vida (sólo interés); límite de crédito sube a 10. |
| 5 | **TIENDA** | Usurer's Purse / **«Monedero del Prestamista»** / 放贷人的钱袋 | tus cartas de Préstamo dan además +10 Estrellas. |
| 6 | **POCO COMÚN** | Hawk Moth / **«Polilla Halcón»** (Riding A) / 鹰蛾 | la primera carta Quick que jugás cada turno: robá 1. |
| 7 | **POCO COMÚN** | Prince's Flower Crown / **«Corona de Flores del Príncipe»** / 王子的花冠 | la primera carta de Préstamo de cada combate NO genera Deuda (la primera dosis es gratis: el anzuelo del prestamista). |
| 8 | **POCO COMÚN** | A Midsummer Night's Dream EX / **«Sueño de una Noche de Verano»** / 仲夏夜之梦EX | sos **inmune a Maldición** (`ICursePreserver`); el primer Débil, Frágil o Vulnerable que recibirías cada combate se anula. (la inmunidad mental real, acotada a 1/combate para lo no-Maldición). |
| 9 | **RARA** | Feather of the Contract / **«Pluma del Contrato»** / 契约之羽 | cuando jugás una carta NP: remové 2 puntos de Deuda (el sueño desplegado salda cuentas). |
| 10 | **RARA** | Clock of Dawn / **«Reloj del Alba»** / 黎明之钟 | al iniciar combates contra Élite o Jefe: +40 NP y Deuda 1 (el doping matutino llega solo). |
| 11 | **RARA/EVENTO (`ILimitBreaker`)** | Holy Grail of the Fae / **«Santo Grial de las Hadas»** / 妖精的圣杯 | +15 HP máx; Vínculo hasta Nv12, NP level hasta 6. |

---

## 8. Stats del personaje

- **HP máx = 72.** Estatuilla de soporte (ATK modesto/HP medio en FGO): ni el tanque Mash/Morgan (78-80) ni la squishy Artoria (70). La **Deuda impaga sangra HP** (3/punto), así que el colchón importa pero no se infla para sobrevivir (regla §1.bis.5). El lift global lo da el **×1.25 del BondRelic**, NO el statline.
- **3⚡, 99 de oro, género Masculine.**
- **Carga NP** 0-300 con ulti auto a 100 (FGOCore). **Estrellas de Crítico** contador global compartido con auto-payoff a 100 → CRÍTICO LISTO (el de Morgan/Jeanne, NO el gateado de Artoria). **Deuda** cap 15, crédito 5 (10 con jefe/Trono), interés +1/turno. Autodaño de cobro SIEMPRE con `ValueProp.Unblockable|Unpowered`.
- **Mazo inicial (10):** 2× Buster + 1× Arts + **2× Quick** + 1× Golpe + 2× Defender + 1× «Palabra del Rey Hada» + 1× «Caer la Noche». QQABB fiel — el único doble-Quick del roster (el sesgo a estrellas es fidelidad, no flavor).
- **Pool total:** 8 básicas (incl. 2 firmas + toggle) + 20 comunes + 28 poco comunes + 20 raras + 6 especiales/sistema generadas.

**Descripción (in-game, eng base):** *"Oberon — the Fae King who lends you the greatest dream of your life, then collects at dawn. Every battery and buff is a loan that comes due at end of turn; cross the ult threshold before anyone else, and bleed for it tomorrow. Pay your debts to mint Critical Stars, or default — become Vortigern, the worm of the end, and make the world pay the bill."*

---

## 9. Notas de arte / animación

- **Servant id 316 / id interno 2800100.** Modelos de batalla (puppets Unity 2D, re-rig en Godot — pipeline §3 del workflow, NO spritesheets por frame; cada manifest declara bundle + `textures/<id>.png`):
  - **2800100** (asc 0/stage 1) = **El Rey del Cuento** (príncipe alado radiante, alas de mariposa cola de golondrina).
  - **2800110** (asc 1-2/stage 2) = **El Príncipe del Invierno** (capa blanca de plumas, viajero).
  - **2800120** (asc 3-4/stage 3) = **VORTIGERN** (paleta negra/abisal, alas venosas de libélula; animaciones y visual del NP viran a la versión siniestra — el swap es literal al juego).
  - 2800130/2800140 = variantes de historia (NPC LB6), **no jugables** (profile.costume vacío, sin svtChange) — ignorar para formas.
- **Charagraphs:** `2800100a@1` (asc1, select Rey), `2800100a@2` (asc2, Invierno), `2800100b@1`/`2800100b@2` (asc3-4, Vortigern/yo-verdadero). Select-bg sugerido: `2800100b@2` (el yo verdadero recostado en la polilla halcón con nomeolvides).
- **Faces** charui: `f_28001000..3`.
- **Iconos de skill (KITs):** `skill_00302` (S1 Velo de la Noche), `skill_00601` (S2 Alondra de la Mañana), `skill_00306` (S3 El Fin de los Sueños). Arte de cartas-NP: `Commands/2800100/card_servant_np.png`. Básicas de comando: `Commands/2800100/card_servant_1.png` en 3 bandas (Buster/Arts/Quick).
- **Iconos de ESTADOS** (Deuda/Dormido/Insomnio): sacar de `buffs[].icon` del JSON del NP 2800101 en Atlas (regla del workflow §5 — bufficon del NP-drain/curse-of-debt y del Sleep real, NUNCA paths inventados).
- **CEs temáticos (charagraphs/CE art):** matcheo con `.claude/workflows/match-ce-art.js` y **dedup contra `mapping_morgan.csv` + mapping_mash + mapping_artoria** — los CEs de LB6/sueño/polillas/mariposas abundan pero **colisionan con Morgan** (misma Lostbelt). Temas a priorizar: sueño, noche/amanecer, mariposas negras, bosque de otoño, contrato/préstamo, el dúo Koyan+Oberon (farming).
- Todas las URLs verificadas HTTP 200 (2026-06-11, `oberon_assets.json`).

---

## 10. Checklist final (§6 de la skill, corrido mentalmente)

- ✓ **Cada carta sobre-tasa paga su exceso (§3):** préstamos = neto ≤ flat de Jeanne, el exceso temprano lo paga el interés+HP+repesesión del ulti; Mentira Piadosa 12/1⚡ = Deuda 1; Hipoteca +100 = 2⚡+Exhaust+Deuda 4; Espina 18/2⚡ = Deuda 2; raras de masa-Sleep = 2⚡+Exhaust; el sueño masivo del ulti vive en sobrecarga ≥150 (paga por frecuencia, P1).
- ✓ **El mazo inicial QQABB gana el acto 1 sin motor:** 26 daño en comandos + 15 bloqueo + firma que acelera el primer ulti a ~turno 3-4; un turno enemigo dormido por combate vía toggle (si se draftea Cuento para Dormir — el starter no lo incluye, pero las firmas cubren las 4 economías).
- ✓ **Plan B sin motor:** Defenders + Canto del Atardecer + Disfraz Absoluto + dormir la amenaza compran el turno malo; presión enemiga (élites tempranas) que fuerza defensa = menos NP = el cobro encuentra el medidor vacío = bajás a tasa normal, nunca por debajo de curva (§1.2 interrumpible).
- ✓ **Nombres trilingües (Mooncell) desde el diseño:** núcleo en §1, cada carta/reliquia con eng/esp/zhs en §6-7. Frases ORIGINALES en VOICE-LINES.md (no transcripciones), JP línea base.
- ✓ **Assets verificados:** modelos 2800100/2800110/2800120 + charagraphs + iconos de skill + iconos de estado, todos HTTP 200.
- ✓ **FGOCore reutilizado:** NpCharge/ConsumeAllForNpCard/Gauge events, OverchargeBlessing, CritStars/CritReady, FormPower familia, BondRelic ×1.25, NpLevels, ILimitBreaker, CursePower (sólo inmunidad). NUEVO acotado: SleepPower+InsomniaPower (compartible), bases de comando (pendiente compartido Morgan), DebtPower + keyword PRÉSTAMO (local).
- ✓ **Power-budget total a nivel Watcher, no por encima:** sin multiplicador global de daño/bloqueo desde el starter (regla §1.bis.2 — el ×1.25 es BondRelic heredado); el burst gateado corre al techo modded SÓLO con candados estructurales intactos (crédito 5, interés, starter cap 3/turno, Vortigern cobra NP primero + cap 5 stacks/turno, Insomnio 2, Hipoteca Exhaust, sueño masivo en sobrecarga, manual Exhaust 1/combate).

### Perillas de playtest (en orden de probabilidad — NUNCA el daño base del ulti)
1. **Sleep-stall vs jefes**: si encadenar Desatados a 150+ traba la pelea, los Jefes "cabecean" (pierden sólo su primera intención) o ganan «Vigilia» 2 turnos tras despertar. 2. **Vortigern default engine**: perillas = 2 daño/punto y cap 5/turno; si "quebrado perpetuo" domina, cap a 4 o exigir Deuda ≥2 impaga. 3. **Trampa del Invierno** (interés en espiral para novatos): mitigado por el toggle y «El Final donde Britania…»; si frustra, cap de interés +3 acumulado/visita. 4. **UX del cobro**: el drain DEBE loguearse (power Deuda con contador + floating text); sin feedback parece bug. 5. **Cobro Adelantado + starter**: un solo contador `_triggersThisTurn` compartido para que pagar 3 manual + 3 auto no doble-dispare el cap. 6. **Loc SimpleLoc**: textos de Deuda llenos de `+`/`−` literales → escapar `/+ /−` y correr `tools/audit_simpleloc.ps1` antes de cada publish (bug real del vínculo, commit 9ce6bbe). 7. **Co-op**: targeting de aliado de El Fin de los Sueños usa el patrón de cobertura de Mash; probar pérdida de buffs ajenos en sync. 8. **Orden de hooks de Dormido**: la anulación de daño (→0 absoluto) va ANTES de Intangible/Bloqueo — los `ModifyHpLost*` son ABSOLUTOS (gotcha `GutsPower`/Hoja de Tilo). Riesgos cerrados por código (no "vigilar"): límite de crédito, Insomnio anti-stunlock, Vortigern NP-primero, sueño masivo en sobrecarga.

---

**Archivo destino:** `f:/Programs/SlayTheSpire2-SmashFromFGO/docs/DESIGN-OBERON.md` (texto markdown de arriba, listo para guardar). FGOCore + Oberon + el resto de personajes se publican JUNTOS (regla §4.5).