using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Disfraz Absoluto (绝对伪装 / Absolute Disguise) — DESIGN-OBERON §6.3. 1⚡ Habilidad · Exhaust: 1
/// Intangible (ni la Clarividencia de Merlin lo percibe); up: además +20 Carga NP (precedente 幻术 de
/// Jeanne).
/// </summary>
public sealed class AbsoluteDisguise() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("Charge", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<IntangiblePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IntangiblePower>(Owner.Creature, DynamicVars["Intangible"].BaseValue, Owner.Creature, this);
        var charge = DynamicVars["Charge"].IntValue;
        if (charge > 0) await NpCharge.Gain(Owner.Creature, charge, this);
    }

    protected override void OnUpgrade() => DynamicVars["Charge"].UpgradeValueBy(20m);
}
