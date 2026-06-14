using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Persecución (追击) — DESIGN-OKITA §5.3. 1⚡ At, RÁFAGA 1: 14 daño (up: 18). 1.5⚡ efectivos.
/// Paga el segundo coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class Pursuit() : OkitaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IRafagaCard
{
    public int RafagaCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override bool IsPlayable => Rafaga.IsPlayable(Owner.Creature, RafagaCost);

    protected override bool ShouldGlowGoldInternal => Rafaga.ShouldGlow(Owner.Creature, RafagaCost);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Rafaga.Pay(choiceContext, Owner.Creature, RafagaCost, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m); // 14 -> 18
}
