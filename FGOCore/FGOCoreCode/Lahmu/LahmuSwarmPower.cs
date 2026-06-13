using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using CursesHelper = FGOCore.FGOCoreCode.Curses.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Enjambre de Laḫmu (ラフム) — los hijos de Tiamat nacidos del Mar de Vida. El contador es el
/// nº de Laḫmu en campo (tope <see cref="MaxSwarm"/>). Al INICIO del turno enemigo, el enjambre
/// (a) le da a su dueña BLOQUE = <c>nº × (2 + Crianza)</c> (la cubren con sus cuerpos) y
/// (b) MUERDE al enemigo más maldito por <c>nº × (1 + Crianza)</c> — la Marea de Caos
/// (<see cref="CursePower"/>) dirige a quién muerden. La Crianza vive en
/// <see cref="LahmuNurturePower"/>. Usar el helper <see cref="Lahmu"/> para parir/alimentar/devorar.
/// </summary>
public sealed class LahmuSwarmPower : FGOCorePower
{
    public const int MaxSwarm = 6;
    public const int BlockPerLahmu = 2;
    public const int BitePerLahmu = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<CursePower>()];

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // Actúa al INICIO del turno enemigo (no el del jugador): el bloque protege durante
        // los ataques enemigos y la mordida castiga. (Patrón espejo de CursePower, invertido:
        // CursePower está en el enemigo y actúa en su propio turno; el enjambre está en la
        // jugadora y actúa en el turno del OTRO lado.)
        if (side == Owner.Side || Owner.IsDead || Amount <= 0) return;

        var nurture = Lahmu.NurtureOf(Owner);
        Flash();

        await CreatureCmd.GainBlock(Owner, Amount * (BlockPerLahmu + nurture), ValueProp.Unpowered, null);

        // La forma Bestia muerde DOS veces (ISwarmBiteAmplifier.ExtraBites=1). Se re-resuelve el
        // objetivo en cada mordida porque el más maldito puede morir entre golpes.
        var bites = 1 + Owner.GetPowerInstances<PowerModel>().OfType<ISwarmBiteAmplifier>().Sum(a => a.ExtraBites);
        for (var i = 0; i < bites; i++)
        {
            var target = CursesHelper.MostCursed(combatState, Owner)
                         ?? combatState.GetOpponentsOf(Owner).FirstOrDefault(e => !e.IsDead);
            if (target == null || target.IsDead) break;
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target,
                Amount * (BitePerLahmu + nurture), ValueProp.Unpowered, Owner, null);
        }
    }
}
