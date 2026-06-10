using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>Form switching. Paladin is permanent: once entered, no other form can replace it.</summary>
public static class Forms
{
    public static bool InOffensiveForm(Creature creature) =>
        creature.HasPower<OrtinaxFormPower>() || creature.HasPower<PaladinFormPower>();

    public static async Task Enter<T>(PlayerChoiceContext? choiceContext, Creature creature, CardModel? source) where T : FormPower
    {
        if (creature.HasPower<T>()) return;
        if (creature.HasPower<PaladinFormPower>()) return;

        await PowerCmd.Remove<ShielderFormPower>(creature);
        await PowerCmd.Remove<OrtinaxFormPower>(creature);
        await PowerCmd.Apply<T>(creature, 1m, creature, source);
        FormVisuals.Apply<T>(creature);

        // Combat-start setup (source == null) doesn't count as "changing form".
        if (source == null) return;

        await PowerCmd.Apply<FormShiftedPower>(creature, 1m, creature, source, silent: true);

        var heart = creature.GetPower<HomunculusHeartPower>();
        if (heart != null)
        {
            await heart.OnFormChanged(choiceContext);
        }
    }
}
