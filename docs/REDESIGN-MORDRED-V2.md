# REDESIGN-MORDRED-V2 — Mordred (Saber of Red)

> **Estado: PROPUESTA — pendiente de la revisión adversarial (Fable 5) y de implementación.**
> Diseño: Opus 5, 2026-08-21. Base de hechos: **el código en `HEAD`**, no `docs/DESIGN-MORDRED.md`
> (el doc de diseño y la implementación difieren en varios números; donde difieren, manda el código).
> Formato: `docs/REDESIGN-MASH-V2.md` / `docs/REDESIGN-KAGETORA-V2.md`.
> Mecanismo save-safe: **ningún ID se renombra, ninguno se borra, cero cartas nuevas, cero DEMOTE**.
> Todo es **re-efecto sobre IDs ya publicados**. **FGOCore no se toca** ⇒ se publica solo
> `MordredSaber`; los otros 11 personajes no se republican.

---

## 1. Qué NO cambia

La identidad está bien y no se toca: *el Caballero de la Traición tanquea su rabia tras el Yelmo de
la Infidelidad, se lo arranca para cobrarla en CRÍTICOS de relámpago rojo, y la corona con Clarent
Blood Arthur.* Verbos: **enmascarar, cobrar, criticar**. Recursos: **Carga NP 0-300** y **Estrellas
de Crítico**, cosidos por la starter (Vida→★, Crítico consumido→NP).

El problema no es la fantasía. **Es que el motor está invertido**, y cada punto está medido.

---

## 2. Diagnóstico verificado

Cada hallazgo tiene su evidencia en el código; ninguno es inferencia de lectura del doc de diseño.

### D1 — El mazo inicial no puede cargar el medidor (ALTO)

`Cards/Basic/ArtsCommand.cs:23` — `new DamageVar(5m), new DynamicVar("NpCharge", 10)`, `OnUpgrade`
sube el NP **+5**.

El estándar del ecosistema para el Arts básico (WORKFLOW-FGO §4.6.1) es **6 de daño + 30 de Carga
NP**, y está implementado así en los dos personajes calibrados más recientemente:
`KagetoraLancer/.../Cards/Basic/StartingCards.cs:37` y `ArtoriaCaster/.../Cards/Basic/ArtsArtoria.cs:20`.
**Mordred está a un tercio.**

Cuenta del mazo inicial. **La composición real está en `Character/Mordred.cs:37-46`** y NO es la que
declara `DESIGN-MORDRED.md §5.0`: son **2 Buster + 1 Arts + 1 Quick + 1 Golpe + 3 Defensa + Rebelión
+ Bajar la Visera**. (Un solo Arts ⇒ el mazo ni siquiera es QAABB, otra desviación de §4.6.1.)

| fuente | ahora | por ciclo de mazo |
|---|---|---|
| **1×** Arts | 10 | 10 |
| 2× Buster (`BusterCommand.cs:23`, `NpCharge` 5) | 5 c/u | 10 |
| 1× Bajar la Visera (`LowerTheVisor`, 5) | 5 | 5 |
| **total impreso** | | **25** |
| + forma Enmascarado (`MordredFormPower.cs:25`, +5 NP al inicio de tu turno) | 5/turno | +10 por ciclo |
| **total real** | | **35** |

Un ciclo de mazo son ~2 turnos ⇒ **el primer NP cae recién en el turno ~6**. El objetivo declarado
del ecosistema, fijado en la recalibración de Artoria del 2026-08-18 (`docs/STATUS.md`), es **turno 3**.

### D2 — Una común de 0⚡ da 3-5× lo que da el Arts básico (ALTO)

`Cards/Common/ManaIgnition.cs`: 0⚡, **+30 NP**, `OnUpgrade` **+20 ⇒ 50**. Sin coste, sin rider, sin
Exhaust, sin condición.

La banda que el propio proyecto fijó tres días antes para las cartas de carga (STATUS 2026-08-18,
recalibración de Artoria): **0⚡ → 10-20 NP; 1⚡ → 20-30 NP**. Ignición de Maná mejorada está a
**2,5-5× su banda**, y da **más que el Arts básico por cero energía**. El resultado es que la
economía del personaje no vive en su mazo: vive en si te tocó draftear esta común.

Es el mismo defecto que reportó 1369642093 en Artoria («la recompensa de carta vale menos que
saltearla»), **invertido**: acá la recompensa de carta vale más que todo el mazo.

### D3 — Dos poco-comunes venden un payoff de FORMA que paga exactamente CERO (ALTO)

