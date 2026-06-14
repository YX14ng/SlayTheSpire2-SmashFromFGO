using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// El Contrato de Sueños (梦境契约 / The Dream Contract) — reliquia STARTER (el motor de Oberon,
/// DESIGN-OBERON §7 #1). Dos efectos:
/// (1) Al iniciar cada combate entrás en EL REY DEL CUENTO (FormSwitch.Enter en BeforeCombatStartLate,
///     source=null → fija la forma inicial + dispara la precarga de FormVisuals; gotcha Morgan v2).
/// (2) Cada punto de Deuda que PAGÁS con NP al final del turno: +<see cref="StarsPerDebtPaid"/>
///     Estrellas (máx <see cref="MaxProcsPerTurn"/> procs/turno, reset AfterSideTurnStart). Convierte
///     el impuesto del kit en la 2ª economía (IDebtPaidRelicListener).
/// </summary>
public sealed class DreamContract : OberonRelic, IDebtPaidRelicListener
{
    public const int StarsPerDebtPaid = 10;
    public const int MaxProcsPerTurn = 3;

    private int _procsThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DebtPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await FormSwitch.Enter<StorybookKingPower>(null, Owner.Creature, null);
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _procsThisTurn = 0;
        return Task.CompletedTask;
    }

    public async Task OnDebtPaid(PlayerChoiceContext? choiceContext, int amountPaid)
    {
        var procs = Math.Min(amountPaid, MaxProcsPerTurn - _procsThisTurn);
        if (procs <= 0) return;
        _procsThisTurn += procs;
        Flash();
        await CritStars.Gain(Owner.Creature, procs * StarsPerDebtPaid, null);
    }
}
