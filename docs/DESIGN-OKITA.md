# DESIGN-OKITA — diseño del personaje (Saber, Shinsengumi) para StS2

Consolidado de las dos propuestas (`okita_design_a.json` = motor **Aliento/Ráfaga**; `okita_design_b.json` = motor **Shukuchi/Tos**). **Veredicto: GANA A como columna vertebral** (el Aliento bancable es más legible y tiene precedente vanilla exacto = el income de Estrellas del Regent), **con tres injertos de B**: (B1) la **Tos como carta-Estado convertible** unificada con la de A; (B2) el rider **Shukuchi N** ("jugaste N cartas este turno") como arquetipo drafteable secundario; (B3) la **tribu iai** ("primera carta del turno") como tensión interna. Kit/assets en `okita_kit.json` / `okita_assets.json`. Diseñado con `sts2-mechanics-design`. Manifest id **`OkitaSaber`** (inmutable). Lore investigado JP+中文 (regla WORKFLOW-FGO §2). Servant 102700, collectionNo 68.

---

## 1. Identidad en una frase

**La espadachina más rápida del Bakumatsu: ATACAR es su economía — cada tajo levanta pétalos-estrella que vuelven como críticos, y su único límite es el ALIENTO de un cuerpo que se está muriendo: un turno de gloria, una tos, y volver a respirar.** DPS Quick-crit single-target de cristal: la usuaria nativa de las Estrellas de Crítico de FGOCore (star weight 98 canon), que paga su techo con HP, Exhaust y Tos en vez de bloquear. Donde Mash tanquea para generar y Morgan sangra para generar, **Okita ATACA RÁPIDO para generar** — y su tensión no es con el enemigo, sino con su propio pulmón.

---

## 2. Inventario del kit FGO (skills + NP + pasivas) → mapeo a carta/reliquia

Regla de fidelidad §4.1: cada skill real = una carta o reliquia RECONOCIBLE con números 1:1; el NP real son las cartas-NP.

| Elemento real (FGO) | Efecto canónico | Implementación en el mod |
|---|---|---|
| **Shukuchi B → B+** (縮地 "Reduced Earth") | Quick Up +50% (1T); B+ añade estado pre-ataque ATK +20% (1T) | **Carta PC «Shukuchi B+»** (velocidad/robo + buff de daño al turno) + **keyword RÁFAGA** (el "paso que acorta la tierra" = la mecánica de Aliento) + **FIRMA básica «Shukuchi»** |
| **Weak Constitution A → Zettou A** (病弱 → 絶刀 "Espada Absoluta") | Star Gather +1000% + Crit Chance +10% oculto + (Zettou) Crit Dmg +50% + NP +30% (todo 1T) | **Carta PC «Zettou A»** (NP+★+crit dmg, "turno de gloria") + **Carta PC «Constitución Enfermiza A»** (el nombre ES el drawback: gloria ahora, Tos después — patrón Flak) |
| **Mind's Eye (Fake) A** (心眼（偽）) | Evade (1T) + Crit Dmg +40% (3T) | **Carta PC «Ojo Mental (Falso) A»** (Intangible/Esquiva — su ÚNICA defensa real) |
| **Magic Resistance E** (pasiva) | Debuff Resist +10% (la más baja posible — maldición Innocent Monster) | **Carta PC «Resistencia Mágica E»** (1 Artifact + NP — el rango E como gag: "al menos lo intenta") |
| **Riding E** (pasiva) | Quick Up +2% (sabe montar… apenas) | **Poder PC «Cabalgar E»** (+5★/turno — el goteo mínimo) |
| **Mumyou Sandanzuki** (無明三段突き) | DEF-piercing 3 hits a ST; OC = DEF Down 3T; Rank-Up JP añade Quick/Crit Resist Down | **Las 3 cartas-NP** (auto/manual/mini) — el daño que IGNORA el Bloqueo (la paradoja de las tres estocadas simultáneas) |
| **Append: Load Magical Energy** | +10-20% NP inicial | Reliquia de pool / starter oculta NP-level |
| Lore beat: tuberculosis, "luchar hasta el final", deseo al Grial | END E, death rate 35%, sin curas, 1 evade | **Tos** (carta-Estado), HP 68, **GutsPower** (Alzarse = "luchar hasta el final"), Intangible puntual |

---

## 3. Recurso / motor del personaje

### 3.1 El loop económico (un círculo, cuatro estaciones)

**ATACAR → ESTRELLAS → CRÍTICO → NP → ULTI → (Vulnerable) → atacar más fuerte**, embudado por el **ALIENTO**.

1. **ATACAR = GENERAR.** La starter «Haori Asagi» convierte el evento universal de Okita (jugar un **Ataque**) en **+10 Estrellas** (máx. 3 procs/turno, reset `AfterSideTurnStart`). El espejo agresivo del "bloqueo→★" de Mash y "perder Vida→★" de Morgan. Con 2-3 Ataques/turno + la Quick básica (+30★), el umbral de **100★** se cruza cada ~2 turnos → **CRÍTICO LISTO** (próximo Ataque ×2; FGOCore `CritStarsPower`/`CritReadyPower`).
2. **CRITICAR = CARGAR.** La 2ª conversión del Haori paga **+20 NP por cada crítico consumado** — su NP gain de 1,09%/hit Quick hecho regla: en Okita el NP entra POR los críticos, no por cartas Arts (lleva 1 sola Arts en el deck QQABB). Esto requiere la **única pieza nueva de FGOCore** (§4): un callback al consumirse un Crítico Listo.
3. **NP → Mumyou Sandanzuki** (auto a 100, FGOCore): pega perforando Bloqueo y deja **2 Vulnerable DESPUÉS del daño** (fidelidad al OC real: se usa PRIMERO en la cadena para potenciar el resto del turno).
4. **EL EMBUDO — ALIENTO** (mecánica propia nueva, contador 0-10, empieza en **6**, **+2 al inicio de tu turno**): las cartas **RÁFAGA** (keyword propio, las técnicas Shukuchi) tienen un **segundo coste en Aliento (1-3)** además de ⚡. Precedente vanilla = doble coste ⚡+★ del Regent. **1 Aliento ≈ ½⚡** (ingreso gratis 2/turno, banco 10 → subsidio máx ~1⚡/turno, idéntico al income pasivo del Regent). Las Ráfagas son TODO el exceso sobre tasa: multi-hits, robo, energía.
5. **COSTO = RECURSO** (lección 焰刑地狱). Si tu Aliento llega a **0**: ganás 1 carta **Tos** mezclada al mazo de robo (máx. 1/turno). La Tos es moneda: ~5 fuentes / ~5 sumideros la convierten en NP/★/curita.

