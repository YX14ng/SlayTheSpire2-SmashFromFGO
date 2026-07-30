# Rubrica de revision de personaje

Puntuar cada dimension de 0 a 3. Un personaje no esta listo para implementacion si alguna dimension
critica queda en 0 o si aparece una bandera roja sin mitigacion.

## Dimensiones

| Dimension | 0 | 1 | 2 | 3 |
|---|---|---|---|---|
| Identidad | Mecanicas intercambiables | Referencias de lore sueltas | Motor reconocible | Cada subsistema refuerza la fantasia |
| Conectividad | Recursos aislados | Varias cartas muertas | Mayoria conectada | Comunes >=90% y multiples conversiones |
| Decisiones | Linea siempre obvia | Pocas bifurcaciones | Costes y ventanas reales | Varias lineas fuertes segun encuentro |
| Potencia | Inviable o infinita | Picos erraticos | Fuerte con setup | Fuerte, fiable y con limites visibles |
| Consistencia | Depende de una rara | Manos muertas frecuentes | Redundancia suficiente | Varias rutas sin picks obligatorios |
| Jefes | Colapsa ante limpieza/fases | Una sola respuesta | Respuestas distribuidas | Adaptable sin carta universal |
| Cooperativo | Desync o escalado roto | No considerado | Determinista | Rol util y escalado defensivo correcto |
| Claridad | Texto/estado opaco | Requiere memorizar | Keywords y contadores claros | Estado, glows y upgrades autoexplicativos |
| Produccion | Assets/loc ausentes | Placeholders ambiguos | Trilingue y recortes viables | Arte oficial trazable y UI verificada |

## Banderas rojas

- Robo neto positivo + energia neta positiva dentro de un ciclo repetible.
- Generacion de una carta que se genera a si misma o recupera toda la cadena sin limite.
- Coste cero permanente apilable sin cap.
- Trigger universal sin limite por turno o reentrancia protegida.
- Multiplicadores globales que se combinan sin techo.
- Reliquia inicial que elimina la debilidad declarada desde el turno uno.
- Una comun que siempre supera alternativas de rareza mayor.
- Una rara que simultaneamente da frontload, defensa, energia, robo y escalado.
- Pool que depende de debuffs frente a jefes que los limpian.
- Cambio de forma estrictamente mejor y sin coste de salida.
- Carta o seleccion que puede esperar una entrada imposible y bloquear el combate.
- RNG local consumiendo el stream compartido, o viceversa.

## Prueba de “roto pero jugable”

El diseno pasa cuando:

1. Supera claramente a vanilla al ensamblar su motor.
2. Las manos malas y los primeros turnos todavia exigen decisiones defensivas.
3. Los payoffs grandes consumen preparacion o recursos observables.
4. Ningun loop produce recursos infinitos de forma determinista.
5. El techo de dano respeta la decision vigente del proyecto salvo excepcion documentada.
6. Un jefe que limpia debuffs o cambia de fase no invalida todo el kit.
7. El cooperativo no duplica economia ni rompe el orden RNG.

## Formato de evaluacion

Entregar:

- Identidad en una frase.
- Recursos con fuentes, sumideros y caps.
- Tabla por carta: rareza, coste, base, mejora, conexion, analogo y riesgo.
- Matriz de cobertura: ataque, defensa, consistencia, energia, escalado, AOE y jefes.
- Banderas rojas encontradas y mitigacion.
- Numeros que requieren playtest, separados de errores logicos verificables.
