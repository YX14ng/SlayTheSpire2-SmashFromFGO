# AUDIT-KAGETORA — ¿es mejorable el rediseño V2? (2026-08-21)

> **Estado: AUDITORÍA — pendiente de la revisión adversarial (Fable 5) y de implementación.**
> Análisis: Opus 5, 2026-08-21, sobre **el código en `HEAD`**.
> Encargo: *«redisená Kagetora para ver si es mejorable»*.
> **Respuesta corta: sí, en dos números concretos. No, en la arquitectura — y hay una razón fuerte
> para no tocarla todavía.**
> Revisado adversarialmente por Fable 5 el 2026-08-21: confirmó la conclusión y los dos hallazgos,
> corrigió la prosa de K-1 (§3) y un dato de herencia en §2. Registro completo en
> `REDESIGN-MORDRED-V2.md §10`.

---

## 1. Por qué esto es una auditoría y no un REDESIGN-V3

`docs/REDESIGN-KAGETORA-V2.md` se cerró el **2026-08-16** —hace cinco días— con panel de tres
propuestas y tres jueces, ganador 3–0, y **~50 parches obligatorios de juez aplicados** (J1 P-1…P-16,
J2 K1…K17, J3 J-01…J-17), con las contradicciones resueltas al más restrictivo. Se publicó como
**v0.1.12** (STATUS 2026-08-16, verificado por API).

**Ese rediseño nunca se jugó.** Cero validación en runtime; el propio STATUS lo deja anotado como
pendiente. Volver a re-arquitecturar ahora sería **diseñar encima de diseño no validado**: se tiraría
el trabajo del panel y se agregaría riesgo sin una sola observación de juego que lo justifique. Es
exactamente el error que este proyecto ya pagó con el Enuma Elish de Gilgamesh —dos parches
especulativos sobre el mismo síntoma sin evidencia— y que su propio método prohíbe.

Así que este documento hace lo que sí se puede hacer con evidencia estática: **buscar defectos
medibles**. Encontró dos, y los dos son de número.

---

## 2. Lo que se midió y salió BIEN

Esto no es cortesía: es la mitad del encargo (*«para ver si son mejorables»*), y explica por qué la
lista de hallazgos es corta.

| control | resultado |
|---|---|
| **Conectividad al recurso propio** | **93%** — 72 de 77 cartas llevan precepto. Reparto: Cielo 23 / Pecho 25 / Pies 24, casi perfectamente equilibrado. La rúbrica pide ≥90% en comunes; el pool entero lo cumple. |
| **Riders muertos** (la familia de bug que hundió a Mordred y a Astolfo) | **0**. Se barrieron todas las bi-condicionales `X ? A : B` buscando ramas iguales: ninguna. Los riders de Kenshin/precepto (`KenshinNpCharge=10`, `KenshinBonus=4`, `KenshinBlock=3`, `FirstChestBonus=3`, `KenshinHits=4`) tienen delta real. |
| **Refund de energía del ciclo** | **acotado por construcción y demostrado en el código**. `Doctrine/Doctrine.cs:239-253`: el mask se vacía en el mismo `AfterCardPlayed`, cada avance enciende un bit, y `MaxAdvancesPerTurn = 3` ⇒ **≤1 ciclo y ≤1⚡ por turno**. La prueba está escrita en el comentario, no asumida. |
| **Peligro del contador de 2 bits** | **documentado con prohibición explícita** (`Doctrine.cs:65-72`): subir `MaxAdvancesPerTurn` a 4 wrappea el campo y abre refund ilimitado. Está anotado con el procedimiento para cambiarlo. |
| **Críticos** | **1 por turno**, con válvula (`DoctrinePower.CanSpendCritical`) que además deja pasar las cartas ajenas y las de Pies. Sin ella el banco compraba dos por turno y el breakpoint «un ciclo = 50★ = un crítico» sería mentira. |
| **Retención de Bloqueo tras el cambio de Baluarte de FGOCore v0.1.25** | **intacta**. `KenshinFormPower` declara su `RetentionCap` en `Powers/DoctrinePowers.cs:76-93` y **hereda** el `ShouldClearBlock` + `AfterPreventingBlockClear` correctos de `FGOCore/FGOCoreCode/Forms/FormPower.cs:59-73`; no depende del `BulwarkPower` que ahora decae. |

La arquitectura está en mejor estado que la de cualquier otro personaje del repo auditado hasta hoy.
**No es candidata a rediseño; es candidata a playtest.**

---

## 3. Hallazgo K-1 — una común de 0⚡ domina en tasa a otra común de 0⚡ (MEDIO)

Las dos son de precepto **Cielo**, las dos 0⚡, las dos convierten Estrellas en Carga NP:

