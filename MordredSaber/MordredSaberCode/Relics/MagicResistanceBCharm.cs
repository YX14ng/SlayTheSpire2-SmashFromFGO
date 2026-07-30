using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (FgoCombatState.GetCombat(Owner.Creature, 9) != 0) return false;
        if (target != Owner.Creature || applier == null || applier.Side == target.Side) return false;
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;
        // Solo debuffs VISIBLES (audit 2026-07-05, espejo de ArtifactPower vanilla): sin este gate
        // anulaba tambien debuffs de infraestructura invisibles y quemaba el 1/combate en silencio.
        if (!canonicalPower.IsVisible) return false;

        modifiedAmount = 0m;
        return true;
    }

    // El COMMIT va aca (audit 2026-07-05, contrato vanilla): el hook Try debe ser puro — el motor
    // puede evaluarlo especulativamente; AfterModifyingPowerAmountReceived corre solo si aplico.
    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        await FgoCombatState.SetCombat(
            new BlockingPlayerChoiceContext(), Owner.Creature, 9, 1);
        Flash();
    }
}
