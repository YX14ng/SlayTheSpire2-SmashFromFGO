using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Pañuelo Carmesí (染血手帕) — DESIGN-OKITA §5.2. 0⚡ Hab: exhaustá todas las *Tos de tu mano:
/// +20 Carga NP por cada una (up: +20 NP y +10★ por cada una). Glow cuando hay una Tos en mano.
/// Conversor de Estados (patrón Flak). Lee la mano vía PlayerCombatState.Hand.
/// </summary>
public sealed class CrimsonKerchief() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 20), new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    private IReadOnlyList<Tos> TosInHand =>
        Owner.Creature.CombatState != null && Owner.PlayerCombatState != null
            ? Owner.PlayerCombatState.Hand.Cards.OfType<Tos>().ToList()
            : [];

    protected override bool ShouldGlowGoldInternal => TosInHand.Count > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var np = DynamicVars["NpCharge"].IntValue;
        var stars = DynamicVars["Stars"].IntValue;
        foreach (var tos in TosInHand)
        {
            await CardCmd.Exhaust(choiceContext, tos);
            await NpCharge.Gain(Owner.Creature, np, this);
            if (stars > 0) await CritStars.Gain(Owner.Creature, stars, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m); // 0 -> +10★ por Tos
}
