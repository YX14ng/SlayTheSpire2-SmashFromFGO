# Guardrails del proyecto

Usar esta referencia como indice rapido. Los documentos del repositorio siguen siendo canonicos.

## Orden de lectura

1. `AGENTS.md`
2. `docs/STATUS.md`
3. `docs/DECISIONS.md`
4. `docs/FINDINGS.md`
5. `docs/CODEX-REVIEW.md` para bugs o auditorias
6. `docs/WORKFLOW-FGO.md` para implementacion y assets
7. `docs/DESIGN-<PERSONAJE>.md` para comportamiento esperado

## Invariantes

- Un mod por personaje y un mod compartido `FGOCore`.
- Los IDs persistentes no se renombran.
- `decompiled/` manda sobre recuerdos, comentarios y ejemplos externos.
- Los hooks `ModifyDamage*` son de preview y deben permanecer puros.
- Confirmar delta frente a absoluto para cada hook de modificacion.
- RNG compartido en simulacion; `PlayerRng.Rewards` para decisiones locales.
- Carga pesada threaded; caches de formas acotados por personaje.
- `CardRarity.Event` para cartas manifestadas/no drafteables.
- `PowerVar<T>` siempre con nombre explicito.
- VFX inexistente puede congelar la resolucion de una carta.
- `.tscn`/`.tres`: UTF-8 sin BOM.
- No mezclar una instalacion local con una suscripcion del mismo ID.

## Validacion

| Cambio | Evidencia minima |
|---|---|
| C# de personaje | Build Release del proyecto afectado |
| C# de FGOCore interno | Build FGOCore y proyectos afectados |
| API publica de FGOCore | Build de FGOCore y los doce personajes en el mismo lote |
| Imagen, escena o JSON | Publish Release y PCK inspeccionado |
| Compatibilidad | Matriz MAIN/BETA |
| Localizacion | `tools/audit_simpleloc.ps1` y `tools/audit_localization_parity.ps1` |
| Assets/VFX | `tools/audit_asset_coverage.ps1` y `tools/audit_vfx_paths.ps1` |
| Runtime/visual | Reproduccion en juego y `godot.log` posterior |

No hay suite automatizada del mod. No presentar un build verde como prueba de comportamiento visual,
de red o de combate.

## Politica de publicacion

- `dist/<Id>/` es staging, no una instalacion del juego.
- Los mods FGO tienen destino publico, pero esa visibilidad no autoriza un upload.
- No ejecutar Steam, SteamCMD ni `tools/workshop_upload.ps1` sin pedido explicito del usuario.
- Si cambia la API publica de FGOCore, preparar todos los artefactos compatibles juntos.

## Herramientas propias

- `tools/build_compat_matrix.ps1`: compilacion MAIN/BETA.
- `tools/audit_simpleloc.ps1`: reglas reales de SimpleLoc.
- `tools/make_card_art.ps1`: recorte reproducible desde mappings oficiales.
- `tools/install-mod.ps1`: solo playtest local y evitando IDs duplicados.
- `%APPDATA%/SlayTheSpire2/logs/godot.log`: primera evidencia de fallos runtime.
