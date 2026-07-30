# Astolfo Rider — documento de diseño

Estado: **implementado y empaquetado; pendiente de playtest dentro del juego**
Mod ID reservado: `AstolfoRider`
Base canónica: Astolfo Rider, No. 094, modelo de batalla principal `400400`, SR 4★, mazo FGO
`QQQAB`.

## 1. Promesa del personaje

Astolfo es el paladín que gana una aventura imposible porque improvisa mejor de lo que planifica.
Cada turno recibe un **Capricho** visible —Quick, Arts o Buster— y decide si lo cumple con la mano
actual, si fuerza un cambio o si abandona el plan para asegurar recursos. Las cartas Quick alimentan
las **Estrellas Críticas** globales; Arts carga NP; Buster abre ventanas de daño. El Hipogrifo
convierte el NP en una huida momentánea del mundo mediante **Evasión**.

Los tres verbos de juego son:

1. **Improvisar:** leer o manipular el Capricho del turno.
2. **Acelerar:** transformar Quick y Estrellas en críticos, energía o NP.
3. **Escapar:** conservar Evasión para el impacto que realmente importa.

Debilidad deliberada: Astolfo tiene defensa base modesta y una mano que no coincide con el Capricho
puede perder eficiencia. Estrellas y Evasión compiten entre daño y supervivencia; gastar el NP con
Evasión llena desperdicia una parte importante de su valor. El jugador siempre ve el problema antes
de decidir, pero no siempre puede obtener todas las recompensas.

## 2. Panel de propuestas y veredicto

Se compararon tres núcleos antes de cerrar el personaje:

| Propuesta | Fantasía | Veredicto adversarial |
|---|---|---|
| A. Caprichos de Razón Evaporada | Bolsa visible Q/A/B, Estrellas y NP | **Ganadora.** Es reconocible, interactúa con cada mano y conserva agencia. Se corrigió el RNG puro con una bolsa sin repetición y controles de cambio. |
| B. Vuelo como forma temporal | Entrar y salir del Hipogrifo | Rechazada: se solapa con las formas de Kagetora y Tiamat, y vuelve al NP una transformación casi siempre superior. |
| C. Cinturón de cuatro tesoros generados | Argalia, libro, cuerno e Hipogrifo como cartas manifestadas | Rechazada como motor: se acerca demasiado al arsenal de Gilgamesh y llena la mano. Los tesoros permanecen como cartas normales, raras y diferenciadas. |

El juez de conectividad exigió que todos los comunes tocaran al menos un sistema central. El juez de
jefes exigió respuestas que no dependieran de acumular perjuicios. El juez de loops limitó la
conversión de Estrellas en energía a cartas con Agotar.

## 3. Contrato de las mecánicas

### 3.1 Capricho

La reliquia inicial instala una bolsa con `Quick`, `Arts` y `Buster`. Al comienzo de cada turno del
jugador se extrae una opción al azar de las que aún quedan; no se repite ninguna hasta vaciar la
bolsa. El Capricho actual es visible antes de robar o jugar cartas.

Solo una **Command Card normal** del tipo indicado puede cumplirlo. NP, cartas Event y cartas que
solo imitan un tipo no cuentan. La primera coincidencia del turno obtiene:

- **Quick:** 20 Estrellas adicionales después de resolverse, además de las 10 globales de Quick.
- **Arts:** 20 NP adicionales después de resolverse.
- **Buster:** +6 de daño total para esa carta, repartido entre sus impactos y objetivos sin
  multiplicarse por cada golpe.

Al cumplirse, el Capricho desaparece y queda un marcador oculto `CaprichoCumplido` hasta el final del
turno. La base permite un solo cumplimiento por turno; una rara puede abrir un segundo.

Las cartas que **eligen** Capricho sustituyen la opción visible sin alterar la bolsa. Las que
**cambian** o **revelan opciones de la bolsa** sí consumen el resultado elegido. Al empezar un nuevo
combate la bolsa vuelve a contener los tres tipos.

Persistencia requerida:

- una Power oculta guarda la máscara restante (`Q=1`, `A=2`, `B=4`);
- una Power visible guarda el Capricho actual;
- los marcadores de cumplimiento y de límite por turno también son Powers, nunca campos privados;
- si una carta cambia de tipo Quick/Arts/Buster durante la partida, ese tipo es una propiedad guardable;
- todo sorteo usa el RNG determinista del combate y sobrevive a guardar/cargar.

