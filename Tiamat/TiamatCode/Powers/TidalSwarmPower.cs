using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using CursesHelper = FGOCore.FGOCoreCode.Curses.Curses;
using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Marea Voraz — el LOOP OFENSIVO de la forma Lily (DESIGN-REVIEW-2 §3). El enjambre de FGOCore solo
/// muerde al INICIO del turno enemigo (1 mordida/ronda en Lily → daño casi nulo, "sala de espera").
/// Este power Tiamat-LOCAL hace que el enjambre muerda TAMBIÉN al final de CADA turno tuyo:
/// <c>nº Laḫmu × (1 + Crianza)</c> al enemigo más maldito, por acumulación (Amount = mordidas extra).
///
/// Espeja la mordida de <see cref="LahmuSwarmPower"/> (misma fórmula <c>BitePerLahmu=1 + Crianza</c>,
/// mismo imán <see cref="CursesHelper.MostCursed"/>) pero SIN tocar FGOCore (no cambia el knob
/// global <c>BitePerLahmu</c> 1→2; mantiene el enjambre del Mash intacto). Es la mordida de Lily; en
/// la forma Bestia el doble-mordida ya vive en <c>TiamatBeastPower</c> (ISwarmBiteAmplifier), así
/// que esto SUMA daño en ambas formas — pero su razón de ser es darle dientes a la fase Lily.
/// </summary>
public sealed class TidalSwarmPower : TiamatPower
{
    public const int BitePerLahmu = 1; // espeja LahmuSwarmPower.BitePerLahmu

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<CursePower>()];

    /// <summary>Al FINAL de TU turno, el enjambre muerde de nuevo (Amount mordidas extra) al más
    /// maldito. Se re-resuelve el objetivo en cada mordida (puede morir entre golpes).</summary>
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        var swarm = Lahmu.Count(Owner);
        if (swarm <= 0 || Amount <= 0) return;

        var nurture = Lahmu.NurtureOf(Owner);
        var perBite = swarm * (BitePerLahmu + nurture);
        if (perBite <= 0) return;

        Flash();
        for (var i = 0; i < Amount; i++)
        {
            var target = CursesHelper.MostCursed((CombatState)Owner.CombatState, Owner)
                         ?? ((CombatState)Owner.CombatState).GetOpponentsOf(Owner).FirstOrDefault(e => !e.IsDead);
            if (target == null || target.IsDead) break;
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, perBite, ValueProp.Unpowered, Owner, null);
        }
    }
}
