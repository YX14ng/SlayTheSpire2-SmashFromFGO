using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Mandoble de Balmung — feeder ofensivo de NP que LEE el recurso de firma. Antes era 6 daño/1⚡
/// plano (DESIGN-REVIEW-2: débil vs el baseline común 9-10). Ahora 8 de daño + 4 más mientras la *Sangre
/// de Dragón sea 3 o más, manteniendo el rider de 10 NP. El umbral SdD≥3 lo comparten los Buster
/// Cazadragones (DragonbloodCut/DragonSlayerStrike) vía <see cref="SiegfriedCard.ScaleThresholdBonus"/>:
/// premia el plan de motor sin auto-alimentarse (no genera SdD). El NP no sube con el up (P10).</summary>
public sealed class BalmungSwing() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int ScalesThreshold = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Bonus", 4), new DynamicVar("Threshold", ScalesThreshold), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonScalesPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => GlowAtScales(ScalesThreshold);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = ScaleThresholdBonus(ScalesThreshold, DynamicVars["Bonus"].IntValue);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
