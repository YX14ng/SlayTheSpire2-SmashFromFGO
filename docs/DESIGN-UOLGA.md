# DESIGN-UOLGA — U-Olga Marie (UnBeast, FGO) para StS2

**Estado: acta de identidad CERRADA, pool PENDIENTE.** Las decisiones de §1-§8 salieron de un
interrogatorio de diseño con Fable 5 (2026-08-23, 22 preguntas de frontera; el usuario contestó
todas). Lo que falta —pool de ~68 recompensas, 12 reliquias, mazo inicial, números finos— es
trabajo del panel §4.6.7 de [`WORKFLOW-FGO.md`](WORKFLOW-FGO.md): propuestas con lentes distintas +
jueces adversariales, **los parches del juez mandan**. NO implementada.

Mod: `UOlgaMarieBeast` · `res://UOlgaMarieBeast/` · depende de BaseLib, RitsuLib y FGOCore.

## 0. Canon verificado (Atlas Academy `4000100` / collectionNo 444 + fgo.wiki)

| | JP | EN | 简体中文 |
|---|---|---|---|
| Personaje | Ｕ－オルガマリー | U-Olga Marie | U－奥尔加玛丽 |
| Clase | ビースト（unBeastOlgaMarie） | UnBeast | 兽 |
| NP | すでに過ぎし人理の終／プラネット・オルガマリー | Planet Olga Marie | 既已过去的人理之终 |
| H1 (F1-2) | 空前絶後 EX | Unparalleled EX | 空前绝后 EX |
| H2 (F1-2) | 驚天動地 B | Earth-Shattering B | 惊天动地 B |
| H3 (F1-2) | 天衣無縫 EX | Flawlessness EX | 天衣无缝 EX |
| H1 (F3) | ウルトラマニフェスト EX | Ultra Manifest EX | 究极宣言 EX |
| H2 (F3) | アトミックプラント B | Atomic Plant B | 核能设施 B |
| H3 (F3) | アルテミット・Ｕ EX | Ultimate U EX | 终极U EX |

- **Mazo canónico: QAABB** (`cards: [Quick, Arts, Arts, Buster, Buster]`) — coincide exacto con el
  estándar §4.6.1 del proyecto. NP = **Buster**, 5 hits, 対界宝具 (anti-World), AoE.
- `starGen 98`, peso de crítico 99, resistencia a muerte instantánea 18,3%; recibir daño → 3% NP.
- Pasivas: 陣地作成 A · 道具作成 B · 神性 EX · **単独顕現 −** (declarada PERDIDA en el accidente de
  invocación) · **人理の防人 EX** · **逆光 EX**.
- ⚠️ **Corrección de canon sobre el resumen del wiki chino**: el Guts de 人理の防人 se dispara
  «〔人類の脅威〕特性の敵に**やられた時**» — cuando una Amenaza te MATA, no cuando vos la matás.
  Además su Extra Attack pasa a AoE **con el daño por objetivo a la mitad** (salvo enemigo único).
- Assets oficiales: 4 charaGraph de ascensión + 1 costume, 5 spriteModels, command cards
  `card_servant_1/2/3.png` (una por ascensión). Producción viable para las tres formas.

## 1. Identidad en una frase

**La Directora que decreta.** Su medidor NP no es un botón de daño: es **presupuesto**. Cada vez que
llega a 100 elige entre **gastarlo** (Planet Olga Marie, el AoE) o **convertirlo en Autoridad** —
una ráfaga de ataques extra que ejecuta ella misma en los turnos siguientes. Y como la Bestia que
desprecia a la humanidad pero muere por defenderla, su única transformación no se compra: llega
cuando una Amenaza la mata y ella se levanta como 人理の防人.

**Eje propio (distinción del roster):** ÚNICO personaje cuyo medidor NP compite consigo mismo.
Mash=muralla, Morgan=Buster/maldición, Artoria=soporte crit, Tiamat=enjambre, Oberon=deuda,
Gilgamesh=arsenal comprado con Oro, Kagetora=ascensión perseguida. U-Olga = **presupuesto y decreto**.

## 2. Motor central — Autoridad (mecánica grande 1)

