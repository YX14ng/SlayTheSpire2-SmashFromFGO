using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Polilla Halcon (鹰蛾 / Hawk Moth) — reliquia POCO COMUN (DESIGN-OBERON §7 #6), la pasiva Riding A
/// mecanizada (monta una polilla halcon a 130 km/h cuando nadie mira): la PRIMERA carta *Quick
/// (<see cref="IQuickCard"/>) que jugas cada turno te hace robar 1. Tope ESTRUCTURAL 1/turno (flag de
/// codigo, no "vigilar"). Reuso puro del robo; sin ×global ni motor nuevo.
/// </summary>
public sealed class HawkMoth : OberonRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _firedThisTurn;

    public override Task BeforeCombatStartLate()
    {
        _firedThisTurn = false;
        return base.BeforeCombatStartLate();
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _firedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_firedThisTurn || cardPlay.Card.Owner != Owner || cardPlay.Card is not IQuickCard) return;
        _firedThisTurn = true;
        Flash();
        await CardPileCmd.Draw(context, 1, Owner);
    }
}
