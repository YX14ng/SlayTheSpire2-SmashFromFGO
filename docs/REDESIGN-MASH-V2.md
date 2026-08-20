# REDESIGN-MASH-V2 — Mash Kyrielight, la muralla que se gasta

> **IMPLEMENTADO 2026-08-20** (orden directa del usuario). FGOCore **v0.1.25**, Mash **v0.1.21**,
> Siegfried **v0.1.22**, Tiamat **v0.1.20**. Build 0 warnings / 0 errores en los 13 proyectos; matriz
> MAIN/BETA verde 3/3 (main, beta, probe main→beta); `audit_simpleloc` 0 ambigüedades; paridad de
> localización 13 proyectos × 5 idiomas; `audit_vfx_paths` OK. Notas de implementación sobre el
> diseño: (a) el keyword `Descargar` es `[CustomEnum]` en `MashKeywords`, con tooltip en 5 idiomas y
> cierre dorado **explícito** `*Descargar*` en todos los idiomas (no sólo zhs) — el terminador ASCII
> no cubre los dos puntos; (b) el multiplicador de forma vive en `Descarga.Multiplier`, un camino
> único que también dispara el flotante; (c) marcadores nuevos `IDischargeCard` e `IBulwarkCard` —
> el primero apaga el peaje de Ortinax (y su rama de preview, que en MAIN habría sumado daño real),
> el segundo evita el doble stack de la pasiva nueva de Shielder; (d) `DistantUtopiaCastlePower`
> guarda su altura en `Amount` y su mejora ya **no abarata**, sube la altura (precedente «Corte del
> Invierno»); (e) `LordCamelotUnleashed` recibe la misma Intercepción permanente que la drafteable —
> la misma NP no puede dar cosas distintas según venga del mazo o del auto-manifest;
> (f) `LordCamelotChargePower` queda inerte, sin borrar. **Pendiente: playtest y publish** (el
> publish necesita orden explícita).
>
> **Estado original: PROPUESTA APROBADA POR PANEL.**
> Origen: reporte de **Moopamoop** (Steam, 2026-08-20): *«Agreed with previous comments about the
> character being way, way too powerful. She's very fun but it's trivially easy to build up absurd
> amounts of block and become practically invincible, from there how you win is trivial.»*
> Encargo del usuario: rediseñar a Mash **bajo los mismos términos que Morgan V2**.
>
> Mismos términos, uno por uno: (a) panel de diseño de WORKFLOW-FGO §4.6.7 — propuestas con lentes
> distintas + jueces adversariales, **los parches del juez MANDAN**, contradicciones resueltas al más
> restrictivo y anotadas; (b) **save-safe: ningún ID se renombra**, el re-pool es re-efecto sobre IDs
> existentes; (c) **cero demotes nuevos**; (d) matriz de arquetipos drafteables; (e) un **keyword
> dorado** que arregla la legibilidad del motor; (f) auditoría de pico documentada ANTES de
> implementar; (g) knobs de playtest declarados como números, no como lógica.
>
> Diferencias con Morgan V2, declaradas: **cero cartas nuevas y cero reliquias nuevas** (Morgan agregó
> 4 + 1) — acá el problema es de reglas, no de cobertura, y el pool ya tiene las cuatro líneas. En
> cambio **sí se toca FGOCore**, cosa que el pase de Morgan evitó: la semántica de Baluarte vive ahí y
> el arreglo no puede ser local (§13, radio de impacto sobre Siegfried y Tiamat).
>
> **Revisión adversarial 2026-08-20 (Fable 5, sólo lectura):** encontró 5 errores fácticos y 2
> huecos técnicos que resucitaban el bug; todos aplicados en esta versión del documento y anotados
> como parches **F1-F9** en §9.3. Veredicto: la idea elegida (P2) es la correcta, la enmienda es de
> **mecanismo, no de diseño**. Las cuentas de pico de §2.5 y §11 fueron rehechas y **corregidas a la
> baja**: las originales estaban infladas.
>
> El panel corrió **en una sola sesión, sin subagentes** (regla activa de la sesión). Las tres
> propuestas y los tres jueces son lentes adversariales aplicadas por el mismo autor, no tres agentes
> independientes; se anota acá para no inflar la evidencia. Todo hallazgo de §2 está **verificado
> contra el código**, con archivo y línea.

---

## 1. Identidad

**Hoy (rota):** *«la muralla que avanza»* — pero la muralla no cuesta nada, no se gasta nunca y encima
es el arma. Bloqueo = defensa + daño + escalado, gratis y monótono creciente.

**Después:** ***«La muralla es munición: cada turno Mash decide cuánto escudo se queda de pie y cuánto
sale disparado — y nunca puede tener las dos cosas.»***

La fantasía no cambia (Lord Camelot es un escudo que se usa como ariete, eso ES el personaje). Lo que
cambia es que el escudo **se consume al usarse como ariete** y que **el muro hay que volver a
levantarlo todos los turnos**.

---

## 2. Diagnóstico verificado (la evidencia, no la impresión)

### 2.1 Baluarte es Barricada vendida en rareza COMÚN

`FGOCore/FGOCoreCode/Block/BlockRetention.cs:66`:

```csharp
return creature.GetPowerAmount<BulwarkPower>() + best;   // cap = SUMA de Baluarte + MAX de fuentes
```

`BulwarkPower` (`Block/BulwarkPower.cs`) es un `Counter` que **no decae en ninguna parte**: no tiene
hook de decremento, nadie lo remueve, y `GainBulwarkBlock` aplica stacks **iguales al Bloqueo ganado**
(`BlockRetention.cs:90` y `:100`). Consecuencia exacta: **cada punto de Bloqueo con Baluarte sube el
techo de retención para SIEMPRE en ese combate.**

Eso convierte a `FirmStance` (**COMÚN**, 1⚡, 6 de Bloqueo con Baluarte) en *Barricade a rareza común*.
El baseline de la skill §2 para una común de 1⚡ es **7-9 de Bloqueo que se BORRA**. Acá se pagan 6 que
**no se borran nunca**. No hay penalidad que lo pague (skill §3): ni Agotar, ni Vida, ni condición.

### 2.2 Hay ocho fuentes que apilan sobre ese techo, dos de ellas automáticas por turno

