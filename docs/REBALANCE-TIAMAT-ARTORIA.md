# REBALANCE — Tiamat (nerf) + Artoria Caster (buff) — propuesta 2026-08-15

> **IMPLEMENTADO 2026-08-15** (aprobación del usuario, "ambos"): FGOCore v0.1.24, Tiamat v0.1.19,
> Artoria v0.1.20. Desvío sobre lo aprobado: **A6** usó el rider «con Crítico Listo: +4/+5» en vez
> de «+10★ en Berserker» — esa versión calcaba a Estocada de la Pradera (común 1⚡: 8 + 10★ en
> Berserker) y habría creado otro duplicado; el rider crítico es además el intent original de la
> carta (6 / Crítico 2★: 13). A7 además pasó el power a `Counter` (el icono muestra 1/2) y movió
> su contador a los bits 9-10 del estado de turno (7-8 ya los usaban Búho/Espada Forjada).
> Pendiente: playtest y publicación con orden explícita.

Origen: reportes de Steam en la página de Tiamat (OneLostGamer 08-07 "insanely OP… pretty much
immortal every run"; 七煌夜 08-14 "强度太超模了" / "从头到尾几乎吃不了什么战损"; transfox girlfail
08-14) y nota de Obsidian con el comentario chino sobre Artoria Caster (cartas bajo curva tras la
consolidación Critical v2). **PROPUESTA — pendiente de aprobación del usuario. Números pendientes
de playtest salvo indicación.**

---

## 1. Tiamat — diagnóstico (verificado en código)

