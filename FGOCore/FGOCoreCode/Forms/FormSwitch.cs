using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// Form switching. A permanent form (<see cref="FormPower.IsPermanent"/>) can never
/// be replaced once entered.
/// </summary>
public static class FormSwitch
{
    public static async Task Enter<T>(PlayerChoiceContext? choiceContext, Creature creature, CardModel? source) where T : FormPower
    {
        if (creature.HasPower<T>()) return;
        if (creature.GetPowerInstances<FormPower>().Any(f => f.IsPermanent)) return;

        foreach (var form in creature.GetPowerInstances<FormPower>().ToList())
        {
            await PowerCmd.Remove(form);
        }
        var applied = await PowerCmd.Apply<T>(creature, 1m, creature, source);
        if (applied != null)
        {
            FormVisuals.Apply(creature, applied);
        }

        // Combat-start setup (source == null) doesn't count as "changing form".
        if (source == null) return;

        await PowerCmd.Apply<FormShiftedPower>(creature, 1m, creature, source, silent: true);

        foreach (var listener in creature.GetPowerInstances<PowerModel>().OfType<IFormChangeListener>().ToList())
        {
            await listener.OnFormChanged(choiceContext);
        }
    }
}