Feedback requerido: icono y texto Q/A/B, no solo color; resplandor dorado en Command Cards que lo
cumplen; vista previa del premio; sonido corto distinto al resolverlo.

### 3.2 Estrellas Críticas

Usa Críticos v2 de FGOCore sin bifurcaciones:

- banco máximo 100;
- una Quick normal genera 10 Estrellas; un NP Quick, 20 salvo texto explícito;
- al jugar un Ataque normal elegible, 50 Estrellas se reservan y consumen para multiplicar todos sus
  impactos por ×1,5;
- hasta tres cargas de Crítico Listo;
- NP y daño indirecto no consumen crítico.

Astolfo añade generación y gastos manuales de 20/30/40/50. Los gastos de una carta ocurren antes de
que el Ataque pueda reservar 50 para crítico y muestran resplandor solo cuando se pueden pagar.

### 3.3 Evasión — nueva Power compartida de FGOCore

`EvasionPower` pertenece a FGOCore porque Hipogrifo, Instinto y habilidades de otros Servants pueden
reutilizar el mismo contrato. No se renombra ninguna Power publicada.

- máximo 3 cargas;
- impide el siguiente impacto de daño de Ataque enemigo que habría perdido HP;
- no se consume si Bloqueo/Buffer ya evitó toda pérdida;
- un multiimpacto consume una carga por golpe que realmente alcanzaría HP;
- no evita pérdida de HP propia, ambiental ni costes de cartas;
- las cargas sobrantes no se convierten en Bloqueo y no hay desborde sobre 3.

Este orden evita gastar Evasión detrás de Bloqueo, pero conserva la debilidad canónica frente a
multiimpactos.

### 3.4 Derribo — herramienta local de Argalia

`DerriboPower` es un perjuicio local y de una sola carga. Si el próximo intento visible del enemigo
es un Ataque, lo sustituye por Aturdido y se consume. Se aplica mediante el flujo normal de Powers,
por lo que Artefacto puede absorberlo. Contra Élite/Jefe no cancela el intento completo: consume la
carga y aplica 3 Débil, para evitar que una rara invalide turnos decisivos.

## 4. Personaje, mazo inicial y reliquia

- Vida máxima: **72**.
- Energía base: **3**.
- Género mecánico: `Neutral`, coherente con los filtros de FGO.
- Afinidad FGO: **Tierra**.
- Mazo de inicio: 10 cartas.

| Cantidad | Carta | Texto base |
|---:|---|---|
| 3 | Quick Command | 1 Energía. 6 daño. Genera 10 Estrellas por la regla global. |
| 1 | Arts Command | 1 Energía. 6 daño. Gana 30 NP. |
| 1 | Buster Command | 1 Energía. 10 daño. |
| 4 | Defender | 1 Energía. 5 Bloqueo. |
| 1 | Corazonada del Paladín | 1 Energía. Elige el Capricho actual. Gana 5 Bloqueo. Mejora: 8 Bloqueo. |

**Reliquia inicial — Razón Evaporada D+:** al comienzo del combate gana 30 NP, instala la bolsa de
Caprichos y extrae uno al comienzo de cada turno. Máximo un cumplimiento por turno.

**Reemplazo de jefe — Razón Completamente Evaporada:** reemplaza la inicial, comienza con 50 NP y,
la primera vez que se cumple un Capricho cada turno, gana 10 NP y 10 Estrellas.

La reliquia inicial no entrega multiplicadores globales ni energía. Es un motor visible con máximo
un disparo base por turno.

## 5. Noble Phantasm

### この世ならざる幻馬 — Hippogriff — 非世间所存之幻马

Carta `Event`, retenible y con Agotar. Se manifiesta al alcanzar 100 NP y consume todo el NP al
jugarse.

- Tipo: Quick, Ataque a todos, 2 Energías.
- 20 daño base a todos, escalado por nivel de NP con la curva común de FGOCore.
- Certero e ignora Bloqueo únicamente para este Ataque.
- Gana Evasión hasta llegar a 3 cargas.
- Genera 20 Estrellas por ser un NP Quick, más 10/15/20/25/30 por Overcharge.
- No puede ser Crítico ni cumplir Capricho.

La secuencia adicional 10/15/20/25/30 es una excepción documentada a las denominaciones de 10:
reproduce la curva canónica del NP y su tope menor compensa las tres cargas defensivas. El NP no crea una forma:
Astolfo sale de la observación del mundo durante los impactos que cubre Evasión y luego conserva su
modelo Rider.

