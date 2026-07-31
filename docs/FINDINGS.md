# FINDINGS — hallazgos técnicos verificados

Conclusiones de alta densidad (no historial). **Verificado** = visto en código/log/decompilado;
lo no verificado se marca *(probable)* / *(a confirmar)*. Decisiones cerradas → [DECISIONS.md](DECISIONS.md).

## Cobertura debe separar preview, confirmación y expiración (verificado, 2026-07-31)

- `ModifyHpLostBeforeOsty` también corre durante previews. `CoverPower` guardaba allí el objetivo y
  el daño candidato, de modo que una consulta de UI podía reemplazar el estado de una resolución
  real antes de `AfterModifyingHpLostBeforeOsty`; el resultado era una transferencia omitida o
  dirigida con un monto ajeno.
- El camino seguro usa `BeforeDamageReceived` para registrar sólo resoluciones reales, mantiene
  `ModifyHpLostBeforeOsty` como lectura pura y deja que `AfterModifyingHpLostBeforeOsty` confirme
  únicamente al Cover que realmente cambió el monto. Stacks por resolución/objetivo cubren daño
  reentrante y varias Mash; `DamageResult.BlockedDamage` permite transferir sólo lo que atravesó el
  Bloqueo del aliado. El guard estático sigue cortando Coberturas mutuas.
- En cooperativo, `!participants.Contains(Owner)` no equivale a «terminó el turno enemigo»: un turno
  extra de otro jugador también excluye al owner y retiraba la defensa antes de la siguiente volea.
  Cobertura, Provocación y Pared Absoluta ahora expiran con `Owner.Side != side`, el mismo contrato
  de `FlameBarrierPower` vanilla.
- Verificación: orden de `CreatureCmd.Damage` contrastado con el decompilado; auditoría de contextos
  con 0 hallazgos; los 13 proyectos compilan sin advertencias en MAIN y BETA y pasan los tres probes
  de enlace runtime.

## `PlaceholderCharacterModel` también hereda al Guerrero en tienda/fogata (verificado, 2026-07-30)

- BaseLib implementa `CustomMerchantAnimPath` y `CustomRestSiteAnimPath` en
  `PlaceholderCharacterModel` mediante `PlaceholderID`, cuyo valor por defecto es `ironclad`. Tener
  una escena `*_merchant.tscn` o `*_rest.tscn` dentro del PCK no basta: si el modelo no sobrescribe
  ambas propiedades, el juego nunca la solicita y muestra al Guerrero.
- La auditoría encontró seis afectados: Mordred, Gilgamesh, Okita, Oberon, Siegfried y Tiamat. Los
  primeros cinco tenían escenas estáticas pero desconectadas; Tiamat no tenía escenas. Después de
  conectar/crear las rutas, los 12 personajes tienen presentación propia y reposo animado.
- Gate reproducible: `tools/audit_character_presentation.py` deriva los 12 modelos desde el código,
  exige ambas rutas, comprueba que las escenas existan, que usen `AnimatedSprite2D` con `idle`, que
  la fogata tenga `ControlRoot`/`Hitbox` y que el suavizado compartido cubra su namespace `res://`.
- El suavizado temporal debe ser sólo presentación: mezclar brevemente el fotograma anterior y
  modificar `AnimatedSprite2D.Offset` evita tocar `Position`/`Scale` (fuente de verdad de
  `FormVisuals`) y no altera la velocidad o duración que BaseLib usa para resolver ataques, casteos,
  daño y muerte.

## Una excepción que anula todo el contador puede hacer que el poder parezca roto (verificado, 2026-07-30)

- La Hoja de Tilo hacía que el primer ataque que alcanzaba a Siegfried cada turno ignorara toda la
  Sangre de Dragón. Contra enemigos de un solo ataque, cada turno anulaba exactamente el único golpe
  en el que el jugador podía observar la defensa; las acumulaciones funcionaban en código, pero no
  tenían efecto visible en ese patrón común.
- La excepción conserva el punto débil narrativo si atraviesa sólo 1 Escama: con 2 acumulaciones el
  primer golpe se reduce en 1 y los posteriores en 2. Así toda acumulación adicional tiene valor sin
  eliminar la debilidad de la espalda.
- Los hooks `ModifyHpLost*` deben seguir siendo lecturas puras. El feedback visual correcto vive en
  `AfterModifyingHpLostBeforeOsty`, que el motor invoca únicamente en la resolución real y sólo para
  los modelos que cambiaron el daño entero; no parpadea durante previews.
- Un contador visible de reliquia necesita explicar qué representa. En la Hoja de Tilo muestra nivel
  NP, no cantidad de Escamas: cada nivel suma 100 al máximo de Carga NP y 15% al daño NP.

## Mejoras repetidas necesitan suelos explícitos (verificado, 2026-07-30)

- `Infinite Upgrades` v1.0.0 sólo parchea `CardModel.MaxUpgradeLevel` a `int.MaxValue`. Cada mejora
  vuelve a ejecutar `OnUpgrade`; al cargar una partida, `CardModel.FromSerializable` lo reproduce una
  vez por nivel guardado. No existe una API especial del mod que las cartas deban implementar.
- `CardEnergyCost.UpgradeBy` ya limita la Energía a 0, pero `DynamicVar.UpgradeValueBy` no tiene
  suelo: costes repetidos de NP, Estrellas, Sake, Deuda o Ráfaga pueden volverse negativos; divisores
  pueden llegar a 0. Todo reductor repetible necesita un suelo semántico explícito.
- La compatibilidad vive en FGOCore y sólo actúa desde la segunda mejora sobre cartas de los 12
  assemblies FGO. Se ejecuta igualmente durante preview, carga y combate, por lo que el resultado es
  determinista y no agrega estado nuevo al guardado.
- Un coste reducido a 0 sigue siendo un pago válido. `NpCharge.Spend` y `CritStars.Spend` deben
  devolver `true` sin exigir que exista el power ni mutarlo; un coste negativo es inválido tanto en
  `CanPay` como en `Spend`. Antes, varias conversiones de Mash, Morgan, Mordred, Okita y Astolfo eran
  jugables a 0 pero abortaban su recompensa o tomaban la rama de fallo.
