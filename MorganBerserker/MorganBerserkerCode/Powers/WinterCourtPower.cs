using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MorganBerserker.MorganBerserkerCode.Cards.Special;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Corte del Invierno (冬之宫廷) — rediseño 2026-06-15 (swap Estrellas→Maldición): at the start
/// of your turn add Amount Knight's Arm(s) to your hand; whenever you play a Knight's Arm (from
/// any source): gain 5 NP Charge ("las espadas de los caídos vuelven como luz"). Generador
/// sostenido + payoff de la tribu en una sola carta — conecta Armas del Caballero a la Carga NP.
/// El NP es fijo por Arma (no escala con stacks; solo la generación de Armas escala).
/// </summary>
public sealed class WinterCourtPower : MorganPower
{
    public const int NpPerArm = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await KnightsArm.AddToHand(Owner, Amount);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not KnightsArm || cardPlay.Card.Owner != Owner.Player) return;
        Flash();
        await NpCharge.Gain(context, Owner, NpPerArm, null);
    }
}
