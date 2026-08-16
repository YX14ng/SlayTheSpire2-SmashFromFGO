# DIAGNÓSTICO — Kagetora: la Doctrina no funciona (reporte de Steam 2026-08-14)

> Producido por un panel de 5 lentes + síntesis (energía, legibilidad, transformación, pool,
> contratos técnicos) contra el código real y el juego decompilado, 2026-08-16. Es el insumo del
> rediseño; las decisiones viven en `REDESIGN-KAGETORA-V2.md`.
>
> **Los bugs P0 de §0 y §3.1 ya fueron corregidos** (`WasUsed` con `?? 0` en Kagetora y Astolfo,
> reset por turno de `AstolfoTurnUsagePower`, y el tercer sitio en `WallOfBanners`).

# Diagnóstico único — Kagetora / Doctrina de los Tres Preceptos (v0.1.11)

Síntesis de las cinco lentes. Todo lo que sigue lo verifiqué yo contra el código; donde dos lentes se contradecían, dice cuál tenía razón. Descarté las afirmaciones sin respaldo.

---

## 0. El hallazgo que reordena todo lo demás

Antes de los tres reclamos: **hay un bug de una línea que deja 12 efectos permanentemente muertos en el build publicado.**

`KagetoraLancer/KagetoraLancerCode/Powers/SystemPowers.cs:98-99`

```csharp
public static bool WasUsed(Creature owner, KagetoraUsage usage) =>
    (owner.GetPower<KagetoraUsagePower>()?.Mask & (int)usage) != 0;
```

