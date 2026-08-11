# STATUS — estado actual (alta densidad)

Backlog canónico de futuros personajes: [`CHARACTER-TODO.md`](CHARACTER-TODO.md).

## 2026-08-11 — Auditoría profunda de bugs (multi-agente): 11 confirmados, arreglados y re-verificados

Pasada de tres etapas (buscar → refutar → arreglar → re-verificar el diff) sobre los 13 proyectos,
con seis lentes: estado de combate, multiplayer/determinismo, entornos (Linux nativo vs Proton),
compatibilidad MAIN/BETA, saves/serialización y contratos duros del vanilla. 12 hallazgos brutos,
11 confirmados, 1 rechazado (el modo Auto de calidad visual en Linux nativo — decisión de diseño ya
cerrada en DECISIONS.md:60, no un defecto). Dos rondas de re-verificación adversarial encontraron
problemas en los primeros arreglos, así que lo que sigue es el estado final.

**Crashes duros arreglados**

- **`CardModel.SelectionScreenPrompt` TIRA si falta la clave de localización**
  (`CardModel.cs:128-137`, igual en BETA; BaseLib NO lo cubre: `MissingLocPatch` parchea
  `GetLocString`/`GetRawText` pero nunca `HasEntry`, que es lo que consulta `Exists()`).
  AstolfoRider tenía 6 usos y 1 clave, KagetoraLancer 3 usos y 0 → **8 cartas crasheaban al
  jugarse**. Agregadas las claves faltantes en eng/esp/kor/rus/zhs. `RelicModel` NO tiene ese
  `throw`, pero le faltaba el texto a `SakeCup` (mostraba la clave cruda): también agregado.
- **Guarda del gacha de NP: `>= 2` no alcanzaba.** Dos agujeros. (1) *Ceder no sirve*:
  `CardReward.OnSelect` captura las alternativas UNA vez antes del loop (`CardReward.cs:189`) y
  resuelve el índice contra esa copia (`:249`), mientras `Reroll()` → `Populate()` solo refresca la
  PANTALLA — con Driftwood el clic sobre el gacha regenerado caía en el REROLL viejo → rerolls
  gratis infinitos y gacha inalcanzable. (2) *Mirar la lista no alcanza*: `IterateHookListeners`
  recorre `Player.Relics` en orden de obtención y la reliquia de identidad es **starter**, así que
  corre primera y no ve lo que agreguen las de después — con **Pael's Wing** (Ancient, agrega
  SACRIFICE sin condición) el total llegaba a 3 y `Generate` tiraba, **rompiendo toda recompensa de
  carta del run**. Fix: `FGOCore/FGOCoreCode/Np/NpDupeAlternative.cs`, una sola implementación
  compartida por los 12 personajes, que reserva el lugar de Pael's Wing y **desplaza** el REROLL en
  vez de cederle el turno (solo cuando de verdad no entran los dos). Tradeoff asumido: con Driftwood
  y NP por debajo del tope, el reroll no se muestra; al llegar al máximo (o con Grial) vuelve.
- **Listas sin tope a `FromChooseACardScreen`** (tira con más de 3 cartas): `OptimalPath` (Astolfo)
  pasó a `FromSimpleGrid` con orden determinista para multiplayer, `FormationRelay` (Kagetora) a
  `FromCombatPile` con filtro — sin recortar candidatas.

**Corrupción de estado y saves**

- **`FgoCombatState`**: `ValidateField` prometía campos hasta el bit 61, pero `PowerModel.Amount` es
  `int` y `SetAmount` **clampea en silencio** en ±999.999.999 (`PowerModel.cs:545`, MAIN y BETA).
  Como se commitea `state+1`, el techo real es **29 bits**; con más, el Amount quedaba pegado al
  tope y todos los campos leían basura. Corregido a `TotalBits = 29` (el bit más alto en uso hoy es
  el 13) y la guarda reescrita como `offset > TotalBits - width` para que no desborde.
- **`BondRelic`**: al continuar un save hecho justo después de un evento, `AfterRoomEntered` se
  re-disparaba y re-sumaba el punto de vínculo. Ahora hay `LastBondMapPoint` (`[SavedProperty]`)
  contra el punto de mapa, y sin coordenada válida se sale en vez de premiar.
- **`ATeamDiary` (Mash)**: mismo replay, +1 Max HP duplicado. Guarda `GrantedRooms` contra el
  historial de mapa, inicializada en `AfterObtained()` (es reliquia de evento: se consigue dentro
  del room que después re-dispara el hook).
- **Guards `_isClamping` sin `try/finally`** (`CritStarsPower`, `CritReadyPower`, `EvasionPower`):
  una excepción en el `ModifyAmount` anidado dejaba el cap del recurso apagado el resto del combate.

**Entornos y UI**

- **Kill-switch del bridge con falso positivo**: el canario corría también sobre nodos huérfanos
  (hoguera/tienda), donde `_Ready` está diferido y `ReadyRan == false` no dice nada — apagaba el
  suavizado en **todos** los entornos, Windows/Proton incluido. Ahora el canario solo cuenta con el
  sprite ya en árbol (combate), y los call-sites huérfanos no agregan nada hasta que el combate
  confirme el bridge (`_bridgeVerified`). Costo conocido: si se continúa un save parado en una
  tienda, esa visita va sin suavizado; se recupera tras el primer combate.
- **`CombatMetersActive` sigue siendo un latch, y ahora está documentado por qué.** Se probó
  cambiarlo por una consulta viva sobre la fila (para recuperarse si el factory falla en un combate
  posterior) y la re-verificación lo tumbó: `NPowerContainer` muestrea `power.IsVisible` UNA sola
  vez al aplicar el power (`NPowerContainer.cs:104-112`) y nunca reevalúa, y `NSceneContainer`
  desprende la sala anterior antes de agregar la nueva — o sea que entre combates la propiedad viva
  daba false justo cuando corre `SetCreature`, y volvían los indicadores DUPLICADOS que v0.1.20
  había sacado. Revertido al latch, con la trampa explicada en un `<remarks>`. La recuperación ante
  un registro fallido ya la daba el `catch` de `RegisterCombatUi`.
- **Comentarios falsos corregidos** (`NpLevels.TryRollDupe` sobre el modelo de ejecución en MP, el
  de `FormationRelay`, el de `CombatMetersActive`): en este repo un comentario falso ya causó una
  vez que se escribiera una guarda sobre una premisa errada (`d52ea883`).

**Versiones:** FGOCore **v0.1.23** (API nueva `NpDupeAlternative` → los 12 personajes se republican
en el mismo lote y ahora piden `FGOCore >= v0.1.23`); Mash v0.1.20, Siegfried v0.1.21,
Artoria/Morgan/Mordred/Gilgamesh/Okita/Oberon/Tiamat v0.1.18-19, Kagetora v0.1.11, Astolfo v0.1.11,
Shuten v0.1.10. Matriz MAIN/BETA verde (26 builds, 0 warnings, Harmony targets 24/24/27).

**Queda pendiente (no bloqueante):** portar `FgoSpriteMotion` a GDScript adjuntado desde el `.tscn`
(como `vortigern_motion.gd` de Oberon) eliminaría de raíz el bridge C# de mod, el kill-switch y sus
dos flags estáticas; hoy el canario del primer combate sigue emitiendo ~20 bloques `0x80070057` por
sesión en Linux nativo. La sala final del Arquitecto no otorga el +1 de ATeamDiary (se entra sin
apendear historial de mapa; es terminal, sin impacto jugable).

## 2026-08-11 — Crash con Driftwood: guarda de alternativas corregida en los 12 personajes

3.er reporte de ArgoDevilian (gist `Argo11/47a09cd…`, log 2026-08-10 13:40): con la reliquia
vanilla **Driftwood** (reroll de card rewards) no se abre ninguna Card Reward tras combate, y al
continuar la run a veces crashea. El log confirma dos cosas:

1. **El kill-switch de v0.1.22 FUNCIONA**: 20 bloques `0x80070057` (vs 96) — el ruido del único
   canario al recrear el combate en `mode=FinishedCombat` — y después el warn "suavizado …
   desactivado para la sesión"; cero errores de bridge desde ahí.
