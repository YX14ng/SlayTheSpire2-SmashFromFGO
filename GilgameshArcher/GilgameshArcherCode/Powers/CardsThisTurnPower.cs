using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Contador OCULTO de CARTAS jugadas este turno (DESIGN-GILGAMESH §5.3, carta Disparo Anticipado).
/// CombatState no expone "¿es la primera carta del turno?", así que lo lleva este power — el motor del
/// rider «si es la 1ª carta del turno» (先制射击). Copia EXACTA del patrón
/// <c>OkitaSaber.AttacksThisTurnPower</c> (mismos métodos estáticos <see cref="PlayedBefore"/> y
/// <see cref="EnsureInstalled"/>), salvo que cuenta TODAS las cartas, no sólo Ataques.
///
/// Clave de ORDEN: <see cref="AfterCardPlayed"/> corre DESPUÉS de resolver la carta, así que en el
/// <c>OnPlay</c> de una carta <see cref="PlayedBefore"/> refleja SÓLO las cartas anteriores del turno:
/// 0 ⇒ ésta es la primera. Se auto-instala con <see cref="EnsureInstalled"/> y se resetea al inicio de
/// tu turno. Counter, oculto, personal: no escala en multijugador.
/// </summary>
public sealed class CardsThisTurnPower : GilgameshPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Cartas propias jugadas en el turno actual (las anteriores a la que se resuelve).</summary>
    public int Played { get; private set; }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) Played = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner) Played++;
        return Task.CompletedTask;
    }

    /// <summary>Cartas jugadas ANTES de la actual este turno (0 si la actual es la primera).</summary>
    public static int PlayedBefore(Creature creature) =>
        creature.GetPower<CardsThisTurnPower>()?.Played ?? 0;

    /// <summary>Garantiza que el contador esté instalado (para que empiece a contar desde ya).</summary>
    public static async Task EnsureInstalled(Creature creature)
    {
        if (creature.GetPower<CardsThisTurnPower>() == null)
        {
            await PowerCmd.Apply<CardsThisTurnPower>(creature, 1m, creature, null);
        }
    }
}