Identidad declarada (REDESIGN-TIAMAT): tempo-controladora que "sobrevive cualquier turno" con
HP 70 **frágil** en fase Lily. El problema: dos motores eliminan la fragilidad por completo —
bandera roja de la rúbrica ("acumulación permanente apilable sin cap" + "motor que elimina la
debilidad declarada").

1. **Bloqueo gratis del enjambre escala con Crianza y no tiene techo.**
   `LahmuSwarmPower.BeforeSideTurnStart` da `n × (2 + Crianza)` de Bloqueo POR TURNO sin jugar
   carta (`FGOCore/FGOCoreCode/Lahmu/LahmuSwarmPower.cs:73`). Con 6 Laḫmu y Crianza 3 son
   30/turno gratis; la Crianza no tiene tope, así que el ingreso pasivo crece sin límite.
2. **Baluarte permanente que compone.** `BulwarkPower` es `Counter` sin decaimiento ni tope:
   cada Carapace (×3 EN EL MAZO INICIAL, 6-9 por uso), Charco de Marea (5), Crisálida (18) y cada
   tick de Limo Protector (+3/turno, `ProtectiveSiltPower`) agrandan el tope de retención PARA
   TODO EL COMBATE. El Limo es "Metallicize en Baluarte": estrictamente mejor que Metallicize
   porque el bloqueo nunca se limpia y se acumula turno a turno.
3. Resultado: ingreso defensivo pasivo (enjambre + Limo) + piso retenido creciente ≫ output
   enemigo → cero daño recibido; mientras, mordidas `n×(1+Crianza)` (×2 en Bestia) + DoT de
   Maldición matan solas. Los tres reportes describen exactamente esto.

### Nerfs propuestos (todo el daño ofensivo queda intacto)

| # | Cambio | Antes | Después | Razón |
|---|---|---|---|---|
| T1 | `LahmuSwarmPower`: el BLOQUEO deja de escalar con Crianza | `n×(2+Crianza)`/turno | `n×2`/turno (máx 12) | La Crianza queda como recurso OFENSIVO (mordidas/Devorar). Corta el ingreso pasivo sin tope. La mordida no cambia. |
| T2 | Limo Protector → Bloqueo plano (sin Baluarte) | +3 Baluarte-Bloqueo/turno acumulativo | +4 Bloqueo/turno (mejora +2→6) | Vuelve a ser el Metallicize temático; deja de componer el piso retenido. Compensación +3→4 por perder retención. |
| T3 | Carapace: el bono por Laḫmu deja de ser Baluarte | 6 (+1/Laḫmu, máx +3) todo Baluarte | 6 de Baluarte + el bono como Bloqueo normal | La parte "los cuerpos amortiguan" es efímera; solo el caparazón propio se retiene. |

- **Alcance técnico:** T1 en FGOCore (fórmula interna, sin cambio de API pública — no rompe
  linkage, pero se republica el lote igual por prudencia de comportamiento); T2/T3 Tiamat-local.
  Ningún ID cambia; saves intactos.
- **NO tocar:** mordidas, Maldición, Devorar, ventana Bestia, TidePool/Crisálida (jugadas
  puntuales acotadas), curación (Lágrimas 2/devorado y Mar Azur 8 son menores).
- **Sugerencia de transfox girlfail** (mazo inicial de strikes/defends básicos y que las mejoras
  agreguen curse/bulwark): NO adoptar literal — la identidad exige sembrar Maldición/Laḫmu desde
  el turno 1 (regla 4.6: básicas conectadas). El problema real era el ingreso pasivo sin tope,
  no el frontload del mazo inicial.
- **Knobs de reserva si el playtest sigue alto:** `BlockPerLahmu 2→1` (REDESIGN §Riesgos-5);
  tope duro de Baluarte para Tiamat (~25) como guarda local.

---

## 2. Artoria Caster — diagnóstico (verificado en código)

El comentario chino es correcto en TODOS sus puntos. Causa raíz: la consolidación **Critical v2**
(×1.5 automático a 50★, `CritStarsPower.CritCost=50`) eliminó las tablas de crítico por carta
(`Crit`/`CritCost`/`PerStar`, podadas como código muerto en la auditoría 2026-08-09b), pero esas
cartas estaban tasadas como "base baja / payoff crítico alto" (DESIGN-ARTORIA §7). Al quedar solo
la mitad base, quedaron bajo curva:

| Carta | Hoy | Diseño original (base / crítico) | Ancla vanilla citada |
|---|---|---|---|
| Estrella Fugaz (PC 0⚡) | 3 plano | 3 / 10 (2★) | — |
| Juicio de la Estrella (PC 1⚡) | 8 plano | 8 / 20 (3★) | peor que 2 comunes propias de 9 |
| Golpe del Anhelo Heredado (RARA 2⚡) | 14 plano | 14 / 32 (4★) | Relentless: PC 2⚡ 14 + rider |
| Embestida Temeraria (PC 3⚡) | 26 plano | 26 (sin ★, intencional) | Sunder: 26 + 3⚡ al matar |
| Marea de Estrellas (PC 1⚡) | 30★ | "3★" pre-conversión ×10 | 50★ = 1 crítico; 30 no llega |
| Tajo de la Espada Sagrada (común 1⚡) | 9 plano | 6 / 13 (2★) | duplicado exacto de Proyección de Caliburn (9) tras el buff 6→9 de 08-09 |
| Recarga de Hechizos (RARA 2⚡) | 1ª Habilidad/turno −1⚡ | Append 5 | única "energía" del pool y es rara |

Además: **cero Vulnerable** en el kit y ninguna carta genera ⚡ (solo la reliquia Diadema).

### Buffs propuestos

Principio: devolver el payoff crítico DENTRO de Critical v2 (rider "con Crítico preparado", que
lee `CritReadyPower` — legible, sin economía paralela), no inflar todo a daño plano.

| # | Carta | Propuesta (base / mejora) | Nota |
|---|---|---|---|
| A1 | Estrella Fugaz | **5 / 8** plano | 0⚡ sin rider vale ~5-7. |
| A2 | Juicio de la Estrella | **9 / 12; con Crítico preparado: +5 / +7** | Con setup 14/19 pre-×1.5 → payoff real ~21/29. |
| A3 | Golpe del Anhelo Heredado | **16 / 20; con Crítico preparado: +8 / +10** | Con crítico ~36/45 — rareza justificada, cerca del intent (32/40). |
| A4 | Embestida Temeraria | **26 / 33; si mata al objetivo: recuperás 2⚡** | Ancla Sunder que citó el reviewer. |
| A5 | Marea de Estrellas | **40★ / 60★** | 0.8 de un crítico; DESIGN §8.bis ya pedía generadores +25%. |
| A6 | Tajo de la Espada Sagrada | **9 / 12; en Berserker: +10★** | Rompe el duplicado devolviéndole su hilo ★; Proyección queda como beater plano. |
| A7 | Recarga de Hechizos | **coste 2⚡→1⚡; mejora: aplica a las 2 primeras Habilidades** | Ataca el agujero de energía sin carta nueva. |
| — | Vulnerable | **NO agregar** | Decisión de identidad: el amp de daño de Artoria ES el crítico global ×1.5; sumar Vulnerable duplicaría el amp y empujaría al techo de saturación. Débil ya existe (Reprimenda, Mirada Feérica). |

- Ningún ID cambia; solo números, riders y loc (5 idiomas + SimpleLoc: ojo `/+` en los "+X").
- Banderas rojas revisadas: el refund de A4 es acotado (al matar, precedente vanilla); los
  riders A2/A3 no crean loops (no generan ★ ni ⚡); A6 genera 10★ ≤ tasa 1★≈½⚡ equivalente.

---

## 3. Alcance de implementación (cuando se apruebe)

1. FGOCore: T1 (+ posible bump de versión y lote de 12 por prudencia).
2. Tiamat: T2, T3 + loc (esp/eng/zhs/kor/rus) + descripción runtime del enjambre (ya es dinámica).
3. Artoria: A1-A7 + loc ×5.
4. `tools/audit_simpleloc.ps1` + matriz MAIN/BETA + publish del lote afectado.
5. Responder a los reporters de la página de Tiamat tras publicar.

**Pendiente de playtest (no son errores lógicos):** todos los números de §1 y §2; el knob
`BlockPerLahmu`; si el Limo a 4 plano queda débil, subir a 5.