### 3.2 Ritmo esperado e interrupciones (plan A interrumpible, §1.2)

- **Turnos 1-2** acumular (atacar, respirar) · **turno 3** GLORIA (Zettou/Shukuchi + Ráfagas + crítico + NP) · **turno 4** respirar (Defenders + Recuperar el Aliento + pagar la Tos).
- **Presión que fuerza defensa** corta la generación (no atacás = no hay ★). **Hordas** castigan el plan ST (AoE escasa adrede = debilidad real de Okita). **Sin Aliento** degrada a tasa vanilla, **NUNCA por debajo** (las Ráfagas son el exceso, no la base). La **Tos** ensucia el robo pero es convertible.

### 3.3 Arquetipos drafteables

- **(a) Lluvia de Pétalos** — ★/críticos máximos (Cinco Destellos, Genio de la Espada, Recuerdo de la Última Primavera).
- **(b) Ráfaga Infinita** — velocidad de cartas + Aliento máximo (Flor del Bakumatsu, Respiración del Tennen Rishin-ryū, Velocidad Cegadora).
- **(c) Shukuchi N** — premia jugar 3+ cartas/turno (Kata de las Mil Estocadas, Corte Cruzado, Espíritu del Bakumatsu) — el arquetipo injertado de B.
- **(d) Hasta el Final** — Tos/HP como combustible (Alzarse, Última Promesa, Hasta el Final, Florecer Tardío).

### 3.4 Sin sistema de formas (justificado) — con UNA excepción clímax

El kit no pide formas: las ascensiones son **ropa**, no modos (misma espadachina con/sin haori). El eje de estados de Okita es **temporal**, no modal: **Aliento alto (gloria) vs Aliento bajo (respirar)** cambia qué cartas son jugables — cambia las DECISIONES sin FormPower (§5 de la skill cumplida). **ÚNICA excepción**, el clímax permanente (patrón Paladín/Avalon): la rara **«Flor del Bakumatsu»** (Poder 2⚡, Exhaust) usa `FormPower` con `IsPermanent` + `FormVisuals` para el swap de modelo **102710 (traje blanco) → 102720 (haori asagi)**: sus Ráfagas dejan de costar Aliento pero gana 1 Tos al final de cada turno — la enfermedad ya ganó, ella pelea igual. Un solo FormPower, sin danza. El costume **102730** queda como skin de menú (ModConfig), no como forma.

---

## 4. Mapeo a FGOCore (reusa vs nuevo)

**REUSA sin cambios:**
- `NpCharge` 0-300 + `GaugeFilled`/`GaugeDropped` (manifestación del Desatado, patrón `TryManifestUlt`).
- `ConsumeAllForNpCard` + Sobrecarga `PerTen` + `INpCostWaiver` (excluye Event, resuelve a tier mínimo) + `Retain` de la Desatada.
- `CritStarsPower` / `CritReadyPower` + helper `CritStars` — **Okita es la consumidora estrella** del sistema compartido (validan lo que Morgan/Jeanne estrenaron).
- `GutsPower` (Última Promesa, capstone del vínculo).
- `BondRelic` (×1.25 daño/bloqueo global + curvas — la palanca §1.bis, NO un ×global de starter).
- `INpLevelStore` / `NpLevels.Scale` (+15%/nivel) + botón Invocar (dupes).
- `OverchargeBlessingPower` (sin uso propio; llega vía reliquia/evento si se draftea).
- `FormPower` / `FormVisuals` (SOLO para el clímax permanente «Flor del Bakumatsu», `IsPermanent`, modelo 102720, precarga en hilos).
- Bases de comando `BusterCardBase` / `ArtsCardBase` / `QuickCardBase` de `Cards/Command` (subclasear `OkitaBuster/Arts/Quick`).

**NUEVO EN FGOCORE (una sola pieza chica, beneficia a todo el roster):**
- **Callback `CritReady.Consumed` (o `ICritConsumedListener`)** — hoy `CritReadyPower` se decrementa en `AfterDamageGiven` sin notificar. Agregar el evento para que el **Haori pague +20 NP por crítico**, Tajo Perfecto robe y futuros personajes enganchen. Es la generalización del helper `CritReady.TryConsume` ya previsto para Morgan (Aliento de Albion) → va en FGOCore, no mod-local.
- *(Opcional, si Morgan no lo subió ya)* `ICritDamageBooster` — interfaz consultada por `CritReadyPower` al multiplicar, para los "tus Críticos hacen +X" (Zettou, Ojo Mental, Genio de la Espada, Kiku-ichimonji Norimune). También lo querrá Artoria.

