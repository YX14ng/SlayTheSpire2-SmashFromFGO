using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (FgoCombatState.GetTurn(Owner, 3) != 0) return false;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Skill) return false;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play)) return false;

        modifiedCost = Math.Max(0m, originalCost - 1m);
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (FgoCombatState.GetTurn(Owner, 3) != 0) return;
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Skill) return;
        if (cardPlay.Card.Pile?.Type is not (PileType.Hand or PileType.Play)) return;

        await FgoCombatState.SetTurn(
            new BlockingPlayerChoiceContext(), Owner, 3, 1, cardPlay.Card);
        Flash();
    }
}
