using System.Collections.Generic;
using System.Linq;
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
/// Reina del Invierno (冬之女王) — forma clímax PERMANENTE: ambas a la vez sin penalidad
/// (rediseño 2026-06-15: swap Estrellas→Maldición). SIEMBRA como la Bruja Y DETONA como la
/// Reina, sin el -2 de daño. Es la meta aspiracional del mazo.
/// (a) Mientras estés en esta forma, tu Maldición NO decae (ICursePreserver).
/// (b) Tus cartas que aplican Maldición aplican +1 (ICurseAmplifier).
/// (c) Al inicio de tu turno: aplica +2 de Maldición a TODOS los enemigos.
/// (d) "Sentencia": tus Ataques infligen daño extra = la Maldición del objetivo y consumen solo la
///     MITAD (redondeo arriba) — el privilegio del clímax permanente, 2026-08-16, feedback de
///     Sac2Loo2Sac: es irreversible y se vende como «lo mejor de ambas», así que retiene campo.
///     No consumir NADA quedó descartado: convertiría la Maldición en un multiplicador sin techo.
///     La regla vive en <see cref="Sentencia.Detonar"/>, que es el único lugar que consume.
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

    private int _pendingSentence;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        if (Owner.CombatState is not { } combatState) return;
        Flash();
        foreach (var enemy in combatState.GetOpponentsOf(Owner).Where(e => !e.IsDead).ToList())
        {
            await Curses.Apply(choiceContext, enemy, RainWitchFormPower.SpreadPerTurn, null, null);
        }
    }

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

    // PURO (no muta): ver FairyQueenFormPower — el hook corre también en preview y NO recibe el
    // previewMode, así que el bono se limpia en AfterDamageGiven (real) y no en una preview.
    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack() || _pendingSentence <= 0) return 0m;
        return _pendingSentence;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
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
        await NpCharge.Gain(choiceContext, Owner, FairyQueenFormPower.NpOnDamage, null);
    }
}
