using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Filo del Cazadragones (屠龙之刃, §7) — el gran Buster que recompensa aguantar la armadura: 22 + 1 por
/// cada Sangre de Dragón (tope +12; up 26 / tope +16). El techo lo pone el cap, no un número plano
/// repetible (P3/P10). Lee la SdD directo (él ES el dragón, §4).
/// </summary>
public sealed class DragonSlayersEdge() : SiegfriedCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IDragonSlayerCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(22m, ValueProp.Move), new DynamicVar("MaxBonus", 12)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override bool ShouldGlowGoldInternal => GlowAtScales(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = CappedScaleBonus(DynamicVars["MaxBonus"].IntValue);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["MaxBonus"].UpgradeValueBy(4m);
    }
}
