# FGOCore — Diseño de Críticos v2

> **Estado:** implementado y migrado; contrato canónico de Críticos v2
> **Alcance:** `FGOCore` y los doce personajes actuales
> **Motivo inmediato:** las estrellas actuales son escasas, el umbral de 100 oculta decisiones y
> Artoria mantiene una economía paralela incompatible con el resto del proyecto.

## 1. Objetivo

Unificar las estrellas críticas en una sola economía global, visible y predecible. La nueva versión
debe conseguir cuatro cosas:

1. que Quick genere estrellas además de conservar el texto propio de cada carta;
2. que un crítico sea una decisión bancaria frecuente, no un premio automático muy tardío;
3. que los consumidores reaccionen al **crítico que realmente ocurrió**, no a la presencia fugaz de
   `CritReadyPower`;
4. que Artoria use el mismo contrato sin perder la identidad de sus formas.

La implementación debe ser un módulo profundo: una interfaz pequeña para cartas y poderes y una
implementación única que esconda pago, prioridad, multiplicador, eventos y compatibilidad.

## 2. Contrato jugable cerrado

### 2.1 Banco de estrellas

- Las estrellas son un recurso global del jugador y persisten entre turnos.
- Mínimo: 0. Máximo: **100**.
- Los valores de producción normales usan denominaciones 10/20/30/50/100.
- La interfaz muestra el total actual, el tope y si el próximo Ataque elegible será crítico.
- Las ganancias que excedan 100 se pierden; la animación debe mostrar el valor realmente ganado.

### 2.2 Pago y daño crítico

- Al jugar un **Ataque normal** elegible con al menos 50 estrellas, se pagan **50** antes de causar
  daño.
- Todos los impactos de esa carta infligen **×1,5** de daño. Hay un solo pago y un solo crítico por
  carta, aunque tenga varios impactos o varios objetivos.
- El redondeo usa la misma regla del juego para daño modificado; FGOCore no redondea cada fuente por
  separado.
- Un Ataque crítico de área paga una vez y aplica el multiplicador a todos sus impactos y objetivos.
- Una carta que no llega a causar daño después de pagar sigue habiendo consumido el crítico: la
  decisión se fijó al jugarla. Cancelaciones anteriores a la resolución no deben pagar.
- Los NP **no pueden ser críticos por defecto**. Una excepción debe declararlo explícitamente en el
  modelo de la carta; Okita conserva su excepción deliberada.

### 2.3 `CritReady`

- `CritReady` representa un crítico gratuito y tiene máximo **3** acumulaciones.
- Si un Ataque elegible comienza con `CritReady`, consume una acumulación antes de considerar el
  banco de estrellas. No paga estrellas.
- Las acumulaciones por encima de 3 se pierden.
- Un NP solo puede consumir `CritReady` si además declara que admite críticos.
- Prioridad fija: `CritReady` → estrellas → no crítico. Nunca se gastan ambos recursos en la misma
  carta.

### 2.4 Recompensa universal Quick

- Toda carta **Quick normal** obtiene **+10 estrellas** después de resolver por completo su propio
  texto y sus disparadores de carta.
- Todo **Quick NP** obtiene **+20 estrellas** después de resolver.
- La recompensa es adicional al texto impreso. Una Quick normal que imprime +20 genera 30 en total.
- Como la recompensa llega al final, una Quick no puede financiar su propio crítico.
- Si la carta abandona la resolución sin llegar al cierre normal, no genera la recompensa.
- Las cartas manifestadas/copias cuentan una vez cada vez que se juegan; los ecos internos que no
  constituyen otra jugada no vuelven a pagar ni generar.

### 2.5 Evento canónico de consumo

Todo efecto que diga «cuando esta carta sea Crítica» o «la primera vez que hagas un Crítico» debe
escuchar un evento único emitido por FGOCore. El evento representa el resultado confirmado, no una
consulta previa al poder.