**MOD-LOCAL (NO contaminar el core):**
- `AlientoPower` (contador 0-10, regen en `AfterSideTurnStart`, registro de "llegó a 0 este turno" para el cap de 1 Tos).
- Keyword **RÁFAGA** (`CustomEnum CardKeyword` + `IsPlayable` por Aliento + `ShouldGlowGoldInternal` + display de doble coste estilo Regent).
- `TosCard` (Estado Etéreo con hook de fin de turno: si está en mano, −1 Aliento; se exhausta al convertirse).
- Rider **Shukuchi N** = helper `CardsPlayedThisTurn` (verificar si `CombatState` ya lo expone; si sí, solo extension method para el glow; si no, contador invisible reset en `AfterSideTurnStart`).
- Patrón **all-crit multi-hit** (copiar `AlbionsBreath` de Morgan).
- **IGNORA Bloqueo** de los NPs: resolver como daño no-bloqueable en el cálculo (verificar `DamageInfo`/`ValueProp` en el decompilado; si no existe flag, leer el Bloqueo del objetivo y sumarlo — equivalente funcional). **Gotcha:** hooks `ModifyHpLost*` son ABSOLUTOS; VFX perforante = `vfx_dramatic_stab` (validar todo path contra el catálogo real).

**Iconos:** estados desde Atlas BuffIcons (★ 320, crit 325); icono **Aliento** propio (pétalo/respiración) en el pck del mod; iconos core en FGOCore. **PUBLICAR FGOCore + TODOS los personajes en la misma pasada** (`MissingMethodException` si no).

---

## 5. Pool COMPLETO de cartas

**Denominaciones fijas (regla Jeanne):** NP y Estrellas hablan en **10/20/30/50/70/100** (10 = rider común = 1 proc del Haori; 30 = paquete de básica; 50 = espejo/gate mini-NP; 70 = gate NP manual; 100 = umbral auto-payoff). **El Aliento habla en 1/2/3** (coste Ráfaga 1-3, regen +2, inicio 6, cap 10). **La Tos vale 20** (NP o ★) al convertirse. **GLOW DORADO** (`ShouldGlowGoldInternal`) en TODA condicional y las Ráfagas brillan cuando el Aliento alcanza.

Terminología trilingüe clave (Mooncell, fijada DESDE el diseño): Okita **冲田总司** · Aliento = Breath = **吐息** · Ráfaga = Dash = **缩地** (Shukuchi) · Tos = Cough = **咯血** · Zettou = **绝刀** · Ojo Mental (Falso) = **心眼（伪）** · Mumyou Sandanzuki = **无明三段突** · Makoto = **诚** · Estrellas de Crítico = **暴击星** · Crítico Listo = **暴击** (FGOCore).

### 5.1 Básicas (7 registradas; mazo inicial 10 = QQABB real de FGO)

| Carta (esp / eng / zhs) | ⚡ | Tipo | Efecto exacto | Engranaje |
|---|---|---|---|---|
| **Buster de Okita** / Okita's Buster / 冲田·Buster | 1 | At | 10 daño. *(up +3)* | comando rojo; salida simple |
| **Arts de Okita** / Okita's Arts / 冲田·Arts | 1 | At | 6 daño; +30 Carga NP. *(up +3/+20)* | comando azul; la **única** Arts del deck inicial (asimetría: el NP entra por críticos) |
| **Quick de Okita** / Okita's Quick / 冲田·Quick | 1 | At | 6 daño; +30 Estrellas. *(up +3/+20)* | comando verde; **lleva DOS** — su carta dominante |
| **Golpe** / Strike / 斩击 | 1 | At | 6 daño. *(up +3)* | compat/tag Strike; **FUERA** del mazo inicial (patrón Morgan) |
| **Defender** / Defend / 守护 | 1 | Hab | 5 Bloqueo. *(up +3)* | plan B frágil; ×3 en deck inicial |
| **FIRMA «Shukuchi»** / Shukuchi / 缩地 | 1 | At, **RÁFAGA 1** | 5 daño ×2; +10★. *(up: 7×2; +20★)* | enseña el doble coste de Aliento + atacar=generar desde el combate 1; arte = charagraph 102700a |
| **FIRMA «Recuperar el Aliento»** / Catch Your Breath / 调息 | 1 | Hab | 5 Bloqueo; +2 Aliento. *(up: 8; +2)* | enseña que defender = respirar; arte = charagraph 102700b (engawa, última primavera) |

**MAZO INICIAL (10, QQABB + sesgo frágil):** 2× Quick + 1× Arts + 2× Buster + 3× Defender + 1× Shukuchi + 1× Recuperar el Aliento. Turno 1 ya enseña las 4 economías. **Gana el acto 1 sin motor armado ✓** (2×10 + 2×6 + 6 + 5×2 = ~38 de daño limpio/ciclo + 15 de Bloqueo).

### 5.2 Comunes (20) — conectividad 18/20 = 90% ✓