- `RafagaCost` es la excepción semántica y conserva suelo 1: Velocidad Cegadora ya cuesta 0 Energía,
  gana 1 Energía y roba 1, de modo que eliminar también su segundo coste habilita un ciclo
  determinista. El diseño de Okita fija expresamente Ráfaga en 1-3 de Aliento.

## Preview y guardado exigen separar cálculo de consumo (verificado, 2026-07-29)

- `ModifyDamageAdditive*` puede ejecutarse para preview antes de la resolución real. Mutar un campo
  desde ese hook consume cargas al pasar el cursor, recalcular intención o reconstruir la carta.
  Shuten hacía esto en dos powers de daño; ahora el hook sólo calcula y `AfterDamageGiven` confirma
  el gasto después del resultado.
- Campos privados como `_usedThisTurn` o `_triggersThisCombat` no forman parte del estado guardable y
  pueden reactivarse al continuar una partida. `FgoCombatState` codifica flags/contadores en powers
  ocultos sincronizados: estado de turno se limpia antes del setup/robo del participante y estado de
  combate permanece hasta que termina el combate.
- La migración cubre los 12 personajes. No se migraron variables puramente transitorias de una
  resolución en curso, como un target pendiente o una guardia de reentrancia.

## Cobertura visual y VFX ahora tienen gates reproducibles (verificado, 2026-07-29)

- `tools/audit_asset_coverage.ps1` deriva modelos desde la localización inglesa y exige retrato
  normal/grande de cada carta, iconos de cada power/reliquia, outlines y un set completo de UI.
- `tools/audit_vfx_paths.ps1` compara cada ruta usada por los mods contra el catálogo extraído del
  decompilado; una ruta inexistente es error porque `VfxCmd.PlayVfx` puede abortar la resolución.
- Cierre actual: 12 personajes, 867 cartas, 257 powers, 122 reliquias y 0 rutas VFX inválidas.

## Tienda/fogata también requieren compensar `process/size_limit` (verificado, 2026-07-22)

- Mash usaba el mismo frame fuente de 1926 px en combate, tienda y fogata. Al limitarlo a 768 px,
  combate recibió una escala compensada pero `mash_merchant.tscn` y `mash_rest.tscn` conservaron
  `scale=0.5`; el personaje quedó `1926/768 = 2.5078` veces más pequeño y sus pies flotaban sobre
  el ancla. La escala equivalente es `0.5 * 1926/768 = 1.253906`; la posición `y=-327.7` no cambia
  porque ya corresponde al encuadre original compensado. Regla: cualquier cambio de límite de
  importación debe actualizar combate, tienda y fogata, no sólo la escena `*_visuals.tscn`.

## `PlayerChoiceContext` debe atravesar toda la cadena de helpers (verificado, 2026-07-22)

- Conservar el contexto solo en `OnPlay` no alcanza: un helper intermedio que crea
  `BlockingPlayerChoiceContext` separa powers, robos o recursos de la resolucion sincronizada que
  inicio la carta. El mismo riesgo aparece en callbacks de listeners que otorgan otro recurso.
- Se agregaron overloads aditivos y se conservaron todas las firmas antiguas. La migracion cubre
  NP, estrellas globales y de Artoria, Maldicion, Lahmu, Aliento/Tos, Deuda, Tesoro, Sueno, Sello y
  ventanas NP. Los listeners viejos siguen funcionando mediante interfaces complementarias.
- Auditoria sintactica reproducible:
  `dotnet run --project tools/choice_context_audit/ChoiceContextAudit.csproj -- .`. Con `--fix`
  inserta el contexto solo dentro de metodos que ya reciben `PlayerChoiceContext`; tambien entiende
  `PlayerChoiceContext?` y conserva un fallback bloqueante.
- Resultado de cierre: 481 llamadas corregidas, segunda pasada con 0 hallazgos. En metodos con
  contexto opcional y varias operaciones se crea un unico fallback y se reutiliza, evitando
  resoluciones hermanas con contextos distintos.

## Pies y anclas verticales con `process/size_limit` (verificado, 2026-07-19)

- MAIN 0.107.1 y BETA 0.109.0 contienen las mismas escenas `combat/creature`,
  `creature_state_display` y `health_bar`; el desplazamiento no era un cambio de layout de BETA.
- La corrección anterior usaba `y = (sourceHeight/2 - alphaBottom) * spriteScale`. Esa fórmula sólo
  sirve con `process/size_limit=0`. Con frames reducidos a 768/1024, el offset interno se achica y el
  `Sprite.Position.Y` queda intacto, elevando al personaje. La fórmula completa es
  `y = (sourceHeight/2 - alphaBottom) * min(1, sizeLimit/max(sourceSize)) * spriteScale`.
- Se recalcularon `Sprite.Position.Y`, `Bounds.offset_top`, `IntentPosition` y `CenterPos` en las
  nueve escenas. Gilgamesh conserva sus valores porque sus frames de 538x867 no superan el límite
  de 1024. `FormVisuals.RegisterFramesWithSpritePosition` guarda X/Y por `FramesPath`, por lo que
  Mash, Morgan, Artoria, Okita y Tiamat tampoco recuperan el desplazamiento al cambiar de forma.

## Centrado horizontal de modelos animados (verificado, 2026-07-18)

- La barra de vida se posiciona desde `Bounds.GlobalPosition.X`; con `Bounds` simétrico, su centro es
  el origen de la criatura. El `AnimatedSprite2D`, en cambio, usa un lienzo transparente asimétrico
  y `flip_h = true`, por lo que un pivote calculado antes del espejo desplaza la figura al lado
  contrario. Morgan Aesc reproducía exactamente el desvío de la captura.
- `process/size_limit` también importa: Godot reduce el offset interno de la textura, pero no la
  propiedad `Sprite.Position`. Para sprites espejados, el pivote correcto es
  `x = (alphaCenterX - sourceWidth/2) * min(1, sizeLimit/max(sourceSize)) * spriteScale`, medido sobre
  todos los frames de `idle` con alfa >= 16. No usar directamente el bbox del WebP original.