| Carta / power | Rareza | Baluarte que aporta |
|---|---|---|
| `FirmStance` | **Común** 1⚡ | 6 (repetible cada turno) |
| `MoldCamelot` | PC 2⚡ | 11 |
| `SnowflakeWall` | PC 2⚡ | 14 |
| `IronWill` | PC 1⚡ Power | **+4 al final de CADA turno, para siempre** |
| `DemiServant` | Rara 2⚡ Power | **+5 al inicio de CADA turno, para siempre** |
| `FarewellSnowfield` | Rara 2⚡ | 20 |
| `LordChaldeas` / `LordCamelot` (+ sus *Unleashed*) | NP | 23-35 **+3-4 por cada 10 de sobrecarga** |
| `RoundTableFragment` (starter) / `LordCamelotRestored` | Starter / Ancient | piso fijo 10 / 25 (esto SÍ está acotado) |

`IronWill` y `DemiServant` son los culpables estructurales: **suben el techo solos, sin gastar cartas,
sin tope y sin fin.**

### 2.3 Y encima hay multiplicadores sobre el mismo eje

- `DefensiveFormation` (PC, 1⚡): *«tus cartas de Bloqueo se juegan dos veces este turno»* — aplicada
  con amount **99** (`Cards/Uncommon/DefensiveFormation.cs` → `PowerVar("DefensiveFormation", 99m)`),
  o sea **todas**, o sea duplica el crecimiento del techo de ese turno.
- `PrayerToGalahad` (PC, 1⚡, Agotar): duplica tu Bloqueo (máx +18).
- `UtopianFortress` (Rara, 2⚡): Bloqueo = 50% de tu Carga NP, **máx 60, sin gastar la carga**.
- `DistantUtopiaCastle` (Rara, 3⚡): retención **infinita** (`RetentionCap => decimal.MaxValue`).
- Forma Shielder: +3 al primer Bloqueo de cada turno. `MashBond`: regalos de Bloqueo inicial (Lv 4/7).

### 2.4 El pecado capital: los payoffs LEEN el Bloqueo y NO lo gastan

| Carta | Coste | Efecto | ¿Gasta el Bloqueo? |
|---|---|---|---|
| `CamelotRam` (PC) | 2⚡ | daño = tu Bloqueo | **NO** (`Cards/Uncommon/CamelotRam.cs:14`) |
| `RoundTablePunishment` (Rara) | 3⚡ | daño = tu Bloqueo **a TODOS** | **NO** (`Cards/Rare/RoundTablePunishment.cs:15`) |
| `LordCamelotCharge` (Rara) | 2⚡ | daño = tu Bloqueo, 1/turno | **NO** — el docstring lo dice con todas las letras: *«No consume el Bloqueo: la muralla embiste sin bajar la guardia»* |

Defensa y ofensa son **la misma pila**, y la pila **nunca baja**. Esa es la frase del reporte
(*«from there how you win is trivial»*) traducida a código.

### 2.5 Auditoría de pico ANTES (acto 3, draft defensivo razonable, turno 6)

Techo de retención acumulado (cuenta corregida por la revisión, parche **F3** — la primera versión de
este documento contaba mal los procs y decía 87):

| Fuente | Procs reales al turno 6 | Aporte |
|---|---|---|
| `IronWill` (jugada t2; proca en `BeforeSideTurnEnd`) | fin de t2, t3, t4, t5 = **4** | 16 |
| `DemiServant` (jugada t3; proca en `AfterPlayerTurnStart`, **no** el turno en que se juega) | inicio de t4, t5, t6 = **3** | 15 |
| `FirmStance` ×2 | — | 12 |
| `SnowflakeWall` | — | 14 |
| `MoldCamelot` | — | 11 |
| `RoundTableFragment` (término `best`, no suma) | — | 10 |
| | | **≈ 78** |

- Daño entrante de élite/jefe de acto 3 ≈ 30-45/turno; el motor pasivo aporta **+9/turno sin gastar una
  sola carta**. **Invulnerable en cuanto el muro pasa el entrante — turno 5-6.**
- Ofensiva: `RoundTablePunishment` (3⚡) = **~78 a TODOS** + `LordCamelotCharge` (2⚡) = **~78 a uno**.
  **Corrección honesta (F3):** ese turno gastás los 5⚡ en pegar, así que el muro sólo crece +9 contra
  30-45 entrantes ⇒ el pico **no** es "174 por turno con el muro intacto"; es **~140-155 UNA vez**, y
  después hay que esperar 2-3 turnos a que el muro se recomponga.
- Aun corregido, el defecto queda intacto: **el pico no cuesta nada, no se gatea, y el piso desde el
  cual se dispara sube solo todos los turnos.** Lo que está roto es el ratchet, no el número.

**Conclusión del panel: el reporte no es una queja de números. Es un defecto de reglas.** Bajar
valores no lo arregla: mientras Baluarte acumule sin tope y los payoffs no gasten, cualquier número
converge al mismo estado terminal, sólo que dos turnos más tarde.

### 2.6 Daño colateral del defecto: las formas no deciden nada

Regla de la skill §5: *«cada forma debe cambiar las DECISIONES, no sólo los números»*. Con Bloqueo
infinito, el costo de Ortinax (consume hasta 5 de Bloqueo por Ataque, −1 de Bloqueo por carta) es
**ruido estadístico**. Las tres formas juegan las mismas cartas igual. Están rotas por el mismo motivo.

---

## 3. Los tres candados (el arreglo estructural)

### CANDADO 1 — Baluarte deja de ser Barricada: es una **prórroga de UN turno**

**Texto nuevo del keyword (5 idiomas):**
> **Baluarte N** — *«Al inicio de tu próximo turno conservás hasta N de Bloqueo. Después el Baluarte
> **se gasta**: el muro hay que volver a levantarlo.»*

Comparar con el texto actual (*«No se quita al inicio de tu turno»*), que es exactamente la frase que
hizo que jugadores **y diseñador** lo leyeran como Barricada permanente (parche J2-2).

- La suma **dentro de un mismo turno se conserva**: podés apilar `SnowflakeWall` + `MoldCamelot` +
  `FirmStance` y llevarte 33 al turno siguiente. Ése es el "turno de muro grande", y se paga con el
  turno que no pegaste.
- Lo que muere es la **acumulación entre turnos**. `IronWill` y `DemiServant` pasan de "+4/+5 al techo
  para siempre" a "+4/+5 de piso garantizado cada turno", que es un power de PC/rara perfectamente
  sano.
- **`DistantUtopiaCastle` queda como la ÚNICA Barricada real** — 3⚡, rara, con tope numérico (§6.3).
  Es el clímax drafteado, como manda vanilla, no el estado por defecto del personaje.

**Dónde va el código — versión corregida (parche J3-1 enmendado por F1/F2/F5):**