| Carta | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **Battōjutsu** / 拔刀术 | 1 | At | 6 daño; si es tu **PRIMER Ataque del turno**: +10★. Glow. *(up 9/+20)* | tribu iai (injerto B) |
| **Tajo de Sakura** / Sakura Slash / 樱斩 | 1 | At | 9 daño. *(up 12)* | pan y manteca (conecta vía Haori) |
| **Estocada Doble** / Double Thrust / 二段突 | 1 | At, **RÁFAGA 1** | 6 daño ×2. *(up 8×2)* | sobre-tasa pagada con Aliento (1⚡+½⚡≈12-13 ✓) |
| **Corte Ascendente** / Rising Cut / 上斩 | 1 | At | 7 daño; +10 NP. *(up 10/+10)* | NP pan y manteca |
| **Paso Shukuchi** / Shukuchi Step / 缩地步 | 0 | Hab, **RÁFAGA 1** | robá 1; +10★. *(up: robá 2)* | cantrip ráfaga |
| **Lluvia de Pétalos** / Petal Rain / 花吹雪 | 1 | Hab | +20★; robá 1. *(up +30★)* | paquete ★ |
| **Envainar** (ESPEJO A) / Sheathe / 收刀 | 0 | Hab | si ≥50★: perdé 50★ → +50 NP. Glow cuando pagable. *(up: consume 30)* | par espejo §4.6.2 (la luz vuelve a la vaina) |
| **Desenfundar** (ESPEJO B) / Unsheathe / 拔刀 | 0 | Hab | si ≥50 NP: perdé 50 NP → +50★. Glow. *(up: consume 30)* | par espejo (el maná estalla en chispas) |
| **Respiración de Combate** / Battle Breath / 战息 | 1 | Hab | 6 Bloqueo; +1 Aliento. *(up 9/+1)* | recuperador de Aliento común |
| **Guardia Seigan** / Seigan Guard / 正眼之构 | 1 | Hab | 8 Bloqueo. *(up 11)* | pan y manteca defensivo |
| **Corte de Ikedaya** / Ikedaya Cut / 池田屋之斩 | 2 | At | 18 daño; ganás 1 **Tos** (al mazo de robo). *(up 23)* | el 2⚡=18-con-downside; el downside es moneda |
| **Pañuelo Carmesí** / Crimson Kerchief / 染血手帕 | 0 | Hab | exhaustá todas las **Tos** de tu mano: +20 NP por cada una. Glow. *(up: +20 NP y +10★ c/u)* | conversor de Estados (patrón Flak) |
| **Mirada del Prodigio** / Prodigy's Glare / 天才之瞳 | 0 | Hab | aplica 1 Débil; +10★. *(up: +20★)* | control suave + ★ |
| **Ojos en la Punta** / Eyes on the Tip / 剑尖之眼 | 1 | At | 8 daño; si ≥10★: +10 NP. Glow. *(up 11/+10)* | rider calibrado al +10 garantizado del Haori |
| **Kiai** / 气合 | 0 | Hab, Exhaust | +20 NP. *(up +30)* | batería con Exhaust |
| **Flor de un Día** / One-Day Bloom / 一日之花 | 1 | Hab, Exhaust | +30★; robá 1. *(up +50★)* | gloria efímera (Exhaust ES lo efímero) |
| **Toque de Atención** / Reprimand / 警示一击 | 1 | At | 6 daño a TODOS. *(up 9)* | la AoE común única (debilidad ST real, −30% §2) |
| **Finta** / Feint / 虚招 | 0 | At | 4 daño. *(up: 6; +10★)* | cantrip de relleno que conecta vía Haori |
| **Tercera Posición** / Third Stance / 第三构 | 1 | Hab | 5 Bloqueo; +10 NP. *(up 8/+10)* | defensa→NP |
| **«¡No disparo rayos!»** (meme canon) / "I Don't Shoot Beams!" / 我才不会射光线 | 0 | At, Exhaust | 3 daño; +20★ (el destello NO era un rayo). *(up 5/+30★)* | el gag Saberface |

### 5.3 Poco comunes (28) — incluye los KITS (skills reales 1:1)

**Los KIT (skills/pasivas reales, Exhaust = cooldown):**

| Carta | ⚡ | Tipo | Efecto | Fidelidad |
|---|---|---|---|---|
| **Shukuchi B+** / 缩地B+ | 1 | Hab, Exhaust | +2 Aliento; robá 2; este turno tus Ataques hacen +3. *(up: +3 Aliento; robá 2; +4)* | Quick Up + estado pre-ataque ATK+20% |
| **Zettou A** / 绝刀A | 1 | Hab, Exhaust | +30 NP; +30★; este turno tus CRÍTICOS hacen +6. *(up +40/+40/+8)* | el turno de gloria embotellado (star gather + crit dmg +50% + NP 30%) |
| **Ojo Mental (Falso) A** / 心眼（伪）A | 1 | Hab, Exhaust | +1 Intangible. *(up: y +20★)* | Evade 1T — su única defensa real (keyword vanilla = complejidad gratis) |
| **Constitución Enfermiza A** / 病弱A | 0 | Hab, Exhaust | +50★; ganás 1 **Tos** (al mazo de robo). Glow siempre. *(up: +50★ y robá 1)* | la skill ES la enfermedad: gloria ahora, tos después |
| **Resistencia Mágica E** / 魔力抵抗E | 1 | Hab | 1 Artifact; +10 NP. *(up: 2 Artifact)* | el rango E como gag |
| **Cabalgar E** / 骑乘E | 1 | Poder | al inicio de cada turno: +5★. *(up: +10)* | sabe montar… apenas |

**Cartas de pool:**

