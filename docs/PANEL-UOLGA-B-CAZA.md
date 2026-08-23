# PROPUESTA B — U-Olga Marie: «La Caza y el Martirio»

*Panel §4.6.7 · Lente: exposición deliberada, Guts como clímax, la conversión de Autoridad como motor de segunda mitad. Construida encima del acta `DESIGN-UOLGA.md` §1-§8 sin tocar una sola decisión cerrada.*

---

## §1. La frase y los tres verbos

**La Directora que sale a buscar el golpe.** El medidor NP es su presupuesto (acta §1), pero acá el presupuesto se **cobra con el cuerpo**: cada golpe enemigo que le toca la Vida financia el decreto, cada combate con Amenaza es una cacería donde el Guts está armado, y la Forma 3 no se compra — se gana muriendo bien. Antes de transformarse juega como Bestia egoísta que apila poderes de **Mal**; después, como Protectora que decreta en área.

**Verbos del jugador:** **EXPONERSE** (elegir cuánto daño dejar pasar — el Bloqueo fino es una decisión, no un fracaso) → **COBRAR** (convertir la exposición en medidor y el medidor en NP o Decretos) → **CAZAR** (buscar la sala con Amenaza, pegarle a la Amenaza, y decidir si el combate termina con ella muerta o con vos levantándote).

**Canon que esta lente monetiza:** el 3% de NP por daño recibido (acta §0) es literalmente el motor; 人理の防人 (acta §3) es el clímax; el marcador de Amenaza (acta §4) es el mapa de la cacería.

---

## §2. Mazo inicial de 10 (QAABB sesgado a Arts/presupuesto)

HP base: **70** (banda 70-72, precedente Tiamat — kit explosivo y expuesto, el extremo frágil de la banda). Básicas conforme §4.6.1 exacto.

| Cant. | Carta ES | Carta EN | ⚡ | Efecto | Mejora |
|---:|---|---|---:|---|---|
| 2 | Buster | Buster | 1 | 10 daño | 13 |
| 2 | Arts | Arts | 1 | 6 daño, +30 Carga NP | 9, +30 |
| 1 | Quick | Quick | 1 | 6 daño, +30★ | 9, +30 |
| 3 | Defensa | Defender | 1 | 5 Bloqueo | 8 |
| 1 | **Cacería Declarada** (firma, `ITranscendenceCard`) | Declared Hunt | 1 | Ataque Arts: 6 daño, +10 Carga NP; **contra una Amenaza: +20 en su lugar** (glow) | 9, +20/+30 |
| 1 | **Amortiguación Calculada** | Calculated Cushion | 1 | 4 Bloqueo, +10 Carga NP | 6, +20 |

La firma enseña la lectura de Amenazas desde el turno 1 y cumple el contrato Ancient de DECISIONS (Archaic Tooth). **Amortiguación Calculada es la tesis del personaje en una común de mazo inicial:** bloquea *poco* a propósito — bloquear todo con ella es imposible, y el resto del golpe que pasa lo cobra el starter. La cuenta del turno 1: mano típica Arts+Arts+Buster = 60 NP + 10 daño; con dos golpes enemigos que tocan Vida, +20 más del starter → **~80 NP al cerrar T1, la primera decisión (NP vs. conversión) llega en T2-T3**. El personaje arranca decidiendo, no ahorrando.

---

## §3. Starter relic — Brazalete de la Dirección (Directorate Armband)

> **Cada Ataque enemigo que te quita al menos 1 de Vida: +10 Carga NP. Máximo 3 por turno.**

