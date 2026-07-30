# Compatibilidad StS2 v0.107.1 / v0.109.0

Estado verificado el 2026-07-22 contra copias aisladas de `sts2.dll`:

- MAIN: `.compat/sts2-main-0.107.1`
- BETA: `.compat/sts2-beta-0.109.0`
- BaseLib de compilación: 3.3.6; runtime comprobado: 3.3.7

## Cambios de API relevantes

- `AttackCommand.FromCard` agrega `CardPlay?`.
- Las sobrecargas de `CreatureCmd.Damage` asociadas a carta agregan `CardPlay?`.
- `CreatureCmd.LoseBlock` agrega `PlayerChoiceContext` y el responsable de quitar Bloqueo.
- Los tres hooks `ModifyDamage*` agregan `CardPlay?`.
- `AfterBlockBroken` agrega contexto y responsable.
- BETA expone `CardPlay.Player`, `BranchingPlayerChoiceContext`,
  `DrawWithoutBlockingOnOtherPlayers`, `CombatManager.CombatBegan` y
  `BeforeCombatRewardOffered`.

La decompilación selectiva confirmó que BETA pasa `CardPlay == null` durante previews y el
`CardPlay` real al resolver el ataque. Mash usa esa diferencia para evitar que el cálculo visual
consuma estado; MAIN conserva el guard explícito de vida de la jugada.

## Diseño universal

El paquete público se compila contra MAIN. En MAIN usa los overrides de cinco argumentos. Cuando
ese mismo DLL corre en BETA, `LegacyDamageHookCompatibility` detecta por firma exacta los hooks de
seis argumentos y reenvía el `CardPlay` a `IFgoDamageHooks`. Una compilación específica de BETA usa
los overrides nativos y desactiva ese bridge para no ejecutar dos veces la modificación.

Las firmas públicas anteriores de FGOCore se conservan para DLL ya compilados. Los overloads nuevos
aceptan `PlayerChoiceContext`; los nueve personajes del repositorio los usan para mantener efectos
anidados dentro de la resolución sincronizada original. Esto cubre NP, estrellas, Aliento, Deuda,
manifestación de NP y consumo/reembolso de cartas NP.

La misma regla se aplica a Maldicion, Lahmu, estrellas de Artoria, Aliento/Tos, Deuda, Tesoro,
Sueno, Sello y ventanas NP. Los listeners que encadenan recursos tienen una interfaz contextual
complementaria; la interfaz anterior no cambia. El auditor de sintaxis impide que una llamada desde
un metodo con `PlayerChoiceContext` vuelva a seleccionar accidentalmente el overload antiguo.

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

1. Los diez proyectos compilan contra MAIN sin advertencias ni errores.
2. El probe MAIN encuentra las firmas antiguas y desactiva `CardPlay`.
3. El DLL MAIN de FGOCore carga contra BETA, detecta `CardPlay` y activa sólo el bridge universal.
4. Los diez proyectos compilan contra BETA sin advertencias ni errores.
5. El probe BETA comprueba overrides nativos y bridge desactivado.

Después de una matriz completa siempre hay que volver a publicar contra MAIN: la fase BETA es la
última y deja en `dist` DLL específicos de BETA. El paquete MAIN es el artefacto universal que se
distribuye para ambas ramas.
