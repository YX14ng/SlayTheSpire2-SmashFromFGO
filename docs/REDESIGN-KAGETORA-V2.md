# REDESIGN-KAGETORA-V2 — Nagao Kagetora / Uesugi Kenshin

> **Estado: PROPUESTA APROBADA POR PANEL — no se implementa nada hasta el visto bueno del usuario.**
> Síntesis del panel de diseño del 2026-08-16 (WORKFLOW-FGO §4.6.7: tres propuestas, tres jueces, «los parches del juez MANDAN»).
> Base: **Propuesta 1 — MOTOR Y ECONOMÍA**, ganadora **3–0**. Todos los parches obligatorios de los tres jueces aplicados (J1 P-1…P-16, J2 K1…K17, J3 J-01…J-17), con las contradicciones resueltas **a favor del más restrictivo** y anotadas en §12.3.
> Mecanismo save-safe: **ningún ID se renombra, ninguno se borra, cero DEMOTE**. El re-pool es re-efecto sobre IDs publicados + **3 cartas [NUEVA]**. **FGOCore no se toca en este pase** ⇒ se publica solo `KagetoraLancer`; los 12 personajes no se republican.
> Base de hechos: `docs/DIAGNOSTICO-KAGETORA.md`, `docs/DESIGN-KAGETORA.md`, formato `docs/REDESIGN-MORGAN-V2.md`, y el código en `HEAD` (`2a0fd4a0`), **no** el estado que describía el diagnóstico: los commits `6f3b1d29` y `42295b7b` ya cerraron 7 bugs que las tres propuestas facturaban como trabajo propio (§11.2).

---

## 1. Identidad

**En una frase:** *La comandante invicta cuyo ciclo de tres preceptos deja de cobrarle el turno y se lo devuelve — cerrar Cielo→Pecho→Pies paga **+1⚡ y 50 estrellas exactas, que son exactamente un crítico** — y que al ascender a Kenshin cambia el orden fijo por libertad de orden, el doble de Carga NP por ciclo, Bloqueo que no se limpia y un Noble Phantasm que arranca el Bloqueo del enemigo antes de pegar.*

**Verbos:** ordenar, cobrar, ascender.

Como el Ironclad tiene Strength / Block / Exhaust / Barricade, Kagetora tiene **cuatro líneas de draft reales** ancladas al canon (el Bishamonten viviente de Echigo, Kawanakajima, la sal enviada al rival, el 車懸りの陣). El motor de `Doctrine.cs` — timing en `AfterCardPlayed`, golpe letal, reentrancia, copias, reset, `MaxAdvancesPerTurn = 3` — **se conserva intacto**. Lo que cambia es la economía alrededor y las cuatro superficies por donde el jugador lo lee.

---

## 2. Arquetipos y matriz de cobertura

| | **A. La Rueda** (Formación/NP) | **B. Muralla de Echigo** | **C. Caballería Crítica** | **D. Ejecución de Kenshin** |
|---|---|---|---|---|
| **Motor** | Cielo barato → ciclos frecuentes → NP temprano | Bloqueo acumulado **con salida a daño** | 50★ por ciclo → un crítico por turno, garantizado | ascender rápido: 20 NP/ciclo, retención, NP de 8 impactos |
| **Fantasía** | la comandante que ordena la formación | la provincia que no cae | la carga de caballería de Kawanakajima | el Bishamonten que ejecuta |
| **Ataque** | Estocada Celeste, Cuatro Golpes, Blanca Llama Fría | **Muro de Lanzas [NUEVA]**, **Muralla de Echigo [NUEVA]** | Biten: Formación de Rueda, Ocho Armas Desatadas, Galope | Hoja Shiranui, Kawanakajima, Báculo del Comandante |
| **Defensa** | Consejo del General, Voto (Artifact) | Murallas de Kasugayama, Dos Evasiones, Defensa del Ruler, Coraza de Seis Placas | Cortina de Disparos (defensa→estrellas) | Camino del Justo, Sorbo en el Centro, **retención de 5 (forma)** |
| **Consistencia** | Orden de Batalla, Relevo de Formación, Sabiduría de 84.000, Lectura del Campo | Coraza de Seis Placas (Retain), Guardia Compartida | Pisadas del Ejército (tutor, ya arreglado en `HEAD`) | pasiva anti-atasco de Kagetora |
| **Energía** | 4 cartas de 0⚡ en Cielo; **el ciclo devuelve 1⚡** | Formación Cerrada (0⚡) | 3 de 0⚡ en Pies | Brasero (+1⚡ el turno del ascenso) |
| **Escalado** | niveles de NP, Pagoda Enjoyada C (OC) | Pecho sin Temor, Juez del Campo | La Victoria Está en los Pies, Cabalgata | Manifestación (**+3 Fuerza tope**), Divinidad (+5, **1 Ataque/turno**) |
| **AoE** | — *(flojo, declarado)* | — **nulo, declarado** | Barrido de Naginata, Naginata Giratoria | Galope Total (2⚡) |
| **Jefes** | el NP limpia mejoras ofensivas + Débil por OC | Artifact/Buffer/Intangible **no son debuffs** ⇒ inmunes al strip | **multi-impacto anti-Buffer en las 3 rarezas** | el NP de Kenshin **quita el Bloqueo antes del daño**; Kawanakajima +8 vs Elite/Jefe |
| **Debilidad declarada** | daño frontal bajo; muere a élites rápidas de Acto 1 | cero AoE; gana por acumulación, pierde por reloj | un solo crítico por turno: sin banco es una común genérica | la primera carrera cuesta 100 NP enteros |

**Regla DECISIONS «el pool no puede depender solo de debuffs»: cumplida por construcción.** Kagetora casi no usa debuffs (Débil en 3 cartas, Vulnerable en 1). Todo su escalado — Fuerza, Bendición, Divinidad, Bloqueo, estrellas, Carga NP — es propio y sobrevive a cualquier `Cleanse`. **Multi-hit anti-Buffer en las tres rarezas:** común (Ataque por Turnos 4×2, Carga de Houshoutsukige 4×3), poco común (Galope 4×3, Asalto Alternado 3×3, Ocho Armas 10×2), rara (Biten 2×6, Ocho Armas Desatadas 4×4).

**Sobre la convergencia 1:1:1 (R2 del diagnóstico), con honestidad:** se conserva el orden fijo, y los tres preceptos dejan de ser intercambiables porque **cuestan distinto y pagan en monedas distintas** (Cielo 0,91⚡→Carga NP · Pecho 1,13⚡→Bloqueo · Pies 1,14⚡→estrellas y daño). Un mazo Cielo-pesado cicla barato y seguido; uno Pies-pesado cicla menos y pega más fuerte. **La convergencia baja de 1:1:1 a curvas 2:1:1 / 1:1:2 / 1:2:1; no desaparece.** Es el costo estructural de conservar el orden fijo, y el orden fijo se conserva porque es la moneda con la que se paga la ascensión. La respuesta fuerte a R2 (el «Refuerzo» de la Propuesta 2) entra como **contingencia declarada §15.3**, no como diseño, porque su autor no pudo demostrar que no mata el ciclo.

---

## 3. La Doctrina rediseñada

### 3.1 Lo que NO se toca

Verificado en `Doctrine.cs` y confirmado por los tres jueces: el avance ocurre en `AfterCardPlayed` tras resolver el texto completo; una carta equivocada se juega normal, no avanza y **no borra progreso**; el progreso parcial persiste entre turnos; `MaxAdvancesPerTurn = 3`; la recompensa innata se concede **antes** de cambiar el estado y de emitir eventos; el mask se vacía antes del evento de ciclo. **Ninguna carta cambia de precepto** ⇒ las 69 etiquetas 天/胸/足 × 5 idiomas quedan intactas.

### 3.2 Reglas nuevas (E1–E8)

| # | Regla | Por qué |
|---|---|---|
| **E1** | **Cerrar un ciclo devuelve 1⚡.** Innato al motor, no a la reliquia. | R1: el ciclo cobraba en energía y pagaba en recursos diferidos. Ahora la moneda que consume es la que devuelve. Es la **única** fuente de energía del kit. |
| **E2** | El refund es **≤1 por turno por construcción**: un ciclo son 3 avances y `MaxAdvancesPerTurn = 3`. El tope se evalúa **antes** de los overrides (`Doctrine.cs:84` y `:131`), así que ni `EightFormationsPower` ni `ForcedDoctrineAdvancePower` lo saltan. | Restricción 6: la prueba es **estructural**, no un cap añadido. Cero estado nuevo, cero superficie de bug. Los tres jueces la verificaron independientemente. |
| **E3** | **La Pagoda (starter) procea POR AVANCE: +10★, exactamente 3 procs/turno** — el cap lo garantiza `MaxAdvancesPerTurn`, no un contador. **Sin robo.** | 4.6.4 exige starter = motor con cap 3 procs/turno; hoy proceaba 1 vez/turno. El robo se saca (P-2, §12.3-1) para que el cierre no pague ⚡ **y** carta. |
| **E4** | **Breakpoint: un ciclo = 50 estrellas exactas = un crítico.** 10+10+10 (Pagoda) + 20 (innato de Pies) = 50 = `CritStarsPower.CritCost`. | La legibilidad no se enseña con texto: se enseña con aritmética que cierra redonda. |
| **E5** | **Todo 0⚡ repetible es una conversión que GASTA más de lo que su propio avance devuelve.** Los 0⚡ de valor neto llevan Agotar. Sin tercera opción. | Los tres jueces cazaron la misma violación en la propuesta ganadora. Auditado carta por carta en §8 y §3.4. |
| **E6** | **Un crítico por turno, y solo donde vale.** `DoctrinePower : ICriticalAccessRule` → `CanSpendCritical(card) => !CriticalUsedThisTurn && (Owner.GetPowerAmount<CritReadyPower>() > 0 || card is not IPreceptCard tagged || tagged.Precept == Precept.Feet)` | `CriticalResolverPower.BeforeCardPlayed` **no tiene gate por turno** y `CritStarsPower.Max = 100`: sin cap, el banco compra dos críticos y el breakpoint E4 es mentira. Las cartas **sin precepto** (incoloras de FGOCore, `BlackKeys`) y el Crítico Listo conservan el crítico: `Criticals.CanSpend` corre **antes** de mirar `CritReadyPower`, y las tres propuestas afirmaban lo contrario. |
| **E7** | `DoctrinePower` implementa **`IResourcePower`**. | Sus dos satélites ya lo hacen y el motor no. `DESIGN §4.4` promete «la Doctrina nunca se pierde». |
| **E8** | **La Bendición solo se arma en Ataques de Pies o sin precepto**, y vale **+2 por impacto con tope de 4 impactos** (en los NP también +2, tope 4). | `BishamontenBlessingPower.BeforeCardPlayed` la convertía en el **primer Ataque de cualquier tipo**: con orden fijo eso es `Arts` (Cielo, 1 impacto de 6) o un Ataque de Pecho. El «+2 por impacto» era, en la línea natural, «+2 total» — y en la línea de pico era «+16 sobre un NP de 8 impactos». Rota por abajo e inflada por arriba al mismo tiempo. |

### 3.3 Presupuesto de energía — la cuenta hecha

Curva de costos del pool, **71 drafteables**, reparseada tras aplicar todos los parches:

| Precepto | n | 0⚡ | 1⚡ | 2⚡ | 3⚡ | **medio** | medio HOY | Ataques |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Cielo | 21 | 4 | 15 | 2 | 0 | **0,905** | 1,048 | 4 |
| Pecho | 24 | 1 | 19 | 4 | 0 | **1,125** | 1,318 | **2** (hoy 0) |
| Pies | 22 | 3 | 13 | 6 | 0 | **1,136** | 1,429 | 16 |
| neutral | 4 | 0 | 4 | 0 | 0 | 1,000 | 1,750 | 0 |
| **total** | **71** | **8 (11,3 %)** | 51 | **12 (16,9 %)** | **0** | **1,056** | 1,26 | 22 |

