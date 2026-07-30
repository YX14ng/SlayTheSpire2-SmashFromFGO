using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public sealed class DoctrineTurnStatePower : KagetoraPower, IResourcePower
{
    private int State => Math.Max(0, (int)Amount - 1);

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Advances => State & 3;
    public int AdvancedMask => State >> 2;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Amount != 1m)
            await PowerCmd.ModifyAmount(
                context, this, 1m - Amount, Owner, null, silent: true);
    }
}

public static class DoctrineTurnState
{
    public static async Task Set(
        PlayerChoiceContext context, Creature owner, int advances, int advancedMask, CardModel? source)
    {
        var desired = 1m + (advances & 3) + (advancedMask << 2);
        var power = owner.GetPower<DoctrineTurnStatePower>();
        if (power == null)
        {
            await PowerCmd.Apply<DoctrineTurnStatePower>(
                context, owner, desired, owner, source, silent: true);
        }
        else if (power.Amount != desired)
        {
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
        }
    }
}

[Flags]
public enum KagetoraUsage
{
    Riding = 1,
    GeneralsDoctrine = 2,
    WhiteFlame = 4,
    EightFormations = 8,
    FieldJudge = 16,
    VictoryInTheFeet = 32,
    EightPetalBanner = 64,
    HoushoutsukigeReins = 128,
    SixPlateArmour = 256,
    ShiranuiTachi = 512,
    SakeCup = 1024,
    WhiteFlameBrazier = 2048
}

public sealed class KagetoraUsagePower : KagetoraPower, IResourcePower
{
    private const int PerTurnMask =
        (int)(KagetoraUsage.Riding | KagetoraUsage.GeneralsDoctrine |
              KagetoraUsage.WhiteFlame | KagetoraUsage.EightFormations |
              KagetoraUsage.FieldJudge | KagetoraUsage.VictoryInTheFeet |
              KagetoraUsage.EightPetalBanner | KagetoraUsage.HoushoutsukigeReins |
              KagetoraUsage.SixPlateArmour | KagetoraUsage.ShiranuiTachi);

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Mask => Math.Max(0, (int)Amount - 1);

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        var desired = (Mask & ~PerTurnMask) + 1m;
        if (desired != Amount)
            await PowerCmd.ModifyAmount(
                context, this, desired - Amount, Owner, null, silent: true);
    }
}

public static class KagetoraUsages
{
    public static bool WasUsed(Creature owner, KagetoraUsage usage) =>
        (owner.GetPower<KagetoraUsagePower>()?.Mask & (int)usage) != 0;

    public static async Task Mark(
        PlayerChoiceContext context, Creature owner, KagetoraUsage usage, CardModel? source)
    {
        var power = owner.GetPower<KagetoraUsagePower>();
        if (power == null)
        {
            await PowerCmd.Apply<KagetoraUsagePower>(
                context, owner, (int)usage + 1m, owner, source, silent: true);
            return;
        }

        var desired = (power.Mask | (int)usage) + 1m;
        if (desired != power.Amount)
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
    }
}
