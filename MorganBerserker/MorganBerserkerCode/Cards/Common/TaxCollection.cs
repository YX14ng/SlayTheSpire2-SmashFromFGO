using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#14 Recaudación (征税) — Carga NP 12; si algún enemigo tiene Maldición: +8 adicional.</summary>
public sealed class TaxCollection() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 12),
        new DynamicVar("Bonus", 8)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amount = DynamicVars["NpCharge"].IntValue;
        if (Curses.CursedEnemies(Owner.Creature.CombatState, Owner.Creature) > 0)
        {
            amount += DynamicVars["Bonus"].IntValue;
        }
        await NpCharge.Gain(Owner.Creature, amount, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(4m);
    }
}