Datos conceptuales mínimos:

| Campo | Propósito |
|---|---|
| Jugador y criatura fuente | Atribución correcta en cooperativo |
| Jugada y modelo de carta | Identificar una única resolución |
| Fuente | `CritReady`, `Stars` o excepción explícita |
| Estrellas pagadas | 0 o 50 |
| Multiplicador | 1,5 por contrato actual |
| Es NP | Permite auditar las excepciones |
| Momento | Emitido tras fijar/pagar el crítico y antes del primer daño |

Los poderes no deben leer `HasPower<CritReadyPower>()` para inferir un crítico. Esa lectura produce
errores cuando la acumulación ya se consumió o cuando el crítico vino de estrellas.

## 3. Orden de resolución

Para una jugada de Ataque normal:

1. validar que la carta puede jugarse y crear su contexto de jugada;
2. determinar elegibilidad crítica;
3. reservar y consumir una acumulación de `CritReady`, o 50 estrellas si corresponde;
4. fijar el multiplicador para toda la carta;
5. emitir el evento canónico de crítico consumido;
6. resolver texto, impactos y disparadores normales de la carta;
7. resolver recompensas que dependan del resultado confirmado;
8. si la carta es Quick, añadir su recompensa universal;
9. cerrar la jugada.

La Doctrina de Kagetora se resuelve en el paso 7, después del texto de la carta. Por eso una Quick
Pies no usa las estrellas que ella misma produce, y una Bendición obtenida al completar el ciclo no
potencia retroactivamente el Ataque que lo completó.

## 4. Arquitectura de FGOCore

### 4.1 Módulo profundo `Critical`

La interfaz pública debe limitarse a operaciones semánticas:

- consultar el banco visible del jugador;
- añadir o intentar consumir estrellas;
- resolver la elegibilidad de una jugada;
- suscribirse al resultado crítico confirmado;
- declarar la excepción NP de una carta.

La implementación oculta el tope, prioridad, coste, multiplicador, orden de resolución, prevención de
doble pago y contexto cooperativo. Las cartas no deben conocer el tipo concreto del poder que
almacena el banco.

Seams previstos:

| Seam | Responsabilidad |
|---|---|
| `CriticalBank` | clamp 0–100, ganancias, pagos y UI |
| `CriticalResolver` | elegibilidad, prioridad y multiplicador por jugada |
| `CriticalConsumed` | evento contextual único |
| `QuickReward` | recompensa posresolución por tipo de comando |
| `LegacyCriticalAdapter` | traduce llamadas/IDs antiguos durante la migración |

No se fija aquí la firma C# definitiva: debe adaptarse a los hooks reales de `CardPlay` y daño de la
versión compilada. El contrato jugable sí es estable.

### 4.2 Localidad de reglas

- FGOCore decide qué es un crítico, cuándo se paga y cuánto multiplica.
- Cada carta decide solamente si es Ataque, si es Quick y si un NP excepcional admite críticos.
- Cada personaje conserva localmente sus recompensas temáticas posteriores al evento.
- Ningún mod duplica la aritmética de 50 estrellas o ×1,5.

Esto reduce una red de lectores oportunistas de `CritReady` a una sola fuente de verdad.

## 5. Normalización de producción

### 5.1 Conversiones generales

| Valor anterior | Valor v2 inicial |
|---:|---:|
| 1–3 de una escala local | 10/20/30 respectivamente |
| 4–5 de una escala local | 50 |
| 4–5 residual en escala global | 10 |
| 15 irregular | 20 |
| 10/20/30/50/100 ya global | se conserva inicialmente |

Estas conversiones son punto de partida de migración, no una promesa de conservar cartas
desbalanceadas. La prueba de cadencia puede justificar ajustes posteriores.

### 5.2 Fuentes base

