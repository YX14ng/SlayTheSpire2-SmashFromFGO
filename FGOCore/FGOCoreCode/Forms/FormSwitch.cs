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

        // El conteo de golpes totalmente bloqueados es POR TURNO, no por instancia de forma:
        // capturarlo antes de remover la forma vieja y sembrarlo en la nueva para que no se reinicie
        // a 0 al switchear a mitad de turno (lo leen Reprisal de Mash, AntiPurge de Artoria, etc).
        var carriedBlockedHits = FormPower.GetBlockedHits(creature);

        foreach (var form in creature.GetPowerInstances<FormPower>().ToList())
        {
            await PowerCmd.Remove(form);
        }
        var applied = await PowerCmd.Apply<T>(choiceContext ?? new BlockingPlayerChoiceContext(), creature, 1m, creature, source);
        if (applied != null)
        {
            applied.SeedBlockedHits(carriedBlockedHits);
            FormVisuals.Apply(creature, applied);
        }

        // Combat-start setup (source == null) doesn't count as "changing form".
        if (source == null) return;

        await PowerCmd.Apply<FormShiftedPower>(choiceContext ?? new BlockingPlayerChoiceContext(), creature, 1m, creature, source, silent: true);

        foreach (var listener in creature.GetPowerInstances<PowerModel>().OfType<IFormChangeListener>().ToList())
        {
            await listener.OnFormChanged(choiceContext);
        }
        if (creature.Player != null)
        {
            foreach (var relic in creature.Player.Relics)
            {
                if (relic is IFormChangeListener listener)
                {
                    await listener.OnFormChanged(choiceContext);
                }
            }
        }
    }
}
