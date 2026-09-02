using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Compatibility;

/// <summary>
/// Puente de firma para <see cref="CardCmd.Exhaust"/>.
///
/// StS2 BETA 0.111.0 cambió su retorno de <c>Task</c> a <c>Task&lt;CardPileAddResult?&gt;</c> sin tocar
/// los parámetros ni la semántica (sigue esperando <c>CardPileCmd.Add</c> y disparando
/// <c>Hook.AfterCardExhausted</c>; verificado en el binario). El retorno es parte de la firma CLR, así
/// que el DLL compilado contra MAIN tiraba <see cref="MissingMethodException"/> al resolver la carta —
/// un fallo que duerme hasta que se juega, igual que el de <c>PoisonedBanquet</c>.
///
/// A diferencia de <see cref="CreatureCmdCompatibility"/>, acá no hacen falta dos ramas: los tipos de
/// parámetro son idénticos entre MAIN y BETA, así que un solo <see cref="Type.GetMethod(string, BindingFlags, Binder, Type[], ParameterModifier[])"/>
/// encuentra la sobrecarga correcta en ambas, y el binding relajado de delegates liga un método que
/// devuelve <c>Task&lt;T&gt;</c> a un delegate que devuelve <c>Task</c> (covarianza de retorno por
/// referencia). Ramificar acá sería complejidad muerta.
/// </summary>
public static class CardCmdCompatibility
{
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

    private delegate Task ExhaustInvoker(
        PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal, bool skipVisuals);

    private static readonly MethodInfo ExhaustMethod = RequireExhaust();

    private static readonly ExhaustInvoker Invoker = ExhaustMethod.CreateDelegate<ExhaustInvoker>();

    /// <summary>
    /// True cuando el runtime devuelve el <c>CardPileAddResult</c> del Agotar (BETA 0.111.0+).
    /// Lo consume la matriz de compatibilidad para comprobar contra qué rama ligó el puente; el
    /// resultado tipado sigue siendo recuperable por reflexión desde el <c>Task</c> que retorna
    /// <see cref="Exhaust"/>, si alguna vez se quiere el tween diferido de <c>fromSilentAdd</c>.
    /// </summary>
    public static bool SupportsExhaustResult { get; } = ExhaustMethod.ReturnType != typeof(Task);

    /// <summary>
    /// Agota una carta resolviendo la sobrecarga correcta según la rama del juego. Los valores por
    /// defecto replican los del runtime (<c>causedByEthereal = false</c>, <c>skipVisuals = false</c>).
    /// </summary>
    public static Task Exhaust(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal = false, bool skipVisuals = false) =>
        Invoker(choiceContext, card, causedByEthereal, skipVisuals);

    private static MethodInfo RequireExhaust() =>
        typeof(CardCmd).GetMethod(nameof(CardCmd.Exhaust), PublicStatic, null,
            [typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool), typeof(bool)], null)
        ?? throw new MissingMethodException(typeof(CardCmd).FullName,
            $"{nameof(CardCmd.Exhaust)}({nameof(PlayerChoiceContext)}, {nameof(CardModel)}, bool, bool)");
}
