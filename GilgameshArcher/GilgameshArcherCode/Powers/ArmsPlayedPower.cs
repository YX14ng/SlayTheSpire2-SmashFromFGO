using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using GilgameshArcher.GilgameshArcherCode.Cards;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Contador OCULTO "Armas del Tesoro jugadas" (DESIGN-GILGAMESH §4.1/§5.5) — vive en el MOD de
/// Gilgamesh, NO en FGOCore (regla §4.1: el contador sube al pool del personaje hasta que otro
/// servant lo pida). Lo consumen los riders «si jugaste un Arma este turno» (Andanada de Portales,
/// Muro de Lanzas, Asalto del Tesoro Abierto, Total Barrage, El Tesoro Protege al Rey...) y el
/// Botín del Conquistador («por cada Arma jugada este COMBATE»). Patrón <c>FormShiftedPower</c> +
/// el flag-por-turno de <c>WeightOfExpectationsPower</c>.
///
/// Dos cuentas separadas:
/// - <see cref="ThisTurn"/> se resetea al inicio de tu turno (BeforeSideTurnStart, patrón
///   NpResolvedThisTurnPower).
/// - <see cref="ThisCombat"/> persiste todo el combate.
///
/// CABLEO (FIX DESIGN-REVIEW-2): el contador sube SOLO desde el hook central <see cref="AfterCardPlayed"/>,
/// que castea la carta jugada a <see cref="ITreasureArm"/> — una sola fuente de verdad, en lugar de que
/// cada una de las 8 Armas llame a un Record manual (riesgo de doble-conteo / olvido). Es el equivalente
/// local del (futuro) evento <c>Arsenal.WeaponPlayed</c> de FGOCore. Para que cuente desde la PRIMERA
/// Arma del turno, el power se siembra al abrir combate (Bab-ilu, <see cref="EnsureInstalled"/>).
/// </summary>
public sealed class ArmsPlayedPower : GilgameshPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Armas jugadas en el turno actual (gate de los riders «este turno»).</summary>
    public int ThisTurn { get; private set; }

    /// <summary>Armas jugadas en todo el combate (gate del Botín del Conquistador).</summary>
    public int ThisCombat { get; private set; }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side) ThisTurn = 0;
        return Task.CompletedTask;
    }

    // Una sola fuente de verdad: cada vez que el dueño juega un Arma del Tesoro, ambas cuentas suben.
    // AfterCardPlayed corre DESPUÉS de resolver la carta, así que el OnPlay de la propia Arma NO se
    // auto-cuenta (no hace falta: los riders leen el contador en SU OnPlay, después).
    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner && cardPlay.Card is ITreasureArm)
        {
            ThisTurn++;
            ThisCombat++;
            Flash();
        }
        return Task.CompletedTask;
    }

    /// <summary>Cuenta de Armas jugadas este turno por la criatura (0 si nunca jugó ninguna).</summary>
    public static int PlayedThisTurn(Creature creature) =>
        creature.GetPower<ArmsPlayedPower>()?.ThisTurn ?? 0;

    /// <summary>Cuenta de Armas jugadas este combate por la criatura.</summary>
    public static int PlayedThisCombat(Creature creature) =>
        creature.GetPower<ArmsPlayedPower>()?.ThisCombat ?? 0;

    /// <summary>Garantiza el contador instalado desde el arranque del combate (lo siembra Bab-ilu), para
    /// que la PRIMERA Arma del turno ya quede registrada por el hook.</summary>
    public static async Task EnsureInstalled(Creature creature)
    {
        if (creature.GetPower<ArmsPlayedPower>() == null)
        {
            await PowerCmd.Apply<ArmsPlayedPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);
        }
    }
}