| carta | rareza | rama base | rama «en la forma correcta» | delta real |
|---|---|---|---|---|
| `Cards/Uncommon/LightningSpeed.cs:22,25,38` | Poco común | `BaseStars = 10` | `Stars = 10` | **0** |
| `Cards/Uncommon/DentedHelm.cs:22,25,35` | Poco común | `BaseStars = 10` | `Stars = 10` | **0** |
| `Cards/Common/SparksOfTheHelm.cs:23,26,36` | Común | `BaseNp = 8` | `NpCharge = 20` | +12 |
| `Cards/Common/KnightsSteadfastness.cs:22,25,35` | Común | `BaseNp = 5` | `NpCharge = 10` | +5 |

Las dos primeras tienen `ShouldGlowGoldInternal => Forms.InRebellion(...)` / `InMaskedForm(...)` y
un `HoverTipFactory.FromPower<RebellionFormPower>()`: **el borde se dora y el tooltip promete un
bonus que no existe**. Es la misma familia de bug que el pase de Kagetora V2 documentó como «12
efectos muertos» (STATUS 2026-08-16) y que el de Astolfo/Kagetora encontró con el null-lifting.

Además `8` y `5` **no son denominaciones legales** (la regla del proyecto fija 10/20/30/50/100).

### D4 — Inversión de rareza: una COMÚN domina estrictamente a una POCO COMÚN (MEDIO)

Consecuencia directa de D3:

- `SlashOfClarent` (**común**): 1⚡, 9 de daño, **10★ siempre**; mejora **+3 daño y +10★**.
- `LightningSpeed` (**poco común**): 1⚡, 9 de daño, **10★ siempre** (D3); mejora **+3 daño**.

Mismo coste, mismo daño, mismas estrellas, peor mejora. La poco común es una copia estrictamente
peor de la común.

### D5 — El cambio de forma es un motor gratuito y card-positivo (ALTO)

Tres piezas que por separado son razonables:

1. `Cards/Uncommon/RoarOfRebellion.cs` — **0⚡**: cambiá de forma **y robá 1**; mejora: **robá 2**.
2. `Powers/BannerOfRebellionPower.cs:38-48` — por **cada** cambio de forma: `StarsPerSwitch * Amount`
   (10★ por stack) + `NpPerSwitch * Amount` (5 NP por stack) + robo si está mejorada.
   **Sin tope por turno.**
3. `Powers/SecretRevealedPower.cs:34-53` (rara `SecretRevealed`, 2⚡ / **1⚡ mejorada**) — por **cada**
   entrada en Rebelión: `Stars * Amount` (**20★ por stack**) **y robá 1**. Tampoco tiene tope por turno,
   y es el doble de grande que el Estandarte.

Juntas: con Estandarte mejorado ×2 stacks, un **Rugido de Rebelión mejorado (0⚡)** da
**+20★, +10 NP y robá 4** (2 del Rugido + `DrawsPerSwitch * Amount` = 2 del Estandarte,
`BannerOfRebellionPower.cs:48`) — se reemplaza a sí mismo con tres cartas de ganancia neta y encima
paga las dos economías. Con `SecretRevealed` en juego se suman otros +20★ por stack y otro robo. Eso
es exactamente lo que la rúbrica prohíbe:
*«Buscar cadenas de coste cero, robo neto positivo, energía neta positiva y generación recursiva»*.

**Y el daño colateral de diseño es peor que el numérico:** el eje de FORMAS —el arquetipo propio del
personaje— quedó al revés. **Hoy el switch es el payoff y la forma es inerte** (D3). El diseño quiere
lo contrario: el switch es el costo (una carta, un turno de exposición) y estar en la forma correcta
es el premio.

### D6 — El par espejo mejorado es positivo-suma (MEDIO)

`SpoilsOfCamelot` (común, 0⚡): gastá 50 NP → 50★; **mejora: el costo baja a 30**.
`TributeToTheThrone` (común, 0⚡): gastá 50★ → 50 NP; **mejora: el costo baja a 30**.

Ida y vuelta con las dos mejoradas: −30 NP +50★, después −30★ +50 NP ⇒ **+20 NP y +20★ netos, por
0⚡ y dos cartas**. El doc de diseño las llama «ESPEJO A/B» y promete fungibilidad; una mejora que
baja el costo de las dos direcciones convierte el espejo en una bomba de recursos.

### D7 — el bonus de crítico se cobra POR IMPACTO y las multi-hit rompen el techo (ALTO)