- Evento universal: recibir daño real. Recurso del kit: Carga NP. Es el 3% canónico (acta §0) escrito como regla.
- **Cap 3/turno, reset en `BeforeSideTurnStart` y sólo si `participants.Contains(Owner)`** (§4.6.4 literal). Techo del motor: 30 NP/turno.
- Un golpe **totalmente bloqueado no paga**. Esa es toda la lente en una línea: el Bloqueo es un dial, no un botón. Bloquear todo te deja sin presupuesto; no bloquear nada te deja sin Vida; el juego está en el medio.
- **Deslinde de Siegfried (mismo trigger, kit opuesto, declarado):** él cobra +5 detrás de una armadura de 80 HP que reduce cada golpe; ella cobra +10 con 70 HP y la cara descubierta. Él absorbe; ella *paga*. Y el deslinde de Morgan es estructural: acá la Vida perdida jamás se paga voluntariamente con cartas — sólo el enemigo te la saca (el candado de sangre del acta §5 se respeta en todo el pool: **cero autodaño**).
- Icono: clase **Beast en oro** (5★ → oro, regla WORKFLOW §6). Índice 0 de `StartingRelics` + `GetUpgradeReplacement()` (contrato Orobas de DECISIONS; su Ancient está en §5).

---

## §4. Pool de 68 recompensas — 20 C / 28 PC / 20 R

Convenciones: «si perdiste Vida este turno» = condición leída **en tu turno** (ningún trigger propio nuevo se apila sobre el starter — presupuesto agregado de triggers defensivos, lección P3 Siegfried). «Contra Amenaza» lee el marcador visible del acta §4. **Mal** = keyword de Poder con dos modos impresos: pleno como Mal, disminuido como Protectora (así no hay cartas muertas post-F3 y el precio de transformarse es real y legible). Glow dorado en TODA condicional. Denominaciones 10/20/30/50/100 en todo número de recurso (los +70 sólo en mejoras, precedente Kagetora §7.1).

### §4.1 COMUNES (20) — conectividad **20/20 bajo criterio duro** (los riders de forma NO cuentan)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---:|---|---|---|
| Rayo de Observación | Observation Ray | Ataque Arts | 1 | 7 daño, +10 NP; contra Amenaza: +10 NP más | 10, +20 base | NP; lee Amenaza |
| Chispa del Núcleo | Core Spark | Ataque Quick | 1 | 6 daño, +10★; si perdiste Vida este turno: +10★ más | 9, +20★ base | ★; lee exposición |
| Mandato Menor | Minor Mandate | Habilidad | 0 | Gastá 50★: +50 NP | +70 NP | ★→NP (espejo) |
| Informe de Daños | Damage Report | Habilidad | 0 | Gastá 50 NP: +50★ | +70★ | NP→★ (espejo) |
| Presupuesto Auxiliar | Auxiliary Budget | Habilidad | 1 | +20 NP, robá 1 | +30 NP | NP; robo |
| Escudo Orbital | Orbital Shield | Habilidad | 1 | 5 Bloqueo, +10 NP | 8, +10 | NP |
| Guardia a Desgano | Grudging Guard | Habilidad | 1 | 6 Bloqueo; si perdiste Vida este turno: +20 NP | 9 | NP; lee exposición |
| Desdén en Contraluz | Backlit Disdain | Ataque Buster | 1 | 12 daño; si perdiste Vida este turno: +10★ | 16 | ★; lee exposición |
| Órbita Rasante | Grazing Orbit | Ataque Quick | 1 | 4×2 daño, +10★ | 6×2 | ★; anti-Buffer |
| Lluvia de Escombros | Debris Rain | Ataque Buster, área | 1 | 6 a TODOS, +10 NP | 9 | NP; multiobjetivo |
| Auditoría Relámpago | Flash Audit | Habilidad | 1 | Robá 2, descartá 1; si descartaste un Ataque: +10★ | robá 3 | ★; consistencia |
| Trámite Urgente | Urgent Paperwork | Habilidad, Agotar | 0 | +20 NP | +30 | NP frontload |
| Polvo de Estrellas Muertas | Dead Star Dust | Habilidad, Agotar | 0 | +20★ | +30★ | ★ frontload |
| Ejecución Observada | Witnessed Execution | Ataque Buster | 1 | 9 daño; si fue Crítico: +20 NP | 12, +30 | crítico→NP |
| Persecución Estelar | Stellar Pursuit | Ataque Quick | 1 | 5 daño; si fue Crítico: +20★ | 8, +30★ | crítico→★ |
| Sanción Directa | Direct Sanction | Ataque Arts | 1 | 8 daño, +10 NP; si el objetivo pretende atacarte: +10 NP más | 11 | NP; lee intención (la caza: encarás al que viene por vos) |
| Línea de Contención | Containment Line | Habilidad | 1 | 6 Bloqueo, +10★ | 9, +20★ | ★ |
| Reasignación de Fondos | Fund Reallocation | Habilidad | 0 | Gastá 20★: robá 1 | robá 2 | ★→robo |
| Zarpazo de la UnBeast | UnBeast Swipe | Ataque Buster | 1 | 10 daño; contra Amenaza: +4 | 14, +6 | lee Amenaza |
| Pequeño Apocalipsis | Minor Apocalypse | Ataque Arts, área | 1 | 4 a TODOS, +10★ | 6, +20★ | ★; multiobjetivo |

