using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>
/// Mash-side facade over FGOCore's FormSwitch, plus her form queries.
/// Paladin is permanent: once entered, no other form can replace it (IsPermanent).
/// </summary>
public static class Forms
{
    public static bool InOffensiveForm(Creature creature) =>
        creature.HasPower<OrtinaxFormPower>() || creature.HasPower<PaladinFormPower>();

    public static Task Enter<T>(PlayerChoiceContext? choiceContext, Creature creature, CardModel? source) where T : FormPower =>
        FormSwitch.Enter<T>(choiceContext, creature, source);
}
