using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// Alzarse (Guts / 毅力) — the first time this combat that damage would kill the
/// owner: they survive at <see cref="Floor"/> HP instead. Single (a new application
/// replaces the old one). Subclass and override <see cref="OnTriggered"/> for
/// source-specific bonuses ("morir es el power spike"). Generalizes the proven
/// FouMiraclePower pattern (the ModifyHpLost* hooks are ABSOLUTE: return the input
/// amount for "no change").
/// </summary>
public class GutsPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private bool _triggered;

    /// <summary>HP left after surviving. Relics implementing <see cref="IGutsFloorBooster"/> can raise it.</summary>
    protected virtual int Floor
    {
        get
        {
            var floor = 1;
            var relics = Owner.Player?.Relics;
            if (relics != null)
            {
                foreach (var relic in relics)
                {
                    if (relic is IGutsFloorBooster booster) floor = Math.Max(floor, booster.ImproveFloor(floor));
                }
            }
            return floor;
        }
    }

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_triggered || target != Owner) return amount;

        var hp = Owner.CurrentHp;
        if (hp <= 0 || amount < hp) return amount;

        _triggered = true;
        var floor = Math.Min(Floor, hp);
        return hp - floor;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!_triggered || target != Owner) return;
        Flash();
        await OnTriggered(choiceContext);
        await PowerCmd.Remove(this);
    }

    /// <summary>Bonus when Guts fires (default: none). Subclasses add Strength, NP, etc.</summary>
    protected virtual Task OnTriggered(PlayerChoiceContext choiceContext) => Task.CompletedTask;
}
