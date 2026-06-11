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

## TAREAS PENDIENTES (pedidas por el usuario, NO implementadas — en orden)

### 1. Límite de NP a 300 (como en FGO)

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