El reset **NO** puede vivir en `BulwarkPower.AfterPreventingBlockClear`: el juego elige **un solo**
preventer (`Creature.cs:718-728` → `Hook.ShouldClearBlock` devuelve **el primero** que dice que no,
`Hook.cs:2193-2205`), así que si gana la reliquia, el hook del power no corre.

La primera versión de este documento mandaba el reset a `BlockRetention.Enforce`. **Está mal, por dos
huecos que la revisión encontró:**
- `DistantUtopiaCastlePower.AfterPreventingBlockClear` (`DistantUtopiaCastlePower.cs:24-28`) **no llama
  a `Enforce`**: sólo hace `Flash()`. Y como el reset re-aplica `BulwarkPower` cada turno, el power
  reaplicado se va **al final** del orden de listeners ⇒ con el Castillo en juego, **el Castillo gana
  siempre la carrera** y no habría ni reset ni trim. Con el tope nuevo de 40 (§6.3) eso ya no es
  inofensivo: la carta diría 40 y retendría todo.
- Con un preventer **vanilla** temporal (Blur) ganando N turnos, los stacks quedan rancios y al expirar
  el primer `Enforce` retiene contra un techo inflado.

**Anclaje correcto: el hook vanilla `AfterBlockCleared`.** `CombatManager.cs:500-507` lo dispara para
**cada** criatura que empieza turno, **incondicionalmente**, después de la fase de clear/prevención —
gane quien gane la carrera, y haya o no Bloqueo en pie. Es el mismo hook que usa el vanilla
`BlockNextTurnPower.cs:19`, y el mismo que ya nos arregló el Bloqueo diferido de Astolfo.

- **Reset**: `BulwarkPower.AfterBlockCleared(creature)` → si `creature == Owner`, `PowerCmd.Remove(this)`.
  Inmune al preventer único, cubre `block == 0` sin tocar guards.
- **Trim**: se queda en `BlockRetention.Enforce`, sin cambios de guard.
- **F1**: `DistantUtopiaCastlePower.AfterPreventingBlockClear` **debe delegar en `Enforce`** (patrón
  `BulwarkPower.cs:27`) para que su tope de 40 se aplique de verdad — incluidas las copias que la carta
  pone en los aliados en co-op (`DistantUtopiaCastle.cs:22-25`).
- **F5**: el corte por `block == 0` está **duplicado** (`BlockRetention.cs:73` y
  `BulwarkEngineRelic.cs:105`). Con el reset fuera de `Enforce` ninguno de los dos hace daño, pero se
  anotan para que el próximo lector no crea que el de la reliquia es intencional.

### CANDADO 2 — Keyword `Descargar`: lo que convierte el muro, **lo gasta**

Es el equivalente estructural del `Detonar` de Morgan V2: el motor ya existía en el código, lo que
faltaba era **nombrarlo, hacerlo visible y aplicarlo de forma consistente**.

**Keyword dorado, tooltip único en 5 idiomas:**
> **Descargar** — *«Consume tu Bloqueo (hasta la cantidad indicada; si no se indica, TODO) y lo
> convierte en el efecto de la carta.»*

| Idioma | Término |
|---|---|
| eng | **Discharge** |
| esp | **Descargar** |
| zhs | **倾泻** (火力倾泻; concuerda con `气缸放电` de `CylinderDischarge`) |
| kor | **방출** |
| rus | **Разряд** |

- **Cero API nueva**: `Extensions/BlockExtensions.cs` ya tiene `ConsumeAllBlock` y `ConsumeBlockUpTo`,
  usados hoy por `BunkerBolt`, `Crush` y `OrtinaxMaintenance`. Las tres cartas del §2.4 pasan a usar
  el mismo helper.
- **Glow dorado** en toda carta con Descargar cuando tenés Bloqueo > 0; **condición vacía = sin glow**
  (regla heredada del parche J2-5 de Morgan).
- **Flotante propio "¡Descarga! X"** reusando la escena vanilla `vfx_blocked_text` con el label
  reescrito — el mismo truco del "¡Sentencia! +X" de Morgan (cero Nodes C# de mod ⇒ inmune al bug del
  bridge en Linux nativo). El jugador tiene que **ver** que el muro se convirtió, no que se perdió.

### CANDADO 3 — Las formas vuelven a ser una decisión

Con los candados 1 y 2, el Bloqueo es escaso y el gasto duele, así que las pasivas por fin dividen el
juego en dos preguntas distintas:

| Forma | Pasiva | La decisión que crea |
|---|---|---|
| **Shielder** | +3 al primer Bloqueo del turno · terminás con ≥8 de Bloqueo → +5 NP · **[NUEVO] el primer Bloqueo que ganás cada turno también es Baluarte** | *quedarme de pie*: piso de muro sin gastar cartas de Baluarte |
| **Ortinax** | tus Ataques **Descargan** hasta 5 de Bloqueo por +daño · tus cartas dan 1 menos de Bloqueo · **[NUEVO] tus efectos de Descargar convierten ×1.5** | *disparar*: el turno que entrás a Ortinax es el turno que vaciás el muro |
| **Paladín** (rara, permanente) | las dos, sin penalidad | el clímax drafteado, no el punto de partida |

El `[NUEVO]` de Shielder se implementa en `MashFormPower.AfterBlockGained` (ya existe el flag
`_blockCardBonusUsed` que marca "primera carta de Bloqueo del turno"): aplicar `BulwarkPower` por ese
monto. Bounded por construcción: **una carta por turno, un turno de duración.**

**Parche F6 (doble conteo):** ese hook corre TAMBIÉN cuando el Bloqueo ya vino por
`GainBulwarkBlock`. Si la primera carta del turno es `FirmStance`, sin guard se aplicarían **dos**
tandas de stacks (14 por 7 de Bloqueo) y el techo quedaría por encima del Bloqueo real. La pasiva
sólo aplica a **Bloqueo que no sea ya de Baluarte** — el guard es obligatorio, no opcional.

---

## 4. Matriz de arquetipos (el encargo de multi-arquetipo, igual que Morgan §2)

