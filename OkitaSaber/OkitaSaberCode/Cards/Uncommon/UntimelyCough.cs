using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Tos en el Peor Momento (不合时宜的咯血, gag invertido) — DESIGN-OKITA §5.3. 0⚡ At: 4 daño; si tenés
/// una *Tos en tu mano: +20★ y +10 Carga NP (up: 6 / +30★ / +10). La enfermedad como combustible.
/// Glow cuando hay una Tos en mano.
/// </summary>
public sealed class UntimelyCough() : OkitaCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Stars", 20), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    private bool HasTosInHand =>
        Owner.PlayerCombatState != null && Owner.PlayerCombatState.Hand.Cards.OfType<Tos>().Any();

    protected override bool ShouldGlowGoldInternal => HasTosInHand;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hasTos = HasTosInHand;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (hasTos)
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);    // 4 -> 6
        DynamicVars["Stars"].UpgradeValueBy(10m); // +20 -> +30
    }
}
