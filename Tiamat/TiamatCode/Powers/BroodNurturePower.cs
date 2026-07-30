using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Vientre de la Bestia — pasiva persistente: al inicio de cada turno tuyo ganás <see cref="NurturePerStack"/>
/// de Crianza por acumulación (la Madre nutre al enjambre turno a turno; cada Crianza hace que TODAS las
/// mordidas de Laḫmu peguen más fuerte). El motor de escalado ofensivo del enjambre — el "DemonForm" de la
/// cría. La concede la carta rara homónima. Apilable (Counter).
/// </summary>
public sealed class BroodNurturePower : TiamatPower
{
    public const int NurturePerStack = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await Lahmu.Feed(choiceContext, Owner, NurturePerStack * Amount, null);
    }
}