Los pares espejo a 0⚡ (Mandato Menor ↔ Informe de Daños) garantizan que ningún medidor se estanque (§4.6.2). Cinco 0⚡, todos con gate de recurso o Agotar — sin cadenas gratis.

### §4.2 POCO COMUNES (28)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---:|---|---|---|
| Desdén Absoluto | Utter Disdain | Poder **Mal** | 1 | Mal: al final de tu turno, si te queda 0 de Bloqueo: +20 NP. Protectora: +10 | +30 / +20 | NP; premia exposición total |
| Apetito de Autoridad | Appetite for Authority | Poder **Mal** | 1 | Mal: tus Críticos otorgan +20 NP. Protectora: +10 | +30 / +20 | crítico→NP |
| Vigilancia Perpetua | Perpetual Vigilance | Poder | 1 | Al inicio de tu turno: +10 NP | además +10★ | NP/★ pasivo |
| Protocolo de Emergencia | Emergency Protocol | Poder | 1 | La primera vez que perdés Vida cada turno: 3 Bloqueo | 5 | defensa; lee exposición |
| Zona de Exclusión | Exclusion Zone | Poder | 1 | Cuando un Ataque enemigo no te quita Vida (anulado del todo): +10★, máx 2/turno | +20★ | ★; **la válvula anti-lente** (ver §4.5) |
| Bombardeo Orbital | Orbital Bombardment | Ataque Buster, área | 2 | 9 a TODOS, +10 NP | 12, +20 | NP; multiobjetivo |
| Doble Sanción | Double Sanction | Ataque Arts | 1 | 5×2 daño, +20 NP | 7×2, +30 | NP; anti-Buffer |
| Lanza de Fotones | Photon Lance | Ataque Buster | 2 | 18 daño; contra Amenaza: +6 | 24, +10 | lee Amenaza |
| Juicio de la Observadora | Observer's Judgment | Ataque Arts | 1 | 8 daño; si tenés 100+ de Carga NP (no la gasta): +20★ | 11, +30★ | lee banco NP→★ (tensión banquear vs convertir) |
| Cacería Mayor | Greater Hunt | Ataque Buster | 1 | 8 daño; contra Amenaza: +10 NP y +10★ | 11 | Amenaza→NP/★ |
| Redistribución Total | Total Redistribution | Habilidad | 1 | +30 NP; si perdiste Vida este turno: +50 en su lugar | +50 / +70 | NP; lee exposición |
| Segunda Línea | Second Line | Habilidad | 1 | 8 Bloqueo; como Protectora: 12 | 11 / 16 | defensa (rider F3) |
| Evacuación de Personal | Personnel Evacuation | Habilidad | 1 | 10 Bloqueo | 14 | defensa pura |
| Directiva a Chaldea | Chaldea Directive | Habilidad, aliado | 1 | Un jugador roba 2; vos +10 NP | roba 3 | co-op; NP |
| Orden de Confiscación | Confiscation Order | Habilidad | 0 | Gastá 30 NP: robá 2 | robá 3 | NP→robo |
| Parte de Batalla | Battle Report | Habilidad | 0 | Si perdiste Vida este turno: +20 NP y +10★ | +30 / +20 | payoff puro de exposición |
| Veda de Regeneración | Regeneration Ban | Habilidad | 1 | 6 daño; Bloqueo de Curación 2 turnos | 9; 3 turnos | anti-jefe (power de Tiamat, acta §5) |
| Depósito Blindado | Armored Deposit | Habilidad | 1 | 6 Bloqueo, +10★ | 9, +20★ | ★ |
| Ascenso Meteórico | Meteoric Rise | Ataque Buster | 3 | 24 daño; si mata al objetivo: +50 NP | 30 | NP; finisher |
| Requisitoria | Requisition | Habilidad, Agotar | 1 | +50 NP | 0⚡ | NP frontload |
| Cielo Cuadriculado | Gridded Sky | Habilidad, Agotar | 1 | +50★ | 0⚡ | ★ frontload |
| Guardaespaldas Involuntaria | Reluctant Bodyguard | Habilidad, aliado | 1 | Vos y otro jugador: 5 Bloqueo; vos +10 NP | 8/8 | co-op |
| Puesto de la Protectora | Sentinel's Post | Poder | 1 | Fin de turno: +2 Bloqueo; como Protectora: +5 | +4 / +8 | defensa escalada F3 |
| Inspección Sorpresa | Surprise Inspection | Ataque Quick | 1 | 6 daño, robá 1 | 9 | robo |
| Cierre de Ejercicio | Fiscal Close | Habilidad | 2 | +30 NP y +30★ | +50 NP y +30★ | NP/★ |
| Anexo al Decreto | Decree Annex | Habilidad, Agotar | 0 | Tu próximo Decreto de este combate pega +10 | +20 | escribe Autoridad |
| Marca de la Presa | Prey's Mark | Habilidad | 1 | 2 Vulnerable, +10 NP | 3 | debuff (el ÚNICO del pool — regla anti-strip) |
| Foco en la Presa | Focus on the Prey | Habilidad, Agotar | 1 | 1 Crítico Listo, +10★ | +20★ | crítico |

