using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Tiamat.TiamatCode.Cards.Rare;

/// <summary>Once Bel Laḫmu — parís una camada entera de golpe (hasta el tope del enjambre) y
/// ganás Crianza igual a tus Laḫmu en campo. La explosion de poblacion. Exhaust.</summary>
public sealed class ElevenBelLahmu() : TiamatCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 6),
        new DynamicVar("NpCharge", 15)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Lahmu.Spawn(Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        await Lahmu.Feed(Owner.Creature, Lahmu.Count(Owner.Creature), this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    // Mejora: mas Carga NP (parir una camada acerca la ventana de Genesis).
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}