`Powers/KnightOfRedLightningPower.cs:32-38` y `Powers/TheMostRadiantSwordPower.cs:35-41` devuelven su
bonus de crítico desde `ModifyDamageAdditiveFgo`, que corre **una vez por impacto**. Y el
multiplicador de crítico **no es ×2**: es **×1,5** (`FGOCore/FGOCoreCode/Stars/Criticals.cs:52`,
`DamageMultiplier = 1.5m`) y por contrato de Críticos v2 se aplica a **todos** los impactos de la
carta (`docs/DESIGN-FGOCORE-CRITICAL-V2.md:40`). Los comentarios de varias cartas de Mordred que
dicen «sólo el PRIMER golpe se dobla (parche P8)» quedaron **stale** desde la migración a v2.

Resultado, con el motor completo mejorado (forma Clímax +2, Caballero+ +3 y +8 al crítico, Espada
Más Resplandeciente+ +12 al crítico, Doble Filo+ +4, Estallido de Maná+ +6): cada impacto lleva
**+15 planos y +20 más si la carta critica**, y todo eso se multiplica por 1,5. Una común de 1⚡:

- `LightningSplinters`+ (5 de daño **×3 impactos**): (5+15+20) × 1,5 = **60 por impacto ⇒ 180 por 1⚡**.
- `RebelsDoubleEdge`+ (6 **×2**): (6+35) × 1,5 = 61 c/u ⇒ **122 por 1⚡**.

El daño base de la carta es irrelevante al lado de los planos: **el techo real de Mordred lo fijan las
multi-hit, no las cartas grandes**, y está en ~300 por turno de 3⚡ (§6), contra la banda de
**180-220** de `DECISIONS.md:40`.

El proyecto **ya falló este mismo caso** en Kagetora: `DivinityPower`
(`KagetoraLancer/.../Powers/UncommonPowers.cs:123-185`) documenta que sin tope «bonificaba el primer
impacto de CADA Ataque … que la auditoría no contaba», y lo cerró ligando el bonus al `CardPlay` y
marcándolo en `AfterDamageGiven` (daño real), dejando `ModifyDamageAdditiveFgo` **puro**. Mordred no
recibió ese tratamiento.

### D8 — `HundredShatteredSwords` se anula a sí misma y hace CERO (ALTO — bug en producción)

`CriticalResolverPower.BeforeCardPlayed` (`Criticals.cs:132-167`) corre **antes** del `OnPlay` de
cualquier Ataque elegible y, si no hay Crítico Listo, **gasta 50★ del banco solo** para criticar.
`Cards/Rare/HundredShatteredSwords.cs:30-31` vuelve a pedir `CanPay(50)` y **retorna sin pegar** si no
llega. Con 50-99★ y sin Crítico Listo la carta **quema las 50★ y hace 0 de daño**; su `IsPlayable`
se evaluó antes del cobro, así que el jugador la ve dorada y jugable.

El repo ya tiene el fix canónico escrito **y comentado con este mismo razonamiento**:
`StarlitCharge` de Kagetora (`Cards/Common/CommonCards.cs:444-455`) gasta primero y pega si pagó
**o** si `Criticals.IsCritical(cardPlay)`.

### D9 — el mismo motor de D5, más grande, en la capa de raras (ALTO)

`SecretRevealedPower` (arriba, D5 punto 3) es el Estandarte al doble y sin tope. Cualquier candado
que se le ponga sólo al Estandarte es cosmético.

### Lo que se verificó y está BIEN (para no arreglar lo que no está roto)

- **Techo de daño por turno dentro del rango.** Ver §6: el pico auditado con motor completo es
  **~126** en un turno de 3⚡, contra el techo de saturación de 180-220. Mordred **no** es Mash: esto
  no es un nerf de potencia.
- **Cleanse acotado a los dos vectores que su kit justifica** (`SecretOfPedigreeEX` +
  `MagicResistanceBCharm`), como manda la regla negativa de `DESIGN-MORDRED.md §2`.
- **`HundredShatteredSwords`** (rara 0⚡, 26 de daño) **paga 50★** — no es una carta gratis. Pero el
  cobro está **roto**: ver D8.
- **La starter no se apila con la reliquia de jefe**: `ClarentOverloadedWithHatred` silencia a
  `ClarentTheStolenSword` (fix del audit 2026-07-05), y la starter tiene tope de 3 procs/turno.
- **La retención de Bloqueo de las formas sobrevive al cambio de Baluarte de FGOCore v0.1.25.**
  `MordredFormPower` no implementa `ShouldClearBlock`, pero **hereda la implementación correcta de
  `FGOCore/.../Forms/FormPower.cs:59-73`**, que responde `false` y llama `BlockRetention.Enforce`.
  Auditados los 6 implementadores de `IBlockRetentionSource` fuera de FGOCore: **todos cumplen el
  contrato**. Ningún daño colateral del lote Mash V2.

---

## 3. Los cuatro candados

### Candado 1 — el mazo inicial carga; la común deja de saltearlo

