# DISEÑO — Artoria Caster (Castoria): «La Estrella de la Esperanza»

> Personaje 3 del proyecto. **Rol inicial CASTER que cambia a BERSERKER** (encargo del
> usuario) — el espejo invertido de Morgan (Berserker→Caster). Diseño v1 sintetizado de un
> panel de 3 propuestas × 3 jueces (2026-06-11): base = propuesta «formas» (ganadora 2/3
> jueces), con la disciplina de traducción de la propuesta «motor» (skills reales→Exhaust,
> Anti-Purga anclada a Impervious) y las herramientas de danza de la propuesta «fidelidad»
> (Escapada de Verano, Lupa de la Detective). Kits reales verificados contra Atlas Academy
> (JSONs en `assets/reference/castoria_*_nice_servant.json`; investigación completa en
> `assets/reference/artoria_design/`).

## 1. Identidad

**«La estrella de la esperanza»**: arranca como la Niña de la Profecía (Caster) que forja
Estrellas y maná para proteger, y abre ventanas de Berserker de verano para cobrarlas como
Críticos devastadores a un solo enemigo. Caster genera, Berserker gasta; Around Caliburn
anula lo imparable, Hope Will Camelot ejecuta.

- **Forma inicial: Caster** (soporte/defensa/generación). Mod id **`ArtoriaCaster`**
  (inmutable), dependencies `["BaseLib", "FGOCore"]`.
- vs Morgan: Morgan pega de entrada y aprende a sostener; Castoria sostiene de entrada y
  aprende a pegar. Colisión temática deliberada: «Protección del Lago» existe en ambos
  pools (rango C de Morgan vs A de Castoria, prefijos de mod distintos) — NO «corregirla».

## 2. Los kits reales (resumen verificado)

**Castoria (Caster 5★, LB6, collectionNo 284, id 504500)** — el soporte Arts definitivo:
- S1 Carisma de la Esperanza B: party ATK+20% + carga NP 30% (CD 6).
- S2 Protección del Lago A: un aliado +20% carga; party NP-gain +30% (CD 5).
- S3 Espada de Selección EX: un aliado Arts+50%, Special Attack +50% vs «Amenaza a la
  Humanidad», Invencible 1t (CD 6).
- NP **Around Caliburn** (Arts, soporte, SIN daño): party ATK up (30-50% por NP level),
  limpia debuffs, **Anti-Purge Defense** 3t — daño a 0, NO puede ser perforada por
  Ignore-Invincible; el **Overcharge escala la CANTIDAD de usos 1→5** (Count verificado).
- Pasivas: Magic Resistance A, Territory Creation EX, Unique Magecraft B (crit dmg Arts).
- Append 5 Skill Reloading: tras usar una skill avanza su cooldown.

**Artoria Caster Berserker (5★ verano 2023, collectionNo 386, id 704700)** — boss-slayer
crítica Arts ST autosuficiente:
- S1 Pulso de Primavera EX: crit dmg +50%, absorción de estrellas +5000%, NP +50% (CD 6).
- S2 Hada del Verano B: un aliado NP+20% y **Overcharge +2** (1 vez); party NP-gain +20%.
- S3 Manipulación de la Espada Sagrada A: Arts +50%, **10 estrellas/turno** ×3t,
  Invencible 1t.
- NP **Hope Will Camelot** (Arts ST, 6 hits, Anti-God): party «Anti-Enforcement DEF»
  (1 uso, daño a 0, no perforable) ANTES del daño; 900-1500% ST; al enemigo −20%
  crit/estrellas. OC: Special ATK previo al daño.
- Pasivas: Madness Enhancement A (+10% Buster; daño recibido ×2 de la clase), Fae Eyes.
- Lore: es Berserker «obviamente por la rivalidad con Morgan».

## 3. Motor

**Doble economía: Carga NP (FGOCore) + ESTRELLAS CRÍTICAS (★)**, y un keyword defensivo
(**ANTI-PURGA**). Dos keywords propias — presupuesto de complejidad igual a Morgan
(Maldición + Alzarse), §1.3 de la skill.