- Mash, Morgan, Artoria y Tiamat tienen lienzos diferentes por forma. `FormVisuals` registra un
  pivote por `FramesPath` y lo aplica en el mismo frame que cambia `SpriteFrames`; así una
  transformación no recupera el desplazamiento anterior. Las nueve escenas base también llevan su
  pivote corregido. La corrección vertical del 2026-07-19 amplió este registro de X a X/Y.

## v0.109.0 BETA — compatibilidad dual (verificada con ilspycmd + arranque real)

- `AbstractModel.ModifyDamageAdditive`, `ModifyDamageMultiplicative` y `ModifyDamageCap` agregan
  `CardPlay?`; FGOCore conserva los overrides MAIN y un patch Harmony BETA los invoca sin duplicar DLL.
- `AttackCommand.FromCard(CardModel)` pasa a `FromCard(CardModel, CardPlay?)`; usar siempre
  `BaseLib.Utils.FromCardCompatibility`.
- Las sobrecargas de `CreatureCmd.Damage` con carta agregan `CardPlay?`. `CreatureCmd.LoseBlock`
  pasa de `(Creature, decimal)` a `(PlayerChoiceContext, Creature, decimal, Creature?)`.
  `CreatureCmdCompatibility` selecciona la firma disponible en runtime.
- **Bloqueo de cambio de turno (2026-07-22)**: la sobrecarga completa de `CreatureCmd.Damage`
  también pasa de 6 a 7 parámetros aunque `cardSource` sea `null`. Las llamadas directas de
  `CursePower` y `BlackGrailPower` lanzaban `MissingMethodException` en BETA al comenzar un turno;
  el turno enemigo quedaba detenido antes de ejecutar sus intenciones. El mismo riesgo existía en
  Morgan (`FaeBloodPactPower`), Okita (`Rafaga`) y Oberon (`DebtPower`/`VortigernPower`). Toda llamada
  completa debe pasar por `CreatureCmdCompatibility.Damage`; las sobrecargas estables de cinco
  parámetros con `dealer` explícito pueden seguir directas. Los hooks `AfterSideTurnStart` deben
  filtrar con `participants.Contains(Owner)`, como `PoisonPower` vanilla, para respetar turnos extra
  en multijugador.
- **Participantes de turno (2026-07-22)**: `CombatSide.Player` no implica que todos los jugadores
  participen. En cooperativo, los turnos extra omiten a los otros personajes de `participants`; un
  guard basado solo en `side` resetea sus contadores, consume poderes temporales o dispara efectos
  fuera de su turno. Los hooks propios deben comprobar `participants.Contains(Owner)` (o
  `Owner.Creature` para reliquias). Se conservaron los guards por lado cuando el efecto pertenece
  deliberadamente a la fase contraria, por ejemplo la cobertura de Lahmu al comenzar el turno
  enemigo y la expiración de Espinas/Espalda Expuesta al terminarlo.
- Referencias aisladas: `.compat/sts2-main-0.107.1` y `.compat/sts2-beta-0.109.0`; no apuntar la
  compilación de compatibilidad a la rama que esté montada en Steam.
- **`CardPlay` debe conservarse hasta el modificador concreto**: en BETA, preview entrega `null` y
  una resolución real entrega la jugada. Un adaptador que sólo invoque el override MAIN mantiene la
  compatibilidad binaria pero pierde esa señal y obliga a heurísticas de estado. `IFgoDamageHooks`
  es el contrato estable de seis argumentos; MAIN le pasa `null`, BETA nativa pasa el valor real y
  el artefacto universal MAIN lo recupera con un bridge Harmony por firma exacta.
- **Los helpers de recursos deben preservar `PlayerChoiceContext`**: manifestar una NP, aplicar su
  marker o generar NP/estrellas desde un hook forma parte de la misma resolución sincronizada. Los
  overloads viejos se conservan para consumidores binarios, mientras el código del repositorio usa
  las variantes con contexto. Los eventos de medidor iteran y esperan cada suscriptor; invocar un
  multicast async directamente sólo espera la última `Task`.
- **La matriz BETA no produce el paquete distribuible**: al terminar deja DLL compilados contra las
  firmas nuevas. Hay que publicar nuevamente contra MAIN, cuyo bridge se verificó cargando sobre el
  runtime BETA. Procedimiento y contrato completo en `docs/COMPATIBILITY-0.109.md`.

## v0.107.1 — cambios de API (verificados con ilspycmd sobre sts2.dll)
- Hooks de inicio de turno: `AfterSideTurnStart(CombatSide, IReadOnlyList<Creature> participants, ICombatState)`; `BeforeSideTurnStart` igual +participants.
- `AfterPowerAmountChanged` gana `PlayerChoiceContext choiceContext` **primero**.
- `AfterTurnEnd`/`BeforeTurnEnd` **eliminados** → `AfterSideTurnEnd`/`BeforeSideTurnEnd(PlayerChoiceContext, CombatSide, IEnumerable<Creature>)`.
- `PowerCmd.Apply<T>` / `ModifyAmount` toman `PlayerChoiceContext` primero.
- `CardPileCmd.AddGeneratedCardToCombat(card, PileType, Player? creator, pos)` (antes `addedByPlayer`; `CardModel.Owner` ES el Player).
- `AttackCommand.Results` → `IEnumerable<List<DamageResult>>`; `CombatState : ICombatState`.
- **Regla de contexto**: si el hook da `choiceContext`, pasalo; si no, `new BlockingPlayerChoiceContext()`; nullable → `choiceContext ?? new BlockingPlayerChoiceContext()`.

## Auditoría integral 2026-07-16 — reglas nuevas
- **No crear un contexto nuevo durante una resolución existente**: robos, daño, selección y
  manifestación disparados por un hook deben reutilizar su `PlayerChoiceContext`. Para helpers
  compartidos, conservar la firma pública y agregar un overload con contexto evita romper DLL
  dependientes. Los robos mid-play además deben limitarse al mazo actual: reshufflear puede recuperar
  la carta que todavía está resolviéndose y corromper su pertenencia a `CombatState`.
