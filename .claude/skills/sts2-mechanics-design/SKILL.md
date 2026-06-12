---
name: sts2-mechanics-design
description: Diseño de mecánicas y stats para personajes custom de Slay the Spire 2 (mods FGO). Baselines numéricos extraídos del juego real, filosofía de balance "rota pero honesta", método de traducción FGO→StS2, y reglas de diseño de formas/recursos. Usar SIEMPRE antes de diseñar un personaje, carta o reliquia nueva.
---

# Diseño de mecánicas y stats — StS2 (mods FGO)

Esta skill existe porque el balance "de oído" falla: cada número de un diseño debe
poder justificarse contra el baseline del juego real (sección 2) o contra un costo
explícito (sección 3). Si un número no tiene justificación, está mal.

## 1. Filosofía: rota pero honesta

**Objetivo: top-tier, no tramposa** (referencia: la Watcher de StS1 — la más fuerte
del elenco sin trivializar jefes). Reglas:

### 1.bis EL TECHO REAL (replanteo 2026-06-11, pedido del usuario): el entorno modded

El usuario NO juega vanilla. Juega con **HextechRunes** (dificultad) + **BetterCharacters**
(personajes vanilla deificados — el mod se llama 我已神化). Medido del decompilado:

- **HextechRunes** (la presión): los enemigos reciben *hexes* por acto que se acumulan —
  daño enemigo ×1.2-1.5, HP máximo +20-40% por tier, sustain/bloqueo ×1.15-1.25, más
  impuestos mecánicos (cada 3er ataque del jugador cuesta doble ⚡, slow, strength drain).
  El jugador compensa con runas MULTIPLICATIVAS (×1.2-×2 en daño/bloqueo/HP/curación)
  y forjas de stats. Peleas más largas y picos de daño enemigo mucho más altos.
- **BetterCharacters** (el nivel de los pares): los personajes vanilla llevan motores
  ×1.5-2 (Forma Demoníaca DUPLICA toda la Fuerza; Maestría Nigromante DUPLICA las
  invocaciones; forja exponencial del Regent), tasas +20-40%, descuentos de coste en
  piezas de motor (3⚡→2⚡, 2⚡→1⚡), Exhaust removido de cartas clave, y topes quitados.
- **BetterSpire2** es solo QoL (información perfecta del daño entrante + retry de
  combate) — no toca números, pero implica que la VARIANZA es barata: diseños de techo
  alto que a veces fallan son aceptables.

**Reglas operativas del replanteo**:
1. Los baselines de §2 siguen siendo el VOCABULARIO (qué es una tasa vanilla), pero el
   TECHO de diseño es ~×1.5 sobre ellos en motores/payoffs y ~×1.25 en tasa cruda.
2. La subida de tasa cruda va por **palanca central**: `BondRelic.ServantDamageMultiplier`
   / `ServantBlockMultiplier` (FGOCore, ×1.25, herencia automática en todos los
   personajes) — NO inflando carta por carta. Perilla: 1.25 → 1.4 si sigue floja.
3. Los MOTORES sí se suben en sus números propios (estilo BetterCharacters: duplicar el
   escalador del arquetipo, acelerar la economía del recurso, levantar topes que ahogan
   en peleas largas — Maldición 15→25 fue el primer caso).
4. Los topes/candados anti-degeneración se conservan ESTRUCTURALMENTE (1/turno,
   1/combate, condición de forma) pero sus montos se presupuestan al techo nuevo.

### 1.ter Los jefes del mod JeanneAlter (re-stat 2026-06-12, análisis del decompilado)

El usuario también enfrenta los 3 jefes custom de JeanneAlter (Kirei acto 1 / U-Olga
acto 2 / Beast VII acto 3): HP ×2.3-3.1 sobre vanilla, **HardenedShell 200 (cap de daño
por turno)**, strips de TODOS los buffs vanilla a umbrales de HP, Artifact 99 (debuffs
muertos), flood de 5 status/turno, Nemesis (Intangible turno por medio), Buffer 30, y
goteo de daño NO bloqueable. Peleas de 12-20 turnos. Reglas derivadas:
1. Multiplicador del BondRelic = **×1.4** (calibrado: cruza los breakpoints ~60/85 DPS
   sostenido sin trivializar vanilla). NO subir más: el cap desperdicia el burst.
2. **Techo de saturación por turno ≈ 180-220** para cartas NP y payoffs (= saturar el
   cap sin desperdicio). No diseñar nukes de 300+.
3. HP iniciales +10 (95/88/80) y **ServantRegenPerTurn 3 en BondRelic** (curación
   a prueba de strip — Regen vanilla SÍ se borra) para el goteo imbloqueable.
