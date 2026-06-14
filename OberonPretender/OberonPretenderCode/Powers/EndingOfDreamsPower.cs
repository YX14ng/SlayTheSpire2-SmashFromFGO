using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// El Fin de los Sueños (Ending of Dreams EX) -- DESIGN-OBERON 6.4, la apoteosis clímax (S3): este
/// turno tus Ataques hacen +50% (×1.5) y tu PRÓXIMA carta-NP hace ×2; al final del turno perdés TODOS
/// tus Poderes positivos y NO robás cartas el próximo turno (caés en el Sueño Eterno). Tasa 500%, el
/// demérito es irrenunciable.
///
/// - El ×1.5 a todo Ataque + ×2 a la primera <see cref="IOberonNpCard"/> viven en
///   <see cref="ModifyDamageMultiplicative"/> (los multiplicadores de StS componen: ×1.5×2 = ×3 sobre
///   la NP); el flag <see cref="_nextNpDoubled"/> se consume cuando esa NP se juega
///   (<see cref="AfterCardPlayed"/>).
/// - En <see cref="AfterTurnEnd"/> instala <see cref="NoDrawNextTurnPower"/> y se quita TODOS los Buff
///   positivos (incluido este power). Las FORMAS (<see cref="FormPower"/>) se conservan: son identidad/
///   visual, no un "Poder positivo" del kit, y Vortigern es permanente.
/// </summary>
public sealed class EndingOfDreamsPower : OberonPower
{
    public const decimal AttackMultiplier = 1.5m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _nextNpDoubled = true;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 1m;
        var mult = AttackMultiplier;
        if (_nextNpDoubled && cardSource is IOberonNpCard) mult *= 2m;
        return mult;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_nextNpDoubled && cardPlay.Card is IOberonNpCard && cardPlay.Card.Owner?.Creature == Owner)
        {
            _nextNpDoubled = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        Flash();

        // El Sueño Eterno: no robás el próximo turno.
        await PowerCmd.Apply<NoDrawNextTurnPower>(Owner, 1m, Owner, null);

        // Perdés todos los Poderes positivos (las formas son identidad → se conservan).
        var buffs = Owner.GetPowerInstances<PowerModel>()
            .Where(p => p.TypeForCurrentAmount == PowerType.Buff && p is not FormPower)
            .ToList();
        foreach (var buff in buffs)
        {
            await PowerCmd.Remove(buff);
        }
    }
}

/// <summary>
/// El Sueño Eterno (No Draw Next Turn) -- el demérito diferido de El Fin de los Sueños: NO robás
/// cartas en tu próximo turno. <see cref="ModifyHandDraw"/> es ABSOLUTO (gotcha WORKFLOW-FGO: el
/// default devuelve el input), así que SOLO devuelve 0 cuando el bloqueo está armado (turno siguiente),
/// nunca en el turno en que se aplicó. Se arma al primer inicio de turno (cualquier lado) tras
/// aplicarse y se auto-remueve tras el robo del turno propio. Patrón DrawCardsNextTurnPower vanilla.
/// </summary>
public sealed class NoDrawNextTurnPower : OberonPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _armed;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player || !_armed) return count;
        return 0m;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // Se aplicó en el AfterTurnEnd del jugador; el primer inicio de turno (enemigo) lo arma para
        // que el robo del PRÓXIMO turno propio sea el que se anula (ModifyHandDraw → 0).
        if (side != Owner.Side) _armed = true;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // El robo del turno propio ya pasó (anulado por el bloqueo armado): consumido, se auto-remueve.
        // AfterPlayerTurnStart corre tras el robo (patrón NightReadingPower que roba aquí).
        if (player != Owner.Player || !_armed || Owner.IsDead) return;
        Flash();
        await PowerCmd.Remove(this);
    }
}
