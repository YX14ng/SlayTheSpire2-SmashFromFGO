# Diseño integral — Shuten Dōji Assassin/Caster

> **Estado:** diseño cerrado; listo para implementación, producción de assets y playtest
> **ID reservado:** `ShutenDouji` — un solo personaje y un solo mod; no cambiar después del scaffold
> **Personaje:** Shuten Dōji, integrando sus Saint Graph Assassin y Caster sin separar el pool
> **Pool previsto:** 20 comunes, 28 poco comunes, 20 raras, 5 cartas iniciales distintas y 2 NP Event

## 1. Fantasía, identidad y límites

**Fantasía:** dirigir un banquete venenoso como la oni de Monte Ōe y, cuando la presa queda expuesta,
vestir el papel de oni guardiana para castigarla a golpes. La seducción prepara el terreno; el
exorcismo cobra la cuenta.

**Identidad en una frase:**

> La anfitriona oni que destila control y Veneno en un banco de Sake, y decide si conservarlo para
> prolongar el banquete Assassin o gastarlo en defensa y ráfagas Caster; al cargar NP elige entre
> intoxicar a todo el campo o ejecutar a una sola presa.

**Verbos:** servir, intoxicar, castigar.

**Rol:** carry de Veneno/control con una segunda ruta de daño directo multiimpacto, defensa reactiva
y apoyo menor. Es especialmente buena convirtiendo una apertura metódica en un turno de remate.

**Debilidad estructural:** tiene 68 de Vida y defensa básica modesta. Assassin sobrevive reduciendo
la amenaza antes de que ocurra; Caster necesita Sake para que sus números defensivos premium se
enciendan. Una mano Caster sin banco y una mano Assassin contra un enemigo que limpia estados son
deliberadamente menos eficientes. Ninguna rara elimina a la vez ambas fricciones.

No hay cambio de forma mecánico, bono pasivo de clase ni multiplicador global nuevo. Las dos
versiones son **Estilos de carta** dentro del mismo personaje. La apariencia puede elegirse como
skin antes de la run, pero nunca altera reglas, números ni recompensas.

## 2. Base canónica investigada

### 2.1 Assassin

- Servant 112, Assassin 5★, atributo Earth, alineamiento Chaotic Evil y baraja FGO QQAAB.
- `果実の酒気 A` combina encanto colectivo y reducción de defensa; el perfil explica que su voz,
  aliento y mirada pueden embriagar.
- `鬼種の魔 A` mejora el ataque del grupo y el daño de NP propio.
- `戦闘続行 A+`, luego `鬼の首 EX`, aporta supervivencia y carga: la cabeza cercenada siguió
  atacando tras el banquete en que Raikō la envenenó.
- NP Arts de área `千紫万紅・神便鬼毒` aplica una batería de debilitaciones, Sellado de Habilidad,
  Veneno y, tras su mejora, 蝕毒: aumento del efecto del Veneno.
- El perfil la vincula con Monte Ōe, Yamata-no-Orochi/una divinidad dragón, el sake, los banquetes y
  una afición por antigüedades y objetos raros.

### 2.2 Caster

- Servant 225, Caster 4★ de evento, atributo Earth, alineamiento Chaotic Evil y baraja FGO QAAAB.
- Es el mismo Saint Graph vestido para una función temporal: «amonestar oni, investigar oni,
  matar oni; de ese modo salvar a los oni». Haku (`白`) es su familiar mágico.
- `護法の鬼・心握殺 A+` sella NP y reduce la capacidad crítica; deriva de Heart Break/Bone
  Collector y aplasta órganos vitales.
- `護法の鬼・殴殺棒 B` potencia Arts/Buster y concentra estrellas.
- `鬼種の魔（護） A` mejora el ataque del grupo y su especialidad contra seres Demonic.
- NP Buster individual `護法少女・九頭竜鏖殺` obtiene Sure Hit antes de una paliza de altísima
  velocidad y aplica Veneno después; el propio perfil recalca que el vino venenoso es apenas el
  remate de la ejecución física.
- Su perfil jugable reconoce durabilidad y apoyo bajos a cambio de daño individual alto.

### 2.3 Terminología china simplificada

Mooncell fija las grafías `酒吞童子`, `千紫万红·神便鬼毒`, `护法少女·九头龙鏖杀`,
`护法之鬼·心握杀`, `护法之鬼·殴杀棒` y `鬼种之魔（护）`. Esas formas serán la base de ZHS;
no se traducirán desde inglés.

### 2.4 Fuentes