| | **A. La Muralla** | **B. Artillería Ortinax** | **C. Carga NP / Lord Camelot** | **D. Galahad: crítico y danza** |
|---|---|---|---|---|
| Motor | Baluarte + **Intercepción**: bloquear ES pegar | **Descargar**: el escudo es munición | cargar el medidor → NPs | Estrellas + cambio de forma → robo |
| Fantasía | el escudo de Camelot aguanta | el cañón de Ortinax escupe el muro | «LORD CAMELOT, despliegue» | Galahad presta el filo |
| **Ataque** | Reprisal, Knight's Vow, Frontal Charge, Shield Ram | Crush, Bunker Bolt, Camelot Ram, Round Table Punishment, Lord Camelot Charge | Siege Lance, Cylinder Discharge, las 4 NP | Quick, Defensive Sweep, Vanguard Strike, Paladin's Assault |
| **Defensa** | Firm Stance, Shields Up, Snowflake Wall, Iron Will, Demi-Servant, Absolute Wall | Ortinax Servos, Mobile Wall, Paradox Cylinder | Utopian Fortress, Lord Chaldeas | Tragic Shield, Guard Step |
| **Consistencia** | Chaldea Sandwich, Fou's Miracle | Mash's Glasses | Reinforcement Order, Chaldea Library | Chaldea Manual, Combat Analysis, **Homunculus Heart** |
| **Energía** | — | Descargar convierte, no cuesta ⚡ extra | Last Order, Combat Breathing (0⚡) | Form Drill (0⚡), Switch Shielder (0⚡) |
| **Escalado** | **Intercepción** (Tireless Guardian, Amalgam Goad, Lord Camelot) | forma Ortinax ×1.5 | niveles NP (gacha), Pioneer of the Stars | Estrellas → críticos, Conceptual Ammo |
| **AoE** | Round Table Punishment (compartida con B) | Defensive Sweep, Rhongomyniad | — | Black Barrel Burst |
| **Jefes que limpian buffs** | Intercepción es un power propio, no un debuff | Black Barrel **ignora Bloqueo y strippea** | los NP no son debuffs | Estrellas son recurso, no buff |
| **Debilidad real** | pegás poco por turno; contra multi-hit chico la Intercepción no alcanza | quedás en 0 de Bloqueo el turno que disparás | pico diferido; frontload flojo | depende del robo y de la bolsa de estrellas |

**La línea A deja de ser el ganador automático**: su payoff pasa a ser **Intercepción** (daño que NO
gasta el muro pero que está capado por cuántos golpes te tiran), no *"daño = mi pila infinita"*. Quien
quiera convertir el muro en daño grande tiene que pasar por B y **quedarse desnudo**.

Puerta de cada línea en **COMÚN** (regla 4.6.2): A → `Provoke`/`ShieldsUp`; B → `Crush`/`OrtinaxMaintenance`;
C → `CombatBreathing`/`ReinforcementOrder`; D → `GuardStep`/`ChaldeaManual` + **`FormDrill` promovida a
común** (§6.2, el parche M2 de Morgan aplicado acá: el interruptor de formas tiene que aparecer en
toda run).

---

## 5. Mazo inicial y básicas — **SIN CAMBIOS**

Verificado contra `Character/MashShielder.cs:31-42`: 2× Golpe, 2× Defender, 1× Buster, 2× Arts,
1× Quick, `ShieldBash`, `ProtectSenpai`. HP 80 (tope del rango de ecosistema para la tanque). Las 4
básicas conservan sus números exactos (Golpe 6 / Defender 5 / Buster 10 / Arts 6+30 NP / Quick 6+20★).
`ProtectSenpai` (1⚡, 8 Bloqueo + 10 NP) y `ShieldBash` (1⚡, 9 daño + 10 NP, la firma
`ITranscendenceCard`) tampoco se tocan.

Razón: el reporte no cuestiona el arranque, y el mazo inicial ya abre las cuatro puertas en el turno 1.

---

## 6. Pool — re-spec sobre IDs existentes (cero cartas nuevas, cero demotes)

*Marcas: **[GASTA]** = pasa a Descargar · **[NÚM]** = sólo número · **[RAREZA]** = cambia de rareza ·
**[=]** = sin cambios. Todo lo que no aparece en las tablas es **[=]**.*

### 6.1 Comunes

| Carta | Antes | Después | Por qué |
|---|---|---|---|
| `FirmStance` | 1⚡ **6** Baluarte | 1⚡ **7** Baluarte **[NÚM]** | Contraintuitivo y correcto: la carta **mejora** mientras el arquetipo se acota. Parche **F7**: la primera versión decía 8, pero la skill §2 tasa la común de 1⚡ **con rider** en 4-7 — 8 + retención estaba sobre-tasa según la tabla que este mismo documento invoca. Arranca en **7**. |
| `Crush` | 1⚡ 8 daño + consume hasta 10 | igual, etiquetado **Descargar hasta 10** **[GASTA]** | Ya gastaba; ahora se llama como se llama y brilla. |
| `OrtinaxMaintenance` | 1⚡ pierde TODO el Bloqueo → NP (máx 30) | igual, etiquetado **Descargar** **[GASTA]** | Ídem: es el prototipo del keyword. |
| `ShieldsUp` | 2⚡ 12 Bloqueo + **3** Intercepción | 2⚡ 12 Bloqueo + **5** Intercepción **[NÚM]** | El payoff de la línea A se muda a Intercepción. |
| `CoveringFire` | 0⚡ 5 daño, exige 8 Bloqueo | **[=]** | El gate de 8 sigue siendo alcanzable dentro del turno. Verificado: la economía nueva no rompe ningún gate (§10). |
| `Provoke` | 0⚡ **4** Intercepción | **[=]** | Parche **F4**: la primera versión de este documento la listaba como "3 → 4". **Ya es 4** (`Cards/Common/Provoke.cs:13`, mejora +3). Fila corregida y cambio retirado: la línea A ya recibe su compensación en `ShieldsUp`, `Reprisal` y `LordCamelot`. |
| `SharpenedEdge` | 1⚡ 5 daño +1 por Bloqueo (máx 8) | **[=] deliberado** | Lee el muro sin gastarlo, o sea contradice «lo que convierte, gasta» — pero con tope 8 es Intercepción-lite, no un payoff de Descargar. Se anota explícito (**F9**) para que el próximo panel no lo reporte como inconsistencia. |

### 6.2 Poco comunes