### ★ Estrellas Críticas (Critical Stars / 暴击星) — power contador propio, tope 10
- **Genera**: pasiva Caster (1ª Habilidad de cada turno → 1★ y NP+3), cartas generadoras
  a tasa Regent (1★ ≈ ½⚡; generadores 1-2★/carta), reliquias, y el power Manipulación
  de la Espada Sagrada (1★/turno).
- **Gasta**: el keyword **CRÍTICO X★** en Ataques — «si tenés ≥X★ **y estás en forma
  Berserker (o Avalon)**: consume X★ y la carta usa su valor crítico». Tasa: 3-3.5
  daño/★ común, ~4/★ poco común, 4.5-6/★ raras (Cometa del Verano = el slot del Comet
  del Regent). Siempre ≤ ½⚡/★ de generación (§3 skill: consumir acumulable).
- **Interrumpe**: tope 10★ (acampar desborda), candado de forma (sin Berserker no hay
  crítico), presión enemiga (defensa forzada genera poco), multi-enemigo (el plan crítico
  es ST; plan B = Anti-Purga/AoE escaso), y caer bajo 100 NP re-arma la ulti.
- **NO reutiliza** las Estrellas del Regent vanilla: power contador propio con icono
  propio (estrella crítica FGO) — evita colisiones en co-op.

### Anti-Purga (Anti-Purge / 对肃正防御) — power contador propio, tope 5
«Los próximos X golpes enemigos que te alcanzarían se **anulan por completo** (a 0, antes
del Bloqueo) y NO puede ser perforada.» Pierde 1 stack por golpe anulado. Cada anulación
cuenta como golpe totalmente bloqueado (alimenta Ojos Feéricos / Contraataque).
- Tasación (ancla Impervious/Buffer): 1 stack ≈ 7-9 Bloqueo vs jefes de golpe grande,
  3-5 vs multi-hit — swingy por diseño. **Todas las cartas de AP puro llevan Exhaust o
  2⚡** (veredicto unánime del panel).
- Implementación: ANTES del Bloqueo y de Intangible (es «no perforable»); los hooks
  ModifyHpLost* son ABSOLUTOS (gotcha GutsPower); registrar la anulación para
  BlockedHitsThisTurn.

### Regla de traducción de skills reales (injerto P2, unánime)
**Skill real de FGO = carta con Exhaust** (el Exhaust ES el cooldown) **con números
1:1 generosos**. Las 6 skills del kit están en el pool poco común con esta regla.

### Loop esperado
Turnos Caster (Habilidades → ★ + NP + Bloqueo/AP) → cruzar a Berserker (Arrebato) →
cobrar Críticos → decidir EN QUÉ FORMA cruzo los 100 (¿muralla o ejecución?) → volver a
Caster antes de que el +2 por golpe duela. **Arquetipos drafteables**: (1) Doble Batería
(NP/Sobrecarga), (2) Lluvia de Estrellas (★/Críticos), (3) La Estrella que Protege
(Anti-Purga + contraataque).

## 4. Formas (FGOCore FormPower/FormSwitch/FormVisuals)

| Forma | Modelo | Pasiva | Decisión que cambia |
|---|---|---|---|
| **Caster** «Niña de la Profecía» (inicial) | 504510 | La 1ª Habilidad que jugás cada turno: +1★ y Carga NP +3 | Jugar Habilidades primero; pegar es desperdicio (no crittea) |
| **Berserker** «Festival de Verano» | 704710 | Ataques +2 de daño; tus cartas pueden activar CRÍTICO; **recibís +2 de daño de cada ataque enemigo** | La ventana de cobro: entrar con banco, reventar, salir antes de pagar de más (anti-parking que escala con la presión) |
| **Artoria Avalon** «Guardiana de la Espada Sagrada» (clímax permanente, `IsPermanent`) | 504520 | Ambas pasivas, sin la penalización | Vía rara Consagración de Avalon (2⚡, Exhaust); la ulti manifestada pasa a ser Around Caliburn: Desatado |

