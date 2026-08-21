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

El problema no es la fantasía. **Es que el motor está invertido en tres lugares**, y en los tres
está medido.

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

Cuenta del mazo inicial (10 cartas: 3 Buster + 2 Arts + 1 Quick + 2 Defender + 1 Rebelión + 1 Bajar
la Visera), umbral de manifestación = 100:

| fuente | ahora | por ciclo de mazo |
|---|---|---|
| 2× Arts | 10 c/u | 20 |
| 3× Buster (`BusterCommand.cs:23`, `NpCharge` 5) | 5 c/u | 15 |
| 1× Bajar la Visera (`LowerTheVisor`, 5) | 5 | 5 |
| **total** | | **40** |

Un ciclo de mazo son ~2 turnos ⇒ **el primer NP cae recién en el turno ~5**. El objetivo declarado
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
3. `Cards/Uncommon/LightningVisit.cs` — **0⚡**: cambiás de forma y **volvés al final del turno** ⇒
   **dos** cambios por carta.

Juntas: con Estandarte mejorado ×2 stacks, un **Rugido de Rebelión mejorado (0⚡)** da
**+20★, +10 NP y robá 3** — se reemplaza a sí mismo con dos cartas de ganancia neta y encima paga
las dos economías. `LightningVisit` lo duplica. Eso es exactamente lo que la rúbrica prohíbe:
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

### Lo que se verificó y está BIEN (para no arreglar lo que no está roto)

- **Techo de daño por turno dentro del rango.** Ver §6: el pico auditado con motor completo es
  **~126** en un turno de 3⚡, contra el techo de saturación de 180-220. Mordred **no** es Mash: esto
  no es un nerf de potencia.
- **Cleanse acotado a los dos vectores que su kit justifica** (`SecretOfPedigreeEX` +
  `MagicResistanceBCharm`), como manda la regla negativa de `DESIGN-MORDRED.md §2`.
- **`HundredShatteredSwords`** (rara 0⚡, 26 de daño) **paga 50★** (`StarCost = 50`, gate en
  `IsPlayable`) — es el slot Cometa, no una carta gratis.
- **La starter no se apila con la reliquia de jefe**: `ClarentOverloadedWithHatred` silencia a
  `ClarentTheStolenSword` (fix del audit 2026-07-05), y la starter tiene tope de 3 procs/turno.
- **La retención de Bloqueo de las formas sobrevive al cambio de Baluarte de FGOCore v0.1.25.**
  `MordredFormPower` no implementa `ShouldClearBlock`, pero **hereda la implementación correcta de
  `FGOCore/.../Forms/FormPower.cs:59-73`**, que responde `false` y llama `BlockRetention.Enforce`.
  Auditados los 6 implementadores de `IBlockRetentionSource` fuera de FGOCore: **todos cumplen el
  contrato**. Ningún daño colateral del lote Mash V2.

---

## 3. Los tres candados

### Candado 1 — el mazo inicial carga; la común deja de saltearlo

La carga vuelve a donde el jugador la paga (el Arts, que gasta energía y una carta) y se va de donde
era gratis (una común de 0⚡ sin rider). Primer NP: turno ~5 → **turno 3**, el objetivo del ecosistema.

### Candado 2 — la forma es el pago; el switch es el costo

Las cuatro cartas bi-condicionales pasan a pagar **exactamente una denominación** (+10) por estar en
la forma correcta, con pisos legales. El motor de switch deja de reemplazarse a sí mismo y deja de
ser ilimitado por turno.

### Candado 3 — las conversiones no dan ganancia neta gratis

La mejora del par espejo sube la **salida** y deja el **costo fijo**, en vez de bajar el costo de las
dos direcciones.

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
| `BusterCommand` | 8 daño + 5 NP; up +3 | **10 daño**, sin NP; up +3 | §4.6.1; la carga se concentra en el Arts |
| `QuickCommand` | 5 daño + 20★; up +3 | **6 daño** + 20★; up +3 **/ +10★** | §4.6.1 |
| `LowerTheVisor` | 4 Bloqueo + enmascara + 5 NP; up +3/+5 | 4 Bloqueo + enmascara + **10 NP**; up +3 / **+10** | denominación legal; es la firma defensiva |
| `Defend`, `Strike`, `Rebellion` | — | **[=]** | ya están en estándar |

Primer NP tras el cambio: 2×30 + 10 = **70 por ciclo de mazo** + la starter (crítico consumido→10 NP)
⇒ **turno 3**.

### 5.2 Comunes — Candados 1 y 3

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `ManaIgnition` | 0⚡ +30 NP; up **+20 (⇒50)** | 0⚡ **+20 NP**; up **+10 (⇒30)** | banda 0⚡ del ecosistema (D2). La mejora la lleva a la banda de 1⚡: eso es lo que una mejora debe hacer |
| `SparksOfTheHelm` | 8 Bloqueo; **8** NP base / 20 Enmascarado | 8 Bloqueo; **10** NP base / **20** Enmascarado; up +3 Bloqueo | piso a denominación legal; el bonus de forma queda en exactamente +10 (D3) |
| `KnightsSteadfastness` | 13 Bloqueo; **5** NP base / 10 Enmascarado | 13 Bloqueo; **10** NP base / **20** Enmascarado; up +4 Bloqueo **/ +10 a la rama Enmascarado** | ídem (D3) |
| `SpoilsOfCamelot` | 50 NP → 50★; up **costo −20** | 50 NP → 50★; up **salida +20 (⇒70★)**, costo fijo | D6 |
| `TributeToTheThrone` | 50★ → 50 NP; up **costo −20** | 50★ → 50 NP; up **salida +20 (⇒70 NP)**, costo fijo | D6 |
| resto de las 20 comunes | — | **[=]** | sin hallazgo |

