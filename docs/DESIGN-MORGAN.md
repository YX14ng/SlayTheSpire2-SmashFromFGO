# Diseño de personaje — Morgan (Berserker → Caster, Lostbelt 6)

> Mod de personaje para **Slay the Spire 2** (rama MAIN, v0.103.x) sobre **BaseLib ≥ 3.1.8** + **FGOCore**.
> Segundo personaje del proyecto (tras Mash). Convive con `MashShielder/` — NO pisa Baluarte, Intercepción ni Black Barrel.
>
> **Proveniencia**: diseñado con la skill [sts2-mechanics-design](../.claude/skills/sts2-mechanics-design/SKILL.md)
> (baselines extraídos de las 577 cartas del decompilado) mediante panel de 3 diseños independientes
> (fiel-a-FGO / roguelike-first / reina-tirana) evaluados por 3 jueces (balance numérico / experiencia
> de juego / lore+implementación). Base: **roguelike-first** (ganador unánime) + injertos puntuales
> de los otros dos. Assets verificados contra Atlas Academy el 2026-06-11.

## 0. Filosofía de balance

**Rota pero honesta (techo Watcher).** Morgan golpea por encima de la tasa estándar, pero todo
exceso está embudado por sus propias mecánicas:

- Lo más fuerte requiere **Maldición instalada en los enemigos** — y la Maldición **se evapora a
  mitades** cada turno: hay que sembrarla, mantenerla y elegir cómo cobrarla. Artefacto, limpiezas
  de debuffs y combates cortos apagan el plan A → Morgan baja a tasa normal, nunca por debajo.
- Lo más explosivo paga **HP propio** (es una Berserker: patrón vanilla Hemokinesis/Bloodletting/
  Offering, siempre `ValueProp.Unblockable|Unpowered`) o **Exhaust**.
- Las cartas NP consumen **TODA** la Carga (FGOCore): gastar a 70 es renunciar a la ulti de 100.
- La danza de formas cuesta cartas y tempo: la reliquia inicial paga solo el PRIMER cambio.

## 1. Identidad en una frase

**"La tirana que siembra maldiciones y la salvadora que las cosecha — dos reinados en un solo cuerpo."**

La pregunta que Morgan responde cada turno y un mazo de Ironclad no: **¿este turno SIEMBRO o COBRO?**

- En forma **Berserker** (la Reina) siembra **Maldición** (daño diferido que se evapora a mitades)
  y paga sus excesos con HP. Acumula. Sangra.
- En forma **Caster** (Aesc, la Bruja de la Lluvia) cobra el **Impuesto**: consume esas Maldiciones
  y las convierte en Carga NP y economía. Gasta. Se estabiliza.
- Cada punto de Maldición tiene **tres compradores**: dejarlo tickear (daño), cobrarlo en Caster
  (recurso) o detonarlo con cartas (tempo). El jugador elige uno por turno.
- El clímax (carta rara, permanente) es la **Reina del Invierno**: los dos reinados a la vez.

La oscilación no es "modo mejor": Berserker defiende peor (−1 de Bloqueo por carta) y quema HP;
Caster deja de sembrar (el Impuesto cobra en vacío si la Maldición ya se evaporó). El motor te
empuja a volver.

## 2. Resumen de lore (fuente del sabor — terminología CN oficial de Mooncell/scripts)

- **Reina del Invierno (冬之女王)**: tirana 2000 años de la Britania de las Hadas (妖精国不列颠).
  Porta **Rhongomyniad** (止境之枪·伦戈米尼亚德) como magia: dispara réplicas de la lanza.
- **Impuesto de Existencia (存在税)**: las hadas le pagan energía mágica una vez al año o mueren
  → el motor del personaje.
- **Aesc (CN oficial: 梣, el fresno), la Salvadora (救世妖精)** del Clan de la Lluvia (雨之氏族) de
  Orkney del Confín (止境的奥克尼): fundó la Mesa Redonda, construyó Londinium (伦蒂尼恩), fue
  traicionada y asesinada por las hadas que salvó, murió incontables veces y volvió del Confín
  (止境) convertida en invierno. En FGO es la servant separada **"Aesc the Rain Witch"** (雨之魔女梣).
- Skills reales mapeadas: Carisma del Anhelo (渴望的魅力), Protección del Lago (湖之加护), Desde el
  Confín del Mundo (来自止境), Hada del País de la Lluvia (雨之国的妖精), Carisma de la Adversidad
  (逆境的魅力), Último Recurso (最后的度假胜地 — el CN oficial conserva el chiste resort/recurso),
  Ojos Feéricos (妖精眼), Madness Enhancement (狂化).
- NPs reales: **Roadless Camelot** (业已无法抵达的理想乡 — Maldición 5t + "Overcharge +1") y
  **Memory of Londinium** (圣剑遥远梦之遗痕 — "las armas de los caballeros del pasado, presente y futuro").
