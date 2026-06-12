using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Embate de Escudo — rediseño v2: 1E Ataque, 8 daño (up +3); si tenés CRÍTICO
/// LISTO: aplica 2 Vulnerable (up +1). EL golpe que querés doblar — el ×2 lo
/// consume y deja Vulnerable para el resto del turno.
/// </summary>
public sealed class ShieldRam() : MashShielderCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new PowerVar<VulnerablePower>("Vulnerable", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    private bool HasCritReady => Owner.Creature.GetPowerAmount<CritReadyPower>() > 0;

    protected override bool ShouldGlowGoldInternal => HasCritReady;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Capturado ANTES de pegar: el CRÍTICO LISTO se consume al resolver la carta.
        var critReady = HasCritReady;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        if (critReady && !cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
