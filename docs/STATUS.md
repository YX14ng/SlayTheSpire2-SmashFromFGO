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

## Pendiente (orden)
1. **Re-publicar TODO a Workshop junto**: webp patch (VRAM) + NP fixes + manifests (formato nuevo) + Tiamat + los servants que faltan subir (Mordred/Gilgamesh/Okita/Oberon/Siegfried). Requiere **staging** (ver DECISIONS §deploy).
2. **Mod de optimización de VRAM** (lazy character loading) — DESPUÉS de Tiamat.
3. Telemetría RitsuLib (futuro).

## Bloqueado / a decidir
- Cómo hacer Tiamat jugable para playtest: **local-rápido vs re-publish a Workshop** (pendiente decisión del user).
- NP fixes que requieren decisión del user: Okita romaji vs EN oficial; Artoria "Hopewill"/"Round of Avalon". (Siegfried `失坐`→`失坠` es fix claro, aplicar.)

## Regla de mantenimiento
Antes de cerrar una sesión, actualizar **STATUS / next-task / HANDOFF**; no dejar que el código quede adelante de los docs. "Instalado" ≠ "validado en juego": leer el `godot.log` y probar.
