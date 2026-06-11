# HANDOFF — avance y tareas pendientes (para Claude Code en otra PC)

Escrito 2026-06-11 al pasar el desarrollo a otra máquina (Linux). Leer PRIMERO
[WORKFLOW-FGO.md](WORKFLOW-FGO.md) (el playbook con la tabla de gotchas) y CLAUDE.md.
El usuario se comunica en español.

## Estado actual

Mod de **Mash Kyrielight** COMPLETO y funcional (v0.103.x rama MAIN del juego, BaseLib 3.2.1
pinneado). El repo tiene DOS proyectos de mod:

- **`FGOCore/`** — mod-librería compartida (compilar/publicar PRIMERO): Carga NP con eventos
  `GaugeFilled`/`GaugeDropped`, formas (`FormPower`/`FormSwitch`/`FormVisuals` con precarga),
  Baluarte + retención (`IBlockRetentionSource`), vínculo 好感度 (`BondRelic` abstracto),
  cartas meme incoloras.
- **`MashShielder/`** — el personaje, depende de FGOCore (`dependencies` del manifest +
  `<Reference>` con `Private=false` al dll publicado en `mods/`).

Ambos publican limpio con `dotnet publish -c Release` (FGOCore primero; el juego debe estar
CERRADO o el dll queda lockeado). Última publicación verificada 2026-06-11.

## Entorno en esta PC (Linux)

- **MegaDot está en `MegaDotLinux/` del repositorio**, comprimido por el límite de 100MB
  de GitHub. Primera vez: `unzip MegaDotLinux/MegaDot_v4.5.1-stable_mono_linux.x86_64.zip
  -d MegaDotLinux/ && chmod +x MegaDotLinux/MegaDot_v4.5.1-stable_mono_linux.x86_64`.
  Después ajustar `GodotPath` en `Directory.Build.props` de AMBOS proyectos (MashShielder
  y FGOCore) para que apunte a ese binario.
- `Sts2PathDiscovery.props` tiene sección Linux para autodetectar el juego; si no lo
  encuentra, fijar `Sts2Path` a mano en `Directory.Build.props`.
- Verificar que la versión de BaseLib instalada en `mods/` del juego sea EXACTAMENTE la
  pinneada en los csproj (hoy 3.2.1) — si no: `ReflectionTypeLoadException` y el mod no carga.
- El log del juego es siempre el primer lugar para diagnosticar (en Linux:
  `~/.local/share/SlayTheSpire2/logs/godot.log` o equivalente de `user://logs`).

## SIGUIENTE GRAN TAREA: implementar a Morgan (diseño COMPLETO y aprobado)

