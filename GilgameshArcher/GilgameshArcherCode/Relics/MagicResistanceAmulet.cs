using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Amuleto de Pacotilla (对魔力 E 护符) — reliquia MEME de tienda/PC (DESIGN-GILGAMESH §6), Resistencia
/// Mágica E: el primer *Débil que recibirías en cada combate se anula (sólo la magia MENOR; «un rey no se
/// digna a esquivar» el resto). Patrón MagicResistanceAmulet de Artoria / RuinedHelmet vanilla, acotado al
/// 1er Débil del combate (flag de código, no "vigilar"), gateado a <see cref="WeakPower"/> (no a cualquier
/// debuff — el rango E sólo niega lo trivial).
/// </summary>
public sealed class MagicResistanceAmulet : GilgameshRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (FgoCombatState.GetCombat(Owner.Creature, 6) != 0) return false;
        if (target != Owner.Creature || applier == null || applier.Side == target.Side) return false;
        if (canonicalPower is not WeakPower) return false;

        modifiedAmount = 0m;
        return true;
    }

    // El COMMIT va aca (audit 2026-07-05, contrato vanilla RuinedHelmet): el hook Try debe ser puro —
    // el motor puede evaluarlo especulativamente; este After corre solo si de verdad aplico.
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (FgoCombatState.GetCombat(Owner.Creature, 6) != 0 || power is not WeakPower) return;
        await FgoCombatState.SetCombat(
            new BlockingPlayerChoiceContext(), Owner.Creature, 6, 1);
        Flash();
    }
}
