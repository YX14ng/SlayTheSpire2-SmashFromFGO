using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// El Milagro de Fou — this combat, the first lethal blow leaves you standing. Exhaust.
/// Rediseño v2: sobrevive con 1 (up 12) de Vida, +25 de Bloqueo y +50 de Carga NP —
/// el milagro arma la ult de la remontada (deja de ser guts genérico aislado).
/// </summary>
public sealed class FousMiracle() : MashShielderCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FouMiraclePower>("FouMiracle", 1m),
        new DynamicVar("RescueBlock", FouMiraclePower.RescueBlock),
        new DynamicVar("NpCharge", FouMiraclePower.RescueNpCharge)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<FouMiraclePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FouMiraclePower>(Owner.Creature, DynamicVars["FouMiracle"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FouMiracle"].UpgradeValueBy(11m);
    }
}
