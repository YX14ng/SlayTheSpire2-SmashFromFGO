using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
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

    private bool _usedThisCombat;

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        return Task.CompletedTask;
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (_usedThisCombat) return false;
        if (target != Owner.Creature || applier == null || applier.Side == target.Side) return false;
        if (canonicalPower is not WeakPower) return false;

        _usedThisCombat = true;
        Flash();
        modifiedAmount = 0m;
        return true;
    }
}
