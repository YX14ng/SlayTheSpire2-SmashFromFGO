using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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

    private int _skillsThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _skillsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner && cardPlay.Card.Type == CardType.Skill)
        {
            _skillsThisTurn++;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        Flash();
        var block = _skillsThisTurn >= SkillThreshold ? Amount * 2 : Amount;
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
    }
}
