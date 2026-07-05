using FGOCore.FGOCoreCode.Cleanse;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TiamatBeast.TiamatCode.Cards;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Pluma de la Bestia — la carta-NP de CIERRE de la ventana Bestia (<see cref="IBeastEphemeral"/>,
/// Exhaust). Apertura y cierre viven en cartas distintas para NO sumar pico el mismo turno: la
/// apertura (Nammu Dur-an-ki, daño FIJO) abre la ventana; ESTA consume la Carga NP recargada DENTRO
/// de la ventana (ConsumeAll) y descarga la cosecha. Limpia TODOS tus debuffs, después muerde a
/// TODOS por (Laḫmu × Crianza × PerNurture) + (Maldición del más maldito × PerCurse) — gateado por
/// lo que montaste, jamás por un ×plano. Cap duro (<see cref="DamageCap"/>) para que el pico no
/// supere ~180-220. Tras disparar, CIERRA la ventana (revierte a Lily y purga el mazo efímero).
/// </summary>
public sealed class PlumaDeLaBestia() : TiamatCard(1, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IBeastEphemeral
{
    public const int ChargeCost = 1;

    /// <summary>Tope duro del pico de cierre (riesgo #3 de REDESIGN-TIAMAT: pico &gt; ~220).</summary>
    public const int DamageCap = 200;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerNurture", 3), // mordida: Laḫmu × Crianza × este
        new DynamicVar("PerCurse", 4),   // mordida: + Maldición del más maldito × este
        new DynamicVar("PerTen", 1)      // overcharge: +1 por cada 10 de carga sobre el mínimo
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<LahmuSwarmPower>(),
        HoverTipFactory.FromPower<LahmuNurturePower>(),
        HoverTipFactory.FromPower<CursePower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1) Consume TODA la carga recargada dentro de la ventana; tier = lo realmente consumido.
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        // 2) Limpia TODOS tus debuffs (el respiro del cierre; preserva recursos y la forma).
        await Cleanse.RemoveDebuffs(Owner.Creature);

        // 3) Mordida: (Laḫmu × Crianza × PerNurture) + (Maldición del más maldito × PerCurse).
        var mostCursed = Curses.MostCursed((CombatState)Owner.Creature.CombatState, Owner.Creature);
        var topCurse = mostCursed != null ? Curses.Of(mostCursed) : 0;
        var bite = Lahmu.Count(Owner.Creature) * Lahmu.NurtureOf(Owner.Creature) * DynamicVars["PerNurture"].IntValue
                   + topCurse * DynamicVars["PerCurse"].IntValue;

        var raw = DynamicVars.Damage.BaseValue + overcharge + bite;
        var damage = NpLevels.Scale(Owner, Math.Min(raw, DamageCap));
        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        // 4) Cierra la ventana: el clímax acaba la EJECUCIÓN temporal — revierte a Lily y purga el
        //    mazo efímero (idempotente; el Counter→0 haría lo mismo, esto adelanta el cierre).
        if (Owner.Creature.GetPower<TiamatBeastWindowPower>() is { } window)
        {
            await window.CloseWindow(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerNurture"].UpgradeValueBy(1m);
        DynamicVars["PerCurse"].UpgradeValueBy(1m);
    }
}