4. Tech estructural obligatoria en cada pool: multi-hit barato repetible (anti-Buffer
   30), purga/exhaust de cartas de status (anti-flood), y daño que NO dependa de
   debuffear al jefe (Artifact 99). Los motores FGOCore (NP/Baluarte/Formas/Estrellas)
   SOBREVIVEN al strip — escalar ahí, no en Strength/Dexterity vanilla.

1. **Números sobre tasa SOLO embudados por mecánica propia.** Una carta puede superar
   el baseline si su exceso está pagado por: un recurso que hay que construir (Carga NP),
   costo de HP, Exhaust, condición de forma/stance, o renunciar a otra cosa (dupes).
2. **El plan A del personaje debe ser interrumpible.** Si el enemigo fuerza el modo de
   juego incómodo (a Mash no dejarla bloquear; a un berserker hacerle pagar el HP),
   el personaje baja a tasa normal — nunca por debajo (frustrante), nunca lo ignora (roto).
3. **Cada mecánica nueva paga su complejidad.** Un personaje soporta ~2 mecánicas
   originales + 1-2 keywords de FGOCore. Más que eso no se lee en una carta.
4. **Sinergia interna > poder individual.** La carta mediocre que alimenta el motor
   vale más que la carta fuerte suelta. Diseñar el MOTOR primero, las cartas después.
5. **Multijugador**: los monstruos escalan HP ×jugadores×1.1–1.3. Bonos defensivos
   personales de un tanque escalan con jugadores; la ofensa NO (el equipo ya escala).

## 2. Baselines del juego real (extraídos del decompilado v0.103.x, 577 cartas)

**Personajes**: HP 66–80 (Ironclad 80, Defect/Regent 75, Silent 70, Necrobinder 66).
Todos: 3⚡, 99 oro, mazo inicial de 10 (4-5 Golpes, 4-5 Defensas, 1-2 únicas).
**Pool por personaje (plantilla vanilla EXACTA)**: 4 básicas + 20 comunes +
36 poco comunes + 26 raras + 2 Ancient = 88. Incoloras: 64.

**Daño por energía** (puro = sin rider):
| Rareza | 0⚡ | 1⚡ puro | 1⚡ + rider | 2⚡ | 3⚡ |
|---|---|---|---|---|---|
| Básica | — | 6 (Strike) | 8+Vuln2 a 2⚡ (Bash) | | |
| Común | 3-6 | **9-10** | 6-8 | 18 con downside | 27 con Retain |
| Poco común | 5-13 c/keyword | **11-15** | | 12-15+rider (7.5/⚡) | 24-32 (Bludgeon 32 puro) |
| Rara | 33 por recurso (Comet 5★) | | | | |

**Bloqueo por energía**:
| Rareza | 0⚡ | 1⚡ | 2⚡ |
|---|---|---|---|
| Básica | — | 5 (Defend) | |
| Común | 4-7 | **7-9** puro / 4-7+rider | 16 con −2 HP |
| Poco común | 3-10 c/keyword | 11-13 con condición | 11-13+rider |
| Rara | 30 Exhaust+drawback (PanicButton) | | 30 Exhaust (Impervious, 15/⚡) |

**Premiums medidos**: Ethereal +30-40% (Defile 13/1⚡); Exhaust ≈ +20-30% o un rider
(Impervious 30 vs 13: ×2.3 con salto de rareza); Innate+Exhaust 0⚡ = 11 (Backstab).
**Multi-hit**: la suma IGUALA la norma single (3×3/1⚡) — paga targeting aleatorio,
gana escalado con Fuerza. **AoE**: −25-35% por golpear a todos (6-8 vs 9-10 a 1⚡;
Whirlwind 5/⚡ AoE). **HP como recurso**: 1 HP ≈ rider chico; 2 HP ≈ +5-6 de efecto
(Hemokinesis 15/1⚡); 3 HP ≈ 2⚡ (Bloodletting); 6 HP ≈ 2⚡ + 3 robos con Exhaust
(Offering). El auto-daño SIEMPRE con `ValueProp.Unblockable|Unpowered`.

**Recurso secundario (el precedente vanilla = Estrellas del Regent)**: 21 cartas con
doble coste (0-2⚡ + 1-7★). Tasa: **1★ ≈ 3-5 de daño ≈ ½⚡**; generadores dan 1-2★
por carta (básica +2★). Comet: 0⚡+5★ = 33 dmg. → Para Carga NP: ~10 NP ≈ 1★ ≈ ½⚡
si los generadores dan ~10 NP/carta.

**Powers tipo "Form" vanilla**: 3⚡, raras, escalan por turno (DemonForm +2 Fuerza/t;
EchoForm 1ª carta ×2, Ethereal; WraithForm 2 Intangible con drawback acumulativo —
la única con castigo). NO existen stances tipo Watcher en vanilla: las formas
exclusivas de personaje custom (FGOCore) son terreno nuestro, sin baseline directo —
usar §5.

