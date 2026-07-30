using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Fachada compatible del antiguo recurso local. Toda lectura, ganancia y gasto nuevo se dirige al
/// banco global de FGOCore; las interfaces históricas permanecen para no romper saves/binarios.
/// </summary>
public static class Stars
{
    public static int Of(Creature creature) => CritStars.Of(creature);

    public static Task Gain(Creature creature, int amount, CardModel? source) =>
        CritStars.Gain(creature, amount, source);

    public static Task Gain(
        PlayerChoiceContext choiceContext, Creature creature, int amount, CardModel? source) =>
        CritStars.Gain(choiceContext, creature, amount, source);

    [Obsolete("Critical v2 always spends 50 stars; discounts no longer alter the global cost.")]
    public static int DiscountedCost(Creature creature, int cost) => CritStarsPower.CritCost;

    [Obsolete("Use a damage hook gated by Criticals.WillCrit instead.")]
    public static int CritBonus(Creature creature)
    {
        var bonus = 0;
        Listeners.ForEach<ICritDamageBoost>(creature, boost => bonus += boost.CritDamageBonus);
        return bonus;
    }

    [Obsolete("Use Criticals.WillCrit(owner, card).")]
    public static bool CanCrit(Creature creature, int cost) =>
        (creature.HasPower<Forms.SummerBerserkerFormPower>() ||
         creature.HasPower<Forms.AvalonFormPower>() ||
         creature.HasPower<AroundCaliburnWindowPower>()) &&
        CritStars.CanPay(creature, CritStarsPower.CritCost);

    public static Task ConsumeForCrit(Creature creature, int cost, CardModel? source) =>
        ConsumeForCrit(new BlockingPlayerChoiceContext(), creature, cost, source);

    public static async Task ConsumeForCrit(
        PlayerChoiceContext choiceContext, Creature creature, int cost, CardModel? source)
    {
        await ConsumeExactStars(choiceContext, creature, CritStarsPower.CritCost, source);
    }

    public static Task ConsumeExactStars(Creature creature, int exactStars, CardModel? source) =>
        ConsumeExactStars(new BlockingPlayerChoiceContext(), creature, exactStars, source);

    public static async Task ConsumeExactStars(
        PlayerChoiceContext choiceContext, Creature creature, int exactStars, CardModel? source)
    {
        if (!await CritStars.Spend(choiceContext, creature, CritStarsPower.CritCost, source)) return;
        await Listeners.ForEachListener<ICritListener>(creature, listener =>
            listener is ICritListenerWithContext contextual
                ? contextual.AfterCritConsumed(choiceContext, CritStarsPower.CritCost)
                : listener.AfterCritConsumed(CritStarsPower.CritCost));
    }
}

/// <summary>Contrato histórico; Critical v2 mantiene fijo el coste global.</summary>
public interface ICritDiscount
{
    int CritCostReduction { get; }
}

/// <summary>Contrato histórico de bono plano de crítico.</summary>
public interface ICritDamageBoost
{
    int CritDamageBonus { get; }
}

public interface ICritListener
{
    Task AfterCritConsumed(int starsSpent);
}

public interface ICritListenerWithContext : ICritListener
{
    Task AfterCritConsumed(PlayerChoiceContext choiceContext, int starsSpent);
}
