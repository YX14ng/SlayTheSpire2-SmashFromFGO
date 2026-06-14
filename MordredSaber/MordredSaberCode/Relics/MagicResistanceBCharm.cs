using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Amuleto de Resistencia Mágica B (对魔力B护符) — reliquia POCO COMÚN (DESIGN-MORDRED §6): el primer
/// debuff enemigo de cada combate se anula. Es la pasiva Resistencia Mágica B real de Mordred (pasiva,
/// no cleanse activo) — uno de los DOS únicos vectores anti-debuff permitidos del pool (regla negativa
/// §2; el otro es el casco «Secreto de Cuna EX»). Patrón MagicResistanceAmulet de Artoria,
/// acotado 1/combate (flag de código, no "vigilar").
/// </summary>
public sealed class MagicResistanceBCharm : MordredRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

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
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;

        _usedThisCombat = true;
        Flash();
        modifiedAmount = 0m;
        return true;
    }
}
