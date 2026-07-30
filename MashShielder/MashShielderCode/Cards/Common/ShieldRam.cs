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

    private bool WillCrit => Criticals.WillCrit(Owner.Creature, this);

    protected override bool ShouldGlowGoldInternal => WillCrit;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Capturado ANTES de pegar: el CRÍTICO LISTO se consume al resolver la carta.
        var critical = Criticals.IsCritical(cardPlay);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        if (critical && !cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