- **Las DOS firmas básicas SON los cambios de forma** (precedente Eruption/Vigilance de
  la Watcher): la danza existe desde el combate 1. El daño de Arrebato se resuelve
  **ANTES** del FormSwitch (no se auto-buffea).
- **Manifestación de ulti por forma** (handler GaugeFilled de Morgan): cruzar 100 en
  Caster/Avalon → Around Caliburn: Desatado; en Berserker → Hope Will Camelot: Desatado.
  Queda fija hasta caer bajo 100. «¿Vestida de qué cruzo los 100?» es la segunda gran
  decisión. (Perilla: si castiga builds, alternativa = elección manual al cruzar.)
- Cambios esperados por combate: 2-4 (firmas + Truco de la Liebre + Escapada).

## 5. Stats y mazo inicial

- **HP máx 70** (Silent como ancla: squishy con las mejores defensas del roster), 3⚡,
  99 oro. Carga NP 0-300, ulti a 100. ★ tope 10.
- Mazo inicial (10): 4× Golpe (1⚡, 6) + 4× Defender (1⚡, 5) +
  **Arrebato de Verano** (1⚡: 6 daño, LUEGO entrás en Berserker. Crítico 2★: 12.
  +: 9 / 16) + **Canción de la Profecía** (1⚡: 4 Bloqueo, entrás en Caster, +1★.
  +: 7 / 2★). El mazo inicial gana el acto 1 sin motor ✓.

## 6. Ultis (especiales generadas, FGOCore ConsumeAllForNpCard)

- **AROUND CALIBURN: Desatado** — Habilidad NP, 0⚡ (mín. 100, consume TODA), Exhaust,
  manifestada en Caster/Avalon: removés tus debuffs; ganás 3 de ANTI-PURGA, 2 de Fuerza
  (solo la primera vez por combate — tope del panel) y 12 de Bloqueo. (Co-op: aliados
  remueven debuffs y +1 AP.) SOBRECARGA: +1 AP por cada 30 sobre 100 (tope 5 = el Count
  real del OC).
- **HOPE WILL CAMELOT: Desatado** — Ataque NP, 0⚡ (mín. 100, consume TODA), Exhaust,
  manifestada en Berserker: 40 de daño a UN enemigo. CRÍTICO 5★: 60. Aplica 1 de Débil.
  Todos los aliados ganan 1 de ANTI-PURGA. SOBRECARGA: +4 por cada 10 sobre 100.
- Las 5 cartas NP escalan +15%/nivel con NpLevels.Scale (dupes).

## 7. Pool de cartas

### Básicas (4)
1. **Golpe** — Ataque 1⚡: 6. +: 9. ×4
2. **Defender** — Habilidad 1⚡: 5 Bloqueo. +: 8. ×4
3. **Arrebato de Verano** (firma) — Ataque 1⚡: 6 de daño; LUEGO entrás en Berserker.
   Crítico 2★: 12. +: 9 / 16.
4. **Canción de la Profecía** (firma) — Habilidad 1⚡: 4 de Bloqueo; entrás en Caster;
   +1★. +: 7 / 2★.

### Comunes (21)
1. Proyección de Caliburn — At 1⚡: 9. +: 12.
2. Tajo de la Espada Sagrada — At 1⚡: 6. Crítico 2★: 13. +: 8 / 17.
3. Golpe de Bastón — At 0⚡: 4; en Caster: NP +5. +: 6 / +8.
4. Embate de Verano — At 2⚡: 15. Crítico 3★: 26. +: 19 / 32. *(13→15: fix del panel)*
5. Lluvia de Familiares — At 1⚡: 3×3 (aleatorio). +: 4×3.
6. Destello Crítico — At 1⚡: 5; +1★. +: 8; 1★.
7. Espada y Canción — At 1⚡: 7; si jugaste una Habilidad este turno: +3. +: 9 / +4.
   *(7+4→7+3: fix del panel)*