Hoy: 0⚡ = 8,8 % (y **una sola repetible**), ≥2⚡ = 35 %, Pies ≥2⚡ = 48 %. Después: 0⚡ = 11,3 % con **6 repetibles**, ≥2⚡ = 16,9 %, Pies ≥2⚡ = 27 %, y **cero cartas de 3⚡**.

**Cadena media = 0,905 + 1,125 + 1,136 = 3,17⚡** (hoy 3,794).

| Escenario | Cadena | Refund E1 | **⚡ libre (base 3⚡)** |
|---|---:|---:|---:|
| **HOY (medido, diagnóstico §1.1)** | 3,79 | 0 | **0,43⚡** · sobrante 0⚡ el 100 % de las veces |
| Rediseño, **cota conservadora** (coste medio, sin poder elegir) | 3,17 | −1,00 | **0,83⚡** |
| Rediseño, **juego real** (elegís la más barata entre 1-2 opciones del precepto esperado) | ~2,5 | −1,00 | **~1,5⚡** |
| Rediseño, **mano de conversión** (tres 0⚡) | 0,00 | −1,00 | **4,0⚡**, pagando 60★ de banco y 3 cartas de mano |
| *Contrafactual del diagnóstico: subir la base a 4⚡* | 3,79 | 0 | 1,43⚡ (tasa de ciclo **idéntica**: 6,50 → 6,50) |

**En qué se gasta lo que queda.** Con 0,43⚡ no hay decisión: cerrás el ciclo y terminás el turno con dos cartas injugables. Con ~1,5⚡ hay tres jugadas competitivas, y esa es la decisión que hoy no existe:
1. **Cerrar el ciclo y gastar el refund en el remate** (Retroceder es el Infierno 1⚡, Ocho Armas Desatadas 1⚡).
2. **No cerrar**: guardar el progreso Cielo+Pecho para el turno siguiente y gastar 2⚡ en un Ataque grande de Pies fuera de secuencia — el progreso **no se pierde**, así que es una jugada legítima, no un castigo.
3. **Cerrar y reinvertir en conversión**: 50 NP → 50★ (Dar Vuelta a la Formación, 0⚡) para asegurar el crítico del turno siguiente.

**Por qué NO se sube `MaxEnergy` a 4** (pregunta 1 del diagnóstico): es `virtual` y sería una línea, pero el contrafactual **medido** demuestra que no mueve la tasa de ciclo. Premia igual al que cicla y al que no —de hecho más al que no cicla— y rompe la paridad de 3⚡ del roster entero. Sobrevive únicamente como **knob de emergencia** (§15.4, último de la lista).

**Por qué la energía NO va en la reliquia inicial:** es la bandera roja «reliquia inicial que elimina la debilidad declarada desde el turno uno», y encima la reliquia se pierde o se reemplaza. El ⚡ va **en el evento que el diseño quiere premiar**.

### 3.4 Auditoría de la mano de tres 0⚡ (la bandera roja n.º 1)

Peor caso construible con el pool final: **Oración a Bishamonten** (Cielo, gasta 20★ → +30 NP) + **Formación Cerrada** (Pecho, gasta 20★ → 8 Bloqueo) + **Carga Estrellada** (Pies, gasta 20★ → 8 daño).

| Recurso | Gasta | Genera | **Neto** |
|---|---:|---:|---:|
| Estrellas | 60 | 30 (Pagoda ×3) + 20 (innato Pies) = **50** | **−10★** |
| Cartas | 3 de mano | 0 (**la Pagoda ya no roba**) | **−3 cartas** |
| Energía | 0 | +1 (E1) | **+1⚡** |
| Carga NP | 0 | 30 + 10 (innato Cielo) = 40 | +40 |

**No hay robo neto positivo y no hay ganancia neta de estrellas.** El ciclo de conversión existe, es intencional (es el arquetipo A) y cuesta tres cartas de mano y 10★ por 1⚡. Está capado a 1/turno por E2. Aun así, la fila de la restricción 6 se firma **⚠️ hasta la re-simulación** (§13), porque la honestidad en esa celda fue el estándar que impuso el panel.

---

## 4. Qué ve el jugador en pantalla

Cinco canales, todos Kagetora-local, todos verificados contra el decompilado por al menos dos jueces:

1. **El icono muestra el número.** `DoctrinePower.StackType` → `PowerStackType.Counter` y `override int DisplayAmount => popcount(ProgressMask)` (0-2). `NPower.cs:234` imprime `DisplayAmount` **solo** con `StackType == Counter`; `PowerModel.SetAmount` dispara `DisplayAmountChanged` y `NPower.RefreshAmount` lo repinta. Ojo: `DisplayAmount` debe devolver el popcount, **no** `Amount` (que es mask+1).
2. **La `smartDescription` dice el orden, lo que falta y las recompensas, y es dinámica por forma.** Hoy es la misma cadena antes y después de ascender y no nombra ningún precepto. Vía **`BaseLib.Patches.Localization.IAddDumbVariablesToPowerDescription`** (no solo `DynamicVars`: `DynamicVar` es `decimal` salvo `StringVar`, y el postfix de `PowerModelLocPatch` corre dentro de `AddDumbVariablesToDescription`, que `PowerModel.HoverTips` invoca en ambas ramas):
   - **Kagetora:** *«Orden: Cielo → Pecho → Pies. Falta: **!Next!**. Avances: !Adv!/3. Cielo +10 Carga NP · Pecho +4 Bloqueo · Pies +20 estrellas. **Cerrar el ciclo: +1⚡.**»*
   - **Kenshin:** *«Orden libre, sin repetir. Faltan: **!Missing!**. Avances: !Adv!/3. … **Cerrar el ciclo: +1⚡ y +10 Carga NP.**»*
3. **Ordinal I/II/III impreso en la etiqueta de las 69 cartas** — el único canal que pone el orden **sobre el objeto que el jugador está mirando**: `(I · Heaven 天)` / `(I · Cielo 天)` / 【一·天】 / `(I · 하늘 天)` / `(I · Небо 天)`. Es **loc pura**: cero código, cero IDs, revert de un solo commit. Contra la objeción legítima de que **el ordinal miente bajo Kenshin**, el tooltip de la Doctrina y la `smartDescription` de `KenshinFormPower` dicen explícitamente *«como Kenshin el orden es libre: los números indican los tres preceptos, no una secuencia»*. Ver §12.3-2.
4. **Las 69 cartas explican el sistema.** `protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DoctrinePower>()]` en `KagetoraCard` (la base). Una línea; hoy el mod tiene 7 tooltips contra los 75 de Artoria y **cero en el pool drafteable**.
5. **Glow dorado en toda condicional** (regla 4.6.5, hoy con 1 solo hit en todo el mod contra 27 de Morgan), en `KagetoraCard`:
   ```csharp
   protected override bool ShouldGlowGoldInternal =>
       Precept != Precept.None &&
       Owner?.Creature?.GetPower<DoctrinePower>()?.WouldAdvance(Precept) == true;
   ```
   **Todo el encadenamiento con `?.`**: `ShouldGlowGold` es getter público y se consulta fuera de combate (compendio, pantalla de recompensa), donde `Owner` puede ser null — un NRE ahí revienta 69 cartas. Regla de condición vacía: sin precepto, sin glow. `KagetoraNpCard` conserva su override.

**No se agrega un tercer medidor.** `FgoSecondaryResources.RegisterCombatMeters` es `private static` dentro de FGOCore: no es consumible desde un mod de personaje, y tocarlo obliga a republicar los 12 mods. Ver §12.5.

---

## 5. La ascensión

Los dos `FormPower` están **vacíos** hoy (punteros a un `.tres`, sin un solo hook). Se llenan:

**`NagaoKagetoraFormPower` — Nagao Kagetora (inicial):**
> *Orden fijo: Cielo → Pecho → Pies. **Al inicio de tu turno, si no tenés en la mano ninguna carta del precepto esperado, robá 1.***

Válvula anti-atasco que ataca exactamente el modo de fallo del orden fijo. Condicional, autolimitada (1/turno y solo cuando estás trabada), y **legible**: el juego te demuestra que sabe qué precepto espera.

**`KenshinFormPower` — Uesugi Kenshin (irreversible):**
> *Orden libre, sin repetir dentro del ciclo. Completar un ciclo da **+10 de Carga NP adicional**. **Conservás hasta 5 de Bloqueo al final del turno.***

| Qué cambia | Hoy | Rediseño |
|---|---|---|
| Orden | libre — **invisible** | libre **y anunciado** (contador + `smartDescription` + tooltip) |
| Velocidad de la 2.ª carrera | 10 NP/ciclo | **20 NP/ciclo** → 5 ciclos hasta los 100 en vez de 10 |
| Arranque de la 2.ª carrera | 0 NP | **+50 NP, +30★ y +1⚡ ese turno** con el Brasero (hoy: efecto muerto por el bug P0, ya corregido) |
| Defensa | nada | **retención de 5 de Bloqueo**, permanente, vía `KenshinFormPower : IBlockRetentionSource` |
| NP | 4 impactos, **peor contra jefes** | **8 impactos**, mejor a cualquier OC, **quita el Bloqueo antes del daño** |
| Cartas con rider de forma | **3 de 74 (4,1 %)** | **9 de 74 (12,2 %)** — Báculo, Hoja Shiranui, Divinidad *(existentes)* + Estocada Celeste, Armadura en el Pecho, Lanza de Ocho Pétalos, Barrido de Naginata, Galope de Houshoutsukige, Muralla de Echigo |
| Forma / UI | swap de sprite seco | + contador, + texto dinámico, + `KENSHIN_FORM_POWER.title` = **上杉謙信 / Uesugi Kenshin** en 5 idiomas |

**Kenshin NO recibe +1⚡ permanente.** Era la fuente aritmética del pico de 257-267 que los tres jueces recalcularon (4⚡ base + refund = 5⚡ en el turno del NP, que es el turno del pico), y es el mismo regalo incondicional que el diagnóstico ya midió como inefectivo, solo que diferido. El paquete de arriba es delta visible, permanente y **energéticamente neutro**. Ver §12.3-3.

**El nombre del personaje en el HUD no cambia.** No es alcanzable: ver §12.5.

---

## 6. Mazo inicial y básicas

**10 cartas, IDs sin cambios.** Regla 4.6.1 respetada (Buster 10 daño / Arts 6 + 30 NP / Quick 6 + 30★; sesgo QAABB).

| Cant. | Carta (eng) | Origen | ⚡ | Precepto | Efecto | Mejora |
|---:|---|---|---:|---|---|---|
| 2 | Buster | [REUSA `Buster`] | 1 | Pies | 10 daño | 13 |
| 2 | Arts | [REUSA `Arts`] | 1 | Cielo | 6 daño, +30 Carga NP | 9, +30 |
| 1 | Quick | [REUSA `Quick`] | 1 | Pies | 6 daño, +30★ | 9, +30 |
| 3 | Defensor (Defender) | [REUSA `Defender`] | 1 | Pecho | 5 Bloqueo | 8 |
| 1 | La Fortuna Está en el Cielo | [REUSA `FortuneIsInHeaven`] | 1 | Cielo | +20 Carga NP, robá 1 | +30, robá 1 |
| 1 | Encarnación de Bishamonten | [REUSA `IncarnationOfBishamonten` — **1⚡→0⚡**] | **0** | — | cada ciclo prepara hasta 1 Bendición | además +20 Carga NP al jugarla |