- Quick normal universal: +10 posresolución.
- Quick NP: +20 posresolución.
- Quick básica: además imprime +20; genera **30 total**.
- `Fragmento de 2030`: +10 al inicio del turno; mejora +20.
- Una fuente que diga «prepara un Crítico» concede una acumulación de `CritReady`, no 50 estrellas.

### 5.3 Cadencia objetivo por personaje

| Grupo | Personajes | Primer crítico normal | Cadencia estable buscada |
|---|---|---|---|
| Núcleo | Okita, Mordred, Artoria, Kagetora/Kenshin | turnos 1–2 | ~1/turno; ocasionalmente 2 al ahorrar |
| Híbrido | Mash, Gilgamesh | turnos 2–3 | 1 cada 1–2 turnos |
| Secundario | Oberon, Siegfried | turnos 2–3 | 1 cada 2–3 turnos salvo inversión |
| Residual | Morgan, Tiamat | turnos 3–4 | 1 cada 3–4 turnos con Quick básica |

Las cadencias se miden sobre encuentros normales con mazo inicial y luego con tres muestras de Acto
1. No se balancean suponiendo la reliquia perfecta.

## 6. Migración de Artoria

Artoria deja de tener una economía crítica activa propia y usa el banco global de FGOCore.

- Tope 100 y pago estándar de 50.
- Caster genera y conserva estrellas, pero normalmente no puede gastarlas.
- Berserker y Avalon pueden gastarlas con el contrato global.
- `Around Caliburn` abre temporalmente la ventana de gasto en Caster.
- Valores locales 1/2/3 pasan a 10/20/30; valores 4–5 pasan a 50.
- Las cartas con «Crit X» pasan a «si esta carta es Crítica…», usando el pago estándar.
- El ID histórico de su poder local no se renombra. Queda como adaptador de carga/migración y
  redirige al banco global para no romper partidas guardadas.
- La UI de forma puede estilizar el mismo banco, pero no mantener un segundo total.

Pruebas específicas:

1. Caster con 100 estrellas no las consume sin ventana;
2. cambiar a Berserker conserva el banco y habilita el pago;
3. volver a Caster conserva el remanente;
4. `Around Caliburn` habilita exactamente las jugadas previstas;
5. una partida guardada con el poder antiguo carga sin duplicar estrellas;
6. no aparecen dos medidores ni dos pagos para una misma carta.

## 7. Migración del resto de personajes

Cada pool publicado debe completar esta lista:

- clasificar todas las cartas de comando como Buster, Arts o Quick cuando corresponda;
- garantizar al menos una Quick jugable por personaje;
- aplicar la recompensa universal sin borrar el efecto propio de la carta;
- convertir lectores de `CritReady` al evento canónico;
- revisar fuentes directas de `CritReady` bajo el tope 3;
- declarar de forma explícita las contadas excepciones de NP crítico;
- convertir números irregulares con la tabla de normalización;
- medir la cadencia del mazo inicial y de dos arquetipos de Acto 1.

Morgan no pierde su texto Quick: también recibe las +10 universales. Artoria tampoco es una excepción
de generación; su particularidad es quién puede gastar el banco según la forma.

### 7.1 Inventario mínimo de migración

La auditoría del repositorio identifica estos centros; la búsqueda debe repetirse antes de codificar
porque hay consumidores adicionales en cartas y reliquias:

- `FGOCore/FGOCoreCode/Stars/CritStarsPower.cs` y `CritReadyPower.cs`;
- `FGOCore/FGOCoreCode/CardTypes/CommandBonusPower.cs`, `ICommandTyped.cs` y `CommandType.cs`;
- `FGOCore/FGOCoreCode/Memes/Fragment2030.cs` y `Fragment2030Power.cs`;
- `ArtoriaCaster/ArtoriaCasterCode/Powers/CriticalStarsPower.cs`, `Stars.cs` y
  `AroundCaliburnWindowPower.cs`;