- Reparto secundario: Caballeros Hada/Tam Lin (妖精骑士: 巴格斯特 / 梅柳齐娜 / 芭万·希), Spriggan el
  tesorero (斯普里根), los Mors (摩耳斯 — criaturas-maldición, NO asistentes), Cernunnos (科尔努诺斯),
  Albion (阿尔比恩), Clan del Espejo (镜之氏族), Oberon (奥伯龙), Habetrot (哈贝特洛特), Vivian (薇薇安).
- Memes CN: 想和摩根组建家庭 ("quiero formar un hogar con Morgan"), 笨蛋妈妈 (mamá boba de Baobhan Sith).

## 3. Stats base

| Atributo | Valor | Justificación contra baseline |
|---|---|---|
| HP máximo | **78** | Rango vanilla 66–80. No es tanque (Mash 85 intocada), pero usa HP como recurso → colchón del arquetipo gastador (Ironclad 80 es el precedente). 78 = casi tope, restado porque tiene Alzarse y curación condicional. |
| Energía | 3⚡ | Estándar universal. |
| Oro inicial | 99 | Estándar universal. |
| Color de cartas | Blanco glacial y azul noche, acentos carmesí | Paleta de Morgan; las cartas de Caster tintean a azul lluvia. |
| Carga NP | 0–300, ulti a 100 | FGOCore estándar (ya probado con Mash). |
| Forma inicial | **Berserker** | Requisito del encargo. |

**Reliquia inicial — Rhongomyniad, el Cetro de la Reina (止境之枪·伦戈米尼亚德(王笏))**:
*La primera vez que cambias de forma en cada combate: gana 1⚡ y roba 1 carta.*
Hace tempo-positivo el primer cambio sin regalar un motor infinito.

**Mazo inicial (10):** 4× Golpe (1⚡, 6) · 4× Defender (1⚡, 5) · 2 firmas (§5.0).
Checklist: gana el acto 1 sin motor — los Golpes + Azote de la Reina son daño a tasa; en Berserker
los Defender dan 4 (la primera lección: defender es del reinado de la lluvia).

## 4. Mecánicas y keywords

Presupuesto (§1.3 de la skill): **una familia original** (Maldición + Impuesto) + **FGOCore**
(Carga NP, Formas, Vínculo/dupes/Grial). El costo de HP no es mecánica nueva (patrón vanilla).

### Maldición (Curse / 诅咒) — keyword nueva, debuff de enemigo
*Al final del turno del enemigo: recibe daño igual a su Maldición (ignora Bloqueo); luego la
Maldición se reduce a la MITAD (redondeo hacia abajo).*

- Daño total si nadie la toca: pila 5 → 8; pila 8 → 15; pila 12 → 22. **Tasa de presupuesto:
  1 punto ≈ 1,7 de daño diferido** (el delay es el premium vs 9-10 inmediato por 1⚡).
- La evaporación a mitades es la urgencia del diseño: **cobrala o perdela** (la diferencia real
  contra Veneno, que premia ignorarlo).
- **REQUISITO de implementación (no negociable, condición de los 3 jueces)**: tooltip con el daño
  proyectado del próximo tick — precedente exacto en el decompilado:
  `PoisonPower.CalculateTotalDamageNextTurn()`. Plan B si las mitades no se leen en playtest:
  decaimiento plano −2 y recortar ~10 aplicadores (tabla en §10).
- Multijugador: ofensa personal → NO escala con jugadores.

### Impuesto (Tax / 存在税) — el verbo de cobro
*Impuesto X: consume hasta X de Maldición (del objetivo, o del enemigo con más si no se especifica).
Cada punto consumido produce el efecto indicado.*

- No es un recurso ni un contador nuevo: es un VERBO sobre la Maldición (cero UI extra).
- Cobrar 1 pt da ~2 NP o ~2 de daño inmediato renunciando a ~1,7 de daño diferido: el loop nunca
  se rompe (§3 de la skill se cumple por construcción — sin doble-dip).

### Carga NP (FGOCore, tal cual)
- 0–300, ulti al cruzar 100 (`GaugeFilled`), re-arme bajo 100, cartas NP con mínimo que consumen
  TODO, Sobrecarga lineal +X por cada 10 sobre el mínimo (evaluada antes de pagar).
- Mínimos del personaje: Lluvia de Rhongomyniad 40 · Memory of Londinium 50 · Roadless Camelot 70 ·
  Lanza del Confín 100 · ultis manifestadas 100. (Escala espejo de Mash.)
- **La ulti manifestada depende de la forma activa** y **se transforma al cambiar de forma con
  ella en mano** (`IFormChangeListener` la intercambia — mismo slot; nunca perdés la ulti por
  oscilar; la decisión es cuándo y en qué forma JUGARLA). Nota de implementación: las ultis son
  0⚡/Exhaust/sin upgrade → el swap es generar-y-reemplazar sin estado que migrar; si diera pelea,
  fallback aprobado por el panel: fijarla al manifestarse (decisión "¿en qué forma cruzo los 100?").