**El único cambio es la Encarnación a 0⚡, y arregla una trampa medida:** costaba 1⚡ y es de precepto **neutral**, así que el turno que la jugabas quedabas con 2⚡ y **el ciclo era imposible** — había que sacrificar un ciclo entero para encender el poder cuya única función es recompensar ciclos. A 0⚡ se juega *encima* del ciclo. Su mejora deja de ser `EnergyCost.UpgradeBy(-1)` (que a 0 es un no-op) y pasa a +20 Carga NP.
*Declarado:* es la firma `ITranscendenceCard` del mazo; a 0⚡ pierde la rebaja de coste de Infinite Upgrades. Aceptado.

### 6.1 La cuenta del turno 1

Mano de 5. Una mano inicial contiene los tres preceptos en **189/252 = 75 %** de los casos (composición sin cambios).

```
 3⚡ disponibles
 ─ Cielo (Arts ó Fortuna)     −1⚡ → +30 NP · innato +10 NP · Pagoda +10★
 ─ Pecho (Defensor)           −1⚡ →   5 Bloqueo · innato +4 Bloqueo · Pagoda +10★
 ─ Pies  (Buster ó Quick)     −1⚡ →  10 daño · innato +20★ · Pagoda +10★
 ─ CIERRE DE CICLO            +1⚡
 ─ Encarnación de Bishamonten  0⚡ → motor de Bendición encendido
 ─ 4.ª carta con el refund    −1⚡
 ═══════════════════════════════════════════════════════════════
 Jugadas en el turno 1: 5 cartas (hoy: 3, con 0⚡ sobrante el 100 % de las veces)
 Estado al cerrar T1: 40 Carga NP · 9 Bloqueo · 50★ · 10 daño
```

**50★ exactas = el crítico está pago.** El primer crítico deja de ser una tirada (hoy 51,6 % en T2) y pasa a ser una **garantía del turno 2** para cualquier mano que haya ciclado en T1.

---

## 7. Pool por rareza — 71 drafteables (23 C / 28 PC / 20 R)

*Números = punto de partida (knobs de playtest). Denominaciones 10/20/30/50/100 en todo el pool (4.6.3). Glow dorado en toda condicional; condición vacía = sin glow. **Ninguna carta cambia de precepto.***

### 7.1 COMUNES (23) — engranajes de conversión, conectividad **23/23 bajo criterio duro**

*Criterio duro: la carta lee o escribe un recurso propio (Doctrina, estrellas, Carga NP, Bloqueo como recurso). **Un rider de forma NO cuenta como conectividad** — es la misma trampa que «tiene tag». Bajo ese criterio, la propuesta ganadora estaba en 19/23 = 83 %; las cuatro filas marcadas ⟳ son las que se arreglaron.*

