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
/// previewMode) y se limpia en AfterDamageGiven tras la primera pegada REAL, así un multi-hit no
/// lo repite (anti doble-dip) y ninguna preview se come el bono antes de la pegada.
/// </summary>
public sealed class FairyQueenFormPower : MorganFormPower
{
    public const int NpOnDamage = 10;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_queen.tres";

    private int _pendingSentence;

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

        // Detonar (helper compartido): con Cernunnos re-especificada consume solo la mitad;
        // el bono de daño sigue siendo la Maldición completa (REDESIGN-MORGAN-V2 §3.2/M5).
        var bonus = await Sentencia.Detonar(Owner, target);
        if (bonus <= 0) return;

        _pendingSentence = bonus;
        Flash();
    }

    // Devuelve el bono cacheado en el golpe del Ataque. PURO (NO muta): el hook ModifyDamage corre
    // también en PREVIEW y NO recibe el previewMode (Hook.ModifyDamage lo tiene pero no lo reenvía al
    // hook por-power), así que mutar acá hacía que una preview consumiera el bono antes de la pegada
    // REAL → la Maldición se consumía pero el daño extra no se aplicaba (bug reportado). El bono se
    // limpia en AfterDamageGiven (que NO corre en preview) tras la primera pegada real.
    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
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

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        // Este callback también corre para golpes letales, a diferencia de AfterDamageReceived.
        if (dealer == Owner && !target.IsPlayer && props.IsPoweredAttack() && _pendingSentence > 0)
        {
            // Flotante «¡Sentencia! +X» sobre la pegada real (nunca en preview) — legibilidad
            // obligatoria del panel (J1-14/J2-4/J3-10).
            Sentencia.ShowFloat(target, _pendingSentence);
            _pendingSentence = 0;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (FgoCombatState.GetTurn(Owner, 3) != 0 || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        await FgoCombatState.SetTurn(choiceContext, Owner, 3, 1, cardSource);
        Flash();
        await NpCharge.Gain(choiceContext, Owner, NpOnDamage, null);
    }
}
