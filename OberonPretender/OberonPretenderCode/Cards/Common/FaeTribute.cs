using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Tributo de los Fae (妖精的贡品 / Fae Tribute) — DESIGN-OBERON §6.2 (espejo B). 0⚡ Habilidad:
/// si tenés ≥ <see cref="Cost"/> Estrellas: −Cost Estrellas → +50 Carga NP. El conversor ★→NP, espejo
/// de la Niebla del Ensueño: cuando la banca de estrellas está llena, la fundís en medidor para cruzar
/// los 100. El up ABARATA la conversión (consume 40★ en vez de 50; el NP queda en 50). Glow si pagable.
/// </summary>
public sealed class FaeTribute() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int Cost = 50;
    private const int Charge = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Cost", Cost),
        new DynamicVar("Charge", Charge)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    private bool CanConvert => CritStars.Of(Owner.Creature) >= DynamicVars["Cost"].IntValue;

    protected override bool ShouldGlowGoldInternal => CanConvert;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CanConvert) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["Cost"].IntValue, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
    }

    // Up: la conversión se abarata (50 → 40 ★); el premio en NP no cambia.
    protected override void OnUpgrade() => DynamicVars["Cost"].UpgradeValueBy(-10m);
}