La carga vuelve a donde el jugador la paga (el Arts, que gasta energía y una carta) y se va de donde
era gratis (una común de 0⚡ sin rider). Primer NP: turno ~5 → **turno 3**, el objetivo del ecosistema.

### Candado 2 — la forma es el pago; el switch es el costo

Las cuatro cartas bi-condicionales pasan a pagar **exactamente una denominación** (+10) por estar en
la forma correcta, con pisos legales. El motor de switch deja de reemplazarse a sí mismo y deja de
ser ilimitado por turno.

### Candado 3 — las conversiones dejan de ser una bomba de recursos

La mejora del par espejo sube la **salida (+10)** y deja el **costo fijo**, en vez de bajar el costo
de las dos direcciones. La ida y vuelta totalmente mejorada baja de **+20 NP/+20★ a +10 NP/+10★**, y
el umbral para usarlas sube de 30 a 50. *No* la lleva a cero: eso queda declarado en R2, no escondido
detrás del título.

### Candado 4 — el crítico deja de cobrarse por impacto

El bonus de crítico de los dos poderes que lo dan se cobra **una vez por carta, en el primer impacto
que pega de verdad**, con el patrón exacto de `DivinityPower` de Kagetora (ligadura al `CardPlay`,
marcado en `AfterDamageGiven`, hook de preview **puro**). El multiplicador ×1,5 sigue aplicando a
todos los impactos: eso es el contrato de Críticos v2 y no se toca. Esto es lo que devuelve a Mordred
adentro de la banda de 180-220 (§6).

---

## 4. Matriz de arquetipos (post-cambio)

| | **A. El Yelmo** (tanque/banco) | **B. La Rebelión** (all-in) | **C. El Relámpago** (crit) | **D. Anti-Autoridad** |
|---|---|---|---|---|
| **Motor** | Enmascarado: retención 10, +5 NP/turno, riders «si Enmascarado» | Rebelión: Ataques +2, cada golpe recibido = +10★ | ★ → Crítico Listo ×2 → +10 NP | riders vs Élite/Jefe |
| **Ataque** | Firmeza del Caballero, Chispas del Yelmo | Rebelión, Relámpago de Clarent, Carga Temeraria | Cien Espadas, Espada Más Resplandeciente | Embate de Odio, Decapitación del Usurpador |
| **Defensa** | Yelmo Abollado, Guardia del Torneo, Secreto de Cuna | (ninguna: es el precio) | Guardia Insolente | Estandarte de Camlann |
| **Consistencia** | «Trátame como caballero» (+1 robo Enmascarada) | Rugido de Rebelión | Instinto de Batalla, Instinto B, León del Cigarrillo | — |
| **Energía** | Paso del Torneo (0⚡) | Rugido (0⚡) | Espadazo Insolente (0⚡) | — |
| **Escalado** | Sangre de Dragón, Ambición al Trono | Doble Filo del Odio | Corona del Relámpago, Caballero del Relámpago Rojo A+ | Desdén al Trono |
| **AoE** | — | Relámpago Residual | Relámpago Encadenado | Tormenta de Camelot |
| **Sustain** | Lealtad Mal Pagada, Memoria de Trifas | Camlann (Guts) | — | — |

Las cuatro líneas siguen teniendo ataque, defensa (salvo B, que es su precio declarado), consistencia
y escalado. **No se abre ningún subsistema nuevo.**

---

## 5. Re-spec carta por carta

**Save-safety:** todo lo de abajo es cambio de número o de rama dentro de un ID ya publicado. Cero
renames, cero borrados, cero cambios de rareza, cero cartas nuevas.