| Carta | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **Ráfaga de Tres Pasos** / 三步缩地 | 2 | At, **RÁFAGA 2** | 6 daño ×4. *(up 7×4)* | 2⚡+1⚡eq=24 ✓ multi-hit |
| **Corte Cruzado** / Cross Cut / 十字斩 | 1 | At | 8 daño; si **ya jugaste otro Ataque** este turno: +10★ y +10 NP. Glow. *(up 11)* | rider Shukuchi (injerto B) |
| **Danza de la Hoja** / Blade Dance / 刃之舞 | 1 | At | 4×2; +10★. *(up 6×2/+20★)* | multi-hit ★ |
| **Tajo Perfecto** / Perfect Slash / 完美一斩 | 2 | At | 16 daño; si tenés CRÍTICO LISTO: robá 1 al criticar. Glow. *(up 20)* | blanco natural del ×2 (patrón Witch) |
| **Embate de la Capitana** / Captain's Onslaught / 队长突击 | 3 | At | 26 daño. *(up 33)* | slot Bludgeon sin condiciones |
| **Barrido del Shinsengumi** / Shinsengumi Sweep / 新选组横扫 | 2 | At | 9 a TODOS; +10★. *(up 12/+20★)* | la 2ª y última AoE |
| **Persecución** / Pursuit / 追击 | 1 | At, **RÁFAGA 1** | 14 daño. *(up 18)* | 1.5⚡ efectivos ✓ |
| **Esgrima Defensiva** / Defensive Fencing / 守势剑术 | 1 | Hab | 9 Bloqueo; +10★. *(up 12/+20★)* | defensa→★ |
| **Alto para Respirar** / Breather / 喘息 | 2 | Hab | 14 Bloqueo; +2 Aliento. *(up 18/+2)* | block premium + Aliento |
| **Instinto de Duelista** / Duelist's Instinct / 决斗者直觉 | 1 | Hab | robá 2; si ≥10★: +10 NP. Glow. *(up: robá 3)* | robo + NP |
| **Medicina Amarga** / Bitter Medicine / 苦药 | 1 | Hab, Exhaust | curá 5; exhaustá todas las Tos de tu mano. *(up: curá 8)* | su única cura: chica + Exhaust (FGO: sin curas) |
| **Velocidad Cegadora** / Blinding Speed / 神速 | 0 | Hab, **RÁFAGA 2** | +1⚡; robá 1. *(up: RÁFAGA 1)* | Aliento→energía, capeado por el income de Aliento |
| **Respiro** / Breather (lite) / 小憩 | 0 | Hab | 3 Bloqueo; +1 Aliento. *(up 5/+1)* | recuperador 0⚡ |
| **Filo Centelleante** / Glinting Edge / 闪光之刃 | 1 | At | 6 daño; +20★. *(up 9/+30★)* | slot Baobhan |
| **Kiku-ichimonji** / 菊一文字 | 1 | At | 12 daño. *(up 16)* | su katana: pan y manteca pc |
| **Tos en el Peor Momento** (gag invertido) / Untimely Cough / 不合时宜的咯血 | 0 | At | 4 daño; si tenés una **Tos** en tu mano: +20★ y +10 NP. Glow. *(up 6/+30★/+10)* | la enfermedad como combustible |
| **Aroma a Sakura** / Scent of Sakura / 樱花之香 | 1 | Hab | +20★ y +10 NP. *(up +30★/+20)* | paquete doble |
| **PODER «Sentido del Prodigio»** / Prodigy's Sense / 天才之感 | 1 | Poder | cada vez que se activa tu umbral de 100★: +10 NP y robá 1. *(up: +20 NP)* | engorda el hilo ★→NP |
| **PODER «Kata de las Mil Estocadas»** / Kata of a Thousand Thrusts / 千突之形 | 1 | Poder | el **3er Ataque** que jugás cada turno: +10★ y +10 NP. *(up +20★/+10)* | arquetipo Shukuchi (injerto B) sin energía gratis |
| **PODER «Voluntad del Shinsengumi»** / Will of the Shinsengumi / 新选组之志 | 2 | Poder | al final de tu turno, si jugaste **3+ Ataques**: 5 Bloqueo y +20 NP. *(up: coste 1⚡)* | arquetipo Shukuchi defensivo |
| **PODER «Florecer Tardío»** / Late Bloom / 迟开之花 | 1 | Poder | cada vez que ganás una **Tos**: +20★. *(up +30★)* | la enfermedad alimenta la gloria |
| **PODER «Paso Constante»** / Steady Step / 稳步 | 1 | Poder | la **primera RÁFAGA** de cada turno reembolsa 1 Aliento. *(up: y +5★)* | descuento de Aliento capeado |

### 5.4 Raras (20)

