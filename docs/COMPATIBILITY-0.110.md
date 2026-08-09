# Compatibilidad StS2 v0.107.1 / v0.110.1

Estado verificado el 2026-08-04 contra copias aisladas de `sts2.dll`:

- MAIN: `.compat/sts2-main-0.107.1`
- BETA: `.compat/sts2-beta-0.110.1` — Steam build `24489008`, commit `db5d3552`
- BaseLib de compilación: 3.4.0 (último NuGet); runtime mínimo: 3.4.1; Workshop 3.4.3 protegido en MAIN
- Dependencia transversal: RitsuLib 0.5.10 (`Compat.0.107.1` en MAIN, paquete regular en BETA)

La referencia BETA mínima debe incluir `sts2.dll`, `sts2.xml`, `0Harmony.dll`, `GodotSharp.dll`,
`Sentry.dll` y `Sentry.Godot.dll`. `Sentry.dll` ya existía como referencia; 0.110.1 agrega
`Sentry.Godot.dll`. La sonda necesita ambas para inicializar el assembly BETA aunque el código
compile.

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

BaseLib 3.4.0 agrega subtipos localizados a la placa de tipo, múltiples reliquias iniciales en la
selección y personajes custom en estadísticas. FGOCore usa `ICustomTypeTextCard` para presentar
Buster/Arts/Quick sin duplicar texto en cada carta. La release 3.4.1 no cambia esa API pública:
agrega `%FormVfx` a `NCreatureVisualsFactory` y omite VFX de forma cuando el runtime antiguo no
dispone del holder. Por eso se compila contra el paquete 3.4.0 pero se exige 3.4.1 en los manifiestos.

BaseLib 3.4.3 fue compilado contra BETA: allí `StartRunLobby.LocalPlayer` retorna
`StartRunLobbyPlayer`, mientras MAIN 0.107.1 retorna `LobbyPlayer`. El tipo de retorno pertenece a la
firma CLR, así que su `CharacterSelectStartingRelicsPatch.OnEmbarkPressedPostfix` falla en MAIN aun
cuando el nombre de la propiedad coincide. FGOCore agrega un finalizer estrecho que reconoce esa
`MissingMethodException` exacta y sólo entonces permite continuar el Embark ya resuelto. La sonda
puede reproducirlo con `FGO_EXPECT_BASELIB_MAIN_LOBBY_MISMATCH=1` y
`FGO_BASELIB_RUNTIME_DLL=<BaseLib 3.4.3>`.

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

La sonda también recorre las referencias binarias a `sts2` de FGOCore y de los doce personajes, y
las fuerza a resolver contra el runtime examinado. Esto cubre métodos que
permanecen dormidos hasta que se juega una carta: fue lo que detectó la llamada directa de
`PoisonedBanquet` a la sobrecarga MAIN de seis parámetros de `CreatureCmd.Damage`. Esa carta usa
ahora `CreatureCmdCompatibility`.

Los 13 DLL declaran referencia directa a `STS2-RitsuLib`. FGOCore expone la capacidad de tags de
comando y los recursos secundarios NP/Estrellas; los doce `MainFile` registran su módulo y sus pares
Ancient. El probe MAIN→BETA enlaza el artefacto universal contra `STS2.RitsuLib` 0.5.10, consulta al
generador oficial para comprobar los seis IDs públicos estables y exige la referencia en cada DLL.
La DLL de RitsuLib no se copia dentro de ningún artefacto FGO.

## Validación

Ejecutar:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\build_compat_matrix.ps1 -Branch all -NoRestore
dotnet run --project .\tools\choice_context_audit\ChoiceContextAudit.csproj -- .
```

La matriz exige:

1. Los 13 proyectos compilan contra MAIN sin advertencias ni errores.
2. El probe MAIN encuentra las firmas antiguas y desactiva `CardPlay`.
3. Los 13 DLL MAIN resuelven todas sus referencias a `sts2` contra MAIN y todos enlazan el paquete
   compat de RitsuLib.
4. Los 13 DLL MAIN cargan contra BETA, detectan `CardPlay`, activan sólo el bridge universal,
   enlazan la variante BETA de RitsuLib y no conservan referencias a miembros eliminados.
5. Los 13 proyectos compilan contra BETA sin advertencias ni errores.
6. El probe BETA comprueba los 13 DLL, los overrides nativos, el bridge desactivado, la capacidad
   de tags y la variante BETA de RitsuLib.

La matriz escribe únicamente en `.compat/build-main` y `.compat/build-beta`; no modifica `dist`.
El paquete distribuible sigue siendo el compilado contra MAIN y se genera por separado mediante el
flujo normal de build/publish hacia `dist`.
