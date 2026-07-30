# Auditoria de arte FGO

Fecha: 2026-07-22
Estado: corregido, empaquetado y staged como v0.1.7; publicacion pendiente de autorizacion

## Alcance

Se revisaron los nueve personajes, sus pantallas de seleccion, modelos de combate, fondos,
629 retratos de carta y los iconos visibles de poderes y reliquias. La revision combino inventario
de archivos, dimensiones, rutas de recursos, hash SHA-256, mapas de procedencia y hojas de contacto.

El criterio de identidad exige que cada carta conserve la ilustracion oficial concreta elegida para
su tema. Ningun retrato puede usar CharaGraph, sprites ni fallbacks de modelo 2D como fuente. Las
Command Cards oficiales se conservan porque son diseños completos de carta, no modelos de combate.

## Resultado final

- Las nueve pantallas de seleccion y los nueve modelos de combate corresponden al personaje correcto.
- Los 629 retratos registrados tienen version normal y grande; no hay entradas sin imagen.
- Las 614 cartas con mapeo tematico muestran ahora su CE, item o escena oficial concreta como arte
  completo. Se retiro la segunda figura de combate que tapaba el sujeto y hacia que todas las cartas
  de un personaje repitieran la misma composicion.
- Los 26 retratos que antes usaban CharaGraph directo fueron reemplazados por ilustraciones oficiales
  tematicas. Se conservaron 15 Command Cards oficiales. El mapa completo de procedencia esta en
  `docs/ART-CARD-IDENTITY.csv`.
- Respaldos genericos sin mapeo explicito: 0. Las variantes nuevas y `Unleashed` reutilizan la
  fuente oficial de su carta o habilidad base cuando corresponde.
- Hay 101 grupos exactos entre modulos porque algunos mapeos reutilizan deliberadamente la misma CE
  oficial. Se registran como informacion: no son una capa equivocada ni una fuente ausente.
- Se reemplazaron los iconos elegidos por hash con 133 asignaciones explicitas de rostros, skills,
  Command Cards y CE personales oficiales. La procedencia esta en `docs/ART-ICON-SOURCES.csv`.
- Los fallbacks de carta, poder y reliquia ahora son especificos para cada personaje.
- `LordCamelotChargePower`, `KingsArrogancePower`, `TreasurePower` y `BabIlu` ya tienen
  recursos dedicados.
- `palingenesis` usa el Santo Grial oficial y ya no repite el arte de `black_grail`.
- Se elimino el recurso antiguo `form_oberon.png`, que no pertenecia al flujo visual actual.
- `Around Caliburn: Unleashed` usa una ilustracion oficial completa en lugar del CharaGraph de
  Castoria. Las cartas Arts/Buster/Quick de Mordred y Gilgamesh conservan el visor oficial de
  Command Cards de FGO.

## Modelos y formas

| Personaje | Recursos oficiales | Resultado |
|---|---|---|
| Mash | 800100, 800150, 800200 | Shielder, Ortinax y Paladin animados |
| Morgan | 505320, 704020, 704030 | Aesc, Reina y Reina del Invierno animadas |
| Artoria Caster | 504520, 704710, 704720 | Caster, Berserker de verano y Avalon animadas |
| Mordred | 100900 | Set animado oficial compartido por las tres formas; poderes e iconos las distinguen |
| Gilgamesh | 200200 | Modelo animado correcto |
| Okita | 102700 | Set animado oficial compartido por la forma final para evitar una referencia inexistente |
| Oberon | 2800100, 2800110, 2800120 | Rey y Principe del Invierno animados; Vortigern usa figura oficial completa estatica |
| Siegfried | 100800 | Modelo animado correcto |
| Tiamat | 9935400, 9935410 | Femme Fatale y Beast II correctas |

Mordred y Okita reutilizan sus propios frames oficiales de manera intencional. Esto evita duplicar
cientos de texturas y reduce el riesgo de pantalla negra al combinar varios personajes FGO. No hay
ningun `FramesPath` concreto en `null` ni referencias a archivos de forma inexistentes.

## Validaciones finales

- Pares normal/grande faltantes: 0.
- Dimensiones incorrectas en retratos registrados: 0.
- Fallbacks `card.png` desincronizados: 0.
- Diferencias entre las 614 cartas mapeadas y sus referencias oficiales limpias: 0.
- Fuentes CharaGraph, sprites o fallbacks de modelo 2D en cartas: 0.
- Grupos exactos compartidos entre personajes: 101, todos permitidos por reutilizacion del mapeo.
- Fuentes oficiales registradas que no existen: 0.
- Composiciones mapeadas sin nombre oficial en el inventario: 0 de 614.
- Pares de iconos mapeados faltantes: 0 de 133.
- Rutas de formas rotas: 0.
- Marcadores `{Campo:diff()}` sin resolver: 0.
- Archivos JSON analizados como UTF-8: 250; errores: 0.
- Ambiguedades de localizacion simple: 0.

## Procedencia

Las imagenes oficiales proceden de los recursos de Fate/Grand Order catalogados por Atlas Academy.
`assets/reference/card_backgrounds/` conserva la referencia limpia de cada arte mapeado y no entra
en los paquetes. El auditor exige igualdad exacta entre esas referencias y las cartas publicables,
por lo que volver a pegar una figura encima rompe la validacion. Las Command Cards se regeneran
desde sus fuentes oficiales para impedir que un archivo viejo sobreviva a un cambio de mapa.
