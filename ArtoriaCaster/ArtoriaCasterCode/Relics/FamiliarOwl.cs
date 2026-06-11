using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    private bool _attackPlayedThisTurn;

    public override Task BeforeCombatStartLate()
    {
        _attackPlayedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _attackPlayedThisTurn = true;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        var played = _attackPlayedThisTurn;
        _attackPlayedThisTurn = false;
        if (played) return;
        Flash();
        await Stars.Gain(Owner.Creature, 1, null);
    }
}