| Carta | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **NP «Mumyou Sandanzuki» (manual)** / 无明三段突 | 2 | At NP (mín. **70**, consume TODA) | 10×3 a UN enemigo, **IGNORA Bloqueo**; tras el daño: 2 Vulnerable. SOBRECARGA +1/golpe por cada 10 sobre 70. *(up 12×3 y aplica «Defensa Crítica Rota»: el objetivo recibe +6 de cada Crítico tuyo 3T — los debuffs del Rank-Up JP)* | dispararla ANTES del auto-ulti (regla Morgan) |
| **NP «Mumyou: Primera Estocada»** / 无明·一之突 | 1 | At NP (mín. **50**, consume TODA) | 14, IGNORA Bloqueo. SOBRECARGA +2 por cada 10 sobre 50. *(up 18)* | el piso spameable de la economía |
| **Cinco Destellos** / Five Flashes / 五连闪 | 2 | At, **RÁFAGA 1** | 3×5; si tenés CRÍTICO LISTO: lo consume y **LOS CINCO golpes critican**. Glow. *(up 4×5)* | los 5 hits de su Quick real; arregla que el multi-hit desperdicie el crítico (patrón Albion) |
| **Carga de la Primera Unidad** / First Unit Charge / 一番队突击 | 2 | At | +100★, LUEGO 12 daño (sale ×2 con el CRÍTICO LISTO recién pagado). *(up 16)* | patrón 黑闪/Black: dispara el umbral y se cobra a sí misma |
| **Saturación de Eventos** / Event Saturation / 事象饱和 | 2 | Hab, Exhaust | este turno tus Ataques **IGNORAN Bloqueo**. *(up: coste 1⚡)* | la paradoja como skill; anti torre-de-bloqueo |
| **Corte del Genio** / Genius Cut / 天才之斩 | 3 | At | 30 daño; si tenés CRÍTICO LISTO: este ataque ignora Bloqueo. Glow. *(up 38)* | slot boss-killer condicional |
| **PODER CLÍMAX «Flor del Bakumatsu»** / Flower of the Bakumatsu / 幕末之华 | 2 | Poder, Exhaust | **forma final permanente** (`FormPower IsPermanent`, modelo **102720** haori asagi): tus RÁFAGAS no cuestan Aliento; al final de cada turno ganás 1 **Tos** (al mazo de robo). *(up: coste 1⚡)* | la enfermedad ya ganó; el único FormPower (§3.4) |
| **Última Promesa** / Final Promise / 最后的约定 | 1 | Hab, Exhaust | +1 **Alzarse** (`GutsPower`); +20 NP. *(up +30 NP)* | "luchar hasta el final" |
| **Hasta el Final** / To the End / 直到最后 | 0 | Hab, Exhaust | este turno tus RÁFAGAS no cuestan Aliento; en su lugar pagás **2 HP** (Unblockable/Unpowered) por cada punto que hubieras pagado. *(up: 1 HP)* | el override: el plan nunca muere, el cuerpo paga |
| **PODER «Genio de la Espada»** / Sword Genius / 剑之天才 | 2 | Poder | tus CRÍTICOS hacen +8. *(up +12)* | slot Magia Única (crit dmg engine) |
| **PODER «Recuerdo de la Última Primavera»** / Memory of the Last Spring / 最后春日之忆 | 2 | Poder | al inicio de cada turno: +20★. *(up +30★)* | motor ★ per-turn (el asc 4: paz en el engawa) |
| **PODER «Respiración del Tennen Rishin-ryū»** / Breath of Tennen Rishin-ryū / 天然理心流之息 | 2 | Poder | +2 Aliento máximo; recuperás **3 Aliento/turno** (en vez de 2). *(up: coste 1⚡)* | el motor del arquetipo Ráfaga |
| **PODER «Makoto (誠)»** / Makoto / 诚 | 2 | Poder | cada activación de tu umbral de 100★: tus Ataques hacen +2 este combate (**máx. +10**). *(up: máx. +16)* | el estandarte: escalado capeado |
| **PODER «Espíritu del Bakumatsu»** / Spirit of the Bakumatsu / 幕末之魂 | 3 | Poder | al inicio de cada turno: +1 Aliento, +10★ y +10 NP. *(up: coste 2⚡)* | engorda los TRES hilos (patrón 龙之魔女) |
| **Mente Despejada** / Clear Mind / 明镜止水 | 1 | Hab | robá 3; +20 NP por cada **Tos** robada este turno. *(up: robá 4)* | convierte el peor robo en combustible |
| **Sakura Fubuki** / Sakura Blizzard / 樱花吹雪 | 2 | At, **RÁFAGA 2** | 9×3. *(up 11×3)* | tormenta de pétalos: 3⚡ efectivos, 27 multi-hit rara ✓ |
| **Instante Infinito** / Infinite Instant / 无限之刹 | 2 | Hab, Exhaust | +1 Intangible; +30★. *(up: y robá 2)* | botón de pánico defensivo |
| **Duelo Bajo la Nieve** / Duel in the Snow / 雪中之决 | 2 | At | 14 daño; contra Élites y Jefes: +10. Glow. *(up 18/+12)* | la nieve de Ikedaya; boss-killer |
| **Velo de Pétalos** / Veil of Petals / 花瓣之帷 | 2 | Hab, Exhaust | 20 Bloqueo; +20★. *(up 26)* | botón de pánico de una frágil (tasa Impervious con rider) |
| **Cerezo en Plena Floración** / Cherry in Full Bloom / 樱花满开 | 1 | Hab, Exhaust | tu Aliento se llena al máximo; ganás 1 **Tos** (al mazo). *(up: sin Tos)* | el enabler del turno de gloria definitivo |

### 5.5 Especiales / tokens (generadas)

| Carta | Cómo se obtiene | Efecto |
|---|---|---|
| **«Mumyou Sandanzuki: Desatado»** / Unleashed / 解放 | auto-manifestada **GRATIS** al cruzar 100 NP (`ConsumeAllForNpCard` ≥100; Retain + Exhaust) | At NP **0⚡** (consume TODA): **14 daño ×3 a UN enemigo, IGNORA Bloqueo**; tras el daño: **2 Vulnerable**. Un CRÍTICO LISTO en espera dobla el 1er golpe (cola FGOCore). **SOBRECARGA: +1/golpe por cada 10 sobre 100** (a banco 300: 34×3 = 102 perforantes; con crítico en el 1er golpe ≈ 136 — paridad con el techo auditado de Morgan ~128). **MEJORADA (up):** si tenés CRÍTICO LISTO lo consume y **LOS TRES golpes critican**; Vulnerable sube a 3. Escala +15%/nivel (`NpLevels`). |
| **«Tos»** / Cough / 咯血 | generada por: Aliento a 0 (máx. 1/turno), Corte de Ikedaya, Constitución Enfermiza, Flor del Bakumatsu, Cerezo en Plena Floración | carta-**Estado, Etérea, No jugable**: al final de tu turno, si está en tu mano: **−1 Aliento**. Consumida por: Pañuelo Carmesí, Medicina Amarga, Florecer Tardío, Tos en el Peor Momento, Mente Despejada, reliquia Medicina de Matsumoto. |
| **Power «Aliento»** / Breath / 吐息 | contador inicial del combate (6) | 0-10; +2 al inicio de tu turno; pagado por el keyword RÁFAGA. Icono propio (pétalo/respiración) en el pck. |
| **Powers compartidos de FGOCore** (NO se reimplementan) | — | Estrellas de Crítico (0-100+, auto-payoff a 100 → CRÍTICO LISTO) y Crítico Listo (próximo Ataque ×2, hace cola). Okita es su mejor clienta. |

