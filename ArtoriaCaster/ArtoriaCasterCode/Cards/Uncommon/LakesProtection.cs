using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Protección del Lago A (skill real S2 de Castoria; existe también en Morgan a rango C —
/// colisión temática deliberada) — Carga NP +25; en forma Caster: +10 más. Exhaust.
/// </summary>
public sealed class LakesProtection() : ArtoriaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 25),
        new DynamicVar("CasterBonus", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<ProphecyCasterFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var charge = DynamicVars["NpCharge"].IntValue;
        if (Owner.Creature.HasPower<ProphecyCasterFormPower>() || Owner.Creature.HasPower<AvalonFormPower>())
        {
            charge += DynamicVars["CasterBonus"].IntValue;
        }
        await NpCharge.Gain(Owner.Creature, charge, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
        DynamicVars["CasterBonus"].UpgradeValueBy(5m);
    }
}
