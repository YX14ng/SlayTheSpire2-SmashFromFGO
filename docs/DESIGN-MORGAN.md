# Diseño de personaje — Morgan (Berserker → Caster, Lostbelt 6) — v2

> Mod de personaje para **Slay the Spire 2** (rama MAIN, v0.103.x) sobre **BaseLib ≥ 3.1.8** + **FGOCore**.
> Convive con `MashShielder/` — NO pisa Baluarte, Intercepción ni Black Barrel.
>
> **v2 por pedido del usuario**: la columna vertebral es la **fidelidad al kit de FGO** (cada skill
> real de Morgan y de Aesc existe como carta o reliquia reconocible; los dos NP reales son las
> cartas-NP). La economía de Maldición/Impuesto del v1 NO desaparece: queda como **uno de los tres
> arquetipos drafteables** del pool (§5).
> Proveniencia: skill [sts2-mechanics-design](../.claude/skills/sts2-mechanics-design/SKILL.md)
> + panel de 3 diseños y 3 jueces (2026-06-11); v2 = el diseño "fiel-a-fgo" con los fixes de balance
> que los jueces le marcaron. Assets verificados contra Atlas Academy.

## 0. Filosofía de balance

**Rota pero honesta (techo Watcher).** Todo exceso embudado:

- Las cartas NP consumen **TODA** la Carga (FGOCore): gastar a 70 es renunciar a la ulti de 100.
- Lo explosivo paga **HP** (es una Berserker) o **Exhaust**; el auto-daño siempre
  `ValueProp.Unblockable|Unpowered`.
- La Maldición tiene **tope de 15 por enemigo** y su conversión a NP está capeada (+6/turno vía
  reliquia de pool) — sin doble-dip que gane por inercia.
- Plan A interrumpible: Artefacto/limpiezas apagan la Maldición; presión alta castiga quedarse
  en Bruja; enemigos con mucho Bloqueo niegan el NP-al-dañar de la Reina.

## 1. Identidad en una frase

**"La Reina que murió incontables veces: maldice, carga su lanza, y se alza de cada muerte."**

Fantasía: el kit real de Morgan jugado como mazo — *Carisma del Anhelo* para abrir, réplicas de
Rhongomyniad para castigar, *Desde el Confín del Mundo* para levantarse de la muerte con más
fuerza, y la danza entre sus dos vidas: la **Reina Berserker** que golpea y siembra maldiciones,
y **Aesc, la Bruja de la Lluvia (Caster)** que carga el Noble Phantasm bajo la lluvia de Orkney.
El loop esperado: sembrar en Reina → cruzar a Bruja a cargar NP → decidir **en qué forma cruzo
los 100** (¿ulti de daño o de supervivencia?) → volver a ejecutar.

## 2. Resumen de lore (terminología CN oficial)

- **Reina del Invierno (冬之女王)**: tirana 2000 años de la Britania de las Hadas (妖精国不列颠).
  Porta **Rhongomyniad** (止境之枪·伦戈米尼亚德) como magia: dispara réplicas de la lanza.
- **Kit Berserker real**: Carisma del Anhelo B (渴望的魅力), Protección del Lago C (湖之加护),
  Desde el Confín del Mundo A (来自止境 — Guts + críticos + aura), Madness Enhancement B (狂化),
  Item Construction EX (道具作成), Territory Creation B (阵地作成), Ojos Feéricos A (妖精眼).
  NP **Roadless Camelot** (业已无法抵达的理想乡): daño masivo + **Maldición 5t a todos** +
  "Overcharge +1 al usar NP".
- **Kit Caster real (Aesc, 雨之魔女梣)**: Hada del País de la Lluvia A (雨之国的妖精 — regen NP),
  Carisma de la Adversidad A (逆境的魅力 — escala con HP perdida), Último Recurso A
  (最后的度假胜地 — inutilizable 5 turnos). NP **Memory of Londinium** (圣剑遥远梦之遗痕):
  Invencible + las armas de los caballeros del pasado, presente y futuro.
- **Impuesto de Existencia (存在税)**: las hadas pagan energía o mueren → el arquetipo de la Tiranía.
- Reparto: Caballeros Hada (妖精骑士: Barghest 巴格斯特 / Melusine 梅柳齐娜 / Baobhan Sith 芭万·希),
  Spriggan (斯普里根), Mors (摩耳斯, criaturas-maldición), Cernunnos (科尔努诺斯), Albion (阿尔比恩),
  Clan del Espejo (镜之氏族), Habetrot (哈贝特洛特), Vivian/la Dama del Lago (薇薇安/水妃),
  el fresno árbol-mundo (梣). Memes CN: 想和摩根组建家庭, 笨蛋妈妈.

## 3. Stats base