- **Bendición de Rhongomyniad** (estado generado — el "Overcharge +1" del NP real): *tu próxima
  carta NP suma +10 a su Sobrecarga.* La otorgan Roadless Camelot a 100+ y la ulti Desatada.

### HP como recurso (lado Berserker)
Baselines en TODAS las cartas con sangre: 1 HP ≈ rider chico · 2 HP ≈ +5-6 · 3 HP ≈ 2⚡ ·
6 HP ≈ 2⚡ + 3 robos con Exhaust. La HP perdida es también combustible de payoffs
(Carisma de la Adversidad, Venganza de la Traicionada, Sangre de la Reina).

### Reglas globales de legibilidad
1. **El cambio de forma se resuelve ANTES que el resto de la carta** (el Bloqueo del toggle ya
   recibe el ±1 de la forma nueva).
2. Cartas duales: máximo UNA cláusula condicional de forma por carta.
3. La Maldición se ve como debuff en el enemigo con tooltip de daño proyectado.

## 5. Formas (FGOCore: `FormPower` + `FormSwitch` + `FormVisuals`)

| Forma | Modelo 2D | Pasivas |
|---|---|---|
| **Berserker — La Tirana de Britania** (狂战士·妖精国的暴君) — inicial | **704020** | (a) Cuando juegas un Ataque: aplica **1 de Maldición** al primer enemigo golpeado. (b) Tus cartas otorgan **1 menos de Bloqueo** (狂化). |
| **Caster — Aesc, la Bruja de la Lluvia** (雨之魔女梣) | **505320** | (a) **Cobro del Impuesto**: al inicio de tu turno, Impuesto 5 (del enemigo con más Maldición): **+2 de Carga NP por punto** (máx. +10/turno). (b) Tus cartas otorgan **+1 de Bloqueo**. |
| **Reina del Invierno** (冬之女王) — permanente (`IsPermanent`), clímax vía carta rara | **704030** (traje oficial) | Las pasivas (a) de AMBAS formas (siembra al atacar + cobro al inicio de turno), sin modificador de Bloqueo, **más composición: al final de tu turno, los enemigos Malditos ganan +2 de Maldición**. La ulti manifestada es la de Berserker. |

- La composición de la Reina del Invierno (injerto del panel) converge sola: pila p → p/2+2 →
  punto fijo ≈ 4 de daño/turno/enemigo — el clímax convierte el motor de oscilación en motor de
  ejecución sin que las cartas de siembra dejen de pagar. Las 3 cartas que premian cambiar quedan
  muertas tras coronarte: costo de oportunidad aceptado, igual que el Paladín de Mash.
- Cambios esperados por combate: 2–4. Pasivas en banda 3-5/turno (§5 de la skill) ✓.
- Implementación: `FramesPath` por forma (swap de modelo con precarga en hilos), `FormShiftedPower`
  como marcador, `IFormChangeListener` para la transformación de ulti y los powers de cambio.

## 6. Pool de cartas

### 6.0 Firmas (en el mazo inicial)

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| S1 | Queen's Scourge / **Azote de la Reina** / 女王的鞭笞 | Ataque, 1⚡ | 6 de daño. Aplica 2 de Maldición. | +: 8 de daño, 3 de Maldición |
| S2 | Rain and Winter / **Lluvia e Invierno** / 雨与冬 | Habilidad, 1⚡ | **Cambia de forma** (toggle; se resuelve primero). Gana 5 de Bloqueo. | +: 8 de Bloqueo y roba 1 |

S1: 6 + 2pts(≈3,4) ≈ 9,4 a 1⚡ = banda común exacta. S2: el mazo inicial ya oscila; con la starter
el primer uso es tempo-positivo.

### 6.1 Comunes (20) — ~9 pan y manteca / ~8 motor / ~3 payoff

**Ataques (10)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 1 | Lance Replica / **Réplica de la Lanza** / 长枪的复制品 | Ataque, 1⚡ | 9 de daño. | +: 12 |
| 2 | Frost Lash / **Azote Gélido** / 寒霜鞭笞 | Ataque, 1⚡ | 6 de daño. Aplica 2 de Maldición. | +: 8 y 3 |
| 3 | Execution Decree / **Decreto de Ejecución** / 处刑敕令 | Ataque, 2⚡ | 18 de daño. Pierde 3 HP. | +: 24 de daño |
| 4 | Storm of Lances / **Tormenta de Lanzas** / 枪之风暴 | Ataque, 1⚡ | 4 de daño a TODOS. 1 de Maldición a TODOS. | +: 6 y 2 a TODOS |
| 5 | Scepter Blow / **Golpe del Cetro** / 权杖重击 | Ataque, 2⚡ | 14 de daño. Gana 4 de Bloqueo. | +: 18 y 6 |
| 6 | Call the Mors / **Llamado de los Mors** / 摩耳斯之唤 | Ataque, 1⚡ | 5 de daño. **Impuesto 3**: +2 de daño por punto. | +: 7, Impuesto 4 |
| 7 | Queen's Wrath / **Cólera de la Reina** / 女王之怒 | Ataque, 0⚡ | Pierde 2 HP: 8 de daño. | +: 11 |
| 8 | Tyrant's Charge / **Embestida Tiránica** / 暴君冲撞 | Ataque, 1⚡ | 8 de daño. En Berserker: aplica 2 de Maldición. | +: 11 y 3 |
| 9 | Cutting Rain / **Lluvia Cortante** / 斩裂之雨 | Ataque, 1⚡ | 3 de daño ×3. En Caster: gana 3 de Bloqueo. | +: 4×3, gana 4 |
| 10 | Disdain / **Desdén** / 蔑视 | Ataque, 1⚡ | 7 de daño. Aplica 1 de Débil. | +: 10 y 2 |

