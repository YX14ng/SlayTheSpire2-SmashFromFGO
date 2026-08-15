using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Golpe del Anhelo Heredado — Ataque 2⚡: 16 de daño; con un Crítico Listo: +8 (mejora 20/+10).
/// Rebalance 2026-08-15 (docs/REBALANCE-TIAMAT-ARTORIA.md A3): el 14 plano era la mitad base de
/// la tabla de crítico propia (14/32) podada en Critical v2; el rider devuelve el payoff de rara
/// dentro del sistema global — con crítico: (16+8)×1.5 = 36, cerca del intent original.
/// </summary>
public sealed class InheritedLongingStrike() : ArtoriaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move),
        new DynamicVar("Bonus", 8)
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
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
