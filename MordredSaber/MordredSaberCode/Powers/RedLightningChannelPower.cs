using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Canalización del Relámpago Rojo — watcher INVISIBLE aplicado al iniciar cada combate por
/// la starter «Clarent, la Espada Robada». Es la pieza que materializa el motor ★→×2→NP sin
/// tocar FGOCore (DESIGN-MORDRED §4.1): vigila la cuenta de *Crítico Listo* (FGOCore
/// <c>CritReadyPower</c>) y, cuando BAJA tras jugar una carta (= se consumió un crítico),
/// (1) marca el turno con <see cref="CritConsumedThisTurnPower"/> (lo leen los riders «si
/// consumiste un CRÍTICO este turno») y (2) avisa a cada <see cref="ICritConsumedListener"/>
/// del owner y de las reliquias del jugador — mismo broadcast que FormSwitch.Enter.
///
/// Arranca el combate con 0 críticos, así que el snapshot lazy (0) es correcto; se resincroniza
/// al inicio del turno por si el conteo cambió fuera de un juego de carta. Patrón
/// <c>_isProcessing</c>/snapshot ya documentado (CritStarsPower). Sin icono: es infraestructura.
/// </summary>
public sealed class RedLightningChannelPower : MordredPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    private int _lastSeen;
    private bool _isProcessing;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _lastSeen = Owner.GetPowerAmount<CritReadyPower>();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_isProcessing || !CombatManager.Instance.IsInProgress) return;
        if (cardPlay.Card.Owner?.Creature != Owner) return;

        var current = Owner.GetPowerAmount<CritReadyPower>();
        // Cada carta de Ataque consume a lo sumo 1 stack (parche P8 de CritReadyPower):
        // una sola baja por carta. Si subió (una carta dio crítico), solo refrescamos.
        var consumed = _lastSeen - current;
        _lastSeen = current;
        if (consumed <= 0) return;

        _isProcessing = true;
        try
        {
            for (var i = 0; i < consumed; i++)
            {
                await PowerCmd.Apply<CritConsumedThisTurnPower>(Owner, 1m, Owner, null);
                await Broadcast(context);
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }

    private async Task Broadcast(PlayerChoiceContext? choiceContext)
    {
        foreach (var listener in Owner.GetPowerInstances<PowerModel>().OfType<ICritConsumedListener>().ToList())
        {
            await listener.OnCritConsumed(choiceContext);
        }
        if (Owner.Player != null)
        {
            foreach (var relic in Owner.Player.Relics)
            {
                if (relic is ICritConsumedListener listener)
                {
                    await listener.OnCritConsumed(choiceContext);
                }
            }
        }
    }
}