### §4.3 RARAS (20)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---:|---|---|---|
| Sin Precedente ni Igual (空前絶後) | Unparalleled | Habilidad | 1 | Descartá tu mano; robá esa misma cantidad y +20★. **Tu próximo turno robás 2 menos** | +30★; robás 1 menos | ★; consistencia con demérito (acta §5 literal; el sabotaje a aliados queda en el flavor text) |
| Decreto de Silencio | Decree of Silence | Habilidad, Agotar | 1 | Sello de Habilidad 1 turno a TODOS los enemigos; +10 NP | +20 NP | anti-jefe (`SkillSeal` AoE reutilizado, acta §5) |
| Perfección Sin Costuras (天衣無縫) | Seamless Perfection | Poder **Mal** | 1 | Mal: inicio de turno +10★ y tu primer Crítico del turno pega +6. Protectora: sólo +10★ | +20★ | ★/crítico |
| Ultra Manifiesto | Ultra Manifest | Poder | 1 | Tus Decretos pegan +10 | y al jugar un Decreto: +10★ | Autoridad (engorda el hilo, no abre uno) |
| Reactor de Emergencia | Emergency Reactor | Poder | 1 | Al consumir tu medidor entero (NP o conversión): +20★ y robá 2 | +30★ | suaviza el turno post-vaciado |
| Gravedad de un Planeta | A Planet's Gravity | Poder **Mal** | 2 | Mal: al final de cada turno en que perdiste Vida: +1 Fuerza (**tope +2**). Protectora: 3 Bloqueo en su lugar | además +10 NP por proc | Fuerza; lee exposición |
| Muralla de la Humanidad | Rampart of Humanity | Habilidad | 2 | 18 Bloqueo, Retain; como Protectora: 24 | 24 / 30 | defensa gorda (rider F3) |
| Manto de Contraluz | Backlight Mantle | Habilidad, Agotar | 2 | 1 Buffer, +10 NP | 2 Buffer | anti-pico enemigo |
| Trofeo de Caza | Hunting Trophy | Ataque Buster | 2 | 20 daño; contra Amenaza: +10 y +20 NP | 26 | Amenaza; finisher |
| Meteoro Dirigido | Guided Meteor | Ataque Buster, área | 2 | 12 a TODOS; contra Humano: +4 | 16, +6 | eco chico del special del NP |
| Presupuesto Total | Full Budget | Habilidad, Agotar | 1 | +50 NP; en combate contra Amenaza: +100 en su lugar | siempre +100 | NP; lee Amenaza |
| Sala de Mandos | Command Room | Poder | 1 | Robá 1 carta adicional cada turno | y +10 NP al inicio | consistencia |
| Custodia Final | Final Custody | Poder | 1 | La primera vez que perdés Vida cada turno: +10★ y 3 Bloqueo | +20★ y 5 | ★; exposición (paga en estrellas, NO en NP — no se apila con el starter) |
| Cuenta Regresiva | Countdown | Poder | 1 | Dentro de 3 turnos: +100 NP | 2 turnos | NP diferido (presupuesto a plazo fijo) |
| Peso del Mundo | Weight of the World | Ataque Buster | 3 | 30 daño; si tenés 200+ NP (no la gasta): +10 | 38 | lee banco |
| Orden Ejecutiva | Executive Order | Habilidad, aliado, Agotar | 1 | Todos los jugadores roban 2; vos +20 NP | +30 | co-op |
| Instinto de la UnBeast | UnBeast Instinct | Poder **Mal** | 1 | Mal: el primer impacto de cada Ataque tuyo contra una Amenaza pega +3 (patrón `DivinityPower`). Protectora: +1 | +5 / +2 | Amenaza |
| La Última Directora | The Last Director | Poder | 2 | Mal: inicio de turno +10 NP. Protectora: tus cartas Buster pegan +1 por impacto | +20 / +2 | capstone dual — cambia de función al transformarte |
| Regeneración del Núcleo | Core Regeneration | Habilidad, Agotar | 1 | Curá 6, +10 NP | 9, +20 | la única curación (la exposición necesita UNA salida) |
| Cinturón de Restos | Debris Belt | Ataque Quick | 1 | 3×3 daño, +20★ | 4×3, +30★ | ★; anti-Buffer |

