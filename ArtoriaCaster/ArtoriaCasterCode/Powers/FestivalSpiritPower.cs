using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Espíritu del Festival — cada vez que cambiás de forma: ganás 1★ y
/// <see cref="BlockPerSwitch"/> de Bloqueo. Notificado por el FormSwitch de FGOCore
/// vía IFormChangeListener (solo cambios iniciados por el jugador).
/// </summary>
public sealed class FestivalSpiritPower : ArtoriaPower, IFormChangeListener
{
    public const int StarsPerSwitch = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Bloqueo por cambio de forma (3; 5 con la carta mejorada).</summary>
    public int BlockPerSwitch { get; set; } = 3;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        Flash();
        await Stars.Gain(Owner, StarsPerSwitch, null);
        await CreatureCmd.GainBlock(Owner, BlockPerSwitch, ValueProp.Unpowered, null);
    }
}