| Carta | Antes | Después | Por qué |
|---|---|---|---|
| `CamelotRam` | **2⚡** daño = tu Bloqueo (no lo gasta) | **1⚡ Descargar**: daño = Bloqueo consumido **[GASTA]** | Body Slam honesto: más barata porque ahora **cuesta el muro**. Mejora: **la conversión pasa a ×1.5**, NO `-1⚡` (parche **F8**: la mejora actual es `EnergyCost.UpgradeBy(-1)`; a 0⚡ sería «vaciá el muro gratis»). |
| `DefensiveFormation` | 1⚡, amount **99** (= TODAS tus cartas de Bloqueo, dos veces) | 1⚡, amount **2** (tus próximas 2 cartas de Bloqueo) **[NÚM]** | Parche J1-2. El power ya decrementa por uso (`AfterModifyingCardPlayCount`); el 99 era un "todas" disfrazado de contador. Cambio de **una constante**. |
| `PrayerToGalahad` | 1⚡ Agotar, duplica Bloqueo (máx +18) | máx **+15** **[NÚM]** | El Bloqueo duplicado **no** es Baluarte (ya era así) ⇒ se evapora salvo que también lo Baluartees. Sólo se recorta el tope. |
| `IronWill` | Power, +**4** Baluarte al final del turno | +**5** **[NÚM]** | Pasa de "sube el techo para siempre" a "piso de 5 cada turno": el buff compensa la pérdida de la acumulación. |
| `SwitchOrtinax` | **1⚡** + 10 Bloqueo | **0⚡** + 8 Bloqueo **[NÚM]** | Paridad exacta con `SwitchShielder` (0⚡). Lección M1 de Morgan: **volver no puede costar más que ir**. |
| `FormDrill` | **Poco común**, 0⚡ toggle | **COMÚN**, 0⚡ toggle **[RAREZA]** | Lección M2 de Morgan: el interruptor de formas tiene que aparecer en **toda** run, si no la mitad del motor no existe. Cambio de campo `rarity`, **no** de ID ⇒ saves intactos. |
| `BunkerBolt` | 2⚡ 12 + Bloqueo consumido ÷2 | igual, etiquetado **Descargar** **[GASTA]** | Ya gastaba. |
| `Reprisal` | 1⚡ 7 daño + **4** por golpe que tu Bloqueo frenó | + **5** **[NÚM]** | Payoff de A que no toca la pila. |

### 6.3 Raras

| Carta | Antes | Después | Por qué |
|---|---|---|---|
| `RoundTablePunishment` | **3⚡** daño = tu Bloqueo a TODOS, **no lo gasta**, escala con Fuerza | **2⚡ Descargar**: daño = Bloqueo consumido a TODOS, **`Unpowered`** (no escala con Fuerza) **[GASTA]** | El peor infractor del §2.4. Baja de precio porque ahora **te deja desnuda**; pierde el escalado con Fuerza (parche J1-1) para que no se combine con `KnightsVow`/`LordCamelot` en un doble motor. |
| `LordCamelotCharge` | 2⚡ daño = tu Bloqueo, `Unpowered`, **1/turno**, no lo gasta | **2⚡ Descargar**: daño = Bloqueo consumido **×1.5**, `Unpowered`, **sin candado de 1/turno** **[GASTA]** | El candado 1/turno era un parche sobre el síntoma; ahora se autolimita solo (hay un solo muro). El ×1.5 la distingue de `CamelotRam` como la versión rara. `LordCamelotChargePower` queda **inerte, no se borra** (save-safety). |
| `DistantUtopiaCastle` | 3⚡ Power: **TODO** tu Bloqueo persiste, sin tope | 3⚡ Power: todo tu Bloqueo persiste, **hasta un máximo de 40 (mejora: 60)** **[NÚM]** + **delega en `Enforce`** | Parche J1-4, injerto de la Propuesta 1. Sigue siendo la Barricada de Mash y el clímax de la línea A, pero con una altura de castillo declarada. Implementación: `RetentionCap` devuelve 40/60 en vez de `decimal.MaxValue`, **y `AfterPreventingBlockClear` tiene que llamar a `BlockRetention.Enforce`** o el tope no se aplica nunca (parche **F1**, §3 CANDADO 1). |
| `UtopianFortress` | 2⚡ Agotar, Bloqueo = 50% de la Carga (máx **60**) | máx **40** (mejora: 60) **[NÚM]** | Parche J1-3: con el banco de 300, 60 por 2⚡ era ×4 Impervious leyendo un recurso que ni se gasta. |
| `LordCamelot` (y `LordCamelotUnleashed`) | NP: Baluarte 23 (+4/10 OC) + **3** Fuerza (mejora 4) + Intercepción de turno | + **3 de Intercepción PERMANENTE**, que **apila** entre casteos **[NÚM]** | Con Baluarte de un turno, una NP de 3⚡ + medidor lleno que sólo da muro de un turno queda floja. La compensación es temática (el escudo de Camelot **es** el contraataque) y **puentea C→A**, que es justo lo que pedía la matriz. |
| `DemiServant` | Power, +**5** Baluarte al inicio del turno | +**6** **[NÚM]** | Mismo criterio que `IronWill`. |
| `AbsoluteWall` | 2⚡ Agotar, tu Vida no baja hasta tu próximo turno | **[=]** | Es Intangible: un turno, Agotar, rara. Está en tasa vanilla. Se deja con knob declarado (§12). |

### 6.4 Especiales / no drafteables

`LordCamelotUnleashed` / `LordChaldeasUnleashed` / `BlackBarrelUnleashed` / `BehindMeSenpai`: sólo
heredan el texto nuevo de Baluarte y el `[NÚM]` de `LordCamelot`. **Sin cambios de estado.**

---

## 7. Reliquias — sin cambios

`RoundTableFragment` (piso 10, 3 procs/turno) y `LordCamelotRestored` (piso 25) son
`IBlockRetentionSource` **con tope fijo**: nunca fueron el problema (son el término `best`, no la
suma). Se quedan exactamente como están, incluido el motor golpe-bloqueado→Estrellas y
perder-Vida→Carga. `MashBond`, `FouAmulet`, `SpareGlasses`, `OrtinaxCore`, `ATeamDiary`,
`SummonTicket`, `HolyGrail`: **[=]**.

---

## 8. Noble Phantasms y la ventana

**Sin cambios de modelo.** Mash sigue con la ulti auto-manifestada a 100 (`MainFile.cs:52`,
`TryManifestUlt` → la NP de la forma activa, Retain + Agotar, marcador `CamelotManifestedPower`). El
modelo de "ventana" de `REDESIGN-MASH.md` §A/§B **queda archivado**: nunca se implementó, el
ecosistema entero (9 personajes) se estandarizó en el auto-manifest el 2026-06-26, y meterlo en el
mismo pase que un cambio de reglas de Baluarte mezcla dos experimentos. Se anota como camino
abandonado, no como pendiente.

Lo único que cambia en las NPs es §6.3 (`LordCamelot` +3 Intercepción permanente) y que su Baluarte
ahora dura un turno.

---

## 9. Registro del panel