### 5.1 Básicas — Candado 1

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `ArtsCommand` | 5 daño + **10** NP; up +5 NP | **6 daño + 30 NP**; up **+3 daño / +10 NP** | §4.6.1 y paridad con Kagetora/Artoria (D1) |
| `BusterCommand` | 8 daño + 5 NP; up +3 | **10 daño** + 5 NP; up +3 | §4.6.1 para el daño. **Los 5 NP se CONSERVAN** (cambio respecto de la primera versión de este documento): el comentario de `Character/Mordred.cs:33-35` los pone ahí a propósito —«modeladas sobre las de Okita: el Buster carga NP al pegar»— y la cuenta de turno 3 cierra igual con o sin ellos (R5). Quitarlos era pisar una decisión deliberada sin evidencia de que haga daño |
| `QuickCommand` | 5 daño + 20★; up +3 | **6 daño** + 20★; up **+3 daño solamente** | §4.6.1. **No se tocan las 20★ impresas**: toda Quick recibe +10★ del motor de tipos (`FGOCore/.../CardTypes/CommandBonusPower.cs:38,81-83`), así que ya son 30★ efectivas. Kagetora lo tiene escrito como advertencia: *«No subir el var a 30: duplicaría el caudal»* (`StartingCards.cs:59`) |
| `LowerTheVisor` | 4 Bloqueo + enmascara + 5 NP; up +3/+5 | 4 Bloqueo + enmascara + **10 NP**; up +3 / **+10** | denominación legal; es la firma defensiva |
| `Defend`, `Strike`, `Rebellion` | — | **[=]** | ya están en estándar |
| **`Mordred.StartingDeck`** (`Character/Mordred.cs:42`) | 2 Buster + **1 Arts** + 1 Quick + **1 Golpe** + 3 Defensa + 2 firmas | 2 Buster + **2 Arts** + 1 Quick + 3 Defensa + 2 firmas (**el Golpe pasa a ser el segundo Arts**) | Sin esto el Candado 1 **no llega a turno 3**: con un solo Arts el ciclo queda en 40 y el primer NP en el turno ~4. Además el mazo pasa a ser **QAABB de verdad** (§4.6.1), que hoy no es. Save-safe: `StartingDeck` sólo afecta runs nuevas y no renombra ningún ID |

Primer NP tras el cambio: 2×30 (Arts) + 10 (Visera) + 2×5 (Buster) = **80 impresos por ciclo de mazo** + 5 NP/turno
de la forma Enmascarado + la starter (crítico consumido → 10 NP) ⇒ **turno 3**.

### 5.2 Comunes — Candados 1 y 3

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `ManaIgnition` | 0⚡ +30 NP; up **+20 (⇒50)** | 0⚡ **+20 NP**; up **+10 (⇒30)** | banda 0⚡ del ecosistema (D2). La mejora la lleva a la banda de 1⚡: eso es lo que una mejora debe hacer |
| `SparksOfTheHelm` | 8 Bloqueo; **8** NP base / 20 Enmascarado | 8 Bloqueo; **10** NP base / **20** Enmascarado; up +3 Bloqueo | piso a denominación legal; el bonus de forma queda en exactamente +10 (D3) |
| `KnightsSteadfastness` | 13 Bloqueo; **5** NP base / 10 Enmascarado | 13 Bloqueo; **10** NP base / **20** Enmascarado; up +4 Bloqueo **/ +10 a la rama Enmascarado** | ídem (D3) |
| `SpoilsOfCamelot` | 50 NP → 50★; up **costo −20** | 50 NP → 50★; up **salida +10 (⇒60★)**, costo fijo | D6. Con +20 la ida y vuelta seguía dando el mismo +20/+20 de hoy; con +10 baja a +10/+10 |
| `TributeToTheThrone` | 50★ → 50 NP; up **costo −20** | 50★ → 50 NP; up **salida +10 (⇒60 NP)**, costo fijo | D6 |
| `TournamentFootwork` | 3 Bloqueo + 10 NP; up +2 Bloqueo / **+5 NP (⇒15)** | up +2 Bloqueo / **+10 NP (⇒20)** | 15 no es denominación legal (`TournamentFootwork.cs:30`) |
| resto de las 20 comunes | — | **[=]** | sin hallazgo |

### 5.3 Poco comunes — Candado 2

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `LightningSpeed` | 9 daño; 10★ base / **10★** Rebelión; up +3 daño | 9 daño; **10★ base / 20★ en Rebelión**; up +3 daño **y +10★ a la rama de Rebelión (⇒30★)** | D3 + D4: la condición pasa a existir, y la poco común gana a la común **solo en su forma** |
| `DentedHelm` | 11 Bloqueo; 10★ base / **10★** Enmascarado; up +4 Bloqueo | 11 Bloqueo; **10★ base / 20★ Enmascarado**; up +4 Bloqueo **y +10★ a la rama Enmascarado** | D3 |
| `RoarOfRebellion` | 0⚡ switch + robá 1; up **robá 2** | 0⚡ switch + robá 1; up **+10★** en vez del segundo robo | D5: el conector del arquetipo puede ser un cantrip neutro en cartas, no card-positivo |
| `BannerOfRebellion` (vía `BannerOfRebellionPower`) | por cada switch, sin tope | **máximo 2 activaciones por turno** (reset al inicio de tu turno) | D5. Idioma del propio repo: `AccumulatedHatred` (`MaxProcs = 2`), `ClarentTheStolenSword` (3/turno), `RoundTableFragment` |
| `LightningVisit` | 0⚡, ida + regreso automático | **[=]** | **no** era un multiplicador: el regreso entra con `source == null` (`LightningVisitReturnPower.cs:46-56`) y `FormSwitch.Enter` no notifica listeners en ese caso (`FGOCore/.../Forms/FormSwitch.cs:36-37`). Proca **una** vez, igual que un Rugido |
| resto de las 28 poco comunes | — | **[=]** | sin hallazgo |

