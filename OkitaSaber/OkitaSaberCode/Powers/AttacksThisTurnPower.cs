using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Contador OCULTO de ATAQUES jugados este turno (DESIGN-OKITA §4: "Shukuchi N = helper
/// CardsPlayedThisTurn… si no lo expone CombatState, contador invisible reset en AfterSideTurnStart").
/// CombatState no expone un conteo de Ataques jugados, así que lo lleva este power — el motor de los
/// riders iai/Shukuchi N: «si es tu PRIMER Ataque del turno» (Battōjutsu) y «si YA jugaste otro
/// Ataque este turno» (Corte Cruzado). Mismo patrón que GilgameshArcher.CardsThisTurnPower.
///
/// Clave de ORDEN: <see cref="AfterCardPlayed"/> corre DESPUÉS de resolver la carta, así que en el
/// <c>OnPlay</c> de un Ataque <see cref="PlayedBefore"/> refleja SÓLO los Ataques anteriores del
/// turno: 0 ⇒ este es el primero. Se auto-instala con <see cref="EnsureInstalled"/> y se resetea al
/// inicio de tu turno. Counter, oculto, personal: no escala en multijugador.
/// </summary>
public sealed class AttacksThisTurnPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Ataques propios jugados en el turno actual (los anteriores al que se resuelve).</summary>
    public int Played { get; private set; }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) Played = 0;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner) Played++;
        return Task.CompletedTask;
    }

    /// <summary>Ataques jugados ANTES del actual este turno (0 si el actual es el primero).</summary>
    public static int PlayedBefore(Creature creature) =>
        creature.GetPower<AttacksThisTurnPower>()?.Played ?? 0;

    /// <summary>Garantiza que el contador esté instalado (para que empiece a contar desde ya).</summary>
    public static async Task EnsureInstalled(Creature creature)
    {
        if (creature.GetPower<AttacksThisTurnPower>() == null)
        {
            await PowerCmd.Apply<AttacksThisTurnPower>(creature, 1m, creature, null);
        }
    }
}
