using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#14 Recaudación (征税) — retocada v2 (denominación skill): Carga NP 20;
/// si algún enemigo tiene Maldición: +20 adicional. Glow dorado. (up: base 20→30)</summary>
public sealed class TaxCollection() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 20),
        new DynamicVar("Bonus", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Curses.CursedEnemies(Owner.Creature.CombatState, Owner.Creature) > 0;

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
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
