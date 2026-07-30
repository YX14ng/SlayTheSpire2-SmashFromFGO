using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Creación de Territorio EX (陣地作成) — al final de tu turno: ganás Amount de
/// Bloqueo; si jugaste 2+ Habilidades este turno: el doble. Contador interno con
/// AfterCardPlayed, reset en AfterSideTurnStart.
/// </summary>
public sealed class TerritoryCreationPower : ArtoriaPower
{
    public const int SkillThreshold = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner && cardPlay.Card.Type == CardType.Skill)
        {
            await FgoCombatState.IncrementTurn(
                context, Owner, 0, SkillThreshold, cardPlay.Card, width: 2);
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        Flash();
        var block = FgoCombatState.GetTurn(Owner, 0, 2) >= SkillThreshold ? Amount * 2 : Amount;
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
    }
}