- **`ICommandTyped` es un contrato mecánico, no decorativo**: toda carta que llama
  `ConsumeAllForNpCard` debe declarar Buster/Arts/Quick, y las cartas básicas de comando también.
  Un personaje cuya reliquia inicial no hereda de `BondRelic` debe llamar
  `CommandBonusPower.EnsureInstalled`; de lo contrario las cartas tienen tipo pero nunca reciben el
  bonus compartido.
- **Toda reliquia `RelicRarity.Starter` debe aparecer en `Character.StartingRelics`**. Declarar la
  rareza no la vuelve alcanzable. Cada personaje también necesita exactamente un store inicial que
  implemente `INpLevelStore`, o el sistema de dupes y nivel NP queda desconectado.
- **SimpleLoc interpreta sintaxis dentro del texto**: `(...)` puede tratarse como pluralización y
  `+`/`-` como marcadores de upgrade. Los paréntesis explicativos y signos literales se escriben
  `/(`, `/+` y `/-`. `tools/audit_simpleloc.ps1` debe ejecutarse junto al parseo JSON y la paridad de
  claves eng/esp/zhs.
- **Subclases locales de modelos vanilla también necesitan `ICustomModel`**. Sin esa interfaz,
  BaseLib no prefija su ID y dos mods pueden registrar el mismo power temporal.
- **Glows y previews se evalúan fuera de combate**: cualquier helper consultado por
  `ShouldGlowGoldInternal` debe aceptar ausencia de `CombatState` y devolver un valor neutro.
  Los hooks de turno/cambio de forma deben validar además `PlayerCombatState`; `OnPlay`, en cambio,
  puede declarar con `!` la precondición de combate que garantiza el motor.
## Gotchas de build (Tiamat, v0.107.1)
- `PileType` válido: `None, Draw, Hand, Discard, Exhaust, Play, Deck` (NO existe `DrawPile`/`DiscardPile`).
- Los overloads internos de `Curses.MostCursed` que recorren enemigos toman `CombatState` concreto,
  pero existe un overload seguro por `Creature` para previews/glows; devuelve `null` fuera de combate.
  En hooks que requieren el estado concreto, usar pattern matching y salir si no está disponible.
- Cada `PowerModel` necesita loc `title/description/smartDescription` o el build tira `STS001`.

