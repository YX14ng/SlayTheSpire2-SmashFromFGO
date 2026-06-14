using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Estocada Doble (二段突) — DESIGN-OKITA §5.2. 1⚡ At, RÁFAGA 1: 6 daño ×2 (up: 8×2). Sobre-tasa
/// pagada con *Aliento (1⚡ + ½⚡ ≈ 12-13). Paga el segundo coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class DoubleThrust() : OkitaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IRafagaCard
{
    private const int Hits = 2;

    public int RafagaCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override bool IsPlayable => Rafaga.IsPlayable(Owner.Creature, RafagaCost);

    protected override bool ShouldGlowGoldInternal => Rafaga.ShouldGlow(Owner.Creature, RafagaCost);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Rafaga.Pay(choiceContext, Owner.Creature, RafagaCost, this);
        for (var i = 0; i < Hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m); // 6 -> 8
}