### 5.3 Poco comunes — Candado 2

| ID | ahora | V2 | por qué |
|---|---|---|---|
| `LightningSpeed` | 9 daño; 10★ base / **10★** Rebelión; up +3 daño | 9 daño; **10★ base / 20★ en Rebelión**; up +3 daño **y +10★ a la rama de Rebelión (⇒30★)** | D3 + D4: la condición pasa a existir, y la poco común gana a la común **solo en su forma** |
| `DentedHelm` | 11 Bloqueo; 10★ base / **10★** Enmascarado; up +4 Bloqueo | 11 Bloqueo; **10★ base / 20★ Enmascarado**; up +4 Bloqueo **y +10★ a la rama Enmascarado** | D3 |
| `RoarOfRebellion` | 0⚡ switch + robá 1; up **robá 2** | 0⚡ switch + robá 1; up **+10★** en vez del segundo robo | D5: el conector del arquetipo puede ser un cantrip neutro en cartas, no card-positivo |
| `BannerOfRebellion` (vía `BannerOfRebellionPower`) | por cada switch, sin tope | **máximo 2 activaciones por turno** (reset al inicio de tu turno) | D5. Idioma del propio repo: `AccumulatedHatred` (`MaxProcs = 2`), `ClarentTheStolenSword` (3/turno), `RoundTableFragment` |
| `LightningVisit` | 0⚡, dos switches por carta | **[=]** | con el tope del Estandarte deja de ser un multiplicador; se deja como está y se mide |
| resto de las 28 poco comunes | — | **[=]** | sin hallazgo |

### 5.4 Raras, especiales, reliquias, formas y NP

**[=] TODO.** No se toca ninguna rara, ninguna carta-NP, ninguna reliquia, ninguna forma, ni el
modelo de Carga NP. El diagnóstico no encontró defecto en esa capa y ampliar el diff sin evidencia
es exactamente lo que este documento evita.

**Total: 13 IDs re-especificados** (7 básicas/comunes + 4 poco comunes + 1 power + 1 mejora), de 78
cartas + 13 reliquias + 27 poderes.

---

## 6. Auditoría de pico (ANTES de implementar)

Escenario de motor completo, acto 3, contra un Jefe, 3⚡, con `KnightOfRedLightningAPlus` (Ataques
+2, Críticos +6), `TheMostRadiantSword` (Críticos +8), forma **Relámpago Carmesí** (Ataques +2) y un
Crítico Listo en cola. El orden de StS es **aditivo → multiplicativo**, así que el ×2 del crítico
multiplica todo lo aditivo.

| jugada | ⚡ | cuenta | daño |
|---|---|---|---|
| `ManaBurstA` (Ataques +4 este turno, Exhaust) | 1 | — | 0 |
| `LightningOfClarent` **con el crítico** | 2 | (18 +4 +2 +2 +6 +8) × 2 | **80** |
| `HundredShatteredSwords` (paga 50★) | 0 | 26 +4 +2 +2 | **34** |
| `InsolentStrike` | 0 | 4 +4 +2 +2 | **12** |
| **total** | **3** | | **~126** |

Contra el techo de saturación vigente del proyecto (**180-220**), Mordred con motor completo está
**~40% por debajo**. Con la carta-NP manifestada en lugar de `LightningOfClarent` el pico sube pero
consume los 100-300 del medidor, que es un recurso de varios turnos.

**Conclusión del audit: este rediseño NO baja potencia.** Los candados 1 y 2 son *net-neutrales o
levemente positivos* (el mazo inicial pega y carga más; dos poco-comunes pasan a pagar de verdad);
el único recorte real es `ManaIgnition` y el tope del Estandarte, y los dos atacan **generación
gratuita**, no daño.

---

## 7. Restricciones duras verificadas

| restricción | estado |
|---|---|
| Ningún ID de mod/modelo/carta/power/reliquia renombrado | ✅ 0 |
| Ninguna carta borrada | ✅ 0 |
| Ningún cambio de rareza (cero DEMOTE) | ✅ 0 |
| Ninguna carta nueva | ✅ 0 |
| Superficie pública de FGOCore intacta | ✅ — se publica solo `MordredSaber` |
| Denominaciones 10/20/30/50/100 | ✅ el cambio **corrige** dos violaciones (8 y 5) |
| Localización: 5 idiomas por clave tocada | ⚠️ pendiente de implementación (eng/esp/zhs/kor/rus) |

---

## 8. Riesgos y perillas declaradas

| # | riesgo | perilla |
|---|---|---|
| R1 | El Arts a 30 NP acelera **todo** el ciclo de ultis; si el NP se siente barato, la ulti pierde peso | bajar el Arts a 20 NP **antes** de tocar cualquier otra fuente |
| R2 | El par espejo **totalmente mejorado** sigue dando +20 NP / +20★ por dos cartas de 0⚡ (≈ un `WarCry` de 1⚡ repartido en dos cartas). Está **acotado** por copias y por mano, y **medido**, no ignorado | si el playtest lo muestra abusivo: bajar las dos mejoras de +20 a +10 |
| R3 | El tope de 2 activaciones/turno del Estandarte puede dejarlo flojo como rara-de-arquetipo | subir a 3/turno (el mismo tope que la starter), nunca quitarlo |
| R4 | `LightningSpeed` a 20★/30★ en Rebelión podría empujar demasiado el arquetipo B | bajar la mejora de la rama de Rebelión a +0 y dejarla en 20★ |
| R5 | Quitarle los 5 NP al Buster resta 15 NP por ciclo a un mazo que ya cambia mucho | devolvérselos (la cuenta de §5.1 sigue dando turno 3 con o sin ellos) |

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
