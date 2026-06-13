using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Tiamat.TiamatCode.Cards.Rare;

/// <summary>Enūma Eliš (carta-NP) — consume TODA la Carga NP: revive la cría hasta el tope del
/// enjambre y muerde a TODOS con ella por Laḫmu × Crianza (+ overcharge). El estallido de la madre.
/// Guiño de fidelidad: Enūma Eliš es el arma que la MATÓ (de Kingu), no su NP — acá es su clímax.
/// Balance: gateada por carga completa (recurso de toda la run) + AoE; pico objetivo ~180-220 con
/// enjambre lleno y Crianza alta, nunca 300+ (tunable por PerLahmu/PerNurture/PerTen).</summary>
public sealed class EnumaElis() : TiamatCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public const int ChargeCost = 70;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 2),      // overcharge: +2 daño por cada 10 de carga sobre el minimo
        new DynamicVar("PerLahmu", 4),    // mordida: +4 por Laḫmu
        new DynamicVar("PerNurture", 2)   // mordida: +2 por Laḫmu por cada Crianza
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<LahmuSwarmPower>(),
        HoverTipFactory.FromPower<LahmuNurturePower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1) Consume TODA la carga; tier = lo realmente consumido (>= ChargeCost).
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        // 2) Revive la cría caída: parí hasta el tope del enjambre.
        await Lahmu.Spawn(Owner.Creature, LahmuSwarmPower.MaxSwarm, this);
        // 3) Mordida del enjambre (ya reabastecido) por Laḫmu × (PerLahmu + PerNurture × Crianza).
        var bite = Lahmu.Count(Owner.Creature)
                   * (DynamicVars["PerLahmu"].IntValue + DynamicVars["PerNurture"].IntValue * Lahmu.NurtureOf(Owner.Creature));
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + bite);
        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerLahmu"].UpgradeValueBy(2m);
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
