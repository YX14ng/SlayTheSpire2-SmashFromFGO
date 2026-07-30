# Diseño integral — Nagao Kagetora → Uesugi Kenshin

> **Estado:** implementación, localización, animaciones, arte e interfaz completos; VFX/audio propios y playtest pendientes
> **ID y carpeta:** `KagetoraLancer`, manifest `v0.1.0` (ID inmutable)
> **Personaje:** Nagao Kagetora, con transformación irreversible en combate a Uesugi Kenshin
> **Pool:** 20 comunes, 28 poco comunes, 20 raras, 6 modelos iniciales y 2 NP Event

## 1. Fantasía, identidad y límites

**Fantasía:** comandar el campo como la encarnación de Bishamonten, encadenando tres preceptos
militares hasta que la primera formación culmina en el NP de Kagetora y revela a Kenshin.

**Identidad en una frase:**

> La comandante invicta que encadena Cielo, Pecho y Pies para convertir preparación, defensa y
> avance en NP y críticos; su primera formación culmina en el NP de Kagetora y la eleva a Kenshin,
> quien domina los tres preceptos en cualquier orden para perseguir una segunda victoria divina.

**Verbos:** ordenar, completar, ascender.

**Rol:** carry ofensiva Arts/crítico adaptable, mayormente de objetivo único, con defensa táctica,
algo de área y apoyo selectivo a aliados.

**Debilidad estructural:** necesita una composición equilibrada y el orden correcto. Las cartas
equivocadas siguen siendo jugables, pero no hacen avanzar la Doctrina; abusar de un solo precepto
reduce NP, Bloqueo, estrellas, robo y escalado.

No se añade un tercer medidor. El personaje usa solo NP, estrellas globales y el progreso visible de
la Doctrina.

## 2. Base canónica investigada

### 2.1 Nagao Kagetora

- Lancer, colección 252, baraja FGO BBAAQ y atributo Human.
- Hit counts: Quick 5, Arts 4, Buster 2, Extra 5 y NP Arts de 8 impactos.
- `運は天に在り` / «La fortuna está en el cielo»: Arts, concentración crítica y carga de NP.
- `鎧は胸に在り` / «La armadura está en el pecho»: evasión y ganancia de NP.
- `手柄は足に在り` / «El mérito está en los pies»: Ataque, daño crítico y generación de estrellas
  para el grupo.
- NP `毘天八相車懸りの陣`: objetivo único, remueve mejoras ofensivas y reduce la capacidad crítica
  del enemigo.

### 2.2 Uesugi Kenshin

- Ruler, colección 400, baraja FGO BBAAQ y atributo Earth.
- Hit counts: Quick 4, Arts 4, Buster 5, Extra 5 y NP Arts de 4 impactos.
- Reúne los tres preceptos en una sola habilidad, tiene `白き焔` («Llama Blanca») y
  `毘天宝塔` («Pagoda Enjoyada de Bishamonten»).
- NP `毘天八相・不知火`: objetivo único, elimina mejoras antes del daño y obtiene especialidad
  contra Power of Man.

### 2.3 Fuentes

