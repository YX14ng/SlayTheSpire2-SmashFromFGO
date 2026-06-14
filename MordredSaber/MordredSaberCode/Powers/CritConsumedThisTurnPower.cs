using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Marcador OCULTO «consumiste un CRÍTICO este turno». Lo aplica <see cref="RedLightningChannelPower"/>
/// al detectar un consumo de *Crítico Listo*, y se auto-remueve al inicio del turno (espejo de
/// NpResolvedThisTurnPower). Lo leen los riders condicionales del pool (Content): «Espadazo
/// Insolente» (+10 NP), «Relámpago Encadenado» (+6 a todos), «Avalancha de Odio» (+2/golpe).
/// Es Counter para que «Aceleración de Homúnculo» pueda cap-ear su 1/turno mirando el conteo si
/// hace falta; las cartas binarias solo consultan <c>HasPower</c>.
/// </summary>
public sealed class CritConsumedThisTurnPower : MordredPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) await PowerCmd.Remove(this);
    }
}
