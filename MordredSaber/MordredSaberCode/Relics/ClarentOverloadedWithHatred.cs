using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Clarent Sobrecargada de Odio (充溢憎恶的克拉伦特) — reliquia de JEFE (DESIGN-MORDRED §6),
/// reemplaza a «Clarent, la Espada Robada». Mantiene la entrada en CABALLERO ENMASCARADO y el
/// watcher del motor (<see cref="RedLightningChannelPower"/>), pero DUPLICA ambas conversiones:
///   - cada pérdida de Vida → +20 Estrellas de Crítico (en vez de 10);
///   - cada CRÍTICO LISTO consumido → +20 NP (en vez de 10), vía <see cref="ICritConsumedListener"/>.
/// Mismos caps que la starter (3 procs/turno por conversión): sube el MONTO, no los procs (espejo
/// LordCamelotRestored del arco Hellup de Mash). Si la starter coexiste, ésta la supersede.
/// </summary>
public sealed class ClarentOverloadedWithHatred : MordredRelic, ICritConsumedListener
{
    public const int StarsPerHpLoss = 20;
    public const int NpPerCritConsumed = 20;
    public const int MaxProcsPerTurn = 3;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    private int _starProcsThisTurn;
    private int _npProcsThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Stars", StarsPerHpLoss),
        new DynamicVar("NpCharge", NpPerCritConsumed),
        new DynamicVar("MaxProcs", MaxProcsPerTurn)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        _starProcsThisTurn = 0;
        _npProcsThisTurn = 0;
        // Hace por su cuenta la precarga de forma + el watcher (la starter cede si coexisten).
        await Forms.Enter<MaskedKnightFormPower>(null, Owner.Creature, null);
        await PowerCmd.Apply<RedLightningChannelPower>(Owner.Creature, 1m, Owner.Creature, null, silent: true);
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _starProcsThisTurn = 0;
            _npProcsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    /// <summary>Perder Vida → Estrellas dobladas (máx 3/turno).</summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature != Owner.Creature || delta >= 0) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (_starProcsThisTurn >= MaxProcsPerTurn) return;
        _starProcsThisTurn++;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerHpLoss, null);
    }

    /// <summary>Crítico Listo consumido → Carga NP doblada (máx 3/turno).</summary>
    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (_npProcsThisTurn >= MaxProcsPerTurn) return;
        _npProcsThisTurn++;
        Flash();
        await NpCharge.Gain(Owner.Creature, NpPerCritConsumed, null);
    }
}
