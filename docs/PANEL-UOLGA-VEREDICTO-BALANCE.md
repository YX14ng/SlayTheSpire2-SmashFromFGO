# PANEL U-OLGA — dictamen del juez adversarial de BALANCE (2026-08-23)

Juez 1 de 3 del panel §4.6.7. **Sus parches MANDAN** (regla del workflow). Los jueces 2 (fidelidad
y legibilidad) y 3 (implementabilidad) **quedaron sin correr**: murieron por límite de sesión —
hay que relanzarlos antes de escribir el pool definitivo.

Material juzgado: [`DESIGN-UOLGA.md`](DESIGN-UOLGA.md) (acta, ley) +
[A](PANEL-UOLGA-A-RAFAGA.md) · [B](PANEL-UOLGA-B-CAZA.md) · [C](PANEL-UOLGA-C-PRESUPUESTO.md).

## Veredicto

**Base: PROPUESTA B («La Caza y el Martirio»)**, con injertos de C y A. La A se rechaza como base.

1. **El starter correcto es el de B/C** (recibir un golpe que te quita Vida → +10 NP): es el 3%
   canónico del acta §0, crece con los actos (10-20 NP/turno en Acto 1, cap 30 en Acto 3) y hace del
   Bloqueo un dial. El de A (jugar Ataque → +10 NP) es un grifo plano controlado por el jugador
   desde el turno 1: vuelve trivial la banca a 300, colapsa la decisión central del acta §1 —su
   propia auto-crítica lo admite— y hace double-dip con sus riders de Ataque.
2. **El keyword Mal con dos modos impresos** (pleno como Bestia / disminuido como Protectora) es la
   única solución de las tres a las cartas muertas post-F3, y la única propuesta que demuestra en
   papel que la línea sin-Guts no queda coja.
3. El **fix del append `技能再装填`** es de B y se adopta.
4. Los defectos de B son numéricos y parchables; los de A y C son estructurales (starter y curva de
   conversión respectivamente).

**Injertos de C:** el candado de auditoría del starter (sólo golpes de origen enemigo; la Vida
imparable pagada no factura en NINGÚN lector), la tabla de cambio como formato de verificación,
«Garantía de Cumplimiento» (conversión→Bloqueo) y el nombre «Autoridad Delegada».
**Injertos de A:** el cap global +15 a los bonos del Decreto, el candado «el token no critica», y
«Trono de la Bestia VII» como rara Mal.

## Las tres cuentas de pico, rehechas

- **A dijo 174/206 — FALSO por su propia hoja de reliquias**: su Bond imprime `ServantDamageMultiplier`
  ×1.25 y no lo aplicó → 217 típico / 257 en el stretch. Dos agujeros más: **doble Decreto en un
  turno** (re-convertir con ventana activa) y, con appends maxeados, su token crítico pega 148 en
  una carta de 0⚡ (turno ~237-275).
- **B dijo 216 — subestimado**: usó ×1,5 por crítico con 50★; el motor compartido es **100★ →
  Crítico Listo → ×2**. Recontado: **225**, y hasta **242** con 200★ construibles. Es el bust más
  chico y se arregla con los parches.
- **C dijo ~221 — FALSO dos veces**: no contó su propio Bond ×1.25 y disparó la Desatada a 150 en vez
  del turno construible a 300. Con su sobrecarga +1 por cada 20 y el +50% multiplicativo anti-Humano:
  **281 sin Bond, ~351 con él**.

## El arbitraje central: la fórmula del token

A y C tienen razón **sobre la fórmula del otro**. Con `5 + tier/10` (A) la banca a 300 domina 2:1;
con `tier/10` (B/C) convertir a 100 rinde 20 de daño total contra una Desatada de ~35 AoE inmediata y
nadie toca el botón. Como el CONTEO de tokens ya escala con el tier (ley: 1 por 50), cualquier
daño-por-token creciente vuelve el total superlineal.

### FÓRMULA DICTADA: **daño del Decreto = 10 + tier÷10, tope 30.**

| Tier | Cargas | Daño/token | Total ventana | Eficiencia /100 NP | Desatada+ Lv1 (vs Humano) | Desatada+ ×3 enemigos |
|---:|---:|---:|---:|---:|---|---:|
| 100 | 2 | 20 | **40** | 40 | 35 (50) | 105 |
| 150 | 3 | 25 | **75** | 50 | 35 (50) | 105 |
| 200 | 4 | 30 | **120** | 60 | 40 (55) | 120 |
| 250 | 5 | 30 | **150** | 60 | 40 (55) | 120 |
| 300 | 5 | 30 | **150** | 50 | 45 (60) | 135 |

Eficiencia en banda 40-60 (ratio máximo 1,5:1). **El óptimo de la conversión es 200-250 y el 300 le
pertenece a la Desatada.** Las tres decisiones quedan defendibles por contexto: a 100 convertís
contra un élite único y disparás contra sala múltiple; a 200-250 es la línea golosa contra jefe largo
(120-150 garantizados que sobreviven cualquier strip); a 300 no convertís, firmás el gasto. El reloj
de 5 turnos y el 1 token/turno son el descuento que impide que 250 domine en peleas cortas. Cap 5 y
tasa 1/50 intactos (ley del acta).

## Loops y economías netas — hallazgos

1. **Espejos mejorados = +20 por ciclo, en LAS TRES** (A consume 30→+50; B gastá 50→+70 en ambas
   direcciones; C consume 40→+50). Bandera roja de la rúbrica: coste cero repetible neto positivo.