8. Reprimenda del Bastón — At 2⚡: 10; 2 de Débil. +: 14; 2.
9. Estocada de la Pradera — At 1⚡: 8; en Berserker: +1★. +: 11.
10. Tiburón de Verano — At 1⚡: 6 a TODOS. +: 9.
11. Plegaria Viajera — Hab 1⚡: 8 Bloqueo. +: 11.
12. Forjar Estrellas — Hab 1⚡: +2★; robás 1. +: 3★.
13. Bendición del Lago — Hab 0⚡: NP +8. +: +14.
14. Escudo de Luz — Hab 1⚡: 5 Bloqueo; +1★. +: 8; 1★.
15. Cántico Feérico — Hab 1⚡: NP +10; +1★. +: +15; 2★.
16. Mirada Feérica — Hab 0⚡: 1 de Débil. +: 1 Débil y +1★.
17. Paso del Peregrinaje — Hab 0⚡: 3 Bloqueo; NP +4. +: 5; +6.
18. Manto del Viaje — Hab 2⚡: 10 Bloqueo; +1★; NP +5. +: 14; 1★; +8. *(12→10: fix)*
19. Hidromancia de Verano — Hab 1⚡: 5 Bloqueo; en Caster: +1★. +: 8.
20. Ensayo del Festival — Hab 1⚡: robás 2; si cambiaste de forma este combate
    (FormShiftedPower): +1★. +: robás 3.
21. «¡Intentémoslo!» (meme) — Hab 0⚡, Exhaust: +1★; NP +5. +: 2★; +5. *(Exhaust: fix)*

### Poco comunes (30)
1. Corte de la Espada de Selección — At 2⚡: 16. Crítico 4★: 30. +: 20 / 38.
2. Espadas Convocadas — At 2⚡: 4×4 (aleatorio); +1★. +: 5×4.
3. Colmillo del Búho — At 1⚡: 12. +: 16.
4. Danza de Verano — At 1⚡: 9; +1★; en Berserker: 2★. +: 12.
5. Juicio de la Estrella — At 1⚡: 8. Crítico 3★: 20. +: 10 / 26.
6. Embestida Temeraria — At 3⚡: 26. +: 33.
7. Contraespada — At 1⚡: 7; +5 por cada golpe enemigo ANULADO por completo este turno.
   +: 9 / +7.
8. Estrella Fugaz — At 0⚡: 3. Crítico 2★: 10. +: 5 / 14.
9. Marea del Festival — At 2⚡: 10 a TODOS. +: 14.
10. Doble Tajo del Verano — At 1⚡: 5×2; en Berserker: +1★ por golpe que dañe HP. +: 7×2.
11. KIT Carisma de la Esperanza B — Hab 2⚡, Exhaust: 2 de Fuerza; NP +30. (Co-op:
    aliados +1 Fuerza y NP +10.) +: 3 Fuerza; +40. *(regla skill→Exhaust 1:1)*
12. KIT Protección del Lago A — Hab 1⚡, Exhaust: NP +25; en Caster: +10 más. +: +35/+15.
13. KIT Espada de Selección EX — Hab 2⚡, Exhaust: 1 ANTI-PURGA; tu próximo Ataque este
    turno +10. +: 1 AP; +15.
14. KIT Pulso de Primavera EX — Hab 2⚡, Exhaust: NP +40; +2★. +: +50; 3★.
15. KIT Hada del Verano B — Hab 1⚡, Exhaust: NP +15; 2 de Bendición de Sobrecarga
    (Co-op: un aliado NP +10). +: +25; 2 Bendición. *(NP 10→15: fix del panel)*
16. Égida Feérica — Hab 1⚡, Exhaust: 1 ANTI-PURGA. +: 1 AP y robás 1. *(Exhaust: fix)*
17. Truco de la Liebre Blanca — Hab 0⚡: cambiá a tu forma opuesta; robás 1. +: robás 2.
18. Escapada de Verano — Hab 0⚡: cambiá de forma; al final del turno volvés a la forma
    anterior. +: al volver, +1★. *(injerto P1: la visita de un turno)*
