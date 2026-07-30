using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Amuleto de Fou — once per combat, the first lethal blow leaves you at 1 HP.
/// Fou never runs out of miracles for Mash.
/// </summary>
public sealed class FouAmulet : MashShielderRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private bool _pendingTrigger;

    public override Task BeforeCombatStart()
    {
        _pendingTrigger = false;
        return Task.CompletedTask;
    }

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (FgoCombatState.GetCombat(Owner.Creature, 0) != 0 || target != Owner.Creature) return amount;

        var hp = target.CurrentHp;
        if (hp <= 1 || amount < hp) return amount;

        return hp - 1;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        _pendingTrigger = true;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!_pendingTrigger || target != Owner.Creature) return;
        _pendingTrigger = false;
        if (result.UnblockedDamage <= 0) return;

        await FgoCombatState.SetCombat(choiceContext, Owner.Creature, 0, 1, cardSource);
        Flash();
    }
}
