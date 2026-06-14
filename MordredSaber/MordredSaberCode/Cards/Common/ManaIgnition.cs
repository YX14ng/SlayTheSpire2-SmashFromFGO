using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Ignición de Maná (魔力点火) — DESIGN-MORDRED §5.1. 0⚡ Hab: +30 de Carga NP (up +50). El feeder de
/// NP puro de 0⚡ (sabor Mana Burst); UN solo rider, sin grapados. Patrón Mana Ignition / Last Will.
/// </summary>
public sealed class ManaIgnition() : MordredCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 30)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(20m);
}