**Habilidades (10)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 11 | Rime Guard / **Guardia de Escarcha** / 霜之守势 | Habilidad, 1⚡ | 8 de Bloqueo. | +: 11 |
| 12 | Rain Veil / **Velo de Lluvia** / 雨之帷幕 | Habilidad, 1⚡ | 5 de Bloqueo. Carga NP +10. | +: 7 y +15 |
| 13 | Royal Edict / **Edicto Real** / 王敕 | Habilidad, 0⚡ | Aplica 2 de Maldición. | +: 3 |
| 14 | Tax Collection / **Recaudación** / 征税 | Habilidad, 1⚡ | **Impuesto 4**: gana 2 de Carga NP y 1 de Bloqueo por punto. | +: Impuesto 6 |
| 15 | Fairy Eyes / **Ojos Feéricos** / 妖精眼 | Habilidad, 1⚡ | Roba 2. | +: roba 2, Carga NP +10 |
| 16 | Territory Creation / **Creación de Territorio** / 阵地作成 | Habilidad, 1⚡ | 6 de Bloqueo. En Caster: roba 1. | +: 9 de Bloqueo |
| 17 | Everfrost / **Escarcha Perenne** / 永冬之霜 | Habilidad, 2⚡ | 13 de Bloqueo. Aplica 2 de Maldición. | +: 16 y 3 |
| 18 | Blood Tribute / **Tributo de Sangre** / 血之贡品 | Habilidad, 0⚡ | Pierde 2 HP: Carga NP +15. | +: +22 |
| 19 | Tyrant's Gaze / **Mirada de la Tirana** / 妖妃的凝视 | Habilidad, 1⚡ | Aplica 2 de Débil y 1 de Maldición. | +: 3 y 2 |
| 20 | Turn of Seasons / **Cambio de Estación** / 季节更替 | Habilidad, 0⚡ | **Cambia de forma**. Carga NP +5. | +: +10 y roba 1 |

### 6.2 Poco comunes (29) — 12 ataques / 12 habilidades / 5 poderes

**Ataques (12)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 21 | Scattered Replicas / **Réplicas Dispersas** / 离散的复制长枪 | Ataque, 2⚡ | 7 de daño ×2. Carga NP +10. | +: 9×2, +15 |
| 22 | Mors Harvest / **Cosecha de Mors** / 摩耳斯的收割 | Ataque, 1⚡ | 8 de daño. Si el objetivo tiene Maldición: roba 1 y Carga NP +5. | +: 11 |
| 23 | Lance of Judgment / **Lanza del Juicio** / 裁决之枪 | Ataque, 3⚡ | 24 de daño. Aplica 4 de Maldición. | +: 30 y 5 |
| 24 | Chill of the End / **Frío del Confín** / 止境之寒 | Ataque, 1⚡ | 11 de daño. Solo jugable en Berserker o Reina del Invierno. | +: 15 |
| 25 | Rod of Torrents / **Vara de los Torrentes** / 激流之杖 | Ataque, 1⚡ | 8 de daño. En Caster: Carga NP +10 y gana 3 de Bloqueo. | +: 10, +15 y 5 |
| 26 | Make an Example / **Castigo Ejemplar** / 杀一儆百 | Ataque, 2⚡ | 16 de daño. Si mata al objetivo: 4 de Maldición a TODOS los demás. | +: 20 y 6 |
| 27 | Silver Avalanche / **Avalancha de Plata** / 银色雪崩 | Ataque, 2⚡ | 12 de daño a TODOS. Pierde 4 HP. | +: 16 a TODOS |
| 28 | Barghest's Fang / **Colmillo de Barghest** / 巴格斯特之牙 | Ataque, 2⚡ | 13 de daño. Cura HP igual a la Maldición del objetivo (máx. 5). | +: 17, máx. 7 |
| 29 | Baobhan Sith's Song / **Canción de Baobhan Sith** / 芭万·希之歌 | Ataque, 1⚡ | 5 de daño ×2. Si el objetivo tiene Maldición: +2 por golpe. | +: 6×2, +3 |
| 30 | Melusine's Flash / **Destello de Melusine** / 梅柳齐娜的闪光 | Ataque, 0⚡ | 7 de daño. Exhaust. | +: 10 y Carga NP +5 |
| 31 | Albion's Roar / **Rugido de Albion** / 阿尔比恩的咆哮 | Ataque, 3⚡ | 30 de daño. Pierde 4 HP. | +: 38 |
| 32 | Rain of Rhongomyniad / **Lluvia de Rhongomyniad** / 伦戈米尼亚德之雨 | **Ataque NP**, 1⚡ (mín. Carga 40, consume TODA) | 16 de daño. **Sobrecarga**: +4 por cada 10 sobre 40. | +: 20 base, +5/10 |