## Deploy / entorno (CRÍTICO)
- Install en uso: `G:\SteamLibrary\steamapps\common\Slay the Spire 2` (v0.107.1). **El juego se movió de biblioteca Steam (C:→G:) el 2026-06-25**; el viejo `C:\Program Files (x86)\Steam\...` quedó con restos (solo `mods/` + `window_state.json`, sin `data_sts2_windows_x86_64`). El build apunta a G: vía `Sts2Path` en los `Directory.Build.props` (machine-local, gitignored); `tools/install-mod.ps1` usa G: por default. Workshop ahora en `G:\SteamLibrary\steamapps\workshop\content\2868840\`.
- El tool **Bash corre SANDBOXEADO** con overlay de FS → para tocar el FS real usar `dangerouslyDisableSandbox:true` (Bash) o la tool PowerShell.
- dotnet **10.0.301** (`/c/Program Files/dotnet/`); MegaDot 4.5.1 en el repo; `ilspycmd` en `/c/Users/YX14n/.dotnet/tools/`.
- **godot.log**: `C:\Users\YX14n\AppData\Roaming\SlayTheSpire2\logs\godot.log` — PRIMER lugar a diagnosticar un mod que no carga.

## VRAM / preload (base del mod de optimización)
- BaseLib carga assets vía `PreloadManager.Cache` (`GetTexture2D`/`GetScene`/`GetAsset`); los modelos declaran qué necesitan con **`GetAssetPaths(IRunState runState)`** (virtual del juego que BaseLib overridea).
- El parámetro `IRunState` ⇒ la carga está pensada para ser **por-run** → palanca para "cargar solo el personaje elegido" (la idea del mod de optimización). *(A confirmar: si hoy precarga TODOS en el menú (eager) o ya por-run.)*
- Síntoma medido: **~120 mods → VRAM 4.7GB en el menú, 320s de arranque**.
- **CONFIRMADO (2026-06-26, reportes de players + código)**: `FormVisuals.Apply` (FGOCore) llamaba `PreloadAll()` que cargaba en VRAM **TODAS las formas de TODOS los mods FGO instalados** (`Registered` es un flat-list de todos los `RegisterFrames` de cada char) a un `Cache` **estático nunca liberado** — aunque jugaras un solo char. Con N chars FGO instalados ⇒ N×(cientos de frames) pinneados. Síntomas de players: en GPU débil las texturas de cartas/intención/NP **no se alocan → "solo barras de vida"** (injugable); con 3 chars instalados → **crash** (掉帧→闪退); multi = "massive performance issues". El usuario (RTX 4080/16GB) NO lo reproduce. **Fix**: `FormVisuals` agrupa por-char (`RegisterFrames` = 1 grupo) y `Apply` solo precarga el grupo del char que pelea (`PreloadGroup`), no todos.
- **REMANENTE CO-OP (2026-07-22, reporte + código)**: el arreglo por grupo seguía precargando todas las formas alternativas de **cada jugador FGO activo**. Con varios personajes animados en la misma partida, la suma todavía podía dejar la pantalla negra por falta de VRAM. **Fix**: en solitario se conserva la precarga del grupo completo; con más de un jugador, `FormVisuals` solicita y retiene sólo la forma actual de cada criatura. Los cambios posteriores siguen siendo asíncronos y mantienen el sprite anterior hasta que la nueva forma esté lista.
- **Textura de frames**: `compress/mode=1` es *lossy de DISCO* pero **descomprime a RGBA8 en VRAM** (`"vram_texture": false`) → VRAM = nº_frames × (lado² × 4 × 1.33 mipmaps). A 1024 ≈ 5.6MB/frame; **Mash tenía `size_limit=0` (sin cap, ~1900px ≈ 19MB/frame ≈ 6GB solo Mash)**. **Fix**: capear los frames de los 4 animados a `process/size_limit=768` (~44% menos VRAM vs 1024; ~6× para Mash). Cambiar el `.import` re-importa solo (no hay que borrar `.godot/imported`); el `.pck` baja como verificación.
- **Cap global de personajes (2026-07-22)**: los 3.354 WebP de combate de los nueve personajes usan ahora `process/size_limit=768`. Los ocho que seguían en 1024 reducen aproximadamente 44% su peor caso de VRAM; Gilgamesh reduce ~22% porque su fuente máxima era 867 px. Las escalas de escena y de cada forma se compensaron con el factor real para que cabeza y pies no cambien de lugar.
- **NO usar `compress/mode=2` (VRAM Compressed)**: guarda BC7 en DISCO → infla el `.pck`/descarga de Workshop ~10×. `size_limit` baja VRAM **y** disco. (`mode=4` Basis = chico en ambos pero con pérdida; no probado.)
- ⚠️ Un **re-render** regenera los `.import` con el default (sin cap) → hay que **re-capear los frames a 768** (cf. Mash, que quedó sin cap).

## Multiplayer / determinismo (CRÍTICO — verificado vs decompiled, 2026-06-26)
El combate de StS2 es **lockstep determinista**: cada cliente simula a TODOS los players (`CombatState.IterateHookListeners` enumera powers de criaturas locales **y remotas**). Las recompensas/fuera-de-combate son **per-player, sincronizadas por RESULTADO** (`RewardSynchronizer`): solo el dueño ejecuta la lógica y manda el resultado.
- **RNG — regla**: `RunState.Rng.*` (incl. `CombatCardGeneration`) es un stream **COMPARTIDO a nivel-run**, consumido dentro del combate sincronizado; el host lo re-sincroniza antes de cada combate (`CombatStateSynchronizer`). **NUNCA consumirlo en un flujo local-only** (card-reward, dupe roll, eventos per-player) → desfasa su `Counter` solo en un cliente → **divergent states** (desync intermitente hasta el próximo combate). Para decisiones per-player usar **`player.PlayerRng.Rewards`** (semilla `seed ^ NetId`, no participa de la simulación). **CONFIRMADO**: `NpLevels.TryRollDupe`/`TryRollDupeWithConsolation` usaban `CombatCardGeneration` en el card-reward de los 8 relics-store → reportado como "multiplayer divergent states". Fix = `PlayerRng.Rewards`.
- Las pantallas de elección (`CardSelectCmd.FromChooseACardScreen`/`FromDeckForUpgrade`/`FromDeckForEnchantment`) **SÍ son MP-safe** (usan `PlayerChoiceSynchronizer`: reserva choiceId + `ShouldSelectLocalCard`/`WaitForRemoteChoice`, igual que relics vanilla). No eran el desync.
- Leer `player.PlayerCombatState.AllPiles...DrawPile.Cards.Count` **es determinista cross-client** (todos los `PlayerCombatState` se pueblan para locales Y remotos en `CombatManager.SetUpCombat`, barajados con el `RunState.Rng.Shuffle` sincronizado). El cap de robo en cambio de forma es seguro.
- `ShouldScaleInMultiplayer` solo afecta el escalado de powers cuyo target es un enemigo (`MultiplayerScalingModel`); para self-buffs (Fuerza/★/NP propio) `false` es correcto y nunca crashea.
- **Carga síncrona = desconexión en MP**: `ResourceLoader.Load<T>()` de un `.tres` pesado **congela el hilo de simulación** → rompe el heartbeat de red → timeout/desconexión (se reporta como "crash"). **CONFIRMADO**: `FormVisuals.GetFrames` tenía un `Load()` síncrono de fallback → la **forma Ortinax de Mash crasheaba en multi** (Ortinax es la forma que se entra MID-combat materializando otro `.tres`; Shielder no, porque entra al inicio con los frames ya cargados). Fix = `GetFrames` nunca bloquea (solo devuelve si `LoadThreadedGetStatus==Loaded`, si no `null`); `Apply` aplica el sprite diferido vía el signal `process_frame` cuando el background-load termina. El grupo se precarga al inicio del combate (la `Apply` de la forma base dispara `PreloadGroup` del grupo entero), así el switch mid-combat suele estar listo.

## Hooks de daño / preview (CRÍTICO — verificado, 2026-06-27)
- **`ModifyDamageAdditive`/`ModifyDamage*` NO deben MUTAR estado.** El hook corre TAMBIÉN en modo PREVIEW (cálculo del número que se muestra), y el hook por-power **NO recibe el `previewMode`**: `Hook.ModifyDamage(...)` sí tiene un param `CardPreviewMode previewMode` (`decompiled/.../Hook.cs:1486`) pero al iterar los listeners llama `item.ModifyDamageAdditive(target, num, props, dealer, cardSource)` **sin** reenviarlo (`Hook.cs:2519`). ⇒ un power no puede distinguir preview de real dentro de ese hook. Si mutás ahí (p.ej. `_pending = 0` para "una vez por golpe"), una preview que ocurra **después** de cachear el bono se lo come y la pegada real recibe 0. **Patrón vanilla correcto = PURO**: `PhantomBladesPower.ModifyDamageAdditive` decide leyendo el historial (`CombatManager.Instance.History.CardPlaysFinished`), nunca muta.
- **Para "bono una vez por carta/golpe"**: cachear en `BeforeCardPlayed` (real, las previews no lo
  invocan), devolver el bono PURO en `ModifyDamageAdditive`, y **limpiar el caché en
  `AfterDamageGiven`** tras la primera pegada real. Ese hook no corre en preview y, a diferencia de
  `AfterDamageReceived`, también corre si el objetivo muere. **CONFIRMADO**: la *Sentencia* de Morgan
  (`FairyQueen`/`WinterQueenFormPower`) y el *Bunker Bolt* de Mash (`MashFormPower`) mutaban en el
  hook de cálculo; luego, al mover la limpieza sólo a `AfterDamageReceived`, los golpes letales aún
  dejaban el bono vivo para el siguiente impacto. Fix: 3 powers con lectura pura y limpieza en
  `AfterDamageGiven`.

## Mods de OTROS autores rotos (desuscribir) — 2026-06-25
- **Crash al iniciar run**: NRE en `HsrSimulatedUniverseCurios` (`CarnivalsTailPatch.OnRelicObtained`) disparado por `ReAstralPartyMod` (reliquia inicial). Workshop `3747553484` / `3747579249`.
- **Downfall/Automaton** (`3747508091`): `HarmonyException` al cargar — no puede patchear `OnPlay` en v0.107.1.
- **LittleWizard** (`3747560296`): arte de carta roto (`element_burst.png`).
- **Crash al arrancar, Archetto + RitsuLib** (`3747563715` + `3747602295`, verificado
  2026-07-16): `MissingMethodException` al construir `Archetto.Cards.Uncommon.CoPerformance`.
  Archetto público fue compilado contra `ModCardTemplate(..., bool, bool)` / RitsuLib 0.4.31,
  mientras MAIN carga RitsuLib 0.4.57. Steam confirma que el manifest público de Archetto sigue en
  la build antigua; la build nueva está asociada a otra rama. Fix seguro: desactivar Archetto en
  MAIN hasta que el autor republique una build pública compatible. No bajar RitsuLib: muchos otros
  mods dependen de la versión actual. `YukiMod` también registra `res://MainFile.cs` dos veces, pero
  su excepción queda atrapada y no es la que aborta `NGame.GameStartup`.
