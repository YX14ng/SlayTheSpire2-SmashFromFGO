using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Herencia Negada (被否定的继承) — DESIGN-MORDRED §5.3. 0⚡ Hab, Exhaust: +30 de Carga NP + 30 Estrellas
/// (up +40/+40). Burst doble de las dos economías de un saque (slot Maldición E): la herencia que le
/// negaron, cobrada en recursos. Exhaust paga el 0⚡ (§9). Sube AMBOS con el up.
/// </summary>
public sealed class DeniedInheritance() : MordredCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 30), new DynamicVar("Stars", 30)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