**Matriz de cobertura** (§4.6 / rúbrica): frontload = Trámite/Requisitoria/Presupuesto Total/Cuenta Regresiva · defensa = 8 cartas + Buffer + Retain · consistencia = 6 fuentes de robo · economía = 5 engranajes 0⚡ con gate · escalado = 5 poderes Mal + Ultra Manifiesto + La Última Directora · multiobjetivo = 4 áreas + NP + Decretos F3 · **jefes que limpian buffs** = la Autoridad es un tanque de cartas en mano (acta §9: el strip no la toca), Sello, Veda, Buffer, y un solo debuff en todo el pool. Salida cuando el recurso central no llega = Zona de Exclusión + las dos Arts básicas + espejos.

---

## §5. Reliquias (12)

| Reliquia ES | EN | Rareza | Efecto |
|---|---|---|---|
| **Brazalete de la Dirección** | Directorate Armband | **Starter** | §3. Índice 0 + `GetUpgradeReplacement()` |
| Autorización Total | Full Authorization | Ancient (Orobas) | Lo mismo, y al tercer proc del turno: +10★. Reinstala forma, Guts y contadores (contrato DECISIONS) |
| **Conmoción de Cielo y Tierra (驚天動地)** | Earth-Shattering | **Ancient (jefe, drafteable)** | 1 vez por combate: llená tu medidor pagando **Vida imparable**: 1 por cada 10 faltante hasta 100; **1 por cada 5 por encima de 100** (el excedente hacia 300 sale más caro, acta §5). La Vida pagada **no alimenta nada** — ni el starter ni ninguna condición «perdiste Vida» (candado implementado, no prometido: el pago se marca y los lectores lo ignoran) |
| El Mañana Observado *(nombre a confirmar contra el Bond CE real de Atlas `444`)* | The Tomorrow She Watched | Bond | `BondRelic` estándar (lifts de la base, sin ×global — DECISIONS); capstone Nv10: al levantarte como Protectora, curá 10 |
| Archivo de la Dirección | Directorate Archive | NP store | `INpLevelStore` — los dupes suben el nivel del NP |
| Grial de la Dirección | Grail of the Directorate | Grial (evento Acto 2, 200 oro) | `ILimitBreaker` temático: **repara 単独顕現** (acta §6) |
| Anteojos de la Directora | Director's Glasses | Común | La primera carta Arts de cada turno: +10 NP |
| Café de la Sala de Mandos | Command Room Coffee | Común | Inicio de combate: +10 NP; contra Amenaza: +20 en su lugar |
| Insignia de Chaldea | Chaldea Badge | PC | Al entrar a un combate contra Amenaza: +20 NP y +10★ (preparar la cacería) |
| Balanza del Presupuesto | Budget Scale | PC | La primera vez que gastás 50★ en un turno: +10 NP |
| Fragmento de Cáldeas | Chaldeas Shard | Rara | Fin de tu turno: si todavía no consumiste el medidor este combate: +10 NP (acelera la primera decisión y se apaga sola) |
| Contrato de Seguro | Insurance Contract | Tienda | La primera vez por combate que un golpe te quita 10+ Vida: +20 NP y 5 Bloqueo |