- **Pantalla negra al continuar una run, `AncientWaifus_Beta` 0.2.5** (Workshop `3759748828`,
  verificado por log 2026-07-23): el propio mod registra que adjunta `LayoutEnforcer` y, desde el
  fotograma siguiente, `LayoutEnforcer._Process` intenta convertir su padre `Godot.SubViewport` a
  `Godot.Control`. El `InvalidCastException` se repite cada fotograma y además rompe el logger de
  Godot. En el caso observado, Mordred completa la precarga de sus 111 assets antes de entrar a una
  sala de Evento; no aparece ninguna excepción de Mordred/FGOCore. Mitigación: actualizar o
  desactivar `AncientWaifus_Beta`; cambiar entre Vulkan/OpenGL no corrige un cast de tipos de C#.

## Conflicto node-factory / visuals de BaseLib (CRÍTICO — verificado vs decompiled, 2026-06-27)
- **Síntoma (reporte de player, combo pesado de mods: figure_Saya + Kafka + chars FGO)**: al continuar/entrar a combate con un char FGO → `InvalidCastException: 'Godot.Control' → 'NCreatureVisuals'` en `CharacterModel.CreateVisuals` (stack: `NCombatRoom.CreateAllyNodes`). Se reporta como "crash al arrancar" pero **el juego llega al menú**; revienta al crear la sala de combate.
- **Causa**: `CharacterModel.CreateVisuals()` (juego, `decompiled/.../CharacterModel.cs:210`) hace `GetScene(VisualsPath).Instantiate<NCreatureVisuals>()` — **asume** que el patch global de BaseLib (`SceneConversionPatch`→`NodeFactory.TryAutoConvert`, registro estático `_registeredScenes`/`_factories`) ya convirtió la raíz de la escena-mod a `NCreatureVisuals`. Cuando OTRO mod **empaqueta su propia BaseLib forkeada** (p. ej. `figure_Saya.ModSupport`: loguea "Created node factory for NCreatureVisuals", carga DESPUÉS) compite por ese patch → la conversión no corre para nuestra escena → raíz `Control` → cast revienta. **NO es nuestro bug ni lo causó un update** (es el combo del player).
- **Fix (nuestro, robusto — 2026-06-27, los 9 chars)**: override `CreateCustomVisuals()` → `NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath)`. El prefix `UseCustomVisuals` (BaseLib) setea `__result` y **saltea el `CreateVisuals` original frágil**. `CreateFromScene` usa el `_instance` estático de NUESTRA `NCreatureVisualsFactory` (la BaseLib real contra la que compilamos) → inmune al clobber del fork. Resultado idéntico en setups normales (mismo `CreateFromNode`). Defensivo: `string.IsNullOrEmpty(CustomVisualPath) ? null` → comportamiento original.
- **Rest-site / merchant (2026-06-27, reporte player "进商店黑屏"):** el MISMO mecanismo frágil lo usan `NRestSiteCharacter`/`NMerchantCharacter`, pero **no hay hook** `CreateCustom*` para ellos. Síntoma: **pantalla negra al entrar a la hoguera/tienda** con personajes de escena propia (Mash/Morgan/Artoria; Tiamat usa placeholder ironclad vanilla → no debería). **Fix shipped** (`FGOCore/FGOCoreCode/SceneFactoryHardening.cs`): transpilers de Harmony sobre `NRestSiteCharacter.Create` y `NMerchantRoom.AfterRoomIsLoaded` que cambian SOLO la llamada `GetScene(path).Instantiate<T>()` por `NodeFactory<T>.CreateFromScene` (factory de NUESTRA BaseLib, inmune al clobber). Guarda: no-op si no matchea el patrón IL; idéntico en setups sin conflicto; API pública de FGOCore intacta (no requiere rebuild de personajes). **Sin test runtime** (no se pudo reproducir local sin figure_Saya). **Energy-counter** sigue sin cubrir (menor).

## Pipeline de arte CE
- `.claude/workflows/match-ce-art.js`: batches `[file, themeEn]` → CE `collectionNo` (catálogo `assets/reference/ce/ce_names.tsv`, 2611 CEs, formato `collectionNo<TAB>assetId<TAB>name`).
- Aplicar: mapear `collectionNo`→`assetId` (col 2) → `tools/make_card_art.ps1 -MappingCsv <csv> -OutDir <mod>/images/card_portraits` baja de Atlas Academy y recorta a 500×380 + `big/` 1000×760.