2. **Doble Decreto por re-conversión**: lo permiten las tres.
3. **Token + Crítico Listo (A)**: un 0⚡ de 100-148.
4. **«Anexo al Decreto» apilable sin cap (B)**: dos copias mejoradas = +40 al token.
5. **«Reasignación de Fondos» mejorada (B)**: 20★ → robá 2 a 0⚡, robo demasiado barato.
6. **Triple motor de exposición (C)**: starter + Doctrina + Cobertura, tres contadores. No se hereda.
7. Verificados limpios: el token no alimenta nada, la sangre es de sentido único, crit→NP con retorno
   0,2, cero conversiones hacia ⚡ y cero recursos→Vida (salvo una curación con Agotar).

## Parches obligatorios

1. **Base = Propuesta B íntegra** (mazo inicial, starter Brazalete, pool 20/28/20, Mal dual, F3,
   reliquias) con los parches siguientes.
2. **Decreto: `tier÷10` → `10 + tier÷10`, tope 30.**
3. **Cap global +15 a los bonos aditivos del Decreto** (cartas + reliquias + Fuerza + Instinto). El
   append `追撃技巧向上` (+3/+6) va POR ENCIMA del cap (acta §6).
4. **El Decreto no critica ni consume Crítico Listo.**
5. **Re-convertir con ventana activa REEMPLAZA la ventana** (los tokens no entregados se agotan).
   Cierra el doble-Decreto y restituye el «5 extras en 5 turnos» auditable del acta §2.
6. **Espejos: la mejora baja el insumo 50→40; la salida queda fija en 50.** Cierra el +20/vuelta.
7. **«Reasignación de Fondos»: la mejora pasa a costar 10★ en vez de robar 2.**
8. **«Presupuesto Total»: mejora «siempre +100» → «+70; contra Amenaza +100».**
9. **«Gravedad de un Planeta»: tope +2 → +1 Fuerza** (cada punto son +5 en el NP y +1 por token).
10. **Semántica de crítico = la del motor**: 100★ → Crítico Listo → ×2, una carga. Toda cuenta futura
    usa esto (la auditoría ×1,5/50★ de B queda anulada).
11. **Bond SIN multiplicador global de daño.** Lifts de Bloqueo/regen y el capstone «curá 10 al
    levantarte» quedan. ⚠️ Verificar en código si `BondRelic` impone un lift de daño heredado; si es
    obligatorio, recortar el pool ANTES de publicar (empezar por el cap +15 → +10).
12. **Desatada dictada**: por impacto `5 + 2·Lv` (mejora `7 + 2·Lv`); sobrecarga +1 por impacto por
    cada 100 sobre el mínimo; **Humano +3 por impacto ADITIVO** (mejora +5) — se prohíbe el +50%
    multiplicativo de C; Estrella +1 por impacto (huevo de pascua, nunca línea de balance).
13. **Se elimina «Fragmento de Cáldeas»** (subsidio a bancar) y **entra «Garantía de Cumplimiento»**:
    al convertir, 5 de Bloqueo por carga (máx 25).
14. **La rara «Peso del Mundo» se reemplaza por «Trono de la Bestia VII», impresa dual**: Mal = +1⚡ al
    inicio de tu turno; Protectora = 3 de Bloqueo. Cubre el hueco de economía de ⚡ y hace que perder
    los Mal al transformarse DUELA.
15. **Renombres canon**: la rara «Ultra Manifiesto» pasa a **«Autoridad Delegada»** (ウルトラマニフェスト
    es el H1 de la Forma 3 y no se ocupa); la carta de conversión muestra **アルテミット・U** en F3.
16. **Append `技能再装填` → versión B**: bajo «tu primera conversión de cada combate deja +20 NP de
    arranque en el medidor», alto +50. Cierra el riesgo del acta §8.
17. **驚天動地 ratificada**: 1 Vida por cada 10 faltante hasta 100, 1 por cada 5 por encima; la Vida
    pagada no factura en NINGÚN lector, contadores incluidos.

## Pico corregido de la base parcheada

**Sin appends — línea Mal (nunca se transformó), jefe-Amenaza único, 8 condiciones simultáneas:**

| Jugada | ⚡ | Cálculo | Daño |
|---|---:|---|---:|
| Decreto (tier 250) | 0 | 30 + cap +15 | 45 |
| Desatada @300 Lv3 | 0 | (7+6+2+1)×5 + 3 | 83 |
| Trofeo de Caza, crítico | 2 | (20+10+3+1)×2 | 68 |
| Desdén en Contraluz | 1 | 12+3+1 | 16 |
| **Total** | **3** | | **212** |

**212, dentro del techo 180-220 en banda alta.** Línea Protectora análoga ≈ 204 a objetivo único más
el Decreto en área: las dos líneas siguen competitivas. Peor esquina construible (Trofeo mejorado y
segundo crítico con 200★ bancadas): ~240 → watch-list #1, sin parche preventivo.
**Con appends maxeados: ~268** — por encima de la vara **por diseño del acta §6**; se documenta, no se
recorta el personaje base por eso.

## Watch-list de playtest (por riesgo)

1. **Esquina de doble crítico** (~240): knobs en orden — rider de Trofeo +10→+6, después Instinto +3→+2.
2. **Uptime de Autoridad convirtiendo a 100 en cadena**: si el sostenido pasa de ~90-110/turno, el
   token a tier 100 baja de 20 a 15.
3. **Motor apagado en salas sin ataques enemigos**: medir el % de turnos con el starter en 0; si supera
   el 25%, crece «Zona de Exclusión» o su efecto migra a la Ancient del starter.
4. **Pesca del Guts en el fallback de élites** (dejarse matar barato para la F3): si transformarse
   temprano domina, apagar «Planta Atómica» el primer turno o 逆光 de 3→2 turnos. Nunca agrandar el castigo.
5. **El lift del Bond en FGOCore** (parche 11).