### 5.4 Raras y poderes — Candados 2 y 4

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `SecretRevealed` (vía `SecretRevealedPower`) | +20★ × stacks **y robá 1** por cada entrada en Rebelión, **sin tope** | **máximo 2 activaciones por turno** (mismo contador que el Estandarte) | D9. Sin esto el Candado 2 es cosmético: se capea el motor chico y se deja libre el doble de grande |
| `HundredShatteredSwords` | pide 50★ en `OnPlay` **después** de que el resolutor de críticos ya se los llevó ⇒ 0 de daño | gastar primero y pegar **si pagó o si la carta está criticando** (`Criticals.IsCritical(cardPlay)`) | D8. Patrón textual de `StarlitCharge` (Kagetora) |
| `KnightOfRedLightningAPlus` (vía su power) | +Ataque plano **y** +8 al crítico, los dos **por impacto** | el +Ataque plano sigue por impacto; **el bonus de crítico, sólo en el primer impacto que pega** | D7 / Candado 4. Patrón `DivinityPower` |
| `TheMostRadiantSword` (vía su power) | +12 al crítico **por impacto** | **sólo en el primer impacto que pega** | D7 / Candado 4 |
| resto de las 20 raras, las 3 especiales, las 13 reliquias, las 3 formas y el modelo de NP | — | **[=]** | sin hallazgo |

**Total: 19 IDs re-especificados** (5 básicas + 1 mazo inicial + 5 comunes + 4 poco comunes +
4 raras/poderes), sobre 78 cartas, 13 reliquias y 27 poderes.

## 6. Auditoría de pico (rehecha tras la revisión adversarial)

**La primera versión de esta auditoría estaba mal por tres lados**: usaba ×2 en vez del ×1,5 real
(`Criticals.cs:52`), no contaba cartas mejoradas ni multi-hit, y contaba 34 de daño por una
`HundredShatteredSwords` que en ese turno hace **0** (D8). Rehecha:

Escenario: acto 3, 3⚡, motor completo **con todo mejorado** — forma **Relámpago Carmesí** (+2),
`KnightOfRedLightningAPlus`+ (+3 plano, +8 al crítico), `TheMostRadiantSword`+ (+12 al crítico),
`DoubleEdgeOfHatred`+ (+4 en forma ofensiva), `ManaBurstA`+ (+6 este turno) y Críticos Listos en cola
(tope 3, `CritReadyPower.cs:16`). Orden de StS: **aditivo → multiplicativo**, y el ×1,5 aplica a
**todos** los impactos de la carta (contrato de Críticos v2).

**Plano por impacto: +15. Extra si la carta critica: +20.**

| jugada | ⚡ | HOY (bonus de crítico por impacto) | CON el Candado 4 (primer impacto) |
|---|---|---|---|
| `ManaBurstA`+ (setup, Exhaust) | 1 | 0 | 0 |
| `LightningSplinters`+ (5 daño **×3**) | 1 | 3 × (5+35)×1,5 = **180** | (5+35)×1,5 + 2×(5+15)×1,5 = **120** |
| `RebelsDoubleEdge`+ (6 **×2**) | 1 | 2 × (6+35)×1,5 = **122** | (6+35)×1,5 + (6+15)×1,5 = **92** |
| **total del turno** | **3** | **~302** | **~212** |

- **Hoy Mordred está ~40% POR ENCIMA** de la banda de saturación de 180-220 (`DECISIONS.md:40`), no
  40% por debajo como decía la versión anterior de este documento. **La conclusión estaba invertida.**
- **Con el Candado 4 aterriza en ~212**, o sea en el borde superior de la banda — que es donde debe
  estar un personaje con el motor completamente armado en el acto 3.
- El resto del rediseño (básicas +1/+2 de daño, cargas de NP, estrellas) mueve el pico en el orden de
  las unidades: es **irrelevante** frente a esto. Los candados 1-3 son de *forma*; el techo lo fija
  el Candado 4.
- La `ChispaDeClarent` que manifiesta la starter al consumir un crítico está **capeada 1/turno**
  (`RedLightningSparkPower.cs:19-20`) y suma un AoE de 0⚡ encima de esa cuenta.

## 7. Restricciones duras verificadas