### 9.1 Las tres propuestas

| | Lente | Tesis |
|---|---|---|
| **P1 — TOPE** | matemática | Dejar todo como está y poner un techo numérico duro a la retención (p. ej. 40). Barato, quirúrgico, cero radio de impacto en FGOCore. |
| **P2 — MUNICIÓN** ★ | economía | El defecto es que el Bloqueo es a la vez defensa, arma y escalado, **gratis y monótono**. Baluarte pasa a durar un turno y todo lo que convierte, **gasta**. |
| **P3 — ARQUETIPOS** | draft | El pool tiene una línea dominante; repartir los payoffs entre cuatro líneas y quitarle a la línea A su nuke escalable. |

### 9.2 Ganadora: **P2 — MUNICIÓN, 3–0**

Motivos convergentes:
- **J1 (pico):** P1 acota el estado terminal pero no lo elimina — con techo 40 seguís siendo
  invulnerable contra la mayoría de los ataques del acto 2 y `RoundTablePunishment` sigue pegando 40 a
  todos, gratis, cada turno. Sólo P2 rompe la identidad "una pila, dos usos".
- **J2 (legibilidad):** el reporte dice *«trivially easy»*. Un tope invisible no cambia lo que el
  jugador **lee** en `FirmStance`. `Descargar` + el texto nuevo de Baluarte sí.
- **J3 (save-safety):** P2 se implementa con re-efecto sobre IDs existentes y una constante en
  `DefensiveFormation`; no necesita cartas nuevas ni demotes. Es la propuesta que **menos** superficie
  de save toca, contra la intuición.

### 9.3 Parches obligatorios aplicados (mandan sobre el diseño base)

| # | Juez | Parche | Dónde quedó |
|---|---|---|---|
| J1-1 | pico | `RoundTablePunishment` pasa a `Unpowered` además de Descargar | §6.3 |
| J1-2 | pico | `DefensiveFormation` 99 → 2 | §6.2 |
| J1-3 | pico | `UtopianFortress` tope 60 → 40 | §6.3 |
| J1-4 | pico | `DistantUtopiaCastle` necesita un techo numérico (injerto de P1) | §6.3 |
| J2-1 | legibilidad | `Descargar` va como **keyword dorado con tooltip propio en 5 idiomas**, no como texto suelto en cada carta | §3 CANDADO 2 |
| J2-2 | legibilidad | El texto de Baluarte debe decir **«se gasta»**; el actual («no se quita al inicio de tu turno») es la causa raíz del malentendido | §3 CANDADO 1 |
| J2-3 | legibilidad | Flotante propio **«¡Descarga! X»** reusando `vfx_blocked_text` | §3 CANDADO 2 |
| J2-4 | legibilidad | Glow dorado en Descargar sólo con Bloqueo > 0 | §3 CANDADO 2 |
| J3-1 | técnico | El reset de Baluarte va en `BlockRetention.Enforce`, **no** en el hook del power (gotcha del preventer único), y `Enforce` deja de cortar con `block == 0` | §3 CANDADO 1 |
| J3-2 | radio | El cambio de FGOCore obliga a re-verificar y publicar **Siegfried y Tiamat en el mismo lote** | §13 |
| J3-3 | save-safety | Cero renombres de ID; `LordCamelotChargePower` queda inerte en vez de borrarse | §6.3 |
| J3-4 | fidelidad | Compensar la pérdida de durabilidad con números (`FirmStance` 7, `IronWill` 5, `DemiServant` 6, Shielder Baluartea la primera carta) — Mash tiene que **seguir siendo la mejor tanque del elenco** | §3, §6 |

### 9.3.bis Parches de la revisión adversarial (Fable 5, 2026-08-20) — MANDAN sobre §9.3

| # | Tipo | Hallazgo | Dónde quedó |
|---|---|---|---|
| **F1** | ERROR | `DistantUtopiaCastlePower.AfterPreventingBlockClear` **no llama a `Enforce`** y, con el reset re-aplicando `BulwarkPower`, **gana siempre la carrera de preventers** ⇒ ni reset ni trim: el tope de 40 sería letra muerta | §3 CANDADO 1, §6.3 |
| **F2** | ERROR | El reset **no puede vivir en `Enforce`**: anclarlo en el hook vanilla `AfterBlockCleared`, que corre incondicionalmente para toda criatura que empieza turno (`CombatManager.cs:500-507`; precedente vanilla `BlockNextTurnPower.cs:19` y el fix de Astolfo). Cierra además el hueco de Blur (stacks rancios) | §3 CANDADO 1 |
| **F3** | ERROR | La auditoría de pico ANTES estaba inflada: procs mal contados (78, no 87) y la energía contada dos veces (~140-155 **una vez**, no 174 **por turno**) | §2.5, §11 |
| **F4** | ERROR | `Provoke` **ya es 4** — la fila "3 → 4" era falsa | §6.1 |
| **F5** | ERROR | `LordCamelot` da **3** Fuerza, no 2; y el guard `block == 0` está duplicado en `BulwarkEngineRelic.cs:105` | §6.3, §3 |
| **F6** | RIESGO | Shielder `[NUEVO]` duplica stacks si no excluye el Bloqueo que ya vino con Baluarte | §3 CANDADO 3 |
| **F7** | BALANCE | `FirmStance` 8 está sobre-tasa contra la propia tabla de la skill (común 1⚡ con rider = 4-7) → arranca en **7** | §6.1 |
| **F8** | BALANCE | La mejora de `CamelotRam` no puede ser `-1⚡` (0⚡ = vaciar el muro gratis): la mejora es la conversión | §6.2 |
| **F9** | FORMA | `SharpenedEdge` lee el muro sin gastarlo: declararlo `[=]` deliberado, no omitirlo | §6.1 |
| **F10** | RADIO | `DragonScaleAegis` (Siegfried) **se rompe** con Baluarte de un turno: no cumple el contrato de `IBlockRetentionSource` | §13 |
| **F11** | FORMA | El changelog necesita el callout **por ID** de las ~14 cartas re-specificadas (disciplina J2-15 de Morgan V2) | §14 |

### 9.4 Contradicciones entre jueces — resueltas al más restrictivo

1. **J1 quería borrar `DistantUtopiaCastle`; J3 lo prohibió** (borrar una rara publicada rompe la
   memoria de los jugadores y las recompensas guardadas). **Resolución:** se queda **con tope 40** — la
   opción más restrictiva que respeta la regla de saves.
