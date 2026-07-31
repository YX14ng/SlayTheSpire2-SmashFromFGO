# STATUS — estado actual (alta densidad)

Backlog canónico de futuros personajes: [`CHARACTER-TODO.md`](CHARACTER-TODO.md).

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
- **Pendiente externo:** publicar FGOCore y Siegfried juntos y confirmar el comportamiento en juego.
  Steam Workshop no fue modificado en este hotfix.

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
3. Telemetría RitsuLib (futuro).

## Bloqueado / a decidir
- ✅ **Resuelto (2026-06-25)**: el juego no estaba desinstalado — se **movió de biblioteca Steam a `G:\SteamLibrary\steamapps\common\Slay the Spire 2`** (el viejo C: quedó con restos). `Sts2Path`→G: en los Directory.Build.props. **Build verificado end-to-end**: FGOCore + los 7 personajes con fixes + Tiamat compilan VERDE → `dist/` (solo faltaba 1 `using` en Gil, arreglado). Falta: **playtest** (balance) + **publish/install**.
- Cómo hacer Tiamat jugable para playtest: **local-rápido vs re-publish a Workshop** (pendiente decisión del user).
- NP fixes que requieren decisión del user: Okita romaji vs EN oficial; Artoria "Hopewill"/"Round of Avalon". (Siegfried `失坐`→`失坠` es fix claro, aplicar.)

## Regla de mantenimiento
Antes de cerrar una sesión, actualizar **STATUS / next-task / HANDOFF**; no dejar que el código quede adelante de los docs. "Instalado" ≠ "validado en juego": leer el `godot.log` y probar.