**Techo pasivo documentado** (estilo Siegfried P3): starter 30 + Vigilancia 10 + Anteojos 10 + Balanza 10 + Fragmento 10 ≈ **70 NP/turno en el caso máximo construible** con 1 rara + 2 PC + jefe multi-hit — mismo orden que el precedente aprobado.

**Sobre el append `技能再装填`** (riesgo abierto del acta §8): esta propuesta lo **cambia** — un append que depende de un drop es mal diseño, confirmado. Propuesta: nivel bajo = *tu primera conversión de cada combate deja +20 NP de arranque en el medidor*; nivel alto = *+50*. Siempre hace algo, sinergiza con la reliquia de jefe sin exigirla, y no toca la tasa 1/50 ni el cap 5.

---

## §6. Carta-NP, Desatada y la conversión

**Planet Olga Marie (すでに過ぎし人理の終 / 既已过去的人理之终)** — `CardRarity.Event`, Ataque Buster, área, 0⚡, manifestada a 100+, `ConsumeAllForNpCard`:

- **5 impactos a TODOS**; por impacto = **6 + 2·Lv + 1 por cada 100 sobre el mínimo**.
- Contra **Humano** (salas Monster): **+3 por impacto** — el special real, nadie más premia limpiar el pasto (acta §5). Contra **Estrella**: +20% (guiño, redondeo hacia abajo, nunca línea de balance).
- **Desatada** (mejora): base 6→8 por impacto; contra Humano +3→+5.
- Referencias: tier 100/Lv1 = 35 AoE (50 vs Humano); tier 300/Lv3 = 70 AoE; Desatada 300/Lv3 = 80 (105 vs Humano). Comparable a Balmung (44-64) con la identidad desplazada al pasto. Los NP nunca critican (motor).

