using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Special;

/// <summary>
/// Tos (咯血 / Cough) — la carta-ESTADO de Okita (DESIGN-OKITA §5.5). Etérea y NO jugable:
/// si está en tu mano al final de tu turno, te cuesta −1 *Aliento (la enfermedad te roba el
/// pulmón). Es moneda: la consumen Pañuelo Carmesí, Medicina Amarga, Florecer Tardío, Tos en
/// el Peor Momento, Mente Despejada y la Medicina del Dr. Matsumoto.
///
/// Generada por: Aliento a 0 (máx. 1/turno), Corte de Ikedaya, Constitución Enfermiza, Flor del
/// Bakumatsu, Cerezo en Plena Floración. Va al mazo de robo (barajada), patrón estado vanilla.
/// </summary>
public sealed class Tos() : OkitaCard(0, CardType.Status, CardRarity.Status, TargetType.None)
{
    public const int BreathDrain = 1;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Unplayable];

    // Estado: no se juega.
    protected override bool IsPlayable => false;

    // Le avisa al motor que esta carta hace algo al final del turno en mano.
    public override bool HasTurnEndInHandEffect => true;

    // Mientras esté en tu mano, al fin de tu turno drena 1 Aliento.
    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        await Aliento.Spend(Owner.Creature, BreathDrain, this);
    }

    /// <summary>
    /// Genera 1 Tos y la baraja en el mazo de robo (único punto de creación para TODAS las fuentes).
    /// Avisa a los <see cref="ILateBloomListener"/> ("cada vez que ganás una Tos": Florecer Tardío).
    /// </summary>
    public static async Task ShuffleIntoDraw(Creature creature, CardModel? source)
    {
        if (creature.Player == null || creature.CombatState == null) return;
        var card = creature.CombatState.CreateCard<Tos>(creature.Player);
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: false), 0.8f);

        foreach (var listener in Listeners.PowersOf<ILateBloomListener>(creature).ToList())
        {
            await listener.OnTosGained(creature, source);
        }
    }
}

/// <summary>Listener de "cada vez que ganás una *Tos" (Florecer Tardío). Lo notifica
/// <see cref="Tos.ShuffleIntoDraw"/>, el único punto por el que se generan todas las Tos.</summary>
public interface ILateBloomListener
{
    Task OnTosGained(Creature creature, CardModel? source);
}