## 6. Pool de recompensa — 68 cartas

Convención: el valor tras `→` es la mejora. Todos los importes de daño son totales antes de Fuerza,
Vulnerable y Crítico. `Cumplido` consulta el marcador de ese turno.

### Comunes — 20

| # | Carta | Tipo / coste | Base | Mejora |
|---:|---|---|---|---|
| 1 | Galope Inesperado | Quick Ataque / 1 | 7 daño. Si cumple Capricho, +10 NP. | 10 daño. |
| 2 | Pirueta de Lanza | Quick Ataque / 1 | 4×2 daño. Si consume Crítico, 5 Bloqueo. | 5×2; 7 Bloqueo. |
| 3 | Carga sin Frenos | Buster Ataque / 1 | 10 daño; +4 si el Capricho visible no es Buster. | 13 daño; +5. |
| 4 | Punta de Argalia | Arts Ataque / 1 | 7 daño, +20 NP; si el objetivo tiene Débil, +10 Estrellas. | 10 daño. |
| 5 | Espada de Caballería | Buster Ataque / 1 | 8 daño; 2 Bloqueo por cada 20 Estrellas, máx. 8. | 11 daño; máx. 10. |
| 6 | Picado del Hipogrifo | Quick Ataque a todos / 2 | 6 a todos; +3 si posee al menos 50 Estrellas. | 8 a todos. |
| 7 | Lanza del Buen Ánimo | Buster Ataque / 2 | 16 daño; si ya cumplió Capricho, +10 NP. | 21 daño. |
| 8 | Pluma Cortante | Quick Ataque / 0, Agotar | 4 daño. | 6 daño. |
| 9 | Cambio de Dirección | Habilidad / 1 | Reemplaza el Capricho con uno aleatorio restante de la bolsa. 5 Bloqueo. | 8 Bloqueo. |
| 10 | Olvidé el Plan | Habilidad / 0, Agotar | Descarta el Capricho actual. +10 NP y +10 Estrellas. | +20 Estrellas. |
| 11 | Defensa Improvisada | Habilidad / 1 | 7 Bloqueo; Q: +10 Estrellas, A: +10 NP, B: +3 Bloqueo. | 10 Bloqueo. |
| 12 | Maniobra Brusca | Habilidad / 1 | 6 Bloqueo; puede gastar 20 Estrellas para +6. | 8 Bloqueo; +7. |
| 13 | Alas como Escudo | Habilidad / 1 | 8 Bloqueo. Se Retiene mientras tenga Evasión. | 11 Bloqueo. |
| 14 | Aterrizaje Suave | Habilidad / 1 | 7 Bloqueo; si ya cumplió Capricho, +10 NP. | 10 Bloqueo. |
| 15 | Sorpresa Compartida | Habilidad a cualquier jugador / 1 | 6 Bloqueo; +3 si el Capricho sigue sin cumplir. | 8 Bloqueo; +4. |
| 16 | Estrellas en el Camino | Habilidad / 0, Agotar | Gasta 50 NP para ganar 50 Estrellas. | Cuesta 40 NP. |
| 17 | Atajo hacia el Cielo | Habilidad / 0, Agotar | Gasta 50 Estrellas para ganar 50 NP. | Cuesta 40 Estrellas. |
| 18 | ¡No Pasa Nada! | Habilidad / 1 | 6 Bloqueo; si ya cumplió Capricho, quita 1 perjuicio propio. | 9 Bloqueo. |
| 19 | Paso de Trifas | Habilidad / 0 | Roba 1 y descarta 1; si descarta una Command que coincide con el Capricho, +10 Estrellas. | También +10 NP. |
| 20 | Toque de Corneta | Habilidad a todos los enemigos / 1 | 1 Débil; si ya cumplió Capricho, 5 Bloqueo. | 8 Bloqueo. |

Conectividad común: **20/20** cartas leen o escriben Capricho, Estrellas, NP, Crítico o Evasión.

### Poco comunes — 28

