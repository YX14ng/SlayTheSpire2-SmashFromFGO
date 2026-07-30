using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Impuesto de Existencia (存在税) — at the end of your turn: gain NP Charge equal
/// to the total Curse on enemies (max +6). The Tyranny archetype's enabler.
/// </summary>
public sealed class ExistenceTax : MorganRelic
{
    public const int MaxPerTurn = 6;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", MaxPerTurn)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature)) return;
        if (Owner.Creature.CombatState is not { } combatState) return;

        var total = 0;
        foreach (var enemy in combatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead) total += Curses.Of(enemy);
        }
        if (total <= 0) return;

        Flash();
        await NpCharge.Gain(choiceContext, Owner.Creature, Math.Min(total, MaxPerTurn), null);
    }
}
