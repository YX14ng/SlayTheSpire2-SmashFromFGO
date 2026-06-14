using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Reloj del Alba (黎明之钟 / Clock of Dawn) — reliquia RARA (DESIGN-OBERON §7 #10): al iniciar
/// combates contra Elite o Jefe, +40 Carga NP y Deuda 1 (el doping matutino llega solo, con la cuenta
/// del amanecer adjunta). Honesta en hordas — no proca ahi. Deteccion del encuentro via
/// CombatState.Encounter.RoomType (patron FirstUnitBadge de Okita), leida en BeforeCombatStartLate.
/// </summary>
public sealed class ClockOfDawn : OberonRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int Charge = 40;
    private const int Debt = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DebtPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        if (Owner.Creature.CombatState?.Encounter?.RoomType is not (RoomType.Elite or RoomType.Boss)) return;
        Flash();
        await NpCharge.Gain(Owner.Creature, Charge, null);
        await DebtPower.Add(Owner.Creature, Debt, Owner.Creature, null);
    }
}