| # | Carta | Tipo / coste | Base | Mejora |
|---:|---|---|---|---|
| 1 | Razón Evaporada D | Poder / 1, Máx. 1 | El primer Capricho cumplido por turno da +10 NP y +10 Estrellas. | Cuesta 0. |
| 2 | Elegir sin Pensar | Habilidad / 1, Agotar | Elige Capricho; la próxima Command coincidente cuesta 0 este turno. | Retener. |
| 3 | Capricho Cambiante | Habilidad / 0, Agotar | Revela hasta 2 opciones restantes de la bolsa, elige una y gana 10 NP. | +20 NP. |
| 4 | Lo Primero que Parezca Divertido | Habilidad / 1, Agotar | Elige una Command normal en la mano y vuelve su tipo el Capricho actual. Roba 1. | Cuesta 0. |
| 5 | Instinto del Paladín | Poder / 1, Máx. 1 | Si termina el turno sin cumplir Capricho, gana 6 Bloqueo al inicio del siguiente. | 9 Bloqueo. |
| 6 | La Mejor Ruta, Quizá | Habilidad / 1 | Roba 2; pon 1 carta de la mano sobre el mazo. Si coincide con Capricho, +10 NP. | Roba 3. |
| 7 | Sin Secretos | Habilidad / 1 | Roba 2, descarta 2; +10 Estrellas por Command descartada que no coincida, máx. 20. | Roba 3. |
| 8 | Buen Humor Contagioso | Habilidad a todos / 1 | Todos ganan 5 Bloqueo; si luego cumple Capricho este turno, todos ganan 3 más, máx. 1. | 7 y 4 Bloqueo. |
| 9 | Equitación A+ | Poder / 1, Máx. 1 | La primera Quick normal de cada turno obtiene +3 daño total y +10 Estrellas. | +5 daño. |
| 10 | Acción Independiente B | Poder / 1, Máx. 1 | El primer Crítico consumido por turno devuelve 10 Estrellas. | 20 Estrellas. |
| 11 | Triple Quick | Habilidad / 1, Agotar | Si jugó al menos 2 Quick este turno, gana 1 Crítico Listo; si no, +20 Estrellas. | Cuesta 0. |
| 12 | Danza de Plumas | Quick Ataque / 1 | 3×3 daño y +10 Estrellas explícitas. | 4×3. |
| 13 | Embestida en Zigzag | Quick Ataque / 2 | 7×2 daño; si cumple Capricho, +20 Estrellas. | 9×2. |
| 14 | Suerte A+ | Habilidad / 1 | +30 Estrellas; elige una carta de la mano para Retener. | +40 Estrellas. |
| 15 | Impulso Irrefrenable | Habilidad / 1, Agotar | Gasta 30 Estrellas: +1 Energía y roba 1. | Cuesta 20 Estrellas. |
| 16 | Despegue Inestable | Habilidad / 1, Agotar | Gasta 50 Estrellas para 1 Evasión; si no puede, 8 Bloqueo. | Cuesta 40; 10 Bloqueo. |
| 17 | Salto Dimensional | Habilidad / 1 | 9 Bloqueo; si tiene Evasión, +20 NP. | 12 Bloqueo. |
| 18 | Existencia Imposible | Poder / 1, Máx. 1 | La primera vez por turno que Evasión evita daño, +20 Estrellas. | +30 Estrellas. |
| 19 | Alas sobre el Grupo | Habilidad a todos / 2 | Todos ganan 9 Bloqueo; +3 si Astolfo tiene Evasión. | 12 Bloqueo. |
| 20 | Picado desde Otro Mundo | Quick Ataque a todos / 2 | 9 a todos; puede gastar 1 Evasión para +8 a todos. | 11; +10. |
| 21 | Reaparición | Buster Ataque / 1 | 9 daño; +6 si tiene Evasión, sin gastarla. | 12; +8. |
| 22 | Toca y Derriba D | Buster Ataque / 2 | 11 daño y 2 Débil; si ya tenía Débil, +10 Estrellas. | 15 daño y 3 Débil. |
| 23 | Luna Break Manual | Habilidad / 1, Agotar | Quita 1 perjuicio propio y gana 1 Artefacto. | 2 Artefacto. |
| 24 | Páginas al Viento | Habilidad a enemigo / 1 | Quita 1 beneficio ofensivo; 7 Bloqueo; si quitó uno, +20 NP. | 10 Bloqueo. |
| 25 | La Black Luna | Buster Ataque a todos / 2 | 9 a todos y 1 Débil. | 12 a todos. |
| 26 | Rescate del Homúnculo | Habilidad a cualquier jugador / 1 | 8 Bloqueo; si el objetivo está a mitad de HP o menos, quita 1 perjuicio. | 11 Bloqueo. |
| 27 | Aventura Compartida | Habilidad a cualquier jugador / 1 | El objetivo roba 1 y gana 5 Bloqueo; Astolfo gana 10 Estrellas. | 8 Bloqueo. |
| 28 | Una Aventura Distinta Cada Vez | Poder / 2, Máx. 1 | Si el Capricho del turno difiere del anterior, +10 NP y +10 Estrellas, máx. 1/turno. | Cuesta 1. |

