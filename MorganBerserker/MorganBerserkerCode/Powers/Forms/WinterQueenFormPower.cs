using System.Collections.Generic;
using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Reina del Invierno (冬之女王) — forma clímax PERMANENTE: ambas a la vez sin penalidad
/// (rediseño 2026-06-15: swap Estrellas→Maldición). SIEMBRA como la Bruja Y DETONA como la
/// Reina, sin el -2 de daño. Es la meta aspiracional del mazo.
/// (a) Mientras estés en esta forma, tu Maldición NO decae (ICursePreserver).
/// (b) Tus cartas que aplican Maldición aplican +1 (ICurseAmplifier).
/// (c) Al inicio de tu turno: aplica +2 de Maldición a TODOS los enemigos.
/// (d) "Sentencia": tus Ataques infligen daño extra = la Maldición del objetivo y la consumen.
/// (e) Primera vez que dañás HP enemigo cada turno: +10 NP. Sin penalización de Ataque.
///
/// La Sentencia replica la implementación de FairyQueenFormPower (caché por carta, single-hit,
/// anti-doble-dip). No hereda de esa clase para no arrastrar la penalidad/firma del Berserker base.
/// </summary>
public sealed class WinterQueenFormPower : MorganFormPower, ICursePreserver, ICurseAmplifier
{
    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_winter.tres";

    public override bool IsPermanent => true;

    public int ExtraCurse => 1;

    private bool _npThisTurn;
    private int _pendingSentence;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.AfterSideTurnStart(side, participants, combatState);
        if (side == CombatSide.Player)
        {
            _npThisTurn = false;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner).Where(e => !e.IsDead).ToList())
        {
            await Curses.Apply(enemy, RainWitchFormPower.SpreadPerTurn, null, null);
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _pendingSentence = 0;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Target is not { } target || target == Owner || target.IsDead) return;

        var curse = Curses.Of(target);
        if (curse <= 0) return;

        _pendingSentence = curse;
        Flash();
        await Curses.Consume(target, curse);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack() || _pendingSentence <= 0) return 0m;
        var bonus = _pendingSentence;
        _pendingSentence = 0;
        return bonus;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            _pendingSentence = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, FairyQueenFormPower.NpOnDamage, null);
    }
}