#32 (injerto del panel): la mini-NP spameable — el SUELO de la economía del cobro. Garantiza que
convertir Maldición→NP en Caster siempre tenga salida aunque no robes las NP raras.

**Habilidades (12)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 33 | Winter's Fury / **Furia del Invierno** / 凛冬之怒 | Habilidad, 1⚡ | Entra en forma **Berserker**. Aplica 3 de Maldición. | +: 5 |
| 34 | Memory of the Savior / **Memoria de la Salvadora** / 救世主的记忆 | Habilidad, 1⚡ | Entra en forma **Caster**. Gana 8 de Bloqueo. | +: 12 |
| 35 | Existence Tax / **Impuesto de Existencia** / 存在税 | Habilidad, 1⚡ | **Impuesto 6**: gana 1 de Bloqueo y 2 de Carga NP por punto. Roba 1. | +: Impuesto 9 |
| 36 | Habetrot's Blessing / **Bendición de Habetrot** / 哈贝特洛特的祝福 | Habilidad, 1⚡ | Cura 4 HP. En Caster: cura 7. Exhaust. | +: 6 / 10 |
| 37 | Mirror Clan's Glass / **Espejo del Clan** / 镜之氏族的魔镜 | Habilidad, 1⚡ | Roba 2. Si cambiaste de forma este turno: roba 3. | +: coste 0⚡ |
| 38 | Spriggan's Hoard / **Tesoro de Spriggan** / 斯普里根的宝库 | Habilidad, 1⚡ | 8 de Bloqueo. **Impuesto 2**: +2 de Bloqueo por punto. | +: 10, Impuesto 3 |
| 39 | Rain of Orkney / **Lluvia de Orkney** / 奥克尼之雨 | Habilidad, 2⚡ | 13 de Bloqueo. Carga NP +10. | +: 17, +15 |
| 40 | Savior's Sacrifice / **Sacrificio de la Salvadora** / 救世主的牺牲 | Habilidad, 0⚡ | Pierde 3 HP: gana 2⚡. Exhaust. | +: pierde 2 HP |
| 41 | Beneath the World-Ash / **Bajo el Fresno** / 梣树之下 | Habilidad, 1⚡ | Carga NP +15. En Caster: +25. | +: +20 / +30 |
| 42 | Winter Decree / **Decreto de Invierno** / 凛冬敕令 | Habilidad, 1⚡ | Aplica 4 de Maldición. | +: 6 |
| 43 | Mist over Norwich / **Niebla de Norwich** / 诺里奇之雾 | Habilidad, 1⚡ | Aplica 2 de Maldición a TODOS. | +: 3 |
| 44 | Pact of the Lake / **Pacto del Lago** / 湖之契约 | Habilidad, 1⚡ | 11 de Bloqueo. Solo jugable en Caster o Reina del Invierno. | +: 14 |

**Poderes (5)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 45 | Mad Enhancement / **Locura Realzada** / 狂化 | Poder, 1⚡ | Tus Ataques aplican +1 de Maldición. | +: coste 0⚡ |
| 46 | Charisma of Adversity / **Carisma de la Adversidad** / 逆境的魅力 | Poder, 2⚡ | Gana 1 de Fuerza. La primera vez que tu HP caiga bajo 75% / 50% / 25%: gana 2 de Fuerza. | +: 3 por umbral |
| 47 | Fairy of the Rainy Land / **Hada del País de la Lluvia** / 雨之国的妖精 | Poder, 1⚡ | Al inicio de tu turno: Carga NP +8. | +: +12 |
| 48 | Faerie Court / **Corte de las Hadas** / 妖精的宫廷 | Poder, 1⚡ | Al inicio de cada turno: aplica 1 de Maldición a TODOS. | +: 2 |
| 49 | Item Construction (EX) / **Construcción de Ítems** / 道具作成(EX) | Poder, 1⚡ | Cada vez que cambias de forma: gana 4 de Bloqueo y Carga NP +5. | +: 6 y +8 |

