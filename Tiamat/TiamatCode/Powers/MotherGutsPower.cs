using BaseLib.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TiamatBeast.TiamatCode.Extensions;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>Agallas de la Madre — subclase de Guts: la 1ª vez que caerías, revivís a 1 HP y
/// PARÍS 3 Laḫmu (la muerte le da más hijos). 1/combate (Guts es Single y se consume al
/// dispararse). Lo concede la reliquia "Cuerno de King Hassan" al iniciar el combate.</summary>
public sealed class MotherGutsPower : GutsPower
{
    public const int LahmuOnRevive = 3;

    // El icono vive en los recursos de Tiamat (no en FGOCore): como esta clase extiende GutsPower
    // (base FGOCore) en vez de TiamatPower, hay que re-overridear las rutas a las imágenes del mod.
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    protected override async Task OnTriggered(PlayerChoiceContext choiceContext)
    {
        await Lahmu.Spawn(Owner, LahmuOnRevive, null);
    }
}
