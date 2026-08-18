using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Búho Familiar — uncommon: at the end of your turn, if you played no Attack:
/// gain 1 Critical Star (rewards true Caster turns; condition distinct from the
/// form passive so they don't stack on autopilot).
/// </summary>
public sealed class FamiliarOwl : ArtoriaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            await FgoCombatState.SetTurn(context, Owner.Creature, 7, 1, cardPlay.Card);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature)) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 7) != 0) return;
        Flash();
        // v0.1.21: era 1 — valor legacy anterior a la migración ×10 de la economía de estrellas
        // (CritStarsPower.CritCost = 50). Daba 1/50 de un crítico por turno: cosméticamente inútil.
        await Stars.Gain(choiceContext, Owner.Creature, 10, null);
    }
}