Fusiona dos piezas canónicas que en FGO ya son la misma (H3 consume el NP entero y entre lo que
otorga está Extra Attack +50-100%).

- **Conversión**: carta `CardRarity.Event` que pasa por `ConsumeAllForNpCard` (el único embudo de
  todas las cartas-NP) y **no hace daño**. Consume el medidor entero.
- **Tasa**: **1 carga de Autoridad por cada 50 de tier consumido** (a 100 = 2, a 300 = 6), **cap 5**.
- **Entrega**: los tokens llegan YA a la mano; **máximo 1 token por turno**; reloj de **5 turnos**,
  después se agotan solos. Pico total auditable por construcción: 5 extras en 5 turnos.
- **Token único** (el decreto), no drafteable, 0⚡, **escala con el tier consumido**, arte oficial de
  command card por ascensión. **En Forma 3 es AoE** (canon; con el daño por objetivo reducido).
- **Candados anti-loop, no negociables**: el token **no genera NP**, **no dispara riders de "jugaste
  un Ataque"** (`IsFirstInSeries`, patrón EchoForm) y no re-dispara `CommandBonusPower`.
- **Diferenciación de Gilgamesh** (verificada contra su implementación): Gil manifiesta **8 armas
  distintas al azar** desde cartas generadoras pagas, con riders de tribu por conteo. U-Olga tiene
  **UN token que sólo nace de convertir el medidor** y cuya única variable es la **magnitud** del
  tier pagado. Arsenal coleccionado vs decreto repetido.

## 3. Guts y Forma 3 (mecánica grande 2)

- **F1-F2 = ascensiones cosméticas** (`FormVisuals` sin `FormPower`, precedente Gilgamesh §3.5).
  **Una sola transformación mecánica**: la Forma 3.
- **Puerta única: el Guts de 人理の防人.** En los combates con **Amenaza para la Humanidad** el Guts
  está armado; si una Amenaza la mata, se levanta como Protectora → **Forma 3, irreversible**.
- **逆光 al levantarse**: Buster arriba, invulnerabilidad 1 vez (reutilizar el Anti-Purga registrado
  de Artoria, no reinventar), algo de NP.
- **Precio de la transformación**: **−MaxHP permanente (~10)** + **pierde los buffs de Mal
  acumulados**. ⚠️ El motor NO tiene pérdida de MaxHP temporal (`LoseMaxHp` es permanente,
  precedente `PaperCutsPower`); emular la expiración tiene bordes filosos y se descartó.
- La decisión del jugador es **si se expone** para armar el Guts o si juega seguro en el kit
  egoísta de Mal, que debe seguir siendo competitivo.

## 4. Amenaza para la Humanidad (mecánica chica) — un concepto, tres consumidores

Marcador **visible** (power con icono y tooltip) aplicado a los enemigos que califican en
`BeforeCombatStartLate`. Alimenta el Guts (§3), el special del NP (§5) y la lectura en pantalla.

- **Jefes: siempre.** **Estrella** (ver abajo): siempre. **Élites: sólo fallback** cuando la run no
  tiene ningún enemigo Estrella — un Guts barato abarata la transformación entera.
- **Estrella = enemigos de otros mods.** Verificado: BaseLib prefija los modelos custom
  (`ICustomModel` / `PrefixIdPatch`), y `FgoAttributes.RegisterOverride` es público → se registra
  desde el mod, sin tocar FGOCore. Caveats aceptados por escrito: la definición es «todo lo
  no-Mega Crit» (incluye mods hermanos), el diccionario de overrides es estático de proceso (hay que
  limpiar), y el balance de esos enemigos no lo controlamos — por eso el bonus es **chico (+20%) y
  se trata como huevo de pascua de compatibilidad, nunca como línea de balance**.

## 5. NP, reliquia de jefe y H1

- **Planet Olga Marie** (Buster, AoE): el peso real del special va contra **Humano** (= salas
  Monster por la convención de `FgoAttributes`) — inédito en el roster: nadie premia limpiar el
  pasto. Anti-Estrella +20% como guiño. El rider canónico «−20% resistencia Q/A/B» se pospone: sería
  un power enemigo nuevo en FGOCore y obliga a republicar los 13.
