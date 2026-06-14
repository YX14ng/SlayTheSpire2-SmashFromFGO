using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

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
/// La fuente que lo alimenta es el (futuro) evento <c>Arsenal.WeaponPlayed</c> de FGOCore; hasta
/// que ese módulo exista, las cartas/reliquias que generan Armas llaman a <see cref="Record"/>
/// directamente cuando un Arma del Tesoro se juega. El power se auto-instala vía <see cref="Of"/>.
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

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) ThisTurn = 0;
        return Task.CompletedTask;
    }

    /// <summary>Llamado cada vez que el dueño juega un Arma del Tesoro. Sube ambas cuentas.</summary>
    public void Bump()
    {
        ThisTurn++;
        ThisCombat++;
        Flash();
    }

    /// <summary>Cuenta de Armas jugadas este turno por la criatura (0 si nunca jugó ninguna).</summary>
    public static int PlayedThisTurn(Creature creature) =>
        creature.GetPower<ArmsPlayedPower>()?.ThisTurn ?? 0;

    /// <summary>Cuenta de Armas jugadas este combate por la criatura.</summary>
    public static int PlayedThisCombat(Creature creature) =>
        creature.GetPower<ArmsPlayedPower>()?.ThisCombat ?? 0;

    /// <summary>Registra una jugada de Arma del Tesoro, auto-instalando el contador si hace falta.</summary>
    public static async Task Record(Creature creature)
    {
        var power = creature.GetPower<ArmsPlayedPower>();
        if (power == null)
        {
            power = await PowerCmd.Apply<ArmsPlayedPower>(creature, 1m, creature, null);
        }
        power?.Bump();
    }
}