**Toma de Autoridad (Seizure of Authority)** — `CardRarity.Event`, Habilidad, 0⚡, manifestada a 100+ **junto con** la carta-NP (la elección del acta §2 es literalmente dos cartas en la mano): consume TODO el medidor, no hace daño, **+1 Decreto por cada 50 consumidos, cap 5**, llegan a la mano a razón de máximo 1 por turno, reloj de 5 turnos.

**Decreto (Decree)** — el token único, 0⚡: pega **tier consumido ÷ 10** (100→10 · 200→20 · 300→30). Sin tipo de comando (no dispara `CommandBonusPower` ni 逆光), no genera NP, no cuenta como «jugaste un Ataque» (`IsFirstInSeries`, candados del acta §2) — pero **sí** recibe Fuerza y los buffs que lo nombran (Ultra Manifiesto, Anexo). **Como Protectora: AoE con el daño por objetivo a la mitad, salvo enemigo único** (ley). Arte: command card oficial por ascensión.

---

## §7. Antes y después de la Forma 3

**Forma de Mal (F1-F2, cosméticas):** sin `FormPower` (precedente Gilgamesh). El «kit de Mal» son las **5 cartas Mal** (Desdén Absoluto, Apetito de Autoridad, Perfección Sin Costuras, Gravedad de un Planeta, Instinto de la UnBeast) más los 10 MaxHP que todavía no pagaste.

**Forma 3 — Protectora (人理の防人), irreversible, sólo si una Amenaza te mata:**
- **逆光 al levantarse** (una vez): 1 Anti-Purga (el registrado de Artoria, acta §3 — invulnerabilidad a 1 golpe), **+30 NP**, y **tus cartas Buster pegan +2 por impacto durante 3 turnos** — la ventana dramática.
- **Pasiva アトミックプラント:** +10 Carga NP al inicio de tu turno — el motor de segunda mitad: el ciclo medidor→decisión se acelera solo.
- **アルテミット・U:** los Decretos pasan a AoE (mitad por objetivo, salvo único).
- Los riders **«como Protectora»** se encienden en 8 cartas del pool (Segunda Línea, Puesto de la Protectora, Muralla de la Humanidad, La Última Directora + los 4 modos disminuidos/alternativos de las Mal duales… y la firma no — la caza es de las dos formas).
- **Precio:** −10 MaxHP permanente (`LoseMaxHp`, acta §3), los poderes Mal **en juego se remueven en el acto** (te levantás desnuda de tu ramp, en el combate más difícil, con 逆光 de red), y de ahí en más las 5 cartas Mal sólo juegan su **modo disminuido impreso**.

**Por qué quedarse en Mal es una línea, no un peaje:** (1) los 5 poderes Mal plenos son el mejor escalado sostenido del pool — la Fuerza de Gravedad, el +20 NP por crítico, el +6 al primer crítico; (2) conservás 10 MaxHP en un personaje de 70 que ya paga Vida por diseño; (3) el Guts **sigue siendo un seguro gratuito en cada combate con Amenaza aunque nunca lo gatilles** — jugar Mal no es renunciar a la red, es no necesitarla; (4) el pico de la línea Mal es ligeramente MÁS alto en objetivo único (§8) — la F3 compra área, consistencia pasiva y defensa, no un botón de «ganar». **El draft decide**: mazo cargado de Mal duales → jugá fino y no te dejes matar; mazo cargado de riders Protectora → salí a buscar el martirio. Ninguna de las dos es la default.

**Y si morís donde no hay Amenaza, morís de verdad.** El Guts sólo está armado en combates con Amenaza (acta §3). La cacería tiene mapa.

---

## §8. La cuenta del pico (techo 180-220)

Semántica usada (auditoría Kagetora §14.1): crítico ×1,5 multiplicativo por carta, 50★ por crítico, sin redondeo entre pasos; los NP no critican; la Fuerza y los flats por impacto multiplican por los 5 hits del NP — por eso Instinto es «primer impacto por Ataque» y Gravedad topea en +2.

