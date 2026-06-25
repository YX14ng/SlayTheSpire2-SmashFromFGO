# DESIGN-REVIEW — mejoras a los personajes vs la disciplina Togawa

Fecha: 2026-06-25. Revisión de los 8 servants implementados contra los principios de diseño del
mod **Togawa Sakiko** (`iryuko/sts2-mod-dev`). Aterrizado en el CÓDIGO real (no solo los DESIGN docs).
Fuente de los principios: [su mechanics-overview / pressure-system / starter-kit]. Reglas → [DECISIONS.md](DECISIONS.md).

## Los principios Togawa (el estándar)
1. **Identidad RANKEADA**: jerarquía de roles explícita; el NP/ejecución es un **payoff de alta rareza, no el loop base**.
2. **Recurso núcleo = divisa que se GASTA** en payoffs específicos (no auto-daño / auto-amplifica / auto-decae).
3. Gastar el recurso **manifiesta cartas temporales** (efímeras/exhaust) o payoffs que expresan la identidad.
4. Estados/debuffs **específicos** con textura de decisión (no un debuff genérico).
5. Mazo inicial = subset `Basic` que **enseña la firma**, integrado al kit.
6. **Riesgo-recompensa** temático (Berserker = decisiones activas de alto riesgo).
7. Reliquia inicial = **ignición pura** del motor, no stat-stick.

## Los 2 hallazgos transversales (lo más importante)
- **El "ulti gratis eclipsa al mazo" YA se arregló en código**: todos tienen el modelo VENTANA-NP (el medidor abre una ventana de 1 turno, la carta-NP es el clímax dentro), no una carta auto-win gratis. ✓
- **Pero reapareció homogeneización en 2 lugares nuevos**:
  1. **Las ventanas-NP son el mismo esqueleto** (+1⚡ / robar 1 / "potenciá tu motor"); las cartas `*Unleashed` comparten molde (`Event` + `Retain|Exhaust` + auto-a-100 + `ConsumeAllForNpCard` + `NpLevels`). La identidad vive solo en el último 20% (el payload). Solo **Mash** la diferencia bien (Bloqueo→daño).
  2. **Morgan ≈ Castoria**: tras el pivote de Morgan (2026-06-13) a Buster/Estrellas, los dos son "Caster junta ★ / Berserker gasta ★ en crítico ×2 / danza de formas". **Mismo personaje con skin distinto** — la corrección de mayor impacto.

## La cura universal (1 sola idea)
**Anclar el disparo del NP (y del Crítico) al recurso PROPIO de cada personaje, no al medidor/crítico genérico de FGOCore.** No toca la restricción del ×daño global ni el techo de daño — solo cambia *qué decisión gatea el disparo*. **Siegfried ya lo hace** (NP manual `Balmung` atado a Sangre de Dragón, sin `GaugeFilled`) → es el patrón de referencia.

| Servant | Recurso propio | ¿NP lo usa hoy? | Mejor mejora (P1) |
|---|---|---|---|
| **Morgan** | Maldición (vestigial) | no | **Re-anclar a Maldición**: Berserker detona Maldición del objetivo; ventana = Sentencia AoE; starter siembra Maldición. Deshomogeneiza vs Castoria. **(MAYOR ROI)** |
| **Castoria** | Estrellas + Anti-Purga | sí (★) | **Re-armar la ventana** (hoy es 1/combate por una asimetría de código → la hace lineal); payoff que gasta Anti-Purga (su divisa única) |
| **Okita** | Aliento (el mejor recurso del roster) | **no** | **NP lee/gasta Aliento** (su firma, hoy ausente del clímax); Crítico reembolsa 1 Aliento |
| **Gilgamesh** | Oro + Armas | no | Oro **manifiesta Armas raras** (no paga números); Enuma consume Armas como Sobrecarga; riesgo estructural de Arrogancia (perdés el buff al bloquear) |
| **Mordred** | Formas (casco) | parcial | Crítico **manifiesta «Chispa de Clarent»** (token efímero); rankear identidad por el casco; coste de parking escalante en Rebelión |
| **Oberon** | Deuda (ejemplar) | auto-gratis | NP escala con **Deuda gastada** (no solo medidor); Deuda manifiesta «Pagaré» efímero (lore de colapso gradual) |
| **Siegfried** | Sangre de Dragón | sí ✓ | **Sink activo**: «Erupción de Escamas» consume SdD → AoE; auto-daño-por-recurso (lore héroe maldito) |
| **Mash** | Bloqueo/Baluarte | sí ✓ (Bloqueo→daño) | Anti-parking en la ventana (payoff pleno solo si atacaste); Estrellas con sabor de muralla. Es la referencia de identidad — pulido, no rescate. |

## Orden de prioridad recomendado
1. **Morgan → Maldición** (P1): arregla identidad/divisa/debuff Y la homogeneización Morgan↔Castoria de un saque. **El de mayor impacto.**
2. **Universal: cada NP lee/gasta el recurso de firma** (Aliento/Armas/Formas/Deuda) — ataca la homogeneización del NP sin tocar números de daño.
3. **Castoria: re-armar la ventana** (corrige una asimetría de código que la hace más lineal que sus pares).
4. El resto (Mash/Mordred/Gil/Siegfried) = pulido por personaje, P2/P3.

## Deuda de documentación detectada
- **REDESIGN-MORGAN.md está OBSOLETO**: describe un motor de Maldición que el código reemplazó por Estrellas (2026-06-13). Si se adopta Morgan-P1, el doc vuelve a ser correcto — reconciliar doc↔código antes de tocar.
- (Detalle completo por personaje en la revisión de los 3 agentes; este doc es el resumen accionable.)