---

## 6. Reliquias

### 6.1 Starters (3)

1. **STARTER (motor) — «Haori Asagi del Shinsengumi»** / Asagi Haori of the Shinsengumi / 新选组浅葱羽织.
   - (1) cada vez que jugás un **Ataque**: **+10 Estrellas** (máx. 3 procs/turno, reset `AfterSideTurnStart`).
   - (2) cada vez que uno de tus ataques **CRITICA** (consume Crítico Listo): **+20 Carga NP**.
   - Convierte los eventos universales de Okita (atacar, critear) en sus dos economías; calibra todos los riders "≥10★" del pool. **Doccomment:** el proc (2) depende de la pieza nueva `CritReady.Consumed` (§4). Icono: clase Saber ORO (5★, `金卡Saber.png`, regla del workflow).

2. **STARTER (BondRelic) — «Lazo de la Primera Unidad»** / Bond of the First Unit / 一番队之绊.
   - Vínculo estándar por run (~100 pts/3 actos) + multiplicador global **×1.25 daño/Bloqueo** heredado de `BondRelic` (la palanca §1.bis — NO un ×global de carta).
   - Overrides Nv4/Nv7: empezás cada combate con **+10★ / +20★** (y +1 Aliento en Nv7).
   - Capstone Nv10 **«Luchar Hasta el Final»**: al inicio de cada combate: **+1 Alzarse** (`GutsPower`, revive al 30%). Su deseo al Grial hecho reliquia.

3. **STARTER OCULTA (INpLevelStore) — «Menkyo Kaiden del Tennen Rishin-ryū»** / Menkyo Kaiden / 天然理心流·免许皆传.
   - Dupes/NP level 1-5, pity 50%+25%, botón «Invocar (dupe)», **+15%/nivel** a las cartas NP (`NpLevels.Scale`). Estándar del roster.

### 6.2 Pool (común → ancient)

| Reliquia (esp / eng / zhs) | Rareza | Efecto |
|---|---|---|
| **Dango de Tres Colores** / Three-Color Dango / 三色团子 | Tienda | +1 Aliento máximo; empezás cada combate con +2 Aliento. *(adora los dulces y a los niños)* |
| **Póster GUDAGUDA con Nobu** (meme) / GUDAGUDA Poster with Nobu / 与信长的GUDAGUDA海报 | Tienda | la primera vez que tu Aliento llega a 0 cada combate: +50★. *(¡el drama es glorioso!)* |
| **Kiku-ichimonji Norimune** / 菊一文字则宗 | Poco común | tus CRÍTICOS hacen +3 de daño. |
| **Pétalos de Sakura** / Sakura Petals / 樱花花瓣 | Poco común | al inicio de cada combate: +30★. |
| **Medicina del Dr. Matsumoto** / Dr. Matsumoto's Medicine / 松本医生之药 | Poco común | la primera **Tos** que ganás cada combate se exhausta de inmediato y da +20 NP. |
| **Estandarte Makoto (誠)** / Makoto Banner / 诚之旗 | Rara | cada activación de tu umbral de 100★: +10 NP y robá 1. |
| **Chapa de la Primera Unidad** / First Unit Badge / 一番队队牌 | Rara | al inicio de combates contra Élites y Jefes: +20★ y +2 Aliento. |
| **JEFE — «Flor de la Capital Imperial»** (reemplaza al Haori) / Flower of the Imperial Capital / 帝都之华 | Ancient/Jefe | **duplica ambas conversiones del Haori**: +20★ por Ataque (máx. 3/turno) y +40 NP por crítico. |
| **EVENTO (ILimitBreaker) — «Santo Grial de la Capital Imperial»** / Holy Grail of the Imperial Capital / 帝都圣杯 | Rara/Evento | +15 HP máx.; Vínculo hasta Nv12 y NP level hasta 6. *(Koha-Ace: la Extraña Historia del Grial de la Capital Imperial — SU grial)* |

---

## 7. Stats del personaje

- **HP máx. 68** — **la más frágil del roster jugable** (Morgan 78, Artoria 70; rango ecosistema 66-80, §1.bis.5). Su END E y death rate 35% son canon; compensa con Intangible puntual, Alzarse y velocidad, NO inflando HP para sobrevivir.
- **3⚡, 99 oro.** Mazo inicial de 10 (QQABB real: 2× Quick + 1× Arts + 2× Buster + 3× Defender + Shukuchi + Recuperar el Aliento).
- **Carga NP 0-300**, ulti auto a 100 (FGOCore). **Estrellas de Crítico 0-100+** compartidas (FGOCore). **Aliento: cap 10, empieza cada combate en 6, +2 al inicio de tu turno** (power contador visible).
- **Género** Feminine; CV Aoi Yuki, arte Takeuchi (flavor del charsheet).
- **Descripción (in-game):** *"Capitana de la Primera Unidad del Shinsengumi y la espadachina más fuerte del Bakumatsu, un prodigio del Tennen Rishin-ryū que alcanzó el menkyo kaiden de adolescente. Murió joven de tuberculosis sin pelear hasta el final — su deseo al Grial. Frágil pero letal: ataca rápido, acapara estrellas, y detona un turno de gloria que su propio cuerpo después le cobra."*
- **Pool total:** 7 básicas registradas + 20 comunes + 28 poco comunes + 20 raras + 2 especiales (Desatado, Tos) + 1 power contador (Aliento) + 12 reliquias.

---

## 8. Notas de arte / animación