| Atributo | Valor | Justificación |
|---|---|---|
| HP máximo | **76** | Rango vanilla 66-80; paga HP en ~8 cartas (precedente Ironclad 80) pero tiene Alzarse y curas condicionales. Entre Defect (75) e Ironclad (80). |
| Energía | 3⚡ | Estándar. |
| Oro inicial | 99 | Estándar. |
| Color | Blanco glacial y azul noche, acentos carmesí | Las cartas de Caster tintean a azul lluvia. |
| Carga NP | 0–300, ulti a 100 | FGOCore estándar. |
| Forma inicial | **Berserker** | Requisito del encargo. |

**Reliquia inicial — Rhongomyniad, el Cetro de la Reina (止境之枪·伦戈米尼亚德(王笏))**:
*La primera vez que cambias de forma en cada combate: gana 1⚡ y roba 1.*
(La mecánica pedida — cambiar de forma vía cartas — definida desde la starter; el primer cambio
es tempo-positivo, los demás los pagás vos. La conversión Maldición→NP que era starter en v1 se
movió al pool: es del arquetipo, no del personaje.)

**Mazo inicial (10):** 4× Golpe (1⚡, 6) · 4× Defender (1⚡, 5) · 2 firmas:

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| S1 | Lance of the World's End / **Lanza del Confín** / 止境之枪 | Ataque, 1⚡ | 8 de daño. Aplica 2 de Maldición. | +: 11 de daño, 3 de Maldición |
| S2 | Queen's Mandate / **Mandato de la Reina** / 女王的敕命 | Habilidad, 1⚡ | 5 de Bloqueo. Carga NP +10. Si algún enemigo tiene Maldición: +3 de Bloqueo. | +: 7 de Bloqueo, Carga +15 |

(Mismo presupuesto que las firmas de Mash; S1 siembra → S2 cobra: enseñan el loop desde el combate 1.)

## 4. Mecánicas y keywords

Presupuesto (§1.3 de la skill): **2 keywords originales** (Maldición, Alzarse) + FGOCore
(Carga NP, Formas, Vínculo/dupes/Grial).

### Maldición (Curse / 诅咒) — keyword nº1
*Al final del turno de la criatura maldita: pierde X de HP (ignora Bloqueo); luego la Maldición
se reduce en 1. Máximo 15 por enemigo.*

- Matemática de Veneno (X ≈ X(X+1)/2 de daño diferido) → **1⚡ ≈ 4-5 de Maldición pura**.
- Origen canónico: la Maldición 5t del NP *Roadless Camelot*.
- El **tope de 15** y la conversión capeada (+6/turno, reliquia de pool) son los fixes del panel
  al doble-dip: sin auto-win por inercia en peleas largas, ni con *Maldición de Cernunnos*.
- Implementación: daño directo a fin de turno (`CreatureCmd.Damage` Unblockable), NO tocar los
  hooks `ModifyHpLost*` (tabla de gotchas). Tooltip de daño del próximo tick = pulido deseable
  (precedente `PoisonPower.CalculateTotalDamageNextTurn()`).

### Alzarse (Guts / 毅力) — keyword nº2, el drama del personaje
*Power único (no acumulable; el más nuevo reemplaza al anterior). La primera vez que tu HP
llegaría a 0 este combate: no morís, quedás a 1 HP (cada fuente detalla bonus al activarse).*

- Es el Guts real de *Desde el Confín del Mundo* y *Último Recurso*: "murió incontables veces y
  volvió como Reina". Las fuentes dan bonus al dispararse — **morir ES el power spike de Morgan**.
- Vive en **FGOCore** (`GutsPower`, generalización del patrón `FouMiraclePower` ya probado):
  reutilizable por futuros servants.

### Carga NP (FGOCore, sin cambios)
- 0–300; mínimos: Lluvia de Rhongomyniad 40 · NPs grandes 70 · ultis 100. Sobrecarga lineal
  (+X por cada 10 sobre el mínimo, evaluada antes de pagar).
- **Manifestación a 100 dependiente de forma, FIJA**: al cruzar 100, se genera la ulti de la
  forma activa (Reina/Reina del Invierno → *ROADLESS CAMELOT: Desatado*; Bruja → *MEMORY OF
  LONDINIUM: Desatado*). La carta NO cambia si después cambiás de forma — "¿en qué forma cruzo
  los 100?" es la decisión de build del medidor. Re-arme al caer bajo 100.
- **Bendición de Rhongomyniad** (estado, el "Overcharge +1" del NP real): *tu próxima carta NP
  suma +10 a su Sobrecarga.*

### Inutilizable: turno N (mecánica menor, una carta)
*Último Recurso* no puede jugarse antes de tu 5º turno (contador visible) — su mecánica real de
FGO. El candado paga ~2× de valor (§3 de la skill).

