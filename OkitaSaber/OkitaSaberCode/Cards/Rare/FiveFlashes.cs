using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Cinco Destellos (五连闪) — DESIGN-OKITA §5.4. 2⚡ At, RÁFAGA 1: 3 daño ×5. Si tenés CRÍTICO LISTO,
/// lo consume y LOS CINCO golpes critican (up: 4×5). Los 5 hits de su Quick real; arregla que el
/// multi-hit desperdicie el crítico — todos los golpes comparten cardSource, así que el ×2 de
/// <see cref="CritReadyPower"/> (parche P8) los dobla a todos y consume 1 stack. Paga el segundo
/// coste vía <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class FiveFlashes() : OkitaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IRafagaCard
{
    private const int Hits = 5;

    public int RafagaCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AlientoPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

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

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m); // 3 -> 4
}
