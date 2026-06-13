# Rediseño desde 0 — Morgan (motor de Maldición ACTIVO + formas que se alimentan)

Fixea el feedback ([[redesign-feedback-2026-06]]): (1) Maldición+Estrellas se sentía
PASIVO, (2) las formas eran "dos personajes desconectados", (3) la ulti a 100 NP gratis
eclipsaba al mazo. Inspiración mecánica del ecosistema (Ryoshu Poise breakpoint+halving,
MzmChar `Alone` stat-conversion cruzada, MeiLin posturas que cambian decisiones).

## A. Identidad nueva en una frase
**La reina hada que SIEMBRA maldición en Caster y la COSECHA en Berserker.** Un solo
motor de dos tiempos: nunca querés acampar una forma porque media máquina se apaga.

## B. El motor de dos tiempos (resuelve "pasivo" + "formas desconectadas")

Patrón MzmChar `Alone` (una forma genera lo que la otra consume) + Ryoshu (breakpoint con
gasto que halva). DROP de las Estrellas de crítico como motor (feedback): Morgan queda con
DOS recursos — **Maldición** (motor activo primario) y **Carga NP** (clímax compartido).

- **Bruja de la Lluvia (Caster) = GENERADOR (sembrar).** Pasiva nueva: tus cartas de
  Maldición aplican +2 extra (mantiene el amp) **y la Maldición que aplicás NO decae este
  turno** (preservación base de Caster — antes era la rara Cernunnos; pasa a ser identidad
  de la forma). Ataques -2 daño (sigue siendo floja en combate cuerpo a cuerpo). Es donde
  apilás la bomba. (+NP/turno se BAJA o se mueve, ver §E — no debe ser la línea óptima.)
- **Hada de la Reina (Berserker) = DETONADOR (cosechar).** Pasiva nueva **"Sentencia"**:
  tus Ataques infligen daño adicional igual a la Maldición del objetivo y **detonan** (la
  consumen). Es donde convertís la maldición en daño. Genera poca maldición propia.
- **El loop = la decisión:** turno(s) en Caster para apilar (sin decaer) → cambiá a
  Berserker → detoná con un Ataque para un golpe enorme. Cada turno: ¿sigo sembrando o ya
  cosecho? ¿qué enemigo detono? El cambio de forma ES la jugada (premiado por
  `SovereignOfTwoFaces`: cambio → roba+NP; mantener/reforzar). Acampar Caster = no hacés
  daño; acampar Berserker = te quedás sin maldición que detonar.
- **Reina del Invierno (permanente) = la válvula "ambas a la vez"** (lección GuiYi de
  MeiLin): genera Y detona sin penalidad. Es la META aspiracional del mazo (clímax), no
  el modo cómodo inicial. Se mantiene permanente.

### Números de arranque (knobs de playtest)
- Cap de Maldición por enemigo: **20** (sube de 15; necesitás espacio para la bomba).
- Sentencia (Berserker): Ataque a objetivo maldito → +daño = Maldición del objetivo,
  luego detona (la consume). Multi-hit detona en el PRIMER golpe que conecta.
  - Alternativa si es muy swingy: detona la MITAD y +daño = mitad (estilo Ryoshu halving).
    Decidir en playtest. Arrancar con detonación total (más legible).
- Caster sin-decay: la maldición aplicada ESTE turno no decae al activarse (el decay normal
  de FGOCore reduce 1 por golpe). Fuera de Caster, decae normal salvo Cernunnos.

## C. Las básicas de comando (mantener B/A/Q con arte original, re-temáticas a Maldición)
- **Buster** (`BusterMorgan`): daño crudo (10) — el detonador básico.
- **Arts** (`ArtsMorgan`): daño + Carga NP (motor del clímax).
- **Quick** (`QuickMorgan`): AHORA aplica **Maldición** en vez de Estrellas (Quick feérico =
  golpes que hexan). Re-skin del sistema FGO a la identidad curse; conserva arte+estructura.

