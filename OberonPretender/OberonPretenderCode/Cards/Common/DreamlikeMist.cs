using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Niebla del Ensueño (梦境之雾 / Dreamlike Mist) — DESIGN-OBERON §6.2 (espejo A). 0⚡ Habilidad:
/// si tenés ≥ <see cref="Cost"/> de Carga NP: −Cost de Carga → +50 Estrellas. El conversor NP→★ que
/// alimenta el camino al Crítico Listo cuando el medidor está alto. El up ABARATA la conversión
/// (consume 40 en vez de 50; las ★ quedan en 50 — P5: el up baja el costo, no infla el premio).
/// Glow cuando es pagable.
/// </summary>
public sealed class DreamlikeMist() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int Cost = 50;
    private const int Stars = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Cost", Cost),
        new DynamicVar("Stars", Stars)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    private bool CanConvert => NpCharge.Current(Owner.Creature) >= DynamicVars["Cost"].IntValue;

    protected override bool ShouldGlowGoldInternal => CanConvert;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CanConvert) return;
        await NpCharge.Spend(Owner.Creature, DynamicVars["Cost"].IntValue, this);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    // Up: la conversión se abarata (50 → 40 NP); el premio en ★ no cambia.
    protected override void OnUpgrade() => DynamicVars["Cost"].UpgradeValueBy(-10m);
}
