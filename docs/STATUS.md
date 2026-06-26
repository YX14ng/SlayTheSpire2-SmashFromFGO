# STATUS — estado actual (alta densidad)

Fecha: **2026-06-25**. Este archivo es la **fuente de verdad del estado**; reemplaza la
sección de estado de [HANDOFF.md](HANDOFF.md) (quedó vieja: hablaba de v0.103.x / BaseLib 3.2.1).
Reglas de decisión cerradas → [DECISIONS.md](DECISIONS.md). Hallazgos técnicos → [FINDINGS.md](FINDINGS.md).
El usuario se comunica en **español**.

## Línea principal actual
- El juego (Steam público) saltó a **v0.107.1 / BaseLib 3.3.0**. Los **10 mods FGO ya están portados** y compilan verde.
- **Deploy = Steam Workshop** (appid `2868840`). La carpeta `mods/` del juego **NO** lleva mods FGO locales.
- **Tiamat** (rediseño dos-pozas) **implementado + arte completo + committeado**. Falta publicarlo.

## Confirmado
- FGOCore + Mash + Morgan + Artoria: en Workshop como **4 items SEPARADOS, privados**. IDs en `tools/.workshop_id_*.txt`.
- BaseLib (dependencia) = Workshop ID `3737335127`.
- v0.107.1 carga nuestros mods limpio (log: "Finished mod initialization").
- Tiamat: 27 cartas (código verde) + arte (match-ce-art) + loc eng/esp/zhs. Commits `115c8fe` (código) + `b86d618` (arte).
- Los crashes recientes del juego son de mods de **otros autores** (ver FINDINGS §mods rotos), no nuestros.

## Hecho recientemente
- **Separación workspace/juego (staging)** implementada y verificada: build/publish → `dist/<ModId>/`; `tools/install-mod.ps1` instala al juego; FGOCore se referencia desde `dist/`. Ver DECISIONS §deploy.
- **Revisión de diseño vs Togawa** ([DESIGN-REVIEW.md](DESIGN-REVIEW.md)) + **fixes implementados, COMPILADOS verde y PUBLICADOS**: Morgan→Maldición (deja de ser clon de Castoria); Castoria re-arma su ventana; Okita NP↔Aliento; Gil Enuma consume Armas; Mordred Crítico manifiesta token; Oberon NP↔Deuda; Siegfried sink «Erupción de Escamas». REDESIGN-MORGAN.md reconciliado.
- **Republish a Workshop COMPLETO** (2026-06-25): los 10 mods FGO en Workshop, **PRIVADOS**, sin webp patch, desde `dist/`. IDs: FGOCore `3747876334` · Mash `3747876464` · Morgan `3747876731` · Artoria `3747876956` · Mordred `3751610432` · Gilgamesh `3751610575` · Okita `3751610728` · Oberon `3751610867` · Siegfried `3751611015` · Tiamat `3751611145`. **Falta**: suscribir los 6 nuevos + playtest + decidir hacerlos públicos.

- **DESIGN-REVIEW-2 (2da pasada) implementada + COMPILADA verde** (8 mods, [DESIGN-REVIEW-2.md](DESIGN-REVIEW-2.md)): **Gil** = motor de Armas (cartas generadoras + contador central `ArmsPlayedPower.AfterCardPlayed`) + módulo Tesoro (fallback del Oro, patrón `DebtPower`) + starter QAABB + reliquia Bab-ilu; **Tiamat** = **SkillSeal REAL** (cancela la habilidad enemiga vía `CreatureCmd.Stun`, patrón Sleep de Oberon) + pool Lily 15→**27** + loop ofensivo (Marea Voraz: el enjambre muerde al fin de tu turno); **Mordred** = starter QAABB + cap a Saberface (100★) + 4 riders bi-condicionales; **Siegfried** = pool 24→**32** + BalmungSwing lee SdD + payoffs de ★; pulido **Morgan/Castoria/Mash/Okita**. **Falta**: arte de las ~41 cartas nuevas (caen al placeholder `card.png` hoy) + publish/install + playtest de balance.
- **Bug de ojos de Castoria Berserker** arreglado (commit `9b78781`): el idle re-ventaneado `[150-154,0-4]` es solo la "subida" (no loopea → cabeza/ojos saltan en la costura 009→000); convertido a **ping-pong** en `artoria_frames_berserker.tres` reusando los frames (sin re-render). Entra con la próxima publicación de Artoria.

## Playtest watch-list (ya compila verde)
Los fixes **compilan** a `dist/`. Falta **playtest** (balance) + **publish (.pck) + install** a G:. Puntos a vigilar EN JUEGO (los riesgos de compilación ya se resolvieron):
- **Morgan**: `CreatureCmd.Damage` 6-arg en MainFile (verificado = calca CursePower ✓); `BeforeCardPlayed`/`cardPlay.Target` (degrada elegante si el target no está resuelto); cap de Maldición = 25 (no se tocó FGOCore).
- **Gil**: escaneo de mano `PlayerCombatState` (verificado canónico ✓); `KingsArrogancePower` rompe por Bloqueo remanente al fin de turno; Bab-ilu no existe → Arrogancia colgada de `OathOfUruk`.
- **Siegfried**: `DamageVar(0)` de `ScaleEruption` — chequear que no muestre "0 daño" en tooltip.
- **Todos**: correr `tools/audit_simpleloc.ps1`; balance sin playtest (Sentencia total de Morgan, +golpes de Okita, +1/Arma de Gil son perillas).

## Pendiente (orden)
1. **Re-publicar TODO a Workshop junto**: webp patch (VRAM) + NP fixes + manifests (formato nuevo) + Tiamat + los servants que faltan subir (Mordred/Gilgamesh/Okita/Oberon/Siegfried). Ahora usa el **staging** (`dist/` → install-mod / upload).
2. **Mod de optimización de VRAM** (lazy character loading) — DESPUÉS de Tiamat.
3. Telemetría RitsuLib (futuro).

## Bloqueado / a decidir
- ✅ **Resuelto (2026-06-25)**: el juego no estaba desinstalado — se **movió de biblioteca Steam a `G:\SteamLibrary\steamapps\common\Slay the Spire 2`** (el viejo C: quedó con restos). `Sts2Path`→G: en los Directory.Build.props. **Build verificado end-to-end**: FGOCore + los 7 personajes con fixes + Tiamat compilan VERDE → `dist/` (solo faltaba 1 `using` en Gil, arreglado). Falta: **playtest** (balance) + **publish/install**.
- Cómo hacer Tiamat jugable para playtest: **local-rápido vs re-publish a Workshop** (pendiente decisión del user).
- NP fixes que requieren decisión del user: Okita romaji vs EN oficial; Artoria "Hopewill"/"Round of Avalon". (Siegfried `失坐`→`失坠` es fix claro, aplicar.)

## Regla de mantenimiento
Antes de cerrar una sesión, actualizar **STATUS / next-task / HANDOFF**; no dejar que el código quede adelante de los docs. "Instalado" ≠ "validado en juego": leer el `godot.log` y probar.
