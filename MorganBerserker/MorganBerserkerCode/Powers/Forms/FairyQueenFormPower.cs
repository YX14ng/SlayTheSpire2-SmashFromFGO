using System.Collections.Generic;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Cards;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// La Reina (Berserker, 妖精女王) — forma inicial. La DETONADORA del motor de Maldición
/// (rediseño 2026-06-15: swap Estrellas→Maldición). "Sentencia": tus Ataques infligen daño
/// adicional igual a la Maldición del objetivo y la CONSUMEN (cosechás lo que la Bruja sembró).
/// Genera poca Maldición propia → te empuja a alternar con Caster para re-sembrar. La primera
/// vez que dañás HP enemigo cada turno: +10 NP.
///
/// El bono se calcula y la Maldición se consume UNA sola vez por carta en BeforeCardPlayed (camino
/// REAL de juego; las previews no lo invocan). _pendingSentence se devuelve en cada golpe del Ataque
/// vía ModifyDamageAdditive (PURO, sin mutar — ese hook corre también en preview y NO recibe el
/// previewMode) y se limpia en AfterDamageReceived tras la primera pegada REAL, así un multi-hit no
/// lo repite (anti doble-dip) y ninguna preview se come el bono antes de la pegada.
/// </summary>
public sealed class FairyQueenFormPower : MorganFormPower
{
    public const int NpOnDamage = 10;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_queen.tres";

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

    /// <summary>
    /// Sentencia: al jugar un Ataque dirigido, lee la Maldición del objetivo, la cachea como bono
    /// y la CONSUME UNA vez (antes de resolver los golpes). Así un Ataque multi-hit no consume ni
    /// suma la Maldición por golpe (era double-dip). El bono ya consumido se aplica al PRIMER golpe.
    /// </summary>
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        _pendingSentence = 0;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;
        // Cartas que consumen la Maldición del objetivo por sí mismas (FinalCollection): NO les robes
        // la Maldición acá, o su consumo da 0 y no hacen daño (reporte de player). La carta la usa.
        if (cardPlay.Card is IUsesTargetCurse) return;
        if (cardPlay.Target is not { } target || target == Owner || target.IsDead) return;

        var curse = Curses.Of(target);
        if (curse <= 0) return;

        _pendingSentence = curse;
        Flash();
        await Curses.Consume(target, curse);
    }

    // Devuelve el bono cacheado en el golpe del Ataque. PURO (NO muta): el hook ModifyDamage corre
    // también en PREVIEW y NO recibe el previewMode (Hook.ModifyDamage lo tiene pero no lo reenvía al
    // hook por-power), así que mutar acá hacía que una preview consumiera el bono antes de la pegada
    // REAL → la Maldición se consumía pero el daño extra no se aplicaba (bug reportado). El bono se
    // limpia en AfterDamageReceived (que NO corre en preview) tras la primera pegada real.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack() || _pendingSentence <= 0) return 0m;
        return _pendingSentence;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // Safety net: si el Ataque no resolvió ningún golpe (fizzle), limpiar el bono cacheado.
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner?.Creature == Owner)
        {
            _pendingSentence = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        // La Sentencia ya se sumó a ESTA pegada (vía ModifyDamageAdditive): limpiala acá —en el camino
        // real, nunca en preview— para que los golpes restantes de un multi-hit no la repitan. Se limpia
        // aunque la pegada haya sido bloqueada (el bono igual se aplicó al cálculo de ese golpe).
        if (dealer == Owner && !target.IsPlayer && props.IsPoweredAttack() && _pendingSentence > 0)
        {
            _pendingSentence = 0;
        }

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, NpOnDamage, null);
    }
}