### Multijugador (fiel al kit de soporte)
- *Carisma del Anhelo*: en co-op da además 1 de Fuerza a los aliados.
- *Hada del País de la Lluvia*: en co-op, aliados con medidor NP +3/turno.
- Regalos defensivos del Vínculo escalan ×(1+0.5×(jugadores−1)) (herencia BondRelic); ofensa no.

## 5. Los tres arquetipos drafteables

El pool está construido para que el mazo se incline hacia 1-2 de estos ejes (cada uno con
habilitadores en poco común y payoffs en raro). El mazo inicial funciona sin ninguno.

1. **La Carga de la Lanza (NP/Sobrecarga)** — el eje del kit: generadores de NP, las 3 cartas NP
   con mínimo, Bendición, y la danza de formas para cargar más rápido. Soportado por las skills
   reales de batería (Protección del Lago, Hada del País de la Lluvia).
2. **La Sangre de la Reina (HP como recurso + Alzarse)** — Madness Enhancement, Carisma de la
   Adversidad (umbrales de HP perdida), pagar vida por efecto, y levantarse de la muerte con
   Fuerza y NP. El lado Berserker del kit.
3. **La Tiranía (Maldición/Impuesto)** — **el diseño v1 conservado como arquetipo**: sembrar
   Maldición, mantenerla y cobrarla (como DoT, como NP vía *Impuesto de Existencia* — ahora
   reliquia de pool — o detonándola con *Cobro Final*). Origen canónico: la Maldición del NP.

## 6. Formas (FGOCore: `FormPower` + `FormSwitch` + `FormVisuals`)

| Forma | Modelo | Pasiva | Qué decisión cambia |
|---|---|---|---|
| **La Reina (Berserker)** — 妖精女王（狂战士） — inicial | **704020** (fallback 704010) | La primera vez que dañes el HP de un enemigo cada turno: **Carga NP +5**. Tus cartas que aplican Maldición aplican **+1**. | Querés ATACAR y que el daño entre (mucho Bloqueo enemigo niega el NP). Se siembra en Reina. |
| **Bruja de la Lluvia (Caster)** — 雨之魔女·梣 | **505320** (fallback 505310) | Al inicio de tu turno: **Carga NP +5**. Tus Ataques hacen **2 menos de daño**. | Cargar sin atacar; turnos de Habilidades/Poderes y defensa. La penalidad muerde si te quedás a pegar. |
| **Reina del Invierno** — 冬之女王 — permanente (`IsPermanent`), clímax vía carta rara | **704030** (traje oficial) | **Ambas pasivas, sin la penalización de Caster.** La ulti manifestada es la de la Reina. | El clímax: deja de existir la danza, empieza la ejecución (rol del Paladín de Mash). |

(Fix del panel: la pasiva de Bruja baja de +6 a **+5 NP/turno** — anti-"Caster estacionaria";
con la starter ya no generando NP pasivo, los grifos apilables quedan en banda.)

