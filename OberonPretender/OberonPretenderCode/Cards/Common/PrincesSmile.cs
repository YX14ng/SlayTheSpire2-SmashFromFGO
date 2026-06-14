using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Sonrisa del Príncipe (王子的微笑 / Prince's Smile) — DESIGN-OBERON §6.2. 0⚡ Habilidad · Exhaust:
/// +20 Carga NP (up +30). La batería de un solo uso (Exhaust paga el burst a 0⚡): empuja el medidor
/// sin Deuda. El up sube el NP.
/// </summary>
public sealed class PrincesSmile() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Np", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(10m);
}