### Raras — 20

| # | Carta | Tipo / coste | Base | Mejora |
|---:|---|---|---|---|
| 1 | Razón Evaporada D+ | Poder / 2, Máx. 1 | Tras cumplir el primer Capricho del turno, extrae otro de la bolsa; permite un segundo cumplimiento. | Cuesta 1. |
| 2 | Improvisación Perfecta | Habilidad / 1, Agotar | Elige Capricho; al cumplirlo este turno, roba 1. | Cuesta 0. |
| 3 | Tres Caprichos, Una Aventura | Poder / 2, Máx. 1 | Al cumplir Q, A y B distintos durante el combate, gana 1 Crítico Listo y +1 Energía el próximo turno; reinicia el registro. | Cuesta 1. |
| 4 | El Camino Óptimo | Habilidad / 1, Agotar | Elige Capricho y busca una Command normal coincidente del mazo o descarte. | Retener. |
| 5 | Noche sin Luna | Habilidad / 1, Agotar | Rellena la bolsa, elige Capricho y gana 1 Artefacto. | También +10 NP. |
| 6 | Golpe Afortunado | Habilidad / 0, Agotar | Gasta 30 Estrellas para ganar 1 Crítico Listo. | Cuesta 20 Estrellas. |
| 7 | Estrellas que No Necesito | Habilidad / 1, Agotar | Gasta hasta 50 Estrellas; el próximo Ataque normal gana +2 daño total por cada 10, máx. +10. | +3 por 10, máx. +15. |
| 8 | Galope a Toda Velocidad | Poder / 2, Máx. 1 | Quick normales ganan +1 por impacto, con máximo +6 total por carta. | +2, máx. +10. |
| 9 | Más Rápido que una Flecha | Quick Ataque / 2 | 4×5 daño y +20 Estrellas explícitas. | 5×5. |
| 10 | Salto entre Dimensiones | Habilidad / 1, Agotar | Gasta 50 Estrellas para 2 Evasión; si no puede, 12 Bloqueo. | Cuesta 40; 15 Bloqueo. |
| 11 | Reverso del Mundo | Poder / 2, Máx. 1 | Tras la primera carga de Evasión consumida cada turno, 8 Bloqueo. | 12 Bloqueo. |
| 12 | No Estaba Ahí | Habilidad / 2, Agotar | Gana 2 Evasión; pierde 1 Energía el próximo turno. | No pierde Energía. |
| 13 | Hipogrifo, ¡Arriba! | Habilidad / 2, Agotar | +50 NP; si no tenía Evasión, 10 Bloqueo. | Cuesta 1. |
| 14 | Choque desde lo Imposible | Buster Ataque a todos / 3 | 18 a todos; puede gastar toda Evasión para +6 por carga, máx. +18. | 22; +7 por carga, máx. +21. |
| 15 | Trap of Argalia — ¡Toca y Derriba! | Buster Ataque / 2, Agotar | 12 daño; quita 1 Artefacto y aplica 1 Derribo. | 16 daño y 2 Débil. |
| 16 | Casseur de Logistille — Declaración de Ruptura | Habilidad / 2, Agotar | Un jugador quita todos sus perjuicios y gana 2 Artefacto; elige un enemigo y quita sus beneficios ofensivos. | Cuesta 1. |
| 17 | La Black Luna — Llamada al Pánico | Buster Ataque a todos / 3 | 15 a todos y 2 Débil. | 20 a todos y 3 Débil. |
| 18 | Akhilleus Kosmos Prestado | Habilidad a todos / 3, Agotar | Todos ganan 16 Bloqueo; Astolfo gana 1 Buffer. | 20 Bloqueo. |
| 19 | Buenas Acciones sin Pensarlo | Poder / 2, Máx. 1 | El primer Capricho cumplido por turno da 5 Bloqueo al jugador con menor proporción de HP; en solitario, a Astolfo. | 7 Bloqueo. |
| 20 | La Aventura Sigue | Poder / 3, Máx. 1 | El primer Crítico consumido por turno da +20 NP; la primera Evasión consumida por turno da +20 Estrellas. | Cuesta 2. |