## D. NP-window (igual que Mash, modelo aprobado)
A 100 NP **NO** se genera ulti gratis (sacar `TryManifestUlt`/las `*Unleashed`). Abre la
ventana **"Sentencia de la Reina"** 1 turno: tus detonaciones de Maldición se duplican (o
detonás a TODOS los enemigos malditos de una), y devuelve recursos (+1⚡, robar 1). Las
cartas NP drafteadas (RoadlessCamelot/RhongomyniadRain/MemoryOfLondinium) son el clímax que
elegís jugar dentro de la ventana. Migración additive (Artoria queda en el viejo hasta su fase).

## E. Estrellas FUERA — qué pasa con las cartas que las tocaban
Drop del motor de Estrellas (feedback "pasivo"). Re-tematizar:
- `QuickMorgan`: estrellas → **Maldición** (§C).
- `ReplicaLance`, `BaobhanSithsShriek`: estrellas → **Maldición** o NP (elegir por carta).
- `MistVeil` (NP→★) y `Vassalage` (★→NP): el par espejo pierde sentido sin estrellas →
  reconvertir a un par espejo **NP↔Maldición** (gastar NP para sembrar maldición masiva /
  gastar maldición acumulada para NP), que SÍ alimenta el motor nuevo.
- `QueensScepter` (starter): perder HP → +10 Estrellas pasa a **perder HP → aplicar
  Maldición** (a un enemigo aleatorio o a todos, cap 3 procs/turno) — el daño propio de
  Morgan (MadLunge/TyrantsBlood/FaeBloodPact) siembra la bomba. Sigue siendo motor
  evento→recurso del patrón ecosistema. `MadnessEnhancement` (HP loss → NP) se mantiene.
- Quitar `CritStarsPower`/referencias de Morgan (queda en FGOCore para Mash/Artoria).

## F. Recalibración de pool
Quitar el ×global ya pasó (fundación). Pasada de control: básicas exactas, comunes como
engranajes de conversión (ahora Maldición↔NP↔daño), detonadores (QueensScorn, RoyalPunishment,
TyrantsSweep, FinalCollection) recalibrados a que el techo lo dé la Sentencia gateada, no
números planos. `CurseOfCernunnos` (preservación) se vuelve redundante con la Caster sin-decay
→ reconvertir esa rara a otra cosa (ej. "la Sentencia no consume la maldición, solo la mitad"
como upgrade del detonador), o a un amplificador de detonación.

## G. Checklist de implementación (próximas sesiones)
1. **Forma Caster** (`RainWitchFormPower`): + no-decay de maldición aplicada; bajar/mover el +NP/turno.
2. **Forma Berserker** (`FairyQueenFormPower`): pasiva Sentencia (Ataque detona Maldición del objetivo por +daño). Hook: `ModifyDamageAdditive` (lee `target` Curse) + consumir en `AfterAttack`/`AfterDamageReceived`. Leer `InterceptPower`/`WinterThornsPower` para el patrón de leer/aplicar Curse al objetivo.
3. **CursePower**: subir cap a 20 (FGOCore `CursePower.MaxPerEnemy`).
4. **MainFile**: `TryManifestUlt` → `TryOpenNpWindow` (ventana "Sentencia de la Reina" + recursos), sacar generación de `*Unleashed`.
5. **Nuevo power** `QueensSentenceWindowPower` (ventana 1 turno) + loc + icono.
6. **Quick/mirror/star cards** re-tematizados (§C, §E) + loc.
7. **QueensScepter**: HP loss → Maldición.
8. Quitar referencias a CritStars en Morgan; recalibrar pool (§F).
9. Build → audit_simpleloc → publish (los 4 juntos si toco FGOCore CursePower) → commit.

## H. Lo que se mantiene
Las 3 formas (identidad FGO), `SovereignOfTwoFaces` (premia cambiar), `MadnessEnhancement`
(HP→NP), Guts/Alzarse (FromTheWorldsEnd/LastResort), el grueso temático del pool.
