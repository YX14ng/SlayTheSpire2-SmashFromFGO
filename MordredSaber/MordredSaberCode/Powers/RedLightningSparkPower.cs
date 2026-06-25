using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using FGOCore.FGOCoreCode.Combat;
using MordredSaber.MordredSaberCode.Cards.Special;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Chispa de Clarent (motor) — watcher INVISIBLE aplicado al iniciar cada combate por la starter
/// «Clarent, la Espada Robada». La 1ª vez por turno que CONSUMÍS un *Crítico Listo*, manifiesta a la
/// mano un token efímero <see cref="ChispaDeClarent"/> (AoE que escala con la forma). Implementa
/// <see cref="ICritConsumedListener"/> (lo dispara <see cref="RedLightningChannelPower"/> al detectar
/// el consumo) — mismo patrón que <see cref="HomunculusAccelerationPower"/>.
///
/// FIX HOMOGENEIZACIÓN (P1 Mordred): hace que gastar el Crítico (recurso compartido de FGOCore)
/// MANIFIESTE poder mordred-específico, no solo el ×2 genérico. CAPEADO 1/turno (reset al inicio de
/// tu turno) para no inundar la mano. Sin icono: es infraestructura, igual que RedLightningChannel.
/// </summary>
public sealed class RedLightningSparkPower : MordredPower, ICritConsumedListener
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    private bool _firedThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side) _firedThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        if (_firedThisTurn || Owner.IsDead || !CombatManager.Instance.IsInProgress) return;
        _firedThisTurn = true;
        Flash();
        await ManifestCards.ManifestToHand<ChispaDeClarent>(Owner);
    }
}