(#48 corregido por el panel: a 2⚡ con 1/turno era carta trampa — baja a 1⚡.)

### 6.3 Raras (20) — 6 ataques / 9 habilidades / 5 poderes

**Ataques (6)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 50 | **ROADLESS CAMELOT** / 业已无法抵达的理想乡 | Ataque NP, 2⚡ (mín. 70, consume TODA) | 28 de daño a TODOS. 4 de Maldición a TODOS. **Sobrecarga**: +3 de daño a todos por cada 10 sobre 70. Si consumiste 100+: gana **Bendición de Rhongomyniad**. | +: 36 base, 5 de Maldición |
| 51 | Spear That Marks the End / **Lanza del Confín** / 止境之枪·伦戈米尼亚德 | Ataque NP, 3⚡ (mín. 100, consume TODA) | 50 de daño. Si mata: gana 30 de Carga NP. **Sobrecarga**: +5 por cada 10 sobre 100. Exhaust. | +: 62 base |
| 52 | Final Collection / **Cobro Final** / 最后的清算 | Ataque, 1⚡ | **Impuesto TOTAL** (toda la Maldición del objetivo): 2 de daño por punto. | +: 3 por punto |
| 53 | Rend of Cernunnos / **Desgarro de Cernunnos** / 科尔努诺斯之撕裂 | Ataque, 3⚡ | 20 de daño a TODOS. Pierde 6 HP. | +: 26 a TODOS |
| 54 | Rain of Holy Swords / **Lluvia de Espadas Sagradas** / 圣剑之雨 | Ataque, 2⚡ | 5 de daño ×4 (aleatorio). En Caster: +2 por golpe. | +: 6×4, +3 |
| 55 | Vengeance of the Betrayed / **Venganza de la Traicionada** / 背叛者的复仇 | Ataque, 1⚡ | 6 de daño, +1 por cada 2 de HP perdida este combate (máx. +12). | +: 8 base, máx. +18 |

**Habilidades (9)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 56 | **MEMORY OF LONDINIUM** / 圣剑遥远梦之遗痕 | Habilidad NP, 2⚡ (mín. 50, consume TODA) | Gana 20 de Bloqueo. Añade 2 **Caballeros Hada** aleatorios a tu mano. **Sobrecarga**: +3 de Bloqueo por cada 10 sobre 50. | +: 26 de Bloqueo, 3 Caballeros |
| 57 | Last Resort / **Último Recurso** / 最后的度假胜地 | Habilidad, 0⚡ | **Solo jugable a partir del turno 5.** Gana 2⚡ y Carga NP +50. Exhaust. | +: a partir del turno 4 |
| 58 | Throne at World's End / **El Trono del Confín** / 止境的王座 | Habilidad, 2⚡ | Entra en forma **Reina del Invierno** (permanente). Aplica 3 de Maldición a TODOS. Exhaust. | +: coste 1⚡, 5 de Maldición |
| 59 | From the World's End / **Desde el Confín del Mundo** / 来自止境 | Habilidad, 1⚡ | Este combate, la primera vez que fueras a morir: quedas a 1 HP y ganas 3 de Fuerza. Exhaust. | +: quedas con 10 HP |
| 60 | Charisma of Yearning / **Carisma del Anhelo** / 渴望的魅力 | Habilidad, 1⚡ | Gana 2 de Fuerza. Carga NP +10. Exhaust. | +: 3 de Fuerza, +15 |
| 61 | Protection of the Lake / **Protección del Lago** / 湖之加护 | Habilidad, 1⚡ | Carga NP +30. Exhaust. | +: +40 |
| 62 | Oberon's Dream / **Sueño de Oberon** / 奥伯龙之梦 | Habilidad, 0⚡ | Roba 3. Al final de este turno: pierde 4 HP. | +: roba 4, pierde 3 |
| 63 | The 10,000-Year Plan / **El Plan de Diez Mil Años** / 万年大计 | Habilidad, 2⚡ | Duplica la Maldición de un enemigo (máx. +15). Exhaust. | +: máx. +25 |
| 64 | Savior's Veil / **Velo de la Salvadora** / 救世主的面纱 | Habilidad, 1⚡ | Gana **Intangible 1**. Ethereal. Exhaust. | +: sin Ethereal |

(#57 nerfeado por el panel: pierde el "roba 2" — el mega-turno determinista de turno 5 excedía el
techo del marker. #60/#46/#59/#57/#61: nombres zhs corregidos a la terminología CN oficial.)

**Poderes (5)**

| # | Carta (eng / es / zhs) | Tipo, Coste | Efecto | Mejora |
|---|---|---|---|---|
| 65 | Fairy Queen of Britain / **Reina de las Hadas de Britania** / 妖精国不列颠的女王 | Poder, 3⚡ | Al inicio de tu turno: 2 de Maldición a TODOS y gana 2 de Carga NP por enemigo con Maldición. | +: 3 de Maldición |
| 66 | Nesting Mors / **Nido de Mors** / 摩耳斯之巢 | Poder, 2⚡ | Cuando un enemigo muere con Maldición: su Maldición pasa a un enemigo aleatorio y ganas 8 de Carga NP. | +: además 4 de Bloqueo |
| 67 | Perpetual Winter / **Invierno Perpetuo** / 永冬 | Poder, 2⚡ | Al final de tu turno: gana 3 de Bloqueo por cada enemigo con Maldición. | +: 4 |
| 68 | Tyrant and Savior / **Tirana y Salvadora** / 暴君与救世主 | Poder, 2⚡ | Cada vez que cambias de forma: roba 1 y gana 1 de Fuerza. | +: coste 1⚡ |
| 69 | Queen's Blood / **Sangre de la Reina** / 女王之血 | Poder, 1⚡ | Cuando pierdas HP en tu turno: gana 2 de Carga NP por punto (máx. +12/turno). | +: máx. +20 |

### 6.4 Especiales generadas

**Ultis manifestadas a 100** (la de la forma activa; se transforma al cambiar de forma):

| Carta (eng / es / zhs) | Tipo, Coste | Efecto |
|---|---|---|
| **ROADLESS CAMELOT: Desatado** / 业已无法抵达的理想乡·全解放 *(Berserker y Reina del Invierno)* | Ataque NP, 0⚡ (mín. 100, consume TODA), Exhaust | 22 de daño a TODOS. 4 de Maldición a TODOS. Gana **Bendición de Rhongomyniad**. **Sobrecarga**: +2 de daño a todos por cada 10 sobre 100 (a 300: 62 a todos). |
| **MEMORY OF LONDINIUM: Desatado** / 圣剑遥远梦之遗痕·全解放 *(Caster)* | Habilidad NP, 0⚡ (mín. 100, consume TODA), Exhaust | Gana 18 de Bloqueo. Añade 3 **Caballeros Hada** aleatorios. Cura 4 HP. **Sobrecarga**: +2 de Bloqueo por cada 10; a 200+: +1 Caballero. |

**Tokens — Caballeros Hada (妖精骑士 / Tam Lin)**, 0⚡, Exhaust, presupuesto ≈ Backstab (11-14):

| Token (eng / es / zhs) | Efecto |
|---|---|
| **Barghest** / 妖精骑士高文·巴格斯特 | Ataque: 14 de daño. Pierdes 2 HP (algo se comió por el camino). |
| **Melusine** / 妖精骑士兰斯洛特·梅柳齐娜 | Ataque: 4 de daño ×3. |
| **Baobhan Sith** / 妖精骑士崔斯坦·芭万·希 | Ataque: 7 de daño. Aplica 3 de Maldición. |

(Generación aleatoria entre los 3 — la UI "elige 1" no está verificada en BaseLib; si algún día
se verifica, Convocatoria elige.)

**Estado generado**: Bendición de Rhongomyniad / 伦戈米尼亚德的祝福 — *tu próxima carta NP suma
+10 a su Sobrecarga.*

**Memes propios del pool** (los incoloros de FGOCore ya existen, no se tocan):

| Carta | Rareza | Efecto |
|---|---|---|
| I Want to Start a Family with Morgan / **Quiero Formar un Hogar con Morgan** / 想和摩根组建家庭 | Poco común | Habilidad, 1⚡: cura 4 HP y gana 4 de Bloqueo. Exhaust. *(+: 6 y 6)* |
| Silly Mama / **Mamá Boba** / 笨蛋妈妈 | Común | Habilidad, 0⚡: roba 1. Carga NP +5. *(+: roba 1, +10, 2 de Bloqueo)* |

## 7. Reliquias del personaje

| Reliquia (eng / es / zhs) | Tier | Efecto |
|---|---|---|
| Rhongomyniad, the Queen's Scepter / **el Cetro de la Reina** / 止境之枪(王笏) | **Inicial** | La primera vez que cambias de forma en cada combate: gana 1⚡ y roba 1. |
| Coronation at World's End / **Coronación del Confín** / 止境的加冕 | Jefe (reemplaza la inicial) | Cada vez que cambias de forma: gana 1⚡ (máx. 1 vez por turno). |
| Remains of Cernunnos / **Vestigio de Cernunnos** / 科尔努诺斯的遗骸 | Jefe/Raro | La Maldición ya no se reduce a la mitad: se reduce en 2 después de dañar. |
| Spriggan's Ledger / **Libro de Cuentas de Spriggan** / 斯普里根的账簿 | Común | Al final de cada combate en el que aplicaste 10+ de Maldición: gana 12 de oro. |
| Mirror Clan's Glass / **Espejo del Clan** / 镜之氏族的魔镜 | Tienda | Empiezas cada combate con 20 de Carga NP. |
| Seed of the World-Ash / **Semilla del Fresno** / 梣树之种 | Evento | Al subir de piso: cura 2 HP. |

(Coronación nerfeada por el panel: pierde el "roba 1" — con el toggle 0⚡ era motor casi gratis.)

Reliquias de sistema (patrón Mash, FGOCore): **Boleto de Invocación** (呼符, `INpLevelStore`,
contador NP1-5, botón "Invocar (dupe)" — clave literal `OPTION_MORGAN_DUPE.name` en
`card_reward_ui.json`, guard de máx. 2 alternativas), **Vínculo de Morgan** (`BondRelic`),
**Santo Grial** (`ILimitBreaker`, ya existe en el pool de Mash — para Morgan se agrega a su pool).

## 8. Vínculo, dupes y la ulti

### Vínculo (subclase de `BondRelic` — los DEFAULTS probados + capstone propio)
Tema: las hadas (y Chaldea) aprenden a querer a la Reina. **Usa la espina dorsal numérica
default de FGOCore** (HP en Nv1/3/6/9 = +3/3/4/5 con escalado multijugador; NP inicial 5/10/15
en Nv2/5/8; Bloqueo inicial 3/6 en Nv4/7) — cero código nuevo — y SOLO overridea el capstone:

- **Nv10 — "La Corte paga el impuesto" (宫廷纳税)**: al inicio de cada combate, aplica 2 de
  Maldición a TODOS los enemigos y gana 10 de Carga NP.

### Dupes / NP level (sistema de Mash, tal cual)
Botón "Invocar (dupe)" en recompensas de carta (50% +25% pity); cada nivel: **+15%** vía
`NpLevels.Scale` en las **6 cartas NP**: #32, #50, #51, #56 y las 2 ultis manifestadas.

### Santo Grial
El de Mash (`ILimitBreaker`): Vínculo hasta Nv12, NP level hasta 6. Se añade al pool de reliquias
de Morgan con la misma carta de presentación.

## 9. Plan de assets (IDs verificados contra Atlas Academy, 2026-06-11)

| Forma | Modelo de batalla | Charagraph | Bundle |
|---|---|---|---|
| Berserker — La Tirana (inicial) | **704020** (asc 3-4; fallback 704010) | collectionNo **309** | `static.atlasacademy.io/JP/Servants/704020/` ✓ |
| Caster — Aesc | **505320** (asc final; fallback 505310) | collectionNo **385** | `static.atlasacademy.io/JP/Servants/505320/` ✓ |
| Reina del Invierno | **704030** (traje oficial) | costume 704030 | `static.atlasacademy.io/JP/Servants/704030/` ✓ |

- svtId base: Morgan **704000**, Aesc **505300**. "Aesc the Savior" NO es jugable (solo lore).
- Pipeline: el de Mash tal cual (WORKFLOW-FGO.md §2-3): export GUI de AssetStudio por modelo
  (PASO MANUAL del usuario, ×3 modelos), render 2-pasadas con crop común, WebP 2×, `.tres` por
  forma, patch de `.import`. Presupuestar ~1.5× el trabajo de Mash (tres rigs).
- Arte de cartas: `match-ce-art.js` contra CEs oficiales (los nombres del pool son matcheables:
  LB6 y los eventos de Morgan/Aesc tienen CEs de lluvia/invierno/fresno/espejo/corona/caballeros).
- Iconos de reliquias: regla del workflow — la starter de mecánica usa el **icono de clase
  Berserker** en la variante de rareza (Morgan 5★ = oro, `Berserkergold.png` del wiki con
  `&format=original`).
- Manifest `id: "MorganBerserker"` — no cambiar nunca. `dependencies: ["BaseLib", "FGOCore"]`.

## 10. Orden de implementación y riesgos abiertos

1. Scaffold (copiar MashShielder como plantilla, §4.5 del workflow) + formas + Maldición/Impuesto
   (con el **tooltip de daño proyectado** desde el día 1 — precedente `PoisonPower`).
2. Pool por rarezas (básicas→comunes→…), reliquias, vínculo, dupes.
3. Assets: modelos (3 exports manuales del usuario) → render → arte de cartas → iconos.
4. Localización trilingüe + pruebas.

**Perillas que MÁS necesitan playtest** (identificadas por el panel):
- Tasa de cobro del Impuesto pasivo de Caster (2 NP/pt, tope 5/turno) — si "nunca cambies" domina,
  subir a tope 6-7/turno antes que tocar la tasa.
- La banda de la mini-NP #32 (16 base / +4 por 10) — el suelo de la economía.
- Si la Maldición a mitades no se LEE en playtest: plan B = decaimiento plano −2, recortando
  aplicadores: S1/#2 → 2→2 (igual), #13 → 2, #23 → 4→3, #42 → 4→3, #43 → 2→1 a TODOS, #63 máx.
  +15→+10, ulti 4→3 a TODOS (la pila vale ~n²/4 en vez de ~1,7n).
- La transformación de ulti en mano: si la implementación da pelea, fallback = fijarla al
  manifestarse (aprobado por el panel).
- Último Recurso (#57) es binaria por diseño (muerta en combates <5 turnos) — fiel a FGO;
  si frustra, bajar a turno 4 base y mejorar otro eje.