| ID | costo | salida | tasa base | tasa mejorada |
|---|---|---|---|---|
| `PrayerToBishamonten` | **20★** | 30 NP (up **50**) | **1,50 NP/★** | **2,50 NP/★** |
| `TurnTheReins` | **50★** | 50 NP (up **70**) | 1,00 NP/★ | 1,40 NP/★ |

`Cards/Common/CommonCards.cs` (clases `PrayerToBishamonten` y `TurnTheReins`).

`Prayer` gana en **las dos** dimensiones que importan: mejor tasa **y** umbral más bajo (20★ contra
50★, o sea que se puede jugar mucho antes y más seguido). `TurnTheReins` sólo tiene sentido como
sumidero cuando estás pegado al tope de 100★ y querés vaciar — un caso de borde, no un rol.

**Propuesta (nerf de la fuerte, no buff de la débil):**

- `PrayerToBishamonten`: 20★ → **20 NP** (up → **30 NP**). Tasa 1,00 / 1,50.
- `TurnTheReins`: **[=]** (50★ → 50 NP, up 70).

Quedan **iguales en tasa base (1,00)** y con roles distintos por **umbral y bulto**, no por tasa:
**Prayer es la granular** (20★, se juega seguido, entra temprano) y **Reins es la de bulto** (50★ de
una, 50-70 NP de golpe, para cuando el banco está lleno).

**Aclaración honesta:** mejoradas, Prayer sigue arriba en tasa (1,50 contra 1,40 de Reins). No hay
denominación legal que invierta eso —80 está fuera de la regla 10/20/30/50/100—, así que la
diferenciación es deliberadamente por **umbral y tamaño de lote**, no por eficiencia. Lo que el fix
sí logra es que la común barata deje de ser **estrictamente mejor en las dos dimensiones**. Y es un
recorte de generación, que es la dirección segura.

---

## 4. Hallazgo K-2 — el par espejo mejorado es positivo-suma (MEDIO-BAJO)

`TurnTheReins` (50★ → 50 NP, up **70**) y `TurnTheFormation` (50 NP → 50★, up **70**), las dos
comunes de 0⚡. Con las dos mejoradas, la ida y vuelta es **−50 NP +70★ → −50★ +70 NP = +20 NP y
+20★ netos por dos cartas de 0⚡**.

Kagetora lo hace **mejor que Mordred** (donde la mejora baja el *costo* de las dos direcciones, que
es peor): acá la mejora sube la salida, así que **sin mejorar el par es exactamente neutro**, que es
lo que un espejo debe ser. La ganancia sólo aparece con dos mejoras invertidas.

**Propuesta: dejarlo como está y declararlo.** Dos cartas de 0⚡ que juntas dan +20 NP y +20★
equivalen a un `WarCry` de 1⚡ repartido en dos cartas; está acotado por copias, por mano y por robo.
**Perilla:** si el playtest lo muestra abusivo, bajar las dos mejoras de +20 a +10.

Lo que **sí** se registra es la regla general que sale de comparar los dos personajes, y que vale
para el próximo diseño del repo:

> **En un par de conversión espejo, la mejora sube la SALIDA y nunca baja el COSTO.** Bajar el costo
> de las dos direcciones convierte el espejo en una bomba de recursos (es el defecto D6 de
> `REDESIGN-MORDRED-V2.md`); subir la salida deja el par neutro hasta que se pagan dos mejoras.

---

## 5. Lo que NO se propone tocar, y por qué

| pieza | por qué se deja |
|---|---|
| El motor de Doctrina (timing, golpe letal, reentrancia, copias, reset, `MaxAdvancesPerTurn`) | Demostrado correcto en el código y con los peligros anotados. Tocarlo sin playtest es riesgo puro. |
| El refund de 1⚡ por ciclo | Es el único retorno de energía del kit y su cota está probada. |
| Las cuatro líneas de draft (Rueda / Muralla / Caballería Crítica / Ejecución) | La matriz de cobertura de V2 §2 se verificó contra el pool implementado: las cuatro tienen ataque, defensa, consistencia y energía. |
| La ascensión a Kenshin y sus riders | Nueve cartas con delta real; ninguna muerta. |
| El NP y las tres cartas de precepto | Sin hallazgo. |

---

## 6. Total del cambio propuesto

**1 ID re-especificado** (`PrayerToBishamonten`, dos números) + **1 regla documentada**.
Cero renames, cero borrados, cero cambios de rareza, cero cartas nuevas, FGOCore intacto ⇒ se
publica solo `KagetoraLancer` (v0.1.12 → v0.1.13) si el cambio se aprueba.

**Riesgo declarado:** el nerf de `PrayerToBishamonten` toca la línea A (La Rueda), que es la que
depende de cargar NP rápido. Si el playtest la muestra lenta, la perilla es devolver la mejora a
+30 NP **antes** de tocar cualquier otra fuente de carga.

**Nada de esto está validado en runtime.** La recomendación operativa sigue siendo: **jugar la V2
antes de volver a diseñarla.**
