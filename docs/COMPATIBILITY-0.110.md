# Compatibilidad StS2 v0.107.1 / v0.110.1

Estado verificado el 2026-07-31 contra copias aisladas de `sts2.dll`:

- MAIN: `.compat/sts2-main-0.107.1`
- BETA: `.compat/sts2-beta-0.110.1` — Steam build `24489008`, commit `db5d3552`
- BaseLib de compilación: 3.3.6; runtime comprobado: 3.3.7

La referencia BETA mínima debe incluir `sts2.dll`, `sts2.xml`, `0Harmony.dll`, `GodotSharp.dll`,
`Sentry.dll` y `Sentry.Godot.dll`. Los dos últimos son dependencias nuevas de 0.110.1; sin ellos la
sonda de enlace no puede inicializar el assembly aunque el código compile.

## Cambios de API relevantes

Los contratos introducidos en BETA 0.109.0 siguen presentes en 0.110.1:

- `AttackCommand.FromCard` recibe `CardPlay?`.
- Las sobrecargas de `CreatureCmd.Damage` asociadas a carta reciben `CardPlay?`.
- `CreatureCmd.LoseBlock` recibe `PlayerChoiceContext` y el responsable de quitar Bloqueo.
- Los tres hooks `ModifyDamage*` reciben `CardPlay?`.
- `AfterBlockBroken` recibe contexto y responsable.
- BETA expone `CardPlay.Player`, `BranchingPlayerChoiceContext`,
  `DrawWithoutBlockingOnOtherPlayers`, `CombatManager.CombatBegan` y
  `BeforeCombatRewardOffered`.

La comparación completa de 0.109.0 y 0.110.1 confirmó que `AbstractModel`, `AttackCommand` y `Hook`
son idénticos, y que `CreatureCmd` conserva las firmas usadas por FGOCore. La decompilación también
confirmó que los métodos objetivo de los hardenings de tienda, fogata, recompensas y Darv siguen
presentes.

0.110.1 cambia otras APIs públicas que los mods FGO no invocan directamente: varios métodos de
`CombatManager` reciben ahora un `CombatId?`, y `CardPileCmd.DrawWithoutBlockingOnOtherPlayers`
agrega el `CardModel source`. Si en el futuro se usan esas APIs, deberán pasar por una capa de
compatibilidad MAIN/BETA.

## Diseño universal

El paquete público se compila contra MAIN. En MAIN usa los overrides de cinco argumentos. Cuando
ese mismo DLL corre en BETA, `LegacyDamageHookCompatibility` detecta por firma exacta los hooks de
seis argumentos y reenvía el `CardPlay` a `IFgoDamageHooks`. Una compilación específica de BETA usa
los overrides nativos y desactiva ese bridge para no ejecutar dos veces la modificación.

Las firmas públicas anteriores de FGOCore se conservan para DLL ya compilados. Los overloads nuevos
aceptan `PlayerChoiceContext`; los doce personajes del repositorio los usan para mantener efectos
anidados dentro de la resolución sincronizada original. Esto cubre NP, estrellas, Aliento, Deuda,
manifestación de NP y consumo/reembolso de cartas NP.

La misma regla se aplica a Maldición, Laḫmu, estrellas de Artoria, Aliento/Tos, Deuda, Tesoro,
Sueño, Sello y ventanas NP. Los listeners que encadenan recursos tienen una interfaz contextual
complementaria; la interfaz anterior no cambia. El auditor de sintaxis impide que una llamada desde
un método con `PlayerChoiceContext` vuelva a seleccionar accidentalmente el overload antiguo.

Los delegates de compatibilidad de `CreatureCmd` se resuelven una sola vez. Las búsquedas de hooks
usan tipos exactos, no sólo cantidad de parámetros, para no seleccionar un overload incorrecto si
el juego agrega nuevas firmas.

## Validación

Ejecutar:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\build_compat_matrix.ps1 -Branch all -NoRestore
dotnet run --project .\tools\choice_context_audit\ChoiceContextAudit.csproj -- .
```

La matriz exige:

1. Los 13 proyectos compilan contra MAIN sin advertencias ni errores.
2. El probe MAIN encuentra las firmas antiguas y desactiva `CardPlay`.
3. El DLL MAIN de FGOCore carga contra BETA, detecta `CardPlay` y activa sólo el bridge universal.
4. Los 13 proyectos compilan contra BETA sin advertencias ni errores.
5. El probe BETA comprueba overrides nativos y bridge desactivado.

Después de una matriz completa siempre hay que volver a publicar contra MAIN: la fase BETA es la
última y deja en `dist` DLL específicos de BETA. El paquete MAIN es el artefacto universal que se
distribuye para ambas ramas.
