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

    private bool _triggeredThisCombat;

    public override Task BeforeCombatStart()
    {
        _triggeredThisCombat = false;
        return Task.CompletedTask;
    }

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_triggeredThisCombat || target != Owner.Creature) return 0m;

        var hp = target.CurrentHp;
        if (hp <= 1 || amount < hp) return 0m;

        _triggeredThisCombat = true;
        Flash();
        return -(amount - (hp - 1));
    }
}
