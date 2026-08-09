using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// El Contrato de Sueños (梦境契约 / The Dream Contract) — reliquia STARTER (el motor de Oberon,
/// DESIGN-OBERON §7 #1). Dos efectos:
/// (1) Al iniciar cada combate entrás en EL REY DEL CUENTO (FormSwitch.Enter en BeforeCombatStartLate,
///     source=null → fija la forma inicial + dispara la precarga de FormVisuals; gotcha Morgan v2).
/// (2) Cada punto de Deuda que PAGÁS con NP al final del turno: +<see cref="StarsPerDebtPaid"/>
///     Estrellas (máx <see cref="MaxProcsPerTurn"/> procs/turno) — la 2ª economía del kit, ahora
///     factorizada en <see cref="DebtPaidStarsRelic"/> (compartida con el jefe).
/// </summary>
public sealed class DreamContract : DebtPaidStarsRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<BookOfDreamsEnd>();

    protected override int StarsPerDebtPaid => 10;

    protected override int MaxProcsPerTurn => 3;

    protected override bool IsActive => Owner.Relics.All(relic => relic is not BookOfDreamsEnd);

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await FormSwitch.Enter<StorybookKingPower>(null, Owner.Creature, null);
    }
}
