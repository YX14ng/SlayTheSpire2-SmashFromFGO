using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Sakura Fubuki (樱花吹雪) — DESIGN-OKITA §5.4. 2⚡ At, RÁFAGA 2: 9 daño ×3 (up: 11×3). Tormenta de
/// pétalos: 3⚡ efectivos, 27 multi-hit rara. Paga el segundo coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class SakuraFubuki() : OkitaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IRafagaCard
{
    private const int Hits = 3;

    public int RafagaCost => 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

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

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m); // 9 -> 11
}