| Carta (eng) | Origen | Tipo | ⚡ | Prec. | Efecto | Mejora | Arq. |
|---|---|---|---:|---|---|---|---|
| Estocada Celeste (Celestial Thrust) | [REUSA] | Ataque Arts | 1 | C | 7 daño, +10 Carga NP; +10 más como Kenshin | 10, +20 | A |
| Lectura del Campo (Field Reading) | [REUSA re-efecto] | Hab. | 1 | C | Robá 2, descartá 1; **si descartás una carta etiquetada, +10★** | robá 3 | A |
| Oración a Bishamonten (Prayer to Bishamonten) | [REUSA re-efecto: **quita Agotar**] | Hab. | **0** | C | **Gastá 20★: +30 Carga NP** | +50 | A |
| Volver las Riendas (Turn the Reins) | [REUSA re-efecto: **quita Agotar**] | Hab. | **0** | C | **Gastá 50★: +50 Carga NP** | +50★→+70 NP | A |
| Orden de Batalla (Battle Order) | [REUSA] | Hab. | 1 | C | Robá la primera carta del mazo que podría avanzar tras esta | **+10 Carga NP** | A |
| Consejo del General (General's Counsel) | [REUSA] | Hab. | 1 | C | 5 Bloqueo, +10 Carga NP | 8, +10 | A/B |
| ⟳ Báculo del Comandante (Commander's Staff) | [REUSA re-efecto] | Ataque Arts | 1 | C | 8 daño, **+10 Carga NP**; +4 daño como Kenshin | 11 | D |
| ⟳ Armadura en el Pecho (Armour Is in the Chest) | [REUSA re-efecto] | Hab. | 1 | Pch | 7 Bloqueo; **si esto avanza Pecho, +10★**; 10 Bloqueo como Kenshin | 10 / 13 | B/D |
| Beber Entre Balas (Drink Among Bullets) | [REUSA re-efecto] | Hab. | 1 | Pch | 5 Bloqueo, 1 Débil; **si esto avanza Pecho, +10★** | 8, 1 | B |
| Guardia de Kasugayama (Kasugayama Guard) | [REUSA re-efecto] | Hab. | 2 | Pch | 15 Bloqueo; **si esto avanza la Doctrina, +10 Carga NP** | 20 | B |
| Coraza de Seis Placas (Six-Plate Cuirass) | [REUSA re-efecto] | Hab. | 1 | Pch | 6 Bloqueo, Retain; +3 si es tu primera Pecho del turno | 9 | B |
| Interponer la Lanza (Interpose the Spear) | [REUSA re-efecto] | Hab. | **1** | Pch | 5 Bloqueo; **si esto avanza la Doctrina, +10 Carga NP** | 8 | B |
| Sal para el Rival (Salt for the Rival) | [REUSA re-efecto] | Hab., aliado | 1 | Pch | 8 Bloqueo, quita **!RemoveWeak!** Débil, +10 Carga NP | 11, quita todo | B |
| Formación Cerrada (Closed Formation) | [REUSA **re-efecto total**: quita Agotar] | Hab. | **0** | Pch | **Gastá 20★: 8 Bloqueo** | 11 | B |
| **Muro de Lanzas (Spear Wall)** | **[NUEVA `SpearWall`]** | **Ataque** Buster | 1 | Pch | 5 daño, 5 Bloqueo; **si esto avanza Pecho, +10★** | 7/7 | B |
| **Muralla de Echigo (Echigo Rampart)** | **[NUEVA `EchigoRampart`]** | **Ataque** Buster | 1 | Pch | 6 Bloqueo, después daño = **¼ de tu Bloqueo, máx 8** | 9 / máx 11 | B |
| ⟳ Lanza de Ocho Pétalos (Eight-Petal Spear) | [REUSA re-efecto] | Ataque Buster | 1 | Pie | 9 daño; **+10★ si fue Crítica**; +4 como Kenshin | 12 | C/D |
| Carga de Houshoutsukige (Houshoutsukige Charge) | [REUSA re-efecto: **2⚡→1⚡**] | Ataque Buster | **1** | Pie | 4×3; **+10★ si fue Crítica** | 5×3 | C |
| Paso de la Victoria (Step of Victory) | [REUSA re-efecto: **+Agotar**] | Hab., Agotar | 0 | Pie | +20★ | +30 | C |
| Dar Vuelta a la Formación (Turn the Formation) | [REUSA re-efecto: **quita Agotar**] | Hab. | **0** | Pie | **Gastá 50 Carga NP: +50★** | +70★ | C |
| ⟳ Barrido de Naginata (Naginata Sweep) | [REUSA re-efecto] | Ataque Buster, área | 1 | Pie | 6 a TODOS, **+10★** | 9, +20★ | C |
| Ataque por Turnos (Alternating Attack) | [REUSA] | Ataque Quick | 1 | Pie | 4×2, +10★ | 6×2 | C |
| **Carga Estrellada (Starlit Charge)** | **[NUEVA `StarlitCharge`]** | **Ataque** Quick | **0** | Pie | **Gastá 20★: 8 daño** | 11 | C |

**Pecho deja de ser el único precepto sin Ataques** (0 en 22 cartas hoy): entran `Muro de Lanzas` y `Muralla de Echigo`, y con E8 **ya no queman la Bendición**.

### 7.2 POCO COMUNES (28)

| Carta (eng) | Origen | Tipo | ⚡ | Prec. | Efecto | Mejora | Arq. |
|---|---|---|---:|---|---|---|---|
| Estrategia de Rueda (Wheel Strategy) | [REUSA **re-efecto total**] | Hab. | **0** | C | **Gastá 30 Carga NP: robá 2** | robá 3 | A |
| Cuatro Golpes del Cielo (Four Heavenly Strikes) | [REUSA] | Ataque Arts | 1 | C | 3×3, +10 Carga NP | 4×3 | A/C |
| Preparar la Caballería (Prepare the Cavalry) | [REUSA] | Hab. | 1 | C | +20 Carga NP; Retain a 1 carta este turno | +30 | A |
| Carga Mágica (Magical Charge) | [REUSA] | Hab., Agotar | 0 | C | +30 Carga NP | +50 | A |
| Relevo de Formación (Formation Relay) | [REUSA] | Hab., Agotar | 1→0 | C | Recuperá del descarte 1 carta que podría avanzar tras esta | 0⚡ | A |
| Mirada de la Comandante (Commander's Gaze) | [REUSA] | Hab. | 1 | C | 2 Débil, +10 Carga NP | 3, +10 | A |
| Mandato a la Vanguardia (Vanguard Mandate) | [REUSA] | Hab., aliado | 1 | C | Un jugador roba 2; vos +10 Carga NP | robá 3 | A |
| Enfoque del Cielo (Heaven's Focus) | [REUSA] | Hab., Agotar | 1 | C | 1 Crítico Listo, +10 Carga NP | +20 | A/C |
| Armadura en el Pecho A (Armour in the Chest A) | [REUSA] | Hab., Agotar | 1 | Pch | 1 Intangible, +20 Carga NP | +30 | B |
| Cortina de Disparos (Bullet Curtain) | [REUSA] | Hab. | 1 | Pch | 9 Bloqueo; el próximo Ataque enemigo del turno que no te dañe: +20★ | 12/+30 | B/C |
| Defensa del Ruler (Ruler's Defense) | [REUSA] | Hab. | 2 | Pch | 14 Bloqueo, 1 Artifact | 18 | B |
| Contraataque Sereno (Serene Counterattack) | [REUSA] | Hab. | 1 | Pch | 7 Bloqueo; el próximo atacante del turno recibe 6 | 9/9 | B |
| Pecho sin Temor (Fearless Chest) | [REUSA] | Poder | 1 | Pch | Cada avance de Pecho: +2 Bloqueo | +3 | B |
| Tesoro en el Corazón B (Treasure in the Heart B) | [REUSA] | Hab., Agotar | 1 | Pch | 2 Artifact, +10 Carga NP | 3 Artifact | B |
| **Guardia Compartida (Shared Guard)** | [REUSA **re-efecto total**] | Hab. | 1 | Pch | 7 Bloqueo, después daño = **⅓ de tu Bloqueo, máx 12**; cada otro jugador gana 4 Bloqueo | 10 / máx 16 | B |
| Muro de Estandartes (Wall of Banners) | [REUSA re-efecto: **2⚡→1⚡**] | Hab. | **1** | Pch | 9 Bloqueo; si otro precepto avanzó este turno, +20★ | 12/+30 | B/C |
| Camino del Justo (The Just Path) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | Pch | Fin de turno: si avanzaste ≥2 preceptos, 6 Bloqueo | 8 | B |
| El Mérito Está en los Pies A (Merit Is in the Feet A) | [REUSA **re-efecto**] | Hab., Agotar | 2 | Pie | **+1 Fuerza (aliados +1), +30★** | **+50★** | C |
| Galope de Houshoutsukige (Houshoutsukige Gallop) | [REUSA re-efecto] | Ataque Quick | 1 | Pie | 4×3, +10★; como Kenshin, 4×4 | 5×3, +20★ | C/D |
| Ocho Armas, Una Guerrera (Eight Weapons, One Warrior) | [REUSA re-efecto: **2⚡→1⚡** + **fix**] | Ataque Buster | **1** | Pie | **7×2**; +10★ por precepto distinto avanzado este turno, **contando ÉSTE** | 9×2 | C |
| Naginata Giratoria (Spinning Naginata) | [REUSA] | Ataque Buster, área | 2 | Pie | 8 a TODOS, +10★ | 11, +20★ | C |
| Persecución Incansable (Relentless Pursuit) | [REUSA] | Ataque Buster | 1 | Pie | 11; +5 si el objetivo no tiene mejoras | 14/+7 | C |
| Asalto Alternado (Alternating Assault) | [REUSA] | Ataque Quick | 1 | Pie | 3×3; si fue Crítica, +20 Carga NP | 4×3/+30 | C |
| Retroceder es el Infierno (Retreat Is Hell) | [REUSA re-efecto: **2⚡→1⚡**] | Ataque Buster | **1** | Pie | 12; al matar +30★ | 16/+50★ | C |
| Cabalgata C (Riding C) | [REUSA] | Poder | 1→0 | Pie | La primera Quick normal de cada turno: +10★ | 0⚡ | C |
| Pisadas del Ejército (Army Footsteps) | [REUSA — **fix de loc**] | Hab. | 1 | Pie | +20★; robá la primera Pies del mazo que podría avanzar tras esta | +30★ | C |
| Doctrina del General (General's Doctrine) | [REUSA] | Poder | 1 | — | La primera carta etiquetada que falla cada turno: 3 Bloqueo | 5 | todos |
| Divinidad C → A (Divinity C to A) | [REUSA re-efecto: **2⚡→1⚡** + **cap**] | Poder | **1** | — | **El primer impacto de UN Ataque por turno** +3; +5 como Kenshin | +4/+6 | D |

⚠️ **`EightWeaponsOneWarrior` hoy es 10×2** (`DamageVar(10m)`, `Hits 2`). Pasar a 7×2 es **−30 % de daño base**, no solo un cambio de coste: va al changelog como delta explícito (§16.7).
⚠️ **`Mérito A` deja de dar +2/+3 de Fuerza** (pasa a +1, mejora en estrellas). Es el parche estructural que hace que la auditoría de pico cierre (§14).

### 7.3 RARAS (20)

| Carta (eng) | Origen | Tipo | ⚡ | Prec. | Efecto | Mejora | Arq. |
|---|---|---|---:|---|---|---|---|
| Llama Blanca A (White Flame A) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | C | Inicio de turno +10★; el primer avance de Cielo de cada turno +10 Carga NP | +20★ | A |
| Pagoda Enjoyada C (Jeweled Pagoda C) | [REUSA] | Hab., Agotar, aliado | 1 | C | +1 Fuerza y +2 niveles de OC al objetivo; vos +20 Carga NP | +2 Fuerza/+30 | A |
| Ocho Formaciones de Bishamonten (Eight Formations) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | C | La primera carta etiquetada de cada turno que fallaría, avanza ignorando el orden | 0⚡ | A |
| Sabiduría de 84.000 Enseñanzas (Wisdom of 84,000) | [REUSA] | Hab., Agotar | 2 | C | Robá 4, +20 Carga NP | 1⚡ | A |
| Voto de Bishamonten (Vow of Bishamonten) | [REUSA] | Hab., Agotar | 1 | C | +50 Carga NP, 1 Artifact | 2 Artifact | A/B |
| Blanca Llama, Fría y Ardiente (White Flame, Cold and Burning) | [REUSA] | Ataque Arts | 2 | C | 6×3, +20 Carga NP, después 2 Vulnerable | 8×3, +30 | A |
| Dos Evasiones del Ruler (Two Ruler Evasions) | [REUSA] | Hab., Agotar | 2 | Pch | 2 Buffer | 3 | B |
| El Tesoro Está en el Corazón (Treasure Is in the Heart) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | Pch | Tras avanzar Pecho, el próximo debuff del turno se evita y da +10 Carga NP | +20 | B |
| Enviar Sal al Enemigo (Send Salt to the Enemy) | [REUSA] | Hab., Agotar, aliado | 1 | Pch | Curá 6, +12 Bloqueo | 9/16 | B |
| Murallas de Kasugayama (Walls of Kasugayama) | [REUSA] | Hab., Retain | 2 | Pch | 20 Bloqueo | 26 | B |
| Juez del Campo (Field Judge) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | Pch | La 1.ª vez por turno que un enemigo gana una mejora: +8 Bloqueo, +10 Carga NP | +12/+20 | B |
| Sorbo en el Centro del Ejército (Sip at the Center) | [REUSA] | Hab., Agotar | 1 | Pch | 1 Intangible, +20★, robá 1 | +30★, robá 2 | B |
| Biten: Formación de Rueda (Biten: Wheel Formation) | [REUSA **re-efecto**] | Ataque Quick | 2 | Pie | **2×6**, +20★ | **+30★** (no sube impactos) | C |
| Hoja Shiranui (Shiranui Blade) | [REUSA] | Ataque Buster | 2 | Pie | 18; Kagetora +20★ después, Kenshin quita el Bloqueo antes | 24/+30★ | D |
| Galope Total de Houshoutsukige (Full Houshoutsukige Gallop) | [REUSA re-efecto: **3⚡→2⚡**] | Ataque Buster, área | **2** | Pie | 5×3 a TODOS | 6×3 | C |
| Kawanakajima | [REUSA] | Ataque Buster | 2 | Pie | 20; +8 contra Elite/Jefe | 26/+10 | D |
| Ocho Armas Desatadas (Eight Weapons Unleashed) | [REUSA] | Ataque Quick | 1 | Pie | 4×4, +20★ | 5×4 | C |
| La Victoria Está en los Pies (Victory Is in the Feet) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | Pie | El primer Crítico de cada turno reembolsa 20★ | además +10 Carga NP | C |
| Fortuna, Armadura y Mérito A (Fortune, Armour and Merit A) | [REUSA re-efecto: **2⚡→1⚡** + **fix de contrato**] | Hab., Agotar | **1** | — | Elegí un precepto incompleto: avanza ignorando el orden. 1 Buffer | 0⚡ | todos |
| Manifestación de Bishamonten (Manifestation of Bishamonten) | [REUSA re-efecto: **2⚡→1⚡**] | Poder | **1** | — | Los próximos 3 ciclos completos dan +1 Fuerza (**tope acumulado +3**) | 0⚡ | D |

### 7.4 Especiales / no drafteables (`CardRarity.Event`)

`BitenHassouKurumaGakariNoJin`, `BitenHassouShiranui` (los dos NP, re-efecto numérico en §9), `ChooseHeaven`, `ChooseChest`, `ChooseFeet` — todas [REUSA]. **No se borran**: `FortuneArmourAndMeritA` las instancia por `ModelDb.Card<>` en runtime; borrarlas tira `ModelDb` nulo.

---

## 8. Reliquias (12 + 1 opcional)

| Reliquia (eng) | Origen | Slot | Efecto |
|---|---|---|---|
| **Pagoda Enjoyada de Bishamonten** | [REUSA **re-efecto**] | **Starter** | **Cada avance de la Doctrina: +10★** → **exactamente 3 procs/turno** (4.6.4) **sin un solo contador nuevo**: el cap lo garantiza `MaxAdvancesPerTurn`. **Sin robo** (P-2). Hoy: 1 proc/turno = ⅓ del caudal contra el que se calibró el pool |
| Gran Pagoda de Bishamonten | [REUSA **re-efecto**] | Ancient (Orobas) | Igual, **y al completar un ciclo: robá 1 y +10 Carga NP**. Reinstala forma/Doctrina/CommandBonus como hoy (contrato DECISIONS). El robo vive acá: es reliquia de jefe, está pagada |
| Brasero de Llama Blanca | [REUSA **re-efecto**] | Rara | Al ascender: **+1⚡ este turno, +30★ y +50 Carga NP**. Pico acotado de un turno, no un techo nuevo |
| Estandarte de Ocho Pétalos | [REUSA] | Común | El primer avance de cada turno: **+2 Bloqueo** *(el valor real de hoy; la propuesta lo subía a 3 sin declararlo)* |
| Copa de Sake | [REUSA] | Común | Turno 1: robá 1, descartá 1 |
| Riendas de Houshoutsukige | [REUSA] | PC | La primera Pies de cada turno: +10★ |
| Armadura de Seis Placas | [REUSA] | PC | La primera **carta de Pecho** de cada turno: +4 Bloqueo ⚠️ ver §16.6 |
| Tachi Shiranui | [REUSA] | Rara | El primer Crítico de cada turno: +10 Carga NP |
| Saco de Sal de Echigo | [REUSA] | Tienda | Inicio de combate: 1 Artifact a todos; +10 Carga NP por aliado |
| Juramento de Echigo | [REUSA] | Bond | Sin cambios |
| Registro de las Ocho Formaciones | [REUSA] | NP store | Sin cambios |
| Grial de la Comandante | [REUSA] | Grial (evento) | Sin cambios |
| *Tambor de Guerra de Bishamonten* | ***[NUEVA] — NO se implementa en este pase*** | PC | *Sustituida por la contingencia §15.3, que reusa `GeneralsDoctrinePower` sin ID nuevo* |

---

## 9. Noble Phantasms

**Kagetora — 毘天八相車懸りの陣 (`BitenHassouKurumaGakariNoJin`)** — [REUSA, sin cambios de estructura]. 8 impactos, `perHit = 3 + Lv`, limpieza de mejoras ofensivas **tras** el daño, Débil 1/2/3 por OC, ascenso al final. El OC compra Débil, no daño: banquear en vez de disparar sigue siendo una decisión legítima.

**Kenshin — 毘天八相・不知火 (`BitenHassouShiranui`)** — [REUSA **con re-efecto**]:

| | Hoy | Rediseño |
|---|---|---|
| Impactos | **4** | **8** |
| `perHit` | `5 + 2·Lv + OC(0–4) + Man(3)` | **`3 + Lv + OC`**, con **OC = +1 por impacto a partir de OC3 (máximo +1)** |
| Anti-Man | +3/impacto | **+3/impacto, conservado como sabor** |
| Antes del daño | quita mejoras ofensivas | quita mejoras ofensivas **y el Bloqueo del objetivo** |

**Por qué 8 impactos:** todos los aditivos del kit son **por impacto** (`StrengthPower`, Bendición, Divinidad en el primero). Con 4 impactos, Kenshin cobraba la mitad de su propio mazo y hacía **69** donde Kagetora hacía **91**. Línea de control recalculada con la Bendición capada (E8) y NP3 / +3 Fuerza / Bendición / Divinidad / objetivo no-Man:

| | Hoy | Rediseño |
|---|---:|---:|
| Kagetora, OC1 | 91 + Débil | **85 + Débil** *(baja por el cap de Bendición — nerf declarado)* |
| Kenshin, OC1 | **69 (−24 %)** | **85**, y limpia + quita Bloqueo **antes** del daño |
| Kenshin, OC3 | 77 | **93** |
| Kenshin, OC5 | — | **93** *(el OC topea en +1)* |

**El anti-Man queda como sabor, no como plan:** `FgoAttributes` mapea `Elite→Earth` y `Boss→Heaven` — contra los enemigos que justifican un ulti nunca se activa. La ventaja real y siempre activa de Kenshin contra jefes es **quitar el Bloqueo antes del daño**, que es exactamente lo que ya hace su rider en `ShiranuiBlade`: coherente y **sin tocar FGOCore**.

**No se abre ventana-NP.** Es convención minoritaria (solo Tiamat), y aritméticamente apila una segunda fuente de energía **en el turno del pico**, que es justo el turno que la auditoría §14 deja al borde. Prohibida para Kagetora.

---

## 10. Lista DEMOTE

### **DEMOTE: ninguno.** Cero IDs quemados.

Todo problema medido en `DIAGNOSTICO §3.3` es reparable por re-efecto, y un demote quema el ID para siempre (misma resolución que el panel de Morgan, §8).

| Candidata | Problema medido | Resolución |
|---|---|---|
| `WheelStrategy` | dominada estrictamente por `VanguardMandate` (misma rareza, coste, precepto, robo; NP incondicional y targeteable) | **re-efecto total** → 0⚡, gate 30 NP, conversión NP→cartas. Llena la única arista que faltaba del grafo |
| `SharedGuard` | idéntica a `ArmourIsInTheChest` en solitario | **re-efecto total** → la salida grande Bloqueo→daño (⅓, máx 12) |
| `ClosedFormation` | 0⚡ con Agotar y lectura muerta de `.Block` | **re-efecto total** → conversión 20★→8 Bloqueo, sin Agotar. Ocupa el hueco que dejó `FrontArmour` al cortarse |
| `FullHoushoutsukigeGallop` | 3⚡ = el turno entero ⇒ cancela el ciclo; peor por energía que una común | **2⚡** |
| `EightWeaponsOneWarrior` | lee el mask **antes** de su propio avance ⇒ techo real +10 contra los +30 declarados | **fix** (cuenta el propio precepto) + 1⚡ + 7×2 |
| `MeritIsInTheFeetA` | +2/+3 Fuerza sobre 19-21 impactos por turno: el motor del pico roto | **re-efecto** → +1 Fuerza, la mejora va a estrellas |
| `ChooseHeaven/Chest/Feet` | «sin uso» | **NO se borran**: `FortuneArmourAndMeritA` las instancia en runtime |

---

## 11. Bugs: lo que ya está cerrado y lo que sigue vivo

### 11.1 Regla de higiene

Las tres propuestas escribieron su ledger contra el diagnóstico en vez de contra el árbol. **La tabla de bugs se re-deriva de `git log` antes del primer lote.**

### 11.2 YA CERRADOS en `HEAD` — no volver a escribirlos

Commits `6f3b1d29` y `42295b7b`: `WasUsed` con `?? 0` (los 12 efectos muertos), guard `amount <= 0` de la Ventana del Tesoro, `JustPathPower ?? 0`, `WallOfBanners ?? 0`, `Flash()` antes del early-return de la Pagoda, tutor de `ArmyFootsteps` con `WouldAdvanceAfter`, «Draw 0.» oculto y los 4 spans dorados de zhs.
**Si `audit_simpleloc` sale rojo, es una regresión de este lote, no el bug del reporte.**

### 11.3 ABIERTOS — obligatorios en este pase

| Sev | Qué | Dónde | Fix |
|---|---|---|---|
| P2 | `SALT_FOR_THE_RIVAL.description` dice «removes Weak» **sin cantidad**; el código quita 1 | loc ×5 | token `!RemoveWeak!` |
| P2 | **`ARMY_FOOTSTEPS.description` quedó desincronizada del fix ya aplicado**: el código usa `WouldAdvanceAfter`, la loc en los 5 idiomas sigue diciendo *«Draw the first Feet card from the Draw Pile»* — texto que miente, en la carta cuyo problema era exactamente ése | loc ×5 | reescribir a «robá la primera carta del mazo que podría avanzar después de ésta» |
| P2 | **zhs `INCARNATION_OF_BISHAMONTEN`: `*毗沙门天的*加护`** — el segundo `*` **cierra** el span y deja 加护 (Bendición) sin dorar. Es una carta del mazo inicial, en el idioma del reporter | `zhs/cards.json` | `*毗沙门天的加护*`; revisar de paso el eng `*Bishamonten's *Blessing` |
| P2 | `EightWeaponsOneWarrior` lee `AdvancedMaskThisTurn` antes de su propio avance | `UncommonCards.cs:254` | contar el propio precepto |
| P3 | `IncarnationOfBishamonten` a 1⚡ hace imposible el ciclo del turno que la jugás | `StartingCards.cs` | 0⚡ |
| Deuda | `TreasureWindowPower._prevented` es un flag «una vez por turno» en campo privado (viola `DECISIONS:79-82`) | `RarePowers.cs` | migrar al mask de `KagetoraUsagePower` |
| Deuda | `FortuneArmourAndMeritA` **muta `KagetoraCard.Precept` en runtime** | `RareCards.cs:238` | el precepto elegido se guarda en `ForcedDoctrineAdvancePower`, no en la carta |
| Deuda | `DoctrinePower` no implementa `IResourcePower` mientras sus satélites sí | `Doctrine.cs` | E7 |
| Nota | `_activePlay` / `_active` / `_usedHit` / `_card` **no** se migran: son **ligaduras de `CardPlay`**, no flags por turno. `DECISIONS:83-84` exige hooks de cálculo puros; migrarlas rompería esa pureza (`ModifyDamage*` corre varias veces por preview). **Excepción declarada y documentada en DECISIONS.** *(El nuevo cap de Divinidad sí usa un bit de turno: ver §16.3.)* |
| Proceso | Tres bugs de la familia «comparación nullable levantada sin `?? 0`» (Kagetora, Astolfo, `JustPathPower`) | — | **hook/analizador que falle el build ante `?.X <op> literal` sin coalescencia**. Regla en prosa = se pierde en el ruido |

---

## 12. Registro de decisiones del panel

### 12.1 Ganadora

**Propuesta 1 — MOTOR Y ECONOMÍA, 3–0.** Motivos convergentes de los tres jueces:

- Es la única que responde la pregunta 1 del diagnóstico **en sus propios términos**: el diagnóstico ofreció tres vías (+1⚡ base / densidad de 0⚡ / el ciclo devuelve energía) y **midió** que la primera no mueve la tasa de ciclo. P2 eligió la primera disfrazada de reliquia; P3 la dejó como knob de emergencia y no entregó presupuesto. P1 eligió (c) como eje y (b) como soporte, y **la cuenta cierra**.
- Es la única con **contabilidad de pool auditable**: dos jueces recontaron sus 72 filas contra su propia tabla de costos y coincidieron en las cuatro filas y las tres columnas.
- Es la única con una **prueba anti-loop estructural verificable**, y los tres la verificaron: `Doctrine.cs:84` y `:131` evalúan el tope **antes** de los overrides ⇒ ≤1 ciclo/turno sin estado nuevo.
- Es la única cuyas **promesas de UI cierran todas contra el decompilado**, y encima rechaza por nombre las dos que no.
- **No toca `Doctrine.cs`.** P2 lo reescribe mientras firma que no.

**Ganó con la nota de potencia más baja del panel.** Su auditoría de pico estaba mal por tres motivos independientes y su regla E5 la violaban sus propias cartas. Por eso los parches mandan.

### 12.2 Parches obligatorios aplicados, por juez

| Juez | Parche | Dónde quedó |
|---|---|---|
| **J1** | P-1 Interponer la Lanza no queda 0⚡ con valor neto | §7.1 (queda 1⚡, ver §12.3-4) |
| J1 | P-2 el cierre paga ⚡ **o** carta, nunca las dos; el robo se muda a la Gran Pagoda | E3, §8 |
| J1 | P-3 fricción real en los 0⚡ (precios arriba) | Oración 20★, Estrategia de Rueda 30 NP |
| J1 | P-4 cap duro de **un crítico por turno** | E6 |
| J1 | P-5 cap de Divinidad a 1 Ataque/turno | §7.2, §16.3 |
| J1 | P-6 **Kenshin NO recibe +1⚡ permanente** | §5 |
| J1 | P-7 re-auditar el pico con la semántica real del motor | §14 |
| J1 | P-8 Biten mejora 3×8 → +30★ | §7.3 (y además 2×8 → 2×6, §14) |
| J1 | P-9 el techo de 100★ entra en el diseño | §14.3, §15.1-4 |
| J1 | P-10 ordinales I/II/III | §4.3 |
| J1 | P-11 adoptar «Descartado, con la evidencia» | §12.5 |
| J1 | P-12 toda contingencia con línea de referencia de la forma base + saturación defensiva | §14.2, §14.4 |
| J1 | P-13/14/15 purgar el ledger, corregir «un crítico por turno está garantizado» y la línea de control | §11.2, E6, §9 |
| J1 | P-16 restricción 6 en ⚠️ hasta re-simular | §13 |
| **J2** | K1 la Bendición solo se arma en Ataques de Pies/sin precepto | E8 |
| J2 | K2 documentar que los Ataques de Pecho dejan de ser trampa | E8, §16.6 |
| J2 | K3 rehacer la auditoría de pico con la energía real | §14 |
| J2 | K4 aplicar E5 literalmente, auditando los 0⚡ uno por uno | §3.4, §7 |
| J2 | K5 **`MaxAdvancesPerTurn` no se sube a 4 jamás** (overflow de 2 bits) | §15.4, §16.4 |
| J2 | K6 ordinales I/II/III | §4.3 |
| J2 | K7 usar `IAddDumbVariablesToPowerDescription` para el texto dinámico | §4.2 |
| J2 | K8 adoptar §9 de P3 textual | §12.5 |
| J2 | K9 corregir E6 y salvar `Enfoque del Cielo` (`CritReady` primero) | E6 |
| J2 | K10 prohibir contar riders de forma como conectividad; arreglar las 4 comunes | §7.1 (filas ⟳) |
| J2 | K11 Bloqueo con ≥2 salidas en COMUNES | Muralla de Echigo (común) + Formación Cerrada; Guardia Compartida como salida grande |
| J2 | K12/K13/K14 ledger corregido, 2 bugs nuevos, deltas silenciosos al changelog | §11 |
| J2 | K15 «Refuerzo» como contingencia #1 declarada | §15.3 |
| J2 | K16/K17 no `MaxEnergy`, no ventana-NP, no 3.er medidor; re-testear contra `HEAD` | §3.3, §9, §12.5, §16.1 |
| **J3** | J-01 rehacer el pico con 2 críticos, Divinidad por Ataque y Fuerza real; contingencias a-d | §14 (y el cap de Fuerza, §12.3-5) |
| J3 | J-02 los tres ceros que rompen E5 | §7 (Estrategia 30 NP, Interponer 1⚡, Orden de Batalla sin mejora a 0⚡) |
| J3 | J-03 firma exacta de `CanSpendCritical`, incoloras conservan el crítico | E6 |
| J3 | J-04 re-derivar §6.2 desde `git log` | §11 |
| J3 | J-05 glow con guarda de null completa | §4.5 |
| J3 | J-06 prohibido el medidor y el rename del HUD | §12.5 |
| J3 | J-07 la retención de Bloqueo se muda de la carta a `KenshinFormPower : IBlockRetentionSource`, cap 5 | §5 |
| J3 | J-08 pool 72 → 71: se corta `FrontArmour`, su función va a `ClosedFormation` | §7.1, §10 |
| J3 | J-09 auditar colisiones de re-tipado | §16.6 |
| J3 | J-10 tres caps separados para Bloqueo→daño | §7.1/§7.2 (¼ máx 8 · ⅓ máx 12) |
| J3 | J-11 Pagoda: E3 tal cual, el ⚡ innato al ciclo | E1, E3 |
| J3 | J-12 knob prohibido | §15.4 |
| J3 | J-13 ventana-NP prohibida | §9 |
| J3 | J-14 contratos §3.6 | §11.3 |
| J3 | J-15 Refuerzo sobre `GeneralsDoctrinePower`, sin ID nuevo | §15.3 |
| J3 | J-16 publish solo Kagetora, changelog, VFX validados, re-simular | §16 |
| J3 | J-17 rechaza los ordinales | **contradicción, ver §12.3-2** |

### 12.3 Contradicciones entre jueces — resueltas al más restrictivo

| # | Tema | Posiciones | **Resolución (más restrictiva)** |
|---|---|---|---|
| 1 | **Robo al cerrar ciclo** | J1 P-2: sacarlo del starter, solo en la Gran Pagoda. J3 J-11: adoptar E3 tal cual (con robo) | **J1.** La Pagoda base **no roba**. Un ciclo repetible que da ⚡ **y** carta es la bandera roja n.º 1 del rúbrico; el robo sobrevive en la reliquia de jefe, que está pagada. Coste: se pierde el breakpoint «robá 1» y hay que reescribir el texto del starter |
| 2 | **Ordinales I/II/III** | J1 P-10 y J2 K6: obligatorios (mayor palanca contra 「看不明白」). J3 J-17: rechazados (estáticos, 345 strings, **mienten bajo Kenshin**) | **Entran, en la forma que no puede mentir.** 2 de 3 jueces los hacen obligatorios y son el único canal sobre el objeto que el jugador mira; la objeción de J3 es factual y se neutraliza haciendo que el tooltip de la Doctrina y la `smartDescription` de Kenshin digan explícitamente que **como Kenshin el orden es libre**. Se declaran el revert más barato del documento (loc pura, un commit) |
| 3 | **+1⚡ permanente de Kenshin** | J1 P-6: eliminarlo. J2 K3 y J3 J-13: conservarlo y auditar a 5⚡ | **J1.** Es la fuente aritmética del pico de 257-267 y el mismo regalo incondicional que el diagnóstico midió como inefectivo. El paquete de ascensión se paga con retención de Bloqueo, 20 NP/ciclo y el NP de 8 impactos |
| 4 | **`InterposeTheSpear`** | J1 P-1: 0⚡ gastando 10 NP. J2 K4: 1⚡ **o** 0⚡ con gasto. J3 J-02: **vuelve a 1⚡** | **J3.** Vuelve a 1⚡. Pecho conserva un 0⚡ repetible (`ClosedFormation`, 20★→8 Bloqueo). Coste: la densidad de 0⚡ baja de 10 a 8 (11,3 %), todavía en el pelotón del roster |
| 5 | **Cómo bajar el pico** | J1: Bendición, OC, Divinidad, Biten. J2: rehacer a 5⚡. J3 J-01: + Mérito A mejorada a +2 Fuerza | **Todas, más una propia y más restrictiva:** el problema real es que **la Fuerza multiplica ~20 impactos por turno**. Se aplica el cap de Manifestación (+3 total) **y** `MeritIsInTheFeetA` deja de escalar Fuerza (+1, mejora en estrellas). Sin esto, ninguna combinación de las contingencias de los tres jueces baja de 245 |
| 6 | **Válvula del gate de crítico** | J2 K9: `CritReadyPower`. J3 J-03: «o si tenés 20+ de Bloqueo» | **Las dos son legales; entra solo la de J2** (`CritReady`), que además **arregla un bug real** (`Enfoque del Cielo` era una trampa). La de J3 **amplía** el acceso ⇒ queda como knob §15.4-7 |
| 7 | **Pagoda a 20★/turno (P-9)** | J1 P-9: procs solo en los avances 1 y 3, ya. Regla 4.6.4: starter = **cap 3 procs/turno** | **Se conserva 3 procs** (bajar a 2 violaría 4.6.4) y se aplica la **segunda rama** que el propio P-9 ofrece: **salidas visibles del banco** — 4 sumideros de estrellas en comunes. Con el cap de un crítico por turno, ingreso 50★ ≈ gasto 50★: el banco de 100 no desborda en estado estacionario. El escalonado queda como knob §15.4-2 |
| 8 | **`Muralla de Echigo`** | P1: PC, mitad del Bloqueo máx 14. J2 K11: **común**, ¼ máx 8. J3 J-10: tres caps separados | **J2 + J3.** Común a ¼ máx 8, y la salida grande se muda a `SharedGuard` (PC, ⅓ máx 12). Los tres caps se recalibran **juntos**, nunca de a uno |

### 12.4 Injertos de las propuestas perdedoras

- **De P3 (presentación):** los ordinales I/II/III (§4.3); la sección «Descartado, con la evidencia» completa (§12.5); la retención de Bloqueo como delta de forma (§5); los tres caps separados de Bloqueo→daño (§7); la disciplina de auditoría — línea de referencia de la forma base + chequeo de saturación defensiva (§14.2, §14.4); la validación de rutas VFX contra `grep '"vfx/' decompiled/` **antes** de escribirlas (§16.9); la auditoría de colisiones de re-tipado (§16.6); 0 IDs nuevos como presión de producción (bajamos de 4 a 3).
- **De P2 (draft):** el cap de la Bendición por impactos (E8); el cap de Divinidad por turno (§7.2); el «Refuerzo» como contingencia declarada sobre `GeneralsDoctrinePower` (§15.3); la métrica de playtest para R2 (ratio de ciclos/turno de un mazo balanceado contra uno 3:1:0).

### 12.5 Descartado, con la evidencia de por qué *(sección obligatoria — injerto de P3, adoptada entera)*

1. **Renombrar al personaje en el HUD al ascender.** `CharacterModel.Title` es `public LocString Title => new LocString("characters", Id.Entry + ".title")` — **no virtual**, derivado del Id. Una «segunda entrada en `characters.json`» crea **otro personaje seleccionable**, no renombra. Además, el único sitio del combate que lo renderiza es la ficha expandida de **multijugador**. Exigiría un patch Harmony (hoy `grep HarmonyPatch KagetoraLancer` = **0**) para un canal que en solitario no se ve. **Compensación única:** `KENSHIN_FORM_POWER.title` → 上杉謙信 / Uesugi Kenshin en los 5 idiomas.
2. **Cambiar el icono del poder de la Doctrina según el precepto esperado.** `NPower.Reload()` — lo único que setea `_icon.Texture` — solo corre al asignar el modelo o al entrar al árbol; `DisplayAmountChanged` refresca **el número, no la textura**; `PowerModel.PackedIconPath` no es virtual (BaseLib lo intercepta una vez). **Imposible sin patch.** Por eso el canal es el número (que sí se refresca) más el texto vivo del hover (que sí se reconstruye).
3. **Tercer medidor en la fila de RitsuLib.** `FgoSecondaryResources.RegisterCombatMeters` es **`private static`** dentro de FGOCore: no es invocable desde un mod de personaje. Tocarlo rompe «FGOCore intocado» y obliga a republicar los 12 mods. Un registry propio crearía una **segunda fila** anclada sobre `EnergyCounterContainer`, sin precedente y con riesgo de superponerse a la fila de NP/Estrellas (el código ya tiene un latch `_positioningBroken` porque eso se rompió una vez). **Descartado**; reserva declarada si el playtest dice que el contador no alcanza.
4. **Subir `MaxEnergy` a 4.** Es `virtual` y sería una línea, pero el diagnóstico **midió** que no cambia la tasa de ciclo (6,50 → 6,50): premia igual al que no cicla y rompe la paridad de 3⚡ del roster. **Descartado como diseño**; sobrevive como knob de emergencia.
5. **Ventana-NP.** Apila una segunda fuente de energía en el turno del pico. Prohibida.
6. **VFX propios de Doctrina / Bendición / ascensión / NP.** 0 archivos de audio, los dos NP comparten `vfx_dramatic_stab`. **Sigue sin hacerse**: se declara no hecho, no se promete por tercera vez.

---

## 13. Verificación de restricciones duras, una por una

| Restricción | Estado | Evidencia |
|---|:--:|---|
| **1 · IDs inmutables + [REUSA]/[NUEVA]/[DEMOTE] por carta** | ✅ | 0 renombres · 0 borrados · **0 DEMOTE** · **3 IDs [NUEVA]** (`SpearWall`, `EchigoRampart`, `StarlitCharge`) · 0 reliquias nuevas. Todo lo demás es re-efecto sobre IDs publicados (número, coste, tipo, texto), save-safe por `ModelIdRunSaveConverter`. Mod id `KagetoraLancer` intacto. `ChooseHeaven/Chest/Feet` **no se borran** (las instancia `FortuneArmourAndMeritA` en runtime) |
| **2 · Se conservan: dos formas, ascenso irreversible, tres preceptos + glosario 天/胸/足, NP y Estrellas de FGOCore, motor de `Doctrine.cs`** | ✅ | **Ninguna carta cambia de precepto** ⇒ las 69 etiquetas × 5 idiomas se conservan (solo se les antepone el ordinal). `AfterCardPlayed` no se toca: timing, golpe letal, reentrancia, copias y reset intactos. Ascensión idempotente por doble candado (`NoblePhantasms.cs:67` + `FormSwitch.IsPermanent`) preservada |
| **3 · NO tocar FGOCore** | ✅ | **Cero archivos de FGOCore modificados.** Todo lo usado ya existe y es público: `ICriticalAccessRule`, `IBlockRetentionSource`, `FormPower`, `AfterEnergyReset`, `NpCharge`, `CritStars`, `FgoAttributes`, `ManifestCards`. **Sin tercer medidor** (`RegisterCombatMeters` es privado). Los 12 personajes **no** se republican |
| **4 · Reglas 4.6** | ✅ | **4.6.1** básicas exactas (Buster 10 / Arts 6+30 / Quick 6+30★), QAABB · **4.6.2** conectividad de comunes **23/23 bajo criterio duro** (riders de forma **no** cuentan) + par espejo a 0⚡ **sin Agotar** (Volver las Riendas ↔ Dar Vuelta a la Formación) · **4.6.3** denominaciones 10/20/30/50/100 en todo el pool · **4.6.4** starter = motor con **exactamente 3 procs/turno**, garantizados por `MaxAdvancesPerTurn` · **4.6.5** glow dorado en toda condicional vía `KagetoraCard`, condición vacía = sin glow · **4.6.6** los poderes engordan hilos existentes · techo auditado en §14 · multi-hit anti-Buffer en las 3 rarezas · el pool no depende de debuffs |
| **5 · Tamaño de pool comparable, no reducir** | ✅ | **71 drafteables (23 C / 28 PC / 20 R)** contra 68 (20/28/20). Preceptos: 21 天 / 24 胸 / 22 足 / 4 neutrales |
| **6 · Sin loops deterministas de energía o robo neto positivo** | ⚠️ **declarado** | El refund es **≤1/turno por construcción** (`Doctrine.cs:84` y `:131`, tope antes de los overrides). La Pagoda base **no roba**; el único robo repetible es el anti-atasco de Kagetora (1/turno, condicionado a estar trabada). Los 6 ceros repetibles **gastan más de lo que su avance devuelve** (auditoría §3.4: la mano de tres 0⚡ da −10★ y −3 cartas por +1⚡); los 2 restantes llevan Agotar. **Se firma ⚠️, no ✅, hasta la re-simulación con el pool nuevo** (P-16) |
| **7 · Estado por turno vía powers visibles / `IResourcePower`** | ✅ | `DoctrineTurnStatePower` y `KagetoraUsagePower` ya son `IResourcePower` con reset en `BeforeSideTurnStart` + `participants.Contains(Owner)`. Se agregan `DoctrinePower : IResourcePower` (E7), el bit de crítico y el de Divinidad al `PerTurnMask`, y se migran `TreasureWindowPower._prevented` y el precepto elegido de `FortuneArmourAndMeritA`. Las ligaduras de `CardPlay` quedan como **excepción documentada** (§11.3) |
| **8 · Los tres reclamos del reporter** | ✅ / ⚠️ | 「太缺费了」 ✅ estructural (§3.3) · 「按顺序推进游戏内有点看不明白」 ✅ cinco canales (§4) · 「意义不明」 (ascensión) ✅ paquete de §5 · ⚠️ **la convergencia 1:1:1 (R2) se mitiga, no se elimina** (§2) |

---

## 14. Auditoría de pico

**Techo objetivo: 180-220 daño/turno** (DECISIONS + `DESIGN §14.3`).

### 14.1 Semántica del motor usada (los tres errores que hundieron las tres auditorías originales)

1. **`CriticalResolverPower` no tiene gate por turno**: se re-arma en cada carta jugada. Con `Max = 100` y `CritCost = 50`, **dos críticos por turno eran legales** ⇒ **E6 impone el cap de uno**, y esta auditoría cuenta uno.
2. **`DivinityPower` se re-arma por `CardPlay`**: +3 (+5 Kenshin) al primer impacto de **cada** Ataque ⇒ **cap de 1 Ataque/turno**, y esta auditoría lo cuenta una sola vez.
3. **El crítico ×1,5 es multiplicativo sobre Fuerza + Divinidad + Bendición y `Hook.ModifyDamageInternal` NO redondea entre pasos** ⇒ se calcula con decimales.
4. **La Fuerza apila de todas las fuentes** y multiplica ~20 impactos por turno: por eso Manifestación topea en +3 y `MeritIsInTheFeetA` deja de dar Fuerza escalable. `CommandBonusPower` (+1 temporal por Buster) **se cuenta**.
5. **Kenshin ya no tiene +1⚡** ⇒ el turno de pico son **3⚡ + 1⚡ de refund = 4⚡**, más el NP a 0⚡.
6. **Los NP nunca critican** (`Criticals.IsEligible` los excluye sin `INoblePhantasmCritical`, que ninguno implementa).

### 14.2 Peor caso construible — Kenshin

Setup previo exigido, todo simultáneo: NP Lv3 (duplicados), **300 de Carga banqueada con `JeweledPagodaC` ⇒ OC5**, Manifestación al tope (+3 Fuerza), Divinidad C→A, Bendición armada, 50★ en banco, mano ideal, y un Buster jugado primero (+1 Fuerza temporal ⇒ **Fuerza total +4**).

| Jugada | ⚡ | Cálculo | Daño |
|---|---:|---|---:|
| **Lanza de Ocho Pétalos** (Buster; arma el +1 de CommandBonus) | 1 | 9 + 3 + 4 (Kenshin) | **16** |
| **NP 不知火** (8 impactos) | 0 | `perHit = 3 + 3(Lv) + 1(OC≥3) = 7`; +4 Fuerza = 11 × 8 = 88; Bendición +2 × 4 impactos = +8 | **96** |
| **Biten: Formación de Rueda** (2×6, **el crítico del turno**) | 2 | 7 impactos… no: 6 impactos × (2+4) = 6 × 1,5 = 9 c/u = 54; Divinidad en el 1.º: (2+4+5) × 1,5 = 16,5 en vez de 9 → +7,5 | **61,5** |
| **Ocho Armas Desatadas** (4×4) | 1 | (4 + 4) × 4 | **32** |
| **Carga Estrellada** (0⚡, gasta 20★) | 0 | 8 + 4 | **12** |
| | **4⚡** | | **≈ 217,5** |

**217,5 — dentro del techo, pegado al borde superior**, y solo si se cumplen simultáneamente ocho condiciones (NP3, 300 de carga, Pagoda C para el OC5, tres ciclos previos, dos poderes montados, 50★, y cinco cartas concretas en la misma mano).

### 14.3 Comprobaciones cruzadas

- **Línea de referencia de la forma base (Kagetora, 3⚡ + refund):** NP 車懸り 85 + Débil, Biten 61,5, Ocho Armas 32, Lanza 12, Carga Estrellada 12 ≈ **202,5**. Correcto: el pico pertenece a la forma ascendida, y la diferencia es modesta (el ascenso paga en calidad, no en un salto de daño).
- **El refund no financia «gratis» el pico:** el turno de arriba cierra ciclo con la mano de conversión, que **gasta 60★ y devuelve 50** y consume tres cartas de mano. La premisa original de P1 («el turno de pico y el de ciclo son distintos por construcción») **era falsa** y se retira del documento.
- **Estrellas:** ingreso del turno = 30 (Pagoda) + 20 (innato Pies) + 10 (Quick) + riders; gasto = 50 (el único crítico) + 20 (Carga Estrellada) + 20 (Oración) + 20 (Formación Cerrada). El banco de 100 **con descarte silencioso** (`CritStars.Gain` retorna si `room <= 0`) no desborda en estado estacionario gracias a los cuatro sumideros en comunes. Es una restricción de diseño, no una nota al pie.
- **Saturación defensiva:** el mejor turno de Bloqueo construible es Murallas mejoradas (26) + Guardia de Kasugayama mejorada (20) + retención 5 = **51 con 4⚡**, sin curación ni Intangible. Y la conversión Bloqueo→daño está capada en **dos lugares independientes** (¼ máx 8 en común, ⅓ máx 12 en PC) y **no existe en el NP**: apilar defensa nunca es una ruta de daño ilimitada.

### 14.4 Contingencias declaradas, en orden, si el playtest pasa de 220

1. **Bendición: cap de 4 → 3 impactos** (−2 en cartas normales, −2 en el NP).
2. **NP de Kenshin: quitar el escalado por OC** (queda `3 + Lv`) → −8.
3. **Manifestación: tope +3 → +2** → −20 en la línea de arriba.
4. **Biten: Formación de Rueda 2×6 → 2×5** → −10.
5. **Divinidad como Kenshin: +5 → +4.**

---

## 15. Riesgos y knobs

### 15.1 Riesgos honestos

1. **El refund puede invertir la decisión.** Si cerrar siempre es correcto, no hay decisión: hay otra rutina. Contrapeso: el 3.er paso es el caro (Pies, medio 1,14⚡) y a veces conviene gastar 2⚡ en un remate y guardar el progreso. **Métrica de playtest: ¿cuántas veces por combate el jugador elige NO cerrar? Si es <10 %, el refund es demasiado grande** (knob 1).
2. **La convergencia 1:1:1 sobrevive.** El pool sigue empujando a draftear un mazo parecido. Es el riesgo #1 de diseño, no de números, y la respuesta está preparada en §15.3.
3. **La tasa de ciclo CAE al crecer el mazo** (75 % → 54,8 % → 52,8 % en el simulador del diagnóstico). El rediseño lo mitiga (tutores, 0⚡, anti-atasco) pero **no lo elimina**. **Es la primera métrica a mirar en la re-simulación**, y la única que decide si el reclamo del reporter se movió de verdad.
4. **Estrellas quemadas.** Con riders de estrellas drafteados encima del caudal base, el banco de 100 descarta en silencio. Los sumideros existen; si el playtest muestra desborde, knob 2.
5. **E6 le saca algo al jugador.** Es la única regla del documento que restringe. Se mitiga con el tooltip y el glow; es reversible en una línea. Nota de honestidad: concentrar los 50★ en Ataques de Pies **también sube el valor del crítico** (de ~+5 en una Estocada a ~+20 en un multi-impacto) — por eso Biten baja a 2×6 y su mejora deja de agregar impactos.
6. **Memoria del jugador.** ~30 IDs cambian de efecto conservando nombre y arte, y **dos comunes de Pecho pasan a ser Ataques**. Changelog de Workshop obligatorio, ítem por ítem, con los deltas silenciosos (`EightWeaponsOneWarrior` −30 % de daño base, `MeritIsInTheFeetA` sin Fuerza escalable, la Pagoda sin robo).
7. **`StackType.Counter`.** El razonamiento (save-safe por ausencia de `SerializableCombat`/`SerializablePower`) está verificado, pero **hay que confirmarlo empíricamente saliendo a mitad de combate y recargando** antes de shippear.
8. **Co-op.** Todo lo nuevo es por jugador y determinista; los riders co-op (`SharedGuard`, `VanguardMandate`, `SaltForTheRival`, `MeritIsInTheFeetA`, `EchigoSaltBag`) conservan estructura y solo cambian números. `ShouldScaleInMultiplayer => false` se mantiene en todos los powers propios.

### 15.2 Lo que este documento NO resuelve, y hay que decirlo

- Los VFX y el audio propios de Doctrina / Bendición / ascensión / NP (los dos NP siguen compartiendo `vfx_dramatic_stab`).
- El nombre del personaje en el HUD (imposible sin Harmony; §12.5-1).
- El icono del poder según el precepto esperado (§12.5-2).
- La convergencia de draft (§15.1-2), mitigada y con contingencia lista.

### 15.3 Contingencia declarada #1 — el «Refuerzo» (injerto de P2, sin ID nuevo)

**Disparador:** si la re-simulación o el playtest confirman que la curva de draft sigue siendo 1:1:1 — métrica: *ratio de ciclos/turno de un mazo balanceado contra uno 3:1:0*.
**Regla:** *«Una carta etiquetada que falla en avanzar entrega **la mitad** de la recompensa innata de su precepto. Máximo 1 por turno. No cuenta para el ciclo y no dispara `IDoctrineAdvanceListener`.»*
**Implementación:** sobre **`GeneralsDoctrinePower`** — ID existente que ya escucha `IDoctrineAdvanceListener` con el guard `!result.Advanced` y el marcado por turno correcto. **Sin ID nuevo, sin reliquia nueva, sin tocar el motor.**
**Se rechaza el resto de P2:** orden libre en la forma base, ⚡ en la reliquia inicial, medidor en FGOCore, rename del HUD, y las tres cartas que cambian de tipo sin auditar sus riders.

### 15.4 Knobs, en orden de prioridad (números, no lógica)

| # | Knob | Cuándo |
|---:|---|---|
| 1 | **Refund del ciclo 1⚡ → 0⚡** y el ⚡ pasa a la Pagoda | si cerrar es siempre correcto (riesgo 1) |
| 2 | Pagoda: +10★ por avance → solo en los avances 1 y 3 (30 → 20★/turno) | si el banco desborda o los críticos inundan |
| 3 | Densidad de 0⚡: 8 → 6 (Oración y Paso de la Victoria vuelven a 1⚡) | si la mano de conversión domina |
| 4 | Gates de las conversiones espejo: 20★/30 NP → 30★/50 NP | si las válvulas 0⚡ se sienten gratis |
| 5 | Contingencias de pico §14.4, en su orden | si el playtest rompe 220 |
| 6 | Retención de Kenshin: 5 → 3 (o → 8 si Pecho se siente irrelevante) | ajuste de la ascensión |
| 7 | Gate de crítico: agregar *«…o si tenés 20 o más de Bloqueo»* | si E6 ahoga a los mazos B |
| 8 | Caps de Bloqueo→daño (¼ máx 8 / ⅓ máx 12), **recalibrados juntos** | si B cierra o no cierra jefes |
| 9 | Paquete de ascensión: +10 NP/ciclo → +20 | si la segunda carrera se siente lenta |
| 10 | *(emergencia)* `MaxEnergy` 3 → 4 | **último recurso**, y solo si la re-simulación demuestra que el refund + los 0⚡ no cierran la brecha |
| — | **PROHIBIDO: `MaxAdvancesPerTurn` 3 → 4** | ver §16.4 |

---

## 16. Notas de implementación

### 16.1 Orden de trabajo (pipeline WORKFLOW-FGO §4.6.7)

0. **Re-testear contra `HEAD` antes de tocar un número.** El playtest del reporter corrió sobre un build con 12 efectos muertos; `HEAD` ya trae el `?? 0` y 6 fixes más. Parte de 「太缺费了」 puede haberse movido sola.
1. **Re-derivar la tabla de bugs desde `git log`** (§11) y cerrar los cuatro que siguen abiertos + los dos de loc. `audit_simpleloc` tiene que salir **verde de entrada**.
2. **Motor y contratos, sin números:** E1-E8 · `DoctrinePower : IResourcePower` · `StackType.Counter` + `DisplayAmount` · `IAddDumbVariablesToPowerDescription` · `ExtraHoverTips` y `ShouldGlowGoldInternal` en `KagetoraCard` · los dos `FormPower` · migración de `_prevented` y del precepto de `FortuneArmourAndMeritA` · bits nuevos en `KagetoraUsage`.
3. **Reliquia inicial + Ancient + Brasero**, y **MEDIR**: cadena media, ciclos/8T, ⚡ libre/turno, tasa de ciclo con mazos de **10 / 20 / 25** cartas, con el script y la semilla del diagnóstico.
4. **Lotes por rareza:** básicas → comunes → PC → raras. **Ningún número del pool se toca antes de la medición del paso 3.**
5. **Los dos NP.**
6. **Loc ×5 idiomas** («el código manda»): re-textos de las ~30 re-especificadas, ordinales I/II/III en las 69 etiquetas, `DOCTRINE_POWER` y los dos `*_FORM_POWER` reescritos, `!RemoveWeak!`, `ARMY_FOOTSTEPS`, span de zhs.
7. `tools/audit_localization_parity.ps1` → `tools/audit_simpleloc.ps1` → `tools/audit_vfx_paths.ps1` → matriz MAIN/BETA → **publish solo `KagetoraLancer`** → changelog completo.
8. Pasar a la skill `sts2-fgo-mod-development` para la implementación.

### 16.2 Hooks: usar estos, no adivinar

- **Retención de Bloqueo de Kenshin:** `KenshinFormPower : IBlockRetentionSource` con `RetentionCap => 5`. **No se cuelga de una carta**: el contrato exige que alguien responda `ShouldClearBlock`, y la base `FormPower` ya lo hace por sus subclases y delega en `BlockRetention.Enforce`. Un power suelto o una reliquia tendrían que overridear `ShouldClearBlock` **y** `AfterPreventingBlockClear`.
- **Ganancia de energía** (Brasero, +1⚡ del turno del ascenso): `AfterEnergyReset(Player)` → `PlayerCmd.GainEnergy(1, player)`. Precedente vanilla exacto: `RadiancePower`. **No usar `AfterSideTurnStart`**: correría antes o después del reset según el orden interno, y eso es exactamente la clase de suposición que produjo el bug P0.
- **Refund del ciclo:** desde `IDoctrineCycleListener` en `DoctrinePower`, no desde la reliquia.
- **`ICriticalAccessRule` en `DoctrinePower`:** la regla tiene que ser **pura** — `Criticals.CanSpend` se consulta también desde `WillCrit`, que es predicción para glow y hover. Leer el bit por turno es puro; **marcarlo** va en el listener de consumo, nunca en el predicado.
- **Texto dinámico del power:** `BaseLib.Patches.Localization.IAddDumbVariablesToPowerDescription` (el postfix corre dentro de `AddDumbVariablesToDescription`, invocado en ambas ramas de `PowerModel.HoverTips`). `DynamicVar` es `decimal` salvo `StringVar`: el nombre del precepto que falta se inyecta como string con `LocString.Add(string, string)`.

### 16.3 Bits de estado por turno — cuáles están libres

**`KagetoraUsagePower.Mask`** (`Amount − 1`; `PerTurnMask` se limpia en `BeforeSideTurnStart` con `participants.Contains(Owner)`): el enum `KagetoraUsage` usa **1 … 2048** (12 flags). **Libres: 4096, 8192, 16384, …** Se agregan:

- `CriticalThisTurn = 4096` — marcado desde el consumo del crítico, leído por E6. **Va dentro de `PerTurnMask`.**
- `Divinity = 8192` — cap de 1 Ataque/turno de `DivinityPower`. **Va dentro de `PerTurnMask`**, y cierra de paso la deuda de `_active`/`_usedHit` **como flag por turno** (la ligadura de `CardPlay` que decide *qué impacto* es el primero sigue siendo local al hook de cálculo, que debe seguir puro).
- `BlessingArmedThisTurn = 16384` — opcional, solo si E8 necesita distinguir «armada» de «gastada» sin tocar `_activePlay`.

**`DoctrineTurnStatePower`** (se resetea **entero** cada turno): `State = (Amount − 1)` con **bits 0-1 = `Advances`** y **bits 2-4 = `AdvancedMask`**. Bits **5 en adelante libres** para flags estrictamente por turno.
**No se toca `FgoCombatState`** (es de FGOCore).

### 16.4 El knob prohibido, y por qué

`DoctrineTurnStatePower.Advances => State & 3` y `DoctrineTurnState.Set` guarda `(advances & 3)`: **es un campo de 2 bits**. Con `MaxAdvancesPerTurn = 4`, el cuarto avance guarda `4 & 3 = 0`, el contador **wrappea**, `WouldAdvance` vuelve a devolver `true` y **el tope desaparece**: avances ilimitados ⇒ ciclos ilimitados ⇒ refund de energía ilimitado. No es un ajuste: es un loop determinista.
**`MaxAdvancesPerTurn` no sube a 4 en ninguna forma.** Si alguna vez se toca: primero se ensancha el campo, se re-verifica `WouldAdvanceAfter`, y se escribe un cap explícito de 1 refund/turno **antes**. **Dejar esto como comentario en `SystemPowers.cs` y en `Doctrine.cs`** para que nadie lo «mejore».

### 16.5 El cap de 3 procs de la Pagoda no se implementa

Es la consecuencia de `MaxAdvancesPerTurn`, no un contador. **Dejar el comentario en `IdentityRelics.cs`** para que nadie lo «arregle» agregando estado redundante — cada contador nuevo es superficie de bug y de desincronización con el motor.

### 16.6 Colisiones de re-tipado, auditar explícitamente

- `SixPlateArmour` (reliquia, «la primera **carta de Pecho** de cada turno») ahora puede dispararse con un **Ataque** (`Muro de Lanzas`, `Muralla de Echigo`). Es coherente (el precepto no cambió), pero hay que verificarlo y decirlo en el changelog.
- `FearlessChestPower` escucha el **avance**, no el tipo → sin cambio (verificado).
- **`BishamontenBlessingPower`**: con E8 los Ataques de Pecho **ya no arman ni queman** la Bendición. Ese es el punto del parche; documentarlo en el tooltip de la Bendición.
- `HoushoutsukigeReins` («la primera Pies de cada turno») ahora puede dispararse con `Carga Estrellada` a 0⚡. Aceptado.

### 16.7 Localización — 5 idiomas, «el código manda»

- Idiomas: `eng`, `esp`, `kor`, `rus`, `zhs`. Formato de clave: `KAGETORALANCER-<ID>.title` / `.description`.
- **345 strings de ordinal** (69 cartas × 5) + ~30 descripciones re-especificadas + 3 cartas nuevas + `DOCTRINE_POWER` + los dos `*_FORM_POWER`.
- **Gotcha de SimpleLoc:** el terminador ASCII no incluye `。` (U+3002); todo span dorado en zhs se cierra **explícitamente** con `*词*`. El caso vivo es `INCARNATION_OF_BISHAMONTEN`: `*毗沙门天的*加护` → `*毗沙门天的加护*` (y revisar el eng `*Bishamonten's *Blessing`).
- **Ningún texto de carta puede quedar sin cantidad** cuando el código la tiene (`SaltForTheRival` → `!RemoveWeak!`).
- **Paridad obligatoria** con `tools/audit_localization_parity.ps1` + `tools/audit_simpleloc.ps1` antes del publish.

### 16.8 Changelog de Workshop — obligatorio, ítem por ítem

Los ~30 IDs re-especificados sobre nombre y arte existentes, los 3 [NUEVA], y **los deltas silenciosos declarados**: `EightWeaponsOneWarrior` 10×2 → 7×2 (**−30 % de daño base**), `MeritIsInTheFeetA` sin Fuerza escalable, la **Pagoda deja de robar**, `Biten` 2×8 → 2×6 y su mejora deja de agregar impactos, el NP de Kagetora baja de ~91 a ~85 por el cap de Bendición, y **Kenshin no da +1⚡** (por si alguien leyó una versión previa del documento).

### 16.9 Qué NO tocar

- **FGOCore: ni un archivo.** Si algo exige tocarlo, el diseño está mal, no el core.
- **`Doctrine.cs`** salvo por E1/E2/E6/E7 (refund, `IResourcePower`, `ICriticalAccessRule`, `DisplayAmount`): el timing de `AfterCardPlayed`, el golpe letal, la reentrancia, las copias y el reset están verificados sanos.
- **Los preceptos de las 69 cartas** y el glosario 天/胸/足.
- **Los IDs**: ninguno se renombra ni se borra, incluidas las tres `Choose*` de `CardRarity.Event`.
- **Los riders co-op**: cambian números, no estructura.
- **Rutas VFX inventadas**: validar cada `vfx/` contra `grep '"vfx/' decompiled/` **antes** de escribirla — un path inexistente tira NRE en `VfxCmd.PlayVfx` y deja la carta congelada.
- **Arte**: 3 cartas nuevas (`Muro de Lanzas`, `Muralla de Echigo`, `Carga Estrellada`) con material oficial de Atlas Academy y procedencia trazable en el CSV **antes** del lote. Al publicar en frío, `patch_webp_imports` regenera los `.import`: restaurar los trackeados después.

---

## 17. Apéndice normativo: el grafo de conversión

Regla: **ningún medidor se estanca — todo recurso tiene ≥2 entradas y ≥2 salidas en COMUNES.**

```
⚡ ──► cartas ──► daño / Bloqueo / Carga NP / estrellas
│
└──► CICLO (3 cartas en orden) ──► +1⚡   ◄── el ÚNICO retorno de energía del kit
          ├──► Carga NP  (Cielo +10)
          ├──► Bloqueo   (Pecho +4)
          └──► estrellas (Pies +20 · Pagoda +10 por avance ×3) = 50★ = 1 CRÍTICO (uno por turno)

Carga NP ──► estrellas (Dar Vuelta a la Formación 50→50)          [común]
         ──► cartas    (Estrategia de Rueda 30→robá 2)            [PC]
         ──► daño      (los dos NP, a 100)
estrellas ──► Carga NP (Volver las Riendas 50→50 · Oración 20→30) [comunes]
          ──► Bloqueo  (Formación Cerrada 20→8)                   [común]
          ──► daño     (Carga Estrellada 20→8 · el crítico 50→×1,5) [comunes]
Bloqueo   ──► daño     (Muralla de Echigo ¼ máx 8 [común] · Guardia Compartida ⅓ máx 12 [PC])
          ──► Carga NP (Interponer la Lanza / Guardia de Kasugayama, al avanzar +10) [comunes]
```

**Tasas patrón (1⚡ común):** 9-10 daño plano · 7 Bloqueo · 4×2 multi-impacto · 6-8 daño + 10 Carga NP · 5 Bloqueo + 10 Carga NP.
**A 0⚡ repetible:** SIEMPRE una conversión que gasta **más de lo que su propio avance devuelve** — 20★→30 NP · 50★→50 NP · 20★→8 Bloqueo · 50 NP→50★ · 20★→8 daño · 30 NP→robá 2. Valor neto a 0⚡ ⇒ **Agotar** (Paso de la Victoria, Carga Mágica).
**Valores de referencia:** 1⚡ ≈ 9-10 daño · 10★ ≈ 3 daño diferido (50★ = 1 crítico) · 10 de Carga NP ≈ 3 daño diferido (100 = un NP de ~85) · 1 Bloqueo ≈ 0,8 daño evitado.

**Breakpoints que el jugador tiene que poder decir en voz alta:**
- **3 avances = un ciclo = +1⚡.**
- **Un ciclo = 50 estrellas exactas = el crítico del turno.**
- **Un crítico por turno**, y va donde vive el multi-impacto.
- **3 avances = el tope del turno** (y ahora muerde: hoy es código muerto porque 3 avances ya costaban las 3 energías).
- **100 de Carga NP = el NP**; el primero de Kagetora **es** la ascensión.
- **Ascender = orden libre, 20 de Carga NP por ciclo, 5 de Bloqueo que no se limpia, y un NP que arranca el Bloqueo antes de pegar.**
