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
/// (1) Sos INMUNE a Maldicion: toda <see cref="CursePower"/> entrante se anula (TryModifyPowerAmountReceived
///     -> 0), permanente. (ANTES se declaraba ICursePreserver, un marcador OFENSIVO que el motor lee sobre
///     el ENEMIGO maldito para frenar el decay de TU Maldicion; sobre una relic del jugador era dead code
///     con semantica equivocada, asi que se removio.)
/// (2) El primer Debil, Fragil o Vulnerable que recibirias cada combate se anula (la Resistencia mental
///     EX, acotada a 1/combate). Patron MagicResistanceAmulet de Artoria / RuinedHelmet vanilla
///     (TryModifyPowerAmountReceived -> 0).
/// </summary>
public sealed class MidsummerNightsDreamEx : OberonRelic
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
        if (target != Owner.Creature || applier == null || applier.Side == target.Side) return false;

        // (1) Inmunidad a Maldicion: SIEMPRE se anula la Maldicion entrante (permanente, fuera del gate 1/combate).
        if (canonicalPower is CursePower && canonicalPower.GetTypeForAmount(amount) == PowerType.Debuff)
        {
            Flash();
            modifiedAmount = 0m;
            return true;
        }

        // (2) SOLO Debil/Fragil/Vulnerable (la resistencia mental EX), 1/combate.
        if (_usedThisCombat) return false;
        if (canonicalPower is not (WeakPower or FrailPower or VulnerablePower)) return false;
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;

        _usedThisCombat = true;
        Flash();
        modifiedAmount = 0m;
        return true;
    }
}
