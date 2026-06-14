using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Ráfaga de Tres Pasos (三步缩地) — DESIGN-OKITA §5.3. 2⚡ At, RÁFAGA 2: 6 daño ×4 (up: 7×4).
/// Multi-hit (2⚡ + 1⚡eq = 24). Paga el segundo coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class ThreeStepDash() : OkitaCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IRafagaCard
{
    private const int Hits = 4;

    public int RafagaCost => 2;

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
            if (cardPlay.Target.IsDead) break;
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m); // 6 -> 7
}