| restricción | estado |
|---|---|
| Ningún ID de mod/modelo/carta/power/reliquia renombrado | ✅ 0 |
| Ninguna carta borrada | ✅ 0 |
| Ningún cambio de rareza (cero DEMOTE) | ✅ 0 |
| Ninguna carta nueva | ✅ 0 |
| Cambio de mazo inicial | ⚠️ 1 (`Strike` → 2.º `ArtsCommand`). Sólo afecta runs **nuevas**; no renombra ni borra IDs |
| Superficie pública de FGOCore intacta | ✅ — se publica solo `MordredSaber` |
| Denominaciones 10/20/30/50/100 | ✅ el cambio **corrige tres** violaciones: 8 (`SparksOfTheHelm`), 5 (`KnightsSteadfastness`) y 15 (`TournamentFootwork` mejorada) |
| Localización: 5 idiomas por clave tocada | ⚠️ pendiente de implementación (eng/esp/zhs/kor/rus) |

---

## 8. Riesgos y perillas declaradas

| # | riesgo | perilla |
|---|---|---|
| R1 | El Arts a 30 NP acelera **todo** el ciclo de ultis; si el NP se siente barato, la ulti pierde peso | bajar el Arts a 20 NP **antes** de tocar cualquier otra fuente |
| R2 | El par espejo **totalmente mejorado** sigue dando +20 NP / +20★ por dos cartas de 0⚡ (≈ un `WarCry` de 1⚡ repartido en dos cartas). Está **acotado** por copias y por mano, y **medido**, no ignorado | si el playtest lo muestra abusivo: bajar las dos mejoras de +20 a +10 |
| R3 | El tope de 2 activaciones/turno del Estandarte puede dejarlo flojo como rara-de-arquetipo | subir a 3/turno (el mismo tope que la starter), nunca quitarlo |
| R4 | `LightningSpeed` a 20★/30★ en Rebelión podría empujar demasiado el arquetipo B | bajar la mejora de la rama de Rebelión a +0 y dejarla en 20★ |
| R5 | Quitarle los 5 NP al Buster resta **10** NP por ciclo (2 Busters, no 3) a un mazo que ya cambia mucho | devolvérselos (la cuenta de §5.1 sigue dando turno 3 con o sin ellos) |
| R6 | El Candado 4 es el cambio de mayor impacto del lote (−30% al pico) y **no está validado en runtime**. Si Mordred queda floja en el acto 3, es lo primero a mirar | revertir el gate de primer impacto en `TheMostRadiantSwordPower` **antes** que en `KnightOfRedLightningPower` (el +12 pesa más que el +8) |
| R7 | Cambiar `StartingDeck` no toca runs en curso: los jugadores con una run abierta **no** ven la mejora del Arts hasta empezar otra | ninguna; es la única forma save-safe de cambiar el mazo |

**Nada de esto está validado en runtime.** Todo el documento es análisis estático sobre `HEAD`.

---

## 9. Notas de implementación

1. `BannerOfRebellionPower` necesita un contador por turno con reset en `BeforeSideTurnStart`
   (patrón exacto de `ArmsPlayedPower.BeforeSideTurnStart` en Gilgamesh y de `AccumulatedHatred`).
   **El combate no se serializa** (verificado en el pase de Gilgamesh v0.1.19), así que un campo
   privado es correcto y no viola la regla de estado efímero de `DECISIONS.md`.
2. Las cuatro bi-condicionales sólo cambian constantes (`BaseStars`/`BaseNp`) y el `OnUpgrade`;
   la lógica de rama y el glow ya están bien escritos.
3. Localización: hay que reescribir la descripción de **9 cartas** (las que cambian de número
   visible) en **5 idiomas**, y verificar `!Var!` contra los `DynamicVars` reales. Correr
   `tools/audit_simpleloc.ps1` y `tools/audit_localization_parity.ps1`.
4. Verificación: build de `MordredSaber` (FGOCore no cambia ⇒ no hace falta el lote completo),
   `tools/build_compat_matrix.ps1` (main y beta en invocaciones separadas, por el OOM conocido),
   `audit_simpleloc`, `audit_localization_parity`, `audit_vfx_paths`, publish local a `dist/` con
   verificación de contenido del PCK y cero churn de `.import`.
5. Bump: `MordredSaber` v0.1.18 → v0.1.19 + `tools/workshop_desc/MordredSaber.txt` **en el mismo
   lote** (la ficha no se deja atrasada).

---

## 10. Registro de la revisión adversarial (Fable 5, 2026-08-21)

Revisión encargada sobre la versión anterior de este documento. **Cada hallazgo se verificó contra el
código antes de aceptarlo**; se aceptaron 12 de 14, se corrigió 1 por ser un dato equivocado del
propio revisor, y 1 se reclasificó.