`GetPower<T>()` devuelve `T?`. Sin el power, `?.Mask` es `int?` = null, `null & N` es null, y **`null != 0` es `true`** (comparación levantada de C#). `KagetoraUsagePower` se crea **exclusivamente** dentro de `KagetoraUsages.Mark` (`SystemPowers.cs:101-116`), y verifiqué que los **12 call-sites de `Mark` están todos detrás de un guard `WasUsed`** (correspondencia 1:1, grep exhaustivo). El estado es absorbente: el power nunca se crea, `WasUsed` nunca deja de ser `true`, y ninguno de los 12 efectos se ejecuta jamás.

Efectos muertos: `EightFormationsPower` (`RarePowers.cs:43` — `CanOverrideDoctrineFailure` devuelve `!true && …` = `false`), `WhiteFlameBrazier` (`PoolRelics.cs:94-99`), `WhiteFlamePower` (`RarePowers.cs:28`), `FieldJudgePower` (`:133`), `VictoryIsInTheFeetPower` (`:151`), `RidingPower` (`UncommonPowers.cs:94`), `GeneralsDoctrinePower` (`:111`), y 6 reliquias (`PoolRelics.cs:23,38,52,66,80,94`).

**Esto toca los tres reclamos a la vez:**
- La **única** válvula de escape del orden fijo en todo el kit (`EightFormationsPower`) no existe en la práctica.
- La **única** línea `GainEnergy` de todo el mod (`PoolRelics.cs:98`, verificado por grep: un solo hit) está dentro de un efecto muerto → **Kagetora no tiene ninguna ganancia de energía funcional en el juego shippeado**.
- La recompensa mecánica más grande de la ascensión (+1⚡ y +30 estrellas) nunca se entrega.

La lente de energía contó `WhiteFlameBrazier` como "la única fuente de energía (rara)". Con el bug, **son cero**. El fix es `?? 0` antes del `&`.

**Radio de explosión:** `AstolfoRider/AstolfoRiderCode/Powers/SystemPowers.cs:33` tiene la expresión idéntica, con 7 efectos afectados; y `AstolfoTurnUsagePower` (`:21-28`) además **no tiene `BeforeSideTurnStart`**, así que aun arreglando `WasUsed` sus efectos serían "una vez por combate" en vez de "por turno". Kagetora sí tiene el reset correcto (`SystemPowers.cs:44-58`, con `participants.Contains(Owner)`).

---

## 1. Veredicto por reclamo

### 1.1 「太缺费了」 (falta energía) — **CONFIRMADO**

Magnitud medida: **≈1⚡ por turno, ≈33 % del presupuesto.**

| Hecho | Valor | Evidencia |
|---|---|---|
| Energía/turno | 3 | `decompiled/MegaCrit.Sts2.Core.Models/CharacterModel.cs:84`; **Kagetora no lo sobreescribe** (grep `MaxEnergy` en `KagetoraLancerCode/` = 0 hits) |
| Robo/turno | 5 | `decompiled/MegaCrit.Sts2.Core.Combat/CombatManager.cs:654` |
| Mazo inicial | 10 cartas, **las 10 cuestan 1⚡** | `Character/Kagetora.cs:23-30` + `Cards/Basic/StartingCards.cs:12,31,53,75,87,104` (leí los seis constructores) |
| Ciclo mínimo con mazo inicial | 1+1+1 = **3⚡ = 100 % del turno** | ídem |
| Cadena a costo medio del pool | **3,79⚡** contra 3 disponibles (**+27 %**) | parseo propio del pool (abajo) |

Pool drafteable (68 cartas), **reparseado y confirmado por mí**:

| Precepto | n | 0⚡ | 1⚡ | 2⚡ | 3⚡ | costo medio | Ataques |
|---|---:|---:|---:|---:|---:|---:|---:|
| Cielo | 21 | 3 | 14 | 4 | 0 | **1,05** | 4 |
| Pecho | 22 | 1 | 13 | 8 | 0 | **1,32** | **0** |
| Pies | 21 | 2 | 9 | 9 | 1 | **1,43** | **15** |
| neutral | 4 | 0 | 1 | 3 | 0 | 1,75 | 0 |

Verifiqué carta por carta las seis de 0⚡: `PrayerToBishamonten` (Cielo, Exh), `TurnTheReins` (Cielo, Exh + gate 50★), `ClosedFormation` (Pecho, Exh), `StepOfVictory` (Pies, **sin Exhaust**), `TurnTheFormation` (Pies, Exh + gate 50 NP), `MagicalCharge` (Cielo, Exh).

→ **Una sola carta de 0⚡ repetible en 68 (`CommonCards.cs:202`). Cielo y Pecho — los dos primeros pasos obligatorios — tienen CERO.** El costo sube monótonamente a lo largo del orden obligatorio (1,05 → 1,32 → 1,43) mientras el presupuesto baja; **48 % de las cartas de Pies (10/21) cuestan ≥2⚡**, y el paso 3 es donde vive el 79 % del daño.

Reproduje la simulación de la lente de energía (mismo script, pool parseado del `.cs`):

```
mazo 10 | 3E | Kagetora | ciclo 75.3% | sobrante 0E el 100% de las veces
mazo 10 | 3E | Kagetora | 6.50 ciclos/8T | 0.43E libre/turno | 20.5/24 en preceptos (86%)
mazo 20 | 3E | Kagetora | 5.51 ciclos/8T | 1.05E libre/turno | 15.6/24 (65%)
contrafactual 4E, mazo 10: 6.50 ciclos/8T (igual) | 1.43E libre/turno
```

El 75,3 % reproduce el `189/252 = 75 %` de `docs/DESIGN-KAGETORA.md:433`, así que el modelo está calibrado contra el propio doc. **Con el mazo inicial: 0,43⚡ discrecionales por turno, y el 100 % de los ciclos termina con 0⚡ sobrante y 2 cartas injugables en la mano.** El contrafactual a 4⚡ no cambia la tasa de ciclo (6,50 → 6,50) pero triplica la energía libre: **el problema no es que el ciclo sea inviable, es que se come el turno entero.**

### 1.2 「按顺序推进游戏内有点看不明白」 (ilegible) — **CONFIRMADO, y es literal**

**El orden Cielo→Pecho→Pies no se puede leer en ningún lugar del combate.**

*(Contradicción resuelta: la lente técnica citó `DOCTRINE_POWER.description` — que sí dice el orden — como "lo que el jugador ve". Es incorrecto. La lente de legibilidad tenía razón.)* `decompiled/MegaCrit.Sts2.Core.Models/PowerModel.cs:360`: `bool flag = HasSmartDescription && base.IsMutable;` — una instancia de combate es mutable, así que el hover muestra **`smartDescription`**. Dumpeé el loc real:

| Clave | eng | zhs |
|---|---|---|
| `DOCTRINE_POWER.smartDescription` | `Doctrine progress. Maximum 3 advances per turn.` | `教条进度。每回合最多推进3次。` |
| `NAGAO_KAGETORA_FORM_POWER.smartDescription` | `Starting form. Doctrine order is fixed.` | `初始形态。教条顺序固定。` |

Ninguna nombra los preceptos. El texto que sí los nombra (`.description`) **solo se renderiza fuera de combate** (compendio).

Y el progreso tampoco se ve: `Doctrine/Doctrine.cs:65` → `StackType => PowerStackType.Single`, documentado en `decompiled/MegaCrit.Sts2.Core.Entities.Powers/PowerStackType.cs:11-13` como *"Amount is hidden, and is always 1"*, y `decompiled/MegaCrit.Sts2.Core.Nodes.Combat/NPower.cs:234` lo respeta literalmente (`Counter ? DisplayAmount : string.Empty`). `DoctrinePower` no sobreescribe `DisplayAmount`. **El icono destella (`Doctrine.cs:190 Flash()`) sin mostrar nada.**

No hay ninguna otra superficie, verificado por mí:
- `find KagetoraLancer -name "*.tscn"` → 4 escenas, todas de arte de personaje. **Cero UI.**
- `grep HarmonyPatch KagetoraLancer` → **0**.
- `grep ShouldGlowGoldInternal` → **1 hit** en todo el mod (`Cards/Special/NoblePhantasms.cs:22`, y es el NP). Contra **Morgan 27, Mash 25, Okita 24, Artoria 8, Tiamat 8**.
- `grep ExtraHoverTips` → **7 en todo el mod**, contra **Artoria 75, Morgan 70, Mash 67**. Las 68 cartas del pool drafteable tienen **cero** tooltips.
- `FGOCore/FGOCoreCode/Ritsu/FgoSecondaryResources.cs:102 RegisterCombatMeters` registra exactamente dos medidores (NP y Estrellas). La Doctrina no está.

Esto **viola dos reglas escritas del propio repo**, `docs/WORKFLOW-FGO.md:151-152` (*«Glow dorado en TODA carta condicional»*) y las promesas de `docs/DESIGN-KAGETORA.md:514-517` (widget con tres iconos y flecha, brillo dorado en mano, preview `Avanza`/`Ya usado`/`Fuera de orden`/`Límite del turno`): **de los 9 ítems de §16 se shippeó 1 parcial.**

Lo único que funciona: el etiquetado por carta. Verificado — 69/69 cartas con precepto llevan `(Heaven)`/`(Chest)`/`(Feet)` en eng y 【天】/【胸】/【足】 en zhs, **0 discrepancias código↔loc**, 161 claves idénticas en ambos idiomas. La información *por carta* está bien; falta el **estado del sistema**.

### 1.3 「进化后没什么显著的变化 / 意义不明」 — **CONFIRMADO**

Inventario completo del delta, verificado por grep exhaustivo de `KenshinFormPower|IsKenshin|IAscensionListener` (9 hits totales, 4 de ellos declaraciones):

1. Sprite (`FormVisuals.Apply` — swap seco, sin VFX; `DESIGN-KAGETORA.md:614` lo tiene **sin tildar**).
2. Icono/título del form power.
3. Orden libre de la Doctrina (`Doctrine.cs:86` y `:108`) — **invisible, ver §1.2**.
4. NP nuevo — **diferido**: el gauge se consume entero y hay que recargar 100.
5. `CommandersStaff` +4 daño (`CommonCards.cs:89`).
6. `ShiranuiBlade` trueque estrellas↔quitar Bloqueo (`RareCards.cs:154`).
7. `DivinityPower` +2 (`UncommonPowers.cs:140-144`, leído).
8. `WhiteFlameBrazier` +1⚡/+30★ — **muerto por el bug §0**.

**3 riders de forma en 74 cartas (4,1 %). 91,9 % del mazo se juega idéntico.** Y los dos form powers están **vacíos** (`Powers/DoctrinePowers.cs:24-34`): son punteros a un `.tres` de sprites, sin un solo hook sobreescrito. Comparación: Morgan mete 104+49+109 líneas de lógica y 3 interfaces en sus formas; Artoria implementa `ICriticalAccessRule` (en Caster **no podés criticar en absoluto**); Tiamat cambia la aritmética de 38 de 49 archivos de carta vía amplificadores.

**El NP de Kenshin es peor.** Fórmulas leídas en `Cards/Special/NoblePhantasms.cs`:
- Kagetora (`:49`): `perHit = 3 + Lv`, **8 impactos**, + Débil 1/2/3 por OC. **No escala con el medidor.**
- Kenshin (`:87-90`): `perHit = 5 + 2·Lv + clamp(tier/100 − 1, 0, 4) + (Man ? 3 : 0)`, **4 impactos**, sin Débil.

*(Precisión que ninguna lente dio del todo: el NP de Kenshin **sí** escala con overcharge, el de Kagetora no. A OC1 es −4 plano a cualquier nivel; a OC3 la base de Kenshin ya supera a la de Kagetora.)* Pero el escalado no lo salva, porque **los bonus aditivos son por impacto y Kenshin tiene la mitad** (`StrengthPower.cs:25`, Bendición +2/impacto en `DoctrinePowers.cs:100`):

| Línea de control NP3, +3 Fuerza, Bendición, Divinidad | Total |
|---|---:|
| **Kagetora**, cualquier objetivo, OC1 | **91** + Débil |
| Kenshin, no-Man (Elite/Jefe), OC1 | **69** (−24 %) |
| Kenshin, no-Man, **OC5** (500 NP banqueados) | 85 |
| Kenshin, Man, OC5 (mejor caso absoluto) | 97 |

Y el anti-Man está apagado justo donde importa: `FGOCore/FGOCoreCode/Attributes/FgoAttributes.cs:63-65` mapea `Monster→Man`, **`Elite→Earth`, `Boss→Heaven`**. Contra los enemigos que justifican tener un ulti, Kenshin nunca activa su bonus, y su primer NP es siempre OC1 porque el medidor arranca en 0.

**El jugador cambia un ulti de 91 con Débil por uno de 69 contra jefes, resetea el medidor, y a cambio recibe un retrato nuevo y una regla que no puede ver.** 「意义不明」 es una lectura correcta del sistema compilado.

---

## 2. Causas raíz (decisiones de diseño, no síntomas)

**R1 — El motor pide N cartas por turno y la curva de costos no se diseñó contra ese requisito.**
Kagetora es *el único personaje del roster con una secuencia ordenada obligatoria de 3 cartas por turno*, y tiene la **segunda menor densidad de 0⚡ (8,8 %)** y la **segunda mayor de ≥2⚡ (35 %)** del roster (Okita 20,6 %/28 %, Mash 16,4 %/23 %, Morgan 13,3 %/21 %). El costo del ciclo no es una variable de balance ajustable: es `Σ costo de tres cartas` y nadie fijó un presupuesto para esa suma. Consecuencia aritmética: **3,79⚡ de cadena media contra 3 de presupuesto**.

**R2 — El precepto se usó como *cupo obligatorio*, no como *eje de draft*.**
Cielo=NP (15 Habilidades), Pecho=Bloqueo (**0 Ataques en 22 cartas**), Pies=daño (15 de los 19 Ataques del pool). Como el orden fijo penaliza la especialización (`Doctrine.cs:82-92`: solo el precepto esperado avanza), **la única curva de draft correcta es 1:1:1** → todos los mazos de Kagetora convergen al mismo mazo. Los 4 arquetipos declarados en `DESIGN-KAGETORA.md:80-85` son en realidad **uno y medio**: Formación/NP existe; "Muralla de Echigo" no es un arquetipo sino un peaje (verifiqué: **ninguna carta del pool lee `Owner.Creature.Block` para convertirlo en daño**; el único hit de `.Block` fuera de `GainBlock` es `ClosedFormation:170` como condición); "Ejecución de Kenshin" tiene 3 cartas de soporte en 74.

**R3 — La UI se dejó para el final y nunca se hizo, y el resto del kit se diseñó asumiendo que existía.**
`WouldAdvance`/`WouldAdvanceAfter` (`Doctrine.cs:82-114`) están escritas como **predicados puros, sin efectos** — o sea, listas para alimentar un `ShouldGlowGoldInternal`. Existe el motor de decisión; falta el canal de salida. Las cartas condicionales (~10) se balancearon como si el jugador pudiera ver el estado.

**R4 — Se eligió `StackType.Single` para un power que guarda un contador.**
`Doctrine.cs:52-55` justifica la codificación `mask+1` diciendo *«para que … sobreviva guardado/carga»*. **Verifiqué que ese problema no existe**: `decompiled/MegaCrit.Sts2.Core.Saves.Runs/` no tiene `SerializableCombat` ni `SerializablePower` (listé el directorio completo). StS2 no serializa estado de combate. La restricción autoimpuesta que fuerza `Single` — y por lo tanto apaga el número en pantalla — **es una solución a un problema que el juego no tiene**.

**R5 — La reliquia inicial entrega un tercio del caudal que el propio repo exige.**
`docs/WORKFLOW-FGO.md:147-150`: *«Starter relic = motor … **SIEMPRE con cap de 3 procs/turno** … Los riders del pool se calibran contra ese flujo garantizado.»* `JeweledPagodaOfBishamonten` (`Relics/IdentityRelics.cs:21,36-43`) procea al **completar ciclo**, y `MaxAdvancesPerTurn = 3` (`Doctrine.cs:59`) permite **1 ciclo/turno** → **máximo 1 proc/turno**. El pool entero se calibró contra un motor que da 1/3 del flujo asumido.

**R6 — "Ninguna ventaja gratis" al ascender, sin ninguna contrapartida entregada.**
`DESIGN-KAGETORA.md:180` se autoimpone «no hay curación, energía ni estadísticas gratis». La única contrapartida prometida (libertad de orden) quedó sin UI, y el NP "de premio" salió numéricamente inferior contra Elite/Jefe. La transformación cuesta un recurso completo y no cambia nada perceptible.

**R7 — El bug §0 no es solo un bug: es la razón de que el kit no tenga válvulas.**
Todos los efectos "una vez por turno/combate" del personaje pasan por el mismo helper. El diseño *sí* previó válvulas de escape (energía al ascender, ignorar el orden una vez por turno, bloqueo al fallar un precepto) — **todas viven en el código y ninguna se ejecuta**.

---

## 3. Hallazgos colaterales

### 3.1 Bugs vivos

| Sev | Qué | Dónde |
|---|---|---|
| **P0** | `WasUsed` nullable-lifted → 12 efectos muertos (§0) | `Powers/SystemPowers.cs:98-99` |
| **P0** | Mismo bug en Astolfo (7 efectos) + falta `BeforeSideTurnStart` en `AstolfoTurnUsagePower` | `AstolfoRider/.../SystemPowers.cs:21-33` |
| **P1** | **Doble gasto Artifact + Ventana del Tesoro.** Verificado en cadena: `Hook.cs:1916-1928` propaga `num` ya modificado a los listeners siguientes y acumula **todos** los que devolvieron `true`; `PowerCmd.cs:152` los llama a todos. `ArtifactPower` (`decompiled/…/ArtifactPower.cs:24-41`) pone 0 y devuelve `true`; `TreasureWindowPower` (`RarePowers.cs:98-105`) recibe `amount=0`, y `GetTypeForAmount(0)` (`PowerModel.cs:460-470`) sigue devolviendo `Debuff` → también devuelve `true` → **se consume**. Con 1 Artifact (4 fuentes en el pool propio) y la Ventana activa, un debuff bloqueado gasta las dos defensas. Precedente vanilla del guard correcto: `RuinedHelmet.cs:42-45` chequea `amount <= 0` | `Powers/RarePowers.cs:98-105` |
| **P2** | **Cerrar un ciclo puede ser completamente silencioso**: `IdentityRelics.cs:38-39` retorna antes del `Flash()` si el mazo de robo está vacío | `Relics/IdentityRelics.cs:36-43` |
| **P2** | `JustPathPower`: `GetPower<DoctrinePower>()?.AdvancesThisTurn < 2` — con null, `null < 2` es `false` → **no** retorna → otorga Bloqueo incondicional. Mismo error de razonamiento que §0, con signo opuesto. Latente hoy (tres reliquias instalan `DoctrinePower`); vivo si un evento saca reliquias | `Powers/UncommonPowers.cs:80` |

### 3.2 Bug de localización zhs — desborde de dorado

Corrí el regex real de BaseLib (`decompiled/_baselib_full/BaseLib.Patches.Localization/SimpleLoc.cs:28`) sobre los 5 idiomas. El terminador de la clase es ASCII y `。` (U+3002) no está incluido:

```
eng 9 spans, 0 desbordes | esp 9/0 | kor 9/0 | rus 9/0
zhs 5 spans, 4 DESBORDES:
   ARTS.description             -> '宝具值。【天】'
   QUICK.description            -> '暴击星。Quick结算后再获得10颗。【足】'
   FORTUNE_IS_IN_HEAVEN         -> '宝具值。抽!C!张牌。【天】'
   BITEN_HASSOU_KURUMA_GAKARI…  -> '谦信。保留。消耗。'
```

**3 de las 4 son cartas del mazo inicial, y la cuarta es el NP que anuncia la transformación** — en el idioma en que jugó el reporter. (Nota extra: zhs tiene 5 spans dorados contra 9 en los demás idiomas; también se perdió marcado.)

### 3.3 Cartas muertas y texto que miente

| Carta | Dónde | Hallazgo |
|---|---|---|
| `TurnTheReins` / `TurnTheFormation` | `CommonCards.cs:51,209` | `new CardsVar(0)` + loc `"Draw !C!."` → **la carta base imprime literalmente «Draw 0.» / «抽0张牌。»** en los 5 idiomas. El código está bien (`if > 0`); el texto no |
| `ArmyFootsteps` | `UncommonCards.cs:323-333` | Tutorea un `Precept: Feet` a secas, sin `WouldAdvanceAfter`. Comparar con `BattleOrder` (`CommonCards.cs:71-72`) que lo hace bien. **Corrijo a la lente de pool:** dijo "vale en ambas formas"; en **Kagetora** la carta tutoreada nunca puede avanzar (traza completa del mask), pero en **Kenshin** hay un caso vivo (mask=Cielo\|Pecho → esta carta cierra el ciclo → mask resetea a 0 → un Pies libre sí avanza). Sigue siendo un tutor roto para la forma base |
| `EightWeaponsOneWarrior` | `UncommonCards.cs:243-256` | Lee `AdvancedMaskThisTurn` **antes** de su propio avance. **Corrijo hacia abajo a la lente de pool** (dijo techo 20): con 3⚡, 2 avances previos + esta carta = 1+1+2 = **4⚡**. Lo alcanzable con el presupuesto real es **count=1 → +10 estrellas**. El doc declara máx 30 (`DESIGN-KAGETORA.md:319`) |
| `WheelStrategy` | `UncommonCards.cs:16-26` | Bono condicionado a `AdvancedMaskThisTurn != 0`. Es Cielo = paso 1, así que **en la línea natural nunca paga** (solo paga si ya avanzaste algo ese turno). Y `VanguardMandate` (`:95-105`) **la domina estrictamente**: misma rareza, mismo coste, mismo precepto, mismo robo, NP **incondicional** y además puede targetear a un aliado |
| `SaltForTheRival` | `CommonCards.cs:149` | La loc dice `"removes Weak"` sin cantidad; el código base quita **1** stack |
| `FullHoushoutsukigeGallop` | `RareCards.cs:171-183` | Rara a **3⚡** = el turno entero → **cancela el ciclo**. 5 daño/impacto ×3 contra el común `NaginataSweep` (1⚡, 6 a todos): peor por energía |
| `SharedGuard` | `UncommonCards.cs:183` | **Matizo a la lente de pool:** no es copia exacta de `ArmourIsInTheChest` (`CommonCards.cs:103`) — da 7 a sí mismo **+4 a cada aliado**. En un jugador es idéntica a una común; en co-op está diferenciada |

### 3.4 Promesas del doc no implementadas

- `DESIGN-KAGETORA.md:514-517` (widget, flecha de orden, brillo dorado, preview de 4 estados): **0 %**.
- `DESIGN-KAGETORA.md:614` (VFX de Doctrina/Bendición/ascensión/NP): sin tildar, no existen. `find *.ogg *.wav *.mp3` → **0 archivos de audio**; ambos NP usan el mismo `"vfx/vfx_dramatic_stab"` (`NoblePhantasms.cs:53` y `:95`), así que el NP de Kenshin se ve como una versión **más corta** del de Kagetora.
- `DESIGN-KAGETORA.md:436-443` publica el 75,7 % de tasa de ciclo como titular pero **solo simula el mazo inicial de 10 cartas**. Reproducido: cae a 54,8 % (mazo 20) y 52,8 % (mazo 25). **El personaje empeora en su propia mecánica central a medida que avanza la run** — invertido respecto de cualquier deckbuilder.
- `WORKFLOW-FGO.md:143-145` (conectividad ≥90 % en comunes): el doc declara 20/20 bajo el criterio «tiene tag». Bajo el criterio real («cada común lee o escribe ≥1 recurso propio»), **7 de 20 son estadística vanilla + etiqueta** → 65 %.
- `WORKFLOW-FGO.md:144` (pares espejo a 0⚡): el par existe (`TurnTheReins`/`TurnTheFormation`) pero **ambos son Exhaust y con gate de 50**. Los equivalentes de Mash y Okita no son Exhaust.
- El nombre en el HUD nunca cambia: `characters.json` tiene una sola entrada, `KAGETORA.title = "长尾景虎"`. El jugador ve el nombre de la forma base toda la partida.

### 3.5 Trampas de diseño (no bugs)

- **`IncarnationOfBishamonten`** (`StartingCards.cs:104`): cuesta 1⚡ y es **precepto neutral** (verificado: constructor sin `Precept`). El turno que la jugás quedan 2⚡ → **el ciclo es imposible**. Hay que sacrificar un ciclo entero para encender el poder cuya única función es recompensar ciclos.
- **El robo de la Pagoda nace muerto:** se dispara al 3.er avance, momento en que tenés 0⚡, y el mazo inicial no tiene ninguna carta de 0⚡ → **la carta robada es injugable el 100 % de las veces** con el mazo inicial.
- **El tope `MaxAdvancesPerTurn = 3` es código muerto en la práctica.** 3 avances exigen 3 cartas de precepto = 3⚡ = todo el presupuesto. El límite solo mordería con una 4.ª carta de precepto, lo que exige un 0⚡ (6/68 del pool). **El tope real no son 3 avances: son 3 energías.**
- **Los críticos no son una decisión.** `FGOCore/Stars/Criticals.cs:132-163` — `CriticalResolverPower.BeforeCardPlayed` **gasta 50 estrellas automáticamente** en el primer Ataque elegible del turno. Ningún poder ni carta de Kagetora implementa `ICriticalAccessRule` (solo Artoria lo hace). `DESIGN §15.2` lista «gastar ahora o reservar para un multiimpacto» como decisión real: **la decide el motor**. Y con el orden fijo, el Ataque siempre es la tercera carta.

### 3.6 Deuda de contrato (real, sin fallo observable hoy)

`DECISIONS.md:79-82` prohíbe flags "una vez por turno/combate" en campos privados del modelo. Violaciones: `TreasureWindowPower._prevented` (`RarePowers.cs:93` — **este sí participa del bug P1**), `ForcedDoctrineAdvancePower._card` (`:56`), `BishamontenBlessingActivePower._activePlay` (`DoctrinePowers.cs:82`), `DivinityPower._active/_usedHit` (`UncommonPowers.cs:122-123`), y `Precept { get; protected set; }` mutado en runtime (`KagetoraCard.cs:15` ← `RareCards.cs:238`).

Por qué no explotan: (a) el combate no se serializa (§R4); (b) el co-op es lockstep determinista (`decompiled/…Multiplayer.Messages.Game/` solo tiene `ActionEnqueuedMessage`/`PlayerChoiceMessage` + checksums), así que cada par computa lo mismo; (c) la mutación de `Precept` está contenida porque `Player.cs:802-811 PopulateCombatState` clona las cartas.

También: `DoctrinePower` **no** implementa `IResourcePower` (`Doctrine.cs:57`) mientras `DoctrineTurnStatePower` y `KagetoraUsagePower` **sí** (`SystemPowers.cs:11,69`). Asimetría invertida: la contabilidad está protegida contra `Cleanse.RemoveBuffs` y el motor no. Contradice `DESIGN §4.4` («La Doctrina nunca se pierde»). Latente: hoy no hay ningún efecto vanilla que barra buffs del jugador.

---

## 4. Restricciones que cualquier rediseño debe respetar

**4.1 — IDs inmutables.** El save de run guarda modelos por ID (`decompiled/MegaCrit.Sts2.Core.Saves.Runs/ModelIdRunSaveConverter.cs`, `SerializableCard.cs`, `SerializableRelic.cs`). **Renombrar o eliminar clases de carta/reliquia rompe runs en curso.** Rebalancear números, costos, textos y loc es libre; cambiar IDs no.

**4.2 — El estado de combate NO se guarda, y eso libera la mano.** Verificado por ausencia: `Saves.Runs/` no tiene `SerializableCombat` ni `SerializablePower`. **Se puede cambiar `DoctrinePower.StackType` a `Counter` y reescribir la codificación de `Amount` sin romper ningún save.** La justificación de `Doctrine.cs:52-55` para el `mask+1` ya no aplica. *(Confirmar con una prueba de salir a mitad de combate antes de shippear — ver §5.)*

**4.3 — Co-op.** El multijugador es lockstep determinista con checksums (`StateDivergenceMessage`). Cualquier estado nuevo tiene que computarse igual en todos los pares: nada de aleatoriedad local, nada que dependa del orden de UI. `ShouldScaleInMultiplayer` y `TargetType.AnyPlayer` ya están usados correctamente (`SharedGuard`, `VanguardMandate`, `SaltForTheRival`).

**4.4 — Contratos FGOCore que ya funcionan bien.** No tocar: `NpCharge` / `CritStars` / `FgoAttributes` / `FormSwitch` / `Cleanse`. La ascensión es **idempotente por doble candado** (`NoblePhantasms.cs:67` + `FormSwitch.cs:17-18` con `IsPermanent`), y eso hay que preservarlo. El registro de medidores secundarios existe y funciona (`FgoSecondaryResources.cs:102`) — **es el lugar natural para meter el medidor de Doctrina, no un widget nuevo**.

**4.5 — El motor de la Doctrina está bien y no es el problema.** Verificado contra el decompilado, no tocar:
- **Timing**: el avance ocurre en `Hook.AfterCardPlayed`, es decir **después** de resolver el texto completo de la carta (`CardModel.cs:1929-1961`).
- **Golpe letal**: `Hook.AfterCardPlayed` se despacha directo por `IterateHookListeners`, y `EndCombatInternal` solo corre desde `CheckWinCondition` **entre acciones** (`ActionExecutor.cs:161-171`). **El golpe que mata sí avanza la Doctrina.**
- **Reentrancia**: `CombatState.cs:410-460` materializa la lista antes de iterar; aplicar `DoctrineTurnStatePower` a mitad del despacho no rompe nada.
- **Copias/multi-play**: `Hook.AfterCardPlayed` está dentro del bucle `for (i < playCount)`, así que N jugadas = N avances, acotado por el tope de 3.
- **Orden Bendición vs ciclo** (`DESIGN §7.2`): se cumple exactamente.
- **Reset por turno** (`SystemPowers.cs:44-58`): usa `BeforeSideTurnStart` con `participants.Contains(Owner)`, tal como manda `DECISIONS:81-82`.

**4.6 — Lo que YA está bien y no hay que romper al arreglar.**
- **Etiquetado de cartas: impecable.** 69/69 en eng y zhs, 0 discrepancias código↔loc, glosario 天/胸/足 respetado. Cualquier cambio de precepto tiene que arrastrar la etiqueta.
- Cobertura de loc completa (161 claves × 5 idiomas), reliquias con `flavor`.
- El pulso al avanzar (`Doctrine.cs:190`) existe; hay *algo* de feedback, solo que indistinguible entre preceptos.
- Los conteos del doc (§6, §19 Fase C) **coinciden exactamente con el código**: 6+20+28+20. El problema no es de cantidad ni de disciplina de implementación — la implementación es limpia. Es de **arquitectura de pool y de superficie de UI**.

**4.7 — Ítems del doc que son inaplicables, no faltantes.** `CardModel` **no tiene** propiedad `Flavor` en el motor (solo `RelicModel.cs:63`); el ítem de `DESIGN §17` sobre flavor de cartas no se puede cumplir.

---

## 5. Preguntas abiertas para el panel de diseño

**Sobre energía (R1):**
1. ¿El fix es **+1⚡ base** (contrafactual medido: 0,43 → 1,43⚡ libres, tasa de ciclo intacta), **subir la densidad de 0⚡ repetibles de 1 a ~8-10 con al menos 2 por precepto** (llevar al 14-17 % del roster), o **hacer que el ciclo devuelva energía** (p. ej. la Pagoda dando ⚡ en vez de robo)? Las tres cierran la brecha; solo la segunda preserva la textura de draft.
2. ¿Kagetora entra a la **ventana-NP** (`NpWindow.OpenWindow` → +1⚡ / robá 1)? **Corrijo a la lente de energía**, que lo presentó como el estándar vigente del que Kagetora se quedó afuera: verifiqué los call-sites y **solo Tiamat usa el helper** (`NammuDuranki.cs:91`); Morgan lo implementa a mano (`MainFile.cs:63-76`); el resto solo manifiesta la carta, igual que Kagetora (`MainFile.cs:46-61`). Es una convención **documentada pero minoritaria**, no un estándar del que Kagetora se desvió. La pregunta sigue abierta como decisión, no como corrección.
3. ¿Se baja el peso de ≥2⚡ en **Pies** (hoy 48 %), que es el paso al que se llega con menos energía?

**Sobre el pool (R2):**
4. ¿Se rompe el monopolio ofensivo de Pies metiendo Ataques en Pecho (contraataque, escudo-espada)? Hoy Pecho tiene **0 ataques en 22 cartas**, lo que obliga a que 2 de las 3 cartas del ciclo no hagan daño.
5. ¿Se le da salida a Pecho con al menos una conversión **Bloqueo→daño o Bloqueo→NP**? Sin eso, "Muralla de Echigo" seguirá siendo un peaje y no un plan.
6. ¿Se acepta que el orden fijo **fuerza la convergencia a 1:1:1**, o se cambia la regla (p. ej. el orden se elige al inicio del combate, o el mask persiste entre turnos de forma más generosa)? Esta es la decisión de la que dependen las otras cinco.

**Sobre legibilidad (R3, R4):**
7. ¿`StackType.Counter` + `DisplayAmount` = **preceptos completados (0-2)** en el icono, o **tercer medidor** reutilizando `FgoSecondaryResources.RegisterCombatMeters`? Confirmado que lo primero no rompe saves (§4.2), pero conviene validar empíricamente saliendo a mitad de combate antes de shippear.
8. Fixes de costo casi nulo, ¿entran ya en el próximo parche? (a) reescribir `DOCTRINE_POWER.smartDescription` en los 5 idiomas para que **diga el orden y las recompensas** — solo loc, sin código; (b) `ExtraHoverTips => [HoverTipFactory.FromPower<DoctrinePower>()]` en `KagetoraCard` — una línea, patrón ya usado por Morgan/Artoria/Mash, y hace que las 69 cartas etiquetadas expliquen el sistema; (c) `ShouldGlowGoldInternal => …WouldAdvance(Precept)` en `KagetoraCard` — cumple la promesa de §16 con lógica que **ya existe**.
9. ¿El texto de forma se vuelve dinámico? Hoy la `smartDescription` de `DOCTRINE_POWER` es **la misma cadena antes y después de ascender**, aunque la regla que gobierna cambió por completo. El motor ya inyecta `DynamicVars` en `smartDescription` (`PowerModel.cs:390`) y `DoctrinePower` no declara ninguno.

**Sobre la transformación (R6):**
10. ¿Se rebalancea el NP de Kenshin a **8 impactos** (para que los aditivos por impacto no valgan la mitad), o se compensa de otra forma? Con 4 impactos, el mejor caso realista (OC5 vs Man) es 97 contra los 91 garantizados de Kagetora a OC1 — y Elite/Boss nunca son Man.
11. ¿La mecánica de forma se muda **adentro** de los `FormPower` (que hoy están vacíos), como en Morgan/Artoria/Tiamat, o se acepta que Kenshin sea solo "orden libre + NP nuevo"? Si es lo segundo, **la única forma de que se sienta es la UI**, porque el 91,9 % del mazo no cambia.
12. ¿Se renombra al personaje en el HUD al ascender? Hoy no hay mecanismo (`characters.json` tiene una sola entrada); sería el cambio de mayor impacto percibido por menor costo de assets.

**De proceso:**
13. Los dos bugs P0 (`WasUsed` en Kagetora y Astolfo) son de la misma familia — **comparación nullable levantada sin `?? 0`** —, y hay un tercero con signo opuesto en `JustPathPower`. ¿Va un hook o un analizador que falle el build ante `?.X <op> literal` sin coalescencia, en vez de una regla en prosa?
14. Antes de decidir cualquier número: **el playtest del reporter corrió sobre un build con 12 efectos muertos.** ¿Se re-testea con el fix de `SystemPowers.cs:99` aplicado antes de rebalancear? Es probable que parte de 「太缺费了」 y 「意义不明」 se muevan solos, aunque las causas raíz R1-R6 sobrevivan al fix.

---

### Anexo: contradicciones entre lentes, resueltas

| Punto | Quién tenía razón | Resolución (verificada por mí) |
|---|---|---|
| Línea de `StackType` en `DoctrinePower` | **legibilidad** (`:65`) | transformación dijo `:60`, técnico `:66`. Es `Doctrine/Doctrine.cs:65` |
| Costo medio por precepto (1,04/1,30/1,39 vs 1,05/1,32/1,43) | **ambas, distinta base** | La lente técnica incluye las 6 básicas (23/23/23, cadena 3,73); pool/energía usan solo el drafteable (21/22/21, cadena **3,79**). Para balance de draft vale el segundo |
| Qué texto ve el jugador al hoverear la Doctrina | **legibilidad** | `PowerModel.cs:360`: en combate (`IsMutable`) se usa `smartDescription`, que **no dice el orden**. El técnico citó `.description`, que solo aparece fuera de combate |
| «Kagetora se quedó afuera del estándar de la ventana-NP» | **ninguna del todo** | Verifiqué los call-sites: solo **Tiamat** usa `NpWindow`; Morgan lo hace inline; los demás solo manifiestan la carta. Es una convención documentada y minoritaria → **PARCIAL**, no desvío |
| «Kenshin reduce la energía libre 72 %» | **artefacto de la simulación** | Reproduje los números (0,43 → 0,12⚡), pero el agente del sim persigue ciclos con avidez: Kenshin **convierte** energía ociosa en ciclos (+9,7 %), no la destruye. El dato honesto es que **no toca la restricción vinculante** (3⚡ para 3 cartas) |
| «El NP de Kenshin es estrictamente peor» | **PARCIAL** | Leí `NoblePhantasms.cs:87-90`: el NP de Kenshin **sí** escala con overcharge (+1/impacto hasta OC5) y el de Kagetora **no**. A OC1 es −4 plano; a OC3 la base ya gana. Con los buffs por impacto, solo supera a Kagetora en OC5 vs Man — y Elite/Boss son Earth/Heaven |
| `ArmyFootsteps` «muerta en ambas formas» | **PARCIAL** | Tracé el mask: muerta siempre en Kagetora; en Kenshin hay un caso vivo (cierra ciclo → mask 0 → Pies libre) |
| `EightWeaponsOneWarrior` «techo real 20» | **corregido hacia abajo** | Con 3⚡ el techo real es **+10** (2 avances + esta carta = 4⚡) |
| `SharedGuard` «copia exacta de una común» | **PARCIAL** | Idéntica en un jugador; en co-op da +4 de Bloqueo a cada aliado |
| `WheelStrategy` «nunca paga» | **PARCIAL** | Nunca paga cuando es el primer avance del turno (la línea natural de una carta de Cielo); paga si ya avanzaste algo |
