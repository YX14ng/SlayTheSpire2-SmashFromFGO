using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Recarga de Hechizos (Append 5) — la PRIMERA Habilidad que jugás cada turno
/// cuesta 1⚡ menos. Usa el hook vanilla TryModifyEnergyCostInCombat (patrón
/// FreeSkillPower: el costo se captura antes de BeforeCardPlayed, así que marcar
/// el flag ahí no le quita el descuento a la propia carta jugada).
/// </summary>
public sealed class SpellReloadingPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private bool _usedThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _usedThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (_usedThisTurn) return false;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Skill) return false;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play)) return false;

        modifiedCost = Math.Max(0m, originalCost - 1m);
        return true;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (_usedThisTurn) return Task.CompletedTask;
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Skill) return Task.CompletedTask;
        if (cardPlay.Card.Pile?.Type is not (PileType.Hand or PileType.Play)) return Task.CompletedTask;

        _usedThisTurn = true;
        Flash();
        return Task.CompletedTask;
    }
}
