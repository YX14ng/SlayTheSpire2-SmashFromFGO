using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Compatibility;

public static class CreatureCmdCompatibility
{
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

    private delegate AttackCommand MainAttackFromCard(AttackCommand command, CardModel card);

    private delegate AttackCommand BetaAttackFromCard(
        AttackCommand command, CardModel card, CardPlay? cardPlay);

    private delegate Task<IEnumerable<DamageResult>> MainCardDamage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, CardModel cardSource);

    private delegate Task<IEnumerable<DamageResult>> BetaCardDamage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props,
        CardModel cardSource, CardPlay? cardPlay);

    private delegate Task<IEnumerable<DamageResult>> MainDamage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource);

    private delegate Task<IEnumerable<DamageResult>> BetaDamage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay);

    private delegate Task MainLoseBlock(Creature target, decimal amount);

    private delegate Task BetaLoseBlock(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, Creature? remover);

    private static readonly Func<PlayerChoiceContext, Creature, decimal, ValueProp, CardModel, CardPlay?, Task>
        DamageFromCardInvoker = CreateDamageFromCardInvoker();

    private static readonly Func<PlayerChoiceContext, Creature, decimal, ValueProp, Creature?, CardModel?, CardPlay?, Task>
        DamageInvoker = CreateDamageInvoker();

    private static readonly Func<PlayerChoiceContext, Creature, decimal, Creature?, Task> LoseBlockInvoker =
        CreateLoseBlockInvoker();

    private static readonly Func<AttackCommand, CardModel, CardPlay?, AttackCommand> AttackFromCardInvoker =
        CreateAttackFromCardInvoker();

    /// <summary>True when the runtime forwards the current card execution into damage hooks.</summary>
    public static bool SupportsCardPlayDamageContext { get; } =
        typeof(AttackCommand).GetMethod(nameof(AttackCommand.FromCard), PublicInstance, null,
            [typeof(CardModel), typeof(CardPlay)], null) is not null;

    public static AttackCommand FromCardFgoCompatibility(
        this AttackCommand command, CardModel card, CardPlay? cardPlay) =>
        AttackFromCardInvoker(command, card, cardPlay);

    public static Task DamageFromCard(PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay) =>
        DamageFromCardInvoker(choiceContext, target, amount, props, cardSource, cardPlay);

    public static Task Damage(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) =>
        DamageInvoker(choiceContext, target, amount, props, dealer, cardSource, cardPlay);

    public static Task LoseBlock(PlayerChoiceContext choiceContext, Creature target, decimal amount,
        Creature? remover = null) => LoseBlockInvoker(choiceContext, target, amount, remover);

    /// <summary>Compatibility overload for callers that have no multiplayer choice context.</summary>
    public static Task LoseBlock(Creature target, decimal amount, Creature? remover = null) =>
        LoseBlock(new BlockingPlayerChoiceContext(), target, amount, remover);

    private static Func<PlayerChoiceContext, Creature, decimal, ValueProp, CardModel, CardPlay?, Task>
        CreateDamageFromCardInvoker()
    {
        var beta = Find(nameof(CreatureCmd.Damage), typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
            typeof(ValueProp), typeof(CardModel), typeof(CardPlay));
        if (beta != null)
        {
            var invoke = beta.CreateDelegate<BetaCardDamage>();
            return (context, target, amount, props, source, cardPlay) =>
                invoke(context, target, amount, props, source, cardPlay);
        }

        var main = Require(nameof(CreatureCmd.Damage), typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
            typeof(ValueProp), typeof(CardModel));
        var mainInvoke = main.CreateDelegate<MainCardDamage>();
        return (context, target, amount, props, source, _) => mainInvoke(context, target, amount, props, source);
    }

    private static Func<PlayerChoiceContext, Creature, decimal, ValueProp, Creature?, CardModel?, CardPlay?, Task>
        CreateDamageInvoker()
    {
        var beta = Find(nameof(CreatureCmd.Damage), typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
            typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay));
        if (beta != null)
        {
            var invoke = beta.CreateDelegate<BetaDamage>();
            return (context, target, amount, props, dealer, source, cardPlay) =>
                invoke(context, target, amount, props, dealer, source, cardPlay);
        }

        var main = Require(nameof(CreatureCmd.Damage), typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
            typeof(ValueProp), typeof(Creature), typeof(CardModel));
        var mainInvoke = main.CreateDelegate<MainDamage>();
        return (context, target, amount, props, dealer, source, _) =>
            mainInvoke(context, target, amount, props, dealer, source);
    }

    private static Func<PlayerChoiceContext, Creature, decimal, Creature?, Task> CreateLoseBlockInvoker()
    {
        var beta = Find(nameof(CreatureCmd.LoseBlock), typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
            typeof(Creature));
        if (beta != null)
        {
            var invoke = beta.CreateDelegate<BetaLoseBlock>();
            return (choiceContext, target, amount, remover) =>
                invoke(choiceContext, target, amount, remover);
        }

        var main = Require(nameof(CreatureCmd.LoseBlock), typeof(Creature), typeof(decimal));
        var mainInvoke = main.CreateDelegate<MainLoseBlock>();
        return (_, target, amount, _) => mainInvoke(target, amount);
    }

    private static Func<AttackCommand, CardModel, CardPlay?, AttackCommand> CreateAttackFromCardInvoker()
    {
        var beta = typeof(AttackCommand).GetMethod(nameof(AttackCommand.FromCard), PublicInstance, null,
            [typeof(CardModel), typeof(CardPlay)], null);
        if (beta != null)
        {
            var invoke = beta.CreateDelegate<BetaAttackFromCard>();
            return (command, card, cardPlay) => invoke(command, card, cardPlay);
        }

        var main = typeof(AttackCommand).GetMethod(nameof(AttackCommand.FromCard), PublicInstance, null,
            [typeof(CardModel)], null)
            ?? throw new MissingMethodException(typeof(AttackCommand).FullName,
                $"{nameof(AttackCommand.FromCard)}({nameof(CardModel)}[, {nameof(CardPlay)}])");
        var mainInvoke = main.CreateDelegate<MainAttackFromCard>();
        return (command, card, _) => mainInvoke(command, card);
    }

    private static MethodInfo? Find(string name, params Type[] parameterTypes) =>
        typeof(CreatureCmd).GetMethod(name, PublicStatic, null, parameterTypes, null);

    private static MethodInfo Require(string name, params Type[] parameterTypes) =>
        Find(name, parameterTypes)
        ?? throw new MissingMethodException(typeof(CreatureCmd).FullName, $"{name}({string.Join(", ", parameterTypes.Select(t => t.Name))})");
}
