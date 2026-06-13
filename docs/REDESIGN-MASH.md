# Rediseño desde 0 — Mash (Fase 1, validación del NP nuevo)

Mash "está bien" (feedback del usuario): identidad muralla/Baluarte/Intercepción/Formas
se mantiene. El rediseño de Mash es LIVIANO y sirve para **validar el modelo de NP nuevo**
con bajo riesgo antes de Morgan/Artoria. Baseline: ecosistema sin multiplicador global
(skill §1.bis corregida); ver [[ecosystem-power-baseline]] y [[redesign-feedback-2026-06]].

## A. Modelo de NP nuevo (COMPARTIDO, FGOCore) — el cambio grande

**Problema (feedback #3/#4):** todos colapsaban en "cargar NP → ulti", y la ulti
auto-generada a 100 (cartas *Unleashed*, 0⚡, Retain+Exhaust) eclipsaba a las cartas NP
drafteadas del mazo (LordCamelot, etc.).

**Modelo nuevo (inspirado en Phrolova "Performing" + Acheron + Kafka):**
- La carga sube **jugando cartas que aplican el recurso propio** (Mash: bloquear / perder
  vida vía el starter; cartas con `+NP`). Eso ya pasa — se mantiene. "Subir NP" = "jugar
  bien el mazo".
- **A 100 NP NO se genera una carta gratis.** En su lugar se abre una **VENTANA de 1 turno**
  (un power que expira al fin de tu turno) que (a) **devuelve recursos** (+1⚡, robar 1) y
  (b) **potencia las cartas del mazo** ese turno (efecto por personaje). El medidor NO se
  consume al abrir la ventana.
- **Las cartas NP drafteadas** (LordCamelot/LordChaldeas/Rhongomyniad/BlackBarrel) son el
  CLÍMAX que elegís jugar DENTRO de la ventana — consumen el medidor (ConsumeAllForNpCard,
  ya existe). Ahora son EL payoff, no algo eclipsado por una carta gratis.
- La ventana dispara una sola vez por "pico" (marker tipo `CamelotManifestedPower`, ya
  existe); se re-arma al bajar el medidor < 100 (gastar una NP). Loop: cargar→ventana→
  soltar NP elegida→re-cargar.

**Migración (additive, no romper a los otros):** el handler de `GaugeFilled` es por
personaje (`creature.Player?.Character is Character.MashShielder`). Migro SOLO a Mash al
modelo de ventana; Morgan/Artoria quedan en el auto-manifest viejo hasta su fase. Cuando
los tres estén migrados, se retira el path viejo.

## B. La ventana de Mash: "Baluarte Absoluto" (Lord Camelot desplegado)

Power `AbsoluteBulwarkWindowPower` (Single, expira `AfterPlayerTurnEnd`), aplicado al cruzar 100:
- Al abrir: **+1⚡ y robar 1** (arranca el turno grande, no lo reemplaza).
- Mientras activa (este turno): **todo el Bloqueo que ganás se inflige además como daño a
  TODOS los enemigos** (la muralla se vuelve arma — "el Bloqueo se refleja como daño").
  Esto hace que sus cartas DEFENSIVAS del mazo se vuelvan ofensivas por un turno → el mazo
  importa MÁS durante el NP, no menos.
- Hook a verificar en BaseLib antes de codear: la forma correcta de enganchar "ganaste
  Bloqueo" (¿`AfterGainBlock`/override en `ModifyBlock`/comando?). Modelar sobre el patrón
  ya-funcional de `InterceptPower` (que refleja golpes bloqueados como contraataque) y
  `MobileWallPower` (Bloqueo→momentum). NO adivinar la API: leer esos dos powers primero.
- Icono: `skill_00419` (escudo/fortaleza, ya cacheado) → `absolute_bulwark_window_power.png`
  (+ big). Loc eng/esp/zhs.

## C. Cambios de código (checklist para implementar)

1. **Mash `MainFile.cs`**: `TryManifestUlt` → `TryOpenNpWindow`: aplicar
   `CamelotManifestedPower` (marker, como ahora) + `AbsoluteBulwarkWindowPower` + `GainEnergy 1`
   + `Draw 1`. ELIMINAR la generación de las cartas *Unleashed* (`CreateCard<...Unleashed>`).
2. **Nuevo** `Powers/AbsoluteBulwarkWindowPower.cs` (ver §B) + su loc + icono.
3. **Retirar las 3 cartas Unleashed** (`Cards/Special/*Unleashed.cs`): ya no se generan.
   Dejar las clases (son solo generadas, no están en el pool de draft) para no romper nada;
   o borrarlas (el usuario acepta romper saves). Decisión: dejarlas inertes (mínimo churn).
   `BehindMeSenpai` (cover multiplayer) NO se toca.
4. **Recalibración de números**: las cartas de Mash se balancearon en v2 ANTES del ×1.4 de
   v3, así que quitarlo las devuelve a su balance v2 (razonable). Pasada de control vs
   ecosistema: básicas exactas (Strike 6/Defend 5 ✓), comunes 9-10 dmg/⚡ o 7-9 blq/⚡,
   picos raros condicionados. Las NP drafteadas son el techo (gateadas por carga). Ajustar
   solo lo que se salga; NO inflar.
5. **Starter `RoundTableFragment`**: ya es motor evento→recurso con cap 3/turno. Se mantiene.
6. Build → (juego cerrado) publish los 4 juntos (cambia FGOCore si toco el modelo NP ahí;
   si la ventana vive 100% en Mash, igual conviene publicar juntos por las firmas).
7. `audit_simpleloc` antes de publicar.

## D. Lo que NO cambia en Mash
Formas (Shielder/Ortinax/Paladín), Baluarte/retención, Intercepción, Estrellas de crítico
+ CombatAnalysis (motor secundario), el grueso del pool. Es recalibración + el NP nuevo,
no un teardown — porque "Mash está bien".