- **驚天動地 → reliquia de JEFE drafteable**, 1 vez por combate: llena el medidor pagando **Vida
  imparable proporcional a lo que falta** (arranque de playtest: 1 Vida por cada 10 de carga
  faltante; el excedente hacia 300 sale más caro). La Vida pagada **no alimenta nada** — es precio
  de tempo, no conversión (candado explícito contra la línea de sangre de Morgan). Comprás *llegar
  antes* a la decisión de §2, nunca *más total*. Se descartó como carta: una carta que regala el
  recurso central vuelve relleno a las otras 40.
- **空前絶後 → invertida a demérito propio**: «redibujá tu mano AHORA (+estrellas); tu próximo robo
  de turno baja en X». Nada del kit puede existir sólo en co-op: el sabotaje literal a los aliados
  (no roban / pierden Vida) queda como texto de sabor. Riders reutilizados: `SkillSeal` AoE 1T para
  el sello de NP enemigo y Bloqueo de Curación (ya en el pool de Tiamat).

## 6. 追加技能 — metaprogresión entre runs (primera del proyecto)

Cinco appends, dos niveles cada uno (bajo al desbloquear, alto al subir) = 10 completions para
maxear.

| Append | Nivel bajo → alto |
|---|---|
| 魔力装填 | Empezás cada combate con **+10 → +20** de Carga NP |
| 追撃技巧向上 | El token de Autoridad pega **+3 → +6** |
| 特攻技巧向上 | Daño crítico **+10% → +20%** |
| 技能再装填 | La reliquia de §5 **se re-arma 1 vez por combate → además cuesta 25% menos Vida** |
| 対Pretender適性 | Re-tematizado: **+10% → +20%** de daño contra **Amenazas** |

- **Ritmo (qué es "completar")**: victoria = elegís libre (desbloquear o subir a nivel alto);
  derrota habiendo llegado al Acto 3 = sólo desbloqueo (nivel bajo). Evita el muro para el jugador
  flojo y el farmeo de muertes rápidas. ⚠️ `ProgressState` cuenta victorias/derrotas por personaje
  (incluidos los modeados) pero **no** el "llegué al Acto 3": eso lo anota el mod.
- **Hook**: postfix Harmony sobre `RunManager.OnEnded(bool isVictory)` — público, cubre victoria,
  derrota y abandono. No existe hook de fin de run para mods; Harmony es idiomático acá.
- **Elección**: **evento estilo Grial al inicio de la run siguiente** («el informe de la
  Dirección»), con la maquinaria `CustomEventModel` de BaseLib que ya usa `HolyGrailRitual`. Los
  puntos pendientes se acumulan si no se gastan. Se descartó la pantalla custom de game-over (sin
  punto de extensión vanilla, sin precedente).
- **Almacenamiento**: JSON del mod (precedente `FgoVisualConfig : SimpleModConfig`). Dos reglas
  firmadas: **(i)** el archivo es editable a mano y se acepta — cero anti-cheat; **(ii)** los
  appends **no se leen durante el combate**: al iniciar la run se materializan como estado de run
  (vía la starter, que ya viaja sincronizada), para que el co-op vea un estado determinista y sólo
  apliquen a quien juega U-Olga.
- **El personaje base se balancea SIN appends.** Son añadido de recompensa por encima de la vara,
  no parte de ella.
- **Grial (`ILimitBreaker`) temático: reparar 単独顕現**, la única habilidad de clase que el canon
  declara perdida.

## 7. Presupuesto (regla ≤2 grandes + 1 chica)

1. **Grande**: Autoridad (conversión del medidor → ataques extra). 2. **Grande**: Guts → Forma 3.
3. **Chica**: el marcador de Amenaza. **No hay tercer medidor**: el estado de Autoridad ES el
contador visible y las Amenazas son metadata marcada, no una economía. Todo lo demás reutiliza
FGOCore (`NpCharge`, `CritStars`, `FgoAttributes`, `FormSwitch`, `GutsPower`, `SkillSeal`,
`Cleanse`, `BondRelic`).

## 8. Riesgos conocidos (van al playtest, no se ocultan)

- **Metaprogresión sin precedente** en los 13 mods: save propio, sincronización en co-op y el hook
  de fin de run son territorio nuevo del proyecto.
