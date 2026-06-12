using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Cometa del Verano — Ataque 0⚡: SOLO jugable con 5★ en forma crítica
/// (Berserker/Avalon): consume las 5★ y hace 28 de daño (el crítico garantizado,
/// «el Comet»). Mejora: 34.
/// </summary>
public sealed class SummerComet() : ArtoriaCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int CritCost = 5;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(28m, ValueProp.Move),
        new DynamicVar("CritCost", CritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override bool IsPlayable => Stars.CanCrit(Owner.Creature, CritCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await Stars.ConsumeForCrit(Owner.Creature, CritCost, this);
        var damage = DynamicVars.Damage.BaseValue + Stars.CritBonus(Owner.Creature);

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
