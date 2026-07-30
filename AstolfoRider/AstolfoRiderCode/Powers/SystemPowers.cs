using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AstolfoRider.AstolfoRiderCode.Powers;

[Flags]
public enum AstolfoTurnUsage
{
    Riding = 1,
    IndependentAction = 2,
    ImpossibleExistence = 4,
    DifferentAdventure = 8,
    WorldReverse = 16,
    AdventureCritical = 32,
    AdventureEvasion = 64
}

public sealed class AstolfoTurnUsagePower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Mask => Math.Max(0, (int)Amount - 1);
}

public static class AstolfoTurnUsages
{
    public static bool WasUsed(Creature owner, AstolfoTurnUsage usage) =>
        (owner.GetPower<AstolfoTurnUsagePower>()?.Mask & (int)usage) != 0;

    public static async Task Mark(
        PlayerChoiceContext context, Creature owner, AstolfoTurnUsage usage, CardModel? source)
    {
        var power = owner.GetPower<AstolfoTurnUsagePower>();
        if (power == null)
        {
            await PowerCmd.Apply<AstolfoTurnUsagePower>(
                context, owner, (int)usage + 1m, owner, source, silent: true);
            return;
        }

        var desired = (power.Mask | (int)usage) + 1m;
        if (desired != power.Amount)
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
    }
}

public sealed class HippogriffManifestedPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}