## 7. Reliquias — 12

| Rareza | Reliquia | Efecto |
|---|---|---|
| Inicial | Razón Evaporada D+ | Bolsa de Caprichos, 30 NP iniciales y un cumplimiento por turno. |
| Jefe | Razón Completamente Evaporada | Reemplaza la inicial; 50 NP y +10 NP/+10 Estrellas en el primer cumplimiento del turno. |
| Vínculo | Juramento del Paladín Alegre | Vínculo 4: +20 NP al inicio. Vínculo 7: también +20 Estrellas. Vínculo 10: todos comienzan con 1 Evasión y Astolfo +10 NP. |
| NP oculta | Libro del Nombre Olvidado | Guarda nivel de NP, pity y progreso con el contrato común de FGOCore. |
| Evento/Grial | Cáliz de los Doce Paladines | +15 HP máximo, requiere Vínculo 12 y eleva el tope a NP6 mediante el evento compartido. |
| Común | Pluma del Hipogrifo | La primera Quick del combate da +20 Estrellas. |
| Común | Cinta de Trifas | Empieza con 6 Bloqueo; el primer Capricho cumplido del combate da +10 NP. |
| Poco común | Punta Dorada de Argalia | La primera vez por turno que aplica Débil, +10 Estrellas. |
| Poco común | Manual Lleno de Garabatos | La primera vez por combate que quita un perjuicio propio o beneficio enemigo, gana 1 Artefacto y +20 NP. |
| Rara | Escama de una Existencia Imposible | La primera Evasión consumida por turno devuelve 10 Estrellas. |
| Rara | Escudo Prestado de Aquiles | La primera vez por combate que Astolfo queda a mitad de HP o menos, todos ganan 8 Bloqueo y Astolfo 1 Evasión. |
| Tienda | Bolsa de Aventuras | Al inicio del combate elige: +30 NP, +30 Estrellas o elegir el primer Capricho. |

La reliquia de Vínculo usa una Evasión grupal solo al máximo y una vez por combate; no modifica
daño. Si el playtest cooperativo muestra demasiada seguridad inicial, la primera perilla es limitar
la carga a Astolfo y dar 8 Bloqueo al resto.

## 8. Arquetipos viables

### Improvisación / Capricho

Controla la bolsa, encuentra Commands y habilita un segundo cumplimiento. Produce recursos mixtos y
decisiones de orden. Funciona sin raras mediante Cambio de Dirección, Defensa Improvisada, Razón
Evaporada D y los filtros poco comunes.

### Quick / Crítico

Genera Estrellas con Quick, elige entre el crítico automático de 50 o gastos manuales, y usa
multiimpactos con límites por carta. Funciona sin Caprichos perfectos y sin una rara obligatoria.

### Hipogrifo / Supervivencia

Arts y conversiones aceleran NP; Evasión cubre golpes decisivos, alimenta Salto Dimensional y abre
un contraataque. No puede sostenerse de forma infinita: las fuentes directas de Evasión tienen
Agotar o dependen del NP.

### Tesoros / Control

Argalia, Casseur y La Black Luna responden a Artefacto, beneficios y grupos. Es un paquete auxiliar,
no otro recurso ni un generador de cartas. Contra jefes conserva valor aun cuando el perjuicio se
degrade.

## 9. Cobertura de problemas

| Necesidad | Común | Poco común | Rara / NP |
|---|---|---|---|
| Daño frontal | Carga sin Frenos, Lanza del Buen Ánimo | Embestida en Zigzag, Toca y Derriba | Más Rápido que una Flecha, Trap of Argalia |
| Bloqueo frontal | Defensa Improvisada, Maniobra Brusca | Salto Dimensional, Alas sobre el Grupo | No Estaba Ahí, Akhilleus Kosmos |
| Área | Picado del Hipogrifo, Toque de Corneta | La Black Luna, Picado desde Otro Mundo | Choque, Llamada al Pánico, Hippogriff |
| Robo / filtro | Paso de Trifas | La Mejor Ruta, Sin Secretos, Aventura Compartida | El Camino Óptimo |
| Energía | Conversiones con Agotar | Impulso Irrefrenable | Tres Caprichos, Una Aventura |
| Escalado | Capricho y Crítico base | Equitación, Acción Independiente, Razón D | Razón D+, Galope, La Aventura Sigue |
| Artefacto / beneficios | ¡No Pasa Nada! | Luna Break Manual, Páginas al Viento | Argalia, Casseur |
| Jefes sin perjuicios | Crítico, NP, daño Buster | Evasión, Powers de Quick | Certero, daño por Estrellas y Caprichos |
| Cooperativo | Sorpresa Compartida | Buen Humor, Alas, Rescate, Aventura | Casseur, Escudo Prestado, Buenas Acciones |