2. **J2 pedía que Descargar consumiera SIEMPRE todo; J1 señaló que `Crush` (común, hasta 10) se
   volvería intragable en el turno del muro grande.** **Resolución:** el keyword admite monto
   (`Descargar hasta N`), y el tooltip lo dice explícito. Más texto, menos trampas.
3. **J3 propuso esconder la semántica nueva detrás de un flag para no tocar a Siegfried/Tiamat; J1 lo
   rechazó** (dos semánticas de Baluarte conviviendo = el próximo bug). **Resolución:** una sola
   semántica, publicación conjunta de los tres mods, y compensación numérica declarada para Tiamat
   (§13).

### 9.5 Injertos de las propuestas perdedoras

- De **P1**: el techo numérico de `DistantUtopiaCastle` y `UtopianFortress`.
- De **P3**: la matriz de §4 completa, y el traslado del payoff de la línea A de "daño = pila" a
  Intercepción (`ShieldsUp` 5, `Provoke` 4, `Reprisal` 5, `LordCamelot` +3 permanente).

---

## 10. Verificación de restricciones duras (una por una)

| Restricción | Estado |
|---|---|
| Ningún ID de carta / power / reliquia se renombra | ✅ 0 renombres |
| Cero demotes nuevos (`CardRarity.Event`) | ✅ 0 |
| Cartas nuevas | ✅ **0** |
| Mazo inicial y 4 básicas intactos | ✅ §5 |
| Denominaciones NP 10/20/30/50/100 | ✅ ninguna cambia |
| Prohibido el multiplicador global de daño en el starter | ✅ la starter no se toca |
| Techo de saturación 180-220/turno | ✅ §11: el pico pasa de ~174 **gratis y repetible** a ~95 **pagado con el muro** |
| Conectividad ≥90% en comunes | ✅ sin cambios de pool; `FormDrill` promovida **suma** conectividad |
| Puerta de cada arquetipo en común | ✅ §4 |
| Glow dorado en toda condicional | ✅ Descargar entra a la lista |
| No depender de debuffs (los jefes los strippean) | ✅ Intercepción, Baluarte, Carga y Estrellas son recursos propios; Black Barrel **ignora Bloqueo y strippea**, no aplica debuffs |
| Multi-hit anti-Buffer en las tres rarezas | ✅ `TwinHaftStrike` (común), `BlackBarrelBurst` (PC), `PaladinAssault` (rara) |
| Ningún gate de Bloqueo queda inalcanzable | ✅ `CoveringFire` 8, `ChaldeaSandwich` 12, `KnightsVow` 20 — los tres se alcanzan **dentro** de un turno (Defender 5 + `ProtectSenpai` 8 + `FirmStance` 8 = 21 con 3⚡). Sólo dependían de la pila acumulada de forma incidental. |
| Las formas cambian decisiones, no números (skill §5) | ✅ §3 CANDADO 3 |
| Ningún rider de "conservaste Bloqueo" se apaga | ✅ `FrontalCharge` (el único rider explícito de Baluarte en común) detecta la retención por **historial de combate** (`BlockGainedEntry`), no por stacks ⇒ el reset no la rompe, y con Baluarte de un turno pasa a ser una decisión real |

---

## 11. Auditoría de pico (turno 6, acto 3, mismo draft)

| | **ANTES** | **DESPUÉS** |
|---|---|---|
| Bloqueo en pie | **~78**, +9/turno automático, sin tope | **~32-42** (12 llevados + 20-30 del turno); un turno de muro grande llega a ~55 y **ese turno no pegás** |
| ¿Invulnerable? | **Sí**, desde el turno 5, permanentemente | **No**: entrante de acto 3 (30-45) queda a tiro del muro |
| Ofensiva del turno pico | `RoundTablePunishment` ~78 AoE **+** `LordCamelotCharge` ~78 ST = **~140-155 con 5⚡** (cuenta corregida F3: ese turno no defendés, el muro sólo crece +9) | `RoundTablePunishment` ~40 AoE (Descargar) + Ataques ~25 + Intercepción ~15 = **~80-95, y quedás en 0 de Bloqueo** |
| ¿Se puede repetir? | Cada 2-3 turnos, y el piso desde el que disparás **sube solo** cada turno | No: hay que reconstruir el muro (2-3 turnos) |
| Con `DistantUtopiaCastle` (rara) | infinito | muro estable de **40**, descarga de 40 cada 2 turnos |
| Costo del pico | **ninguno** | **toda tu defensa del próximo turno enemigo** |

**Nota de pico en Paladín (F-riesgo 4, a auditar en implementación):** `LordCamelotCharge` ×1.5 dentro
de la forma Ortinax/Paladín ×1.5 da **×2.25** — 40 de muro ⇒ ~90 de daño. Sigue bajo el techo 180-220,
pero hay que computarlo explícito. Además, el orden de consumo importa: `MashFormPower.BeforeCardPlayed`
Descarga hasta 5 de Bloqueo **antes** del `OnPlay`, así que en Ortinax toda carta de Descargar lee 5
menos; y el reembolso de `AfterCardPlayed` (para Ataques `Unpowered` como `LordCamelotCharge`) devuelve
esos 5 **después** de que la carta vació el muro — o sea que "quedás desnuda" en realidad te deja con 5.
Decidir en implementación si el reembolso se suprime cuando la carta tiene Descargar.

Mash queda como **la mejor tanque del elenco** (nadie más lleva 30-40 de Bloqueo estable + Intercepción
+ el piso de la starter) sin ser inmortal, y su daño grande vuelve a ser **una apuesta**.

---

## 12. Riesgos y knobs

### Riesgos honestos

1. **Es un nerf grande y se va a notar.** Un jugador con una run guardada en curso va a ver su muro
   evaporarse. No hay forma de evitarlo: el defecto es de reglas. La mitigación es la compensación
   numérica de J3-4 y decirlo con todas las letras en el changelog del Workshop.
2. **`AbsoluteWall` sigue siendo un turno de invulnerabilidad** y ahora, relativamente, vale más. Es
   rara + Agotar + un turno (tasa de Intangible vanilla), así que se deja — pero es el primer
   sospechoso si el playtest sigue mostrando "no me pueden matar".
3. **Tiamat pierde arranque** (`Carapace` es **básica** y da Baluarte): su curva de acto 1 baja. §13.
4. **Descargar puede leerse como trampa** ("¿me sacó el bloqueo?") si el flotante J2-3 no entra. El
   parche de legibilidad **no es opcional**: es la mitad del arreglo del reporte.

### Knobs (números, no lógica; en orden de prioridad)