**Línea Mal (nunca F3) — peor caso construible, vs jefe-Amenaza único.** Setup simultáneo exigido (8 condiciones): NP Lv3, una conversión previa a 300 (Decretos de 30 en reloj), re-banqueo a 300 (ingresos + reliquia de jefe pagando ~30 de Vida imparable), Gravedad al tope (+2 Fuerza), Instinto, Ultra Manifiesto, Anexo jugado, 100★ en banco.

| Jugada | ⚡ | Cálculo | Daño |
|---|---:|---|---:|
| Decreto (tier 300) | 0 | 30 +10 (Ultra) +10 (Anexo) +2 (Fuerza) +3 (Instinto) | 55 |
| NP Planet Olga Marie (300, Lv3, 5 impactos) | 0 | (6+6+2)=14/imp; +2 Fuerza ×5 = +10; +3 Instinto (1er imp) | 83 |
| Trofeo de Caza (el 1er crítico) | 2 | (20+10+3+2) × 1,5 | 52,5 |
| Desdén en Contraluz (el 2.º crítico) | 1 | (12+3+2) × 1,5 | 25,5 |
| | **3⚡** | | **≈ 216** |

**216 — dentro del techo, pegado al borde**, y sólo pagando Vida real y ocho condiciones a la vez. **Contesta la pregunta del encargo: el mazo sin Guts NO queda cojo** — ésta es la línea que nunca se transformó.

**Línea Protectora (post-F3, misma estructura, poderes Mal en modo disminuido, 逆光 ya expirado):** Decreto 51 + NP 71 + Trofeo 46,5 + Desdén 19,5 + La Última Directora (+1/impacto Buster) ≈ **196 a objetivo único** — y contra 3 enemigos el Decreto reparte 25×3 y el NP ya era AoE: la F3 gana en total de sala, no en single-target. Las dos líneas son competitivas por construcción, que era la carga de la prueba de esta lente.

**Contingencias en orden si el playtest pasa de 220** (nunca el daño base del NP — lección P10): 1) Anexo +10→ sin mejora +20; 2) Gravedad tope +2→+1; 3) Ultra Manifiesto +10→+5; 4) Instinto +3→+2.

---

## §9. Auto-crítica honesta — lo más frágil de ESTA propuesta

1. **La exposición no es agencia plena en StS2.** No elegís que te peguen; elegís no bloquear. Contra enemigos que pasan el turno debuffeando o cargando, el starter produce 0 y todas las condiciones «perdiste Vida» se apagan a la vez — el riesgo de «peaje aburrido» que mi lente declara no está eliminado, está acorralado (Zona de Exclusión, las Arts, los espejos). Si el playtest muestra salas enteras con el motor apagado, la válvula tiene que crecer.
2. **El martirio tiene un timing perverso optimizable.** El fallback de élite (acta §4) permite «pescar» la F3 barata dejándose matar a propósito en un combate fácil, convirtiendo el clímax dramático en un trámite de eficiencia. Si transformarse temprano domina, la línea Mal queda de peaje y la propuesta falla en su propia vara. Es EL número a vigilar en playtest; el knob honesto es achicar las pasivas de F3 (Atomic Plant +10→ nada el primer turno, 逆光 3→2 turnos), no agrandar el castigo.
3. **El starter comparte evento universal con Siegfried.** Mismo trigger (Ataque enemigo que quita Vida), distinta tasa y kit opuesto (armadura vs cara descubierta). La diferenciación es de diseño, no técnica — igual que el token vs el arsenal de Gilgamesh (acta §8) — y un juez puede legítimamente exigir un evento distinto. La defensa: es el 3% canónico de U-Olga; cambiar el evento rompe la fidelidad, no sólo la lente. Riesgo menor emparentado: los Decretos se pudren en peleas que terminan rápido (reloj de 5 turnos + 1/turno) — anti-sinergia declarada entre cazar rápido y decretar largo, que considero una tensión sana, no un bug.
