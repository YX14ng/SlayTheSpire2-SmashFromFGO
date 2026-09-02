# DECISIONS — reglas ya definidas (no re-discutir)

Solo decisiones **cerradas** + caminos abandonados, para no volver a girar sobre lo mismo.
Patrón tomado de `iryuko/sts2-mod-dev`. Estado vivo → [STATUS.md](STATUS.md). Evidencia → [FINDINGS.md](FINDINGS.md).

## Versión / plataforma
- Target dual con un único artefacto de Workshop: **MAIN v0.107.1 + BETA v0.111.0**. Baseline de compilación **BaseLib 3.4.5** (última publicada en NuGet, verificada 2026-09-02); manifiestos exigen runtime `>= v3.4.5`. Se compila contra la misma versión que corre en Workshop a propósito: con baseline 3.4.0 el compilador quedaba ciego a las rupturas de firma de 3.4.1-3.4.5 (`CustomResource.PrepForCombat()`, ctor de `ResourceHandler`, `CardTransformReward.Amount`) y sólo aparecían en runtime.
- **MegaDot 4.5.1** para exportar el `.pck` (el juego no carga un `.pck` de un Godot más nuevo).

## Deploy
- **Workshop-only**: los mods FGO no viven en `mods/` del juego; se cargan desde suscripciones de Workshop.
- **Visibilidad pública**: FGOCore y todos los mods de personajes FGO presentes o futuros se publican como items públicos. Esta decisión no autoriza uploads automáticos: Steam/SteamCMD solo se ejecutan ante un pedido explícito del usuario.
- **Staging (separación workspace/juego, estilo `iryuko/sts2-mod-dev`)**: build/publish van SIEMPRE a `dist/<ModId>/` del repo (gitignoreado), NUNCA a la carpeta del juego. La referencia a FGOCore sale de `dist/` (`$(StagingPath)`) → el build no depende de tener el juego ni FGOCore instalado. Instalar al juego solo vía `tools/install-mod.ps1 -Mod <Id>` / `-All`; `-Clean` restaura Workshop-only. Atajo sin script: `/p:DeployToGame=true`. (Implementado en `Sts2PathDiscovery.props`: `StagingPath`/`DeployDir`/`DeployToGame`.)
- **FGOCore.dll no puede estar local Y en Workshop a la vez** (mismo id → duplicado → crash). El build local de FGOCore es **temporal** y se borra al terminar.
- Cuando cambia la API pública de FGOCore, **los 12 mods de personaje se republican JUNTOS** (dll viejo contra FGOCore nuevo → `MissingMethodException`/`ReflectionTypeLoadException`, falla silenciosa). Nunca shippear FGOCore solo.
- Descripciones **localizadas** de Workshop: solo por web UI o Steamworks API (SteamCMD/VDF setea una sola, la default).
- **Importación de texturas controlada (actualizado 2026-07-29)**: `tools/patch_webp_imports.ps1` normaliza imports PNG/WebP nuevos antes del publish (VRAM, mipmaps y límite 1024; 768 para frames). No se reescalan escenas ni se reescriben masivamente assets antiguos. El paso se usa solo cuando entran texturas nuevas y después se vuelve a publicar el `.pck`.
- Manifest dependencies en formato nuevo: `[{"id":"BaseLib","min_version":"v3.4.5"}, {"id":"STS2-RitsuLib","min_version":"v0.5.10"}, {"id":"FGOCore","min_version":"v0.1.23"}]`.

## Diseño / balance
- **Touch of Orobas refina la primera Starter:** la reliquia mecánica debe ocupar el índice 0 de
  `StartingRelics` e implementar `GetUpgradeReplacement()`. Su Ancient reemplaza físicamente a la
  inicial, por lo que debe reinstalar forma, contadores y motores esenciales; si ambas almacenan
  `INpLevelStore`, FGOCore transfiere nivel NP y piedad al reemplazo.
- **Ancient por personaje:** todo mazo inicial FGO mantiene una firma `ITranscendenceCard` para
  `ArchaicTooth`; Sea Glass usa su título genérico cuando el personaje es FGO; Yummy Cookie registra
  mediante RitsuLib una visual identitaria por Servant. No se agregan IDs persistentes sólo para
  resolver estas tablas o fallbacks vanilla.
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
- **Calidad visual FGO centralizada en FGOCore + BaseLib:** la configuración es local y puramente
  visual; no se sincroniza como estado de gameplay. Los personajes conservan sus rutas lógicas y
  FGOCore resuelve por convención variantes `character/quality_high/`, con fallback al recurso de
  768 px. `Automática` sólo usa Alta con GPU dedicada, VRAM suficiente y partida en solitario; la
  elección se congela por combate. Sólo se carga la forma visible; las alternativas se solicitan
  de manera asíncrona y las referencias se liberan al salir de la sala de combate. RitsuLib no se
  vuelve dependencia para esta función.
- **RitsuLib 0.5.10 es dependencia transversal:** los 13 mods de gameplay lo declaran directamente.
  MAIN usa `STS2.RitsuLib.Compat.0.107.1` y BETA el paquete regular. FGOCore publica tags dinámicos
  estables Buster/Arts/Quick mediante una capacidad de modelo, expone NP/Estrellas como recursos
  secundarios y registra Orobas/Archaic Tooth por personaje. Los powers previos conservan sus IDs y
  se sincronizan como puente de saves. La DLL del framework sigue siendo externa y nunca se redistribuye.
- **El guard de Embark de BaseLib 3.4.3 se retiró (2026-09-02).** BaseLib 3.4.5 arregló el mismatch
  de raíz: su `CharacterSelectStartingRelicsPatch.OnEmbarkPressedPostfix` ahora pasa por
  `BetaMainCompatibility.LobbyLocalReady()`, que resuelve `StartRunLobby.LocalPlayer` con un
  `VariableReference` por nombre — inmune a la diferencia de retorno CLR entre `LobbyPlayer` (MAIN) y
  `StartRunLobbyPlayer` (BETA). Verificado decompilando 3.4.5. Con `min_version >= v3.4.5` el cargador
  del juego impide que reaparezca un runtime 3.4.3, así que `BaseLibCharacterSelectCompatibility` era
  código muerto y se borró junto con su reproductor en el probe. **Las dos cosas van juntas:** bajar el
  `min_version` obliga a reponer el guard.
- **BaseLib 3.4.5 no resuelve contra MAIN 0.107.1** (hallazgo colateral, no nos afecta): referencia
  `NTreasureRoom._chestButton` tipado `NTreasureButton`, que en MAIN es `NButton` — cambiaron un
  `AccessTools.FieldRef` por acceso directo compilado contra BETA. Rompe sólo a quien use
  `CustomActModel.CustomActTreasureChest`; el repo no lo usa. Es un segundo desajuste MAIN además del
  de lobby, así que la clase de bug no está cerrada: no asumir que un BaseLib nuevo corre en MAIN.
- **Telemetry abandonado:** `FGOTelemetry` no forma parte del grafo, del build ni de futuros
  releases. La compatibilidad útil vive en FGOCore/RitsuLib y no captura ni persiste telemetría.
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
- No sustituir BaseLib ni migrar identidades de modelos a RitsuLib. Se usa como capa interoperable
  para tags, capacidades, recursos secundarios, contratos Ancient, lifecycle y diagnóstico local.
- El install cracked `F:\Games\...v0.103.3` quedó **descartado**: solo se usa el Steam legítimo (v0.107.1).
