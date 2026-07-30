using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Adaptador invisible y de ID estable para el motor de Mordred. FGOCore emite el evento crítico
/// canónico; este power sólo marca que ocurrió uno este turno. Los demás listeners reciben el mismo
/// evento directamente, sin inferirlo de cambios en Crítico Listo.
/// </summary>
public sealed class RedLightningChannelPower : MordredPower, ICriticalConsumedListener
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (critical.Owner != Owner || Owner.IsDead) return;
        await PowerCmd.Apply<CritConsumedThisTurnPower>(context, Owner, 1m, Owner, critical.Card);
    }
}
