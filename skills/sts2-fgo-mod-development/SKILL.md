---
name: sts2-fgo-mod-development
description: Desarrollar, depurar, revisar y empaquetar los mods C# de Fate/Grand Order para Slay the Spire 2 de este monorepo. Usar al cambiar cartas, poderes, reliquias, personajes, formas, animaciones, assets, localizacion, manifiestos, FGOCore, compatibilidad MAIN/BETA, rendimiento, logs o staging. No usar como guia generica de Godot fuera de este repositorio.
---

# Desarrollo de mods StS2 FGO

## Objetivo

Trabajar sobre el contrato real de MegaDot, BaseLib y el juego decompilado, preservando saves,
determinismo cooperativo y compatibilidad entre los diez artefactos. Combinar este flujo con
`systematic-debugging` para investigar fallos y con `verification-before-completion` antes de
declarar un resultado terminado.

## Preparacion obligatoria

1. Leer `AGENTS.md` y luego `docs/STATUS.md`, `docs/DECISIONS.md` y `docs/FINDINGS.md`.
2. Para bugs o revisiones, leer `docs/CODEX-REVIEW.md`. Para assets o un personaje nuevo, leer
   `docs/WORKFLOW-FGO.md` y el `docs/DESIGN-<PERSONAJE>.md` correspondiente.
3. Revisar `git status --short`. Preservar todos los cambios existentes y limitar el diff al pedido.
4. Usar `decompiled/` y `decompiled/_baselib_full/` como fuente de verdad para hooks, enums, VFX,
   escenas y semantica del runtime. No implementar desde memoria si el contrato se puede verificar.
5. Resolver contradicciones documentales a favor de `docs/DECISIONS.md`, luego `docs/STATUS.md`.

## Delimitar el cambio

- Identificar mod, proyecto, recursos y localizaciones afectados.
- Determinar si cambia la superficie publica de `FGOCore`. Si cambia, el alcance de compilacion son
  FGOCore y los nueve personajes; preferir una correccion compatible cuando sea razonable.
- No renombrar IDs de mod, modelos, cartas, poderes o reliquias con saves activos.
- No mover mecanicas entre mods sin una migracion deliberada.
- No duplicar codigo compartido que pertenece a FGOCore, pero tampoco ampliar su API por comodidad.

## Implementacion

- Seguir precedentes existentes del mismo tipo de carta, power, reliquia o forma.
- Mantener puros los hooks de preview como `ModifyDamage*`; comprobar en el decompilado si cada hook
  devuelve un delta o un valor absoluto.
- En simulacion sincronizada usar el RNG lockstep correspondiente. Para decisiones locales de un
  jugador usar `player.PlayerRng.Rewards`; no mezclar ambos flujos.
- No cargar `.tres` pesados sincronicamente durante combate. Usar carga threaded y aplicacion
  diferida siguiendo `FGOCore/FGOCoreCode/Forms/FormVisuals.cs`.
- Acotar caches estaticos por personaje y liberar o invalidar entradas fallidas. Nunca precargar
  formas de todos los mods.
- Validar cada ruta VFX contra `decompiled/`; una ruta inexistente puede dejar una carta sin resolver.
- Escribir `.tscn` y `.tres` en UTF-8 sin BOM. En scripts PowerShell con CJK, respetar la excepcion
  documentada para Windows PowerShell 5.1.

## Assets y presentacion

- Para personajes y animaciones, comprobar cabeza, pies, centro alfa, pivote, `flip_h`, bounds e
  intent markers con las formulas y gotchas de `docs/WORKFLOW-FGO.md`.
- Evitar cargas eager y texturas sin limite efectivo. Medir dimensiones importadas y memoria, no
  inferirlas por el peso comprimido del archivo.
- Para mods publicos, usar assets oficiales obtenidos de fuentes registradas por el proyecto o
  material con permiso/licencia clara. No incorporar fanart de Pixiv sin autorizacion verificable.
- Registrar procedencia y foco de recorte en los CSV de `assets/reference/` cuando corresponda.
- Comprobar que los fondos muestran cara y silueta legibles en el viewport real; no aprobarlos solo
  por inspeccion del archivo fuente.

## Localizacion

- Mantener paridad entre `eng`, `esp` y `zhs`; `esp` es espanol latino y `zhs` chino simplificado.
- El codigo y sus variables dinamicas mandan. Los powers necesitan `description` y
  `smartDescription`.
- Usar `tools/audit_simpleloc.ps1`, no el checker web generico de `i18n-localization`.
- Verificar escapes de SimpleLoc, nombres de variables y texto mejorado. Inspeccionar tambien que el
  texto quepa en cartas, reliquias y pantallas de seleccion.

## Diagnostico

1. Leer primero `%APPDATA%/SlayTheSpire2/logs/godot.log` y conservar la excepcion completa.
2. Establecer una reproduccion y separar carga del mod, registro, combate, UI, assets y red.
3. Comparar con un ejemplo funcional del repositorio y con el juego decompilado.
4. Formular una sola hipotesis verificable y hacer el cambio minimo que ataque la causa raiz.
5. Si tres correcciones no resuelven el mismo sintoma, detener parches incrementales y revisar el
   supuesto arquitectonico.

## Verificacion proporcional

- Cambio solo de codigo: compilar FGOCore primero si se usa su DLL y despues cada proyecto afectado.
- Cambio de asset, escena o localizacion: ejecutar `dotnet publish -c Release` hacia `dist/<Id>/` y
  verificar que el PCK contiene el recurso; no confiar solo en el exit code de MegaDot.
- Cambio transversal o de compatibilidad: ejecutar `tools/build_compat_matrix.ps1` para MAIN/BETA.
- Localizacion: ejecutar `tools/audit_simpleloc.ps1` y revisar paridad de claves.
- Runtime: cuando sea posible, probar el flujo y volver a leer `godot.log`. Si el usuario difiere el
  playtest, informar claramente que esa validacion queda pendiente.
- No ejecutar Steam, SteamCMD ni subir al Workshop salvo pedido explicito del usuario. Que el destino
  sea publico no autoriza una publicacion automatica.

## Cierre

Informar archivos cambiados, evidencia de compilacion/publicacion local, validaciones omitidas y
riesgo residual. No afirmar que un bug visual o de runtime esta resuelto solo porque compila.

La lista compacta de invariantes y comandos esta en
[`references/project-guardrails.md`](references/project-guardrails.md).