19. Marea de Estrellas — Hab 1⚡, Exhaust: +3★. +: 4★.
20. Canto del Bastón — Hab 1⚡: robás 2; en Caster: NP +10. +: robás 3.
21. Vacaciones Forzadas — Hab 1⚡, Exhaust: curás 5; +1★. +: 8; 1★.
22. Pose de Detective — Hab 0⚡, Exhaust: +1★ y 3 de Bloqueo. +: 2★ y 4.
23. Despejar la Tormenta — Hab 1⚡: removés tus debuffs; 5 Bloqueo. +: 8.
24. PODER KIT Manipulación de la Espada Sagrada A — Poder 2⚡: al jugarla +1 ANTI-PURGA;
    al inicio de cada turno: +1★. +: coste 1⚡.
25. PODER KIT Creación de Territorio EX — Poder 1⚡: al final de tu turno: 3 de Bloqueo;
    si jugaste 2+ Habilidades: 6. +: 4 / 8.
26. PODER KIT Magia Única B — Poder 1⚡: tus CRÍTICOS hacen +4. +: +6.
27. PODER KIT Locura de Pleno Verano — Poder 1⚡: en Berserker, tus Ataques +2 adicional.
    +: +3.
28. PODER KIT Ojos Feéricos «?» — Poder 1⚡: cada golpe enemigo anulado por completo:
    +1★ y NP +5. +: 1★ y +8.
29. PODER Espíritu del Festival — Poder 1⚡: cada cambio de forma: +1★ y 3 de Bloqueo.
    +: 1★ y 5.
30. PODER Rivalidad de Verano — Poder 1⚡: en Berserker: tus Ataques +2. +: +3.
    *(injerto P2: lore de la rivalidad con Morgan; redundante adrede con Locura — son
    stacks del mismo arquetipo)*

### Raras (21)
1. NP **Hope Will Camelot** — At NP 2⚡ (mín. 70, consume TODA): 32 a UN enemigo; aliados
   +1 ANTI-PURGA. SOBRECARGA: +4/10. +: 40.
2. NP **Around Caliburn** — Hab NP 2⚡ (mín. 70, consume TODA): removés debuffs; 2 AP,
   2 Fuerza, 8 Bloqueo. (Co-op: aliados cleanse y +1 Fuerza.) SOBRECARGA: +1 AP/20
   (tope 5). +: 3 Fuerza; 12 Bloqueo.
3. NP **Estrella de Caliburn** — At NP 1⚡ (mín. 40, consume TODA): 16; +2★.
   SOBRECARGA: +3/10. +: 22.
4. Cometa del Verano — At 0⚡: solo jugable con 5★: consume 5★; 28. +: 34. *(el Comet)*
5. Golpe del Anhelo Heredado — At 2⚡: 14. Crítico 4★: 32. +: 18 / 40.
6. Ejecución de la Guardiana — At 3⚡: 26; vs Élites/Jefes: +12. +: 32 / +15.
7. Avalancha de Espadas Sagradas — At 2⚡: 7×3 ST. Crítico 3★: +4 por golpe. +: 8×3 / +5.
8. Seis Golpes de la Espada — At 2⚡: 3×6 ST. Crítico 2★: 5×6. +: 4×6 / 6×6.
   *(injerto P2 con tope 2★ del panel; los 6 hits del NP real)*
9. Núcleo de Avalon — Hab 1⚡, Exhaust: NP +30. +: +45.
10. Guía del Bastón de Selección — Hab 2⚡: robás 3; +2★. +: robás 4.
11. Velo de Avalon — Hab 2⚡, Exhaust: 3 ANTI-PURGA. +: 4.
12. Lágrimas tras la Sonrisa — Hab 1⚡, Exhaust: curás 8; removés tus debuffs. +: 12.
13. Coro de las Hadas — Hab 1⚡: +1★ por carta jugada este turno (máx. 4). +: máx. 5 y
    robás 1.
14. Plan Secreto del Festival — Hab 1⚡, Exhaust: 3 de Bendición de Sobrecarga. +: 4.
15. Promesa bajo la Tormenta — Hab 2⚡, Exhaust: 2 ANTI-PURGA. +: 2 AP y +2★.
    *(Exhaust: fix del panel)*
