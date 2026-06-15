using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Ejecución del Mestizo (杂种处刑) — lector de Crítico. 2⚡ At: 14 de daño; si tenés Crítico
/// Listo, además 2 de Vulnerable (up 18 / 3). Glow cuando hay Crítico Listo. El golpe que querés doblar
/// deja Vulnerable para el resto del turno.</summary>
public sealed class MongrelsExecution() : GilgameshCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new PowerVar<VulnerablePower>("Vulnerable", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    private bool HasCritReady => Owner.Creature.GetPowerAmount<CritReadyPower>() > 0;

    protected override bool ShouldGlowGoldInternal => HasCritReady;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Capturado ANTES de pegar: el Crítico Listo se consume al resolver la carta.
        var critReady = HasCritReady;

        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (critReady && !cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