## 3. Tabla de costos (cuánto "paga" cada penalidad)

Equivalencias aproximadas para presupuestar cartas (derivadas de comparar pares
vanilla con/sin penalidad; verificar contra §2 al usarlas):

| Penalidad | Paga aproximadamente |
|---|---|
| Exhaust | +30–50% de efecto sobre tasa |
| Ethereal | +20–30% |
| Costo de HP (N de vida) | ~+2N de efecto (el HP es el recurso más caro) |
| Condición de forma/stance | +20–30% (la condición debe poder fallar de verdad) |
| Consumir recurso acumulable (NP, etc.) | el efecto/punto debe ser ≤ al costo de generarlo, o el loop se rompe |
| Innata de un solo uso por combate (marker) | +50–100% |
| Carta que ocupa slot de mazo sin efecto directo (setup puro) | el payoff debe ganar el combate, no un turno |

## 4. Método FGO → StS2 (en orden)

1. **Identidad en una frase.** "La muralla que avanza" (Mash). Si no se puede decir en
   una frase, el diseño no está listo. **La fidelidad al kit manda** (regla del usuario,
   aprendida con Morgan v1→v2): cada skill real del servant debe existir como carta o
   reliquia RECONOCIBLE, y los NP reales son las cartas-NP. Las mecánicas novedosas van
   como arquetipos drafteables DENTRO del pool, nunca como reemplazo del kit.
2. **Inventario del kit FGO**: skills, NP con Overcharge, pasivas, lore beats. Cada
   skill de FGO es candidata a carta/reliquia; el NP son las cartas-NP (mín. de carga +
   escalado por consumo total — ya resuelto en FGOCore).
3. **Elegir el RECURSO/motor del personaje** (además de Carga NP, que es universal FGO):
   ¿qué acumula, qué lo gasta, qué lo interrumpe? El recurso debe verse en pantalla
   (power contador o contador de reliquia).
4. **Mapear mecánicas a FGOCore primero** (no reinventar): NpCharge 0-300 con ulti a 100
   (`GaugeFilled`/`GaugeDropped`), formas (`FormPower`/`FormSwitch`/`FormVisuals` con
   swap de modelo), Baluarte/retención (`IBlockRetentionSource`), vínculo (`BondRelic`),
   dupes/NP level (`INpLevelStore`+`NpLevels`+alternativa de recompensa), Santo Grial
   (`ILimitBreaker`), memes incoloros. Solo diseñar desde cero lo que no exista.
5. **Pool**: 4 básicas (2 firmas) + ~20 comunes + ~28 poco comunes + ~20 raras +
   especiales generadas. Distribución interna: cada rareza necesita ~40% cartas "pan
   y manteca" que funcionen sin el motor (deck inicial malo existe), ~40% que alimenten
   el motor, ~20% payoffs.
6. **Reliquias**: 1-3 starter (mecánica core del personaje), ~4-6 del pool propio
   (común→ancient). La starter define el personaje; las del pool lo amplifican.
7. **Nombrar todo trilingüe** (eng / esp latino / zhs con terminología Mooncell) DESDE
   el diseño — renombrar después rompe IDs.

## 5. Reglas de diseño de formas/stances (lo aprendido con Mash)

- Una forma es un **power Single** con pasivas como flags; el swap de visual es
  `FormPower.FramesPath` + FGOCore precarga en hilos (sin congelones).
- **Cada forma debe cambiar las DECISIONES, no solo los números**: si las dos formas
  juegan las mismas cartas igual, es un buff disfrazado. La pregunta de diseño:
  "¿qué carta juego distinto según la forma?"
- Cartas de cambio de forma: baratas (0-1⚡), con un efecto inmediato pequeño para no
  ser cartas muertas, y el cambio debe poder hacerse ~2-3 veces por combate.
- Una forma puede ser **permanente** (`IsPermanent`) solo como clímax (Paladín).
- Pasivas de forma con números chicos por turno (3-5) — se acumulan 30+ veces por combate.
- El multiplicador real de una forma está en cartas condicionales ("si estás en X: +Y"),
  no en la pasiva.

## 6. Checklist final (correr antes de dar el diseño por terminado)

- [ ] ¿Cada carta sobre-tasa tiene su exceso pagado según §3?
- [ ] ¿El mazo inicial (4 golpes, 4 defensas, 2 firmas) gana el acto 1 sin el motor armado?
- [ ] ¿Hay un plan B cuando el motor no llega (élites tempranas, jefes con presión)?
- [ ] ¿Los nombres existen en los 3 idiomas con terminología oficial CN?
- [ ] ¿Todo asset necesario existe y está verificado (IDs de Atlas Academy, modelos de batalla por forma)?
- [ ] ¿Las mecánicas reutilizan FGOCore donde corresponde?
- [ ] ¿El power-budget total queda al nivel Watcher, no por encima?
