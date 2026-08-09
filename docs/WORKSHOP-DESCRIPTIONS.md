# Guía editorial para las descripciones de Workshop

## Hallazgos de referencia (2026-07-28, aplicados 2026-07-29)

Se compararon fichas actuales de personajes de Slay the Spire 2 en Steam Workshop: [Marisa2](https://steamcommunity.com/sharedfiles/filedetails/?id=3747647777), [The Runesmith 2](https://steamcommunity.com/sharedfiles/filedetails/?id=3747609123), [Watcher](https://steamcommunity.com/sharedfiles/filedetails/?id=3747526116) y [The Queen](https://steamcommunity.com/sharedfiles/filedetails/?id=3747540049).

Los patrones más útiles son:

1. abrir con personaje, fantasía jugable y una mecánica diferenciadora; las primeras líneas son el resumen visible en listados;
2. separar contenido, mecánicas, dependencias, compatibilidad, estado y créditos;
3. dar cifras concretas de cartas, reliquias, formas e idiomas;
4. declarar claramente placeholders, rama compatible y estado de desarrollo;
5. enlazar dependencias y canal de errores;
6. usar Change Notes para el historial y conservar sólo la versión/aviso actual en la descripción;
7. usar BBCode de Steam (`[h1]`, `[h2]`, `[list]`, `[b]`, `[url]`), no encabezados Markdown `###`.

## Orden estándar

1. resumen en inglés de una o dos frases;
2. aviso importante: versión, playtest o incompatibilidad;
3. mecánicas principales, con tres a cinco puntos cortos;
4. contenido cuantificado;
5. requisitos y versiones del juego;
6. idiomas y multijugador;
7. estado honesto de pruebas/placeholders;
8. fuente, feedback, créditos y aviso no oficial;
9. traducciones completas al español y chino simplificado, sin repetir changelogs antiguos.

## Promesa principal por ítem

| Ítem | Primera promesa de la ficha |
|---|---|
| FGOCore | Biblioteca requerida que unifica NP, Overcharge, formas, atributos y críticos FGO. |
| Mash | Baluarte: guardar Bloqueo y convertir defensa en un cierre de Lord Camelot. |
| Morgan | Maldición y Buster/críticos con evolución visual de Aesc a Reina. |
| Artoria Caster | Soporte de críticos, NP y Bloqueo para el grupo a través de tres formas. |
| Mordred | Alternar Enmascarada/Rebelión para decidir entre seguridad y daño explosivo. |
| Gilgamesh | Gastar Oro en combate y desplegar Armas del Tesoro de Gate of Babylon. |
| Okita | Administrar Respiración y Tos para sostener ráfagas Quick y críticos. |
| Oberon | Tomar Deuda de NP/Estrellas ahora y pagarla con riesgo al final del turno. |
| Siegfried | Escamas persistentes y cazadragones que premia recibir/controlar impactos. |
| Tiamat | Dos barajas: control Larva y ventana temporal de Bestia. |
| Kagetora | Completar los Tres Preceptos y ascender irreversiblemente a Kenshin. |
| Shuten Dōji | Destilar Veneno en Sake y cruzar estilos Assassin/Caster dentro del mismo mazo. |
| Astolfo | Cumplir una bolsa visible de Caprichos Q/A/B y convertir Quick en Críticos/Evasión. |

Las 13 fichas de `tools/workshop_desc/` ya siguen este formato: inglés primero, traducciones completas de la ficha al español y chino simplificado, BBCode de Steam, cifras concretas, requisitos enlazados y estado de pruebas visible. Una edición puramente editorial no requiere recompilar binarios ni volver a exportar PCK.

Regla de mantenimiento: las 12 fichas de personaje deben declarar la versión mínima indicada por sus manifiestos —actualmente FGO Core `0.1.20+`, BaseLib `3.4.1+` y RitsuLib `0.5.10+`— y compatibilidad con MAIN `0.107.1` / BETA `0.110.1`, hasta que cambie el contrato real del repositorio. La ficha de FGOCore enlaza BaseLib y RitsuLib, pero no depende de sí misma.