- **Balance ciego del bonus anti-Estrella**: depende de contenido de terceros; por eso es chico.
- **`技能再装填` depende de un drop** (la reliquia de jefe): un append que puede no hacer nada es mal
  diseño — el panel debe confirmarlo o cambiarlo por «podés convertir con el medidor a 40+».
- **El token 0⚡ vive al lado del arsenal de Gilgamesh**: la diferenciación es de diseño, no técnica.
- **Techo 180-220 de daño por turno**: la ráfaga de Autoridad + crítico + Buster de 逆光 se suma en
  la misma ventana; el panel debe presentar la cuenta del pico, no prometerla.
- **HP base**: banda 70-72 del roster (precedente Tiamat 70, Gilgamesh 72) para un kit explosivo.

## 9. Estado del panel §4.6.7 (2026-08-23) — INTERRUMPIDO, retomar acá

**Las tres propuestas están hechas y guardadas** (con las ~68 cartas, 12 reliquias y mazo inicial de
cada una): [A — Ráfaga y Tempo](PANEL-UOLGA-A-RAFAGA.md) · [B — La Caza y el
Martirio](PANEL-UOLGA-B-CAZA.md) · [C — El Presupuesto de la Directora](PANEL-UOLGA-C-PRESUPUESTO.md).

**Corrió 1 de 3 jueces.** El de balance dictaminó: **base = B**, con injertos de C (candados del
starter, «Garantía de Cumplimiento») y de A (cap +15 al Decreto, el token no critica, «Trono de la
Bestia VII»), **17 parches obligatorios** y la fórmula del token resuelta —
`daño = 10 + tier÷10, tope 30` — que es lo que hace defendibles las tres decisiones de conversión.
Pico de la base parcheada: **212** sin appends (dentro del techo), ~268 con appends maxeados (por
encima de la vara por diseño). Todo en
[PANEL-UOLGA-VEREDICTO-BALANCE.md](PANEL-UOLGA-VEREDICTO-BALANCE.md).

**El juez de implementabilidad ya corrió** (2026-08-23):
[PANEL-UOLGA-VEREDICTO-IMPLEMENTACION.md](PANEL-UOLGA-VEREDICTO-IMPLEMENTACION.md). Veredicto: la
base parcheada se construye en ~85% con piezas probadas y **nada obliga a tocar FGOCore** (cero
republish). 11 parches más, entre ellos que el **Decreto sea `CardType.Skill`** (con eso los tres
candados del acta y el «no critica» salen gratis), que `GaugeDropped` **exhauste a la hermana no
jugada**, y que el Anti-Purga y el Bloqueo de Curación se **reimplementen locales** (el acta prometía
un reuso cross-mod que no existe). Orden de lotes con riesgos, incluido que la metaprogresión es
**aislable y recortable** del primer release.

**Pendiente para la próxima sesión, en orden:**
1. **Relanzar el juez de fidelidad y legibilidad** — murió por límite de sesión. Encargo: si se
   juega como U-Olga y no como una Bestia genérica; auditoría de los ~68 nombres (los genéricos de
   fantasía espacial se van); techo de complejidad en mano; glows faltantes; refrito contra
   Gilgamesh (token vs arsenal) y Siegfried (mismo trigger de starter); tabla de nombres definitivos
   ES/EN/中文.
2. **Síntesis final**: aplicar los 17 parches del juez de balance + los 11 del de implementabilidad
   sobre B, y volcar el pool definitivo a este documento.
3. Después recién: assets (Atlas `4000100`, tres ascensiones), scaffold del mod y localización.

## 10. Lo que falta (encargo del panel §4.6.7)

Pool de ~68 recompensas (20/28/20) con conectividad ≥90% en comunes y denominaciones 10/20/30/50/100;
mazo inicial de 10 sesgado a QAABB; starter relic como motor con cap 3/turno; ~12 reliquias; la
carta-NP y su Desatada; los riders de Forma 3; cobertura de frontload, defensa, consistencia,
economía, escalado, multiobjetivo y respuesta a jefes que limpian buffs (la Autoridad es un tanque
de cartas, no un buff: eso ya ayuda); glow dorado en toda condicional; y la localización en cinco
idiomas con los nombres canónicos de §0.