- [Datos y perfil JP de Kagetora](https://w.atwiki.jp/f_go/pages/4126.html)
- [Materiales y líneas JP de Kagetora](https://w.atwiki.jp/f_go/pages/4150.html)
- [Datos y perfil JP de Kenshin](https://w.atwiki.jp/f_go/pages/6155.html)
- [Materiales y líneas JP de Kenshin](https://w.atwiki.jp/f_go/pages/6158.html)
- [Mooncell — 景虎](https://fgo.wiki/w/%E6%99%AF%E8%99%8E)
- [Mooncell — 大景虎](https://fgo.wiki/w/%E5%A4%A7%E6%99%AF%E8%99%8E)
- [Anuncio oficial de Kenshin, 2023](https://ma-inc.jp/game_support/wp-content/uploads/2023/12/20231206_%E3%80%90FGO_PressRelease%E3%80%91_%E3%80%8C%E3%81%90%E3%81%A0%E3%81%A0%E8%B6%85%E4%BA%94%E7%A8%9C%E9%83%AD-%E4%B8%8A%E6%9D%89%E8%AC%99%E4%BF%A1%E3%83%94%E3%83%83%E3%82%AF%E3%82%A2%E3%83%83%E3%83%97%E5%8F%AC%E5%96%9A%E3%80%8D%E9%96%8B%E5%82%AC.pdf)
- [Evento oficial Final Honnōji, 2019](https://news.fate-go.jp/2019/final-honnoji/)

Las cifras de este documento son decisiones de deckbuilder, no una traducción literal de FGO.

## 3. Bucle de combate

1. Identificar el precepto que falta.
2. Jugar Cielo para preparar NP y mano.
3. Jugar Pecho para sostener el turno.
4. Jugar Pies para alimentar el siguiente crítico.
5. Completar el ciclo, robar con la reliquia inicial y, si Encarnación está activa, preparar una
   Bendición multiimpacto.
6. Alcanzar 100 NP, manifestar el NP y decidir cuándo soltarlo.
7. El primer NP de Kagetora remata la formación y la transforma en Kenshin.
8. Kenshin conserva el progreso parcial, libera el orden y persigue ciclos/NP más agresivos.

Arquetipos soportados:

| Arquetipo | Núcleo | Pago |
|---|---|---|
| Formación/NP | Cielo, tutores, ciclos rápidos | menor daño inmediato |
| Caballería crítica | Quick, Pies, multiimpactos, Bendición | necesita banco y orden |
| Muralla de Echigo | Pecho, Artifact/Buffer, apoyo | ascenso y daño más lentos |
| Ejecución de Kenshin | segundo NP, anti-Man, limpieza | exige completar la primera carrera |

## 4. Doctrina de los Tres Preceptos

### 4.1 Reglas base

- Cada carta normal tiene exactamente un precepto o ninguno.
- Arts → **Cielo**.
- cartas de defensa → **Pecho**.
- Buster y Quick → **Pies**.
- Los NP, estados y algunos poderes de identidad son neutrales.
- El avance ocurre **después de resolver por completo el texto de la carta**.
- Una carta equivocada, repetida o fuera de orden se juega normalmente. No avanza, no reinicia y no
  borra progreso.
- Máximo **3 avances exitosos por turno**. Por tanto, como máximo un ciclo natural por turno.
- El progreso parcial persiste entre turnos.

### 4.2 Kagetora

Orden fijo: **Cielo → Pecho → Pies**.

Solo el precepto esperado avanza. Completar Pies cierra el ciclo y el siguiente vuelve a ser Cielo.

### 4.3 Kenshin

Orden libre, sin repeticiones dentro del ciclo. Los tres preceptos se marcan como un conjunto; al
obtener el tercero se completa el ciclo y el conjunto se vacía.

Al transformarse, el progreso parcial se traduce al conjunto de Kenshin. Ejemplo: si Kagetora ya
completó Cielo y Pecho, Kenshin solo necesita Pies.

### 4.4 Recompensas innatas

| Precepto que avanza | Recompensa |
|---|---|
| Cielo | +10 NP |
| Pecho | +4 Bloqueo |
| Pies | +20 estrellas globales |

La Doctrina no depende de una reliquia y nunca se pierde. La reliquia inicial añade una recompensa
de ciclo, pero no contiene el motor.

### 4.5 Evento de avance

El módulo local expone un resultado contextual después de cada carta: precepto intentado, si avanzó,
estado anterior/posterior, si completó ciclo y cantidad de avances del turno. Las cartas con texto
«Al avanzar» escuchan ese resultado; no vuelven a implementar la secuencia.

Orden interno: aplicar recompensa innata → actualizar progreso → emitir `PreceptAdvanced` → si
corresponde, reiniciar ciclo y emitir `CycleCompleted` → resolver la recompensa Quick global. Así un
Ataque que completa el ciclo nunca usa una Bendición recién creada.

## 5. Transformación y NP

### 5.1 Medidor y manifestación

- NP mínimo 0, umbral 100, máximo 300.
- Al llegar a 100 se manifiesta el NP de la forma actual con coste 0, Retain, Exhaust y rareza Event.
- Solo puede existir una copia manifestada de ese NP en todas las zonas.
- Puede retenerse y continuar cargando hasta 300.
- Si una conversión baja el medidor de 100, la carta manifestada permanece pero queda injugable
  hasta recuperar 100; nunca permite lanzar un NP con carga insuficiente.
- Al jugarse consume todo el NP almacenado. El OC se calcula con el valor consumido y preparaciones
  explícitas.
- Ningún NP del personaje puede ser crítico.
- La Bendición de Bishamonten sí puede potenciar sus impactos.

### 5.2 NP de Kagetora

**毘天八相車懸りの陣 — Biten Hassō Kuruma Gakari no Jin**
Ataque Arts, objetivo único, 8 impactos.

1. inflige daño;
2. después del daño elimina Fuerza positiva y las demás mejoras marcadas como ofensivas;
3. aplica Débil según OC;
4. tras terminar toda la resolución transforma a Kagetora en Kenshin.

| NP Lv | Daño por impacto | Total base |
|---:|---:|---:|
| 1 | 4 | 32 |
| 2 | 5 | 40 |
| 3 | 6 | 48 |
| 4 | 7 | 56 |
| 5 | 8 | 64 |
| 6 (Grial) | 9 | 72 |

Débil por OC efectivo: OC1–2 = 1; OC3–4 = 2; OC5 = 3. El OC no aumenta daño. Esto hace que usar el
NP al llegar a 100 sea una decisión legítima en vez de una trampa.

### 5.3 Ascenso a Kenshin

La transformación ocurre una sola vez por combate y después del NP, incluso si el objetivo muere.

Se conservan HP, Bloqueo, energía, mazo, mano, pilas, estados, estrellas y progreso parcial de
Doctrina. Cambian retrato/modelo, nombre de forma, reglas de orden y NP futuro. El medidor empieza la
segunda carrera en 0. No hay curación, energía ni estadísticas gratis.

Las cartas normales no se reemplazan. Algunas tienen un rider explícito de forma.

### 5.4 NP de Kenshin

**毘天八相・不知火 — Biten Hassō Shiranui**
Ataque Arts, objetivo único, 4 impactos.

1. antes del daño elimina Fuerza positiva y todas las mejoras marcadas como ofensivas;
2. inflige daño, con especialidad contra enemigos de atributo Man;
3. permanece como Kenshin.

| NP Lv | Daño por impacto | Total base |
|---:|---:|---:|
| 1 | 7 | 28 |
| 2 | 9 | 36 |
| 3 | 11 | 44 |
| 4 | 13 | 52 |
| 5 | 15 | 60 |
| 6 (Grial) | 17 | 68 |

- Anti-Man: +3 daño por impacto, aplicado antes de modificadores globales.
- OC: +1 daño por impacto por cada nivel por encima de OC1, máximo +4 en OC5.
- El OC no mejora la limpieza ni el anti-Man.

## 6. Atributos FGO compartidos

Este personaje necesita Power of Man, pero la solución pertenece a FGOCore.

### 6.1 Modelo

`FgoAttribute` es un conjunto cerrado y exclusivo: **Man, Earth, Heaven, Star, Beast**. Una criatura
puede tener cero o uno. No es un Buff, no se elimina, no ocupa la barra de poderes y no recibe
bonificadores universales.

El adaptador inicial de StS2 usa:

| Encuentro | Atributo predeterminado |
|---|---|
| Monster normal | Man |
| Elite | Earth |
| Boss | Heaven |
| Star/Beast | solo override explícito |
| Encuentro Event ambiguo | ninguno |

Este mapeo es una convención de diseño del mod, no una afirmación canónica sobre cada enemigo. Un
registro de overrides puede corregir modelos concretos. La UI puede mostrar un marcador neutral e
inamovible, pero el resolver es la fuente de verdad.

Los atributos no se mezclan con traits multi-valuados como Dragon, Divine, King o Humanoid. Los
traits tendrán otra interfaz y una criatura podrá poseer varios.

### 6.2 Seam

Las cartas preguntan al resolver `IsAttribute(target, Man)` mediante una interfaz. No inspeccionan
rareza, clase de encuentro ni nombres de modelo. El adaptador encierra defaults y overrides en un
solo lugar.

## 7. Estadísticas y mazo inicial

- Vida máxima: **72**.
- Energía: **3**.
- Oro inicial: **99**.
- Robo base: el estándar del juego.
- Reliquia inicial: **Pagoda Enjoyada de Bishamonten**.

### 7.1 Composición

| Cant. | Carta | Coste/tipo | Precepto | Base → mejora |
|---:|---|---|---|---|
| 2 | Buster | 1, Ataque Buster | Pies | 10 → 13 daño |
| 2 | Arts | 1, Ataque Arts | Cielo | 6 daño, +30 NP → 9 daño, +30 NP |
| 1 | Quick | 1, Ataque Quick | Pies | 6 daño, +20 estrellas impresas → 9 daño, +20; además +10 universal |
| 3 | Defender | 1, Habilidad | Pecho | 5 → 8 Bloqueo |
| 1 | La Fortuna Está en el Cielo | 1, Habilidad | Cielo | +20 NP, roba 1 → +30 NP, roba 1 |
| 1 | Encarnación de Bishamonten | 1→0, Poder | neutral | cada ciclo prepara máx. 1 Bendición |

Distribución: 3 Cielo, 3 Pecho, 3 Pies y 1 neutral. Una mano inicial de cinco contiene los tres
preceptos en **189/252 = 75 %** de los casos.

### 7.2 Bendición de Bishamonten

- máximo 1;
- persiste entre turnos;
- el siguiente Ataque, incluido NP, inflige +2 daño por impacto y la consume;
- si el Ataque completa el ciclo, consume una Bendición anterior antes de obtener la nueva;
- no se consume si la carta no llega a iniciar daño.

## 8. Pool común — 20

Todos los comunes conectan directamente con la Doctrina; cobertura estructural 20/20.

| # | Carta | Coste/tipo | Prec. | Base → mejora | Función y arte |
|---:|---|---|---|---|---|
| 1 | Estocada Celeste | 1, Ataque Arts | C | 7 daño, +10 NP → 10, +20 | ataque/NP; lanza al cielo |
| 2 | Lectura del Campo | 1, Habilidad | C | roba 2, descarta 1 → roba 3, descarta 1 | filtrado; mapa de batalla |
| 3 | Oración a Bishamonten | 0, Habilidad, Exhaust | C | +20 NP → +30 | aceleración acotada; oración |
| 4 | Volver las Riendas | 0, Habilidad, Exhaust | C | si tienes 50 estrellas, pierde 50 y gana 50 NP → además roba 1 | conversión; giro del caballo |
| 5 | Orden de Batalla | 1→0, Habilidad | C | roba la primera carta del mazo que podría avanzar tras esta resolución | tutor; abanico de mando |
| 6 | Consejo del General | 1, Habilidad | C | 5 Bloqueo, +10 NP → 8, +10 | híbrida; consejo sereno |
| 7 | Báculo del Comandante | 1, Ataque Arts | C | 8 daño; Kenshin 12 → 11; Kenshin 15 | rider de forma; bastón |
| 8 | La Armadura Está en el Pecho | 1, Habilidad | Pch | 7 → 10 Bloqueo | defensa eficiente; armadura |
| 9 | Beber Entre Balas | 1, Habilidad | Pch | 5 Bloqueo, aplica 1 Débil → 8 y 1 | mitigación; copa y disparos |
| 10 | Guardia de Kasugayama | 2, Habilidad | Pch | 15 → 20 Bloqueo | defensa grande; castillo |
| 11 | Coraza de Seis Placas | 1, Habilidad, Retain | Pch | 6 → 9 Bloqueo | orden flexible; placas blancas |
| 12 | Interponer la Lanza | 1, Habilidad | Pch | 7 Bloqueo; al avanzar roba 1 → 10 | conectividad; parada de lanza |
| 13 | Sal para el Rival | 1, Habilidad, objetivo aliado | Pch | 8 Bloqueo y quita 1 Débil → 11 y quita todo Débil | apoyo; saco de sal |
| 14 | Formación Cerrada | 0, Habilidad, Exhaust | Pch | 4 Bloqueo, o 8 si empezaste sin Bloqueo → 6/11 | emergencia; escudos cerrados |
| 15 | Lanza de Ocho Pétalos | 1, Ataque Buster | Pie | 9 → 12 daño | golpe base; punta floral |
| 16 | Carga de Houshoutsukige | 2, Ataque Buster | Pie | 5×3 → 6×3 daño | Bendición; caballo blanco |
| 17 | Paso de la Victoria | 0, Habilidad | Pie | +10 → +20 estrellas | banco; pisada en barro |
| 18 | Dar Vuelta a la Formación | 0, Habilidad, Exhaust | Pie | si tienes 50 NP, pierde 50 y gana 50 estrellas → además roba 1 | conversión; rueda militar |
| 19 | Barrido de Naginata | 1, Ataque Buster, área | Pie | 6 → 9 a todos | cobertura; barrido ancho |
| 20 | Ataque por Turnos | 1, Ataque Quick | Pie | 4×2 → 6×2 daño; +10 universal | multiimpacto; ataque alternado |

## 9. Pool poco común — 28

| # | Carta | Coste/tipo | Prec. | Base → mejora | Función y arte |
|---:|---|---|---|---|---|
| 1 | Estrategia de Rueda | 1, Habilidad | C | roba 2; si avanzó otro precepto este turno, +10 NP → roba 3 | mano/NP; diagrama circular |
| 2 | Cuatro Golpes del Cielo | 1, Ataque Arts | C | 3×3, +10 NP → 4×3, +10 | impactos; lluvia de puntas |
| 3 | Preparar la Caballería | 1, Habilidad | C | +20 NP; da Retain a 1 carta en mano este turno → +30 | orden; caballo preparado |
| 4 | Carga Mágica | 0, Habilidad, Exhaust | C | +30 NP → +50 | pico único; aura celeste |
| 5 | Relevo de Formación | 1→0, Habilidad, Exhaust | C | recupera del descarte 1 carta que podría avanzar después de esta resolución | tutor; cambio de filas |
| 6 | Mirada de la Comandante | 1, Habilidad | C | 2 Débil, +10 NP → 3, +10 | control; sonrisa inquietante |
| 7 | Mandato a la Vanguardia | 1, Habilidad, objetivo aliado | C | objetivo roba 2; tú +10 NP → roba 3 | apoyo; orden señalada |
| 8 | Enfoque del Cielo | 1, Habilidad, Exhaust | C | 1 CritReady, +10 NP → +20 | crítico seguro; halo azul |
| 9 | Armadura en el Pecho A | 1, Habilidad, Exhaust | Pch | 1 Intangible, +20 NP → +30 | evasión canónica; bala esquivada |
| 10 | Cortina de Disparos | 1, Habilidad | Pch | 9 Bloqueo; próxima vez este turno que un ataque enemigo no te haga daño, +20 estrellas → 12/+30 | defensa→crítico; humo |
| 11 | Defensa del Ruler | 2, Habilidad | Pch | 14 Bloqueo, 1 Artifact → 18, 1 | estabilidad; círculo de Ruler |
| 12 | Contraataque Sereno | 1, Habilidad | Pch | 7 Bloqueo; próximo atacante este turno recibe 6 → 9/9 | represalia; lanza inmóvil |
| 13 | Pecho sin Temor | 1, Poder, máx. 1 | Pch | cada avance de Pecho da +2 Bloqueo → +3 | motor; corazón blindado |
| 14 | Tesoro en el Corazón B | 1, Habilidad, Exhaust | Pch | 2 Artifact, +10 NP → 3 Artifact | defensa de estado; pagoda interior |
| 15 | Guardia Compartida | 1, Habilidad | Pch | tú 7 Bloqueo; aliados 4 → 10/6 | cooperativo; línea de escudos |
| 16 | Muro de Estandartes | 2, Habilidad | Pch | 14 Bloqueo; si otro precepto avanzó este turno, +20 estrellas → 18/+30 | puente; banderas blancas |
| 17 | Camino del Justo | 2, Poder | Pch | fin de turno: si avanzaste ≥2 preceptos, 6 Bloqueo → 8 | defensa de ciclo; sendero nevado |
| 18 | El Mérito Está en los Pies A | 2, Habilidad, Exhaust | Pie | tú +2 Fuerza, aliados +1, +30 estrellas → tú +3, aliados +1, +50 | apoyo/pico; tropa avanzando |
| 19 | Galope de Houshoutsukige | 1, Ataque Quick | Pie | 4×3, +10 estrellas impresas → 5×3, +20; más universal | multiimpacto; galope frontal |
| 20 | Ocho Armas, Una Guerrera | 2, Ataque Buster | Pie | 10×2; +10 estrellas por precepto distinto avanzado este turno, máx. 30 → 12×2 | payoff; ocho armas |
| 21 | Naginata Giratoria | 2, Ataque Buster, área | Pie | 8 a todos, +10 estrellas → 11, +20 | área; giro completo |
| 22 | Persecución Incansable | 1, Ataque Buster | Pie | 11; +5 si el objetivo no tiene mejoras → 14/+7 | limpieza sin dependencia; persecución |
| 23 | Asalto Alternado | 1, Ataque Quick | Pie | 3×3; si fue Crítica, +20 NP → 4×3/+30 | puente crítico-NP; estocadas rápidas |
| 24 | Retroceder es el Infierno | 2, Ataque Buster | Pie | 18; al matar +30 estrellas → 23/+50 | remate; retirada cortada |
| 25 | Cabalgata C | 1→0, Poder, máx. 1 | Pie | primera Quick normal de cada turno genera +10 estrellas adicionales | motor Quick; riendas tensas |
| 26 | Pisadas del Ejército | 1, Habilidad | Pie | +20 estrellas; roba la primera Pies del mazo → +30 | tutor; marcha de tropas |
| 27 | Doctrina del General | 1, Poder, máx. 1 | — | primera carta etiquetada que no avance cada turno da 3 Bloqueo → 5 | amortigua fallo, no lo borra; tablero |
| 28 | Divinidad C → A | 2→1, Poder | — | primer impacto de cada Ataque +3 daño; como Kenshin +5 | escalado acotado; luz divina |

## 10. Pool raro — 20

| # | Carta | Coste/tipo | Prec. | Base → mejora | Función y arte |
|---:|---|---|---|---|---|
| 1 | Llama Blanca A | 2→1, Poder | C | inicio de turno +10 estrellas; primer Cielo de cada turno +10 NP | motor mixto; llama blanca |
| 2 | Pagoda Enjoyada C | 1, Habilidad, Exhaust, objetivo aliado | C | próximo NP del objetivo +2 OC, máx. 1 preparación; +1 Fuerza; tú +20 NP → +2 Fuerza/+30 | apoyo NP; pagoda enjoyada |
| 3 | Ocho Formaciones de Bishamonten | 2→1, Poder, máx. 1 | C | primera carta etiquetada de cada turno que fallaría puede avanzar ignorando orden/repetición | rompe una restricción, no el límite; rueda |
| 4 | Sabiduría de 84 000 Enseñanzas | 2→1, Habilidad, Exhaust | C | roba 4, +20 NP | turno explosivo; sutras |
| 5 | Voto de Bishamonten | 1, Habilidad, Exhaust | C | +50 NP, 1 Artifact → 2 Artifact | aceleración/defensa; estatua |
| 6 | Blanca Llama, Fría y Ardiente | 2, Ataque Arts | C | 6×3, +20 NP, aplica 2 Vulnerable después → 8×3, +30 | setup; tajo en llamas |
| 7 | Dos Evasiones del Ruler | 2, Habilidad, Exhaust | Pch | 2 Buffer → 3 | defensa premium; dobles siluetas |
| 8 | El Tesoro Está en el Corazón | 2, Poder, máx. 1 | Pch | tras avanzar Pecho, el próximo debuff recibido ese turno se evita y da +10 NP → +20 | defensa condicionada; pagoda en pecho |
| 9 | Enviar Sal al Enemigo | 1, Habilidad, Exhaust, objetivo aliado | Pch | objetivo cura 6 y gana 12 Bloqueo → cura 9 y gana 16 | rescate; entrega de sal |
| 10 | Murallas de Kasugayama | 2, Habilidad, Retain | Pch | 20 → 26 Bloqueo | ancla; castillo completo |
| 11 | Juez del Campo | 2, Poder | Pch | primera vez por turno que un enemigo gana una mejora: +8 Bloqueo y +10 NP → +12/+20 | respuesta a jefes; mirada judicial |
| 12 | Sorbo en el Centro del Ejército | 1, Habilidad, Exhaust | Pch | 1 Intangible, +20 estrellas, roba 1 → +30, roba 2 | turno bisagra; copa en batalla |
| 13 | Biten: Formación de Rueda | 2, Ataque Quick | Pie | 2×8, +20 estrellas impresas → 3×8; más +10 universal | Bendición/crítico; rueda de lanzas |
| 14 | Hoja Shiranui | 2, Ataque Buster | Pie | 18; Kagetora genera +20 estrellas después; Kenshin elimina Bloqueo antes → 24; Kagetora +30 | rider de forma; hoja de fuego |
| 15 | Galope Total de Houshoutsukige | 3, Ataque Buster, área | Pie | 5×3 → 6×3 a todos | área fuerte; carga panorámica |
| 16 | Kawanakajima | 2, Ataque Buster | Pie | 20; +8 contra Elite/Boss → 26/+10 | objetivo grande; choque de ejércitos |
| 17 | Ocho Armas Desatadas | 1, Ataque Quick | Pie | 4×4 al objetivo, +20 estrellas impresas → 5×4; más universal | hits/estrellas; abanico de armas |
| 18 | La Victoria Está en los Pies | 2, Poder, máx. 1 | Pie | primer Crítico de cada turno reembolsa 20 estrellas → además +10 NP | motor acotado; estandarte vencedor |
| 19 | Fortuna, Armadura y Mérito A | 2→1, Habilidad, Exhaust | — | elige un precepto no completado; lo avanza ignorando orden; gana 1 Buffer | comodín único; tres ideogramas |
| 20 | Manifestación de Bishamonten | 2→1, Poder, máx. 1 | — | al completar ciclo +1 Fuerza, máximo +3 por combate | clímax limitado; deidad detrás |

## 11. Poderes y estados auxiliares

| Estado | Visibilidad | Límite | Regla |
|---|---|---:|---|
| Doctrina de los Tres Preceptos | poder de personaje | 1 | forma, progreso y avances del turno |
| Bendición de Bishamonten | buff | 1 | +2 por impacto al siguiente Ataque |
| Preparación de OC | buff | 1 | +2 niveles al próximo NP del objetivo |
| Ventana del Tesoro | buff temporal | 1 | evita próximo debuff del turno |
| Avances de Manifestación | contador de combate | 3 | Fuerza concedida por ciclos |
| FgoAttribute | marcador neutral opcional | 1 | solo información; el resolver manda |

No se implementa ninguna de estas reglas leyendo texto localizado. Todos los tags son metadatos de
modelo.

## 12. Reliquias

| Rareza | Reliquia | Efecto exacto | Papel |
|---|---|---|---|
| Inicial | Pagoda Enjoyada de Bishamonten | al completar un ciclo, roba 1 | hace visible el motor sin contenerlo |
| Boss (intercambio) | Gran Pagoda de Bishamonten | reemplaza la inicial; al completar, roba 2 | acelera con el mismo límite de un ciclo/turno |
| Bond | Juramento de Echigo | Bond 4: inicia con 10 estrellas; 7: además 20 NP; 10: además 1 Bendición | progreso persistente |
| Oculta NP | Registro de las Ocho Formaciones | guarda NP Lv 1–5; no aparece como recompensa normal | escalado meta estándar |
| Grail | Grial de la Comandante | +15 Vida máxima, Bond máximo 12 y permite NP Lv 6 según contrato global | progresión meta |
| Común | Copa de Sake | al inicio del primer turno roba 1 y descarta 1 | arregla orden sin dar ventaja neta |
| Común | Estandarte de Ocho Pétalos | el primer avance de cada turno da +2 Bloqueo | suaviza daño temprano |
| Poco común | Riendas de Houshoutsukige | la primera Pies de cada turno genera +10 estrellas después de resolverse | crítico |
| Poco común | Armadura de Seis Placas | la primera carta Pecho de cada turno da +4 Bloqueo adicional | defensa |
| Rara | Tachi Shiranui | el primer Crítico de cada turno da +10 NP | puente crítico-NP |
| Rara | Brasero de Llama Blanca | al transformarte, una vez por combate, gana 1 Energía y 30 estrellas | paga el turno de ascenso |
| Tienda | Saco de Sal de Echigo | al inicio del combate todos los jugadores ganan 1 Artifact; tú +10 NP por cada aliado | apoyo cooperativo con piso solo |

El starter mejorado no elimina la Doctrina si se pierde o reemplaza; solo cambia el robo por ciclo.

## 13. Arquitectura y fronteras

### 13.1 Módulos compartidos de FGOCore

- **Critical:** banco, pago, evento contextual y recompensa Quick, según
  `DESIGN-FGOCORE-CRITICAL-V2.md`.
- **NP:** gauge, manifestación única, OC, consumo y escalado de NP Lv.
- **Forms:** cambio visual/modelo conservando estado de combate.
- **FgoAttributes:** enum exclusivo, resolver, defaults y overrides.
- **CommandCards:** metadatos Arts/Buster/Quick; no inferencia por nombre.

### 13.2 Módulos locales

- **Doctrine:** orden de Kagetora, conjunto de Kenshin, progreso, límite y evento de avance.
- **Ascension:** escucha la resolución del NP de Kagetora y hace una transformación idempotente.
- **KagetoraCards/Relics:** contenido y riders; consumen interfaces semánticas.

### 13.3 Invariantes de implementación

- transformar dos veces es imposible;
- guardar/cargar no duplica NP ni poderes;
- una carta solo intenta avanzar una vez por jugada;
- una copia/Replay real es otra jugada; un impacto no lo es;
- ninguna carta conoce el tipo concreto de poder usado para estrellas o NP;
- los IDs de modelos no cambian una vez publicados;
- un cambio de API de FGOCore se construye con todos los personajes en el mismo lote.

## 14. Simulación y presupuesto de poder

### 14.1 Apertura

- 75 % de manos iniciales contienen Cielo, Pecho y Pies.
- Con 3 Energías pueden completar un ciclo si se juegan en orden.
- La Quick inicial genera 30 por sí misma y un Pies correcto añade 20: deja exactamente 50 para un
  Ataque posterior. No puede criticarse a sí misma.
- El primer crítico natural aparece normalmente en turno 2; manos adversas lo desplazan sin bloquear
  el combate.
- Con el mazo inicial, el primer NP natural tiene mediana objetivo en turno 3. Turno 2 requiere una
  aceleración externa, una mejora o una línea especialmente favorable; no se garantiza.

Una simulación Monte Carlo de 100 000 barajados del mazo inicial (semilla 252400), con juego
codicioso hacia el precepto requerido y luego hacia NP, dio:

| Hito | T1 | T2 | T3 | T4+ | Mediana |
|---|---:|---:|---:|---:|---:|
| primer ciclo | 75,7 % | 17,9 % | 5,8 % | 0,6 % | 1 |
| primer Crítico | — | 51,6 % | 39,5 % | 8,9 % | 2 |
| primer NP disponible | — | 21,2 % | 68,2 % | 10,6 % | 3 |

Supuestos: sin reliquias externas, sin cartas añadidas, tres Energías, descarte normal y sin
interrupciones enemigas. Es una prueba de cadencia, no una IA óptima ni un pronóstico de victoria.

### 14.2 Economía de un ciclo inicial

Un ciclo correcto concede +10 NP, +4 Bloqueo, +20 estrellas y roba 1 por la Pagoda. Con
`Encarnación de Bishamonten` activa también prepara una Bendición. El máximo de tres avances evita
encadenar dos ciclos aunque el robo encuentre coste 0.

La recompensa total es alta porque exige tres familias y orden, pero ninguna parte aislada supera
una carta común. Sin la reliquia, el ciclo conserva recursos y Bendición, pero pierde robo.

### 14.3 Daño esperado

- Acto 1: el daño viene de Buster/Quick de 9–18 y un crítico cada 1–2 turnos; el NP1 aporta 32 antes
  de Fuerza/Bendición.
- Mitad de partida: Bendición favorece 3–8 impactos, pero solo potencia un Ataque por ciclo.
- Final: un turno de Kenshin con NP, un multiimpacto crítico y 1–2 Ataques debe quedar en el rango
  objetivo aproximado de **180–220 daño** con cuatro Energías y poderes ya montados.
- `Manifestación de Bishamonten` se detiene en +3 Fuerza por combate.
- `La Victoria Está en los Pies` reembolsa 20, nunca los 50 completos.
- `Divinidad` mejora solo el primer impacto, evitando multiplicación descontrolada con NP8.

Línea de control reproducible, sin Vulnerable: Kenshin NP3 contra Man con +3 Fuerza, Divinidad y
Bendición = 81; `Biten: Formación de Rueda` Crítica ≈ 68; `Ocho Armas Desatadas` = 33; `Cuatro
Golpes del Cielo` = 23. Total aproximado **205** por 4 Energías más el NP de coste 0; el valor exacto
depende del redondeo de los impactos críticos. Requiere tres ciclos para la Fuerza, 50 estrellas, NP3
y cuatro cartas concretas; sirve como techo de referencia, no como mano media.

### 14.4 Riesgos vigilados

| Riesgo | Contención |
|---|---|
| infinitos de coste 0 + robo | un ciclo/turno; tutores Exhaust o coste; starter roba una vez |
| NP demasiado temprano | umbral 100, cartas iniciales limitadas por energía y orden |
| multiimpacto explosivo | Bendición máx. 1, Fuerza de ciclo máx. +3, NP no crítico |
| orden irrelevante | comodines raros/Exhaust o una vez por turno; fallo no avanza |
| defensa automática excesiva | recompensas pequeñas; Intangible/Buffer principalmente Exhaust |
| anti-Man domina jefes | defaults ponen Boss en Heaven; bonus solo en encuentros normales salvo override |
| co-op escala por cantidad de aliados | efectos grupales de cifras fijas; Saco de Sal limitado al inicio |

## 15. Cobertura y rúbrica de diseño

### 15.1 Conectividad

- Comunes conectados a la identidad: 20/20 = 100 %.
- Poco comunes: 26/28 etiquetadas; las dos neutrales recompensan fallo o forma.
- Raras: 18/20 etiquetadas; las dos neutrales son comodín/clímax de Doctrina.
- Ninguna rara es necesaria para que el mazo inicial complete ciclos, transforme o consiga críticos.

### 15.2 Decisiones reales

- gastar 50 estrellas ahora o reservarlas para un multiimpacto;
- usar Cielo fuera de orden por su texto o retenerlo para avanzar;
- lanzar el primer NP a 100 para liberar Kenshin o esperar OC por más Débil;
- gastar NP en conversión a estrellas y retrasar ascenso;
- elegir aliado para robo, defensa, OC o curación;
- conservar progreso parcial entre turnos o cerrar el ciclo con una carta menos eficiente.

### 15.3 Jefes y encuentros largos

- La limpieza ofensiva tiene utilidad contra escalado enemigo, pero no borra defensas/especiales en
  los NP.
- Anti-Man no se activa por defecto en Elite/Boss.
- Kenshin mejora flexibilidad, no vida ni energía base.
- El escalado permanente de combate tiene topes; la progresión restante depende del mazo.

### 15.4 Cooperativo

- estrellas, NP, Doctrina, forma y Bendición pertenecen a cada jugador.
- las cartas de objetivo aliado siempre admiten autoobjetivo.
- buffs grupales usan una cifra fija por aliado y no multiplican su efecto sobre la propia Kagetora.
- el evento crítico lleva jugador fuente; un crítico aliado no dispara poderes de Kagetora.
- si un aliado no tiene módulo NP, `Pagoda Enjoyada C` conserva Fuerza pero omite preparación de OC
  sin error ni objetivo inválido.

## 16. UI, feedback y accesibilidad

- Widget de Doctrina con tres iconos y flecha de orden en Kagetora.
- En Kenshin desaparece la flecha; los completados se iluminan y los restantes siguen seleccionables.
- El precepto esperado brilla en dorado sobre cartas de la mano; no recolorear todo el marco.
- Preview de carta indica `Avanza`, `Ya usado`, `Fuera de orden` o `Límite del turno`.
- Al avanzar: pulso del icono, número de recurso y sonido corto distinto por precepto.
- Al completar: campana de Pagoda; el robo y la Bendición tienen mensajes separados.
- La transformación espera a terminar NP y usa una transición única; no interrumpe animaciones de
  muerte.
- El atributo FGO usa icono con tooltip, no solo color.
- Todos los estados importantes deben ser distinguibles sin depender de rojo/verde.

## 17. Localización

Idiomas de producción: español, inglés, chino simplificado, coreano y ruso.

- Español es la fuente de diseño.
- Japonés se conserva solo en nombres de NP/terminología canónica y metadata de investigación.
- Chino simplificado usa los nombres oficiales de Mooncell como referencia terminológica, no
  traducción automática desde inglés.
- Placeholders de daño, Bloqueo, NP, estrellas y límites deben ser tokens; ninguna cifra se duplica en
  texto manual.
- Glosario obligatorio: Cielo/Heaven/天, Pecho/Chest/胸, Pies/Feet/足; Kagetora y Kenshin deben usar
  la misma traducción en cartas, tutorial y UI.
- Cada carta necesita `Title`, `Description`, `UpgradeDescription` cuando cambie estructura y
  `Flavor` original; no copiar líneas extensas de FGO.

## 18. Plan de assets

### 18.1 Fuente y procedencia

- usar assets oficiales de FGO/Atlas Academy: CharaGraph, modelos, iconos de skill/buff, Command
  Cards y materiales de evento;
- registrar URL/asset ID, hash, recorte, transformación y destino en el CSV de procedencia;
- no usar fanart;
- Kagetora: colección 252, modelo oficial `303800`;
- Kenshin: colección 400, modelo oficial `901820` (tercera ascensión);
- hashes, URLs, clips, recorte y destinos de ambas formas: `assets/reference/kagetora_animation_sources.csv`.

### 18.2 Entregables visuales

- retrato, icono de personaje, silueta, banner y dos modelos de combate;
- VFX separados para Cielo, Pecho, Pies, ciclo, Bendición y transformación;
- arte de 68 cartas según la columna «arte», más 6 iniciales, 2 NP y 12 reliquias;
- iconos para Doctrina, Bendición, preparación de OC, ventana del Tesoro y los cinco atributos;
- exportar el `.pck` siempre que cambie un asset o localización.

Dirección visual:

- Kagetora: índigo oscuro, blanco, acero y acentos rojos; silueta de lanza/caballo y geometría de
  rueda militar.
- Kenshin: blanco luminoso, azul frío y oro; la misma rueda se abre y la llama blanca sustituye los
  acentos rojos.
- Cielo usa azul vertical/ideograma 天; Pecho blanco hexagonal/胸; Pies dorado horizontal/足.
- Iniciales: Buster = estocada frontal; Arts = formación azul; Quick = carrera de Houshoutsukige;
  Defender = placas sobre el pecho; Fortuna = cielo abierto; Encarnación = Bishamonten tras la
  comandante.
- No cambiar el color del marco por precepto: el tag y el brillo dorado deben ser suficientes para
  conservar legibilidad de rareza.

Audio mínimo: tres acentos cortos distinguibles para los avances, campana de pagoda para ciclo,
impacto propio para Bendición, transición de ascenso y dos clímax de NP. El volumen de los avances
debe permitir tres en un turno sin fatiga.

### 18.3 Reglas de recorte

- medir cada SpriteFrames/clip en el motor antes de fijar offsets;
- anclar ambas formas al mismo punto de suelo para evitar salto visual;
- conservar lectura de arma/cara en 256 px y silueta reconocible a tamaño de carta;
- reservar escenas de NP para las dos cartas Event; no reutilizar el mismo clímax.

## 19. Checklist de implementación

### Fase A — FGOCore

- [x] Implementar Críticos v2 y migrar todo el roster existente.
- [x] Añadir metadatos Buster/Arts/Quick a todo el contenido anterior.
- [x] Implementar `FgoAttribute` y su registro de overrides.
- [x] Verificar API de NP, OC, manifestación y formas contra el decompile vigente.
- [ ] Añadir pruebas de banco, eventos, Quick, atributos y cooperativo.

### Fase B — esqueleto del mod

- [x] Reservar ID/carpeta `KagetoraLancer`.
- [x] Crear manifest con ese ID, proyecto, registro, personaje, pools y localización mínima.
- [x] Implementar Doctrina como módulo local y auditar sus transiciones.
- [x] Implementar transformación idempotente y persistencia de progreso.

### Fase C — contenido

- [x] Implementar 6 modelos iniciales y reliquia starter.
- [x] Implementar 20 comunes, 28 poco comunes y 20 raras.
- [x] Implementar ambos NP Event, NP Lv, OC y limpieza ofensiva.
- [x] Implementar 12 reliquias y estados auxiliares.
- [x] Completar cinco idiomas y tooltips.

### Fase D — assets y validación

- [x] Resolver IDs oficiales, descargar, registrar procedencia y procesar las dos animaciones de combate.
- [x] Validar modelos, clips, offsets y transición con auditor automático y PCK de staging.
- [x] Crear arte de 79 cartas, 25 powers, 12 reliquias y toda la interfaz desde fuentes oficiales.
- [ ] Crear y validar los VFX propios de Doctrina, Bendición, ascensión y ambos NP.
- [ ] Simular ≥100 combates por curva de mazo y revisar cadencias.
- [ ] Probar Elite/Boss, multiobjetivo, muerte durante NP, guardado/carga y cooperativo.
- [x] Compilar FGOCore primero y luego todos los personajes en el mismo lote tras el cambio de API.
- [ ] Publicar solo tras autorización explícita.

## 20. Criterios de terminado

El personaje está listo para playtest cuando:

1. todos los modelos y textos cargan sin warning;
2. las tres recompensas y el límite se comportan igual con cartas normales, copias y Retain;
3. la transformación conserva exactamente el estado declarado y nunca duplica el NP;
4. las 68 cartas pasan pruebas de base/mejora y objetivos válidos;
5. no hay infinito reproducible con el pool propio;
6. la primera crítica y el primer NP caen dentro de sus cadencias objetivo;
7. daño final sostenido queda aproximadamente en 180–220 por turno sin reliquias externas rotas;
8. solo Kenshin obtiene anti-Man y los overrides de atributo son visibles/diagnosticables;
9. los cinco idiomas tienen cobertura completa;
10. assets y fuentes quedan auditados y el paquete de staging contiene DLL, manifest y PCK.