- cartas y reliquias de Artoria que leen/escriben su escala local;
- listeners de Okita, Mordred, Gilgamesh, Mash, Oberon, Siegfried y Tiamat que reaccionan a
  `CritReady` o generan estrellas;
- `MordredSaber/.../ICritConsumedListener.cs`, que debe converger en el evento compartido en vez de
  convertirse en una segunda interfaz global.

La migración no se considera completa mientras una búsqueda de los tipos antiguos siga mostrando
lectores de jugabilidad fuera de adaptadores de compatibilidad.

## 8. Invariantes y pruebas automáticas

### 8.1 Banco

- nunca baja de 0 ni supera 100;
- una ganancia parcial informa solo lo efectivamente ganado;
- dos jugadores mantienen bancos independientes;
- guardar/cargar conserva exactamente el valor.

### 8.2 Pago

- 49 estrellas: no crítico, no pago;
- 50: crítico y queda 0;
- 100: un Ataque deja 50; un segundo Ataque elegible deja 0;
- `CritReady` + 50: consume una acumulación y conserva 50;
- `CritReady` ×3: tres Ataques gratuitos; una cuarta fuente no aumenta el total;
- multiimpacto y área pagan una sola vez;
- habilidad/poder nunca consume;
- NP común no consume ni critica;
- NP excepcional consume una sola vez.

### 8.3 Evento

- se emite exactamente una vez por carta crítica;
- lleva al jugador correcto en cooperativo;
- distingue `CritReady` de Stars;
- no se emite por una carta no crítica ni por un impacto interno;
- los efectos «si esta carta fue Crítica» funcionan aun cuando el recurso ya fue consumido.

### 8.4 Quick

- una Quick con 40 estrellas no se autocritica y termina con 50 más su texto impreso;
- una Quick crítica con 50 paga 50 y luego obtiene su recompensa;
- una Quick NP no critica por defecto y genera 20 después de resolver;
- copias jugadas son jugadas nuevas; daño repetido no es otra jugada;
- el tope recorta la ganancia sin desbordar.

## 9. Auditoría de balance

Registrar por combate:

- turno del primer crítico;
- estrellas generadas, gastadas y desperdiciadas por tope;
- críticos por turno y por combate;
- porcentaje de críticos usados en multiimpacto/área;
- cantidad de `CritReady` perdida por tope;
- daño adicional aportado por el sistema;
- decisiones donde el jugador retuvo una carta o evitó gastar para reservar 50.

Alertas de diseño:

- más de 1,5 críticos por turno de forma sostenida sin una inversión rara;
- más del 25 % de estrellas desperdiciadas por tope en el mazo inicial;
- primer crítico posterior al turno objetivo en más de la mitad de las partidas;
- una única carta produciendo 50 o más repetidamente sin coste, Exhaust o límite;
- bucles de robo/energía que además reembolsen las 50 estrellas completas.

## 10. Despliegue seguro

El cambio de API en FGOCore obliga a reconstruir y publicar **FGOCore y los doce DLL de personaje
en el mismo lote**. Orden:

1. introducir el nuevo módulo y adaptadores sin borrar IDs;
2. migrar Artoria y todos los lectores/generadores de cada personaje;
3. ejecutar auditorías de contrato, localización, recursos, VFX y carga de guardados;
4. construir FGOCore y los doce personajes juntos;
5. probar localmente una instalación sin IDs duplicados;
6. hacer una pasada de combate por cada personaje;
7. publicar solo con autorización explícita del usuario.

No se elimina el adaptador de compatibilidad hasta que la ventana de partidas guardadas publicada se
considere cerrada.

## 11. Decisiones fuera de alcance

- No se introduce probabilidad aleatoria de crítico.
- No se crea un triángulo pasivo de atributos FGO.
- No se permite que todos los NP critiquen.
- No se agregan multiplicadores distintos por personaje en v2.
- No se rebalancean todas las cifras de daño del proyecto antes de medir la nueva economía.