16. PODER CLÍMAX **Consagración de Avalon** — Poder 2⚡, Exhaust: entrás en ARTORIA
    AVALON (permanente, ambas pasivas, sin penalización; la ulti pasa a Around Caliburn:
    Desatado). +: coste 1⚡.
17. PODER Bendición de Avalon — Poder 2⚡: al inicio de cada turno: NP +8. +: +12.
18. PODER Dos Caras del Verano — Poder 1⚡: cada cambio de forma: robás 1, +1★, NP +5.
    +: robás 2.
19. PODER Instinto de la Espada — Poder 2⚡: tus CRÍTICOS consumen 1★ menos (mín. 1).
    +: coste 1⚡.
20. PODER Contraataque de la Guardiana — Poder 2⚡: cada golpe enemigo anulado por
    completo: el atacante recibe 4 (máx. 3/turno). +: 6.
21. PODER Recarga de Hechizos (Append 5) — Poder 2⚡: la primera Habilidad de cada turno
    cuesta 1⚡ menos. +: coste 1⚡.

### Especiales (2 cartas + 2 powers contadores)
- Around Caliburn: Desatado / Hope Will Camelot: Desatado (§6).
- Power «Estrellas Críticas» (contador, tope 10, icono estrella FGO) y power
  «Anti-Purga» (contador, tope 5) — ver §3.

## 8. Reliquias

1. **STARTER: El Bastón de Selección** — La primera vez que cambiás de forma en cada
   combate: +2★ y 4 de Bloqueo.
2. **STARTER (BondRelic): Juramento del Peregrinaje** — vínculo estándar; override
   Nv 4/7: empezás con 1★/2★ (en vez de Bloqueo). Capstone Nv 10 «La Estrella de la
   Esperanza»: al inicio de cada combate +1 ANTI-PURGA.
3. **STARTER OCULTA (INpLevelStore): Talismán de la Niña de la Profecía** — dupes/NP
   level 1-5, pity 50%+25%, botón «Invocar (dupe)», +15%/nivel a cartas NP.
4. **JEFE: Espada Sagrada Forjada** (reemplaza starter) — Cada cambio de forma: +2★
   (máx. 1/turno).
5. TIENDA: Flotador de Tiburón Blanco — Empezás cada combate con 3★.
6. PC: Búho Familiar — Al final de tu turno, si no jugaste ningún Ataque: +1★.
7. PC: Diadema de Orejas de Conejo — La primera vez que entrás en Berserker cada
   combate: +1⚡.
8. PC: Amuleto de Resistencia Mágica A — El primer debuff enemigo de cada combate se
   anula.
9. RARA: Cristal de Anti-Purga — Al inicio de combates vs Élite/Jefe: +1 ANTI-PURGA.
10. RARA: Programa del ServantFes — Cada carta NP jugada: +3★.
11. RARA: **Lupa de la Detective del Verano** — Tus Críticos hacen +2 y dan NP +3 al
    consumir las ★. *(injerto P1: cose ★→NP, corrige el sesgo «siempre cruzo en Caster»)*
12. RARA/EVENTO (ILimitBreaker): Cáliz del Mar Interior del Planeta — +15 HP máx;
    Vínculo hasta 12, NP level hasta 6.

## 8.bis Re-baseo al entorno real (2026-06-11)

El usuario juega HextechRunes + BetterCharacters (ver skill §1.bis). Los números de
este diseño son tasa vanilla: quedan vigentes como TASA RELATIVA entre cartas, y el
lift global lo da `ArtoriaBond` heredando `BondRelic.ServantDamageMultiplier`/`Block`
(×1.25 automático, ya implementado en FGOCore). Ajustes de motor al techo nuevo ANTES
del playtest: pasiva Caster 1★+NP3 → **1★+NP5** por primera Habilidad; tope de
Estrellas 10 → **12**; los generadores con Exhaust (Marea de Estrellas, Núcleo de
Avalon, Pulso) +25% en sus montos. El resto se ajusta con las perillas de §11 tras
jugar.

## 9. Justificación de balance (resumen; detalle en artoria_design/proposal_0.json)

