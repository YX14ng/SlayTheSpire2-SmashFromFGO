using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>Agallas de la Madre — subclase de Guts: la 1ª vez que caerías, revivís a 1 HP y
/// PARÍS 3 Laḫmu (la muerte le da más hijos). 1/combate (Guts es Single y se consume al
/// dispararse). Lo concede la reliquia "Cuerno de King Hassan" al iniciar el combate.</summary>
public sealed class MotherGutsPower : GutsPower
{
    public const int LahmuOnRevive = 3;

    protected override async Task OnTriggered(PlayerChoiceContext choiceContext)
    {
        await Lahmu.Spawn(Owner, LahmuOnRevive, null);
    }
}