## 10. Auditoría de seguridad y balance

### Apertura esperada

Con 5 cartas robadas del mazo inicial de 10, Astolfo suele ver 2–3 Commands y 1–2 defensas. El
Capricho visible permite ordenar el turno, pero solo Corazonada puede garantizar la coincidencia. La
reliquia entrega 30 NP, no Energía ni daño. La apertura más fuerte razonable sigue siendo un Buster
bonificado, una Command adicional y defensa; no hay multiplicador global.

### Techo de daño

- Más Rápido que una Flecha+ aporta 25 base antes de Crítico; con ×1,5 son 37,5.
- Galope a Toda Velocidad+ está limitado a +10 total por Quick, no por cada impacto sin tope.
- Choque desde lo Imposible+ llega a 43 de área si sacrifica las 3 Evasiones.
- Hippogriff ronda 20–40 de área según NP y no puede ser Crítico.
- La conversión Estrellas→Energía se agota; Tres Caprichos requiere tres tipos y entrega Energía al
  turno siguiente.

Incluso una mano rara preparada queda por debajo del techo objetivo de 180–220 por turno antes de
Fuerza externa. El riesgo principal son multiimpactos con Fuerza; los límites de Equitación se
aplican al bono propio, no neutralizan Fuerza del juego, por lo que esta interacción debe medirse en
playtest.

### Loops revisados

- Las dos conversiones comunes y todas las fuentes directas de Energía tienen Agotar o retraso.
- Cambiar/seleccionar Capricho no roba de forma repetible sin coste y la bolsa se persiste.
- Acción Independiente y Existencia Imposible disparan máximo una vez por turno.
- Evasión tiene máximo 3 y no se regenera a sí misma.
- La Aventura Sigue cruza Crítico→NP y Evasión→Estrellas, pero cada rama tiene máximo 1/turno y no
  alimenta directamente su propio disparador.

No se encontró un infinito determinista de una o dos cartas.

### Respuestas de jefe

- Artefacto absorbe Derribo; la rara de Argalia puede retirar uno, pero se Agota.
- En Élite/Jefe Derribo se degrada a 3 Débil en vez de cancelar el turno.
- Casseur se Agota y cuesta 2, por lo que quitar beneficios es una decisión, no un candado.
- Crítico, Capricho Buster y Hippogriff siguen funcionando si el jefe limpia perjuicios.
- Multiimpactos consumen varias Evasiones y evitan inmunidad prolongada.

## 11. Perillas de playtest, en orden

1. Si fallar Caprichos se siente frustrante, subir Corazonada a 7 Bloqueo antes de alterar la bolsa.
2. Si sobran Estrellas, bajar el premio Quick del Capricho de 20 a 10.
3. Si Evasión trivializa jefes, subir los gastos manuales de 40/50 antes de reducir las 3 cargas
   canónicas del NP.
4. Si el NP es demasiado defensivo, subir daño; si es demasiado frecuente, bajar NP inicial, no la
   fantasía de Evasión.
5. Si Argalia decide demasiado contra jefes, degradar Derribo a 2 Débil en lugar de 3.
6. Si el Vínculo domina cooperativo, Evasión solo para Astolfo y 8 Bloqueo para aliados.
7. Si Fuerza rompe las Quick multiimpacto, bajar impactos antes de recortar generación de Estrellas.
8. Si falta energía, mejorar Impulso; no añadir energía a la reliquia inicial.

## 12. Contrato visual y de assets

- Paleta: rosa, negro, oro y azul cielo; Capricho siempre combina color, letra y silueta.
- Modelo principal: `400400`, Collection No. 094. No se asigna ID a Memories at Trifas hasta
  verificar el bundle exacto; no se inventan `400410`/`400430`.
- Estados mínimos: idle, Quick/lanza, cast/libro, daño y muerte; el NP puede añadir vuelo/VFX sin
  cambiar de forma persistente.