Crítico = consumir acumulable con doble candado (★ + forma), tasa ≤ ½⚡/★ siempre;
Berserker paga con +2 recibido/golpe (anti-parking que escala con la presión); Caster no
pega (1ª-Habilidad condicional, tope 10★); Anti-Purga anclada a Impervious con
Exhaust/2⚡ y contraataque capeado 3/turno; NP consume TODO; generadores sobre-tasa con
Exhaust; reliquias de danza capeadas 1/combate o 1/turno (sin loop ⚡-positivo); skills
reales = Exhaust con números 1:1; co-op: defensa escala, ofensa no. Techo Watcher.

## 10. Plan de assets (TODO verificado HTTP 200, 2026-06-11)

| Forma | Modelo (bundle) | Charagraph visual v1 |
|---|---|---|
| Caster | **504510** (asc 1-2; bundle bajado: `assets/reference/bundles/504520.bundle` es el de AVALON — para Caster bajar 504510 si se prefiere la viajera; HOY el plan usa 504520 para Caster-regalia) | `chara_504500b_1.png` (regalia) |
| Berserker | **704710** | `chara_704700a_1.png` (verano + águila) |
| Avalon | **504520** o **704720** (ambas asc 3 son AA — convergen como en el lore) | `chara_704700b_1.png` (diosa radiante) |

- DECISIÓN VISUAL v1: Caster usa el charagraph de REGALIA (504500b@1, modelo 504520) —
  más icónica que la viajera; Avalon usa la radiante del Berserker (704700b@1, modelo
  704720). Bundles ya bajados: 504520, 704710, 704720 (+ texturas en
  `assets/reference/extracted/<id>_anim/`). Export AssetStudio ×3 = paso manual usuario.
- Iconos de skill verificados: Carisma skill_00300, Protección/Hada skill_00601, Espada
  de Selección/Manipulación skill_00305, Pulso skill_00303 (más los que hagan falta del
  catálogo make_morgan_icons.ps1).
- Icono starter de mecánica: **icono de clase Caster DORADO 5★** (Mooncell
  `金卡Caster.png`, patrón Morgan).
- Faces para charui: f_5045000..3 / f_7047000..3.
- Arte de cartas: `match-ce-art.js` (CEs LB6/verano/ServantFes abundan); **dedup contra
  `mapping_morgan.csv` y `mapping_mash`** (gotcha del panel); charagraphs 504500/704700
  para cartas firma; gotcha «Crown Saber Morgan» = hamburguesa.
- Manifest `id: "ArtoriaCaster"` — NUNCA cambiar.

## 11. Orden de implementación y perillas de playtest

**Orden**: FGOCore sin cambios estructurales (todo existe: NpCharge/ConsumeAll/Blessing,
FormPower/Switch/Visuals/FormShifted, BondRelic, INpLevelStore, BlockedHits) → powers
contadores (Estrellas, Anti-Purga) + 3 form powers → personaje/MainFile → cartas (fan-out
por rareza) → reliquias → loc ×3 (CORRER `tools/audit_simpleloc.ps1` ANTES de publicar —
ojo con los «+1★», «+2», «(máx. 3/turno)»: ¡escapar `/+` y `/(`!) → assets → publish.

**Perillas** (en orden de probabilidad de uso):
1. Caster-camping: bajar Búho a 1★/2 turnos, o pasiva Caster requiere no haber jugado
   Ataques (variante P2: por bloqueo-completo).
2. Anti-Purga vs jefes de golpe único: AC Desatado 3→2 AP base; o AP no anula golpes ≤6.
3. Contraataque+Velo vs multi-ataque: el tope 3/turno es la perilla.
4. Instinto de la Espada: subir a «mín. 2» o 3⚡ si los mazos ★ ganan por inercia.
5. Manifestación por forma castiga builds: cambiar a elección manual al cruzar 100.
6. Crítico ilegible en playtest: imprimir valores por carta (ya hecho) y/o subir tope ★.
7. Auditar que ningún ciclo de cambio devuelva >1⚡ (Recarga+Liebre+powers de cambio).
