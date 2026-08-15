# REDESIGN-MORGAN-V2 — Morgan, Reina Hada de Britania

> **Estado: PROPUESTA APROBADA POR PANEL — PENDIENTE DE APROBACIÓN DEL USUARIO.**
> Síntesis del panel de diseño del 2026-08-15 (WORKFLOW-FGO §4.6.7: tres propuestas, tres jueces, "los parches del juez mandan"). Base: **Propuesta 3 (arquetipos)**, ganadora por unanimidad (3–0), con todos los parches obligatorios de los tres jueces aplicados e injertos de las propuestas perdedoras. **No se implementa nada hasta el visto bueno del usuario.**
> Mecanismo save-safe obligatorio: ningún ID se renombra; el re-pool es re-efecto sobre IDs existentes + 4 cartas y 1 reliquia [NUEVA]; **cero demotes nuevos**. FGOCore no se toca en este pase.

---

## 1. Identidad

**En una frase:** *La reina de dos caras que siembra Maldición como Bruja de la Lluvia y la Detona como Reina Hada — y que puede, según el draft, elegir NO bailar: congelar el campo en invierno perpetuo, coronarse a sangre, o cobrarlo todo con Rhongomyniad.*

Como el Ironclad tiene Strength / Block / Exhaust / Barricade, Morgan tiene cuatro líneas de draft reales, cada una anclada a una faceta canónica de LB6 (Vivian / Tonelico / Morgan, los Fairy Knights, Cernunnos, la tiranía y el invierno de Faerie Britain). El motor implementado en REDESIGN-MORGAN (3 formas, Maldición cap 25, Carga NP 0–300, Sentencia, Guts, Knight's Arms) **se conserva intacto**.

---

## 2. Arquetipos y matriz de cobertura

| | **A. La Danza de Dos Caras** | **B. Invierno Perpetuo** | **C. Corte y Sobrecarga NP** | **D. Sangre de la Tirana** |
|---|---|---|---|---|
| Motor | cambiar de forma; sembrar→**Detonar** | acampar Bruja/Invierno; DoT sin decay + spread | Carga NP → ventana + NPs + Knight's Arms | HP propio → NP/daño/Guts |
| Fantasía | la reina castiga con la Sentencia | el invierno eterno de Britania | Rhongomyniad y la corte de caballeros | la tirana que sangra y no cae |
| **Ataque** | Furia de la Reina, Desdén, Golpe Espejado, Aliento de Albión | Rayo Maldito, Lanza Réplica, Carga de la Cacería, Colmillo de Barghest | Golpe de Cetro, Garra de Melusine, Ráfaga de Réplicas, Knight's Arms | Arremetida, Sangre de la Tirana (puente D→B), Juramento de Sangre, Lanza de la Tirana |
| **Defensa** | Guardia Cambiante [NUEVA], Canto de Lluvia, Lealtad de Woodwose [NUEVA] | Acero Invernal, Espinas, Abrazo del Lago, Muro de Granizo, Vigilia del Spriggan | Escarcha Protectora, Creación de Territorio, Mandato (básica) | Sacrificio de la Reina, Lágrimas, Corona de Espinas [reliquia NUEVA] |
| **Consistencia/robo** | Truco del Espejo, Soberana de Dos Caras | Marca de la Bruja (0⚡) | Edicto Real, Mamá Boba, Memoria del Fresno (válvula NP→cartas), Bajo el Árbol del Mundo | Precio de la Corona [NUEVA] |
| **Energía** | cetro (1er cambio: +1⚡), Coronación del Fin del Mundo (Ancient) | compensa con 0⚡: Marca, Vasallaje | ventana NP (+1⚡) | Precio de la Corona [NUEVA] |
| **Escalado** | Cernunnos, Cosecha de Maldición | Ojos de Hada, Invierno Perpetuo, Construcción de Objetos | Hada de la Tierra de la Lluvia, niveles NP (gacha) | Carisma de la Adversidad, Pacto de Sangre |
| **AoE** | Barrida de la Tirana, QSU (ventana) | Lluvia Maldita, Velo de Niebla, Rayo Maldito | ROADLESS CAMELOT, LONDINIUM | Venganza de la Salvadora (ST grande) + ventana |
| **Jefes que limpian debuffs** | re-siembra automática de la Bruja + multi-hits crudos; cobro pre-strip (Vasallaje, Barrida) | los POWERS re-aplican solos cada turno | **NP y Arms no son debuffs** — inmunes al strip | HP/Guts/Strength propios — inmune al strip |
| Debilidad real | secuenciar cuesta turnos; manos sin interruptor | lenta (−2 daño en Bruja) vs élites rápidas | pico diferido, frontload flojo | jugás al filo |

Regla DECISIONS "el pool no depende solo de debuffs": **cumplida por diseño** — C y D no leen Maldición; A convierte la danza en recursos propios; B pierde a lo sumo 1 turno de stacks ante un strip. Multi-hit anti-Buffer existe en las tres rarezas, **incluida la común** (Lanza Réplica re-spec, injerto de P2). Ninguna carta pertenece a los 4 arquetipos a la vez.

**Densidad por línea y rareza (aprox.):** Común A6/B7/C7/D4 + flex · PC A7/B9/C9/D5 · Rara A4/B4/C5/D7. D compensa su común corto con la rareza alta más profunda (precedente Exhaust-Ironclad). Señalización de Acto 1: las cuatro puertas se abren en común.

---

## 3. Formas y Sentencia

### 3.1 Pasivas (motor REDESIGN intacto; solo texto nuevo, 5 idiomas)

- **Reina Hada (Berserker, forma inicial):** *«Tu Maldición no decae AL FINAL DEL TURNO. Tus Ataques la **Detonan**.»* +10 NP la primera vez por turno que dañás HP.
- **Bruja de la Lluvia (Caster):** tus cartas aplican +1 Maldición; tu Maldición no decae al final del turno; +2 Maldición a TODOS al inicio de tu turno; tus Ataques −2 daño.
- **Reina del Invierno (rara, permanente):** ambas pasivas sin penalidad. Clímax, no punto de partida.

### 3.2 Keyword `Detonar` — la solución de legibilidad (reporte 2)

1. **Keyword registrado, dorado, tooltip único en 5 idiomas** (parche J2-3, texto completo con las reglas de resolución):
   *«**Detonar**: tus Ataques infligen daño adicional igual a la Maldición del objetivo y la consumen. Multi-golpe: solo el primer golpe. Ataques a TODOS: detona a cada objetivo golpeado.»*
   (La regla multi-golpe está verificada contra `WinterQueenFormPower._pendingSentence`; la regla multi-objetivo se escribe ANTES de que sea el próximo reporte de Steam.)
2. **Re-texto de pasivas** en dos frases / dos verbos (M4): "no decae al final del turno" ≠ "tus Ataques la Detonan". Imposible leer una sin la otra.
3. **Feedback visual obligatorio** (injerto P1, parches J1-14/J2-4/J3-10): número flotante propio **«¡Sentencia! +X»** con bufficon_521, distinto del número del golpe — el jugador VE que la Maldición se volvió daño, no que "se perdió".
4. **Glow dorado** en todo Ataque cuando estás en Hada/Invierno y el objetivo tiene Maldición. **Glow con condición vacía = NO glow** (parche J2-5): Vasallaje / Colección Final / Barrida con 0 Maldición no brillan.
5. Cerrar el dorado explícito `*词*` en zhs; `audit_simpleloc` + paridad 5 idiomas.

### 3.3 Interruptores de forma por rareza (el fix estructural del reporte 1)

| Rareza | → Reina Hada (cosechar) | → Bruja de la Lluvia (sembrar) | Toggle / permanente |
|---|---|---|---|
| **Starter** (cetro) | arrancás EN Hada | — | *Metamorfosis de la Reina* gratis, Ethereal, turno 1 — y **se re-arma al llegar a 100 NP** (M3, 1 vez por ventana) |
| **Común** | **Furia de la Reina** (Ataque: entra Y Detona en el mismo golpe) | **Canto de Lluvia** (entra + Block) | **Truco del Clan del Espejo** (baja de PC: toggle 1⚡ + robá 2) |
| **Poco común** | **Forma: La Reina Hada** — **0⚡** (+Maldición) | **Forma: Bruja de la Lluvia** — 0⚡ (+NP) | payoffs: Golpe Espejado, Ira de la Tormenta, Guardia Cambiante |
| **Rara** | — | — | **Coronación del Invierno** (permanente); **Soberana de Dos Caras** (premia cada cambio, cap 2/turno) |

Paridad exacta 2–2 por dirección, puerta de cada dirección en COMÚN (conectividad ≥90%, regla 4.6.2), y **la vuelta a Hada es un Ataque** — cambiar y pegar es una sola carta, exactamente lo que pedía el reporte 1. El toggle común + el cetro re-armable hacen imposible el «没遇见».

### 3.4 Ventana NP — modelo único y cerrado (parche J3-1, manda sobre J1-3)

Verificado contra código: la "ventana inline" se eliminó el 2026-06-26; hoy `GaugeFilled` solo manifiesta `QueensSentenceUnleashed`. El modelo de la síntesis:

**A 100 NP:** `GaugeFilled` → manifiesta **Sentencia de la Reina: Desatada** (QSU: Retain/Agotar; al JUGARLA, Detona la Maldición de cada enemigo **sin consumirla**) **+** `NpWindow.ReturnResources` (+1⚡, robá 1 — helper FGOCore existente, cero API nueva) **+** re-armado de la Metamorfosis (M3).

- **NO hay detonación AoE automática** separada de la carta. **Una cosecha por pico, no dos.**
- **PROHIBIDO «detonaciones ×2»** (parche J1-3; el motor no existe y rompería el techo).
- El switch del re-armado **no cuenta doble** para la Coronación (+1⚡) si la reliquia ya procesó su 1/turno (parche J1-10).

---

## 4. Mazo inicial y básicas

**Mazo inicial (10) — SIN CAMBIOS**, verificado contra `MorganBerserker.cs:34-43` (parches J2-2/J3-6; quedan anuladas las composiciones de P1 y P2):
**2× Buster, 2× Arts, 1× Quick, 1× Strike, 2× Defend, 1× Lanza del Fin del Mundo, 1× Mandato de la Reina** (QAABB, regla 4.6.1).

| Carta | ID | ⚡ | Efecto | Mejora |
|---|---|---|---|---|
| Golpe / Defensa | [REUSA StrikeMorgan / DefendMorgan] | 1 | 6 daño / 5 Block | 9 / 8 |
| Buster | [REUSA BusterMorgan] | 1 | 10 daño | 14 |
| Arts | [REUSA ArtsMorgan] | 1 | 6 daño + 30 NP | 9 + 30 |
| Quick | [REUSA QuickMorgan] | 1 | 6 daño + 3 Maldición (conserva rider 20★ globales, Critical v2) | 9 + 4 |
| Lanza del Fin del Mundo | [REUSA LanceOfTheWorldsEnd] | 1 | 8 daño + 2 Maldición (firma, `ITranscendenceCard`) | 11 + 3 |
| Mandato de la Reina | [REUSA QueensMandate] | 1 | 6 Block + 10 NP; +4 Block si hay enemigo maldito (glow); co-op: +NP aliados | 8 / +6 |

Las básicas **conservan los números actuales del código**; cualquier renumeración de esta tabla que difiera del estado publicado va como knob declarado, nunca como "descripción" (parche J3-6). Las 4 puertas abren turno 1: Buster pega (A/D), Quick+Lanza siembran (B), Arts+Mandato cargan (C), el cetro convierte sangre en Maldición (D→B).

---

## 5. Pool por rareza — 75 drafteables (25 C / 30 PC / 20 R)

*Números = punto de partida (knobs de playtest). Glow dorado en toda condicional; condición vacía = sin glow. Denominaciones NP: solo 10/20/30/50/100 (regla 4.6.3, corregida en todo el pool).*

### 5.1 COMUNES (25) — engranajes de conversión, conectividad 25/25

| Carta (eng) | Origen | Tipo | ⚡ | Efecto | Mejora | Arq. | Análogo |
|---|---|---|---|---|---|---|---|
| Furia de la Reina (Queen's Fury) | [REUSA QueensFury — re-efecto] | Ataque | 1 | Entrá en Reina Hada. 9 daño *(Detona en este golpe)* | 13 | **A** | Eruption (Watcher) |
| Canto de Lluvia (Rain Chant) | [REUSA RainChant] | Hab. | 1 | Entrá en Bruja de la Lluvia. 6 Block | 9 | **A**/B | Vigilance |
| Truco del Clan del Espejo (Mirror Clan's Trick) | [REUSA MirrorClansTrick — **PC→común**] | Hab. | 1 | Cambiá a tu forma opuesta. Robá 2 | **sigue 1⚡; además +10 NP** (parche J1-1: la mejora 0⚡ queda PROHIBIDA) | **A** | Inner Peace |
| Guardia Cambiante (Shifting Guard) | **[NUEVA `ShiftingGuard`]** | Hab. | 1 | 5 Block; si cambiaste de forma este turno: 10 (glow) | 7/13 | **A** | defensa de la danza |
| Desdén de la Reina (Queen's Scorn) | [REUSA QueensScorn] | Ataque | 1 | 7 daño; +5 si el objetivo tiene Maldición (glow) | 9/+7 | A/B | Bane (Silent) |
| Barrida de la Tirana (Tyrant's Sweep) | [REUSA TyrantsSweep] | Ataque | 2 | 8 a TODOS, +1 por cada 2 Maldición del objetivo (máx +8 c/u) | 10, máx +10 | A/B | All-Out Attack; cobro pre-strip |
| Réplicas Gemelas (Twin Replicas) | [REUSA TwinReplicas] | Ataque | 1 | 5 daño ×2 + 10 NP | 7×2 | A/C | Twin Strike; anti-Buffer |
| Marca de la Bruja (Witch's Mark) | [REUSA WitchsMark] | Hab. | 0 | Aplicá 4 Maldición | 6 | **B** | Deadly Poison — **sin «robá 1»** (parche J2-6) |
| Lluvia Maldita (Cursed Rain) | [REUSA CursedRain] | Hab. | 1 | 3 Maldición a TODOS + 10 NP | 5 | **B**/C | Crippling Cloud-lite |
| Rayo Maldito (Cursed Bolt) | [REUSA CursedBolt] | Ataque | 1 | 5 a TODOS + 2 Maldición c/u | 7/3 | **B** | Cleave + rider |
| Acero Invernal (Winter Steel) | [REUSA WinterSteel] | Hab. | 1 | 5 Block; +4 si hay enemigo maldito (glow) | 7/+5 | **B** | Defend condicional |
| Mirada de la Reina (Queen's Gaze) | [REUSA QueensGaze] | Hab. | 1 | 2 Weak + 3 Maldición | 3/4 | **B** | Sucker Punch |
| Lanza Réplica (Replica Lance) | [REUSA ReplicaLance — **re-spec de P2** (parche J3-9)] | Ataque | 1 | 4 daño ×2; 1 Maldición por golpe | 6×2; 2 por golpe | B/A | **multi-hit anti-Buffer en COMÚN** que P3 no tenía |
| Golpe de Cetro (Scepter Blow) | [REUSA ScepterBlow] | Ataque | 1 | 8 daño + 10 NP | 12 | **C** | Arts engordada |
| Recaudación (Tax Collection) | [REUSA TaxCollection] | Hab. | **1** | 20 NP; 30 si hay enemigo maldito (glow) | 30/50 | **C** | **nunca 0⚡** (parche J1-8) |
| Edicto Real (Royal Edict) | [REUSA RoyalEdict] | Hab. | 1 | Robá 2 + 10 NP | robá 3 | **C**/todos | Acrobatics-lite |
| Velo de Niebla (Mist Veil) | [REUSA MistVeil] | Hab. | 0 | Requiere ≥50 NP (glow): gastá 50 NP → 5 Maldición a TODOS | 7 | C→B | válvula NP→Maldición |
| Vasallaje (Vassalage) | [REUSA Vassalage] | Hab. | 0 | Consumí hasta 5 Maldición del más maldito → 4 NP por punto | hasta 8 | B→C | válvula inversa; cobro pre-strip |
| Castigo Real (Royal Punishment) | [REUSA RoyalPunishment] | Ataque | 1 | 8 daño + 1 Vulnerable; si maldito: +10 NP (glow) | 10/2 | C/A | Uppercut-lite |
| Mamá Boba ("Silly Mama") | [REUSA SillyMama — **demote de P2 anulado**] | Hab. | 0 | Robá 1 + 10 NP | robá 1 + 20 NP (**no muta de rol** — parche J2-16) | C flex | el meme querido; cantrip |
| Arremetida Demente (Mad Lunge) | [REUSA MadLunge] | Ataque | 0 | **7 daño. Perdés 3 HP** | 10; perdés 3 | **D** | Hemokinesis-lite — resolución J1-7×J2-10 (la más restrictiva de ambas) |
| Sangre de la Tirana (Tyrant's Blood) | [REUSA TyrantsBlood — **re-spec de P2** (parche J3-9)] | Ataque | 1 | 8 daño; perdés 2 HP; aplicá 3 Maldición | 11; 4 Mald | **D→B** | el puente sangre→siembra en común |
| Sacrificio de la Reina (Queen's Sacrifice) | [REUSA QueensSacrifice — re-efecto] | Hab. | 1 | 9 Block. Perdés 2 HP. +10 NP | 12 Block | **D**/C | blood-Defend (deja de ser dupe de Lanza de la Tirana) |
| Respiro Feérico (Fae Respite) | [REUSA FaeRespite] | Hab. | 1 | Curá 4 HP. Agotar | 6 | D/B | Bandage Up |
| Escarcha Protectora (Protective Frost) | [REUSA ProtectiveFrost — **demote de P3 ANULADO** (parche J2-12)] | Hab. | 1 | 6 Block + 10 NP | 9 + 10 | C/B | densidad defensiva común = seguro contra manos muertas |

### 5.2 POCO COMUNES (30)

| Carta (eng) | Origen | Tipo | ⚡ | Efecto | Mejora | Arq. | Análogo |
|---|---|---|---|---|---|---|---|
| Forma: La Reina Hada (Form: The Fairy Queen) | [REUSA FairyQueenForm — **1⚡→0⚡** (M1)] | Hab. | 0 | Entrá en Reina Hada. Aplicá 4 Maldición | 7 | **A** | par espejo exacto |
| Forma: Bruja de la Lluvia (Form: Rain Witch) | [REUSA RainWitchForm] | Hab. | 0 | Entrá en Bruja. +20 NP | +30 | **A** | par espejo |
| Golpe Espejado (Mirror Strike) | [REUSA MirrorStrike — re-efecto: "este combate"→**"este turno"**] | Ataque | 1 | 5×2; si cambiaste de forma este turno: ×3 (glow) | 7 | **A** | anti-Buffer; premia bailar HOY (flag por turno vía `FgoCombatState`) |
| Ira de la Tormenta (Storm's Wrath) | [REUSA StormsWrath — re-efecto] | Ataque | 1 | 8 daño; si cambiaste de forma este turno: +8 y aplicá 3 Maldición (glow) | 10/+10/4 | **A** | switch-payoff |
| Aliento de Albión (Albion's Breath) | [REUSA AlbionsBreath] | Ataque | 2 | 6×3; Detonar solo en el primer golpe | 8×3 | **A** | Pummel; fiel al motor verificado |
| Ejecución Real (Royal Execution) | [REUSA RoyalExecution] | Ataque | 1 | 10 daño; si mata: +20 NP y 3 Maldición a TODOS | 14/+30/4 | A/C | Feed sin maxHP |
| Chillido de Baobhan Sith (Baobhan Sith's Shriek) | [REUSA BaobhanSithsShriek] | Ataque | 1 | 6 daño + 4 Maldición + 1 Weak | 8/5/2 | B | Fairy Knight Tristan |
| Ojos de Hada (Fairy Eyes) | [REUSA FairyEyes] | Poder | 1 | Inicio de turno: 2 Maldición a TODOS | 3 | **B** | 妖精眼; re-siembra tras strip |
| Construcción de Objetos (Item Construction) | [REUSA ItemConstruction] | Poder | 1 | Tus cartas aplican +2 Maldición | +3 | **B** | 道具作成; amplificador |
| Carga de la Cacería Salvaje (Wild Hunt Charge) | [REUSA WildHuntCharge] | Ataque | 2 | 8 a TODOS + 3 Maldición a TODOS | 11/4 | B/A | AoE mayor |
| Decreto Invernal (Winter Decree) | [REUSA WinterDecree] | Hab. | 1 | 8 Block + 2 Maldición a TODOS | 11/3 | **B** | defensa + siembra |
| Espinas del Invierno (Winter Thorns) | [REUSA WinterThorns] | Hab. | 1 | 8 Block; quien te ataque este turno gana 3 Maldición | 11/4 | **B** | Flame Barrier variante |
| Abrazo del Lago (Embrace of the Lake) | [REUSA EmbraceOfTheLake] | Hab. | 1 | Solo en Bruja/Invierno (glow): 14 Block | 18 | **B** | big block con gate de forma |
| Colmillo de Barghest (Barghest's Fang) | [REUSA BarghestsFang] | Ataque | 1 | 9 daño; si maldito: curá 3 | 12/4 | B/D | Fairy Knight Gawain; Reaper-lite |
| Cosecha de Maldición (Curse Harvest) | [REUSA CurseHarvest] | Hab. | 1 | Duplicá la Maldición del objetivo (máx +10; cap global 25) | 0⚡ | A/B | Catalyst; **gate J1-19: si el pico >220, máx +8** |
| Garra de Melusine (Melusine's Talon) | [REUSA MelusinesTalon — **rider de P1** (parche J2-11)] | Ataque | 1 | 8 daño + 10 NP; si tenés un Knight's Arm en mano: +5 (glow) | 11/+6 | **C** | Fairy Knight Lancelot, la más leal |
| Ráfaga de Réplicas (Replica Barrage) | [REUSA ReplicaBarrage] | Ataque | 1 | 3×3; +10 NP por golpe que dañe HP (denominación J2-8) | 5×3 | **C** | anti-Buffer C (knob: ×2 golpes si el total asusta) |
| Memoria del Fresno (Memory of the Ash Tree) | [REUSA MemoryOfTheAshTree — **válvula de P2** (parche J2-7)] | Hab. | 0 | Gastá 30 NP: robá 2 (glow) | robá 3 | C→todos | **la única arista NP→cartas del grafo** |
| Protección del Lago (Protection of the Lake) | [REUSA ProtectionOfTheLake] | Hab. | 1 | +20 NP; en Bruja/Invierno: robá 1 (glow) | +30 | C/A | 湖の加護 A |
| Hada de la Tierra de la Lluvia (Fairy of the Rainland) | [REUSA FairyOfTheRainland] | Poder | 1 | Inicio de cada turno: +10 NP (denominación J2-8; co-op rider existente intacto) | además +20 NP al jugarla | **C** | Prisma Cosmos-ish |
| Llamado de los Caballeros Feéricos (Call of the Fairy Knights) | [REUSA CallOfTheFairyKnights] | Hab. | 2 | Barghest: 3 Maldición a TODOS; Baobhan: 1 Weak a TODOS; Melusine: 6 Block; +1 Knight's Arm | +2 Arms | **C**/B | la tríada canónica |
| Corte del Invierno (Winter Court) | [REUSA WinterCourt] | Poder | 2 | Inicio de turno: +1 Knight's Arm; Arm jugada → +10 NP (denominación J2-8) | + robá 1/turno | **C** | motor de Arms |
| Carisma del Anhelo (Charisma of Yearning) | [REUSA CharismaOfYearning] | Poder | 2 | +2 Fuerza, 1 Vulnerable a TODOS, +10 NP (co-op: aliados +1 Fuerza — intacto) | 3 Fuerza | A/C/D | Inflame+ |
| Creación de Territorio (Territory Creation) | [REUSA TerritoryCreation] | Poder | 1 | Fin de turno: 3 Block + 10 NP (denominación J2-8) | 5 Block + 10 | B/C | 陣地作成; Metallicize+NP |
| Furia de la Adversidad (Adversity's Fury) | [REUSA AdversitysFury] | Ataque | 1 | 8; +4 con HP ≤75%, +4 más ≤50% (glow) | 10/+5/+5 | **D** | 逆境 |
| Realce de Locura (Madness Enhancement) | [REUSA MadnessEnhancement] | Poder | 1 | Perdés HP en tu turno → +10 NP (máx 3/turno; denominación J2-8) | máx 4/turno ⚙ | **D**→C | 狂化; Rupture |
| Lágrimas de la Salvadora (Savior's Tears) | [REUSA SaviorsTears] | Hab. | 1 | Curá 5; con HP ≤50%: 9 | 7/12 | **D** | Tonelico, la salvadora |
| Juramento de Sangre (Blood Oath) | **[NUEVA `BloodOath`]** | Ataque | 2 | Perdés 4 HP. 22 daño | 28/4 | **D** | frontload anti-élite que a D le faltaba |
| El Precio de la Corona (Price of the Crown) | **[NUEVA `PriceOfTheCrown`]** | Hab. | 0 | Perdés 3 HP. Ganá 1⚡. **Agotar** | perdés 2 | **D** energía | Seeing Red a sangre — **versión P3 obligatoria** (J1-6/J3-8); la versión poder de P1 queda prohibida |
| Lealtad de Woodwose (Woodwose's Loyalty) | **[NUEVA `WoodwoseLoyalty` — injerto de P1** (parche J1-15)] | Hab. | 1 | 8 Block; en Reina Hada/Invierno: se retiene este turno | 12 | **A**/D | tapa el hueco de ACAMPAR en la forma detonadora (`IBlockRetentionSource` existente) |

### 5.3 RARAS (20)

| Carta (eng) | Origen | Tipo | ⚡ | Efecto | Mejora | Arq. | Análogo |
|---|---|---|---|---|---|---|---|
| Soberana de Dos Caras (Sovereign of Two Faces) | [REUSA SovereignOfTwoFaces — **+cap**] | Poder | 1 | Cambiás de forma → robá 2 y +10 NP (**máx 2/turno** — J1-9/J2-9/J3-7, el cap más estricto del panel) | máx 3/turno | **A** | Mental Fortress |
| Maldición de Cernunnos (Curse of Cernunnos) | [REUSA CurseOfCernunnos — re-efecto M5] | Poder | 1 | Tus **Detonaciones** consumen solo la MITAD de la Maldición (redondeo arriba) | 0⚡ (**prohibida** «la primera detonación no consume» — J1-4) | **A**↔B | Ryoshu halving; el puente danza–attrition |
| Colección Final (Final Collection) | [REUSA FinalCollection — re-tipado] | **Hab.** | 2 | Consumí TODA la Maldición del objetivo: 3 daño por punto (sin glow con 0 Mald) | 4 | A/B payoff | Catalyst invertida — ver resolución de contradicción en §9 |
| Coronación del Invierno (Winter Coronation) | [REUSA WinterCoronation] | Hab. | 3 | Entrá en Reina del Invierno PERMANENTE. 4 Maldición a TODOS | 2⚡ | **A/B** clímax | Wraith Form-tier |
| Invierno Perpetuo (Perpetual Winter) | [REUSA PerpetualWinter] | Poder | 2 | Inicio de turno: 4 Maldición a TODOS | 5 | **B** | Noxious Fumes+; anti-strip |
| Impuesto Extraordinario (Extraordinary Tax) | [REUSA ExtraordinaryTax] | Hab. | 1 | 4 Maldición a TODOS; curá 2 por enemigo maldito | 5/3 | **B** | 臨時徴税 |
| Muro de Granizo (Hailstorm Wall) | [REUSA HailstormWall] | Hab. | 2 | 20 Block + 3 Maldición a TODOS | 26/4 | **B** | defensa mayor |
| Vigilia del Spriggan (Spriggan's Vigil) | [REUSA SprigganVigil — re-efecto] | Poder | 2 | Inicio de turno: Block = Maldición del enemigo más maldito (**máx 10**) | **máx 14** (cap conservador de P2 — parche J1-17) | **B** | conversión Maldición→defensa |
| CAMELOT SIN CAMINOS (ROADLESS CAMELOT) | [REUSA RoadlessCamelot] | NP | — | Consume TODA la Carga (**mín 50 — CAMBIO declarado: hoy es 70**, J3-13): AoE + escala por 10 de exceso + Maldición a TODOS + Bendición de Rhongomyniad | + | **C**/B/A | el NP canónico (AoE Arts + Curse) |
| Lluvia de Rhongomyniad (Rhongomyniad Rain) | [REUSA RhongomyniadRain] | NP | — | Consume TODA la Carga (mín 50): nuke ST, escala por 10 de exceso | + | **C**/D, jefes | el bombardeo de lanzas |
| MEMORIA DE LONDINIUM (MEMORY OF LONDINIUM) | [REUSA MemoryOfLondinium] | NP | — | Consume TODA la Carga (mín 50): AoE + 2 Knight's Arms; ≥100: +1 Arm y 1 Intangible | + | **C** | clímax defensivo |
| Regalo de Vivian (Vivian's Gift) | [REUSA ViviansGift] | Hab. | 0 | Agregá 3 Knight's Arms a tu mano. **Agotar (conservado — hoy lo tiene**; J1-11/J3-5) | Arms mejoradas | **C** | Infernal Blade; burst, no motor |
| Bajo el Árbol del Mundo (Under the World Tree) | [REUSA UnderTheWorldTree] | Hab. | 1 | Robá 3 + 20 NP | robá 4 | **C** | consistencia rara |
| Lanza de la Tirana (Tyrant's Lance) | [REUSA TyrantsLance] | Ataque | 2 | 18 daño; +10 si tenés Guts (glow). Perdés 3 HP | 24/+12 | **D** | payoff Guts |
| Venganza de la Salvadora (Savior's Vengeance) | [REUSA SaviorsVengeance] | Ataque | 1 | 8; +1 por cada 3 HP faltantes (máx 30 total) | máx 40 | **D** | la salvadora traicionada |
| Carisma de la Adversidad (Charisma of Adversity) | [REUSA CharismaOfAdversity] | Poder | 2 | Ataques +2 daño por umbral de HP faltante (0/25/50/75%); co-op: aliados +1 Fuerza (intacto) | +3 | **D** | 逆境のカリスマ A, 1:1 |
| Desde el Fin del Mundo (From the World's End) | [REUSA FromTheWorldsEnd] | Hab. | 1 | Guts (a 1 HP: +3 Fuerza, +50 NP) + 1 Weak a TODOS. Agotar | 2 Weak | **D**/C | el origen de Rhongomyniad |
| Último Recurso (Last Resort) | [REUSA LastResort] | Hab. | 1 | No antes del turno 3. +30 NP + Guts | turno 2 | **D**/C | fricción temporal |
| Pacto de Sangre Feérica (Fae Blood Pact) | [REUSA FaeBloodPact] | Poder | 1 | Inicio de turno: perdés 2 HP, +10 NP | +20 (corregido a denominación; el «+15» de P3 rompía 4.6.3) | **D**→C | Brutality |
| "Un Hogar con Morgan" (A Home with Morgan) | [REUSA AHomeWithMorgan] | Hab. | 1 | +4 HP máx y curá 4; con Guts: doble (glow). Agotar | 6 | **D** | el sueño de hogar |

### 5.4 Especiales / no drafteables (`CardRarity.Event`, sin cambios de estado)

| Carta | Origen | Rol |
|---|---|---|
| Knight's Arm | [REUSA KnightsArm] | 0⚡ generada; el proyectil de C; no genera Arms (sin recursión) |
| The Queen's Metamorphosis | [REUSA QueensMetamorphosis] | toggle Ethereal del cetro (T1 + re-armado a 100 NP) |
| Queen's Sentence: Unleashed | [REUSA QueensSentenceUnleashed] | la cosecha de la ventana: Detona a TODOS **sin consumir**, Retain/Agotar |

---

## 6. Reliquias

| Reliquia (eng) | Origen | Slot | Efecto |
|---|---|---|---|
| Rhongomyniad, Cetro de la Reina (Queen's Scepter) | [REUSA — 1 cambio: M3] | Starter | Como está (arranque en Hada; 1er cambio de forma por combate: +1⚡, robá 1, +10 NP; perder HP → 3 Maldición a enemigo aleatorio, cap 3/turno; Metamorfosis Ethereal gratis T1) **+ la Metamorfosis se re-arma al llegar a 100 NP** (1 vez por ventana). Sin multiplicador global ✓ |
| Coronación en el Fin del Mundo (World's End Coronation) | [REUSA — **re-efecto, injerto de P2** (parches J2-1/J3-4)] | Ancient (Touch of Orobas, `GetUpgradeReplacement()`) | **Reinstala TODO el motor del cetro** (perder HP → 3 Maldición cap 3/turno; Metamorfosis T1; re-armado a 100 NP) **+ conserva su +1⚡ por cambio de forma (1/turno)**. Cierra el contrato Ancient de DECISIONS: tomar Orobas ya no amputa la sembradora de D ni el M3. **El «+5 al cap de Maldición» de P2 queda DESCARTADO** (el cap es `const` en FGOCore, sin hook — J1-5/J3-3) |
| Espejo del Clan (Mirror Clan Glass) | [REUSA — **re-efecto de P2** (parche J1-2)] | Tienda/PC | La primera vez que cambiás de forma cada turno: 3 Block. (Queda prohibido el robo por cambio sin cap — falla compartida de P1/P3) |
| Impuesto de Existencia (Existence Tax) | [REUSA] | Tienda | Sin cambios (fin de turno: NP = Maldición total, con cap). Puente B→C |
| Tesorería del Spriggan (Spriggan Treasury) | [REUSA] | Tienda | Sin cambios (empezás con NP). Línea C |
| Mors Embotellada (Bottled Mors) | [REUSA] | PC | Sin cambios + **verificación multi-fase obligatoria** (§14: si la transición de fase emite muerte, debe transferir la Maldición) |
| Hilo de Habetrot (Habetrot Thread) | [REUSA] | PC | Sin cambios (Guts revive a 10 HP). Línea D |
| Corona de Espinas (Crown of Thorns) | **[NUEVA `CrownOfThorns`** — aprobada J2-18] | PC | La primera vez por turno que una CARTA te hace perder HP: 4 Block. Sustain D capado que no borra la debilidad |
| ~~Piedra de Afilar de los Caballeros (Knights' Whetstone)~~ | — | — | **CORTADA** (J2-17 sobre J1-20 — resolución al más restrictivo, §9). Se reconsidera solo si el playtest muestra a C-Arms corta de daño |
| Cáliz de la Dama del Lago / Sello de Invocación / Voto a la Reina | [REUSA ×3] | Grial (evento) / gacha NP / Bond | Sin cambios (DECISIONS cerradas; sin ×daño global ✓) |

---

## 7. Noble Phantasms y ventana

- **Cartas NP drafteables (raras C):** ROADLESS CAMELOT (AoE + Maldición + Blessing — el NP canónico de FGO, razón por la que el motor de Maldición ES fiel), Lluvia de Rhongomyniad (nuke ST para jefes), MEMORIA DE LONDINIUM (AoE + Arms + Intangible). Las tres consumen TODA la Carga con **mínimo 50** (cambio declarado desde el 70 actual de Camelot, escalado por 10 recalibrado en el mismo pase — knob J3-13). Denominaciones ✓.
- **Ventana a 100 NP (modelo único, §3.4):** manifiesta QSU + `ReturnResources` (+1⚡, robá 1) + re-arma la Metamorfosis. Una cosecha por pico. Sin ×2, sin eco automático.
- **Overcharge / niveles NP** por duplicados vía Sello de Invocación: sin cambios.
- El texto de QSU usa el keyword: *«**Detoná** la Maldición de cada enemigo **sin consumirla**»* — cierra el círculo de legibilidad.

---

## 8. Lista DEMOTE

**Demotes nuevos: NINGUNO.** Resolución del panel:

| Candidata | Propuso | Resultado |
|---|---|---|
| StormsWrath, HailstormWall, UnderTheWorldTree | P1 | **Rechazado** — la base P3 los conserva con re-efecto (re-efecto > quemar ID) |
| SillyMama | P2 | **Rechazado** (J2-16/J3-11) — se queda drafteable; su mejora no muta de rol |
| ProtectiveFrost | P3 | **Rechazado** (J2-12; contradicción con J3-11 resuelta en §9) — vuelve al pool común |

`KnightsArm`, `QueensMetamorphosis`, `QueensSentenceUnleashed` ya son `CardRarity.Event` y quedan así. **Ningún ID se renombra; ningún ID se quema.** Los saves cargan todo.

---

## 9. Registro de decisiones del panel

### 9.1 Ganadora

**Propuesta 3 (arquetipos), por unanimidad (3–0).** Motivos de los jueces: única cuyo contenido se sostiene contra el código real (mazo inicial, inventario 23/28/20, cetro completo, FGOCore intocado); única que cazó las cartas-trampa (`MirrorStrike` "este combate", condicionales de glow); la mejor contabilidad de draft (picks por línea por rareza, señalización de Acto 1); economía defensiva por construcción (caps en todo trigger, Precio de la Corona con Agotar, pico calculado dentro del techo).

### 9.2 Parches obligatorios aplicados (por juez)

**Juez 1 (infinitos/economía):** Truco del Espejo nunca 0⚡ (mejora = +10 NP de P2) · Espejo del Clan reliquia → 3 Block 1/turno (P2) · ventana sin ×2 · Cernunnos sin exención de consumo · cap Maldición 25 intacto, Ancient no lo sube · Precio de la Corona versión P3 (Agotar) · Recaudación 1⚡ · Soberana cap 2/turno (mejora 3) · re-arm 1/ventana sin doble ⚡ · Vivian con Agotar · denominaciones restauradas · injertos: flotante «¡Sentencia! +X», Woodwose (P1), grafo normativo (P2), Vigilia cap conservador, test-ledger de sangre, auditoría de pico escrita (§11) · `HuntCry`/`CourtStep`/`DanceOfTheSeasons` de P2 NO se crean.

**Juez 2 (draft/legibilidad):** Ancient porta el motor completo del cetro (P2) · mazo inicial sin cambios · tooltip de Detonar con reglas multi-golpe y multi-objetivo · flotante obligatorio · condición vacía = sin glow · Marca de la Bruja sin robo · Memoria del Fresno = válvula 30 NP→robá 2 (P2) · denominaciones +5→+10 · Soberana cap 2 · Mad Lunge nerfeada · Melusine con rider de Arms (P1) · Escarcha no se demotea · Colección Final Habilidad · Golpe Espejado "este turno" · QueensFury re-spec con **callout obligatorio en el changelog de Workshop de toda carta re-especificada sobre ID existente** (honestidad de P1) · SillyMama se queda · Whetstone cortada · Corona de Espinas aprobada.

**Juez 3 (jefes/co-op/técnica):** ventana = modelo único QSU + ReturnResources + re-arm (una cosecha por pico) · FGOCore intacto (cap `const`, sin hook) · Ancient reinstala motor + conserva +1⚡ · Vivian Agotar · mazo inicial y básicas con números actuales · Soberana cap 2 · Precio de la Corona P3 · injertos de P2: TyrantsBlood puente D→B, ReplicaLance multi-hit común, grafo como referencia · keyword y flotante de P1 · SillyMama se queda · verificación multi-fase de BottledMors · NPs a mín 50 declarado como cambio · co-op congelado · todo estado por turno vía `FgoCombatState`/powers visibles con reset en `BeforeSideTurnStart`.

### 9.3 Contradicciones entre jueces — resueltas al más restrictivo (anotadas)

| # | Tema | Posiciones | Resolución |
|---|---|---|---|
| 1 | **Ventana NP** | J1: eco AoE sin consumir + retorno; J3: sin eco automático, la cosecha vive SOLO en QSU | **J3** — una cosecha por pico (más restrictivo, y además el "eco" describía un motor que no existe desde 2026-06-26) |
| 2 | **Colección Final** | J1+J2: Habilidad; J3: Ataque (evidencia de código: `IUsesTargetCurse` ya resuelve la colisión con la Sentencia) | **Habilidad** (más restrictivo, 2–1). Anotado: el hallazgo de J3 invalida la premisa original del re-tipado, pero el tipo Habilidad se sostiene por un argumento de potencia independiente (evita que el cash-out total escale además con Fuerza/tipo Buster). Verificar igualmente el orden de hooks al implementar y dejar nota en el código |
| 3 | **Mad Lunge** | J1: 8 daño / perdés 3 HP; J2: 7 daño / perdés 2 HP | **7 daño / perdés 3 HP** (mejora 10 / perdés 3) — combinación más restrictiva de ambos extremos |
| 4 | **Demote de Escarcha Protectora** | J2: demote anulado (densidad defensiva común); J3: "demotes finales: solo ProtectiveFrost" | **No se demotea** — es la opción reversible (un demote quema el ID para siempre) y J2 da el argumento estructural explícito. Resultado: 0 demotes en la síntesis |
| 5 | **Knights' Whetstone** | J1 (parche 20): la lista entre los IDs a crear; J2 (parche 17): cortada | **Cortada** (menos IDs nuevos = más restrictivo). Reconsiderable post-playtest |

### 9.4 Injertos de las propuestas perdedoras

**De P1 (fidelidad):** keyword `Detonar` con su presentación (dorado, 5 idiomas) · texto flotante «¡Sentencia! +X» · **Lealtad de Woodwose** [NUEVA] · Agotar de Vivian's Gift · rider de Knight's Arm en Garra de Melusine · la disciplina de changelog para re-specs · su tabla de supervivencia al strip como doc de diseño.
**De P2 (motor):** el fix del Ancient/Orobas (el hallazgo estructural del panel) · el **grafo de conversión + tasas patrón + valores de referencia** como apéndice normativo (§13) · Memoria del Fresno como válvula NP→cartas · TyrantsBlood puente sangre→siembra · ReplicaLance multi-hit común anti-Buffer · re-efecto de Espejo del Clan · cap conservador de Vigilia del Spriggan · la regla del tooltip con resolución completa.

### 9.5 Balance del re-pool

**~50 [REUSA] sin cambio de efecto · ~24 [REUSA con re-efecto/coste/rareza nuevos] · 4 cartas [NUEVA]** (`ShiftingGuard`, `BloodOath`, `PriceOfTheCrown`, `WoodwoseLoyalty`) **· 1 reliquia [NUEVA]** (`CrownOfThorns`) **· 2 reliquias re-especificadas** (`WorldsEndCoronation`, `MirrorClanGlass`) **· 1 reliquia con 1 añadido** (cetro, M3) **· 0 demotes**.

---

## 10. Verificación de restricciones duras (una por una)

| Restricción | Estado | Evidencia |
|---|---|---|
| **IDs inmutables + plan DEMOTE completo** | ✅ | 0 renombres; 0 demotes nuevos (§8, decisión razonada); mod id `MorganBerserker` intacto; saves cargan todo |
| **Formas/recursos conservados** | ✅ | Motor REDESIGN intacto (3 formas, Sentencia, Guts, Arms, NP 0–300); Maldición cap 25 (`CursePower.MaxPerEnemy` no se toca); FGOCore intacto; HP/stats intactos; mazo inicial sin cambios verificado contra `MorganBerserker.cs` |
| **Regla 4.6** | ✅ | Básicas 4.6.1 exactas (Buster 10, Arts 6+30); denominaciones 10/20/30/50/100 en TODO el pool (corregidos los +5 de P3 y el +15 de Pacto de Sangre); conectividad comunes 25/25; glow 4.6.5 con regla de condición vacía; caps 3/turno en triggers del starter |
| **Techo 180–220** | ✅ | Auditoría de pico escrita en §11: peor caso construible ≈ 150–190; contingencias definidas |
| **~70–80 drafteables** | ✅ | 25 C + 30 PC + 20 R = **75** |
| **Arquetipos viables (4, tipo Ironclad)** | ✅ | Matriz §2 con las 7 columnas cubiertas por línea + debilidad real declarada; fila anti-strip por arquetipo; ≥4 picks por línea por rareza |
| **Interruptores de forma resueltos** | ✅ | Paridad 2–2 por dirección con puerta en COMÚN; vuelta a Hada es un Ataque; toggle común; cetro re-armable a 100 NP; sobrevive a Orobas gracias al re-efecto del Ancient |
| **Legibilidad de la Sentencia** | ✅ | Keyword `Detonar` (tooltip con las 3 reglas de resolución), pasivas M4 en dos frases/dos verbos, flotante «¡Sentencia! +X», glow, QSU con "sin consumirla" explícito |

---

## 11. Auditoría de pico (gate J1-19 — documentada ANTES de implementar)

**Peor caso construible en un turno** (setup previo: objetivo a cap 25 Maldición vía Cosecha, Cernunnos base activo, Carisma de la Adversidad al máximo (+8/ataque), 100 NP → ventana, mano ideal):

| Jugada | Cálculo | Daño |
|---|---|---|
| QSU (ventana): Detona sin consumir | 25 | 25 |
| Juramento de Sangre: 22 + 8 (Carisma) + Detonar 25 (Cernunnos consume 13 → quedan 12) | 55 | 80 |
| Buster: 10 + 8 + Detonar 12 (consume 6 → quedan 6) | 30 | 110 |
| Arremetida Demente (0⚡): 10 + 8 + Detonar 6 (consume 3 → quedan 3) | 24 | 134 |
| Quick (con el +1⚡ de la ventana): 6 + 8 + Detonar 3 | 17 | **151** |

Con Fuerza adicional del Anhelo (+2–3 por golpe) y mejoras: **~165–190**. Techo ≤220 ✅ (el formato de ventana sin ×2 y sin eco es lo que lo garantiza).
**Contingencias en orden si el playtest lo pasa:** 1) Cosecha de Maldición máx +10 → +8; 2) Cernunnos consume dos tercios en vez de la mitad.

---

## 12. Riesgos y knobs de playtest

### Riesgos honestos
1. **Danza demasiado fluida:** 2 switches por dirección + toggle común + formas 0⚡ + cetro re-armable puede pasar de "inaccesible" (el bug del reporte 1) a "trivial". Los frenos: Soberana cap 2/turno, Espejo del Clan sin robo, Coronación 1/turno, y el hecho de que ningún interruptor devuelve la carta que lo jugó.
2. **B contra strip-bosses:** mitigado por powers auto-re-sembradores y cobro pre-strip, no eliminado. Knob: subir re-siembra por turno; nunca proteger la Maldición del strip.
3. **Memoria del jugador:** ~24 IDs cambian de efecto conservando nombre y arte. Mitigación obligatoria: callout completo en el changelog de Workshop (parche J2-15).
4. **Cadena de sangre (test obligatorio, J1-18):** medir con ledger explícito Sangre de la Tirana → cetro → Locura (8 daño + 3 Mald carta + 3 Mald cetro + 10 NP por 1⚡ y 2 HP) contra las tasas patrón del apéndice.
5. **Cuatro 0⚡ de velocidad** (Velo, Vasallaje, Mamá Boba, Fresno): todas gastan un recurso (NP o Maldición) — ninguna rompe sola; juntas engrasan; vigilar.

### Knobs (números, no lógica; en orden de prioridad)
1. Furia de la Reina 9→7 si el switch-attack común domina el draft.
2. Soberana: mejora a 3/turno solo si la danza se siente ahogada.
3. Re-armado de Metamorfosis: cada ventana → cada 2 ventanas si acelera combates largos.
4. Cernunnos flojo tras el cap: mejora alternativa «+10 NP por Detonación» (nunca exención de consumo).
5. Ráfaga de Réplicas: 3×3 → ×2 golpes si el +10 NP/golpe corrido asusta.
6. Juramento de Sangre / Precio de la Corona: subir el costo de vida antes que bajar el efecto (la identidad D es sangrar de verdad).
7. Colección Final: 3/punto → 4 solo si B no cierra jefes.
8. Mínimo 50 de las cartas NP y su escalado por 10: recalibrar juntos.
9. Cap de Maldición: sigue en 25 en FGOCore — cualquier cambio es un pase FGOCore aparte con republish de los 12 personajes (fuera de alcance).

---

## 13. Apéndice normativo: grafo de conversión (injerto de P2, parche J1-16)

Toda carta de este pool debe poder ubicarse como arista del grafo y justificar su tasa contra el patrón. Regla: **ningún medidor se estanca — todo recurso tiene ≥2 entradas y ≥2 salidas en comunes.**

```
⚡ ──► daño (Buster, Desdén, Barrida)
│
├──► Maldición ◄── HP propio (cetro, Sangre de la Tirana)
│        │  ▲
│        │  └── NP (Velo de Niebla: 50 NP → 5 AoE)
│        ├──► daño    (Detonar, Barrida, Colección Final)
│        ├──► NP      (Vasallaje, Castigo Real, Recaudación)
│        ├──► HP      (Colmillo de Barghest, Impuesto Extraordinario)
│        └──► Block   (Acero Invernal, Vigilia del Spriggan)
│
├──► NP (Arts, Golpe de Cetro, Escarcha Protectora)
│        ├──► daño     (cartas NP, QSU)
│        ├──► Maldición(Velo de Niebla)
│        ├──► cartas   (Memoria del Fresno: 30 NP → robá 2)
│        └──► ⚡ + robo (ventana a 100)
│
└──► HP propio ──► NP (Realce de Locura, Sacrificio) ──► ciclo
```

**Tasas patrón (1⚡ común):** 9–10 daño plano · 6–8 daño + 3–4 Maldición · 4–5 AoE + 2 Maldición AoE · 6–8 daño + 10–20 NP · 5–6 Block + 10 NP. **A 0⚡:** 4 Maldición, o una válvula que GASTA (50 NP / 30 NP / Maldición / HP con Agotar).
**Valores de referencia:** 1 Maldición ≈ 1,4 daño esperado · 10 NP ≈ 3 daño diferido (las cartas NP pagan ≈ +1 daño por punto sobre el mínimo 50) · 1 evento de pérdida de HP ≈ 10 NP (Locura) ≈ 3 Maldición (cetro), ambos cap 3/turno.
**Breakpoints observables:** 100 NP = ventana · 25 = cap de Maldición = detonación plena · ≥50 NP = gate de Velo/NPs · Guts pendiente = gatillo de la línea D.

---

## 14. Notas de implementación (cuando el usuario apruebe)

1. **Estado por turno** (cap de Soberana; "cambiaste de forma este turno" de Golpe Espejado/Ira de la Tormenta/Guardia Cambiante; "primera pérdida de HP por carta" de Corona de Espinas): vía `FgoCombatState`/powers visibles — nunca campos privados (DECISIONS) — con reset en `BeforeSideTurnStart` solo si `participants.Contains(Owner)`.
2. **Colección Final:** verificar orden de hooks contra la Sentencia (`IUsesTargetCurse` ya existe en `FinalCollection.cs:18` / `WinterQueenFormPower.cs:54`); dejar test manual en forma Reina Hada y nota en el código.
3. **Ancient:** implementar el traspaso completo del motor del cetro en `WorldsEndCoronation`; test: tomar Orobas y verificar que perder HP sigue sembrando y que el re-arm a 100 NP sobrevive.
4. **Multi-fase (J3-12):** verificar si la transición de fase de jefe emite muerte de criatura; si sí, `BottledMors` transfiere la Maldición; si no, documentar que B pierde stacks al cambio de fase y que sus respuestas son el cobro pre-transición (Barrida, Vasallaje, Colección Final).
5. **Co-op congelado (J3-14):** riders existentes intactos, cero economía nueva hacia aliados, targeting aleatorio por `Rng.CombatCardGeneration` (patrón BottledMors). Cualquier rider co-op nuevo necesita pase propio del panel.
6. **RNG:** sin fuentes nuevas; todo por el stream compartido.
7. **Pipeline 4.6.7:** lotes por rareza (keyword Detonar + básicas/loc → comunes → PC → raras → reliquias) → loc ×5 idiomas → `audit_localization_parity` + `audit_simpleloc` (cierre `*词*` en zhs) → matriz MAIN/BETA → publish (FGOCore SIN cambios + Morgan). Pasar a la skill `sts2-fgo-mod-development`.
8. **Changelog de Workshop:** listado completo de toda carta/reliquia cuyo efecto cambió sobre ID existente (obligatorio, J2-15).
9. **Arte pendiente:** 4 cartas + 1 reliquia nuevas (Shifting Guard, Blood Oath, Price of the Crown, Woodwose's Loyalty, Crown of Thorns) — todas con material de Atlas/LB6 trazable.