[DESIGN-MORGAN.md](DESIGN-MORGAN.md) tiene el diseño terminado **v2** (panel multi-agente +
jueces; rehecho a pedido del usuario: FIEL AL KIT de FGO como columna — cada skill real de
Morgan/Aesc es una carta reconocible, Alzarse/Guts central — y la economía Maldición/Impuesto
del v1 conservada como UNO de los tres arquetipos drafteables). Berserker→Caster vía cartas
(swap de modelo con FGOCore FormSwitch) + Reina del Invierno permanente. 68 cartas + especiales,
reliquias, vínculo sobre defaults, assets verificados (Morgan 704020/704030, Aesc 505320).
Orden de implementación y perillas de playtest en su §11. Nuevo para FGOCore: GutsPower +
candado "Inutilizable: turno N" + manifestación de ulti por forma.
**ARTE DE CARTAS YA HECHO** (2026-06-11): las 74 imágenes (500×380 + big/) están pre-armadas en
`MorganBerserker/MorganBerserker/images/card_portraits/` (carpeta pre-scaffold: al crear el
proyecto con el template, conservarla). Mapping en `assets/reference/ce/mapping_morgan.csv`
(entradas `CHARA:` = charagraphs oficiales de los servants, no CEs). Auditado visualmente:
los retratos de Morgan/Aesc usan charagraphs (704000a@1/a@2/b@1/b@2, 704030a, 505300a@1/a@2/b@1);
21 matches malos del workflow fueron re-elegidos a mano (gotcha: CEs con nombre-trampa tipo
"Crown Saber Morgan" = hamburguesa del collab con McDonald's). Herramientas nuevas:
tools/make_contact_sheet.ps1 (auditar arte en lote) y tools/fix_morgan_art.ps1.
La skill [.claude/skills/sts2-mechanics-design](../.claude/skills/sts2-mechanics-design/SKILL.md)
contiene los baselines numéricos reales del juego para cualquier ajuste.
OJO: el export de AssetStudio GUI (×3 modelos) es PASO MANUAL del usuario.

## TAREAS — IMPLEMENTADAS el 2026-06-11 (quedan acá como referencia de diseño)

Las tres se implementaron y publicaron en la PC Windows. Lo que quedó:

1. **NP 300**: `NpChargePower.Max=300` + `ManifestThreshold=100`; la ulti se manifiesta
   al cruzar 100 y se re-arma al caer bajo 100; `IsOvercharged` = ≥100; el strip de buffs
   del Black Barrel dispara a tier ≥100 (era `>= Max`, se habría roto). El Overcharge
   sigue LINEAL hasta 300 (decisión: FGO también es lineal por tramo; si en la práctica
   queda roto, bajar `PerTen` de las cartas NP o capear el tier).
2. **Dupes**: reliquia starter `SummonTicket` (呼符, contador = NP level 1-5) implementa
   `INpLevelStore` + `TryModifyCardRewardAlternatives` (el sistema NATIVO de alternativas
   de recompensa del juego — patrón copiado de la reliquia `PaelsWing`, NO hace falta
   Harmony). Botón "Invocar (dupe)": renuncia a la carta, 50% +25% pity por fallo
   (`FGOCore.NpLevels.TryRollDupe`). Cada nivel: +15% en las 5 cartas NP (escalado en
   OnPlay vía `NpLevels.Scale`; el texto de la carta NO lo muestra, igual que el
   Overcharge). El título del botón vive en `localization/<lang>/card_reward_ui.json`
   con clave LITERAL `OPTION_MASH_DUPE.name` (el juego mergea tablas de mods por nombre
   de archivo). OJO: máximo 2 alternativas por pantalla (guard con alternatives.Count).
3. **Santo Grial**: reliquia rara `HolyGrail` (icono = Lágrima del Grial 7998):
   +15 Vida máx al obtenerla, y mientras se tenga implementa `ILimitBreaker` (FGOCore):
   Vínculo hasta Nv12 (umbrales extra de a 14 pts, +5 Vida máx c/u) y NP level hasta 6.

### Diseño original de referencia — 1. Límite de NP a 300 (como en FGO)

Hoy el medidor está capeado a 100 (`FGOCore/FGOCoreCode/Np/NpChargePower.cs` → `Max = 100`).
En FGO el NP carga hasta 300%. Cambios propuestos:

- `NpChargePower.Max = 300` + nueva constante `ManifestThreshold = 100`.
- La ulti debe manifestarse al CRUZAR 100 (no al llegar al tope): revisar `NpCharge.Gain`
  (dispara `GaugeFilled` cuando `IsFull`) — cambiar la condición a `>= ManifestThreshold`,
  y `GaugeDropped`/re-armado del marcador (`CamelotManifestedPower` en MashShielder) a
  "cae por debajo de 100".
- `IsOvercharged` hoy significa "==100": redefinir (¿>=100?) y revisar TODOS los call sites.
- **Rebalanceo obligatorio**: `ConsumeAllForNpCard` devuelve tier = todo lo consumido; con
  tope 300 los bonus `PerTen` de las cartas NP escalan al triple (Lord Chaldeas a 300 NP
  daría +75 de Bloqueo extra). Opciones: bajar `PerTen`, o tier cap por carta, o que el
  Overcharge por encima de 100 escale a la mitad. Decidir jugando.
- `CylinderDischarge` (consume todo en bloques de 10) también triplica su techo — revisar.
- Actualizar localización (eng/esp/zhs): los textos del power NP en FGOCore dicen
  "hasta un máximo de 100" (`FGOCore/FGOCore/localization/*/powers.json`), y revisar cartas
  de Mash que mencionen 100.

### 2. Mecánica de dupes (subir "NP level" renunciando a cartas)

Idea del usuario: al elegir carta de recompensa, que aparezca un botón para RENUNCIAR a
la carta a cambio de la chance de obtener un **dupe** del personaje, que sube el nivel de
las habilidades que son ultis en el juego original (las cartas NP). Ajustar las
probabilidades para que NO sea tedioso.

Sugerencias de diseño (a validar con el usuario):

- **Probabilidad**: alta o garantizada con pity (p. ej. 50% + garantizado tras 2 fallos);
  el costo real ya es renunciar a la carta.
- **NP level** (1→5 como FGO): contador persistente por run — patrón ya resuelto:
  `[SavedProperty]` en una reliquia (ver `BondRelic`/`MashBond`). Podría ser una reliquia
  starter oculta o sumarse al propio `MashBond`.
- **Efecto**: +stats SOLO en cartas NP (LordChaldeas, LordCamelot, BlackBarrelFullBurst,
  RhongomyniadReplica, LordCamelotUnleashed). Sugerencia: interface `INpCard` en FGOCore
  con `int NpLevel` o un helper `NpLevel.Get(player)` que las cartas NP lean en `OnPlay`
  (+X% daño/bloqueo por nivel, p. ej. +10% por dupe).
- **El botón en la pantalla de recompensa**: requiere Harmony patch sobre la pantalla de
  card reward del juego (buscar en `decompiled/` clases tipo `NCardReward*` /
  `NCardGridSelectionScreen` — la pantalla ya tiene botón de "skip", usarlo de referencia
  para agregar otro). Es la parte más delicada: investigar primero cómo agregan botones
  los mods instalados de referencia en `mods/`.
- Localización trilingüe del botón/power/tooltips (eng/esp latino/zhs — terminología
  Mooncell: 宝具强化 o NP升级 para NP level, 同名从者/重复 para dupe).

### 3. Santo Grial / Palingenesia (superar límites máximos)

Idea del usuario: la mecánica de Santo Grial de FGO (superar el nivel máximo) aplicada al
mod — "pensa una forma de aplicarlo". Propuestas para discutir con el usuario antes de
implementar (elegir UNA o combinar):

- **(a) Reliquia rara "Santo Grial"**: rompe topes mientras la tengas — el vínculo puede
  superar Nv10 (Nv11+ repiten la curva de regalos o escalan el capstone), y/o +100 al tope
  de NP (sinergia con la tarea 1: 300→400 con grial).
- **(b) Evento custom "Palingenesia"**: una vez por run ofrece "grailear" a Mash:
  +HP máx grande permanente y +1 NP level (conecta con la tarea 2). Los eventos custom
  van vía BaseLib (revisar wiki / mods de referencia con eventos).
- **(c)** Doble-upgrade de cartas (más allá de +) — el más invasivo, probablemente
  requiere demasiado parcheo. Última opción.

La carta meme `Palingenesis` ya existe en FGOCore (圣杯转临) — mantener coherencia temática
y de naming con lo que se elija.

## Notas para no romper nada

- IDs de modelos NUNCA se renombran entre versiones si hay saves activos (prefijo del mod
  incluido). Migrar algo entre mods cambia su ID → rompe runs a medio terminar.
- Cualquier cambio no-código (loc, imágenes, escenas) requiere `dotnet publish`, no build.
- `PowerVar<T>` SIEMPRE con nombre explícito. Hooks `ModifyHpLost*` son ABSOLUTOS.
  VFX: validar contra `grep '"vfx/' decompiled/`. Todo en la tabla de gotchas del workflow.
- Los iconos de powers core viven en el pck de FGOCore; subclases de personaje re-overridean
  las rutas de imagen (ver `MashFormPower`/`MashBond`).
