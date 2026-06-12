using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MorganBerserker.MorganBerserkerCode.Cards.Special;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Corte del Invierno (冬之宫廷) — rediseño v2: at the start of your turn add
/// Amount Knight's Arm(s) to your hand; whenever you play a Knight's Arm (from any
/// source): gain 10 Critical Stars ("las espadas de los caídos vuelven como luz").
/// Generador sostenido + payoff de la tribu en una sola carta — conecta Armas del
/// Caballero al hilo de estrellas. Las estrellas son fijas por Arma (no escalan con
/// stacks; solo la generación de Armas escala).
/// </summary>
public sealed class WinterCourtPower : MorganPower
{
    public const int StarsPerArm = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            var card = Owner.CombatState.CreateCard<KnightsArm>(Owner.Player);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true), 0.8f);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not KnightsArm || cardPlay.Card.Owner != Owner.Player) return;
        Flash();
        await CritStars.Gain(Owner, StarsPerArm, null);
    }
}
