using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Sueño de una Noche de Verano EX (仲夏夜之梦EX / A Midsummer Night's Dream EX) — reliquia POCO COMUN
/// (DESIGN-OBERON §7 #8), la inmunidad mental real de Oberon acotada al pool:
/// (1) Sos INMUNE a Maldicion (<see cref="ICursePreserver"/> marcador del lado-jugador + la negacion del
///     hook de abajo cubre una Maldicion entrante).
/// (2) El primer Debil, Fragil o Vulnerable que recibirias cada combate se anula (la Resistencia mental
///     EX, acotada a 1/combate). Patron MagicResistanceAmulet de Artoria / RuinedHelmet vanilla
///     (TryModifyPowerAmountReceived -> 0).
/// </summary>
public sealed class MidsummerNightsDreamEx : OberonRelic, ICursePreserver
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _usedThisCombat;

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        return base.BeforeCombatStartLate();
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (_usedThisCombat) return false;
        if (target != Owner.Creature || applier == null || applier.Side == target.Side) return false;
        // SOLO Debil/Fragil/Vulnerable (la resistencia mental EX), no cualquier debuff.
        if (canonicalPower is not (WeakPower or FrailPower or VulnerablePower)) return false;
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;

        _usedThisCombat = true;
        Flash();
        modifiedAmount = 0m;
        return true;
    }
}