- [Datos y perfil JP de Shuten Assassin](https://w.atwiki.jp/f_go/pages/957.html)
- [Datos y perfil JP de Shuten Caster](https://w.atwiki.jp/f_go/pages/3670.html)
- [Perfil japonés recopilado en TMdict](https://www.tmdict.com/ja/profile/shuten-douji)
- [Mooncell — Shuten Dōji Caster](https://fgo.wiki/w/%E9%85%92%E5%90%9E%E7%AB%A5%E5%AD%90(Caster))
- [Mooncell — Shuten Dōji Assassin](https://m.fgo.wiki/w/%E9%85%92%E5%90%9E%E7%AB%A5%E5%AD%90)
- [Atlas Academy — catálogo de Servants JP](https://apps.atlasacademy.io/db/JP/servants)

Las cifras y equivalencias de este documento son decisiones de deckbuilder; no son una traducción
literal del sistema de combate de FGO.

## 3. Distinción respecto del roster

| Personaje existente | Centro actual | Lo que Shuten no copia |
|---|---|---|
| Morgan | tres formas y Maldición | no usa formas ni otro daño diferido propio |
| Artoria Caster | tres formas, NP defensivo y críticos | no alterna forma para activar el motor |
| Tiamat | dos mazos/formas y Lahmu | no reemplaza cartas ni invoca unidades |
| Oberon | tres formas, préstamos y Deuda | no compra potencia con daño futuro |
| Kagetora/Kenshin | secuencia de tres preceptos y ascenso irreversible | no exige orden fijo ni transformación |

La combinación exclusiva es: **Veneno nativo compartible + banco propio de Sake + Estilos sin
forma + elección de dos NP**. Silent puede aportar Veneno en cooperativo, pero no controla el Sake;
otra Shuten puede compartir el Veneno, pero cada una administra su propio banco.

## 4. Bucle de combate

1. Jugar Assassin para aplicar Veneno/Débil/Vulnerable y llenar Sake.
2. Decidir si conservar el banco para payoffs Assassin o gastarlo en una carta Caster.
3. Jugar el segundo Estilo cuando el orden y la mano hagan rentable **Cruce**.
4. Usar Quick para preparar Críticos globales y Arts para acercarse al umbral NP.
5. A 100 NP retener los dos desenlaces y elegir el apropiado al encuentro.
6. Repetir sin que la limpieza enemiga borre el Sake ni vuelva inútil el daño Caster.

Arquetipos soportados:

| Arquetipo | Núcleo | Pago | Fricción |
|---|---|---|---|
| Banquete Assassin | Veneno, debuffs, ganancia de Sake | daño diferido, control, NP de área | limpieza/Artifact y poco frontload |
| Oni guardiana Caster | Buster, Bloqueo y gastos de Sake | daño directo, multiimpactos, NP individual | banco corto y defensa base modesta |
| Destilación híbrida | alternar Estilos y Cruce | mejor economía, robo y conversión NP↔Sake | secuenciación y consumo oportuno |
| Quick/crítico | Quick de ambos Estilos y golpes múltiples | estrellas globales y ráfagas por impacto | requiere reunir 100 estrellas antes del remate |

## 5. Estilos, Cruce y Sake

### 5.1 Estilos

- Cada carta propia normal tiene exactamente un tag **Assassin** o **Caster**.
- El tag no es una forma, no modifica estadísticas y no se limpia.
- Los NP tienen su Estilo correspondiente, pero no activan la reliquia inicial.
- Las cartas incoloras, estados y memes de FGOCore no tienen Estilo.
- El Estilo se muestra con una insignia pequeña; nunca se infiere desde el nombre localizado.

### 5.2 Cruce

**Cruce** significa: «ya jugaste al menos una carta del otro Estilo este turno».

- La carta actual se registra después de resolver por completo, por lo que no habilita su propio
  Cruce.
- Cruce es una condición, no una recompensa universal. Solo hace algo si la carta o reliquia lo
  dice.
- El historial se reinicia al comienzo del turno de Shuten.
- Las cartas condicionadas brillan en dorado cuando Cruce ya está activo.

### 5.3 Sake

- Recurso personal de combate de **0 a 100**; empieza en 0 salvo fuentes explícitas.
- Persiste entre turnos, desaparece al acabar el combate y nunca puede ser negativo.
- Es `IResourcePower`: un cleanse no lo quita y Artifact no evita su ganancia.
- Las ganancias usan 10/20/30/50; solo cuenta la cantidad realmente añadida antes del tope.
- «Gasta X Sake: …» paga después del efecto base y solo resuelve el rider si había X completos.
  La carta sigue siendo jugable sin el pago salvo que diga expresamente lo contrario.
- «Gasta hasta X» consume automáticamente el mayor múltiplo de 10 disponible hasta X. La decisión
  es cuándo jugar la carta; la previsualización muestra el gasto y el resultado exactos.
- Los eventos de gasto se emiten una vez por pago agregado, no una vez por cada 10, para impedir
  dobles disparos e infinitos.
- Los gastos y ganancias provocados por NP no activan la reliquia inicial, pero sí reliquias que
  escuchen explícitamente `SakeSpent`.
- Todo Ataque Quick normal genera **+10 estrellas** después de resolverse mediante la regla global
  de FGOCore. Las cantidades «impresas» en una carta se suman a esas 10; los NP quedan excluidos.

### 5.4 Reliquia inicial

**Calabaza Escarlata**:

- al comienzo del combate gana 20 Sake;
- después de la primera carta Assassin de cada turno gana 10 Sake;
- después de la primera carta Caster de cada turno gana 10 Sake;
- máximo 2 disparos por turno, uno por Estilo; los NP están excluidos.

Una build pura recibe 10 por turno y una híbrida 20. El disparo posterior a la resolución impide
que una carta Caster pague su propio rider gratis y deja visible el orden correcto.

## 6. Veneno, estados y respuesta a limpiezas

Se reutiliza `PoisonPower` del juego base. El Veneno daña al comienzo del turno del lado afectado,
es `Unblockable|Unpowered`, pierde una carga por activación y reconoce `AccelerantPower`. No se crea
un Veneno paralelo ni una barra propia.

Estados adicionales:

| Estado | Tipo | Límite | Regla |
|---|---|---:|---|
| Sake | recurso propio | 100 | banco persistente entre turnos del combate |
| Historial de Estilo | contador invisible | 2 marcas | alimenta Cruce y se reinicia cada turno |
| Sello de Habilidad | debuff enemigo | contador | cancela la próxima intención no-Ataque mientras dure; los ataques pasan |
| Próximo Ataque Certero | buff temporal | 1 | el siguiente Ataque propio ignora Bloqueo; luego se consume |
| Gasto por impacto | buff de carta/poder | indicado | bono máximo por carta, nunca un trigger ilimitado |

El **Sello de Habilidad** usa la semántica FGO ya probada por Tiamat: si la intención visible no es
un ataque, la reemplaza por `STUNNED`; si es un ataque, el sello permanece hasta una intención de
habilidad o hasta agotar su duración. No inspecciona mejoras individuales ni altera recursos,
formas o marcadores. Artifact puede bloquearlo; el NP Assassin retira primero 1 Artifact para
representar la reducción de resistencia a debuffs.

La ruta no colapsa ante un cleanse de jefe:

- Sake vive en Shuten y se conserva.
- Todos los ataques Caster tienen daño base útil sin Veneno.
- `Golpe Matademonios` cobra su rider también contra Elite/Jefe limpio.
- `Reprimenda del Oni`, `Gohō no Oni` y el NP Assassin responden a mejoras/Artifact.
- Ambos NP infligen daño inmediato; Veneno nunca es el único clímax.

## 7. Noble Phantasm: un medidor, dos desenlaces

### 7.1 Manifestación compartida

- Medidor FGOCore con umbral 100 y techo por nivel NP: 100/200/300; el Grial y NP6 permiten 400.
  Preparaciones explícitas pueden elevar el OC efectivo hasta 5.
- Al cruzar 100 desde abajo se manifiesta una copia de **cada** NP en la mano.
- Ambos son 0, Retain, Exhaust, rareza Event y no pueden ser Críticos.
- Solo puede existir una copia **viva** de cada uno entre mano, mazo y descarte. Las copias ya
  agotadas son historial y no bloquean el siguiente ciclo.
- La manifestación apunta a la mano; el comportamiento nativo envía al descarte cualquier carta que
  exceda el límite de mano. Nunca se pierde un desenlace por tener una mano llena.
- Si el medidor cae bajo 100, permanecen visibles pero injugables hasta recuperar 100.
- Jugar uno consume todo el NP, agota la carta jugada, elimina al hermano de todas las pilas vivas y
  permite una nueva pareja al volver a cruzar el umbral.
- El nivel NP oculto escala solo el daño base con `NpLevels.Scale`; OC y Sake se suman después.

La doble carta ocupa dos espacios de mano mientras se retiene: es una fricción deliberada por tener
dos respuestas disponibles.

### 7.2 千紫万紅・神便鬼毒 — Senji Bankō, Shinpen Kidoku

NP Arts Assassin, área, un impacto por enemigo.

1. retira 1 Artifact de cada enemigo;
2. inflige daño a todos;
3. aplica 1 Débil, 1 Vulnerable, 1 Sello de Habilidad y Veneno según OC.

| NP Lv | Daño a todos |
|---:|---:|
| 1 | 28 |
| 2 | 32 |
| 3 | 36 |
| 4 | 41 |
| 5 | 45 |
| 6 (Grial) | 49 |

Veneno por OC1–5: **8 / 11 / 14 / 17 / 20**. No gasta Sake. Su función es recuperar el control,
resembrar un campo limpio y preparar al Caster o al equipo; no intenta ser el mayor remate ST.

### 7.3 護法少女・九頭竜鏖殺 — Gohō Shōjo, Kuzuryū Ōsatsu

NP Buster Caster, objetivo único, seis impactos.

1. obtiene Certero durante esta resolución: los seis impactos ignoran Bloqueo;
2. gasta automáticamente hasta 50 Sake, +1 daño por impacto por cada 10 gastados;
3. inflige seis impactos;
4. aplica Veneno según OC.

| NP Lv | Daño base por impacto | Total base |
|---:|---:|---:|
| 1 | 6 | 36 |
| 2 | 7 | 42 |
| 3 | 8 | 48 |
| 4 | 9 | 54 |
| 5 | 10 | 60 |
| 6 (Grial) | 11 | 66 |

OC añade **+0/+1/+2/+3/+4 por impacto** y el Veneno final es **5/7/9/11/13**. A NP6, OC5 y 50
Sake llega a 120 antes del vínculo/Fuerza: una ráfaga alta, pero exige dos economías llenas y no
resuelve un campo ancho.

## 8. Estadísticas y mazo inicial

- Vida máxima: **68**.
- Energía: **3**.
- Oro inicial: **99**.
- Atributo FGO: **Earth**.
- Reliquia de motor: **Calabaza Escarlata**.
- Vínculo y nivel NP usan las implementaciones estándar de FGOCore.

### 8.1 Composición inicial

| Cant. | Carta | Estilo | Coste/tipo | Base → mejora |
|---:|---|---|---|---|
| 2 | Buster | Caster | 1, Ataque Buster | 10 → 13 daño |
| 2 | Arts | Assassin | 1, Ataque Arts | 6 daño, +30 NP → 9, +30 |
| 1 | Quick | Assassin | 1, Ataque Quick | 6 → 9 daño; genera estrellas por la regla global Quick |
| 4 | Defender | Caster | 1, Habilidad, tag Defend | 5 → 8 Bloqueo |
| 1 | Aroma del Vino Frutal | Assassin | 1, Habilidad | 4 → 6 Veneno; gana 20 Sake |

Es un QAABB de cinco ataques, cuatro defensas y una firma. Tiene `Strike` y `Defend` básicos para
los consumidores vanilla. El primer turno puede preparar 40 Sake con firma + un Caster, pero hacerlo
cuesta al menos 2 de Energía y compite con defender: el motor se enseña sin regalar el turno.

## 9. Pool común — 20

Leyenda: **A** = Assassin, **C** = Caster. La última columna incluye conexión, análogo de curva y
riesgo principal a vigilar.

| # | Carta | Estilo / coste / tipo | Base → mejora | Conexión, análogo y riesgo |
|---:|---|---|---|---|
| 1 | Aguja Envenenada | A, 1, Ataque Quick | 6 daño +3 Veneno → 8/+4 | Poisoned Stab más pequeño; Quick alimenta estrellas; vigilar co-op con Silent |
| 2 | Uña de Oni | A, 1, Ataque Buster | 9 daño; si tiene Veneno, +3 daño y +10 NP → 12/+4 | frontload condicionado; no queda muerta sin debuff |
| 3 | Aliento Embriagador | A, 1, Habilidad | 4 Veneno, +10 Sake → 6/+10 | Deadly Poison sacrifica 1 Veneno por banco; no es mejora plana |
| 4 | Mirada de Banquete | A, 1, Habilidad | 1 Débil, +20 Sake → 2 Débil | defensa preventiva; Artifact puede absorberla |
| 5 | Niebla del Pabellón | A, 1, Habilidad, enemigo | 6 Bloqueo y 1 Débil → 9/1 | defensa inmediata; objetivo obligatorio mientras haya enemigos |
| 6 | Vino Derramado | A, 0, Habilidad, Exhaust | 3 Veneno a todos, +10 Sake → 4/+20 | cobertura de área única; Exhaust impide spam 0 |
| 7 | Paso sin Presencia | A, 0, Habilidad | roba 1, descarta 1; Cruce: +10 Sake → +20 | filtrado neto cero; no crea bucle positivo de robo |
| 8 | Copa de Medianoche | A, 1, Ataque Arts | 7 daño, +20 NP; si el objetivo tiene debuff, +10 Sake → 10 daño | puente NP/Sake; exige estado solo para el rider |
| 9 | Regalo Peligroso | A, 1, Habilidad | 3 Veneno; Cruce: 5 Bloqueo → 5/7 | híbrida barata; baseline sigue siendo razonable |
| 10 | Invitación Roja | A, 1, Habilidad | 2 Vulnerable, +10 Sake → 3/+10 | prepara Caster/equipo; poco valor contra Artifact |
| 11 | Garrote del Gohō | C, 1, Ataque Buster | 10 daño; gasta 20: +5 → 13/+7 | Strike premium condicionado; puede vaciar banco temprano |
| 12 | Patada Correctiva | C, 1, Ataque Quick | 3×2; gasta 20: +1 por impacto → 4×2/+1 | estrellas + multiimpacto; vigilar bendiciones por impacto |
| 13 | Lección Mágica | C, 1, Ataque Arts | 7 daño, +20 NP; gasta 10: roba 1 → 10 daño | roba pagando banco; sin gasto sigue en curva |
| 14 | Guardia de Haku | C, 1, Habilidad | 7 Bloqueo; gasta 10: +4 → 10/+5 | defensa estable; pago pequeño intencional |
| 15 | Amuleto Blanco | C, 1, Habilidad | 5 Bloqueo, +20 Sake → 8/+20 | fuente Caster pura a cambio de tempo |
| 16 | Barrido del Gohō | C, 1, Ataque Buster, área | 6 a todos; gasta 20: +3 → 8/+4 | área inmediata; techo acotado por un solo pago |
| 17 | Agarre de Vísceras | C, 1, Habilidad | 1 Débil; si ya tenía debuff, +20 Sake → 2 Débil | Caster puede autoabastecerse, pero requiere preparación |
| 18 | Trago de Guardia | C, 2, Habilidad | 14 Bloqueo; gasta 20: +6 → 18/+8 | ancla defensiva; costo 2 evita trivializar turnos |
| 19 | Receta de Monte Ōe | C, 1, Habilidad | elige: +30 Sake, o gasta 20 para aplicar 8 Veneno → +40/11 | válvula pura y puente; segunda opción exige pago completo |
| 20 | Brindis Compartido | C, 1, Habilidad, todos los jugadores | 4 Bloqueo; Cruce: +4 → 6/+5 | apoyo con piso en solitario; escala por destinatario, no por daño |

## 10. Pool poco común — 28

| # | Carta | Estilo / coste / tipo | Base → mejora | Conexión, análogo y riesgo |
|---:|---|---|---|---|
| 1 | Aroma del Vino Frutal A+ | A, 2, Habilidad, área | 4 Veneno y 1 Débil a todos, +20 Sake → 6/1/+20 | versión colectiva canónica; costo 2 contiene el swing |
| 2 | Veneno de Siete Colores | A, 2, Habilidad, aleatorio | 3 veces: 4 Veneno a enemigo aleatorio, +20 Sake → 4 veces | análogo Bouncing Flask; concentración ST aleatoria muy fuerte |
| 3 | Ocultación de Presencia C | A, 1, Poder, máx. 1 | primera Quick normal/turno: +10 estrellas y +10 Sake → +20 estrellas | motor Quick acotado; excluye NP y una sola vez por turno |
| 4 | Magia de la Especie Oni A | A, 2, Habilidad, Exhaust | todos +1 Fuerza; prepara +1 OC para tu próximo NP; +30 Sake → tú +2 Fuerza | apoyo canónico; pico único, no Power acumulable |
| 5 | Oni sin Cabeza | A, 2, Ataque Buster, Exhaust | 18 daño; si estás a mitad de Vida o menos, gana 1 Alzarse → 24 | fail-safe condicionado; no cura ni da energía |
| 6 | Invitación a la Perdición | A, 1, Habilidad | -2 Fuerza este turno al enemigo, +20 Sake → -3 | mitigación inmediata que no persiste indefinidamente |
| 7 | Mesa Envenenada | A, 1, Poder, máx. 1 | fin de turno: si ganaste ≥30 Sake, 2 Veneno a todos → 3 | Noxious Fumes condicionado; cuenta ganancia real |
| 8 | Colección de Antigüedades | A, 1, Habilidad | roba 2; pon 1 carta de la mano arriba del mazo; +10 Sake → roba 3 | selección neta moderada; sin energía positiva |
| 9 | Coleccionista de Huesos | A, 1, Ataque Quick | 4×3; +1 por impacto si el objetivo tiene debuff → 5×3/+2 | multiimpacto/estrellas; rider depende de estado visible |
| 10 | Dulzura Falsa | A, 1, Habilidad | si tiene Artifact, quita 1 y +20 Sake; si no, aplica 6 Veneno → 8 | respuesta temprana a resistencia; nunca hace ambos efectos |
| 11 | Bruma de Monte Ōe | A, 2, Habilidad | 12 Bloqueo, 1 Débil a todos, +20 Sake → 16 Bloqueo | estabiliza campo; Artifact puede reducir la mitad ofensiva |
| 12 | Susurro Demoníaco | A, 1, Habilidad | 2 Débil; Cruce: roba 1 → 3 Débil | control/mano; robo condicionado y con costo |
| 13 | Última Gota | A, 0, Habilidad, Exhaust | próximo Veneno aplicado este turno +5; +10 Sake → +8/+20 | amplificador de una vez; no recurre ni roba |
| 14 | Hija del Dios Dragón | A, 2, Poder, máx. 1 | primer impacto sin bloquear de cada Ataque Assassin aplica 1 Veneno, máx. 3/turno → 2 | Envenom acotado por Estilo y cap; vigilar multi-hit |
| 15 | Gohō no Oni: Agarre Mortal A+ | C, 1, Habilidad | 2 Débil; gasta 20: además 2 Vulnerable → 3/2 | control Caster canónico; pago prepara su propio remate |
| 16 | Garrote de Matanza B | C, 2, Ataque Buster | 7×3; gasta 30: +2 por impacto → 9×3/+2 | ráfaga media; 30 Sake evita repetición gratuita |
| 17 | Refuerzo Arts/Buster | C, 1, Poder, máx. 1 | primera Arts/turno +10 NP; primera Buster/turno +3 daño total → +20/+4 | dos ramas, dos caps; no multiplica por impacto |
| 18 | Magia Oni (Protección) A | C, 2, Habilidad, Exhaust | +2 Fuerza y 1 Artifact; gasta 20: aliados +1 Fuerza → +3 propia | fuerza/supervivencia; apoyo exige banco |
| 19 | Golpe Certero | C, 0, Habilidad, Exhaust | próximo Ataque ignora Bloqueo, +10 Sake → además roba 1 | setup de NP/ráfaga; Exhaust cierra loops de coste 0 |
| 20 | Guardia del Familiar Blanco | C, 1, Habilidad, Retain | 10 Bloqueo; gasta 20: +10 → 13/+12 | defensa premium retenible; banco alto y no roba |
| 21 | Golpe Matademonios | C, 2, Ataque Arts | 16, +20 NP; +7 si tiene debuff o es Elite/Jefe → 21/+9 | plan B ante cleanse; objetivo grande conserva rider y ruta NP |
| 22 | Pasos de Chica Mágica | C, 1, Ataque Quick | 3×3; gasta 20: +1 por impacto → 4×3/+1 | Quick Caster; presión de bendiciones/críticos vigilada |
| 23 | Reprimenda del Oni | C, 1, Habilidad, enemigo | elimina 1 mejora ofensiva; gana 8 Bloqueo → 2 mejoras/11 | respuesta de jefe vía Cleanse compartido |
| 24 | Advertencia de Haku | C, 1, Habilidad, cualquier jugador | 9 Bloqueo y quita 1 debuff; gasta 20: +5 Bloqueo → 12/+7 | apoyo/co-op; no borra recursos ni formas |
| 25 | Barrido del Caldero | C, 2, Ataque Arts, área | 9 a todos, 3 Veneno, +20 NP → 12/+4 | paquete híbrido caro; no gasta Sake |
| 26 | Lección del Gohō | C, 1, Habilidad | roba 2, descarta 1; si descartaste Assassin, +20 Sake → roba 3 | filtrado con puente; upgrade neto +2 cuesta energía |
| 27 | Romper la Protección Divina | C, 1, Poder, máx. 1 | primer Ataque Caster/turno contra objetivo con debuff: +5 daño total y +10 NP → +7/+20 | payoff acotado; bono total, no por impacto |
| 28 | Reanudación del Combate A+ | C, 1, Habilidad, Exhaust | gana 1 Alzarse; gasta 30: cura 6 → gasto 20, cura 9 | fail-safe real; una vez por copia/combate |

## 11. Pool raro — 20

| # | Carta | Estilo / coste / tipo | Base → mejora | Conexión, análogo y riesgo |
|---:|---|---|---|---|
| 1 | Shinpen Kidoku | A, 2, Poder, máx. 1 | gana 1 Accelerant → cuesta 1 | reutiliza la amplificación vanilla; duplicados prohibidos |
| 2 | Mil Púrpuras, Diez Mil Rojos | A, 2, Habilidad, área, Exhaust | 10 Veneno, 2 Débil y 2 Vulnerable a todos; +30 Sake → 14 Veneno | clímax de setup; una sola resolución por copia |
| 3 | Banquete de Monte Ōe | A, 2, Poder, máx. 1 | inicio de turno +20 Sake; fin de turno, si jugaste Assassin, 2 Veneno a todos → 3 | motor puro; requiere carta Assassin cada turno |
| 4 | Aroma Frutal EX | A, 1, Poder, máx. 1 | primera vez/turno que una carta propia aplica un debuff visible: +20 Sake y +10 NP → +20 NP | motor doble estrictamente capado a 1 |
| 5 | Cabeza Cercenada | A, 2, Ataque Arts, Exhaust | 22 daño, +20 NP; después gana 1 Alzarse → 28 | supervivencia narrativa; no puede reciclarse por sus propias cartas |
| 6 | Banquete Envenenado | A, 2, Habilidad | activa una vez el Veneno del objetivo y lo decrementa; +30 Sake → cuesta 1 | adelanta daño, no lo duplica gratis; puede fallar sin Veneno |
| 7 | Voz que Derrite la Razón | A, 2, Habilidad, área, Exhaust | 2 Débil y 1 Sello de Habilidad a todos, +30 Sake → 3 Débil | control masivo único; Artifact sigue siendo contrajuego |
| 8 | Sangre de Orochi | A, 2, Poder, máx. 1 | primera vez/turno que un enemigo pierde HP por Veneno: +10 NP y +10 estrellas → +20/+10 | puente Poison→NP/crítico, un trigger global por turno |
| 9 | Danza de la Cabeza | A, 2, Ataque Quick | 3×7, +20 estrellas impresas → 4×7/+30; además regla Quick global | payoff multiimpacto; alto riesgo con bonos por impacto |
| 10 | Último Servicio | A, 0, Habilidad, Exhaust | pierde todo el Sake; aplica Veneno igual al 40% gastado → 50% | sink Assassin de 0–50 Veneno; banco completo y Exhaust pagan el pico |
| 11 | Paliza de Velocidad Extrema | C, 2, Ataque Buster | 4×6; gasta hasta 50: +1/impacto por 10 → 5×6 | gran sink Caster; preview obligatorio para evitar sorpresas |
| 12 | Haku, Familiar Blanco | C, 2, Poder, máx. 1 | inicio de turno: 6 Bloqueo; si puede, gasta 10 para +4 Bloqueo y +10 NP → 8/+5 | defensa sostenida con drenaje visible; toggle de auto-gasto no necesario |
| 13 | Gohō no Oni | C, 2, Poder, máx. 1 | primera vez/turno que un enemigo gana mejora ofensiva: la quita y +10 Sake → +20 Sake y 5 Bloqueo | respuesta a jefes; solo buffs registrados como ofensivos |
| 14 | Nueve Cabezas del Dragón | C, 3, Ataque Buster, área | 5×3 a todos; gasta 50: +2/impacto → 6×3/+2 | área de techo alto; costo 3 y pago fijo completo |
| 15 | Destilar el Veneno | C, 1, Habilidad, enemigo, Exhaust | quita hasta 10 Veneno; por cada punto, +3 Sake y +3 NP → hasta 15 | conversión central; sacrifica daño futuro y no funciona en objetivo limpio |
| 16 | Chica Mágica a Toda Potencia | C, 2, Poder, máx. 1 | primer Ataque Caster normal/turno gasta 20; si paga, +5 por impacto, máx. +20 total → +6/máx. +24 | escalado con pago/cap; no se dispara sin banco ni sobre NP |
| 17 | Garrote del Gohō Liberado | C, 2, Ataque Buster | 25; +10 contra objetivo con debuff; si mata, +50 Sake → 32/+12 | ejecución/frontload; matar agrega banco, no energía/robo |
| 18 | Salvación del Oni | C, 2, Habilidad, todos los jugadores | 12 Bloqueo y quita 1 debuff; gasta 30: tú +1 Buffer → 16 Bloqueo | rescate co-op; Buffer solo para Shuten y exige pago |
| 19 | Un Saint Graph, Dos Vestidos | C, 2, Poder, máx. 1 | primera vez/turno que se activa Cruce: roba 1 y +10 NP → +20 NP | motor híbrido capado; requiere dos cartas y no da energía |
| 20 | El Banquete No Termina | C, 3, Poder, máx. 1 | fin de turno: si hubo Cruce, gasta 20; si paga, +1 Energía el próximo turno → cuesta 2 | energía positiva con costo, secuencia y cap de una vez/turno |

## 12. Matriz funcional del pool

| Necesidad | Piso común | Escalado/premium |
|---|---|---|
| Frontload | Uña de Oni, Garrote del Gohō | Golpe Matademonios, Garrote Liberado, NP Caster |
| Defensa inmediata | Niebla, Guardia de Haku, Amuleto, Trago de Guardia | Bruma, Guardia del Familiar, Salvación del Oni |
| Robo/filtrado | Paso sin Presencia, Lección Mágica | Antigüedades, Lección del Gohō, Dos Vestidos |
| Energía | ninguna en comunes | El Banquete No Termina y reliquias Boss; nunca repetible sin pago |
| Escalado | Veneno y Sake | Accelerant, Sangre de Orochi, Haku, ambos poderes de Estilo |
| Área | Vino Derramado, Barrido del Gohō | Bruma, Caldero, Nueve Cabezas, NP Assassin |
| Jefe/cleanse | daño Caster base, Receta | Dulzura Falsa, Reprimenda, Matademonios, Gohō no Oni, ambos NP |
| Fail-safe | Débil común y Trago de Guardia | Oni sin Cabeza, Reanudación, Cabeza Cercenada, Salvación |
| Cooperativo | Vulnerable/Débil/Veneno compartidos | buffs de equipo, limpieza aliada y Bloqueo grupal |

## 13. Reliquias — 12

| Rareza | Reliquia | Efecto exacto | Papel/riesgo |
|---|---|---|---|
| Inicial | Calabaza Escarlata | combate: +20 Sake; primera Assassin y primera Caster normales/turno: +10 Sake cada una | motor universal, máx. 2 triggers |
| Boss (intercambio) | Calabaza Inagotable | reemplaza la inicial; combate +30 Sake; primeros Estilos/turno dan +20 cada uno | mejora el mismo motor sin energía gratis |
| Bond | Juramento del Banquete | Bond 4: +20 Sake inicial; 7: además +20 NP y +20 estrellas; 10: 1 Artifact y +10 Sake | meta estándar; multiplicador global solo el de FGOCore |
| Oculta NP | Recuerdo de la Cabeza Cercenada | almacena NP Lv 1–5/6 y dupes; no entra al pool | progresión estándar |
| Grial | Cáliz de Kuzuryū | +15 Vida máxima, Bond hasta 12 y NP Lv 6 mediante `ILimitBreaker` | Santo Grial del evento global |
| Común | Kanzashi de Monte Ōe | primera vez por combate que una carta propia aplica Veneno: +20 Sake y +10 NP | acelera apertura, una vez |
| Común | Campanilla de Haku | comienzo de combate: 6 Bloqueo; primera Caster del combate gana +10 Sake adicional | corrige fragilidad sin escalar |
| Poco común | Copa Envenenada | primera vez por turno que Veneno daña a un enemigo: +10 NP | sinergia nativa/co-op, cap global 1 |
| Poco común | Cúbito del Dragón Rojo | al gastar ≥30 Sake en un solo pago: +30 estrellas, máx. 1/turno | puente hacia críticos; no trocea pagos |
| Rara | Cabeza de la Oni | comienzo de combate: 1 Alzarse; la primera vez que se activa, +50 Sake | un solo seguro de muerte; no cura ni concede una segunda vida |
| Rara | Fragmento de Kuzuryū | primera vez/turno que se activa Cruce: próximo Ataque normal +2 por impacto, máx. +12 total | premio híbrido limitado; excluye NP |
| Tienda | Tesoro de Antigüedades | al inicio de cada combate elige +30 NP, +30 Sake o +30 estrellas | flexibilidad paga precio de tienda y una elección |

La reliquia Boss no añade una cuarta economía ni energía; solo amplía el puente ya aprendido. Si se
reemplaza la inicial, Sake y Estilos continúan existiendo porque pertenecen al personaje, no a la
reliquia.

## 14. Arquitectura y fronteras

### 14.1 Reutilización de FGOCore

- **NP:** `NpCharge`, consumo total, OC, `OverchargeBlessingPower`, nivel NP y Grial.
- **Críticos:** banco global, `CritReadyPower` y recompensa Quick. Ninguna copia local.
- **Cleanse:** `RemoveOffensiveBuffs` para Reprimenda/Gohō no Oni, preservando recursos y formas.
- **Alzarse:** `GutsPower`; las tres cartas/reliquia de supervivencia no crean variantes.
- **Vínculo/dupes:** `BondRelic`, `INpLevelStore`, `NpLevels.Scale` e `ILimitBreaker`.
- **Command Cards:** metadatos Quick/Arts/Buster, no detección por nombre.
- **Sello de Habilidad:** la implementación compartible vive en FGOCore. El modelo ya publicado de
  Tiamat conserva su ID por compatibilidad de saves y delega al mismo resolver/interfaz; Shuten usa
  el modelo core nuevo. No se renombran powers existentes.
- **Certero/Sure Hit:** power y resolución compartidos en FGOCore para que futuros Servants puedan
  declarar ataques que ignoran Bloqueo sin copiar hooks.

El Sake **no** pertenece a FGOCore: es identidad de Shuten. Tampoco se agrega un «poder Assassin» o
«poder Caster» global. Los Estilos son metadatos locales hasta que un segundo personaje demuestre
que la abstracción es compartible.

### 14.2 Módulos locales

| Módulo | Responsabilidad |
|---|---|
| `ShutenStyle` | enum Assassin/Caster y metadatos de carta |
| `SakePower` / `Sake` | cap, ganancia real, pago atómico, eventos y preview |
| `StyleHistoryPower` | primeras cartas por Estilo, Cruce y reset de turno |
| `DualNpManifestation` | unicidad viva, overflow de mano, gate de 100 y eliminación del hermano |
| poderes de cartas | contadores por turno y caps declarados en las tablas |

### 14.3 Invariantes de implementación

- Un pago de Sake se valida y descuenta una sola vez antes de emitir `SakeSpent`.
- Llegar al cap no informa ganancia ficticia a Mesa Envenenada.
- La reliquia inicial resuelve después de la carta y comparte un contador de dos bits, no dos hooks
  independientes sin límite.
- Las cartas NP no activan la reliquia inicial ni Cruce; sí pueden activar efectos que mencionen NP.
- Al jugar un NP se borra su hermano de mano/mazo/descarte antes de permitir otra manifestación; el
  NP agotado no cuenta como copia viva.
- El Sello de Habilidad compartido solo cancela intenciones no-Ataque y conserva el comportamiento
  actual de Tiamat; los ataques nunca se convierten en habilidades por heurísticas de nombre.
- Los bonos «por impacto» tienen un máximo por carta y se calculan en preview con el Sake actual.
- Los buffs que dicen «Ataque normal» excluyen las cartas Event/NP aunque tengan tipo Ataque.
- Todo targeting aliado conserva un piso útil en solitario y usa comandos sincronizados.
- Ningún efecto rastrea nombres localizados, nombres de tipos por string ni el aspecto elegido.

## 15. Simulación y presupuesto de poder

### 15.1 Apertura del mazo inicial

Secuencia pedagógica de 3 Energías:

1. **Aroma del Vino Frutal**: 4 Veneno y +20 Sake; la reliquia Assassin deja el banco en 50
   contando los 20 iniciales.
2. **Defender**: 5 Bloqueo; al ser Caster, la reliquia deja el banco en 60.
3. queda 1 Energía para otro Defender o un comando.

La alternativa Buster + Defender + otro comando tiene frontload/defensa normales y termina con
40–50 Sake, pero renuncia al Veneno inicial. La decisión aparece desde la primera mano sin exigir
una rara ni un tutorial textual largo.

### 15.2 Economías puras

- **Assassin puro:** la reliquia garantiza 10/turno. Aliento suma otros 10, Mirada 20 y varias
  fuentes grandes cuestan Energía. Puede acumular 100 para Último Servicio o guardarlo para el NP
  Caster como salida de emergencia, pero su NP propio no lo consume.
- **Caster puro:** la reliquia garantiza 10/turno. Amuleto y la opción de Receta producen 20–40;
  Agarre produce 20 sobre un objetivo preparado. Los pagos comunes de 10/20 son sostenibles, los
  raros de 30/50 no lo son sin sacrificar cartas/tempo.
- **Híbrido:** obtiene 20/turno por reliquia y Cruce mejora ciertas cartas, pero jugar un Estilo malo
  solo por cobrar 10 Sake suele costar más que el premio. No hay obligación de alternar cada carta.

### 15.3 Techo ofensivo

- **Paliza de Velocidad Extrema** con 50 Sake: 54 sin mejorar y 60 mejorada antes de Fuerza. Un
  Crítico y el vínculo global llevan la mejora a 168; una segunda carta normal completa un turno de
  180–210.
- **Nueve Cabezas del Dragón** mejorada con 50 Sake: 8×3 = 24 por enemigo antes de Crítico; llega a
  unos 67 por enemigo con Crítico y vínculo, pero consume las 3 Energías y todo el pago.
- **NP Caster máximo:** 120 antes de Fuerza/vínculo; con ×1,4 queda en 168 y seis puntos de Fuerza
  efectivos pueden acercarlo a 218. Requiere NP6, OC5 y 50 Sake, por lo que es el techo deliberado,
  no la media.
- **Último Servicio** a 100 crea 40–50 Veneno. Con un Accelerant, los dos próximos ticks pueden ser
  40+39 o 50+49: muy fuerte, pero consume todo el banco, una rara Exhaust y un Power raro previo.
  En cooperativo con más Accelerant esta es la interacción que más playtest necesita.

El objetivo de saturación sigue siendo aproximadamente **180–220 por turno** en la configuración
máxima del ecosistema. La primera perilla de ajuste será el bono por impacto/gasto, no el daño base
de los NP.

### 15.4 Defensa y fallos

- Las comunes ofrecen Bloqueo inmediato de 5–20 según pago, más Débil accesible.
- Caster sin Sake conserva 7 Bloqueo por 1 y 14 por 2; no queda injugable, solo pierde sobretasa.
- Assassin contra cleanse conserva Sake y puede pasar a Uña/Buster o al NP Caster.
- Alzarse aparece en dos cartas Exhaust y una reliquia rara, nunca como regeneración libre.
- No hay curación común. Reanudación cura 6/9 solo después de pagar y agotarse.

### 15.5 Perillas de playtest, en orden

1. Si Caster domina sin preparación: pagos comunes 20→30 o bonos por impacto -1.
2. Si Assassin tarda demasiado en Acto 1: Aguja 3→4 Veneno o Uña +3→+4; no tocar el NP.
3. Si Accelerant + Último Servicio excede el techo: 40/50%→30/40% o tope 40 Veneno aplicado.
4. Si la doble manifestación ahoga demasiado la mano: un contenedor de elección único; mantener la
   decisión, no manifestar automáticamente según último Estilo.
5. Si el Sello de Habilidad trivializa bosses: reducir su duración antes de alterar la semántica
   compartida con Tiamat; conservar el strip de Artifact del NP.
6. Si el híbrido es obligatorio: reducir riders de Cruce antes de aumentar motores puros.
7. Si el híbrido no compensa: Reliquia inicial híbrida 20→30 total por turno solo tras playtest.

## 16. Auditoría con la rúbrica

| Eje | Nota | Evidencia |
|---|---:|---|
| Identidad | 3/3 | banquete venenoso, oni guardiana y dos NP se traducen a decisiones distintas |
| Conectividad | 3/3 | 68/68 recompensas tienen Estilo; las 20 comunes conectan además con Sake, NP, Veneno, debuffs o Cruce |
| Decisiones | 3/3 | conservar/gastar, orden del Cruce, objetivo del Veneno y elección de NP |
| Potencia | 3/3 | techo 180–220 con costes estructurales; piso de Acto 1 funcional |
| Consistencia | 3/3 | ambas rutas tienen fuentes, sinks, defensa, robo y acceso NP desde comunes |
| Jefes | 3/3 | daño directo, strip, Sello de Habilidad, rider Elite/Jefe y recurso inmune a cleanse |
| Cooperativo | 3/3 | Veneno/debuffs compartidos, apoyo con piso solo y Sake estrictamente personal |
| Claridad | 3/3 | un banco 0–100, dos tags y una condición derivada; preview/glows explícitos |
| Producción | 2/3 | assets, recortes y auditoría automática completos; falta la prueba visual dentro del juego |

**Resultado de staging: 26/27.** El punto faltante no es de diseño ni de producción; exige
validación dentro del juego.

### 16.1 Banderas rojas revisadas

- No hay combo repetible de robo + Energía. El único motor de Energía cuesta 2–3, requiere Cruce,
  paga 20 Sake y dispara una vez por turno.
- No hay generación recursiva de cartas ni recuperación del exhaust.
- Todas las cartas de coste 0 que mejoran economía se agotan o filtran neto cero.
- La reliquia inicial tiene límite explícito de 2, no roba, no da Energía y no multiplica daño.
- No hay forma permanentemente superior ni cambio de forma mecánico.
- Ninguna build requiere una rara: comunes cubren fuentes y consumidores para ambos Estilos.
- No es un personaje solo-debuff: nueve comunes infligen daño/bloquean sin exigir estado enemigo.
- Los bonos por impacto están capados por carta o pagan 20–50 Sake.

## 17. UI, feedback y accesibilidad

- `SakePower` aparece junto a NP/estrellas con icono de calabaza y texto `actual/100`.
- Cartas Assassin llevan insignia de daga morada; Caster, garrote blanco. Además de color se usan
  letras `A`/`C` y hover tip para daltonismo.
- Al activarse Cruce, una línea une brevemente ambas insignias y el rider brilla en dorado.
- Un gasto muestra primero `−N Sake` y luego el efecto ampliado; el preview nunca enseña el máximo
  teórico si el banco actual es menor.
- Al llegar a 100 NP, las dos cartas entran juntas con un encabezado «Elegí cómo termina el
  banquete». La carta hermana se oscurece y desaparece al jugar una.
- Veneno usa icono, barra y VFX nativos. Sello de Habilidad usa el icono/feedback compartido y deja
  un log cuando cancela una intención.
- El modelo de combate usa la versión Assassin; la identidad Caster se comunica en cartas, NP e
  interfaz. El rig `504000` queda registrado como fuente opcional para una futura selección
  cosmética, sin `FormPower` ni efecto jugable.

## 18. Localización

Idiomas de lanzamiento: español, inglés, chino simplificado, coreano y ruso.

- JP y ZHS canónicos se conservan para NP/habilidades; ES/EN describen efectos, no transcriben voz.
- `Sake`, `Cruce`, `Assassin`, `Caster`, `Sello de Habilidad` y `Certero` tienen entradas de glosario
  únicas.
- SimpleLoc requiere escapar signos especiales según el auditor del repo.
- Números de Sake, caps y condiciones se imprimen con DynamicVars; no se duplican en strings.
- Las líneas de voz serán originales y breves, inspiradas en tono/perfil pero nunca copiadas de FGO.

## 19. Plan de assets

### 19.1 Fuentes verificadas

Comprobación corregida durante producción el 2026-07-29 contra Atlas Academy:

| Uso | ID | Estado | Decisión |
|---|---:|---|---|
| Assassin principal | `602100` | collection 112, bundle/textura y clips verificados | modelo integrado |
| Caster | `504000` | collection 225, bundle/textura y clips verificados | fuente cosmética opcional, no empaquetada |
| `602500` / `602510` | — | pertenecen a otro Assassin | descartados tras inspección visual |
| IDs supuestos `602520`, `504010`, `504020` | — | manifest 404 | descartados |

Los bundles `602100` y `504000` fueron descargados, extraídos y registrados con SHA-256; el primero
produce el set de batalla incluido y el segundo se conserva solo como fuente de referencia.

### 19.2 Entregables

- Export Animator + clips de `602100` y `504000`; empaquetar únicamente el rig Assassin validado.
- Modelo jugable Assassin, sin forma mecánica. Una futura skin Caster sería puramente cosmética.
- Char select, retratos de mapa/combate, iconos de cara, silueta y banner Workshop.
- Command cards desde `Servants/Commands/602100`; NP art de las dos versiones mediante CharaGraph.
- Iconos de Sake, Sello de Habilidad, Estilos y poderes desde skills/buffs oficiales cuando exista un match;
  si no, composición derivada registrada, no path inventado.
- 68 retratos únicos o deliberadamente reutilizados por familia, con CSV
  `assets/reference/ce/mapping_shuten.csv` y deduplicación contra el roster.
- Temas de búsqueda: Monte Ōe, banquete, calabaza/sake, veneno violeta, oni, Haku, garrote,
  Kuzuryū, cabeza cercenada, antigüedades, Kintoki y Raikō.
- Registros de procedencia `assets/reference/shuten_animation_sources.csv`,
  `assets/reference/ce/mapping_shuten.csv` y `assets/reference/icons/mapping_shutendouji.csv`.

### 19.3 Reglas visuales

- Assassin: violetas, negro, dorado y humo licoroso; Caster: blanco, rojo, rosa y golpes limpios.
- Las cartas híbridas se identifican por composición, no mezclando marcos o colores ilegibles.
- El NP Assassin prioriza campo/veneno; el Caster, movimiento diagonal y seis impactos claros.
- No se exportan cientos de frames. Se reutiliza el pipeline FBX/rig documentado en
  `docs/ANIMATIONS.md`.
- El banner Workshop debe mostrar ambos atuendos y la frase «One Servant, two ways to end the
  banquet» sin insinuar dos personajes descargables.

## 20. Orden de implementación

### Fase A — esqueleto e identidad

1. Crear `ShutenDouji/` con manifest inmutable y dependencias BaseLib + FGOCore.
2. Registrar pool, personaje, stats, comandos, tags Strike/Defend y localización mínima.
3. Implementar Sake/Estilos/Cruce con pruebas antes de cualquier carta de recompensa.

### Fase B — sistemas de combate

1. Reliquia inicial y reemplazo Boss con contador único por turno.
2. Sello de Habilidad/Certero y su interacción con Artifact, intenciones, Buffer, Block y Cleanse.
3. Doble manifestación NP, sibling cleanup, save/load y OC/NP level.
4. Glows y previews de Sake/Cruce.

### Fase C — contenido

1. Mazo inicial y las 20 comunes; smoke test de Acto 1.
2. 28 poco comunes y poderes con caps.
3. 20 raras, 12 reliquias y localización completa.
4. Simulaciones deterministas para pagos, multiimpactos, Veneno y co-op.

### Fase D — producción y validación

1. Descargar/extractar bundles verificados y producir el set de batalla Assassin.
2. Mapear CEs, generar retratos, iconos, char select y animaciones.
3. Build FGOCore primero y recompilar los 11 personajes en el mismo lote si cambia su API.
4. Probar MAIN 0.107.1 y BETA 0.109.0, SimpleLoc, PCK, guardado/carga, cooperativo y bosses con
   Artifact/cleanse.
5. Playtest de tres drafts forzados: Assassin puro, Caster puro e híbrido.

## 21. Criterios de terminado

- El personaje puede ganar Acto 1 con cada uno de los tres enfoques sin una rara obligatoria.
- Una mano sin Sake sigue siendo jugable y una limpieza no borra toda la progresión.
- El jugador entiende antes de confirmar cuánto Sake gastará y cuánto daño/Bloqueo obtendrá.
- A 100 NP se generan ambos NP sin pérdida por mano llena; con espacio puede elegir de inmediato y
  nunca conservar/jugar al hermano después de pagar uno.
- No existe secuencia determinista de Energía/robo infinito con cartas propias o incoloras comunes.
- Todos los triggers por turno se reinician correctamente en solo/co-op y tras save/load.
- Las 68 recompensas, 5 básicas distintas y 2 NP tienen arte/localización/preview válidos.
- Assets oficiales, hashes y recortes quedan registrados; ninguna URL supuesta entra al mod.
- Build/publish y logs quedan verdes en MAIN/BETA antes de cualquier subida a Workshop.
