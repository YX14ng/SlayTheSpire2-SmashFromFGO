using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Núcleo de la Diosa — pasiva persistente del combate: al inicio de TUS turnos obtenés
/// <see cref="NpPerStack"/> de Carga NP por acumulación (el núcleo divino bombea el medidor).
/// Apilable (Counter): jugar la carta de nuevo suma otra acumulación. La concede la carta
/// <c>CoreOfTheGoddess</c>; el lado de resistencia a debuffs lo da un Artefacto aparte.
/// </summary>
public sealed class TidalCorePower : TiamatPower
{
    public const int NpPerStack = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await NpCharge.Gain(Owner, NpPerStack * Amount, null);
    }
}
