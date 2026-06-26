# DESIGN-REVIEW-3 — expansión de sistemas FGO (visión del usuario, 2026-06-26)

Roadmap de 6 features pedidas. Continúa [DESIGN-REVIEW.md](DESIGN-REVIEW.md) (NP-anchor) y
[DESIGN-REVIEW-2.md](DESIGN-REVIEW-2.md). **Regla**: cada NP/tipo se modela según el **juego original**
(tipo de carta de la NP, efectos de los 礼装), investigando en fuentes JP + zhs (cf. CLAUDE.md).

## ESTADO (2026-06-26): TODO IMPLEMENTADO + COMPILADO VERDE
Las 6 features están hechas (FGOCore + 9 personajes compilan verde) en 2 etapas:
- **Etapa 1** (commit `254208b`): **A** carta de ulti a 100 en los 9 (los 3 públicos revertidos antes en `6af3df9`) · **B** cap por dupes (`NpCharge.Max(creature)` dinámico) · **C** sistema de tipos `CommandType`+`ICommandTyped`+`CommandBonusPower` (Buster→Fuerza temp/perm, Quick→★, Arts ulti→NP) · **D** las 9 ultis tipadas (Mash/Castoria=Arts, Okita=Quick, resto=Buster).
- **Etapa 2** (commit `f45f6c3`): **E** 8 CEs colorless en `FGOCore/Memes/` (Kaleidoscope, Black Grail, 2030, Prisma Cosmos, Imaginary Element, Heaven's Feel, Formal Craft, Zero Over) drafteables por todos · **F** consolación de dupe en `NpLevels.TryRollDupeWithConsolation` (pity bajo→oro, medio→upgrade/encantar, alto→elegir carta) en los 7 relics-store · ulti de Siegfried (`BalmungUnleashed`).
- Investigación verificada (NP types JP+zhs, CEs, reward APIs, cap/dupe) en los workflows `fgo-roadmap-research`.

**Decisiones tomadas:** Morgan queda `QueensSentenceUnleashed` (publicada) pero **tipo Buster** (su NP real es Roadless Camelot). Montos de bonus por tipo y de los CEs = primera pasada, tuneables.

**FLAGS PARA PLAYTEST (no bloquean, compila verde):**
1. Consolación pity-alto: abre una pantalla de **elección de carta anidada** dentro de la recompensa → riesgo de UX/sync en multi *(a confirmar jugando)*. Si glitchea, degradar a oro/upgrade.
2. `MapoTofu` (opción de pity-alto) **hace daño** al jugarse — raro para consuelo; candidato a swap.
3. Balance sin playtest: bonus por tipo, números de CEs, fórmula del cap.

**PENDIENTE:** arte de las 8 CEs (con el arte del 礼装 real vía match-ce-art) + ultis + powers (caen al placeholder).

## A. Carta de ulti a 100 NP + escala con NP — ✅ HECHO (falta verificar consistencia)
El usuario prefiere el diseño viejo: cruzar 100 **manifiesta la carta de Noble Phantasm** (no una
ventana), y **más NP la potencia** (Sobrecarga). Estado: los 5 nuevos ya lo hacían; revertí los 3
públicos (Mash=`LordCamelotUnleashed`; Morgan=`QueensSentenceUnleashed` nueva; Artoria=
`AroundCaliburnUnleashed` nueva) — compilan. **Pendiente**: confirmar que los 8 escalan con Sobrecarga
de forma comparable; las cartas consumen el medidor (`ConsumeAllForNpCard`).

## B. BUG: el medidor llega a 300 sin dupes — debe gatearse por dupes (como el juego)
Hoy `NpCharge.Gain` capea en **300 fijo**; `NpLevels.NpLevel` (sube por dupes, con `DupePity` + Grail
vía `ILimitBreaker.ExtraNpLevels`) solo afecta el poder de la carta, NO el techo. → sin dupes igual
llegás a 300.
**Fix propuesto**: `NpCharge.Max(player) = clamp(100 × NpLevel(player), 100, 300)`. Sin dupes (NpLevel 1)
= **100** (sin Sobrecarga); 1 dupe (NpLevel 2) = 200; 2 dupes (NpLevel 3) = 300. El Grail puede subir
`MaxLevel` y por ende el techo. **DECISIÓN ABIERTA**: ¿esa fórmula 100/200/300, o NpLevel 1 permite algo
de Sobrecarga? ¿El Grail sube el techo arriba de 300 o solo el poder?

## C. Bonus por tipo de carta de comando (refina el starter QAABB)
- **Buster** normal → **+Ataque TEMPORAL** (1 turno). **Buster ulti (burst)** → **+Ataque PERMANENTE**.
- **Quick** normal → **★ de crítico**. **Quick ulti** → **más ★**.
- **Arts** normal → NP (ya lo dan). **Arts ulti** → **también dar NP** (falta).
**Propuesta de montos** (a tunear): Buster +1 temp / ulti +2 perm; Quick +2★ / ulti +5★; Arts ulti +X NP.
Vive en el sistema de cartas de comando (FGOCore) + lo aplican las cartas de ulti según su tipo (→ D).

## D. Cada ulti referencia su TIPO según el juego original (Buster/Arts/Quick)
Cada carta de Noble Phantasm se etiqueta con el tipo de su NP en FGO y recibe el bonus de C.
**A investigar (JP+zhs) y confirmar**: Mash (Lord Camelot = Arts?), Morgan (Buster?), Artoria Caster
(Around Caliburn = Arts?), Mordred (Clarent Blood Arthur = Buster), Gilgamesh (Enuma Elish = Buster),
Okita (Mumyou Sandanzuki = Quick), Oberon (= Buster?), Siegfried (Balmung = Buster). *(todos a confirmar)*

## E. Pool GENERAL de cartas Craft Essence (礼装), compartida por todos los Servants
Cartas que NO pertenecen a un personaje, en una pool colorless común (extender el subsistema **`Memes`**
de FGOCore, que ya hace cartas colorless FGO). Son **礼装 del juego original, sobre todo 5★**, con efectos
según el juego. **A definir**: lista de CEs (candidatas icónicas 5★: Kaleidoscope = +NP de arranque,
Black Grail = +daño NP −HP, The Imaginary Element/Formal Craft = +daño Arts, Heaven's Feel, 2030/Aerial
Drive = +★, Limited/Zero Over, Golden Sumo, etc.) + sus efectos + cuántas. Arte = el del CE (pipeline
match-ce-art ya existe). **DECISIÓN ABIERTA**: ¿cuántas y cuáles para empezar?

## F. Consolación de dupe estilo mod Miyabi (cuando NO sale dupe)
Hoy `NpLevels.TryRollDupe` falla silencioso (solo sube `DupePity`). Como en el mod **Miyabi**, cuando no
sale dupe dar una recompensa: **oro, carta a elección, mejora de carta, encantamiento**, etc.
**A investigar**: el mod Miyabi instalado (`G:\SteamLibrary\...\workshop\content\2868840\` o el repo) para
ver cómo lo ofrece. **A definir**: el menú de consolación + cuándo se ofrece (¿al fin de combate? ¿en el
roll de dupe?).

## Plan por etapas (propuesto)
1. **B** (bug del cap por dupes) — fix acotado + decisión de fórmula. Foundational.
2. **C + D** (bonus por tipo + tipar las ultis) — coherente; refina las cartas de ulti recién revertidas. Mediano.
3. **E** (pool de CEs 5★) — contenido grande; empezar con ~6-10 CEs icónicas.
4. **F** (consolación Miyabi) — requiere estudiar el mod Miyabi; mediano-grande.

**No publico** los 3 públicos todavía: bundleo la carta de ulti + su tipado (C/D) + VRAM-512 en UNA
actualización coherente, para no churnear el Workshop dos veces.
