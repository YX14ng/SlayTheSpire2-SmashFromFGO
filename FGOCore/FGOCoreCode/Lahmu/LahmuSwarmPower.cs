using System.Collections.Generic;
using System.Linq;
using FGOCore.FGOCoreCode.Curses;
using CursesHelper = FGOCore.FGOCoreCode.Curses.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Enjambre de Laḫmu (ラフム) — los hijos de Tiamat nacidos del Mar de Vida. El contador es el
/// nº de Laḫmu en campo (tope <see cref="MaxSwarm"/>). Al INICIO del turno enemigo, el enjambre
/// (a) le da a su dueña BLOQUE = <c>nº × 2</c> (la cubren con sus cuerpos; rebalance 2026-08-15:
/// la Crianza ya NO suma al bloqueo — con Crianza alta el ingreso pasivo superaba todo output
/// enemigo, ver docs/REBALANCE-TIAMAT-ARTORIA.md) y
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

    // PowerModel does not inject custom runtime values into its normal description. Route mutable
    // instances through Description so the hover tip can show the exact next Block and bite totals.
    protected override string SmartDescriptionLocKey => $"{Id.Entry}.runtimeDescription";

    public override LocString Description
    {
        get
        {
            if (!IsMutable) return base.Description;

            var nurture = Lahmu.NurtureOf(Owner);
            var biteCount = GetBiteCount();
            var biteDamage = Amount * (BitePerLahmu + nurture);
            var description = new LocString("powers", $"{Id.Entry}.smartDescription");
            description.Add("Nurture", nurture);
            description.Add("Block", Amount * BlockPerLahmu);
            description.Add("BiteDamage", biteDamage);
            description.Add("BiteCount", biteCount);
            description.Add("TotalBiteDamage", biteDamage * biteCount);
            return description;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<CursePower>()];

    // BLOQUE al inicio del turno enemigo (BeforeSideTurnStart: los cuerpos cubren a la dueña antes de
    // los ataques). Acá NO se daña a nadie — el daño en este borde colgaba el turno al matar.
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side || Owner.IsDead || Amount <= 0) return;

        Flash();
        await CreatureCmd.GainBlock(Owner, Amount * BlockPerLahmu, ValueProp.Unpowered, null);
    }

    // MORDIDA al final de TU turno (BeforeSideTurnEnd) — 2026-07-04: antes mordía al inicio del turno
    // enemigo, pero una MUERTE en un borde de turno sin precedente vanilla deja colgados a los enemigos
    // restantes (reportes: "matar uno → no se puede operar; matar todos a la vez → pasa"). El ÚNICO
    // patrón vanilla de daño-a-enemigos en borde de turno es BeforeSideTurnEnd del lado propio con el
    // choiceContext del hook (Hailstorm/TheBomb) — este es ese patrón. La ventana efectiva es la misma
    // (entre tu último input y los ataques enemigos): mismo objetivo (más maldito), mismo daño.
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.IsDead || Amount <= 0) return;
        if (Owner.CombatState is not CombatState combatState) return;

        var nurture = Lahmu.NurtureOf(Owner);
        Flash();

        // La forma Bestia muerde DOS veces (ISwarmBiteAmplifier.ExtraBites=1). Se re-resuelve el
        // objetivo en cada mordida porque el más maldito puede morir entre golpes.
        var bites = GetBiteCount();
        for (var i = 0; i < bites; i++)
        {
            var target = CursesHelper.MostCursed(combatState, Owner)
                         ?? combatState.GetOpponentsOf(Owner).FirstOrDefault(e => !e.IsDead);
            if (target == null || target.IsDead) break;
            await CreatureCmd.Damage(choiceContext, target,
                Amount * (BitePerLahmu + nurture), ValueProp.Unpowered, Owner);
        }
    }

    private int GetBiteCount() =>
        1 + Owner.GetPowerInstances<PowerModel>().OfType<ISwarmBiteAmplifier>().Sum(a => a.ExtraBites);
}
