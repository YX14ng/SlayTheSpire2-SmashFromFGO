# DESIGN-REVIEW-2 — segunda pasada (balance · profundidad · gaps de implementación)

Fecha: 2026-06-25. Continúa [DESIGN-REVIEW.md](DESIGN-REVIEW.md) (1ª pasada = homogeneización del NP, **ya
implementada**). Esta pasada NO toca el NP-anchor; mira **balance vs vanilla, profundidad de pool, sinergia,
arco de run, cartas muertas**. Aterrizado en el CÓDIGO real (los agentes verificaron a mano; corrigieron
varias alucinaciones). Baselines: skill `sts2-mechanics-design` (común 1⚡ ≈ 9-10 daño; pool vanilla ~82 drafteables).

## El hallazgo dominante: NO todos están al mismo nivel de IMPLEMENTACIÓN (eso ES el desbalance)
| Servant | Estado real | Recurso de firma ¿vive en código? |
|---|---|---|
| Okita | **completo** (~65 cartas, NP-ancla, Aliento/Tos cableados) | sí — listo para tunear números |
| Oberon | **completo** (68 drafteables, 4 arquetipos, Sleep real) | sí |
| Mash / Morgan / Castoria | completos y balanceados | sí |
| Siegfried | kit excelente pero **pool de solo 24** (6 PC) | sí, pero poco contenido |
| **Mordred** | 60+ cartas PERO **mazo inicial genérico** (sin Buster/Arts/Quick) | parcial — falta motor en el deck base → acto 1 roto |
| **Gilgamesh** | 49 cartas PERO **Oro y Armas DESCONECTADOS** | **NO** — nada genera Armas, ningún rider de oro funciona |
| **Tiamat** | rediseño dos-pozas PERO **pool Lily de 15** + SkillSeal placeholder | sí, pero pobre y con control roto |

## Prioridad GLOBAL (lo que más importa)
1. **GILGAMESH — implementar el motor de Armas + módulo Oro/Tesoro.** Verificado: no existe `Arsenal.AddRandom`, no hay módulo Gold, no hay Bab-ilu; **ninguna carta genera Armas** (`ArmsPlayedPower` siempre 0). → ~40% del pool de Gil es PAPEL hoy (`TreasureGuardsTheKing` = 0 bloqueo, todos los "Pagá X Oro" impagables). Es el peor desbalance del roster, **por omisión**. Mientras tanto, sacar del draft las cartas injugables para no ensuciar recompensas.
2. **TIAMAT — `SkillSealPower` es un no-op placeholder** (`Powers/SkillSealPower.cs`): `TidalSeal` (carta del pool) y el NP de apertura `NammuDuranki` prometen un control que **no ocurre** (el enemigo igual usa su habilidad). Es **contenido roto**, no solo número. Fix: implementarlo de verdad (el **SleepPower de Oberon YA skipea la intención enemiga** — copiar ese patrón) o rediseñar el efecto a algo que funcione (−Fuerza/Débil) y renombrar.
3. **MORDRED (y Gil) — cablear el mazo inicial QAABB** (Buster/Arts/Quick de comando, como YA tiene Okita). Sin generación de NP/★ en el deck base, el acto 1 está roto por falta de motor, no por números.
4. **PROFUNDIDAD de pool: Tiamat (15 Lily) y Siegfried (24)** vs baseline ~82. Feast-or-famine por **escasez de opciones** (mismas cartas cada run). Subir a ~26-30 sobre los motores existentes (no mecánica nueva).
5. **TIAMAT — la fase Lily se siente "sala de espera"** (daño turno-a-turno bajísimo + control roto → solo cargás el medidor). Darle un loop ofensivo: knob `BitePerLahmu` 1→2, o que el enjambre muerda también en tu turno.

## Pulido de balance por personaje (P2/P3)
- **Mash** (balanceado, sesgo defensa): falta cierre ofensivo en acto 3 → 1 rara Baluarte→daño fuera de la ventana NP. `ConceptualAmmo` = power muerto (trigger casi nunca proca) → re-perfilarlo a "quitar buff → ★".
- **Morgan** (motor Maldición OK, cap real **25** no 15): `CursedBolt` es **clon mecánico de `QuickMorgan`** (carta muerta) → re-perfilar (3 Maldición o AoE). Eje "Sangre de la Reina/HP" subdimensionado (~4 cartas, sin cura) con HP 72.
- **Castoria** (doble economía sólida): el Crítico es **feast-or-famine binario** (umbral 4★) → escalar daño con ★ extra en vez de umbral. Rescatar `StarTide`/`FaerieAegis` (Exhaust-setup dominadas). Avalon (clímax) hace Berserker obsoleto.
- **Okita** (el más maduro): feast-or-famine de Aliento (Ráfaga colapsa sin `TennenRishinBreath`) → subir `SteadyStep` a común o +Aliento en el BondRelic. `Feint`/`MagicResistanceE` flojas.
- **Oberon** (pool más profundo, 68): subtema "Sueño" feast-or-famine (payoffs muertos sin un dormidor) → mover un dormidor a común o que `Nightfall` aplique 1 Dormido. Combo `SovereignDebt`+`ScaleAvalanche` (42 a 2⚡) = outlier a vigilar. `IronEyes` anémica.
- **Siegfried** (kit excelente, pool POBRE): subir a ~30-34 drafteables (+6-8 PC) = su mayor ROI. `BalmungSwing` común débil (6 daño/1⚡) → 9 o lee SdD. Estrellas vestigiales (sin payoffs) → darles uno o sacarlas.
- **Mordred**: `Saberface` (Power) tiene **escalado infinito** (+2 Ataque permanente/turno) → ponerle cap (tipo Makoto de Okita). Riders mono-forma fragmentan la mano → volver ~3-4 bi-condicionales suaves.

## Correcciones a docs (deuda detectada)
- Cap de Maldición ya es **25** (no 15 que dicen docs viejos).
- Ventana de Castoria **ya se re-arma** (no es lineal — fix 2026-06-15).
- Cartas `CardRarity.Event` (ChalkWall, ChaldeaLibrary, EnumaElis viejo, etc.) **NO son drafteables** (excluidas a propósito; pendientes de borrado).
- `DESIGN-TIAMAT.md` (single-pool) superseded por `REDESIGN-TIAMAT.md`; ambos dicen "~14 pool Lily" como si alcanzara — no alcanza vs el baseline 82.

## Lectura para el lead
La 1ª pasada arregló la **identidad** (cada NP usa su recurso). La 2ª revela que **3 personajes tienen el recurso de firma roto o ausente en código** (Gil = Armas/Oro; Mordred = motor en el deck base; Tiamat = control SkillSeal) y **2 tienen pools demasiado chicos** (Tiamat 15, Siegfried 24). **No tiene sentido tunear números de Gil/Mordred/Tiamat hasta que su motor exista**; Okita/Oberon/Mash/Morgan/Castoria ya están listos para playtest de balance.
