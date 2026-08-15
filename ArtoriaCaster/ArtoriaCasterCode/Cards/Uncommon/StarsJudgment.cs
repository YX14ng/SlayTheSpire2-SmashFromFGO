using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Juicio de la Estrella — 9 de daño; con un Crítico Listo: +5. Rebalance 2026-08-15
/// (docs/REBALANCE-TIAMAT-ARTORIA.md A2): el 8 plano era la mitad base de la carta con tabla de
/// crítico propia (8/20), podada en Critical v2; el rider devuelve el payoff DENTRO del sistema
/// global (el bono se suma antes del ×1.5, que consume el Crítico Listo en el mismo golpe).
/// </summary>
public sealed class StarsJudgment() : ArtoriaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Bonus", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool ShouldGlowGoldInternal => Owner?.Creature?.HasPower<CritReadyPower>() == true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.HasPower<CritReadyPower>())
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