2. **Bug nuevo, nuestro:** `InvalidOperationException: More than 2 card reward alternatives are
   not supported` en `CardRewardAlternative.Generate` ← `CardReward.OnSelect` al clickear la
   reward. **`Generate` TIRA con más de 2 alternativas en MAIN v0.107.1 Y BETA v0.110.1**
   (verificado en ambos decompilados). Con Driftwood, Skip + Reroll ya son 2; nuestras 12
   reliquias de identidad (gacha de NP) agregaban la suya con guarda `alternatives.Count >= 3`
   → 3 → throw. Esa guarda venía del fix de junio `d52ea883`, cuya premisa ("la pantalla no topa
   en 2, vanilla muestra 3") era falsa contra el juego actual.

**Fix:** guarda `>= 2` en las 12 reliquias (`TryModifyCardRewardAlternatives`). Versiones
bumpeadas: Mash v0.1.19, Morgan/Mordred/Gilgamesh/Okita/Oberon/Tiamat/Artoria v0.1.17-18,
Siegfried v0.1.20, Kagetora v0.1.10, Shuten v0.1.9, Astolfo v0.1.10. FGOCore NO cambia.
Matriz MAIN/BETA verde (13 artefactos, 0 warnings).

> **Corrección (2026-08-11, auditoría profunda):** este texto decía que "no se pierde el gacha con
> Driftwood porque `Reroll()` → `Populate()` → `Generate` regenera Skip + gacha". **Es falso** y la
> guarda `>= 2` quedó **reemplazada** — ver la entrada de la auditoría más abajo.

## 2026-08-10 — FGOCore v0.1.22: kill-switch del smoothing (respuesta al 2.º gist de ArgoDevilian)

ArgoDevilian reportó que el crash sigue con v0.1.21 (gist `Argo11/db13665c…`, log del 2026-08-09
18:31, FGOCore actualizado confirmado por timestamp del item y por los logs de `RelicPoolFallback`
funcionando en producción). Forense: 96 errores `ArgumentException 0x80070057`, TODOS de
`FgoSpriteMotion`, ahora desde `InvokeGodotClassMethod`/`HasGodotClassMethod` durante el `AddChild`
de `Prepare` y el teardown de sala (`RemoveChildSafely` en `SetCurrentScene`); el log muere justo
en la transición al rest site — mismo patrón que su crash original del Treasure Chest.

**Hallazgo:** en el build NATIVO Linux del juego (Argo corre el binario Linux, no Proton), toda
llamada engine→script sobre un Node C# instanciado con `new` desde un assembly de mod falla —
no era solo `set_name`. Las clases del juego pasan por node factories (BaseLib logea "Created
node factory for X"); RitsuLib inicializa las suyas; `FgoSpriteMotion` era el único Node de mod
`new`-eado crudo. **Fix (v0.1.22, `b6bae9b9`):** canario con kill-switch de sesión — si el
`_Ready` del primer `FgoSpriteMotion` no corrió sincrónicamente dentro del `AddChild`
(`ReadyRan==false`), se retira el nodo y el smoothing (puramente cosmético) queda desactivado.
En Windows/Proton el canario pasa y no cambia nada. Matriz MAIN/BETA verde (13 artefactos).

**PUBLICADO 2026-08-10 21:32 UTC** y verificado por API (item 3747876334, público, 14,5 MB).
La credencial de la copia `tools/steamcmd/` había expirado; el usuario re-logueó en el steamcmd
DE SISTEMA (`/usr/sbin/steamcmd`) y el uploader ahora resuelve el binario desde
`tools/.steamcmd_path.txt` (gitignoreado, `85701f97`) para reutilizar siempre esa sesión cacheada
— usar UNA sola instalación de steamcmd es lo que mantiene el login persistente.

## 2026-08-09 (e) — re-render de Astolfo: attack completo, preparado v0.1.9

- **Causa cerrada:** `MEASURE_SKIP["400400"]` excluía el attack del union de crop asumiendo props
  en el borde, pero el `attack_q` es galope terrestre de la FIGURA — 54/55 frames amputados en el
  WebP. Re-render con el attack incluido: canvas 1513×1010 → **1909×1541**, mismos conteos
  (78/55/30/17) y misma lista de archivos. Residual aceptado: la punta de la lanza roza el borde
  izquierdo en 21 frames del galope (banda de 13-33 px, el cuerpo nunca — mismo criterio que los
  props de Aesc/704710).
- **Transform recalculado** (fórmula WORKFLOW-FGO, `cx_idle=1264.6`, `alphaBottom=1433`, factor
  `768/1909`): escala `1.009`, combate `+126/−269`, tienda/fogata `−126/−269`; altura visible
  ~362 px (baseline, sin cambio de tamaño percibido). Bounds/markers sin cambios. HD sigue con el
  multiplicador default 0.75 (ambas variantes clampean el mismo canvas).
- **Gotcha del pipeline en frío:** el `render_all` borra la carpeta de frames y el reimport
  regenera los `.import` con params DEFAULT (lossless, sin mipmaps) y uids nuevos — el PCK saltó a
  98 MB. `patch_webp_imports.ps1` los renormaliza (lossy 0.85 + mipmaps + 768/1024) → PCK final
  **40,4 MB** (menos que los 46 del v0.1.8 pese al canvas mayor). El churn de uid es inocuo (los
  `.tres` referencian por path). Portabilidad arreglada: `-MegaDot` en `render_all_astolfo.ps1` y
  separadores en el match de `patch_webp_imports.ps1`.
- **Verificación:** inventario del PCK idéntico al v0.1.8 publicado (1325/1325); alpha de los
  frames medido con PIL (idle/cast/hurt sin contacto de borde). Fuentes de render enlazadas desde
  la copia hermana (`assets/reference/{extracted,bundles}`, ahora gitignoreados).
- **PUBLICADO:** `v0.1.9` subido al item `3774222236` con orden explícita del usuario y
  verificado por API (visibility=0, 40,6 MB, descripción actualizada). El conteo de éxitos del
  uploader ya funciona en Linux (fix de la regex de la entrada (d)). Pendiente sólo el playtest
  visual del galope y del tamaño en pantalla.

## 2026-08-09 (d) — lote publicado en Workshop desde la máquina Linux

- **Publicado con orden explícita del usuario:** FGOCore `v0.1.21` (`3747876334`), Artoria
  `v0.1.17` (`3747876956`) y Astolfo `v0.1.8` (`3774222236`) en UNA sesión de SteamCMD Linux,
  `-Visibility 0` explícito (el stage fresco de esta máquina defaultea privado — gotcha a
  recordar). Verificado contra la API de Steam: los tres `time_updated` al momento del upload,
  `visibility=0`, descripciones con las versiones nuevas. El resto del roster no cambió
  (FGOCore aditivo).
- **Login:** SteamCMD no puede tomar la contraseña vía la sesión del agente; el usuario hizo el
  `+login` interactivo una vez en una terminal propia y la credencial quedó cacheada para las
  corridas no-interactivas siguientes.
- **Fix del uploader:** el conteo de éxitos usaba un match literal `Committing update...Success`
  que en el steamcmd de Linux cuenta 0 (ANSI + avisos IPC intercalados) y hacía fallar el script
  DESPUÉS de subir bien — regex ahora Singleline con `.*?`. Los `tools/.workshop_id_*.txt` de los
  13 items se recrearon en esta máquina (gitignoreados) para nunca duplicar items.
- **Pendiente:** playtest real con los items sincronizados (medidor NP/★, tienda, Artoria co-op,
  escala de Astolfo), re-render de attack de Astolfo, responder a los reporters de Steam
  (ArgoDevilian: pedir log del cofre y avisar de OstyAnime/VoltaicAnime; reporte chino del
  Circlet: confirmar que v0.1.20+ lo arregla).

## 2026-08-09 (c) — toolchain Linux operativo; matriz MAIN/BETA verde en esta máquina

- **Herramientas nativas instaladas:** MegaDot Linux 4.5.1.m.13 (descomprimido del zip del repo,
  `GodotPath` en `Directory.Build.props` local), PowerShell 7.6.4 (`~/.local/opt`, symlink
  `~/.local/bin/pwsh`), SteamCMD Linux (`tools/steamcmd/`, gitignoreado; upload sigue requiriendo
  orden explícita). `.compat/sts2-{main,beta}` y los decompilados son symlinks a la copia hermana
  de `/mnt/Programs`; los 7375 objetos LFS (2,6 GB) se materializaron desde el store hermano
  (content-addressed, sin red) — la causa de los punteros era que los filters de LFS no estaban
  instalados en esta máquina (`git lfs install` ya corrido; los próximos pull vienen completos).
- **Matriz MAIN/BETA: VERDE local.** 454 contratos + tres probes `Compatibility OK`
  (MAIN→MAIN, MAIN→BETA, BETA→BETA; 13 artefactos, 1.846 tipos, 2.288 miembros, 24/27 targets
  Harmony). Cierra el gate BETA de la revisión (b): `EnergyCounterContainer` enlaza en BETA.
  Fixes de portabilidad para correrla acá: `Sts2PathDiscovery.props` con fallback Proton
  (`data_sts2_windows_x86_64`) y respeto del valor preseteado; `audit_vanilla_contracts.ps1`
  normaliza separadores para la allowlist; `build_compat_matrix.ps1` extrae el nombre de proyecto
  con separadores normalizados.
- **Publish + PCK verificados por contenido:** FGOCore/Artoria/Astolfo exportados con MegaDot
  Linux y montados con el motor para listar archivos — paridad EXACTA contra los PCK publicados
  del Workshop (FGOCore 285 = 283 + los 2 scripts nuevos; Artoria 2389/2389; Astolfo 1325/1325,
  con los .tscn nuevos adentro). ⚠️ El export sale con código -1 por la validación «no .sln»
  (ruido conocido, el repo no usa .sln): NO usar el exit code como verde — verificar contenido
  (dump por `load_resource_pack` + diff, script en scratchpad de sesión). Gotcha resuelto: un
  export con punteros LFS deja `valid=false` en los `.import` y el PCK pierde esos ctex —
  restaurar los `.import`, borrar los `.md5` huérfanos de `.godot/imported/` y `--headless
  --import`.
- **Pendiente que sigue igual:** playtest visual (medidor NP/★ y escala de Astolfo), re-render de
  attack de Astolfo (ahora POSIBLE en esta máquina: assets LFS reales + MegaDot), y upload a
  Workshop con orden explícita.

## 2026-08-09 (b) — revisión de dos ejes + caza de defectos sobre el lote

- **Corregido [ALTA]:** `RelicPoolFallback` consultaba `RelicModel.Owner`, que hace
  `AssertMutable()` y tira `CanonicalModelException` en el modelo canónico (hover en biblioteca,
  fuera de run) — ahora replica el guard vanilla `IsMutable` de `EnergyIconHelper`. Además el
  finalizer sólo suprime la `InvalidOperationException` del `First()`; todo lo demás se propaga.
- **Endurecido:** la fila de medidores NP/★ aísla el anclado a `EnergyCounterContainer` (si el
  contrato difiere en BETA, degrada a posición por defecto y el `Refresh` corre igual — RitsuLib
  traga las excepciones del callback entero) y `CombatMetersActive` recién se enciende cuando la
  fila dibujó al menos una vez: si el factory/attach falla, los powers legacy quedan visibles.
  `IsVisible` de powers se lee en vivo (sin cache/serialización) → el flip es seguro.
- **Corregido (co-op):** el drain de «Dos Caras del Verano» filtra dueño
  (`cardPlay.Card.Owner != Owner.Player`), patrón vanilla universal; sin esto una carta del
  compañero podía robar para una Artoria muerta (Thorns letales mid-carta dejaban el pendiente).
  El pendiente además se limpia en `BeforeSideTurnStart` (regla de estado efímero de DECISIONS).
- **Deuda saldada — Critical v2:** la consolidación (`cc8b6669`) migró el crítico de Artoria al
  sistema global (×1.5 automático, 50★) pero dejó `ResolveCritDamage`/`ResolveCritDamageScaling`
  como stubs que devolvían el daño base y tablas `Crit`/`CritCost`/`PerStar` huérfanas en 10
  cartas — **código muerto que aparentaba mecánica**. Stubs eliminados y cartas podadas a daño
  directo; cero cambio de runtime (esos números ya no se ejecutaban). Ninguna loc usaba `!Crit!`.
- **Preventivos:** `PreviousFrame` ya no se nombra desde C# (mismo criterio del fix del bridge
  Linux); `DrainFinishedRequests` hace `Dispose()` inmediato (el unref no queda diferido al GC) y
  memoiza `Failed` como `GetFrames`.
- **Verificados sin defecto (contra decompilado MAIN):** contratos Harmony del guard de tienda
  (`_Ready` declarado en la clase, `PlayAnimation(string anim, bool)` bindea, skip seguro);
  robo diferido no reintroduce el soft-lock (la carta en curso vive en la pila Play; el reshuffle
  toma sólo Descarte+Mazo); Astolfo coherente en las tres superficies y con el flujo HD
  (0.8×0.75=0.6 sobre 1024 = invariante exacta); publish de FGOCore+Artoria+Astolfo solos es
  correcto (API aditiva). **Gate para publicar:** correr la matriz MAIN/BETA en Windows — el
  riesgo restante es que `EnergyCounterContainer` difiera en BETA (ya degradado, la matriz lo
  detecta).

## 2026-08-09 — lote de fixes por reportes de Steam (preparado en la máquina Linux)

- **Crash de tienda (ArgoDevilian, Linux MAIN):** `NMerchantCharacter._Ready` construye
  `MegaSpineBinding` sobre `GetChild(0)` asumiendo SpineSprite; el camino `TryAutoConvert` de
  BaseLib no marca `CreatedFromFactory`, así que la guardia de BaseLib 3.4.3 no corre y la escena
  raster FGO abortaba el juego al entrar a la tienda. Fix: `MerchantSpineGuard` en FGOCore
  (prefixes `Priority.Low` sobre `_Ready` y `PlayAnimation` con fallback raster; mismo chequeo
  `GetClass()=="SpineSprite"` que usa `NRestSiteCharacter`). Reportar upstream a Alchyr.
- **~100 `ArgumentException: Undefined resource string ID:0x80070057` por sesión (Linux):**
  `set_name` C# sobre `FgoSpriteMotion` propaga `NOTIFICATION_PATH_RENAMED` al bridge, roto en el
  runtime Linux recortado (MegaDot 4.5.1.m.12; misma firma documentada por carlineng/STS2Dojo).
  Fix: el nodo de suavizado ya no se nombra desde C# (dedup por tipo), guard null en `_Process`.
  FGOCore ahora loggea `FrameworkDescription`/`OSDescription` al iniciar para triaje futuro.
- **Tooltips de reliquias FGO rotos:** `RelicModel.Pool` hace `First()` y explota para TODA
  reliquia fuera de un relic pool (todas las de personajes custom; RitsuLib ya lo advertía).
  Fix: `RelicPoolFallback` (finalizer que devuelve el pool del personaje dueño o el primero).
  No se registran pools nuevos: gobiernan drops.
- **NP/Estrellas invisibles post-v0.1.20 (Smooth, AnneFlank88):** RitsuLib NO dibuja UI por
  defecto para recursos secundarios — `RegisterCombatUi` es API para el mod y nadie la llamaba,
  con los powers legacy ya ocultos. Fix: FGOCore registra `NSecondaryResourceCounterRow` anclada
  sobre `EnergyCounterContainer` (offset −72 px, ajustar en playtest); si el registro falla, los
  powers legacy vuelven a ser visibles (`CombatMetersActive`).
- **Reliquia inicial → Circlet (reporte chino 1/8):** era el mapeo de Touch of Orobas sin
  registrar, cubierto por v0.1.20 (los 12 personajes registran los 5 genéricos + adaptador que
  repara saves). Confirmar en runtime; no requiere código nuevo.
- **Artoria `v0.1.17`:** «Dos Caras del Verano» ahora difiere a `AfterCardPlayed` los robos que
  el mazo no cubre (la carta en curso sigue en la pila Play y el reshuffle sólo toma
  Descarte+Mazo; precedente vanilla GamePiece — la cita anterior a Driftwood era errónea, esa
  reliquia rerollea recompensas). «Tajo de la Espada Sagrada» 6→**9** (up 12): base al piso de
  común 1⚡ pura.
- **Astolfo `v0.1.8` (parcial):** escala 1.0→**0.8** y pivotes X/Y −102/−229→**+47/−182**
  (tienda/fogata −47 por la regla de no-espejado); Bounds/markers a valores tipo Okita. Estaba
  +24,5% sobre la baseline (~360 px visibles) por el factor de import de su lienzo 1513×1010 sin
  compensar. **Pendiente re-render:** los frames de attack están recortados EN los WebP (54/55
  tocan borde; `MEASURE_SKIP["400400"]` excluyó attack del union de crop en `render.gd:91`) —
  sacar el skip, re-medir, regenerar 768+1024 y recalcular pivotes/escala con el canvas nuevo.
- **Descartado:** la detección de VRAM ya estaba guardada por plataforma (en Linux Auto→Balanced
  768 px, seguro); `aliento_power.png` existe (WARN benigno del PreloadManager); el frame de
  `MumyouUnleashed` en los logs era ruido de OstyAnime (mod ajeno roto en v0.107.1); el crash de
  Treasure Chest no muestra mecanismo FGO en los gists — pedir log completo al jugador.
- **Versiones preparadas:** FGOCore `v0.1.21`, Artoria `v0.1.17`, Astolfo `v0.1.8` (manifiestos
  y fichas de Workshop actualizados; el resto del roster no cambia — FGOCore v0.1.21 es aditivo
  y satisface los `min_version v0.1.20`).
- **Validación:** los 13 proyectos compilan en verde en Linux contra el juego local MAIN
  v0.107.1 (Sts2DataDir del install de Steam). **Omitido en esta máquina:** matriz MAIN/BETA
  (PowerShell/Windows), export de PCK (MegaDot), `.uid` de los dos .cs nuevos (los genera el
  editor al importar), playtest visual (posición del medidor NP/Estrellas y escala de Astolfo) y
  publicación a Workshop — todo queda para la máquina Windows con orden explícita de upload.

## 2026-08-04 — migración de FGOCore a RitsuLib 0.5.10 publicada

- **Recursos interoperables:** Carga NP y Estrellas de Crítico se registran como recursos secundarios
  estables de RitsuLib. La UI nueva evita duplicados; los powers publicados conservan sus IDs y se
  sincronizan en ambos sentidos para que runs y combates guardados sigan cargando.
- **Ancient 12/12:** cada personaje registra oficialmente en RitsuLib su par de Touch of Orobas y
  su transformación de Archaic Tooth. FGOCore conserva sólo los adaptadores necesarios para reparar
  un Circlet ya guardado y transferir `NpLevel`/`DupePity` durante el reemplazo.
- **Framework actualizado:** los 13 proyectos compilan contra RitsuLib `0.5.10` y sus manifiestos lo
  exigen como mínimo. Continúan los parches FGOCore que RitsuLib no cubre: Sea Glass, historial SFX,
  firmas MAIN/BETA y el guard específico de BaseLib 3.4.3.
- **Versiones publicadas:** FGOCore `v0.1.20`; Mash `v0.1.18`; Morgan, Artoria, Mordred, Gilgamesh,
  Okita, Oberon y Tiamat `v0.1.16`; Siegfried `v0.1.19`; Kagetora `v0.1.9`; Shuten `v0.1.8` y
  Astolfo `v0.1.7`. Todos los personajes exigen FGOCore `v0.1.20`.
- **Validación:** matriz MAIN→MAIN, MAIN→BETA y BETA→BETA verde para 13 artefactos, cero errores o
  advertencias; 454 contratos, 1.837 tipos, 2.275 miembros y 21/24 destinos Harmony resueltos.
  También pasan paridad 13×5, SimpleLoc (0 ambigüedades), 288 VFX, 867 cartas, 257 poderes y
  124 reliquias. Los 13 PCK abren con manifiesto idéntico al externo y cero DLL internas.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` para los 13 ítems
  públicos existentes. La API pública confirmó las versiones nuevas y visibilidad pública; Steamworks
  verificó los 38 vínculos de BaseLib, RitsuLib y FGOCore. No se instaló ninguna copia local. Falta
  el playtest visual/runtime de los recursos secundarios y las transformaciones Ancient.

## 2026-08-04 — Mash v0.1.17 publicada: Paladín y Lord Chaldeas

- **Paladín legible:** su tooltip ya no obliga a recordar Shielder y Ortinax; enumera directamente
  el Bloqueo adicional, el umbral y ganancia de NP, el consumo ofensivo de Bloqueo, la ausencia de
  penalización defensiva y su permanencia en los cinco idiomas.
- **NP final corregida:** `LordChaldeasUnleashed` conserva sus 35 de Baluarte y ahora también otorga
  3 de Fuerza y 12 de Intercepción. En cooperativo replica el soporte de Lord Camelot —12 de
  Baluarte y 6 de Intercepción por aliado— y su mejora agrega 10 de Baluarte y 1 de Fuerza.
- **Artefacto publicado:** Mash `v0.1.17` compila sin errores ni advertencias. Paridad 13×5,
  SimpleLoc y ficha de Workshop aprobaron; el PCK contiene 2.610 archivos, manifiesto interno
  idéntico al externo y cero DLL embebidas. Se verificaron dentro del paquete la versión y los
  textos corregidos de los cinco idiomas.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` para el ítem existente
  de Mash (`3747876464`). La API pública de Steam devolvió resultado correcto, visibilidad pública
  y descripción `v0.1.17`. No se instaló una copia local; queda pendiente el playtest real de la
  transición a Paladín y de Lord Chaldeas en solitario/cooperativo.

## 2026-08-03 — hardening de transiciones y ajuste de Siegfried publicados

- **Presión de memoria acotada:** `FormVisuals` ya no precarga todas las formas alternativas al
  entrar al combate. Solicita únicamente el modelo visible, mantiene la imagen anterior durante un
  cambio asíncrono y libera las referencias fuertes al salir de `NCombatRoom`, evitando que evento,
  tienda o descanso hereden varios GiB de texturas. El reporte externo no aportó un log reproducible;
  se corrigió una causa verificable de presión de memoria sin atribuirle definitivamente el crash.
- **Siegfried más claro y flexible:** la alternativa Invocar incrementa de forma permanente las
  Escamas iniciales de 2 hasta 5 aunque falle el dupe. Cada Ataque que quite Vida otorga 5 NP, con
  límite de 3 activaciones por turno; la Hoja de Tilo conserva su debilidad canónica de ignorar una
  Escama en el primer golpe. El progreso nuevo se guarda y sobrevive al reemplazo Ancient de Orobas.
- **Versiones publicadas:** FGOCore `v0.1.19` y Siegfried `v0.1.18`, que exige FGOCore `v0.1.19`.
  La matriz MAIN/BETA completó 26/26 builds sin errores ni advertencias, incluidas las sondas
  MAIN→BETA y 450 contratos fuente. Pasan paridad 13×5, SimpleLoc, 288 VFX, 867 cartas, 257 poderes,
  124 reliquias, calidad HD (21 recursos/3.324 frames), presentación 12/12 y animaciones (0 errores).
  Los 13 PCK abren con manifiesto exacto y cero DLL internas; FGOCore y Siegfried contienen las
  versiones nuevas.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` para los ítems públicos
  existentes de FGOCore (`3747876334`) y Siegfried (`3751611015`). No se instaló ninguna copia
  local, por lo que no hay IDs duplicados con las suscripciones. Sigue pendiente el playtest real y
  un `godot.log` posterior del reporte externo.

## 2026-08-01 — auditoría de colisiones vanilla y referencias inexistentes publicada

- **Gate nuevo:** `tools/audit_vanilla_contracts.ps1` ejecuta 440 comprobaciones sobre los doce
  mazos y los consumidores sensibles de MAIN/BETA. La matriz lo corre automáticamente antes de
  compilar y corta ante cambios de contrato, reflexión no inventariada, llamadas directas frágiles,
  cargas síncronas o localización derivada faltante.
- **Colorful Philosophers 12/12:** los pools implementan el marcador oficial de RitsuLib 0.5.4 y
  FGOCore incluye sus títulos/descripciones en los cinco idiomas. El audit abre el PCK y verifica
  las 24 claves por idioma, evitando opciones ausentes o `LocException`.
- **Historial con audio:** el juego devolvía cero SFX para todo tipo custom. FGOCore agrega un
  fallback de sonidos vanilla únicamente para personajes de assemblies que lo referencian y sólo
  cuando el juego no produjo ningún sonido.
- **Referencias reales:** la sonda resuelve 1.823 tipos, 2.254 miembros y 19 destinos Harmony contra
  MAIN/BETA; el artefacto universal MAIN resuelve 22 destinos en BETA. Incluye `sts2`, BaseLib,
  RitsuLib, FGOCore, Harmony y Godot, además de los contratos de reflexión conocidos.
- **Validación local:** 26/26 builds con cero errores/advertencias; paridad 13×5, SimpleLoc,
  288 VFX, 867 cartas, 257 poderes y 124 reliquias verdes. Se regeneraron y abrieron los 13 PCK:
  manifiestos idénticos, localización nueva presente y cero DLL internas.
- **Workshop publicado:** SteamCMD actualizó los 13 items públicos existentes en una sola sesión y
  confirmó 13/13 commits exitosos. Se conservaron todos los IDs; Kagetora, Shuten y Astolfo también
  actualizaron sus previews desde los fondos oficiales configurados. No se realizó instalación
  local, por lo que no existe riesgo de IDs duplicados con las suscripciones de Workshop.

## 2026-08-01 — compatibilidad completa con reliquias Ancient preparada

- **Sea Glass / Orobas:** el título vanilla concatena el ID del personaje y sólo existen claves
  para los cinco personajes base. FGOCore detecta los doce prefijos FGO y usa el título genérico
  ya localizado, evitando la `LocException` que bloqueaba la opción de Orobas.
- **Archaic Tooth 12/12:** una carta firma de cada mazo inicial implementa ahora el contrato
  `ITranscendenceCard` de BaseLib y se transforma en una carta temática existente del mismo
  personaje. No se renombró ningún ID ni se alteró la composición de los mazos guardados.
- **Yummy Cookie 12/12:** RitsuLib registra una visual por personaje para la reliquia vanilla. Se
  reutiliza el icono completo de su starter identitaria (normal, contorno y grande), eliminando el
  fallback visual de Ironclad sin agregar recursos duplicados.
- **Versiones preparadas:** FGOCore `v0.1.18`; Mash `v0.1.16`; Morgan, Artoria, Mordred, Gilgamesh,
  Okita, Oberon y Tiamat `v0.1.15`; Siegfried `v0.1.17`; Kagetora `v0.1.8`; Shuten `v0.1.7` y
  Astolfo `v0.1.6`. Los personajes exigen FGOCore `v0.1.18`.
- **Validación local:** matriz MAIN/BETA 26/26 y tres sondas verdes (1.284 referencias del juego),
  incluida la carga del artefacto MAIN sobre BETA. Paridad 13×5, SimpleLoc, 288 VFX, 629
  identidades, 867 cartas, 257 poderes, 124 reliquias, 15 perfiles animados y presentación 12/12
  sin errores. Los 13 PCK contienen el manifiesto staged exacto y cero DLL internas.
- **Pendiente externo:** no se instaló ni se subió nada a Steam; el lote queda preparado para
  publicación conjunta después del playtest real de las tres reliquias.

## 2026-08-01 — hotfix global de Touch of Orobas preparado

- **Causa verificada:** Orobas refina la primera reliquia `Starter`; su tabla vanilla cae a
  `Circlet` para IDs desconocidos. BaseLib sólo evita esa caída cuando el `CustomRelicModel`
  sobrescribe `GetUpgradeReplacement()`, cosa que ningún starter FGO hacía.
- **Cobertura 12/12:** cada starter mecánica ahora declara su Ancient correspondiente. Gilgamesh
  coloca Bab-ilu primero y la refina en **Ea, la Espada de la Ruptura**; Tiamat incorpora
  **Mar de Vida: Génesis**. Ambas tienen iconos y localización en cinco idiomas.
- **Reemplazo completo:** Morgan, Artoria, Okita, Oberon, Siegfried y Kagetora reinstalan la forma,
  los contadores o el motor que antes sólo sembraba la starter eliminada. Siegfried y Tiamat
  conservan `NpLevel` y `DupePity` durante el reemplazo.
- **Protección compartida:** FGOCore evita que una futura starter FGO olvidada se convierta en
  Circlet y recalcula elecciones de Orobas ya preparadas por una versión anterior. La sonda de
  compatibilidad exige en adelante los doce mapeos.
- **Versiones preparadas:** FGOCore `v0.1.17`; Mash `v0.1.15`; Morgan, Artoria, Mordred, Gilgamesh,
  Okita, Oberon y Tiamat `v0.1.14`; Siegfried `v0.1.16`; Kagetora `v0.1.7`; Shuten `v0.1.6` y
  Astolfo `v0.1.5`. Los personajes exigen FGOCore `v0.1.17`.
- **Validación local:** matriz MAIN/BETA 26/26 y tres sondas verdes (1.283 referencias del juego);
  paridad 13×5, SimpleLoc, 288 VFX, 629 identidades, 124 reliquias, 15 perfiles animados sin
  errores, presentación 12/12, calidad visual y fichas de Workshop aprobadas. Los 13 PCK en
  `dist/` contienen manifiestos idénticos,
  cinco idiomas y ningún DLL interno; Ea y Génesis incluyen sus texturas importadas.
- **Pendiente externo:** no se instaló ni se subió nada a Steam. Falta un playtest real de Orobas
  y, cuando el usuario lo pida explícitamente, publicar el lote completo en Workshop.

## 2026-08-01 — hotfix de carga RitsuLib, energía de Kagetora y BaseLib 3.4.3 publicado

- **Reporte externo reproducido:** MAIN `0.107.1` con BaseLib `3.4.3`, RitsuLib `0.5.4`, FGOCore
  `v0.1.15` y Kagetora `v0.1.5` fallaba en tres puntos independientes: el inicializador de FGOCore
  rechazaba los IDs generados por RitsuLib, BaseLib llamaba una firma BETA de
  `StartRunLobby.LocalPlayer`, y Kagetora pedía dos texturas de energía ausentes.
- **RitsuLib corregido:** el normalizador oficial separa `FGOCore` como `FGO_CORE`; los IDs estables
  reales son `FGO_CORE_CARDTAG_COMMAND_{BUSTER,ARTS,QUICK}` y
  `FGO_CORE_MODELCAPABILITY_COMMAND_TAG`. La sonda ahora pregunta al generador de RitsuLib 0.5.4 y
  compara su salida con los cuatro contratos fijos, evitando el falso verde anterior.
- **Kagetora corregida:** sus pools ya no fuerzan `charui/big_energy.png` ni
  `charui/text_energy.png`; vuelven al fallback nulo seguro de BaseLib, igual que Shuten y Astolfo.
  El auditor de assets valida en adelante cualquier override explícito de iconos de energía.
- **BaseLib 3.4.3 protegido en MAIN:** esa DLL fue compilada con el retorno BETA
  `StartRunLobbyPlayer`, mientras MAIN devuelve `LobbyPlayer`. Un finalizer de FGOCore neutraliza
  exclusivamente la `MissingMethodException` de
  `CharacterSelectStartingRelicsPatch.OnEmbarkPressedPostfix`; cualquier excepción distinta se
  sigue propagando. La sonda reprodujo el fallo contra la DLL de Workshop y validó el filtro.
- **Versiones publicadas:** FGOCore `v0.1.16`; Mash `v0.1.14`; Morgan, Artoria, Mordred, Gilgamesh,
  Okita, Oberon y Tiamat `v0.1.13`; Siegfried `v0.1.15`; Kagetora `v0.1.6`; Shuten `v0.1.5` y
  Astolfo `v0.1.4`. Los 13 manifiestos exigen RitsuLib `v0.5.4`; los personajes exigen FGOCore
  `v0.1.16`.
- **Validación local completa:** matriz MAIN/BETA 26/26 con 0 errores y 0 advertencias; tres sondas
  con 1.277 referencias del juego y RitsuLib 0.5.4 en los 13 DLL; reproducción adicional de BaseLib
  3.4.3 en MAIN; localización 13×5, SimpleLoc, 288 referencias VFX, 629 identidades de carta,
  contextos, assets, 15 perfiles animados, presentación 12/12 y 3.324 fotogramas HD aprobados. Los
  13 paquetes de `dist/` tienen manifiesto interno idéntico por SHA-256, cero DLL dentro del PCK y
  Kagetora no contiene los recursos de energía inválidos.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` 13/13 sobre los ítems
  públicos existentes. Las tres previews nuevas se enviaron sólo para Kagetora, Shuten y Astolfo.
  Una consulta posterior por Steamworks dejó los 13 ítems en `OK`: FGOCore requiere BaseLib y
  RitsuLib; cada personaje requiere BaseLib, RitsuLib y FGOCore (38 vínculos en total).
- **Pendiente externo:** no se instaló ninguna copia local. Antes de declararlo validado en juego
  falta que Steam sincronice el lote y un playtest real del inicio de partida y de Embark en MAIN
  con BaseLib 3.4.3 + RitsuLib 0.5.4.

## 2026-08-01 — BaseLib 3.4.1 + integración transversal RitsuLib 0.5.3 completada

- **Contrato actualizado:** los 13 proyectos compilan contra BaseLib `3.4.0` y exigen runtime
  `v3.4.1`; además compilan contra RitsuLib `0.5.3` (`Compat.0.107.1` en MAIN, paquete regular en
  BETA). Los 13 manifiestos declaran `STS2-RitsuLib >= v0.5.3`; los personajes también exigen
  FGOCore `v0.1.15`.
- **Interoperabilidad de comandos:** FGOCore registra IDs estables para Buster, Arts y Quick y una
  capacidad de modelo de RitsuLib los agrega a toda carta `ICommandTyped`, incluidas copias y
  transformaciones. El lifecycle audit valida exactamente un tag correcto por carta.
- **Integración de los 12 personajes:** cada DLL referencia RitsuLib directamente, usa su factory de
  logger y registra el mod en `FgoRitsuIntegration`. La matriz falla si un artefacto queda fuera.
- **Telemetry abandonado:** se eliminó `FGOTelemetry` del código, build, instalador y release graph.
  La nueva ruta no captura historial, no pide consentimiento y no persiste lotes JSON.
- **Mejoras BaseLib:** `ICommandTyped` implementa `ICustomTypeTextCard`; Buster, Arts y Quick aparecen
  en la placa de tipo, localizados en los cinco idiomas. BaseLib 3.4.1 también corrige `%FormVfx`.
- **Versiones preparadas:** FGOCore `v0.1.15`; Mash `v0.1.13`; Morgan, Artoria, Mordred, Gilgamesh,
  Okita, Oberon y Tiamat `v0.1.12`; Siegfried `v0.1.14`; Kagetora `v0.1.5`; Shuten `v0.1.4`;
  Astolfo `v0.1.3`. El lote completo debe publicarse unido.
- **Validación completa:** matriz MAIN/BETA 26/26 con 0 errores y 0 advertencias; probes MAIN→MAIN,
  MAIN→BETA y BETA→BETA con las 13 referencias directas a RitsuLib; auditorías de manifests,
  Workshop, localización, SimpleLoc, VFX, identidad de cartas, contextos de elección, assets,
  presentación, calidad visual y animaciones aprobadas. Los 13 PCK fueron publicados a `dist/` e
  inspeccionados: manifiestos internos idénticos a los externos y ningún DLL de BaseLib/RitsuLib
  empaquetado.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` 13/13 sobre los ítems
  públicos existentes. Kagetora, Shuten y Astolfo usan ahora como `mod_image` y preview la misma
  ilustración oficial de su fondo de selección; las otras diez previews se preservaron. Las tres
  imágenes quedaron por debajo de 1 MB, Steam confirmó tres `Uploading preview image...` y
  `stderr.txt` quedó vacío.
- **Required Items sincronizados:** la API oficial de Steamworks confirmó 38 vínculos en total.
  FGOCore requiere BaseLib `3737335127` y RitsuLib `3747602295`; cada uno de los 12 personajes
  requiere esos dos ítems y FGOCore `3747876334`. Se agregaron únicamente los 10 vínculos que
  faltaban y una segunda consulta independiente dejó los 13 ítems en `OK`. La herramienta
  idempotente queda en `tools/workshop_dependencies/` para futuras verificaciones.
- **Pendiente externo:** no se instaló ninguna copia local. Falta que Steam sincronice el lote y
  realizar el playtest de carga con BaseLib 3.4.1 + RitsuLib 0.5.3, además de validar visualmente las
  tres previews en la interfaz real de Workshop.

## 2026-08-01 — calidad visual adaptativa extendida a los 12 personajes

- **Cobertura completa:** los 12 mods disponen ahora de una variante activable de combate a
  1024 px, además del fallback previo de 768 px. Son 21 recursos animados y 3.324 fotogramas WebP;
  cada variante reutiliza exactamente el render fuente y compensa su escala para no cambiar el
  tamaño del personaje en pantalla.
- **Modelos de una sola forma:** Astolfo, Gilgamesh, Shuten y Siegfried entraron en la misma ruta
  asíncrona de calidad que los personajes con transformaciones. Gilgamesh conserva sus 867 px
  nativos mediante un factor propio; Vortigern mantiene su imagen estática a resolución fuente.
- **Configuración común:** FGOCore `v0.1.14` conserva `Automática`, `Equilibrada` y `Alta`, junto con
  el suavizado opcional. No precarga personajes ajenos: sólo el modelo activo y, en solitario, sus
  formas alternativas. En cooperativo evita esa precarga adicional y mantiene el fallback seguro.
- **Versiones preparadas:** Mash `v0.1.12`; Morgan, Artoria, Mordred, Gilgamesh, Okita, Oberon y
  Tiamat `v0.1.11`; Siegfried `v0.1.13`; Kagetora `v0.1.4`; Shuten `v0.1.3`; Astolfo `v0.1.2`.
  Los 12 manifiestos exigen FGOCore `v0.1.14`, por lo que el lote debe publicarse unido.
- **Paquetes inspeccionados:** los 13 PCK contienen su manifiesto correcto, cinco idiomas y las
  escenas de tienda/fogata de cada personaje. Los imports HD presentes en cada PCK coinciden con
  su inventario esperado. SteamCMD confirmó el 2026-08-01 las 13 actualizaciones en los ítems
  públicos existentes (`Committing update...Success` 13/13). No se instaló localmente; queda
  pendiente reiniciar/sincronizar Steam y hacer un playtest visual real.
- **Validación:** matriz MAIN/BETA 26/26 con 0 errores y 0 advertencias; las tres sondas enlazaron
  1.288 referencias del juego. Las auditorías de calidad HD, animaciones, presentación, assets,
  VFX, localización, SimpleLoc y fichas de Workshop aprobaron. El auditor de fichas ahora controla
  el límite real de 8.000 bytes UTF-8 de Steam, después de detectar y corregir FGOCore antes del lote.

## 2026-08-01 — Kagetora jugable y calidad visual adaptativa preparada

- **Hotfix público:** Kagetora `v0.1.2` fue publicado en su item existente de Workshop
  (`3773261707`) y sincronizado con `main`. El diálogo Ancient propio evita que Neow quede sin
  opciones de reliquia inicial en MAIN y BETA. Falta confirmar el flujo completo en un playtest
  real después de que Steam descargue la actualización.
- **Configuración común:** FGOCore `v0.1.13` agrega, mediante BaseLib, opciones de calidad de
  modelos (`Automática`, `Equilibrada`, `Alta`) y suavizado (`Desactivado`, `Estándar`, `Mejorado`),
  localizadas en los cinco idiomas. No se agregó RitsuLib como dependencia.
- **Selección segura:** en Windows, `Automática` identifica el adaptador activo y lee su VRAM
  dedicada registrada. Sólo elige Alta en solitario con GPU dedicada de al menos 8 GiB y 3 GiB de
  margen estimado; en cooperativo, hardware desconocido o recursos HD ausentes vuelve a 768 px.
  La elección queda fija durante cada combate y las cargas siguen siendo asíncronas.
- **Piloto Kagetora `v0.1.3`:** sus formas Nagao Kagetora y Uesugi Kenshin incluyen recursos de
  1024 px con escala compensada, además del fallback de 768 px. Los 306 WebP HD conservan las
  fuentes originales, tienen imports únicos y el PCK final queda en 68,7 MB.
- **Validación:** matriz MAIN/BETA 26/26 con 0 errores y 0 advertencias; las tres sondas resolvieron
  1.286 referencias. Paridad de localización, SimpleLoc, presentación tienda/descanso, assets,
  descripciones y animaciones aprobaron. FGOCore/Kagetora `v0.1.13`/`v0.1.3` aún no se publicaron:
  falta el playtest visual y una orden explícita de upload.

## 2026-07-31 — Kagetora v0.1.2: hotfix de Neow preparado

- **Reporte externo:** al iniciar una partida con Kagetora, Neow podía quedar mostrando el diálogo
  sin habilitar las opciones de reliquia; el usuario confirmó el mismo síntoma en MAIN y BETA con
  sólo BaseLib, FGOCore y Kagetora.
- **Mitigación dirigida:** el flujo de `NEventRoom.SetupLayout` sólo agrega y habilita las opciones
  después de resolver un diálogo Ancient válido. Kagetora dependía del diálogo genérico `ANY`; ahora
  aporta un diálogo propio de Neow, de una sola línea y repetible, en los cinco idiomas. BaseLib
  obtiene así una secuencia no vacía para la primera visita y todas las posteriores, y la misma línea
  habilita las opciones sin requerir un clic intermedio.
- **Paquete v0.1.2:** se regeneraron DLL, manifest y PCK. El manifest interno y externo marca
  `v0.1.2`; el PCK contiene las cinco claves de Neow y sus archivos coinciden por SHA-256 con las
  fuentes.
- **Validación:** builds MAIN/BETA con 0 errores y 0 advertencias; sondas MAIN→MAIN y MAIN→BETA
  resolvieron 255 referencias de juego entre FGOCore y Kagetora. Paridad de localización, SimpleLoc,
  ficha de Workshop y `git diff --check` aprobados.
- **Publicación posterior:** el hotfix fue publicado el 2026-08-01. Falta el playtest real del
  inicio de partida; no se instaló una copia local ni se modificaron las suscripciones.

## 2026-07-31 — publicación global MAIN/BETA y hotfixes

- **Lote público completo:** SteamCMD confirmó `Committing update...Success` 13 veces en una sola
  sesión para FGOCore y los 12 personajes, reutilizando sus IDs existentes y visibilidad pública.
- **Versiones nuevas:** FGOCore y Siegfried quedaron en `v0.1.12`; Shuten subió a `v0.1.2` con el
  puente de `CreatureCmd.Damage` requerido por BETA v0.110.1. Los otros diez paquetes se
  reconstruyeron y republicaron para mantener el conjunto alineado.
- **Paquetes inspeccionados:** los 39 DLL/JSON/PCK de `.workshop_stage` coinciden por SHA-256 con
  `dist`. Los 13 PCK contienen un manifiesto idéntico al externo, los cinco idiomas y, para cada
  personaje, las escenas compiladas de tienda y descanso. Se corrigieron los filtros de exportación
  que excluían por error los manifiestos internos de Shuten y Astolfo.
- **Validación previa:** matriz MAIN/BETA 26/26 sin errores ni advertencias; sondas MAIN→MAIN,
  MAIN→BETA y BETA→BETA sobre los 13 DLL, paridad de localización, SimpleLoc, assets, VFX,
  presentación, animaciones, contextos y las 13 fichas de Workshop aprobados.
- **Sin instalación duplicada:** no se copiaron mods FGO a la carpeta local del juego. Falta que
  Steam sincronice los nuevos paquetes y realizar el playtest de Siegfried y `PoisonedBanquet` en
  BETA v0.110.1.

## 2026-07-31 — compatibilidad BETA v0.110.1

- **Nueva referencia verificada:** la rama `public-beta` instalada corresponde al build Steam
  `24489008`, versión `v0.110.1` y commit del juego `db5d3552`.
- **Ruptura latente corregida:** `PoisonedBanquet` de Shuten todavía enlazaba la sobrecarga MAIN de
  seis parámetros de `CreatureCmd.Damage`, eliminada en BETA. La carta pasa ahora por
  `CreatureCmdCompatibility` y conserva la atribución nula de daño que ya tenía.
- **Matriz completa endurecida:** 26/26 builds MAIN/BETA con 0 errores y 0 advertencias. Las sondas
  MAIN→MAIN, artefactos MAIN→BETA y BETA→BETA resolvieron 1.286 referencias a `sts2` en los 13 DLL;
  la auditoría de contexto terminó con 0 hallazgos.
- **Preflight y configuración corregidos:** la matriz valida ambas fixtures completas antes de
  compilar, deriva las versiones desde `Sts2Compatibility.props` y reconoce que sólo
  `Sentry.Godot.dll` es una referencia nueva de 0.110.1. Las cuatro rutas absolutas versionadas se
  retiraron del índice y permanecen como configuración local ignorada.
- **Release publicado:** `ShutenDouji` subió a `v0.1.2`; su DLL universal y PCK fueron regenerados y
  publicados dentro del lote global. Queda pendiente el playtest real de `PoisonedBanquet` dentro
  de BETA v0.110.1.

## 2026-07-31 — Mash v0.1.11 publicada

- **Cobertura endurecida:** la previsualización de daño ya no muta el objetivo/monto pendiente; el
  camino real usa `BeforeDamageReceived`, confirmación posterior y stacks por resolución para
  soportar daño reentrante y varias Mash sin omitir ni duplicar transferencias.
- **Duración cooperativa correcta:** Cobertura, Provocación y Pared Absoluta expiran al terminar el
  lado enemigo, no por un turno extra de otro jugador.
- **Paquete verificado:** el manifiesto `v0.1.11` está dentro y fuera del PCK; sus 1.709 entradas
  incluyen combate, tienda, fogata y las tres formas. DLL/JSON/PCK coinciden por SHA-256 entre
  `dist` y `.workshop_stage`.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` para MashShielder
  (`3747876464`), conservando visibilidad pública (`0`) y la preview existente; `stderr` quedó vacío.
- **Pendiente externo:** reiniciar Steam para forzar la descarga y probar Cobertura en cooperativo,
  especialmente con turnos extra y dos Mash.

## 2026-07-30 — paquete final publicado y hotfix de costes

- **Pagos gratuitos válidos:** FGOCore acepta correctamente costes de 0 NP y 0 Estrellas tras
  reducciones repetidas, pero sigue rechazando valores negativos. La compatibilidad con
  `Infinite Upgrades` mantiene Ráfaga con suelo 1 para que su cadena no se vuelva gratuita.
- **Workshop actualizado en un solo lote:** SteamCMD confirmó `Committing update...Success` para
  FGOCore y los 12 personajes, todos sobre sus 13 IDs existentes y con visibilidad pública (`0`).
  Los 39 DLL/JSON/PCK preparados coinciden por SHA-256 con `dist`; la auditoría de las 13 fichas
  también terminó correctamente.
- **Sin instalación duplicada:** no se copió ningún mod FGO a la carpeta local del juego.
- **Pendiente externo:** reiniciar Steam para forzar la sincronización y hacer el playtest visual y
  funcional dentro del juego.

## 2026-07-30 — tienda/fogata propias y suavizado visual global

- **Fallback del Guerrero eliminado:** Mordred, Gilgamesh, Okita, Oberon, Siegfried y Tiamat ahora
  sobrescriben `CustomMerchantAnimPath` y `CustomRestSiteAnimPath`. Los primeros cinco ya tenían
  escenas desconectadas; Tiamat recibió ambas escenas nuevas. Los 12 personajes FGO pasan la nueva
  auditoría de presentación con modelo propio en tienda y fogata.
- **Reposo animado 12/12:** las diez escenas estáticas de esos cinco personajes pasaron a sus
  `SpriteFrames` oficiales y Tiamat usa su forma Femme Fatale. Las 24 escenas de tienda/descanso
  reproducen `idle` y las 12 fogatas conservan `ControlRoot` + `%Hitbox`.
- **Suavizado compartido:** FGOCore añade una capa visual para todo `AnimatedSprite2D` FGO: mezcla
  tenue y corta del fotograma anterior más interpolación subpíxel de respiración/anticipación. No
  cambia FPS, duración de clips ni esperas del combate, y usa `Offset`, separado de los pivotes que
  actualiza `FormVisuals` al cambiar de forma.
- **Validación:** 26/26 builds MAIN/BETA con 0 errores y 0 advertencias; sondas MAIN→MAIN,
  MAIN→BETA y BETA→BETA correctas. Auditoría de 15 formas aprobada con 0 errores (48 advertencias
  conocidas), presentación 12/12 aprobada y los seis PCK afectados contienen sus cuatro entradas
  compiladas de tienda/descanso.
- **Pendiente externo:** playtest visual dentro del juego para confirmar encuadre, orientación y
  percepción del suavizado en combate, tienda y fogata. No se instaló ninguna copia local; el lote
  final se publicó en el cierre descrito arriba.

## 2026-07-30 — hotfix 0.1.12 de Sangre de Dragón de Siegfried

- **Causa confirmada:** la Hoja de Tilo anulaba toda la Sangre de Dragón en el primer ataque que
  alcanzaba al personaje cada turno. Contra enemigos que atacan una vez, la pasiva parecía no hacer
  nada aunque el contador se estuviera acumulando.
- **Regla corregida:** el primer ataque sólo ignora 1 Escama; las restantes todavía reducen ese golpe
  y los ataques posteriores usan el valor completo. El icono de Sangre de Dragón ahora parpadea en
  la resolución real cuando modifica el daño, sin efectos secundarios en previews.
- **Texto transparente:** las cinco traducciones de la Hoja de Tilo aclaran la excepción, que el +5 NP
  corresponde a ataques posteriores y que su contador visible es el nivel NP (+100 de Carga máxima
  y +15% de daño NP por nivel). Las fichas de Workshop y el documento de diseño usan la misma regla.
- **Paquetes preparados:** FGOCore v0.1.12 y SiegfriedSaber v0.1.12 están en `dist`; Siegfried exige
  Core 0.1.12. Los PCK montados contienen manifiestos y traducciones correctos.
- **Validación:** 26 builds MAIN/BETA sin errores ni advertencias, sondas MAIN→MAIN, MAIN→BETA y
  BETA→BETA correctas; paridad de cinco idiomas, SimpleLoc y las 13 fichas de Workshop aprobadas.
- **Workshop actualizado:** FGOCore y Siegfried `v0.1.12` se publicaron juntos dentro del lote global.
  Queda pendiente confirmar el comportamiento en juego después de que Steam sincronice el contenido.

## 2026-07-30 — hotfix de animación y texto de invocación de Kagetora/Kenshin

- **Animación reparada:** Kenshin deja de usar el recorte aéreo de `attack_q`; su ataque combina
  los cuadros seguros 0–20 y 56–70 de `attack_a`, evitando el salto de posición y el tramo donde el
  rig oficial se separa. Son 36 cuadros y el auditor termina con 0 errores.
- **Texto de duplicado compartido:** FGOCore incorpora `OPTION_*_DUPE.name` para los siete personajes
  que no tenían la clave, con traducciones `eng`, `esp`, `zhs`, `kor` y `rus`. También se reemplazaron
  los marcadores de Doctrina que SimpleLoc interpretaba como BBCode inválido.
- **Paquetes verificados y publicados:** los PCK montados contienen las cinco traducciones y los 36
  cuadros de ataque. SteamCMD confirmó dos actualizaciones con `Committing update...Success` para
  FGOCore (`3747876334`) y KagetoraLancer (`3773261707`), conservando visibilidad pública. Los seis
  DLL/JSON/PCK de staging coinciden por SHA-256 con `dist` y `stderr` quedó vacío.
- **Pendiente externo:** reiniciar Steam para forzar la sincronización y comprobar ambos arreglos en
  una partida nueva o recargada.

## 2026-07-30 — compatibilidad global con mejoras infinitas

- **FGOCore v0.1.11 preparado:** se verificó `Infinite Upgrades` v1.0.0 instalado. El mod eleva
  `MaxUpgradeLevel` a `int.MaxValue` y vuelve a ejecutar la mejora normal; no requiere una API propia.
- **Escalado compartido:** la primera mejora no cambia. Desde la segunda, Poderes y Habilidades con
  Agotar pueden bajar hasta 0 de Energía; las Habilidades reutilizables conservan un suelo de 1 y
  los Ataques no acumulan rebajas de coste más allá de la mejora diseñada.
- **Topes de seguridad:** costes de NP, Estrellas, Sake y Deuda, además del autodaño, no bajan de 0;
  Ráfaga conserva un suelo de 1, y divisores y turnos tampoco bajan de 1. Esto evita pagos negativos,
  división por cero y cadenas gratuitas deterministas sin quitar el escalado numérico normal.
- **Validación:** matriz completa de 26 builds sin errores ni advertencias; sondas MAIN→MAIN,
  MAIN→BETA y BETA→BETA correctas; 25 reductores auditados sin suelos faltantes y las 13 fichas
  de Workshop coherentes. El paquete MAIN final contiene el manifest v0.1.11 y los cinco idiomas.
- **Workshop actualizado:** SteamCMD confirmó `Committing update...Success` para FGOCore
  (`3747876334`) con visibilidad pública. Los DLL/JSON/PCK enviados coinciden por SHA-256 con el
  paquete v0.1.11 validado.
- **Pendiente externo:** reiniciar Steam para descargar v0.1.11 y hacer el playtest real con
  `Infinite Upgrades` activo.

## 2026-07-29 — hotfix de inicialización de Kagetora, Shuten y Astolfo

- **Causa confirmada en el log:** los tres inicializadores intentaban obtener su personaje desde
  `ModelDb` antes de que BaseLib terminara de registrarlo, provocando `KeyNotFoundException` para
  `CHARACTER.KAGETORALANCER-KAGETORA`, `CHARACTER.SHUTENDOUJI-SHUTEN` y
  `CHARACTER.ASTOLFORIDER-ASTOLFO`.
- **Hotfix v0.1.1 preparado:** el atributo Tierra se registra con el identificador estable calculado
  por `ModelDb.GetId<T>()`, sin consultar prematuramente el diccionario de modelos. FGOCore no cambia.
- **Validación y publicación:** compilaciones Release limpias y matriz MAIN/BETA completa; las sondas
  MAIN→MAIN, MAIN→BETA y BETA→BETA pasan. SteamCMD confirmó las tres actualizaciones con
  `Committing update...Success`, conservando visibilidad pública. Falta confirmar el arranque real
  después de que Steam descargue v0.1.1.

## 2026-07-29 — publicación pública de los 13 mods FGO

- **Workshop actualizado en un solo lote:** SteamCMD confirmó `Committing update...Success` 13 veces
  con visibilidad pública (`0`). Se actualizaron los once ítems existentes y se crearon
  `ShutenDouji` (`3774222164`) y `AstolfoRider` (`3774222236`). No se instaló ninguna copia local.
- **Descripciones y traducciones cerradas:** las 13 fichas de Workshop usan BBCode y contienen la
  versión completa en inglés, español y chino simplificado. La auditoría editorial cubre 68.261
  caracteres. Los 13 proyectos mantienen paridad de archivos, claves y variables en `eng`, `esp`,
  `zhs`, `kor` y `rus`; SimpleLoc informa 0 ambigüedades.
- **Calidad previa a publicación:** cobertura de 867 cartas, 257 powers y 122 reliquias, 288
  referencias VFX válidas y 15 formas animadas con 0 errores. Okita ya no usa los fotogramas con el
  brazo separado: daño reproduce los dos cuadros íntegros y derrota queda en la pose limpia final.
- **Binarios universales verificados:** 26/26 builds Release para MAIN v0.107.1 y BETA v0.109.0,
  sondas MAIN→MAIN, MAIN→BETA y BETA→BETA correctas. Los 13 PCK contienen sus recursos y los cinco
  idiomas; los 39 DLL/JSON/PCK coinciden por SHA-256 entre `dist` y `.workshop_stage`, y el `stderr`
  de SteamCMD quedó vacío.
- **Pendiente externo:** playtest dentro del juego de guardado/carga, cooperativo y balance de
  Kagetora, Shuten y Astolfo. La publicación pública ya está completada.

## 2026-07-29 — endurecimiento global y cierre visual de Kagetora

- **FGOCore v0.1.10:** `FgoCombatState` guarda flags y contadores efímeros en powers ocultos
  sincronizados. Los 12 personajes migraron sus usos «una vez por turno/combate» y configuraciones
  de carta para que guardar/cargar o reconstruir modelos no reactive beneficios.
- **Hooks de preview puros:** Shuten ya no consume contadores desde `ModifyDamageAdditive`; el gasto
  ocurre únicamente después del daño confirmado. Sus dos VFX inexistentes fueron reemplazados por
  una ruta real del juego y ahora `tools/audit_vfx_paths.ps1` evita regresiones.
- **Kagetora visualmente completa:** 79 cartas, 25 powers y 12 reliquias usan arte oficial curado;
  también tiene icono, marcador, selector bloqueado, fondo de selección, portada, mercader y fogata
  propios. Los mappings reproducibles viven en `assets/reference/ce/` e `assets/reference/icons/`.
- **Cobertura global de recursos:** `tools/audit_asset_coverage.ps1` comprueba 12 personajes, 867
  cartas, 257 powers y 122 reliquias. Astolfo y Shuten recibieron fallback propio y se eliminó el
  texto huérfano de `BondPower` en Mash.
- **Fichas de Workshop unificadas:** las 13 descripciones usan BBCode de Steam, promesa jugable y
  contenido cuantificado con inglés primero y versiones completas en español y chino simplificado.
  Dependencias, ramas, versiones y estado de playtest reflejan el repositorio actual;
  `tools/audit_workshop_descriptions.ps1` deja este contrato automatizado. Steam no se modificó.
- **Localización protegida:** paridad estricta de archivos, claves y `!Variables!` en `eng`, `esp`,
  `zhs`, `kor` y `rus`, además de SimpleLoc, para los 13 proyectos.
- **Validación de cierre:** 13 builds Release sin warnings; matriz MAIN/BETA completa y sondas
  MAIN→MAIN, MAIN→BETA y BETA→BETA correctas; contexto cooperativo 0 hallazgos; 15 formas animadas
  con 0 errores; PCK de Kagetora auditado con 1.162 entradas y optimizado de 99,3 a 39,5 MB.
- **Pendiente externo:** playtest dentro del juego, guardado/carga y cooperativo. Este trabajo no
  instala mods ni modifica Steam Workshop.

## 2026-07-29 — Astolfo Rider implementado y empaquetado

- **Personaje completo en staging:** `AstolfoRider` v0.1.0 implementa la bolsa visible de Caprichos
  Q/A/B, Críticos v2, Evasión, Derribo, mazo inicial de 10, 68 recompensas exactas (20/28/20), el NP
  Quick `Hippogriff`, 35 powers y 12 reliquias.
- **Persistencia y claridad:** bolsa, Capricho actual/anterior, usos una vez por turno y primer turno
  viven en Powers guardables; también persiste el tipo de Comando que una carta pueda cambiar durante
  la partida. Los Caprichos elegidos al inicio ya no se descartan antes de la mano y el icono visible
  identifica por texto Quick, Arts o Buster además del color.
- **FGOCore v0.1.9 en ese cierre:** Evasión es una mecánica compartida con máximo tres cargas; se consume por cada
  impacto de Ataque enemigo que alcanzaría PV después de Bloqueo y Buffer. Capricho y Derribo quedan
  locales a Astolfo.
- **Producción visual oficial:** modelo FGO `400400`, selector, marcador, portada, mercader y descanso;
  180 cuadros de idle/ataque Quick/casteo/daño, 80 modelos de carta con retratos oficiales y arte
  propio para los 35 powers y las 12 reliquias. La auditoría de animación pasa con 0 errores y un
  aviso esperado por la lanza saliendo del encuadre, sin cortar el cuerpo.
- **Localización:** inglés, español y chino simplificado están editados por completo; coreano y ruso
  mantienen paridad exacta con fallback inglés seguro. Claves, variables dinámicas y SimpleLoc pasan
  con 0 hallazgos.
- **Validación:** los 13 proyectos compilan sin errores ni advertencias contra MAIN v0.107.1 y BETA
  v0.109.0; las sondas runtime MAIN→MAIN, MAIN→BETA y BETA→BETA pasan. El PCK final optimizado pesa
  30.868.616 bytes.
- **Workshop preparado:** descripción editorial en español, inglés y chino, título y staging local
  validados para `AstolfoRider`. No hubo conexión con Steam.
- **Pendiente externo:** playtest dentro del juego, en especial guardado/carga, cooperativo y ajuste
  de balance. No se instaló localmente ni se creó/publicó un ítem de Workshop.
  Documento canónico: [`DESIGN-ASTOLFO.md`](DESIGN-ASTOLFO.md).

## 2026-07-29 — Shuten Dōji implementada y empaquetada

- **Personaje completo en staging:** `ShutenDouji` implementa Sake 0–100, Estilos Assassin/Caster,
  Cross, 68 cartas de recompensa (20/28/20), cinco cartas iniciales propias, dos NP excluyentes,
  37 powers y 12 reliquias. Los usos por turno críticos persisten mediante markers ocultos, por lo
  que guardar/cargar no vuelve a habilitar Arts, Buster ni Romper Protección Divina.
- **FGOCore compartido:** Sello de Habilidad y Certero/Sure Hit viven en el núcleo; Tiamat conserva
  sus IDs publicados y delega al comportamiento común. El lote entero se recompiló para evitar
  incompatibilidades binarias.
- **Producción visual:** el modelo oficial correcto de Assassin es `602100` (collection 112), no
  `602500`/`602510`; se integraron 193 cuadros de idle, ataque Quick, casteo, daño y muerte. La
  auditoría termina con 0 errores y dos avisos de props que rozan el borde sin cortar el cuerpo.
- **Arte e interfaz:** 80 pares de retratos 500×380/1000×760, 37 iconos de poder, 12 reliquias,
  Command Cards oficiales, selector, icono, marcador de mapa, portada, mercader y descanso. El mapeo
  semántico y la procedencia de Atlas Academy son reproducibles desde `assets/reference/` y `tools/`.
- **Localización:** inglés, español y chino simplificado están editados por completo; `kor` y `rus`
  mantienen paridad exacta de claves y variables con fallback inglés seguro. SimpleLoc informa 0
  ambigüedades.
- **Validación:** los 12 proyectos pasan MAIN; los 12 pasan BETA y las sondas runtime MAIN→MAIN,
  MAIN→BETA y BETA→BETA son correctas. La matriz ampliada también detectó y corrigió dos llamadas
  antiguas de Kagetora incompatibles con BETA. El PCK optimizado pesa 30.430.216 bytes frente a
  87.226.952 sin compresión de las nuevas ilustraciones.
- **Workshop preparado:** descripción editorial propia en español, inglés y chino, objetivo
  `ShutenDouji` registrado y staging verificado con visibilidad privada por defecto. No se creó ni
  actualizó ningún item de Steam.
- **Pendiente externo:** playtest dentro del juego (incluidos guardado/carga y cooperativo) y decidir
  publicación en Workshop. No se instaló localmente para no duplicar el `FGOCore` suscrito y Steam no
  se modificó en este cierre.

## 2026-07-28 — diseño cerrado de Shuten Dōji

- **Un personaje híbrido, no dos mods:** se reservó `ShutenDouji`; Assassin y Caster son Estilos de
  carta sin formas mecánicas ni bonificaciones pasivas de clase.
- **Motor cerrado:** Veneno nativo + Sake personal 0–100 + condición Cruce. La reliquia inicial da
  20 Sake al entrar y 10 tras la primera carta de cada Estilo por turno (máx. 2); una build pura
  funciona y la híbrida obtiene mayor eficiencia sin ser obligatoria.
- **Clímax:** al cruzar 100 NP se manifiestan dos cartas Event retenibles y mutuamente excluyentes:
  `千紫万紅・神便鬼毒` de área/control y `護法少女・九頭竜鏖殺` individual/multiimpacto.
- **Contenido diseñado:** 68 recompensas (20/28/20, exactamente 34 Assassin y 34 Caster), cinco
  cartas iniciales distintas, dos NP y 12 reliquias; auditoría previa a producción 26/27.
- **Powers FGO compartidos:** el diseño coloca Sello de Habilidad y Certero/Sure Hit en FGOCore. El
  power de Tiamat mantiene su ID publicado y delegará al resolver común para no romper saves.
- **Corrección posterior de assets:** la investigación de producción confirmó que Assassin usa
  `602100` y Caster `504000`; `602500`/`602510` pertenecen a otro personaje. La implementación y los
  assets quedaron completados el 2026-07-29; sigue pendiente el playtest dentro del juego.
- Documento canónico: [`DESIGN-SHUTEN.md`](DESIGN-SHUTEN.md).

## 2026-07-28 — animaciones oficiales de Kagetora y Kenshin

- **Workshop privado**: creado el ítem `3773261707` para `KagetoraLancer` con visibilidad `2` y
  staging idéntico a `dist` por SHA-256. SteamCMD confirmó `Committing update...Success`; no se
  modificó la visibilidad ni el contenido de los otros diez ítems.
- **Nueva ficha editorial**: Kagetora estrena descripción BBCode en inglés, español y chino, con
  promesa jugable primero, mecánicas/contenido cuantificados, dependencias enlazadas y estado de
  playtest visible. `docs/WORKSHOP-DESCRIPTIONS.md` registra el patrón para migrar los demás mods.
- **Dos formas animadas**: Nagao Kagetora (`303800`) y Uesugi Kenshin (`901820`) usan sus modelos
  oficiales de FGO con `idle`, ataque Quick, casteo, daño y muerte; son 153 cuadros por forma.
- **Transformación real**: la reliquia inicial instala la forma de Kagetora y el primer NP entra de
  manera permanente en Kenshin mediante el sistema compartido de formas, cambiando animación y
  pivote sin saltar el plano de suelo.
- **Pipeline reproducible**: un exportador propio corrige la omisión de clips del CLI de
  AssetStudioMod 0.19; manifest, hashes, ventanas, recorte y procedencia quedaron registrados.
- **Validación de staging**: FGOCore y KagetoraLancer compilan sin errores ni advertencias. El PCK
  contiene escena, ambos SpriteFrames y 306 texturas; compresión VRAM/mipmaps/tope 768 redujeron el
  paquete de 156 MB a 17,08 MB. Falta únicamente el playtest dentro del juego para ajustar escala o
  hitbox si la composición real lo exige; no se instaló localmente y el Workshop permanece privado.

## 2026-07-27 — Kagetora/Kenshin funcional y Críticos v2

- **FGOCore v0.1.8**: Críticos v2 reserva 50 estrellas antes de un Ataque elegible y aplica ×1,5 a
  todos sus impactos; Quick genera 10 estrellas (20 si es NP), con banco máximo 100 y hasta tres
  cargas de Crítico Listo. Artoria usa ahora el mismo sistema global.
- **Base FGO compartida**: Poder del Hombre/Tierra/Cielo/Estrella/Bestia y la preparación de
  Overcharge quedan en FGOCore, con defaults de encuentros y overrides por personaje.
- **Nuevo proyecto `KagetoraLancer` v0.1.0**: Doctrina Cielo→Pecho→Pies, ascensión irreversible a
  Kenshin, seis modelos iniciales, 68 cartas de recompensa, dos NP Event, tres elecciones de
  precepto, 21 poderes y 12 reliquias.
- **Localización**: `eng`, `esp`, `zhs`, `kor` y `rus` tienen paridad exacta (278 claves por idioma);
  SimpleLoc y la auditoría de contextos terminan con cero hallazgos.
- **Validación**: FGOCore y los diez personajes se compilaron juntos sin errores ni advertencias.
  El cierre de Kagetora volvió a compilar y regeneró `dist/KagetoraLancer` con DLL, manifest y PCK.
- **Pendiente real actualizado el 2026-07-29**: animaciones, arte de cartas/reliquias e interfaz ya
  están integrados. Faltan VFX/audio exclusivos y playtest en juego, guardado/carga y cooperativo.
  No se instaló localmente ni se tocó Steam Workshop.

## 2026-07-26 - localización coreana y rusa publicada
- **Cinco idiomas dentro del juego**: FGOCore y los nueve personajes incluyen ahora localización
  completa `kor` y `rus`, además de `eng`, `esp` y `zhs`. La terminología coreana prioriza el
  servicio oficial de Netmarble; la rusa sigue las convenciones comunitarias documentadas en
  `docs/LOCALIZATION-KOR-RUS-SOURCES.md` porque FGO no dispone de cliente ruso oficial.
- **Versiones**: FGOCore `v0.1.7`; Mash, Morgan, Artoria Caster, Mordred, Gilgamesh, Okita, Oberon y
  Tiamat `v0.1.9`; Siegfried `v0.1.10`. Los nueve personajes requieren FGOCore `>= v0.1.7`.
- **Validación**: 110 archivos y 2.271 valores por idioma, con paridad exacta de claves y marcadores;
  SimpleLoc terminó con 0 ambigüedades. Los diez PCK contienen las 110 rutas esperadas.
- **Workshop actualizado**: SteamCMD confirmó `Committing update...Success` diez veces en una sola
  sesión. Los 30 DLL/JSON/PCK de `.workshop_stage` coinciden por SHA-256 con `dist`, `stderr` quedó
  vacío y los diez items conservaron visibilidad pública.

## 2026-07-23 - Siegfried v0.1.9 y Mordred v0.1.8 publicados
- **Siegfried publicado**: el item publico `3751611015` quedo en v0.1.9 con la secuencia de ataque
  corta y a nivel del suelo. SteamCMD confirmo `Committing update...Success`; los DLL/JSON/PCK del
  staging coincidieron por SHA-256 con `dist`.
- **Causa del arte ajeno en Mordred**: las diez cartas que generan Bloqueo heredaban CE asignadas a
  Mash, Lancelot y otros personajes. `Defend` y `TournamentGuard`, por ejemplo, mostraban exactamente
  `The Noble Sword and Shield`, usada tambien por Mash.
- **Correccion de retratos**: esas diez cartas ahora usan seis CE oficiales centradas en Mordred,
  con recortes normal `500x380` y grande `1000x760`; el CSV de procedencia y el mapeo reproducible
  quedaron sincronizados.
- **Segundo cruce cerrado**: `form_mordred.png` era byte-identico a `form_siegfried.png` desde el
  scaffold inicial. Descanso y mercader usan ahora el CharaGraph oficial de Mordred y ya no conservan
  IDs internos `sieg`.
- **Validacion y publicacion de Mordred**: Mordred compila y su PCK v0.1.8 contiene los 21 imports
  corregidos, incluidos `Strike` y `Defend` en tamaño normal y grande, y las dos escenas remapeadas.
  Los cuatro CTEX basicos coinciden por MD5 con los imports locales. SteamCMD confirmo
  `Committing update...Success` para el item publico `3751610432`; falta playtest visual tras la
  resincronizacion de Steam.

## 2026-07-22 - Santo Grial por evento de Acto 2 (Plan 2)
- **Evento compartido**: `HolyGrailRitual` vive en FGOCore y entra solo en el pool de eventos de
  `Hive` (Acto 2). Aparece únicamente si todos los jugadores usan un personaje cuyo pool contiene
  un `ILimitBreaker` y ninguno posee ya uno.
- **Elección**: por 200 de oro entrega el Grial propio del personaje; con menos oro la opción queda
  bloqueada y siempre se puede abandonar el evento. El cobro y la entrega usan `PlayerCmd.LoseGold`
  y `RelicCmd.Obtain`, igual que los eventos base y con dueño correcto en cooperativo.
- **Sin duplicados aleatorios**: los nueve Griales son `RelicRarity.Event` y además responden
  `IsAllowed=false`, lo que también purga sus IDs de los grab bags de partidas iniciadas antes del
  cambio. `Palingenesis` permanece como carta menor de Vida máxima y no rompe límites.
- **Roster completo**: Siegfried recibe el `Santo Grial del Matadragones` y Tiamat el `Santo Grial
  del Mar de Vida`, ambos con arte e idioma eng/esp/zhs. FGOCore sube a v0.1.6 y los personajes a
  v0.1.8 con dependencia mínima sincronizada.
- **Validación local**: diez builds limpios, diez PCK regenerados y auditados, localización SimpleLoc
  sin ambigüedades y matriz MAIN/BETA 20/20 con sondas runtime correctas. Steam no se tocó.

## 2026-07-22 - cartas sin modelos 2D (personajes v0.1.7 preparada)
- **Mash en tienda/fogata**: sus frames ya estaban limitados a 768 px, pero ambas escenas aun
  conservaban la escala previa `0.5`; la figura quedaba 2.51 veces mas pequena y flotaba sobre el
  ancla. Escala compensada a `1.253906`, conservando la posicion que deja los pies en `y=0`.
- **Causa del diseño repetitivo**: el ultimo compositor pegaba un fotograma de combate del Servant
  sobre cada CE ya completa. En Morgan se veia como una segunda Morgan identica encima de Ataque,
  Defensa y el resto del pool, tapando el sujeto de la ilustracion oficial.
- **Correccion global**: las 614 cartas no-Command de los nueve personajes usan solo la CE, escena o
  item oficial completo asignado. Los 26 CharaGraph directos restantes se reemplazaron por arte
  tematico y se conservan 15 Command Cards oficiales, que son diseños de carta y no modelos de combate.
- **Regresion cerrada**: el generador ya no puede componer sprites, aceptar CharaGraph directos ni
  recurrir a un fallback de modelo 2D; un mapeo ausente hace fallar la generacion. Resultado: 629 pares
  normal/grande, 0 faltantes, 0 dimensiones incorrectas, 0 fuentes ausentes, 0 fallbacks invalidos,
  0 fuentes de modelo 2D y 0 diferencias en las 614 cartas mapeadas.
- **Publicacion**: los nueve PCK v0.1.7 contienen todos sus imports de retrato; matriz MAIN/BETA
  20/20, 0 errores y 0 advertencias. El staging publico tiene 27 DLL/JSON/PCK identicos a `dist` por
  SHA-256 y textos trilingues v0.1.7. Solo falta la autorizacion para reemplazar los items de Workshop.

## 2026-07-22 - cierre de auditoria de contexto y recorridos
- **Resolucion sincronizada completa**: se migraron 481 llamadas de cartas, powers y reliquias para
  reutilizar el `PlayerChoiceContext` que ya entrega el motor. La cobertura incluye NP, estrellas,
  Maldicion, Lahmu, Aliento, Deuda, Tesoro, Sueno, Sello y las ventanas NP.
- **Compatibilidad binaria conservada**: los helpers compartidos mantienen sus firmas anteriores y
  agregan overloads con contexto. Los listeners de Critico y Tos usan interfaces complementarias,
  por lo que un DLL viejo sigue cargando y uno nuevo conserva el contexto hasta sus recompensas.
- **Menos trabajo en rutas frecuentes**: `Listeners` ya no encadena `OfType`/`Concat`/`Any` para
  consultas de powers y reliquias; los recorridos sincronos salen temprano y los asincronos toman
  una sola snapshot segura. Estrellas y Lahmu eliminan busquedas LINQ adicionales.
- **Regresion automatizada**: `tools/choice_context_audit` analiza sintaxis C#, cubre contextos
  obligatorios y opcionales, y termina con 0 hallazgos. La sonda binaria comprueba que las firmas
  viejas y nuevas de Maldicion/Lahmu coexistan.
- **Validacion local**: matriz MAIN/BETA 20/20, 0 errores y 0 advertencias; artefacto MAIN cargable
  sobre BETA. Los diez DLL/JSON/PCK universales se regeneraron en `dist`.
- **Workshop actualizado**: FGOCore y los nueve personajes se publicaron en una unica sesion de
  SteamCMD; Steam confirmo `Committing update...Success` diez veces y `stderr` quedo vacio. Los diez
  items conservaron visibilidad publica y sus portadas existentes.

## 2026-07-22 - arte de cartas oficial por personaje y forma (v0.1.5, superado por v0.1.7)
- **Mapeo temático aplicado carta por carta**: las 588 composiciones usan la CE, escena, objeto o
  fotograma oficial asignado en los CSV de revisión como fondo legible y superponen el CharaGraph
  oficial del Servant y forma correctos. Ya no comparten una composición genérica por personaje.
- **Fuentes directas renovadas**: se regeneraron 26 retratos desde sus CharaGraph y se conservaron
  15 Command Cards oficiales. Esto corrige el iris viejo de `Around Caliburn: Unleashed` y reemplaza
  las cartas de comando antiguas de Mordred y Gilgamesh por el visor oficial de FGO.
- **Validacion**: 629 retratos auditados, 0 pares faltantes, 0 dimensiones incorrectas, 0 fuentes
  ausentes, 0 nombres oficiales vacíos, 0 coincidencias con fondos viejos y 0 duplicados exactos
  entre personajes. Procedencia completa en `docs/ART-CARD-IDENTITY.csv`; auditoria reproducible con
  `tools/audit_card_identity.ps1`.
- **Estado de publicación**: los nueve PCK locales contienen los retratos nuevos. Esta revisión no
  conectó Steam ni modificó Workshop; la publicación queda separada del cierre técnico.

## 2026-07-22 — cartas de ataque bloqueadas en BETA 0.108.0
- **Error confirmado en log**: `MASHSHIELDER-ARTS_MASH` terminaba con
  `MissingMethodException` al invocar `AttackCommand.FromCard(CardModel)`. BETA 0.108.0 ya usa la
  firma de dos argumentos; la acción fallaba antes de resolver el ataque y dejaba la carta en
  pantalla.
- **Cobertura global**: las 223 construcciones de ataques de FGOCore y los nueve personajes pasan
  ahora por `FromCardFgoCompatibility`. FGOCore detecta una vez la firma de uno o dos argumentos y
  conserva `CardPlay` cuando el runtime lo admite. Los DLL ya no dependen del adaptador equivalente
  de BaseLib, importante porque el entorno reportado cargaba BaseLib 3.3.5 mientras los manifests
  actuales requieren 3.3.6.
- **Validación**: matriz MAIN/BETA 20/20, 0 errores y 0 advertencias; sondas runtime correctas en
  0.107.1 y 0.109.0. La inspección IL de `ArtsMash` confirma que no referencia la firma antigua y la
  búsqueda binaria confirma el puente propio en los veinte artefactos. Los diez DLL/JSON de `dist/`
  fueron reemplazados; PCK, instalación local y Steam no se tocaron.
- **Error ajeno en el mismo log**: Faust no puede aplicar
  `CardPileCmd_Add_AfterAddedToHandPatch` en 0.108.0. Es independiente del bloqueo de Arts y debe
  corregirlo su autor.

## 2026-07-22 — resolución de turno MAIN/BETA y turnos extra
- **Causa del bloqueo reportado**: BETA 0.109.0 eliminó la sobrecarga completa de seis parámetros de
  `CreatureCmd.Damage` y agregó `CardPlay?`. `CursePower` la llamaba al comenzar el turno enemigo:
  el `MissingMethodException` cortaba la transición antes de ejecutar las intenciones. El mismo
  riesgo quedó corregido en Grial Negro, Morgan, Okita y Oberon mediante
  `CreatureCmdCompatibility.Damage`.
- **Puente optimizado**: la compatibilidad resuelve la firma una vez y usa delegados tipados; ya no
  crea arreglos ni usa `MethodInfo.Invoke` en cada golpe. La matriz ejecuta además una sonda .NET 9
  que fuerza el enlace real contra los DLL MAIN y BETA.
- **Cooperativo**: los efectos propios de FGOCore y los nueve personajes ahora filtran por
  `participants.Contains(Owner)`; un turno extra de otro jugador ya no resetea contadores ni
  consume/dispara poderes ajenos. Los efectos intencionales del turno enemigo se conservaron.
- **Validación**: matriz MAIN/BETA 20/20 con 0 errores y 0 advertencias; las dos sondas runtime
  inicializan correctamente los delegados de compatibilidad. No se tocó Steam ni la instalación
  del juego. Los diez DLL/JSON de `dist/` se regeneraron con el código corregido; los PCK no
  cambiaron porque no hubo modificaciones de recursos.

## 2026-07-22 — fondos de selección sin recorte de cabeza (personajes `v0.1.5`)
- **Causa**: las nueve escenas usaban `TextureRect.STRETCH_KEEP_ASPECT_COVERED`; al llenar 16:9
  recortaban verticalmente las ilustraciones menos panorámicas. Tiamat convertía un retrato 512×724
  en fondo completo y eliminaba de pantalla toda la cabeza.
- **Encuadre corregido**: todos los fondos usan `KEEP_ASPECT_CENTERED`, por lo que conservan la
  imagen completa y sus proporciones en cualquier resolución. El retrato de Tiamat ocupa el sector
  derecho para mantener visible la cara sin invadir el panel de descripción.
- **Validación y Workshop**: matriz MAIN/BETA 20/20 con 0 errores y advertencias; se regeneraron los
  nueve PCK y Steam confirmó `Committing update...Success` nueve veces en una única conexión. Los
  27 DLL/JSON/PCK coinciden por SHA-256 entre `dist/` y `.workshop_stage`; `stderr` quedó vacío.
- **Pendiente visual**: confirmar dentro del juego el balance entre ilustración y espacio lateral
  en 16:9, 16:10 y ultrawide; la cabeza ya no puede quedar fuera porque el arte no se recorta.

## 2026-07-22 — memoria de animaciones en co-op (`FGOCore v0.1.5`, personajes `v0.1.4`)
- **Causa restante**: cada jugador FGO activo precargaba también todas las formas alternativas de su
  personaje. Varias selecciones FGO en una partida podían agotar VRAM y dejar una pantalla negra.
- **Carga acotada**: en co-op `FormVisuals` carga únicamente la forma actual de cada criatura; los
  cambios de forma continúan asíncronos y conservan el sprite anterior mientras esperan.
- **Frames reducidos**: los 3.354 WebP de combate están limitados a 768 px. Se compensó la escala de
  escenas y formas con el factor real, por lo que el tamaño visible y el apoyo de los pies no cambian.
- **Validación y Workshop**: matriz MAIN/BETA 20/20 con 0 errores y advertencias; se regeneraron los
  diez PCK y Steam confirmó `Committing update...Success` diez veces dentro de una sola conexión.
  Los 30 DLL/JSON/PCK coinciden por SHA-256 entre `dist/` y `.workshop_stage`; `stderr` quedó vacío.

## 2026-07-22 — descripciones trilingües de Workshop
- **Formato unificado**: los diez items siguen la estructura de la página de referencia de Rimuru:
  presentación breve, versión reciente, mecánicas centrales, dependencias y reporte de errores.
- **Tres idiomas visibles**: español primero, inglés y chino simplificado en la misma descripción.
- **Publicación verificada**: Steam confirmó `Committing update...Success` para FGOCore y los nueve
  personajes. La página pública de FGOCore ya devuelve el texto nuevo; se conservaron los cinco
  items públicos y los cinco privados.
- **Cargador más seguro**: `workshop_upload.ps1` conserva la visibilidad previa, permite indicar una
  nota de cambio y ahora publica todo el lote dentro de una sola sesión de SteamCMD. Antes iniciaba y
  cerraba sesión una vez por item, lo que podía desconectar repetidamente el cliente de Steam.
  `-StageOnly` prepara y valida los VDF sin conectarse; para cambios sólo de texto o imágenes se puede
  usar directamente el editor web de Workshop.

## 2026-07-22 — Darv compatible con Acheron + previsión exacta de Laḫmu (`FGOCore v0.1.4`)
- **Bloqueo de Darv confirmado**: Acheron parchea `DustyTome.SetupForPlayer` y vuelve a ejecutar
  `NextItem(empty).Id` para personajes sin cartas Ancient. Según el orden Harmony, ese prefix corría
  antes del hardening de FGOCore y Tiamat quedaba tras el diálogo sin opciones de reliquia.
- **Compatibilidad cerrada**: `DustyTomeHardening` usa `Priority.First` y `HarmonyBefore("Acheron")`;
  al preparar una carta Rara segura devuelve `false` y evita que corra el prefix inseguro posterior.
  Verificado contra el DLL instalado de Acheron y las implementaciones MAIN/BETA de DustyTome.
- **Laḫmu legible**: el tooltip del enjambre muestra cantidad, Crianza, mordidas, daño por mordida,
  daño total y Bloqueo próximo. Las tres localizaciones reflejan el timing real: mordida al final del
  turno propio y Bloqueo al inicio del turno enemigo.
- **Validación y Workshop**: FGOCore y Tiamat compilan en MAIN 0.107.1 y BETA 0.109.0 con
  0 errores/advertencias; SimpleLoc informa 0 ambigüedades. FGOCore v0.1.4 fue publicado en el item
  `3747876334` y Steam confirmó `Committing update...Success`; DLL/JSON/PCK coinciden por SHA-256
  entre `dist/` y `.workshop_stage`. Pendiente confirmar el encuentro visualmente dentro del juego.

## 2026-07-19 — pies alineados con la barra de vida
- **Release**: lote completo `v0.1.3`; los nueve personajes requieren `FGOCore >= v0.1.3` para
  evitar una carga parcial con la API de posición anterior.
- **Causa cerrada**: la fórmula vertical usaba el bbox del WebP original sin el factor de
  `process/size_limit`; Godot reducía el lienzo a 768/1024 pero dejaba `Sprite.Position.Y` sin
  escalar. MAIN y BETA tienen exactamente las mismas tres escenas base de criatura/barra.
- **Nueve personajes cubiertos**: se recalcularon pies, `Bounds`, `IntentPosition` y `CenterPos`.
  Gilgamesh ya estaba correcto porque sus frames no superan 1024; los otros ocho bajaron según su
  factor real de importación.
- **Formas cubiertas**: `FormVisuals.RegisterFramesWithSpritePosition` aplica X/Y al intercambiar
  frames de Mash, Morgan, Artoria, Okita y Tiamat. La API anterior de sólo X se conserva para
  compatibilidad binaria.
- **Validación y Workshop**: matriz MAIN/BETA 20/20 con 0 errores y 0 advertencias; los diez
  DLL/JSON/PCK se regeneraron y Steam confirmó `Committing update... Success` en los diez items.
  `.workshop_stage` coincide por SHA-256 con `dist/` y `stderr` quedó vacío. Se conservaron cinco
  items públicos y cinco privados. Pendiente confirmar visualmente dentro del juego.

## 2026-07-18 — auditoría integral y retiro del proyecto abandonado
- **Hooks de cálculo corregidos**: Resistencia Mágica de Artoria y Sueño de una Noche de Verano EX
  de Oberon ya consumen su uso sólo en el hook de confirmación. Guts, Amuleto de Fou, Pared
  Absoluta, Cobertura y Sueño separan lectura de preview y commit real; Corona del Sin Par dejó de
  producir VFX durante previews.
- **Daño multigolpe y cooperativo**: Cobertura registra cada traspaso por aliado para no mezclar ni
  perder cantidades en ataques de área. La Sentencia de las formas Reina de Morgan se limpia en el
  callback que también corre al matar, evitando repetir el bono en el siguiente golpe de la carta.
- **Poderes persistentes endurecidos**: 26 cartas de Mordred, Gilgamesh, Okita y Oberon ya no pueden
  degradar campos mejorados al jugar después una copia base. Los acumuladores usan `Math.Max`, los
  descuentos `Math.Min` y las capacidades booleanas se conservan con `|=`.
- **Oberon**: El Libro del Fin de los Sueños sustituye realmente la conversión de Deuda de la starter;
  ambas reliquias ya no premian el mismo pago. Se conservó la entrada de forma inicial de la starter.
- **Limpieza**: cuatro `PowerVar` de Mash recibieron nombres estables, los robos de Mordred reutilizan
  el `PlayerChoiceContext` del hook y se eliminó un callback de daño redundante de Mash.
- **Proyecto externo retirado**: se eliminaron su fuente, configuración, staging, entrada del
  instalador y referencias de la documentación activa. El monorepo vuelve a contener sólo FGOCore y
  los nueve personajes FGO.
- **Validación**: auditor SimpleLoc 0 ambigüedades; matriz MAIN 10/10 y BETA 10/10 con 0 errores y
  0 advertencias; `git diff --check` sin errores. Pendiente playtest dentro del juego.

## 2026-07-18 — centrado horizontal de los 9 personajes (`v0.1.2`)
- **Causa cerrada**: `flip_h = true` invertía la compensación del lienzo transparente y
  `process/size_limit` reducía el offset interno de la textura sin modificar `Sprite.Position`.
  La captura de Morgan Aesc se reproduce con esos dos factores.
- **Todas las escenas corregidas**: Mash, Morgan, Artoria, Mordred, Gilgamesh, Okita, Oberon,
  Siegfried y Tiamat usan pivotes X medidos sobre sus frames `idle` ya escalados como los importa
  Godot. No cambiaron la altura, pies, `Bounds`, hitbox, barra ni anclas.
- **Formas cubiertas**: `FormVisuals.RegisterFramesWithSpriteX` guarda un pivote por `FramesPath` y
  lo aplica junto al cambio de forma. Mash, Morgan, Artoria y Tiamat permanecen centrados al
  transformarse; los fallbacks de Mordred/Okita conservan el pivote de su escena base.
- **Release completo**: FGOCore + 9 personajes `v0.1.2`, `FGOCore >= v0.1.2`; matriz MAIN/BETA
  20/20 con 0 advertencias y 0 errores. Los diez DLL/JSON/PCK se regeneraron y Steam confirmó
  `Upload finished ... OK` en los diez items. Los 30 archivos de `.workshop_stage` coinciden por
  SHA-256 con `dist/` y `stderr` quedó vacío. Se conservaron 5 públicos y 5 privados.

## 2026-07-18 — compatibilidad MAIN 0.107.1 + BETA 0.109.0
- **Un solo lote para ambas ramas**: FGOCore + 9 personajes en `v0.1.2`, dependencias
  `BaseLib >= v3.3.6` y `FGOCore >= v0.1.2`. Los DLL finales se compilan contra MAIN y usan
  puentes runtime para las firmas BETA; no hay dos variantes incompatibles en Workshop.
- **API BETA adaptada**: `AttackCommand.FromCard` conserva `CardPlay` mediante
  `FromCardCompatibility`; `CreatureCmd.Damage`/`LoseBlock` pasan por adaptadores de firma; los
  hooks `ModifyDamage*` de BETA se redirigen a los overrides legacy mediante Harmony.
- **Matriz verde 20/20**: los 10 proyectos compilan con 0 advertencias y 0 errores contra MAIN v0.107.1 y BETA
  v0.109.0 (`tools/build_compat_matrix.ps1`). BaseLib de compilación 3.3.6; runtime 3.3.7.
- **Arranque real 10/10 en ambas ramas**: los mismos DLL/JSON/PCK de `dist/` inicializaron FGOCore,
  Mash, Morgan, Artoria, Mordred, Gilgamesh, Okita, Oberon, Siegfried y Tiamat en MAIN y en una
  instalación BETA aislada. Sin `MissingMethodException`, `ReflectionTypeLoadException` ni fallo de
  Harmony atribuible a FGO. Workshop y `mods/` fueron restaurados tras cada prueba.
- **Publicado en Steam Workshop**: SteamCMD confirmó `Committing update... Success` para los diez
  items `v0.1.2`. Se conservaron públicos FGOCore, Mash, Morgan, Artoria y Tiamat; y privados
  Mordred, Gilgamesh, Okita, Oberon y Siegfried. Los 30 DLL/JSON/PCK preparados en
  `.workshop_stage` coinciden por SHA-256 con `dist/`; `stderr` quedó vacío.

## 2026-07-16 — auditoría integral de código
- **FGOCore + 9 personajes auditados de punta a punta** contra MAIN v0.107.1 / BaseLib 3.3.0:
  contratos de hooks, contextos de elección, RNG, previews, VFX, IDs, pools, mazos iniciales,
  reliquias iniciales, tipos Buster/Arts/Quick, localización y recursos. Release final:
  **10/10 proyectos, 0 errores y 0 warnings**.
- **Contextos y robos mid-play endurecidos**: los efectos que roban al cambiar de forma conservan el
  `PlayerChoiceContext` del hook y se limitan al mazo existente para no reshufflear la carta que se
  está resolviendo. `NpWindow` conserva su firma pública y agrega overload con contexto; Tiamat lo usa.
- **Tipos de comando completos**: las 31 cartas que consumen Carga NP implementan `ICommandTyped`
  con su tipo de lore, y las básicas Buster/Arts/Quick de Mash, Morgan y Artoria ya alimentan el
  sistema compartido. Las reliquias iniciales que no pasan por `BondRelic` instalan
  `CommandBonusPower`.
- **Contenido inicial reconciliado**: toda reliquia `Starter` es alcanzable desde `StartingRelics`;
  los nueve personajes arrancan con un `INpLevelStore`; Gilgamesh vuelve al mazo inicial diseñado de
  10 cartas. Los poderes temporales vanilla-locales de Mordred y Siegfried implementan
  `ICustomModel`, evitando IDs sin prefijo.
- **Previews y hooks tolerantes a null**: glows de Maldición/Sueño/Barril Negro y efectos de
  turno/cambio de forma ya no asumen que existe `CombatState` o `PlayerCombatState`. Los `OnPlay`
  declaran explícitamente la precondición garantizada por el motor. Resultado: se eliminaron las 88
  advertencias nullable que ocultaban regresiones reales.
- **Localización validada**: paréntesis explicativos y `+` literales escapados para SimpleLoc;
  `tools/audit_simpleloc.ps1` cubre dinámicamente los 10 módulos. JSON, claves eng/esp/zhs y auditor
  pasan sin diferencias, duplicados ni ambigüedades.
- **Distribución regenerada**: los nueve personajes se publicaron a `dist/<Id>/` con DLL/JSON/PCK
  frescos (49,3–126,5 MB).
- **Lote completo subido a Steam Workshop**: SteamCMD confirmó `Committing update... Success` para
  los diez items existentes. Públicos: FGOCore, Mash, Morgan, Artoria y Tiamat. Privados: Mordred,
  Gilgamesh, Okita, Oberon y Siegfried. Los DLL/JSON/PCK de `.workshop_stage` coinciden por SHA-256
  con `dist/` y `stderr` quedó vacío. No se instaló una copia local y todavía falta playtest en MAIN.

## 2026-07-16 — arte Mash completo
- **Mash 88/88 cartas con arte**: auditoría clase→retrato normal/big = 0 faltantes y 0 copias del
  placeholder. La única brecha era `LordCamelotCharge`; usa el charagraph oficial Paladín
  (`CHARA:800200a`), con Lord Camelot restaurado en primer plano.
- Mapping persistido en `assets/reference/ce/mapping.csv` y delta reproducible en
  `assets/reference/ce/mash_missing_mapping.csv`.
- FGOCore Release compila; la auditoría integral posterior dejó los 10 proyectos en 0 warnings.
  Mash Release publica a
  `dist/MashShielder/`; `MashShielder.pck` contiene ambos imports de `lord_camelot_charge`.
  **Subido al Workshop público existente** `3747876464` el 2026-07-16: SteamCMD confirmó
  `Committing update... Success`. No se instaló localmente ni se validó todavía dentro del juego.
- El publish inicial reveló que el re-render posterior había dejado los 466 `.webp.import` otra vez
  en `size_limit=0`. Se reaplicó `patch_webp_imports.ps1 -SizeLimit 768` y se republicó: los 466
  quedaron capeados y el `.pck` final bajó de 219.5 MB a **93.4 MB**.
- **Posición de Mash corregida antes del upload**: el ajuste global de 700 px dejaba la figura pegada
  al borde superior del combate (`scale=0.7231`, `y=-516.3`). Se restauró la transformación ya usada
  por tienda/fogata (`scale=0.5`, `y=-327.7`): idle base de ~484 px con pies en `y=0`; Bounds,
  IntentPosition y CenterPos se recalibraron para esa altura. Pendiente confirmar visualmente en juego.
- **Auditoría del mismo defecto en los 9 personajes**: Mash era el único corregido; Morgan, Artoria,
  Mordred, Gilgamesh, Okita, Oberon, Siegfried y Tiamat seguían entre ~687 y ~795 px. Reducidos a
  ~484–511 px (Tiamat Bestia ~558), con pies y anclas recalculados. La posición derivada de las
  burbujas de diálogo bajó de hasta `y≈-863` a `y≈-438…-473`.
- **Reportes Black Grail/Pioneer revisados**: ya corregidos/aclarados desde `72233028` y publicados el
  2026-07-06. Replay vuelve a aplicar Black Grail y suma una acumulación; Pioneer cubre la primera NP
  manual del mazo, no la ulti Event auto-manifestada. Verificación contra `CardModel.Play`,
  `PowerCmd.Apply` y `NpCharge.ConsumeAllForNpCard`.
- **Fix visual 9/9 publicado a Workshop**: Mash ya estaba actualizado; Morgan, Artoria y Tiamat
  actualizaron sus items públicos, y Mordred, Gilgamesh, Okita, Oberon y Siegfried sus items privados.
  SteamCMD confirmó `Committing update... Success` en los ocho. Los archivos de `.workshop_stage`
  coinciden por SHA-256 con `dist/`. Gilgamesh además pasó sus 249 frames de `size_limit=0`,
  `compress/mode=0` a `size_limit=1024`, `compress/mode=1`.
- **Error de arranque externo diagnosticado**: Archetto público (`3747563715`) llama una firma
  eliminada de RitsuLib 0.4.57 y aborta `ModelDb.Init` con `MissingMethodException`. No involucra
  FGOCore ni los personajes FGO. Solución pendiente de autorización: desactivar Archetto en MAIN.

## 2026-07-06 — evento de pociones + pies/barra de vida + pool Tiamat
- **CardRewardHardening (FGOCore)**: `CreateForReward` tiraba con pools chicos filtrados por rareza×tipo (evento 药水的未来 y el 天命芝士 de otro mod → "无法选牌" en Tiamat). Finalizer Harmony: reintenta permitiendo duplicados. Detalle en FINDINGS.
- **Tiamat +6 cartas** (todo combo del evento ahora ≥3): Fauces de la Larva (C/A, 9 puro), Vigilia de la Madre (U/P, robo si ≥3 Laḫmu), Limo Protector (U/P, Metallicize en Baluarte), Diluvio del Génesis (R/A, AoE +1 por 2 Maldición), Crisálida Abisal (R/S, 18 Baluarte+2 Crianza, Exhaust), Instinto Depredador (R/P, `ISwarmBiteAmplifier` +1 mordida). Pool: C 8 (A3/S4/P1) · U 18 (A6/S9/P3) · R 9 (A3/S3/P3). Loc eng/esp/zhs.
- **Arte Tiamat completo**: 20 retratos (14 de DESIGN-REVIEW-2 que estaban SIN arte + 6 nuevos) vía match-ce-art → `assets/reference/ce/tiamat_missing_mapping.csv` → make_card_art.
- **Pies→y=0 en los 9 personajes** (reporte "血条在胯部"): la barra de vida es Y-fija en el origen del creature; los sprites quedaron hundidos tras la normalización (Okita +608px). posY corregido por bbox alfa + Bounds/Intent/CenterPos al alto real. Detalle en FINDINGS.
- **v0.108.0 existe SOLO en el branch beta de Steam** (main sigue v0.107.1): "beta版没法出攻击牌" = incompatibilidad esperada (los mods apuntan a MAIN). Migrar cuando 0.108 llegue a main.
- Los 10 publicados a `dist/` (dll+pck verificados con los cambios adentro) y **subidos a Workshop 10/10 Success** (públicos: FGOCore/Mash/Morgan/Artoria/Tiamat; privados: Mordred/Gil/Okita/Oberon/Siegfried). Falta que el user re-sincronice Steam y pegue las respuestas en chino.

Fecha anterior: **2026-06-25**. Este archivo es la **fuente de verdad del estado**; reemplaza la
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

- **DESIGN-REVIEW-2 (2da pasada) implementada + COMPILADA verde** (8 mods, [DESIGN-REVIEW-2.md](DESIGN-REVIEW-2.md)): **Gil** = motor de Armas (cartas generadoras + contador central `ArmsPlayedPower.AfterCardPlayed`) + módulo Tesoro (fallback del Oro, patrón `DebtPower`) + starter QAABB + reliquia Bab-ilu; **Tiamat** = **SkillSeal REAL** (cancela la habilidad enemiga vía `CreatureCmd.Stun`, patrón Sleep de Oberon) + pool Lily 15→**27** + loop ofensivo (Marea Voraz: el enjambre muerde al fin de tu turno); **Mordred** = starter QAABB + cap a Saberface (100★) + 4 riders bi-condicionales; **Siegfried** = pool 24→**32** + BalmungSwing lee SdD + payoffs de ★; pulido **Morgan/Castoria/Mash/Okita**. **Publicado a `dist/` + instalado a G: + re-subido a Workshop** (8 mods cambiados, privados, 2026-06-25). **Falta**: arte de las ~41 cartas nuevas (placeholder `card.png` hoy) + playtest de balance.
- **Bug de ojos de Castoria Berserker** arreglado (commit `9b78781`): el idle re-ventaneado `[150-154,0-4]` es solo la "subida" (no loopea → cabeza/ojos saltan en la costura 009→000); convertido a **ping-pong** en `artoria_frames_berserker.tres` reusando los frames (sin re-render). Entra con la próxima publicación de Artoria.

- **🔴 CRASH/VRAM de players arreglado + 10 mods PÚBLICOS** (2026-06-26, commit `75bddf5`): reportes de Workshop (cartas/intención/NP no renderizaban = "solo barras de vida"; crash con 3 chars; multi lageaba). Causa (godot.log + código): (1) `FormVisuals.Apply→PreloadAll()` cargaba en VRAM las formas de TODOS los mods FGO **instalados** a un `Cache` estático nunca liberado → ahora **lazy por-char** (solo el que pelea); (2) frames de **Mash sin cap** (`size_limit=0` ~1900px ~6GB) → capeados los 4 animados a **768** (`.pck` Mash 136→104 MB). Los **10 mods ahora son PÚBLICOS** (visibility 0). Detalle en FINDINGS §VRAM.

- **DESIGN-REVIEW-3 — expansión de sistemas FGO IMPLEMENTADA** (2026-06-26, commits `254208b`+`f45f6c3`, [DESIGN-REVIEW-3.md](DESIGN-REVIEW-3.md)): los **9 manifiestan su carta de ulti a 100** (escala con Sobrecarga) **tipada a su NP del juego** (Mash/Castoria=Arts, Okita=Quick, resto=Buster); **cap de NP por dupes** (sin dupes→100; dupes suben a 200/300; Grial extiende); **sistema de tipos** Buster/Arts/Quick con bonus (Buster→Fuerza temp/perm, Quick→★, Arts ulti→NP); **8 CEs 5★ colorless** drafteables por todos (Kaleidoscope/Black Grail/2030/Prisma Cosmos/Imaginary Element/Heaven's Feel/Formal Craft/Zero Over); **consolación de dupe** (oro/upgrade/encantar/elegir-carta según pity) en los 7 relics-store; ulti de Siegfried (`BalmungUnleashed`). FGOCore + 9 verde + republicado (4 públicos, 6 amigos). **Flags de playtest** en el doc (consolación pity-alto = pantalla anidada; MapoTofu daña; balance sin tunear). **Arte de las CEs pendiente** (match-ce-art).

- **🔴 Bugs de MULTIPLAYER + soft-lock arreglados y LIVE** (2026-06-26): (1) soft-lock al cambiar de forma con el mazo vacío (el robo reshuffleaba la carta jugada → `must be added to a CombatState`) — capeado el draw en 7 powers/relics; (2) **divergent states** de co-op — `NpLevels` tiraba el dado del dupe con `RunState.Rng.CombatCardGeneration` (stream compartido de combate) en flujo card-reward local-only → `PlayerRng.Rewards`; (3) **crash de Ortinax en MP** — `FormVisuals.GetFrames` hacía `Load` síncrono que congelaba el hilo → no-bloqueante + apply diferido. Reglas en [FINDINGS.md](FINDINGS.md §MP).
- **🔴 AUDITORÍA de código + remediación de 23 hallazgos LIVE** (2026-06-26, commit `07bf1dad`, [plan](../C:/Users/YX14n/.claude/plans/iterative-napping-snowflake.md)): workflow multi-agente (19 áreas, verificación adversarial) → 38 hallazgos → 23 ítems (5 High/4 Med/14 Low, 0 Critical). **Arreglados TODOS**: 2 soft-locks nuevos (Mordred `CigaretteLion`/`KairisCigarettes`, mismo draw-cap), interés de Deuda de Oberon que duplicaba el saldo, ults de Mash gateadas por forma (Shielder→Camelot, Ortinax→BlackBarrel, Paladin→Chaldeas — mapeo Mooncell, docstrings estaban invertidos), guard de amplificación NP **compartido** (FormalCraft↔GoldenRule), gauge-cross en `NpCharge`, `FormVisuals` failed-cache + token anti-race, `BlockedHits` transfer en form-switch, `FouMiracle`→`GutsPower` (Grial), Okita Counter ×Amount, Artoria doble-descuento, ExpEmber upgrade, + null-checks/docs/dead-code. FGOCore + 9 verde, republicado (4 v0 / 5 v1). `decompiled/` regenerado a v0.107.1 (gitignored, local).

## Playtest watch-list (ya compila verde)
Los fixes están **publicados y en Workshop (PÚBLICOS)** (2026-06-26). Falta **playtest** (balance) + arte de las cartas nuevas. Puntos a vigilar EN JUEGO (los riesgos de compilación ya se resolvieron):
- **Morgan**: `CreatureCmd.Damage` 6-arg en MainFile (verificado = calca CursePower ✓); `BeforeCardPlayed`/`cardPlay.Target` (degrada elegante si el target no está resuelto); cap de Maldición = 25 (no se tocó FGOCore).
- **Gil**: escaneo de mano `PlayerCombatState` (verificado canónico ✓); `KingsArrogancePower` rompe por Bloqueo remanente al fin de turno; Bab-ilu no existe → Arrogancia colgada de `OathOfUruk`.
- **Siegfried**: `DamageVar(0)` de `ScaleEruption` — chequear que no muestre "0 daño" en tooltip.
- **Todos**: correr `tools/audit_simpleloc.ps1`; balance sin playtest (Sentencia total de Morgan, +golpes de Okita, +1/Arma de Gil son perillas).

## Pendiente (orden)
1. **Re-publicar TODO a Workshop junto**: webp patch (VRAM) + NP fixes + manifests (formato nuevo) + Tiamat + los servants que faltan subir (Mordred/Gilgamesh/Okita/Oberon/Siegfried). Ahora usa el **staging** (`dist/` → install-mod / upload).
2. **Mod de optimización de VRAM** (lazy character loading) — DESPUÉS de Tiamat.
3. Playtest de carga con RitsuLib 0.5.10 y confirmar recursos NP/Estrellas, los contratos Ancient y
   el audit de tags Buster/Arts/Quick en `godot.log`.

## Bloqueado / a decidir
- ✅ **Resuelto (2026-06-25)**: el juego no estaba desinstalado — se **movió de biblioteca Steam a `G:\SteamLibrary\steamapps\common\Slay the Spire 2`** (el viejo C: quedó con restos). `Sts2Path`→G: en los Directory.Build.props. **Build verificado end-to-end**: FGOCore + los 7 personajes con fixes + Tiamat compilan VERDE → `dist/` (solo faltaba 1 `using` en Gil, arreglado). Falta: **playtest** (balance) + **publish/install**.
- Cómo hacer Tiamat jugable para playtest: **local-rápido vs re-publish a Workshop** (pendiente decisión del user).
- NP fixes que requieren decisión del user: Okita romaji vs EN oficial; Artoria "Hopewill"/"Round of Avalon". (Siegfried `失坐`→`失坠` es fix claro, aplicar.)

## Regla de mantenimiento
Antes de cerrar una sesión, actualizar **STATUS / next-task / HANDOFF**; no dejar que el código quede adelante de los docs. "Instalado" ≠ "validado en juego": leer el `godot.log` y probar.
