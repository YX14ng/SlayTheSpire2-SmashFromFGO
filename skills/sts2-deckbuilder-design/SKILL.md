---
name: sts2-deckbuilder-design
description: Disenar, balancear y revisar personajes FGO, pools de cartas, reliquias, Noble Phantasms, formas y recursos para los mods de Slay the Spire 2 de este monorepo. Usar para personajes nuevos, redisenos, evaluaciones de poder, sinergias, upgrades, infinitos, identidad de lore o problemas de experiencia de juego.
---

# Diseno de deckbuilder StS2 FGO

## Objetivo

Convertir el lore y las habilidades de un Servant en decisiones de deckbuilder legibles. El objetivo
del proyecto es que los personajes sean deliberadamente fuertes, pero que conserven costes,
preparacion, riesgos y encuentros capaces de castigarlos.

## Fuentes antes de disenar

1. Leer `docs/DECISIONS.md`, `docs/WORKFLOW-FGO.md` seccion 4.6 y el documento de diseno del
   personaje. Revisar tambien las cartas, powers, reliquias y localizacion ya implementados.
2. Investigar el material canonico japones como base y corroborar terminologia en chino simplificado.
   Separar hechos de lore, interpretacion de diseno y restricciones tecnicas.
3. Buscar analogos funcionales en cartas/reliquias vanilla y en personajes estables del repositorio.
   Comparar contratos y economia, no copiar numeros sin considerar el motor completo.

## Definir la identidad

Redactar antes del pool:

- Una frase de fantasia jugable: que hace repetidamente el jugador y por que se siente como el
  personaje.
- Dos o tres verbos mecanicos principales, por ejemplo acumular, convertir, bloquear o detonar.
- Uno o dos recursos centrales y sus entradas, salidas y limites.
- Una debilidad real: tiempo de preparacion, vida, consistencia, energia, cartas muertas o exposicion.
- El papel de NP, formas y reliquia inicial dentro del mismo motor.

Si una mecanica no refuerza esa identidad, eliminarla o integrarla en un hilo existente.

## Fuerte sin romper el juego

Aceptar que el personaje supere el promedio vanilla cuando arma su motor. Rechazar poder que quite
decisiones o invalide sistematicamente los encuentros.

- Respetar el techo de saturacion vigente de aproximadamente 180-220 de dano por turno.
- No colocar un multiplicador global de dano en la reliquia inicial.
- Exigir al menos una de estas fricciones para los payoffs mayores: preparacion, umbral, gasto de
  recurso, exhaust, vida, forma, condicion enemiga o coste de oportunidad real.
- Acotar generacion gratuita, recursion, robo, energia, reduccion de coste y triggers repetibles.
- Revisar explicitamente infinitos deterministas y ciclos que solo terminan por animacion o UI.
- No depender exclusivamente de debuffs: algunos jefes los limpian. Incluir rutas contra Buffer y
  enemigos con multiples fases sin hacer que cada carta resuelva todo.
- Una carta rara puede ser espectacular; no debe sustituir por si sola defensa, escalado y consistencia.

## Arquitectura del pool

- Mantener las basicas Buster/Arts/Quick y un mazo QAABB sesgado hacia la identidad.
- Lograr al menos 90% de conectividad en comunes: cada comun lee o escribe un recurso propio.
- Usar denominaciones 10/20/30/50/100 salvo que exista una razon documentada para apartarse.
- Hacer que la starter relic sea un motor predecible y limitar sus triggers a tres por turno cuando
  convierta eventos universales.
- Usar los powers para profundizar hilos existentes, no para abrir subsistemas aislados.
- Marcar con glow dorado todas las condiciones activas que el jugador debe reconocer en la mano.
- Las cartas manifestadas o no drafteables usan `CardRarity.Event`.

Cubrir en el pool: frontload, defensa inmediata, consistencia/robo, economia de energia, escalado,
solucion a multiobjetivo, respuesta a jefes y al menos una salida cuando el recurso central no llega.

## Evaluar cada carta

Para cada diseno registrar:

1. Tipo, rareza, coste, objetivo y si entra al pool o se manifiesta.
2. Efecto base y mejora; la mejora debe cambiar una decision o un breakpoint relevante.
3. Piso sin sinergia, rendimiento esperado dentro del motor y techo con setup razonable.
4. Recurso que produce/consume y cartas o powers que la conectan.
5. Analogo vanilla o precedente local y motivo de cualquier ventaja numerica.
6. Riesgos: carta muerta, snowball, loop, orden de hooks, multijugador, UI o texto ambiguo.
7. Tags de arte que representen el efecto y permitan mostrar cara/silueta en el recorte de carta.

No evaluar una carta solo por dano por energia. Valorar tempo, densidad del mazo, flexibilidad,
retencion, exhaust, generacion, fiabilidad y coste de oportunidad.

## Sistemas y formas

- Cada forma debe cambiar prioridades o conversiones; un aumento numerico sin decision no justifica
  una forma nueva.
- Los cambios de forma necesitan una via de entrada, una salida y reglas claras sobre powers,
  bloqueo y recursos que sobreviven.
- El NP debe ser un payoff del motor, no una carta ajena agregada al final. Revisar sobrecarga,
  duplicados, waivers y manifestacion al cruzar umbrales.
- Escalar defensas personales de tanques para cooperativo segun el precedente del proyecto; no
  multiplicar de la misma manera dano o economia personal.
- Toda eleccion aleatoria debe respetar el RNG determinista o local que corresponda.

## Revision del pool completo

1. Simular manos malas, mazo inicial, primer elite, jefe que limpia debuffs y combate largo.
2. Trazar cada recurso desde sus fuentes hasta al menos dos usos significativos.
3. Buscar cadenas de coste cero, robo neto positivo, energia neta positiva y generacion recursiva.
4. Revisar si una sola rara arregla todas las debilidades o si una comun es pick obligatorio.
5. Comprobar claridad de keywords, glows, contadores, localizacion `eng/esp/zhs` y upgrades.
6. Aplicar la rubrica de [`references/review-rubric.md`](references/review-rubric.md) y documentar
   riesgos que requieran playtest.

## Entrega

Presentar primero la identidad y los recursos, despues una tabla de cartas y finalmente las
interacciones de reliquias, NP y formas. Separar con claridad hechos canonicos, decisiones propuestas
y numeros pendientes de playtest. Al implementar, pasar a `sts2-fgo-mod-development`.