- Arte de cartas: Command Cards oficiales y Craft Essences centradas en Astolfo. El catálogo local ya
  registra `9305750 Touring Portrait: Astolfo` y `9308370 Exhibition Attire: Astolfo`; deben
  verificarse y ampliarse antes de asignar las 80 imágenes.
- Iconos: bolsa de tres lunas para Capricho, estrella alada para Estrellas y silueta fuera de fase
  para Evasión. Todos deben leerse a 32 px y en escala de grises.
- Audio: no copiar voces del juego. Usar frases originales y efectos propios/licenciados.

## 13. Criterios de implementación

- `AstolfoRider` compila contra FGOCore y BaseLib en MAIN y BETA.
- Todo cambio de API pública de FGOCore obliga a recompilar el lote completo.
- Evasión vive en FGOCore; Capricho y Derribo viven en Astolfo.
- Máscara de bolsa, Capricho, cumplimiento y límites sobreviven guardar/cargar.
- 68 recompensas exactas `20/28/20`; NP y cartas manifestadas son `CardRarity.Event`.
- Mazo inicial exacto de 10; 12 reliquias contando inicial, jefe, vínculo, NP y Grial.
- Ninguna carta requiere una rara para funcionar y al menos 90% del pool toca el motor; el diseño
  actual marca 68/68 conexiones directas o de apoyo.
- Resplandores cubren coincidencia de Capricho, pagos de Estrellas/NP, Evasión y condiciones de
  aliado/objetivo.
- Pruebas obligatorias: bolsa sin repetición, save/load, Crítico antes/después de gasto, multiimpacto
  de Evasión, Buffer+Bloqueo+Evasión, Artefacto+Derribo, NP en 100/200/300 NP, cooperativo y stripping
  de Powers en jefes.

## 14. Rúbrica de cierre

| Eje | Nota / 3 | Motivo |
|---|---:|---|
| Identidad | 3 | Improvisar, acelerar y escapar producen decisiones propias de Astolfo. |
| Conectividad | 3 | 20/20 comunes y 68/68 cartas tocan Capricho, Estrellas, NP, Evasión o sus respuestas. |
| Decisiones | 3 | Cumplir/cambiar/abandonar Capricho y gastar/guardar Estrellas/Evasión. |
| Potencia | 2 | Curvas y límites están cerrados; falta telemetría/playtest real. |
| Consistencia | 3 | Bolsa sin repetición, selección y filtros; las builds puras tienen piso. |
| Jefes | 3 | Daño no dependiente de perjuicios, Certero, control degradado y limpieza. |
| Cooperativo | 3 | Siete herramientas de aliado/grupo con piso en solitario. |
| Claridad | 3 | Contratos, orden de consumo, límites, persistencia y feedback definidos. |
| Producción | 3 | Modelo 400400, 180 cuadros, UI, 80 retratos, 35 iconos de power, 12 reliquias y PCK integrados. |
| **Total** | **26/27** | Implementación cerrada; potencia queda en 2 hasta obtener telemetría de playtest real. |

No hay ningún eje crítico en 0. La producción ya está validada; la nota de potencia solo puede subir
tras playtest real.

## 15. Fuentes canónicas

- Sitio oficial FGO, trayectoria de Servants: <https://www.fate-go.jp/trajectory/servant/>
- TYPE-MOON Wiki japonesa, Astolfo y Noble Phantasms:
  <https://typemoon.wiki.cre.jp/wiki/%E3%82%A2%E3%82%B9%E3%83%88%E3%83%AB%E3%83%95%E3%82%A9>
- FGO atwiki japonesa, No. 094, deck, skills, NP y modelo interno:
  <https://w.atwiki.jp/f_go/pages/736.html>
- Mooncell/Wiki china, terminología de 阿斯托尔福:
  <https://fgo.wiki/w/%E9%98%BF%E6%96%AF%E6%89%98%E5%B0%94%E7%A6%8F>

Decisiones canónicas incorporadas: QQQAB; Razón Evaporada, Equitación, Acción Independiente y
Fuerza Monstruosa; Argalia derriba, Casseur rompe magecraft, La Black Luna causa pánico y el
Hipogrifo salta de dimensión, ignora defensas y concede tres evasiones. La alegría y nobleza del
miembro más débil de los Doce Paladines sostienen el paquete cooperativo sin convertirlo en un
personaje de soporte puro.