| # | severidad | qué encontró | resolución |
|---|---|---|---|
| **E1** | ALTO | La composición del mazo inicial de D1 estaba tomada de `DESIGN-MORDRED.md`, no del código. El real tiene **1 Arts, no 2**, y un `Strike` | **ACEPTADO.** D1 rehecho (25 impresos + 10 de forma = 35/ciclo, primer NP turno ~6). Y la proyección «turno 3» era inalcanzable con un solo Arts ⇒ se agrega el swap `Strike`→`ArtsCommand` a §5.1 |
| **E2** | ALTO | El crítico multiplica **×1,5**, no ×2 (`Criticals.cs:52`) | **ACEPTADO.** Error mío: copié el ×2 del doc de diseño viejo en vez de leer la constante. §6 rehecho |
| **E3** | ALTO | `HundredShatteredSwords` hace **0 de daño** cuando el resolutor de críticos ya se llevó las 50★ | **ACEPTADO.** Bug en producción, ver D8. Es el hallazgo de más valor de la revisión |
| **E4** | ALTO | Mordred no tiene `ICriticalAccessRule`: todo Ataque elegible drena 50★ del banco | **RECLASIFICADO a MEDIO y documentado, no arreglado.** Sólo los **Ataques** son elegibles (`Criticals.IsEligible` exige `CardType.Attack`), así que las Habilidades que leen el banco (Tributo al Trono, Instinto de Batalla) **no** compiten. El único Ataque que gastaba ★ manualmente era Cien Espadas, y eso lo cierra D8. Queda como propiedad declarada: **con Mordred, las cartas que leen el banco se juegan antes que los Ataques** |
| **E5** | MEDIO | «`LightningVisit` son dos cambios de forma» es falso: el regreso entra con `source == null` y `FormSwitch` no notifica | **ACEPTADO.** Premisa mía equivocada; D5 y §5.3 corregidos |
| **E6** | ALTO | `SecretRevealedPower` es el mismo motor sin tope, más grande, y §5.4 lo declaraba limpio | **ACEPTADO.** Ver D9; §5.4 reescrito |
| **E7** | BAJO | La cuenta de D5 se quedaba corta: son 4 robos, no 3 | **ACEPTADO** |
| **E8** | MEDIO | El Candado 3 no hacía lo que su título decía: la mejora a +20 dejaba el mismo +20/+20 neto | **ACEPTADO**, opción (ii): mejora **+10** ⇒ neto +10/+10, y el título y el texto dicen la verdad |
| **E9** | MEDIO | El up «+10★» del Quick ignoraba el +10★ universal del motor de tipos | **ACEPTADO.** Up = +3 de daño solamente |
| **E10** | BAJO | D1 omitía el +5 NP/turno de la forma Enmascarado | **ACEPTADO**, agregado a la tabla |
| **E11** | BAJO | `TournamentFootwork` mejorada da 15 NP, denominación ilegal, y §7 no lo contaba | **ACEPTADO**, up +10 ⇒ 20 |
| **E12** | MEDIO | La conclusión «~40% por debajo del techo» está mal; el techo real lo fijan las multi-hit | **ACEPTADO y ES EL HALLAZGO QUE CAMBIA EL SIGNO DEL REDISEÑO.** Rehaciendo la cuenta con ×1,5, cartas mejoradas y multi-hit, Mordred está **~302 por turno, ~40% POR ENCIMA** de la banda. De ahí salen D7 y el **Candado 4**, que no existían en la versión anterior |
| **E13** | MEDIO | (Kagetora) La justificación de K-1 afirma que Reins gana en tasa al mejorarse, y es falso | **ACEPTADO** el arreglo de prosa. **RECHAZADO** el argumento extra del revisor de que «Prayer es Cielo y avanza Doctrina» diferencia a las dos: **las dos son `Precept.Heaven`** (verificado en `CommonCards.cs`), así que ese factor no las separa |
| **E14** | BAJO | «`DoctrinePowers.cs` implementa `ShouldClearBlock`» — en realidad lo **hereda** de `FormPower` | **ACEPTADO**, corregido en la auditoría de Kagetora |

**Lo que el revisor confirmó y quedó como estaba:** D2 (banda de 0⚡), D3 (los dos riders de forma con
delta 0, con glow y tooltip que prometen de más), D4 (dominación estricta común > poco común), D6 (el
espejo baja el costo en las dos direcciones), la cadena de retención de Bloqueo intacta tras
FGOCore v0.1.25, los topes reales de la starter, el cleanse acotado a dos vectores, y —en Kagetora—
la conectividad 72/77, el reparto 23/25/24, la cota de ≤1⚡ por turno, el cap de 1 crítico por turno y
**la conclusión de no rediseñar**.
