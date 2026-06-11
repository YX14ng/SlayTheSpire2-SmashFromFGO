using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Amuleto de Resistencia Mágica A — uncommon: the first time an enemy would apply
/// a debuff to you each combat, it is negated (her real Magic Resistance A passive;
/// Ginger/Turnip pattern scoped to once per combat).
/// </summary>
public sealed class MagicResistanceAmulet : ArtoriaRelic
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
