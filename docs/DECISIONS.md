# DECISIONS — reglas ya definidas (no re-discutir)

Solo decisiones **cerradas** + caminos abandonados, para no volver a girar sobre lo mismo.
Patrón tomado de `iryuko/sts2-mod-dev`. Estado vivo → [STATUS.md](STATUS.md). Evidencia → [FINDINGS.md](FINDINGS.md).

## Versión / plataforma
- Target dual con un único artefacto de Workshop: **MAIN v0.107.1 + BETA v0.109.0**. Baseline de compilación **BaseLib 3.3.6**; manifiestos exigen `>= v3.3.6`; runtime verificado con Workshop **3.3.7**.
- **MegaDot 4.5.1** para exportar el `.pck` (el juego no carga un `.pck` de un Godot más nuevo).

## Deploy
- **Workshop-only**: los mods FGO no viven en `mods/` del juego; se cargan desde suscripciones de Workshop.
- **Visibilidad pública**: FGOCore y todos los mods de personajes FGO presentes o futuros se publican como items públicos. Esta decisión no autoriza uploads automáticos: Steam/SteamCMD solo se ejecutan ante un pedido explícito del usuario.
- **Staging (separación workspace/juego, estilo `iryuko/sts2-mod-dev`)**: build/publish van SIEMPRE a `dist/<ModId>/` del repo (gitignoreado), NUNCA a la carpeta del juego. La referencia a FGOCore sale de `dist/` (`$(StagingPath)`) → el build no depende de tener el juego ni FGOCore instalado. Instalar al juego solo vía `tools/install-mod.ps1 -Mod <Id>` / `-All`; `-Clean` restaura Workshop-only. Atajo sin script: `/p:DeployToGame=true`. (Implementado en `Sts2PathDiscovery.props`: `StagingPath`/`DeployDir`/`DeployToGame`.)
- **FGOCore.dll no puede estar local Y en Workshop a la vez** (mismo id → duplicado → crash). El build local de FGOCore es **temporal** y se borra al terminar.
- Cuando cambia la API pública de FGOCore, **los 12 mods de personaje se republican JUNTOS** (dll viejo contra FGOCore nuevo → `MissingMethodException`/`ReflectionTypeLoadException`, falla silenciosa). Nunca shippear FGOCore solo.
- Descripciones **localizadas** de Workshop: solo por web UI o Steamworks API (SteamCMD/VDF setea una sola, la default).
- **Importación de texturas controlada (actualizado 2026-07-29)**: `tools/patch_webp_imports.ps1` normaliza imports PNG/WebP nuevos antes del publish (VRAM, mipmaps y límite 1024; 768 para frames). No se reescalan escenas ni se reescriben masivamente assets antiguos. El paso se usa solo cuando entran texturas nuevas y después se vuelve a publicar el `.pck`.
- Manifest dependencies en formato nuevo: `[{"id":"BaseLib","min_version":"v3.3.6"}, {"id":"FGOCore","min_version":"v0.1.10"}]`.

## Diseño / balance
- **Compatibilidad con Infinite Upgrades:** la primera mejora de cada carta FGO conserva su diseño.
  Desde la segunda, los Poderes bajan su coste de Energía hasta 0; las Habilidades con Agotar,
  hasta 0; las Habilidades reutilizables, sólo hasta 1. Los Ataques no repiten indefinidamente una
  rebaja de Energía. Costes de recursos y autodaño tienen suelo 0, salvo Ráfaga, que conserva suelo
  1 para no crear una cadena gratuita; divisores y turnos también tienen suelo 1.
- **Santo Grial = evento exclusivo de Acto 2 (Plan 2, 2026-07-22)**: cuesta 200 de oro, concede el
  `ILimitBreaker` temático del personaje y no aparece si ya se posee uno. Los Griales son reliquias
  de Evento, nunca recompensas/tienda; la carta `Palingenesis` conserva únicamente su aumento menor
  de Vida máxima. FGOCore descubre el Grial desde el pool del personaje, sin referencias circulares
  ni una tabla de IDs.
- **Nada de multiplicador global ×daño en el starter** (lección del Bond ×1.4 = "demasiado roto"). Escalar en motores FGOCore que sobreviven al strip de buffs.
- Techo de saturación ~**180-220 daño/turno**; multi-hit anti-Buffer; no depender de debuffs (los jefes los strippean).
- **Lore**: investigar SIEMPRE en japonés (baseline de diseño) **+** chino simplificado para corroborar. Frases de voz ORIGINALES (no transcripción del juego). NP names = canónico JP → ZH/EN.
- **Shuten Dōji = un solo mod `ShutenDouji`**: Assassin/Caster son Estilos de carta, no formas ni
  personajes separados. Puente cerrado: Veneno nativo + Sake 0–100 + Cruce; a 100 NP elige entre
  dos NP mutuamente excluyentes. Fuente de verdad: `docs/DESIGN-SHUTEN.md`.
- **Astolfo = Rider `AstolfoRider`, sin forma de vuelo**: Razón Evaporada extrae un Capricho visible
  Q/A/B de una bolsa sin repetición; Estrellas/Crítico son el eje ofensivo y Hippogriff/Evasión el
  defensivo. Los cuatro tesoros son cartas concretas, no un segundo arsenal generado. Fuente de
  verdad: `docs/DESIGN-ASTOLFO.md`.
- **Powers FGO reutilizables nuevos van a FGOCore**: Sello de Habilidad y Certero/Sure Hit se
  comparten. Modelos ya publicados conservan su ID; Tiamat delega su power legado al resolver común
  en vez de migrarlo/renombrarlo y romper saves.
- **Evasión FGO es compartida**: vive en FGOCore, tiene máximo 3 y evita el próximo impacto de Ataque
  enemigo que realmente alcanzaría HP; Bloqueo/Buffer resuelven antes, cada impacto consume como
  máximo una carga y no cubre pérdidas propias ni ambientales.

## Técnico (cerrado)
- `CardRarity.Special` **no existe** en el enum del juego → usar `CardRarity.Event` para cartas manifestadas / no-drafteables (quedan fuera de recompensas).
- Las cartas se **auto-registran** por `[Pool(typeof(<X>Pool))]` en la clase base; no hay lista manual que editar.
- IDs de mod / model / power **NUNCA** se renombran con saves activos (el prefijo del mod es parte del ID; migrar una mecánica entre mods rompe runs en progreso).
- `PowerVar<T>` siempre con nombre explícito; `ModifyHpLost*` son ABSOLUTOS; validar rutas de VFX contra `grep '"vfx/' decompiled/`.
- **Estado efímero guardable:** flags/contadores «una vez por turno/combate» y configuración mutable
  que afecte gameplay usan `FgoCombatState`, powers visibles propios o `[SavedProperty]`; nunca campos
  privados del modelo. El estado por turno se limpia en `BeforeSideTurnStart` y sólo cuando
  `participants.Contains(Owner)`.
- **Hooks de cálculo son puros:** `ModifyDamage*`/`ModifyBlock*` no consumen cargas ni mutan estado;
  preview puede ejecutarlos varias veces. El consumo se confirma en hooks posteriores al resultado.

## Caminos abandonados (no volver a girar)
- No mezclar mods locales + Workshop de los mismos ids.
- No tratar "instalado" como "validado en juego".
- No usar RitsuLib para reestructurar el código (coexiste con BaseLib, pero su valor para nosotros es la **telemetría**, que es futuro).
- El install cracked `F:\Games\...v0.103.3` quedó **descartado**: solo se usa el Steam legítimo (v0.107.1).