## Contenido vanilla que ASUME propiedades del card-pool del personaje (CRÍTICO — verificado, 2026-06-30)
Clase de crash cross-character: reliquias/eventos del juego base filtran `character.CardPool` por **tag** o **rareza** y hacen un sink que **tira en vacío** (`.First(pred)` → InvalidOperationException; `Rng.NextItem(empty)` → devuelve `default`/null y el `.Id`/`.X` posterior → NullReferenceException). Nuestros Servants FGO no cumplen ciertas asunciones que tienen los 5 personajes vanilla → softlock/crash. **Regla**: todo personaje debe tener un Basic con tag `Strike`, un Basic con tag `Defend`, y al menos una carta por rareza estándar (Common/Uncommon/Rare). Las cartas de rareza `Ancient` NO las tenemos (salvo Siegfried) y NO es viable agregarlas → se blinda el consumidor.
- **`LargeCapsule` (巨大扭蛋, Ancient)** `AfterObtained`: `CardPool.AllCards.First(Basic && Tags.Contains(Strike))` y `.First(... Defend)` → crash al obtenerla si falta el tag. Afectaba 9/10 (solo Mash tenía ambos). **Fix**: taggear el Basic Strike/Defend de cada personaje (`CanonicalTags => { CardTag.Strike }` / `{ CardTag.Defend }`). Cubre además **toda** la familia Strike/Defend: `Fasten`, `GhostSeed` (幽灵种子 — antes no-opeaba sin tag, reportado en Artoria como "no da 虚无"), `SoldiersStew`, `NutritiousSoup`, `NeowsTalisman`, `LeafyPoultice`, `Amalgamator`, `Tezcatara`.
- **`DustyTome` (la ofrece el Ancient `Darv`/达弗, Acto 2)** `SetupForPlayer`: `player.PlayerRng.Rewards.NextItem(pool.Where(Rarity==Ancient)).Id` → NRE para todo char sin cartas Ancient (9/10). Darv lo ofrece ~50% (`Rng.NextBool()`) y llama `SetupForPlayer` AL GENERAR LAS OPCIONES → el evento se cuelga antes de mostrar reliquias ("solo diálogo, no sale reliquia, no avanza"). **Fix**: Harmony prefix en `DustyTome.SetupForPlayer` (`FGOCore/FGOCoreCode/DustyTomeHardening.cs`) → si no hay cartas Ancient, cae a Raras (luego cualquiera) del propio pool. **Compatibilidad confirmada 2026-07-22**: Acheron (`DustyTomeExcludeFinalStrikePatch`) instala otro prefix que repite `NextItem(empty)` y podía ejecutarse antes del hardening, reabriendo el bloqueo reportado con Tiamat + AncientAffection + AncientWaifus. El hardening ahora usa `Priority.First` + `HarmonyBefore("Acheron")`; cuando aplica devuelve `false`, por lo que Harmony omite el prefix inseguro posterior. API pública de FGOCore intacta (ship solo FGOCore).
- **`CardFactory.CreateForReward` (TODA recompensa de cartas — verificado 2026-07-06)**: elige las N opciones (típico 3) UNA por una con `GetPossibleCards(player).Except(blacklist)` (las ya elegidas van a la blacklist → opciones DISTINTAS); si el pool filtrado menos la blacklist queda vacío, `Rng.NextItem` → null → `InvalidOperationException("couldn't generate a valid card")` (`CardFactory.cs:239`). Lo pisa cualquier filtro rareza×tipo sobre un pool chico: el evento **`TheFutureOfPotions` (药水的未来) pide 3 DISTINTAS de (rareza de la poción × tipo rolleado)** y Tiamat tenía combos con 1-2 cartas (U/Poder=1; R/Ataque=R/Habilidad=R/Poder=2; C/Ataque=2) → "无法选牌" (reportado 2×; ídem el evento 天命芝士 de OTRO mod — mismo sistema). **Fix doble**: (a) `FGOCore/FGOCoreCode/CardRewardHardening.cs` — Harmony **finalizer** que ante esa excepción reintenta el MISMO método con blacklist vacía (permite duplicados en vez de reventar; guard `_retrying` contra la recursión del detour; si el reintento también falla, propaga como vanilla; MP-determinista: consume el mismo stream de RNG en todos los clientes); (b) pool de Tiamat +6 cartas para que todo combo alcanzable por el evento tenga ≥3. **Regla ampliada**: todo personaje quiere **≥3 cartas por (rareza estándar × tipo)**; la única excepción tolerable es Común/Poder (el evento excluye Poder para pociones Common/Token, y el hardening cubre el resto de consumidores).
- **Audit (2026-06-30)**: el resto de picks del pool quedó **safe** — `ScrollBoxes` (Common×2 + Uncommon×1; todos los chars tienen ≥2 Common y ≥1 Uncommon, mínimo Tiamat 6/16), `VexingPuzzlebox` (pool completo, no vacío), `FishingRod` (null-checked), `Bookmark` (`?.`). Las asunciones de OTROS tags (`Shiv`/`Minion`) son chequeos condicionales (`if (!Tags.Contains(...))`) → no-op, no crashean. **No quedaron crashes de esta clase sin cerrar.**

## Daño en hooks de turno DEBE usar el choiceContext del hook, no uno nuevo (CRÍTICO — verificado, 2026-06-30)
Reporte de player (entorno multi-mod): "termino el turno pero no se resuelve (无法结算)" — el turno se CUELGA cuando un efecto hace **daño de borde-de-turno que MATA a un enemigo**. Reproductor claro: el **enjambre de Tiamat** matando a un bicho. Le pasa con varios personajes (patrón general de mods, no solo el nuestro).
- **Causa**: los hooks de fin/inicio de turno reciben un `PlayerChoiceContext choiceContext` que es el contexto **sincronizado del flujo de resolución del turno**. Si un efecto de daño usa `new ThrowingPlayerChoiceContext()` (u otro contexto FRESCO) en vez del pasado, y ese daño MATA, la muerte queda FUERA del flujo → el turno no puede resolverse (cuelga, sobre todo en MP donde el settle requiere sincronización). Los powers vanilla de fin-de-turno (`MagicBombPower`/`HailstormPower`/`TheBombPower`) SIEMPRE dañan con el `choiceContext` del hook.
- **Hooks y contexto**: `BeforeSideTurnEnd`/`AfterSideTurnEnd`/`BeforeSideTurnStart` traen `choiceContext` (usarlo). `AfterSideTurnStart(CombatSide, IReadOnlyList, ICombatState)` NO trae → para dañar enemigos al inicio del turno mover el hook a `BeforeSideTurnStart(choiceContext, ...)`.
- **CONFIRMADO/fix (2026-06-30)**: `TidalSwarmPower.AfterSideTurnEnd` (Tiamat) y `LahmuSwarmPower` (FGOCore; movido de `AfterSideTurnStart`→`BeforeSideTurnStart`) dañaban con `new ThrowingPlayerChoiceContext()` → cuelgue al matar. Fix = usar el `choiceContext` del hook. Audit del resto: `DebtPower`/`VortigernPower` (Oberon), `AbsoluteWallPower`/`CoverPower` (Mash) YA usaban el pasado (OK). `CursePower` self-daña al enemigo en su propio turno (espejo de PoisonPower vanilla, que funciona) → se deja.

