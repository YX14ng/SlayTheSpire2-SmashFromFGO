using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Revés del Héroe — payoff que recompensa el motor SdD SIN número plano repetible.
/// Daño base 6 + 1 por cada Sangre de Dragón, con TOPE DURO +6 (techo §1.bis ×1.5; lección
/// TyrantsSweep P3). No genera SdD → no se auto-alimenta; depende de invertir en escamas.</summary>
public sealed class HerosBackswing() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int ScaleCap = 6; // tope duro del escalado por SdD (techo §1.bis ×1.5)

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("MaxBonus", ScaleCap)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.GetPowerAmount<DragonScalesPower>() > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = System.Math.Min(Owner.Creature.GetPowerAmount<DragonScalesPower>(), DynamicVars["MaxBonus"].IntValue);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
