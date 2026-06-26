# DECISIONS — reglas ya definidas (no re-discutir)

Solo decisiones **cerradas** + caminos abandonados, para no volver a girar sobre lo mismo.
Patrón tomado de `iryuko/sts2-mod-dev`. Estado vivo → [STATUS.md](STATUS.md). Evidencia → [FINDINGS.md](FINDINGS.md).

## Versión / plataforma
- Target: rama **MAIN pública** del juego, **v0.107.1**. BaseLib **3.3.0** (pin EXACTO en cada csproj; debe coincidir con el de `mods/` del juego o el mod no carga).
- **MegaDot 4.5.1** para exportar el `.pck` (el juego no carga un `.pck` de un Godot más nuevo).

## Deploy
- **Workshop-only**: los mods FGO no viven en `mods/` del juego; se cargan desde suscripciones de Workshop.
- **Staging (separación workspace/juego, estilo `iryuko/sts2-mod-dev`)**: build/publish van SIEMPRE a `dist/<ModId>/` del repo (gitignoreado), NUNCA a la carpeta del juego. La referencia a FGOCore sale de `dist/` (`$(StagingPath)`) → el build no depende de tener el juego ni FGOCore instalado. Instalar al juego solo vía `tools/install-mod.ps1 -Mod <Id>` / `-All`; `-Clean` restaura Workshop-only. Atajo sin script: `/p:DeployToGame=true`. (Implementado en `Sts2PathDiscovery.props`: `StagingPath`/`DeployDir`/`DeployToGame`.)
- **FGOCore.dll no puede estar local Y en Workshop a la vez** (mismo id → duplicado → crash). El build local de FGOCore es **temporal** y se borra al terminar.
- Cuando cambia la API pública de FGOCore, **los 10 mods se republican JUNTOS** (dll viejo contra FGOCore nuevo → `MissingMethodException`/`ReflectionTypeLoadException`, falla silenciosa). Nunca shippear FGOCore solo.
- Descripciones **localizadas** de Workshop: solo por web UI o Steamworks API (SteamCMD/VDF setea una sola, la default).
- **Webp patch ELIMINADO (2026-06-25)**: ya no se capean las texturas de animación con `size_limit` (`tools/patch_webp_imports.ps1` borrado; sin más `publish→patch→publish`). El VRAM se maneja por el mod de optimización (lazy character loading). Los `.import` ya horneados quedan capeados (con el scale del `.tscn` ya compensado); revertir a full-res es un cambio aparte (revertir 3891 `.import` + ajustar scales) que **sube** VRAM — no hacer hasta tener lazy-loading.
- Manifest dependencies en formato nuevo: `[{"id":"BaseLib","min_version":"v3.3.0"}, {"id":"FGOCore","min_version":"v0.1.0"}]`.

## Diseño / balance
- **Nada de multiplicador global ×daño en el starter** (lección del Bond ×1.4 = "demasiado roto"). Escalar en motores FGOCore que sobreviven al strip de buffs.
- Techo de saturación ~**180-220 daño/turno**; multi-hit anti-Buffer; no depender de debuffs (los jefes los strippean).
- **Lore**: investigar SIEMPRE en japonés (baseline de diseño) **+** chino simplificado para corroborar. Frases de voz ORIGINALES (no transcripción del juego). NP names = canónico JP → ZH/EN.

## Técnico (cerrado)
- `CardRarity.Special` **no existe** en el enum del juego → usar `CardRarity.Event` para cartas manifestadas / no-drafteables (quedan fuera de recompensas).
- Las cartas se **auto-registran** por `[Pool(typeof(<X>Pool))]` en la clase base; no hay lista manual que editar.
- IDs de mod / model / power **NUNCA** se renombran con saves activos (el prefijo del mod es parte del ID; migrar una mecánica entre mods rompe runs en progreso).
- `PowerVar<T>` siempre con nombre explícito; `ModifyHpLost*` son ABSOLUTOS; validar rutas de VFX contra `grep '"vfx/' decompiled/`.

## Caminos abandonados (no volver a girar)
- No mezclar mods locales + Workshop de los mismos ids.
- No tratar "instalado" como "validado en juego".
- No usar RitsuLib para reestructurar el código (coexiste con BaseLib, pero su valor para nosotros es la **telemetría**, que es futuro).
- El install cracked `F:\Games\...v0.103.3` quedó **descartado**: solo se usa el Steam legítimo (v0.107.1).