- **Servant id 102700, collectionNo 68** ("Sakura Saber"). Todos los assets verificados HTTP 200 (`okita_assets.json`).
- **Modelos de batalla** (puppets Unity 2D, NO spritesheets → re-rig en Godot, DESIGN.md §7):
  - **102710** = base (traje blanco asc 1-2) → modelo inicial del mod.
  - **102720** = clímax (haori asagi del Shinsengumi, asc 3-4) → swap vía `FormPower IsPermanent` de «Flor del Bakumatsu» (precarga en hilos).
  - **102730** = costume "Kan'i Reii: Asagi no Haori Hakama" (formal) → skin de menú (ModConfig), NO forma.
  - **102700** = kimono civil rosa (asc 0) → pose de menú/charsheet.
- **Charagraphs:** 102700a@1 (pose de combate, civil) y 102700b@1/b@2 (haori, nieve de Ikedaya) → firmas (Shukuchi usa a@1; Recuperar el Aliento usa b@1 = engawa/última primavera); select-bg = 102700b@2 (asc final).
- **Faces:** f_1027000..f_1027003 + f_1027300 (costume).
- **Iconos de skill** (para las cartas KIT): Shukuchi = `skill_00304`; Zettou/Constitución Enfermiza = `skill_00311`; Ojo Mental = `skill_00402`.
- **Icono starter** = clase Saber DORADA 5★ (`Sabergold.png`/`金卡Saber.png` vía fandom API `&format=original`, regla WORKFLOW §6). **Icono Aliento** propio (pétalo/respiración) en el pck del mod. Iconos de estados desde Atlas BuffIcons (★ 320, crit 325).
- **Arte de cartas / CEs temáticos:** vía `match-ce-art.js` con dedup contra `mapping_morgan.csv` y `mapping_mash` — abundan CEs GUDAGUDA / sakura / Shinsengumi (p. ej. para la tribu Tos/última primavera, CEs de cerezo y engawa; para Ráfaga, CEs de velocidad/Shukuchi). VFX perforante de los NP = `vfx_dramatic_stab` (validar todo path contra el catálogo real — gotcha de carta congelada).
- **Loc trilingüe** (esp latino / eng / zhs Mooncell) fijada DESDE el diseño; correr `tools/audit_simpleloc.ps1` antes de publicar (escapes SimpleLoc: los `+10★`, `(máx. 3/turno)`, `×3` necesitan `/+`, `/(` ). **Manifest `id: "OkitaSaber"` — NUNCA cambiar.**

---

## Checklist final (§6 de la skill — corrido)

- [x] **¿Cada carta sobre-tasa paga su exceso (§3)?** Ráfaga = consumir Aliento (1≈½⚡, income 2/turno, banco 10 → subsidio máx ~1⚡/turno = precedente Regent): Estocada Doble 1⚡+R1=12, Persecución 14, Ráfaga de Tres Pasos 2⚡+R2=24, Sakura Fubuki 2⚡+R2=27 ✓. Crítico ×2 pagado por 100★ (~3 cartas ≈ 1.5⚡): Tajo Perfecto 16→32 por ~3.5⚡ ✓. IGNORA Bloqueo reservado a NPs (consume-all) + 2 raras (Exhaust/condición) — nunca en tasa base. Tos = downside-moneda con sumidero a valor fijo 20 y cap 1/turno por agotamiento. Sobre-tasa de generación siempre con Exhaust (Flor de un Día, Kiai, Constitución Enfermiza, Cerezo). Skills reales = Exhaust 1:1 (regla Artoria).
- [x] **¿El mazo inicial gana el acto 1 sin el motor?** Sí: 3× Defender + 68 HP + Buster/Quick a tasa vanilla, ~38 daño/ciclo limpio.
- [x] **¿Plan B cuando el motor no llega?** Forzada a defender no genera ★ (atacar es el motor); 2 AoE en todo el pool (debilidad ST adrede); sin Aliento baja a tasa vanilla, nunca por debajo; "Hasta el Final" compra la excepción con HP a 2:1.
- [x] **¿Nombres en los 3 idiomas con terminología oficial CN?** Sí (Mooncell: 缩地/绝刀/无明三段突/吐息/咯血/诚/暴击星/暴击).
- [x] **¿Assets verificados (IDs Atlas)?** Sí, HTTP 200 (102700/710/720/730, faces, skill icons).
- [x] **¿Reutiliza FGOCore donde corresponde?** Sí: NpCharge, ConsumeAll+PerTen, CritStars/CritReady, GutsPower, BondRelic, NpLevels, FormPower (solo clímax). UNA pieza nueva chica al core (`CritReady.Consumed`), que beneficia a todo el roster.
- [x] **¿Power-budget nivel Watcher, no por encima?** Techo del Desatado a banco 300 ≈ 102-136 = paridad con Morgan ~128; lift global ×1.25 vía BondRelic (NO ×daño de starter, regla §1.bis.2); motores gateados por umbral (100★/100 NP) y Aliento.

**Perillas de playtest (en orden):** (1) regen de Aliento 2↔1 si las Ráfagas se sienten gratis; (2) cap del Haori 3→2 procs/turno si los mazos de 0⚡ inundan ★; (3) quitar el all-crit del Desatado mejorado si el pico a 300 ofende; (4) Velocidad Cegadora a Exhaust si aparece loop ⚡-positivo con Paso Constante + Respiración del Tennen (auditar que ningún ciclo devuelva >1⚡/turno, regla Artoria §11.7); (5) valor de la Tos 20→30 si nadie la draftea como arquetipo; (6) co-op: Estrellas/Aliento son contadores personales, los bonos ofensivos NO escalan (el star-weight 98 queda como flavor, no griefing).