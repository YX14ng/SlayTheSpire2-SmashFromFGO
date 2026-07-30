using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Análisis de Combate — el 契约天使 de Mash, motor del hilo de estrellas:
/// al inicio de tu turno +<c>Amount</c> Estrellas de Crítico; cada vez que obtienes
/// CRÍTICO LISTO: roba 1 (fijo — el draw-on-CritReady usa el patrón ViciousPower:
/// AfterPowerAmountChanged + BlockingPlayerChoiceContext).
/// </summary>
public sealed class CombatAnalysisPower : MashShielderPower, ICriticalConsumedListener
{
    /// <summary>Robo fijo por cada CRÍTICO LISTO obtenido (no escala con stacks).</summary>
    public const int DrawPerCritReady = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        await CritStars.Gain(choiceContext, Owner, Amount, null);
    }

    public async Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (critical.Owner != Owner || Owner.Player == null) return;
        Flash();
        var inDeck = PileType.Draw.GetPile(Owner.Player).Cards.Count;
        var toDraw = Math.Min(DrawPerCritReady, inDeck);
        if (toDraw > 0)
            await CardPileCmd.Draw(choiceContext, toDraw, Owner.Player);
    }
}