1. Si sigue siendo demasiado duradera: `FirmStance` 8→7, `IronWill` 5→4, `DistantUtopiaCastle` 40→30.
2. Si el daño de descarga se siente pobre: `CamelotRam` mejora ×1.5→×2, Ortinax ×1.5→×1.75.
3. Si Baluarte de un turno se siente **inútil** (riesgo opuesto): que la forma Shielder Baluartee las
   **dos** primeras cartas de Bloqueo del turno en vez de una.
4. Si `AbsoluteWall` domina: pasa a 3⚡, o *«tu Vida no puede bajar de 1»* en vez de "no baja".
5. Si Tiamat queda floja tras §13: `Carapace` +2 de Bloqueo base.

---

## 13. Radio de impacto en FGOCore (el costo real de este pase)

`BlockRetention` es **compartida**. El CANDADO 1 toca a tres mods:

| Mod | Usos de Baluarte | Efecto del cambio |
|---|---|---|
| **Mash** | 8 fuentes (§2.2) | el objetivo del pase |
| **Siegfried** | `DraconicRampart` (PC, 10), `StrategicWithdrawal` (rara, 18, Agotar) **y `DragonScaleAegis`** | dos cartas pasan de "muro permanente" a "muro de un turno" (nerf real pero chico: no tiene motores automáticos por turno) **+ un ERROR que hay que arreglar en el mismo lote → F10, abajo** |
| **Tiamat** | `Carapace` (**BÁSICA**), `TidePool` (común), `AbyssalChrysalis` (rara) | **el más afectado**: una básica que Baluartea todos los turnos tenía exactamente la misma degeneración que Mash, sólo que más lenta. Es un arreglo para ella también, pero le mueve el acto 1. Knob declarado: `Carapace` +2 de Bloqueo base. |

### 13.bis Parche F10 — `DragonScaleAegis` se ROMPE, no queda igual

La primera versión de este documento decía que la reliquia de Siegfried "no se ve afectada porque tiene
tope fijo". **Es falso.** `SiegfriedSaber/.../Relics/DragonScaleAegis.cs` implementa `IBlockRetentionSource`
(o sea aporta al cálculo del cap) pero **no overridea `ShouldClearBlock` ni `AfterPreventingBlockClear`**,
que es justo el contrato que documenta `FGOCore/.../Block/IBlockRetentionSource.cs` — y que la cita como
el ejemplo a seguir. Hoy su piso funciona **de prestado**: hay un `BulwarkPower` cuasi-permanente que
responde `false` por ella y dispara el `Enforce`.

Con Baluarte de un turno, `BulwarkPower` desaparece cada turno ⇒ **en todo turno en que Siegfried no
jugó una carta de Baluarte no hay preventer y el Bloqueo se limpia entero**: la reliquia queda muerta la
mayoría de los turnos. **Hay que agregarle los dos overrides del contrato** (patrón
`BulwarkEngineRelic.cs:99-108`) en el mismo lote. No es opcional: sin eso, el pase de Mash rompe una
reliquia publicada de otro personaje.

**Consecuencia operativa (parche J3-2):** este pase publica **FGOCore + Mash + Siegfried + Tiamat en
el mismo lote**, con la matriz MAIN/BETA verde para los cuatro y `audit_simpleloc` + paridad 5 idiomas.
No se puede publicar Mash sola.

**Alternativa descartada (queda registrada):** dejar `BulwarkPower` como está y agregar a FGOCore una
interfaz opcional `IBulwarkCeiling` que sólo Mash implemente — radio de impacto cero, pero deja dos
semánticas de Baluarte conviviendo y no arregla a Tiamat, que tiene el mismo defecto. Rechazada por
J1 (§9.4-3). **Si el usuario prefiere no mover a Siegfried y Tiamat en este pase, ésta es la variante
a activar**, con el costo declarado.

---

## 14. Notas de implementación (cuando el usuario apruebe)

Orden de trabajo (pipeline WORKFLOW-FGO §4.6.7: lotes por rareza → loc → auditorías → matriz → publish):

1. **FGOCore** — `BulwarkPower.AfterBlockCleared(creature)`: si `creature == Owner`, removerse
   (el reset; **F2**). `BlockRetention.Enforce` queda como está (sólo el trim). Comentario obligatorio
   con el gotcha del preventer único y con por qué el reset NO vive en `Enforce`. Bump de FGOCore +
   revisión de firmas públicas (no cambia ninguna: es comportamiento interno).
1.bis. **Los dos preventers que esquivan el helper**: `DistantUtopiaCastlePower.AfterPreventingBlockClear`
   → delegar en `Enforce` (**F1**); `DragonScaleAegis` → agregar los dos overrides del contrato (**F10**).
2. **Keyword `Descargar`** — registro del keyword dorado + tooltip en `card_keywords.json` ×5 idiomas
   (eng/esp/zhs/kor/rus), con el cierre explícito `*词*` en zhs.
3. **Texto de Baluarte** ×5 idiomas (`powers.json`), con la frase «se gasta».
4. **Lote comunes** (§6.1) → **lote PC** (§6.2, incluye el cambio de rareza de `FormDrill`) → **lote
   raras** (§6.3) → **formas** (§3 CANDADO 3, `MashFormPower`).
5. **Flotante «¡Descarga! X»** (patrón `vfx_blocked_text` de Morgan V2 §14) + glow.
6. **Loc de las 14 cartas re-specificadas** ×5 idiomas; `tools/audit_localization_parity` +
   `tools/audit_simpleloc.ps1` + `audit_vfx_paths`.
7. **Tiamat / Siegfried**: revisión de números (§13) y sus loc si cambia algún texto.
8. **Matriz MAIN/BETA** (`tools/build_compat_matrix.ps1`, **una rama por corrida** — las tres juntas
   se mueren con exit 137 por OOM) + probe del artefacto universal main→beta.
9. **Versiones**: Mash `v0.1.21`, FGOCore `v0.1.24`, Siegfried y Tiamat bump; `tools/workshop_desc/*.txt`
   bumpeados **en el mismo commit** (el olvido histórico de §275 de STATUS).
10. **Publish** con orden explícita del usuario; changelog del Workshop en eng/zhs explicando el nerf,
    con el **callout carta por carta, por ID**, de las ~14 re-specificadas (disciplina J2-15 de Morgan
    V2; parche **F11**), y **respondiendo a Moopamoop** con lo que cambió y por qué.

Commits separados por tipo: `feat` (keyword + reglas), `fix` (los topes de §6.3), `docs` (este archivo
+ STATUS), `chore` (bumps de versión y fichas).