## `AttackCommand.FromCard` cambió también en BETA 0.108.0 (CRÍTICO — verificado por log, 2026-07-22)
- **Síntoma**: una carta de ataque queda ampliada en pantalla y no se resuelve. El log registra
  `PlayCardAction ... completed with exception`.
- **Causa confirmada**: un DLL compilado contra MAIN invoca directamente
  `AttackCommand.FromCard(CardModel)`, pero el host 0.108.0 sólo expone la variante que recibe
  también `CardPlay`. El caso observado fue `MASHSHIELDER-ARTS_MASH`.
- **Regla**: ninguna carta FGO debe invocar `FromCard` directamente ni depender del helper de una
  revisión concreta de BaseLib. Usar `FromCardFgoCompatibility`, cuyo delegado se resuelve una vez
  en FGOCore y cubre las firmas `(CardModel)` y `(CardModel, CardPlay)`.
- **Audit**: 223 sitios migrados en FGOCore y los nueve personajes; los artefactos MAIN/BETA no
  contienen referencia binaria a `BaseLib.Utils.BetaMainCompatibility.FromCardCompatibility`.

## La barra de vida vive en y=0 del nodo del creature — los PIES del sprite deben pisar y=0 (verificado, 2026-07-06)
Reporte post-normalización de escala ("人物下降到血条在胯部" — el personaje se hunde y la barra queda a la altura de la entrepierna). **Cómo posiciona el juego la barra**: `NCreatureStateDisplay.SetCreatureBounds` → `NHealthBar.UpdateLayoutForCreatureBounds(bounds)` usa los Bounds SOLO para **X y ancho** (`HpBarContainer.GlobalPosition.X` / `Size.X`); la **Y es fija** respecto al origen del nodo del creature (la línea de piso, y=0). Conclusión: si el sprite queda con los pies POR DEBAJO de y=0, la barra "sube" visualmente por el cuerpo. La normalización a 700px (2026-07-04) corrigió `scale` pero dejó `position.y` con los pies hundidos en TODOS los personajes (medido con el bbox alfa del frame idle: Okita +608px, Siegfried +595, Oberon +426, Mash +330, Mordred +312, Artoria +266, Morgan +219, Tiamat +131; solo Gilgamesh casi bien +19). **Fix** (`scratchpad/fix_feet.py`, 2026-07-06): `position.y = (H/2 − bbox.bottom) × scale` (sprite centrado → pies en y=0) en los 9 `*_visuals.tscn`, + `Bounds.offset_top = −(altoFigura+10)`, `IntentPosition.y = −(altoFigura+40)`, `CenterPos.y = −altoFigura/2`. **Regla**: al escalar/mover un sprite de combate, verificar SIEMPRE pies→y=0 con el bbox alfa; los Bounds NO mueven la barra en Y (solo ancho/X, targeting y layout de powers/intents).

### Corrección global: 700 px coloca modelos y burbujas de diálogo en el techo (2026-07-16)
El ajuste global posterior dejó ocho personajes entre ~687 y ~795 px; solo Mash se había reducido a
~484 px. Aunque los pies quedaran cerca de `y=0`, la figura alcanzaba el borde superior del combate.
Además, `NSpeechBubbleVfx.GetCreatureSpeechPosition` deriva la burbuja desde `CenterPos` y el alto de
`Bounds`; valores extremos como Siegfried (`Bounds=-992`, `CenterPos=-491`) colocaban el diálogo en
`y≈-863` relativo al suelo, fuera de pantalla. Fix: normalizar la forma inicial de los nueve a ~484 px
(Tiamat Bestia conserva ~558 px), mantener pies→`y=0` y recalibrar Bounds/Intent/Center; las burbujas
quedan en `y≈-438…-473`. No tomar el bbox alfa bruto: algunos renders contienen píxeles aislados que
falsifican los bordes; ignorar filas/columnas con menos de una masa mínima de píxeles.

### Black Grail con Replay y waiver de Pioneer (verificado, 2026-07-16)
El reporte del 2026-07-04 ya estaba cubierto por `72233028`. Replay ejecuta `CardModel.OnPlay` por cada
repetición (`CardModel.Play`, bucle `i < playCount`) y Black Grail ahora es `PowerStackType.Counter`, por
lo que la segunda aplicación suma Amount mediante `PowerCmd.ModifyAmount`; daño y pérdida de HP escalan
por acumulación. Pioneer tampoco gasta Carga para la primera NP manual del mazo: `ConsumeAllForNpCard`
marca el waiver usado y omite `Spend`. Las ults auto-manifestadas son `CardRarity.Event` y se excluyen
deliberadamente en `GetWaiver`; la localización eng/esp/zhs ya lo declara.

## Workshop: "Limit exceeded" = previewfile > 1MB, NO es rate-limit (verificado, 2026-07-05)
SteamCMD `workshop_build_item` falla con `ERROR! Failed to update workshop item (Limit exceeded).` cuando el `previewfile` supera ~1MB — el mensaje despista a rate-limit (perdimos ~2 días reintentando "cuando resetee el cap"). Diagnóstico que lo delató: los mismos 3 ítems fallaban SIEMPRE mientras otros 7 subían bien en la misma sesión; los 3 eran exactamente los de preview >1MB (2.9/2.8/1.4MB) y los que subían pesaban <600KB. Fix: preview a JPG ≤1024px q88 (~200KB). Regla para `tools/`: todo previewfile se genera ≤1MB.
