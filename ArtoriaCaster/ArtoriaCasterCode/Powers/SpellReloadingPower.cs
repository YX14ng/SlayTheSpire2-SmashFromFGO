using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Recarga de Hechizos (Append 5) — las primeras <see cref="PowerModel.Amount"/> Habilidades que
/// jugás cada turno cuestan 1⚡ menos. Usa el hook vanilla TryModifyEnergyCostInCombat (patrón
/// FreeSkillPower: el costo se captura antes de BeforeCardPlayed, así que marcar el contador ahí
/// no le quita el descuento a la propia carta jugada). Rebalance 2026-08-15: el contador pasó del
/// bit 3 (flag) a los bits 9-10 (width 2) del estado de turno para poder contar hasta 2 con la
/// carta mejorada; el bit 3 queda libre (7 y 8 los usan Búho Familiar y Espada Sagrada Forjada).
/// </summary>
public sealed class SpellReloadingPower : ArtoriaPower
{
    private const int StateOffset = 9;
    private const int StateWidth = 2;

    public override PowerType Type => PowerType.Buff;

    // Counter: el icono muestra cuántas Habilidades por turno reciben el descuento (1 base, 2 con
    // la carta mejorada; dos copias también apilan a 2).
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (FgoCombatState.GetTurn(Owner, StateOffset, StateWidth) >= (int)Amount) return false;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Skill) return false;
        if (card.Pile?.Type is not (PileType.Hand or PileType.Play)) return false;

        modifiedCost = Math.Max(0m, originalCost - 1m);
        return true;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var used = FgoCombatState.GetTurn(Owner, StateOffset, StateWidth);
        if (used >= (int)Amount) return;
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Skill) return;
        if (cardPlay.Card.Pile?.Type is not (PileType.Hand or PileType.Play)) return;

        await FgoCombatState.SetTurn(
            new BlockingPlayerChoiceContext(), Owner, StateOffset, used + 1, cardPlay.Card, StateWidth);
        Flash();
    }
}
