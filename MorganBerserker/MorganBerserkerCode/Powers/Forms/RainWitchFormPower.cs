using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Bruja de la Lluvia (Caster, 雨之魔女·梣) — Aesc, el pasado de Morgan. La SEMBRADORA del
/// motor de Maldición (rediseño 2026-06-15: swap Estrellas→Maldición). Es donde APILÁS la
/// bomba para después cosecharla en Berserker. Recurso cruzado = Maldición (Caster siembra /
/// Berserker detona). Patrón calcado de TiamatBeastPower (ICurseAmplifier + spread propio).
/// (a) Mientras estés en esta forma, tu Maldición NO decae (ICursePreserver) — la lluvia la
///     mantiene fresca, así la bomba crece turno a turno.
/// (b) Tus cartas que aplican Maldición aplican +1 (ICurseAmplifier).
/// (c) Al inicio de tu turno: aplica +2 de Maldición a TODOS los enemigos (la lluvia maldita cae).
/// (d) Tus Ataques hacen -2 daño (floja pegando; su rol es sembrar, no golpear).
/// </summary>
public sealed class RainWitchFormPower : MorganFormPower, ICursePreserver, ICurseAmplifier
{
    public const int SpreadPerTurn = 2;
    public const int AttackPenalty = 2;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_aesc.tres";

    public int ExtraCurse => 1;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        // Spread propio de la pasiva: applier=null para NO auto-amplificarse (este power es
        // ICurseAmplifier y en Owner, así que pasar Owner doblaría el +1). Patrón TiamatBeastPower.
        foreach (var enemy in Owner.CombatState.GetOpponentsOf(Owner).Where(e => !e.IsDead).ToList())
        {
            await Curses.Apply(enemy, SpreadPerTurn, null, null);
        }
    }

    // ModifyDamageAdditive es DELTA (default 0): devolver -2 para la penalidad.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return -AttackPenalty;
    }
}