Cartas de cambio (§5 de la skill: baratas, efecto inmediato, ~2-3 cambios por combate):
*Cambio: La Reina* (#33), *Cambio: Bruja de la Lluvia* (#34), *Truco del Clan del Espejo* (#40),
*Coronación del Invierno* (#57, permanente).

## 7. Pool de cartas

### 7.1 Comunes (20) — 10 Ataques / 10 Habilidades

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 1 | Replica Lance / **Réplica de Lanza** / 长枪复制品 | Ataque, 1⚡ | 9 de daño. | +: 12 |
| 2 | Cursed Bolt / **Saeta Maldita** / 诅咒之矢 | Ataque, 1⚡ | 6 de daño. Aplica 2 de Maldición. | +: 8 y 3 |
| 3 | Queen's Scorn / **Desdén de la Reina** / 女王的轻蔑 | Ataque, 0⚡ | 4 de daño. +3 si el objetivo tiene Maldición. | +: 6, +4 |
| 4 | Mad Lunge / **Embestida Demente** / 狂化突进 | Ataque, 2⚡ | 20 de daño. Pierdes 2 HP. | +: 26 |
| 5 | Tyrant's Sweep / **Barrido de la Tirana** / 暴君横扫 | Ataque, 1⚡ | 6 de daño a TODOS. | +: 9 a TODOS |
| 6 | Scepter Blow / **Golpe de Cetro** / 王笏之击 | Ataque, 1⚡ | 8 de daño. Carga NP +5. | +: 11, +8 |
| 7 | Twin Replicas / **Réplica Doble** / 双重复制 | Ataque, 1⚡ | 4 de daño ×2. Carga NP +4. | +: 6×2, +6 |
| 8 | Royal Punishment / **Castigo Real** / 女王的惩罚 | Ataque, 2⚡ | 12 de daño. Aplica 2 de Vulnerable. | +: 16 |
| 9 | Tyrant's Blood / **Sangre de Tirana** / 暴君之血 | Ataque, 0⚡ | 6 de daño. Pierdes 1 HP. | +: 9 |
| 10 | Queen's Fury / **Furia de la Reina** / 女王之怒 | Ataque, 1⚡ | 10 de daño. En forma Reina: Carga NP +8. | +: 13, +10 |
| 11 | Mist Veil / **Velo de Niebla** / 雾之帷幕 | Habilidad, 1⚡ | 8 de Bloqueo. | +: 12 |
| 12 | Cursed Rain / **Lluvia Maldita** / 诅咒之雨 | Habilidad, 1⚡ | Aplica 2 de Maldición a TODOS. Carga NP +5. | +: 3 a TODOS, +8 |
| 13 | Witch's Mark / **Marca de la Bruja** / 魔女的印记 | Habilidad, 0⚡ | Aplica 3 de Maldición. | +: 5 |
| 14 | Tax Collection / **Recaudación** / 征税 | Habilidad, 1⚡ | Carga NP +12. Si algún enemigo tiene Maldición: +8 adicional. | +: +16, +12 |
| 15 | Royal Edict / **Edicto Real** / 女王敕令 | Habilidad, 1⚡ | Roba 2. Carga NP +5. | +: roba 3 |
| 16 | Protective Frost / **Escarcha Protectora** / 护身之霜 | Habilidad, 1⚡ | 6 de Bloqueo. Carga NP +6. | +: 8, +10 |
| 17 | Queen's Gaze / **Mirada de la Reina** / 女王的凝视 | Habilidad, 0⚡ | Aplica 1 de Débil. Carga NP +5. | +: 2 de Débil, +8 |
| 18 | Rain Chant / **Canto de la Lluvia** / 雨之咏唱 | Habilidad, 1⚡ | 5 de Bloqueo. En forma Bruja: Carga NP +10. | +: 8, +14 |
| 19 | Winter Steel / **Acero del Invierno** / 寒冬之铁 | Habilidad, 2⚡ | 15 de Bloqueo. | +: 20 |
| 20 | Vassalage / **Vasallaje** / 臣从之礼 | Habilidad, 0⚡ | 4 de Bloqueo. | +: 6, roba 1 |

### 7.2 Poco comunes (28) — 10 Ataques / 12 Habilidades / 6 Poderes

Aquí viven 4 skills del kit (#31, #32, #43, #44) y las cartas de cambio de forma.

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 21 | Replica Barrage / **Andanada de Réplicas** / 复制枪连射 | Ataque, 2⚡ | 5 de daño ×3. Carga NP +5 por golpe que dañe HP. | +: 6×3 |
| 22 | Barghest's Fang / **Colmillo de Barghest** / 巴格斯特之牙 | Ataque, 2⚡ | 16 de daño. Si el objetivo tiene Maldición: cura 4 HP. | +: 21, cura 6 |
| 23 | Melusine's Talon / **Garra de Melusine** / 梅柳齐娜之爪 | Ataque, 1⚡ | 12 de daño. | +: 16 |
| 24 | Baobhan Sith's Shriek / **Chillido de Baobhan Sith** / 芭万·希的尖啸 | Ataque, 1⚡ | 9 de daño. Aplica 2 de Maldición. | +: 12 y 3 |
| 25 | Wild Hunt Charge / **Carga de la Cacería** / 狂猎冲锋 | Ataque, 2⚡ | 11 de daño a TODOS. | +: 15 a TODOS |
| 26 | Storm's Wrath / **Ira de la Tormenta** / 风暴之怒 | Ataque, 3⚡ | 26 de daño. Aplica 3 de Maldición. | +: 32 y 4 |
| 27 | Adversity's Fury / **Furia de la Adversidad** / 逆境之怒 | Ataque, 1⚡ | 8 de daño. +4 si tu HP ≤ 75%; +4 más si ≤ 50%. | +: 10 base |
| 28 | Royal Execution / **Ejecución Real** / 女王处刑 | Ataque, 2⚡ | 14 de daño. Si mata al objetivo: Carga NP +25 y 2 de Maldición a TODOS. | +: 18, +30 |
| 29 | Albion's Breath / **Aliento de Albion** / 阿尔比恩的吐息 | Ataque, 3⚡ | 9 de daño ×3 (aleatorio). | +: 11×3 |
| 30 | Mirror Strike / **Golpe del Espejo** / 镜之一击 | Ataque, 1⚡ | 6 de daño ×2. Si cambiaste de forma este combate: ×3. | +: 8 por golpe |
| 31 | Charisma of Yearning / **Carisma del Anhelo** / 渴望的魅力 | Habilidad, 2⚡ | Gana 2 de Fuerza. Aplica 1 de Vulnerable a TODOS. Carga NP +10. *(co-op: aliados +1 Fuerza)* | +: 3 de Fuerza, +15 |
| 32 | Protection of the Lake / **Protección del Lago** / 湖之加护 | Habilidad, 1⚡ | Carga NP +20. En forma Bruja: roba 1. | +: +28 |
| 33 | Form: The Fairy Queen / **Cambio: La Reina** / 切换：妖精女王 | Habilidad, 1⚡ | Entra en forma **Reina**. Aplica 3 de Maldición. | +: 5 |
| 34 | Form: Rain Witch / **Cambio: Bruja de la Lluvia** / 切换：雨之魔女 | Habilidad, 0⚡ | Entra en forma **Bruja de la Lluvia**. Carga NP +10. | +: +18 |
| 35 | Winter Decree / **Decreto de Invierno** / 寒冬敕令 | Habilidad, 2⚡ | 12 de Bloqueo. Aplica 2 de Maldición a TODOS. | +: 16 y 3 |
| 36 | Embrace of the Lake / **Abrazo de la Dama del Lago** / 水妃的拥抱 | Habilidad, 1⚡ | 12 de Bloqueo. Solo jugable en forma Bruja. | +: 16 |
| 37 | Winter Thorns / **Espinas del Invierno** / 寒冬荆棘 | Habilidad, 1⚡ | 7 de Bloqueo. Los enemigos que te ataquen este turno ganan 2 de Maldición. | +: 10 y 3 |
| 38 | Curse Harvest / **Cosecha de Maldiciones** / 诅咒收割 | Habilidad, 1⚡ | Duplica la Maldición de UN enemigo (máx. +6). | +: máx. +10 |
| 39 | Memory of the Ash Tree / **Memoria del Fresno** / 梣树之忆 | Habilidad, 0⚡ | Carga NP +8. Roba 1. Exhaust. | +: +14 |
| 40 | Mirror Clan's Trick / **Truco del Clan del Espejo** / 镜之氏族的把戏 | Habilidad, 1⚡ | Cambia a tu forma opuesta. Roba 2. | +: roba 3 |
| 41 | Savior's Tears / **Lágrimas de la Salvadora** / 救世妖精之泪 | Habilidad, 1⚡ | Cura 5 HP; si tu HP ≤ 50%: cura 9. Exhaust. | +: 7 / 12 |
| 42 | Call of the Fairy Knights / **Llamado de los Caballeros Hada** / 妖精骑士召集 | Habilidad, 2⚡ | 2 de Maldición a TODOS (Barghest), 1 de Débil a TODOS (Baobhan Sith), 6 de Bloqueo (Melusine). Exhaust. | +: 3 / 2 / 9 |
| 43 | Fairy of the Rainland / **Hada del País de la Lluvia** / 雨之国的妖精 | Poder, 1⚡ | Al jugarla: Carga NP +15. Al inicio de cada turno: +5. *(co-op: aliados con NP +3/turno)* | +: +20 al jugarla, +8/turno |
| 44 | Madness Enhancement / **Refuerzo de Locura** / 狂化 | Poder, 1⚡ | Cada vez que pierdas HP durante tu turno: Carga NP +6 (máx. +12 por turno). | +: +9 (máx. +18) |
| 45 | Fairy Eyes / **Ojos Feéricos** / 妖精眼 | Poder, 1⚡ | Al inicio de cada turno: aplica 1 de Maldición a TODOS. | +: 2 a TODOS |
| 46 | Territory Creation / **Creación de Territorio** / 阵地作成 | Poder, 1⚡ | Al final de tu turno: gana 4 de Bloqueo. | +: 6 |
| 47 | Item Construction / **Creación de Objetos** / 道具作成 | Poder, 2⚡ | Tus cartas que aplican Maldición aplican +1. | +: coste 1⚡ |
| 48 | Winter Court / **Corte del Invierno** / 冬之宫廷 | Poder, 2⚡ | Roba 1 carta adicional cada turno. | +: coste 1⚡ |

(#44 ahora con tope por turno — fix del panel a los grifos de NP apilables.)

### 7.3 Raras (20) — 6 Ataques / 8 Habilidades / 6 Poderes

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 49 | **ROADLESS CAMELOT** / 业已无法抵达的理想乡 | Ataque NP, 2⚡ (mín. 70, consume TODA) | 30 de daño a TODOS. Aplica 4 de Maldición a TODOS. Ganas **Bendición de Rhongomyniad**. **Sobrecarga**: +3 de daño por cada 10. | +: 38, 5 de Maldición |
| 50 | **MEMORY OF LONDINIUM** / 圣剑遥远梦之遗痕 | Ataque NP, 2⚡ (mín. 70, consume TODA) | 18 de daño a TODOS. Gana 1 de **Intangible**. Añade 2 **Armas del Caballero**. **Sobrecarga**: +2 de daño por cada 10; a 100+: +1 Arma. | +: 24, 3 Armas |
| 51 | Rhongomyniad Rain / **Lluvia de Rhongomyniad** / 伦戈米尼亚德之雨 | Ataque NP, 1⚡ (mín. 40, consume TODA) | 18 de daño. **Sobrecarga**: +4 por cada 10. | +: 24 |
| 52 | Tyrant's Lance / **Lanza de la Tirana** / 暴君之枪 | Ataque, 2⚡ | 24 de daño. Pierdes 4 HP. | +: 30 |
| 53 | Savior's Vengeance / **Venganza de la Salvadora** / 救世妖精的复仇 | Ataque, 1⚡ | 6 de daño + tu HP perdida ÷ 5 (máx. 20 total). | +: ÷4 (máx. 24) |
| 54 | Final Collection / **Cobro Final** / 最后的清算 | Ataque, 1⚡ | Consume TODA la Maldición del objetivo: 2 de daño por punto. | +: 3 por punto |
| 55 | From the World's End / **Desde el Confín del Mundo** / 来自止境 | Habilidad, 2⚡, Exhaust | Gana **Alzarse**: al activarse quedás a 1 HP, ganás 3 de Fuerza y Carga NP +50. Aplica 1 de Débil a TODOS. | +: quedás a 10 HP, 2 de Débil |
| 56 | Last Resort / **Último Recurso** / 最后的度假胜地 | Habilidad, 1⚡, Exhaust | **No puede jugarse antes de tu 5º turno.** Carga NP +50. Gana **Alzarse** (1 HP). | +: desde el turno 4, Carga +60 |
| 57 | Winter Coronation / **Coronación del Invierno** / 冬之女王戴冠 | Habilidad, 2⚡, Exhaust | Entra en forma **Reina del Invierno** (permanente). Aplica 3 de Maldición a TODOS. | +: coste 1⚡, 4 |
| 58 | Extraordinary Tax / **Impuesto Extraordinario** / 临时增税 | Habilidad, 1⚡, Exhaust | Aplica 4 de Maldición a TODOS. Cura 2 HP por cada enemigo con Maldición. | +: 6, cura 3 |
| 59 | Under the World Tree / **Bajo el Árbol del Mundo** / 世界树之下 | Habilidad, 2⚡ | Roba 3. Carga NP +20. | +: roba 4, +25 |
| 60 | Vivian's Gift / **Regalo de Vivian** / 薇薇安的赠礼 | Habilidad, 1⚡, Exhaust | Añade 2 **Armas del Caballero**. Este combate, tus Armas hacen +3 de daño. | +: 3 Armas |
| 61 | Hailstorm Wall / **Muralla de Granizo** / 冰雹之壁 | Habilidad, 2⚡ | 16 de Bloqueo. Aplica 2 de Maldición a TODOS. | +: 22 y 3 |
| 62 | "A Home with Morgan" / **«Un Hogar con Morgan»** / 想和摩根组建家庭 | Habilidad, 1⚡, Exhaust | +3 HP máximos. Cura 3 HP. *(meme CN del pool)* | +: +5 y 5 |
| 63 | Charisma of Adversity / **Carisma de la Adversidad** / 逆境的魅力 | Poder, 1⚡ | Tus ataques hacen +1/+2/+3/+4 de daño si te falta al menos 1%/25%/50%/75% de tu HP. | +: +2/+3/+4/+6 |
| 64 | Curse of Cernunnos / **Maldición de Cernunnos** / 科尔努诺斯的诅咒 | Poder, 2⚡ | Tus Maldiciones ya no se reducen al activarse. | +: coste 1⚡ |
| 65 | Sovereign of Two Faces / **Soberana de Dos Rostros** / 双面君主 | Poder, 1⚡ | Cada vez que cambias de forma: roba 2 y Carga NP +10. | +: roba 3, +15 |
| 66 | Spriggan's Vigil / **Vigilia de Spriggan** / 斯普里根的看守 | Poder, 2⚡ | Al inicio de cada turno: gana 5 de Bloqueo. | +: 7 |
| 67 | Perpetual Winter / **Invierno Perpetuo** / 永恒之冬 | Poder, 3⚡ | Al inicio de cada turno: aplica 2 de Maldición a TODOS. | +: coste 2⚡ |
| 68 | Fae Blood Pact / **Pacto de Sangre Feérica** / 妖精血之契约 | Poder, 2⚡ | Al inicio de cada turno: pierdes 2 HP y Carga NP +12. | +: +18 |

Fixes del panel sobre el v-base: #54 ahora CONSUME (el detonador del arquetipo de Tiranía, con
tope implícito por el cap de 15 → máx. 30/45); #56 pierde el "+50% a la próxima NP" (excedía el
presupuesto del candado); #64 queda acotado por el tope de Maldición 15 y el cap del Impuesto.
Nombres zhs corregidos a terminología oficial (渴望的魅力, 逆境的魅力, 来自止境, 最后的度假胜地, 湖之加护).

### 7.4 Especiales generadas

| Carta (eng / es / zhs) | Tipo, Coste | Efecto |
|---|---|---|
| **ROADLESS CAMELOT: Desatado** / 业已无法抵达的理想乡·全解放 *(Reina / Reina del Invierno)* | Ataque NP, 0⚡ (mín. 100, consume TODA), Exhaust | 36 de daño a TODOS. 5 de Maldición a TODOS. Ganás **Bendición de Rhongomyniad**. **Sobrecarga**: +3 de daño por cada 10 sobre 100. |
| **MEMORY OF LONDINIUM: Desatado** / 圣剑遥远梦之遗痕·全解放 *(Bruja de la Lluvia)* | Ataque NP, 0⚡ (mín. 100, consume TODA), Exhaust | 24 de daño a TODOS. Gana 1 de **Intangible**. Añade 2 **Armas del Caballero**. **Sobrecarga**: +2 de daño por cada 10; a 200+: +1 Arma. |
| Knight's Arm / **Arma del Caballero** / 骑士的武器 | Ataque, 0⚡, Exhaust | 5 de daño. *("Las espadas de los caballeros del pasado, presente y futuro.")* |
| **Bendición de Rhongomyniad** / 伦戈米尼亚德的祝福 | Estado (power) | Tu próxima carta NP suma +10 a su Sobrecarga. |

Meme común adicional del pool: **Mamá Boba** / 笨蛋妈妈 — Habilidad, 0⚡: roba 1, Carga NP +5
*(+: roba 1, +10, 2 de Bloqueo)*.

## 8. Reliquias del personaje

| Reliquia (eng / es / zhs) | Tier | Efecto |
|---|---|---|
| Rhongomyniad, the Queen's Scepter / **el Cetro de la Reina** / 止境之枪(王笏) | **Inicial** | La primera vez que cambias de forma en cada combate: gana 1⚡ y roba 1. |
| Coronation at World's End / **Coronación del Confín** / 止境的加冕 | Jefe (reemplaza la inicial) | Cada vez que cambias de forma: gana 1⚡ (máx. 1 vez por turno). |
| **Existence Tax / Impuesto de Existencia / 存在税** | Tienda/Poco común | Al final de tu turno: gana Carga NP igual al total de Maldición de los enemigos (máx. +6). *(El habilitador del arquetipo de la Tiranía — ya no es starter.)* |
| Spriggan's Treasury / **Tesoro de Spriggan** / 斯普里根的宝库 | Tienda | Empiezas cada combate con 30 de Carga NP. |
| Mirror Clan's Mirror / **Espejo del Clan** / 镜之氏族的魔镜 | Poco común | Cada vez que cambias de forma: roba 1. |
| Bottled Mors / **Mors Embotellado** / 瓶中的摩耳斯 | Rara | Cuando un enemigo muere con Maldición: su Maldición salta a un enemigo aleatorio. |
| Habetrot's Thread / **Hilo de Habetrot** / 哈贝特洛特的纺线 | Rara/Evento | La primera vez que **Alzarse** se active en cada combate: quedás a 10 HP en vez de 1. |
| Chalice of the Lady of the Lake / **Cáliz de la Dama del Lago** / 水妃的圣杯 | Rara (`ILimitBreaker`) | +15 HP máx al obtenerla; mientras la tengas: Vínculo hasta Nv12 y NP level hasta 6. *(El "Santo Grial" de Morgan.)* |
| Queen's Summon Seal / **Sello de Invocación de la Reina** / 女王的呼符 | Starter oculta (`INpLevelStore`) | Contador NP level 1-5; botón "Invocar (dupe)" en recompensas (50% +25% pity, patrón SummonTicket). |
| Oath to the Queen / **Juramento a la Reina** / 向女王宣誓效忠 | Starter (`BondRelic`) | El Vínculo de Morgan (§9). |

## 9. Vínculo, dupes y la ulti

### Vínculo — «Juramento a la Reina»
Puntos y umbrales default de FGOCore. Regalos (overrides mínimos sobre los defaults):

| Nv | Regalo |
|---|---|
| 1 / 3 / 6 / 9 | +3/+3/+4/+5 HP máx (defaults, escalan en multijugador) |
| 2 / 5 / 8 | Empiezas con 5/10/15 de Carga NP (defaults) |
| 4 / 7 | Empiezas aplicando 1/2 de Maldición a TODOS *(reemplaza el Bloqueo default — Morgan no es tanque; mismo valor, sabor de tirana)* |
| 10 | **Capstone «Un hogar con Morgan» (想和摩根组建家庭)**: al inicio de cada combate ganás **Alzarse** (1 HP). *Ya tiene algo por lo que vivir.* |
| 11-12 | Solo con el Cáliz: +14 pts por nivel, +5 HP máx c/u (defaults). |

### Dupes / NP level
Sistema de Mash tal cual: botón "Invocar (dupe)", clave literal `OPTION_MORGAN_DUPE.name` en
`card_reward_ui.json`, guard de 2 alternativas; **+15%** vía `NpLevels.Scale` en las **5 cartas
NP** (#49, #50, #51 y las 2 ultis).

### La ulti a 100 — depende de la forma, y queda FIJA
Cruzar los 100 vestida de Reina da la ulti de daño; vestida de Bruja, la de supervivencia.
Cargar en Bruja es eficiente, pero si querés *Roadless Camelot* tenés que cruzar el umbral como
Reina: la danza de formas se vuelve una decisión de build del medidor.

## 10. Plan de assets (IDs verificados, 2026-06-11)

| Forma | Modelo | Charagraph | Bundle |
|---|---|---|---|
| Reina (Berserker) | **704020** (fallback 704010) | collectionNo **309** | `static.atlasacademy.io/JP/Servants/704020/` ✓ |
| Bruja de la Lluvia (Caster) | **505320** (fallback 505310) | collectionNo **385** | `static.atlasacademy.io/JP/Servants/505320/` ✓ |
| Reina del Invierno | **704030** (traje oficial) | costume 704030 | `static.atlasacademy.io/JP/Servants/704030/` ✓ |

- svtId base: Morgan **704000**, Aesc **505300**. La forma Caster es la servant separada "Aesc
  the Rain Witch"; "Aesc the Savior" no es jugable (solo lore).
- Pipeline de Mash tal cual (WORKFLOW-FGO.md §2-3); el export GUI de AssetStudio ×3 modelos es
  PASO MANUAL del usuario. Tres rigs → presupuestar ~1.5× el trabajo de Mash.
- Arte de cartas: `match-ce-art.js` (CEs de LB6/lluvia/invierno/caballeros hada abundan).
- Icono de la starter de mecánica: regla del workflow — **icono de clase Berserker dorado**
  (Morgan 5★), `Berserkergold.png` del fandom con `&format=original`.
- Manifest `id: "MorganBerserker"` — no cambiar nunca. `dependencies: ["BaseLib", "FGOCore"]`.

## 11. Orden de implementación y riesgos

1. FGOCore: `GutsPower` (generalizar `FouMiraclePower` — hook `ModifyHpLostAfterOstyLate` ya
   domado) + candado "Inutilizable: turno N" + manifestación por forma (handler `GaugeFilled`
   consulta la `FormPower` activa).
2. Scaffold MorganBerserker (plantilla MashShielder) + formas + Maldición (daño directo fin de
   turno, NO hooks ModifyHpLost; tope 15; verificar `IntangiblePower` del juego — confirmado
   en decompilado por el panel).
3. Pool → reliquias → vínculo/dupes → assets (3 exports manuales) → localización ×3 → pruebas.

**Perillas de playtest** (del panel):
- Grifos de NP apilables (Bruja +5 + Hada del País + Madness capeado + Impuesto +6): si la ulti
  sale cada <4 turnos en mazos dedicados, recortar primero el Impuesto (+6→+4).
- "Caster estacionaria": si la danza no aparece, condicionar la pasiva de Bruja ("si no atacaste
  este turno") antes que bajarla más.
- Con HP 76 y ~8 cartas de sangre, si el acto 1 castiga mucho, promover *Lágrimas de la
  Salvadora* a común.
- #64 Cernunnos + #45/#67: vigilar el DoT permanente en jefes largos (el tope de 15 es el freno).


## REDISEÑO v2 DE POOLS (2026-06-11, post-playtest, estilo JeanneAlter)

A pedido del usuario, ambos pools (Mash y Morgan) se rediseñaron con los idiomas de
diseño del mod JeanneAlter (decompilado y analizado en `assets/reference/jeanne_anatomy.json`;
auditoría previa en `pools_audit.json`; tablas completas y veredictos de jueces en
`assets/reference/redesign/*.json` — LA FUENTE DE VERDAD del pool actual es el CÓDIGO):

- **Estrellas de Crítico** (FGOCore `CritStarsPower`): contador compartido; a 100 se
  descuentan solas y dan 1 **Crítico Listo** (`CritReadyPower`: próximo Ataque ×2, un
  stack por carta). NO confundir con el contador chico de Artoria.
- **Buster/Arts/Quick** como básicas de comando en ambos mazos iniciales (números
  Jeanne: 10 daño / 6 + 30 NP / 6 + 30 estrellas).
- Comunes = engranajes de conversión (pares espejo NP↔estrellas a 0⚡, denominaciones
  fijas 10/20/30/50/100); conectividad 75%→97.6% (Mash) y 83%→97% (Morgan).
- Starters como motores de conversión de eventos→recursos con topes de 3 procs/turno:
  Mash (golpe totalmente bloqueado→estrellas, perder Vida→NP), Morgan (perder Vida→estrellas).
- Cartas excluidas del pool sin borrar (compat con runs guardadas): rarity→Event +
  comentario; borrar en la próxima versión.
