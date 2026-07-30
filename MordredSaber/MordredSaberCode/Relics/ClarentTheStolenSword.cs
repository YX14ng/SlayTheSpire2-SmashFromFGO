using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Clarent, la Espada Robada (克拉伦特·被窃之剑) — reliquia STARTER y motor del personaje
/// (lección 焰刑地狱: convierte eventos universales en recursos del kit, DESIGN-MORDRED §6).
///   (1) al iniciar cada combate entrás en forma CABALLERO ENMASCARADO (dispara la precarga de
///       FormVisuals) y se aplica el watcher invisible <see cref="RedLightningChannelPower"/>;
///   (2) cada pérdida de Vida → +10 Estrellas de Crítico (sangrar carga el relámpago);
///   (3) cada CRÍTICO LISTO consumido → +10 NP (el odio del golpe canaliza a Clarent), vía
///       <see cref="ICritConsumedListener"/>.
/// Calibra TODOS los riders de ★. Parche anti multi-hit: cada conversión proca máx 3 veces por
/// turno (reset al inicio de tu turno), espejo de RoundTableFragment.
/// </summary>
public sealed class ClarentTheStolenSword : MordredRelic, ICritConsumedListener
{
    /// <summary>Clarent Sobrecargada de Odio (boss relic) REEMPLAZA este motor con numeros al doble
    /// (audit 2026-07-05 HIGH: coexistiendo se APILABAN las conversiones — hasta 90 estrellas/turno).
    /// Mientras este presente, la starter calla (sus handlers y su setup de combate).</summary>
    private bool ReplacedByOverloaded()
    {
        foreach (var r in Owner.Relics)
        {
            if (r is ClarentOverloadedWithHatred) return true;
        }
        return false;
    }

    public const int StarsPerHpLoss = 10;
    public const int NpPerCritConsumed = 10;
    public const int MaxProcsPerTurn = 3;

    public override RelicRarity Rarity => RelicRarity.Starter;

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
        if (ReplacedByOverloaded()) return; // la Sobrecargada hace su propio setup (forma + watcher + Chispa)
        // Forma inicial: Enmascarado (source == null → no cuenta como "cambio de forma").
        await Forms.Enter<MaskedKnightFormPower>(null, Owner.Creature, null);
        // Watcher del motor ★→×2→NP (la pieza que detecta el consumo de Crítico Listo).
        await PowerCmd.Apply<RedLightningChannelPower>(new BlockingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, null, silent: true);
        // Motor «Chispa de Clarent»: al consumir un Crítico (cap 1/turno) manifiesta el token efímero
        // AoE que escala con la forma — el Crítico de Mordred EXPRESA su identidad, no solo el ×2.
        await PowerCmd.Apply<RedLightningSparkPower>(new BlockingPlayerChoiceContext(), Owner.Creature, 1m, Owner.Creature, null, silent: true);
    }

    /// <summary>Perder Vida → Estrellas (cualquier fuente; máx 3/turno).</summary>
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (ReplacedByOverloaded()) return;
        if (creature != Owner.Creature || delta >= 0) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 6, 2) >= MaxProcsPerTurn) return;
        var context = new BlockingPlayerChoiceContext();
        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 6, MaxProcsPerTurn, null, width: 2);
        Flash();
        await CritStars.Gain(context, Owner.Creature, StarsPerHpLoss, null);
    }

    /// <summary>Crítico Listo consumido → Carga NP (máx 3/turno).</summary>
    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        if (ReplacedByOverloaded()) return;
        if (!CombatManager.Instance.IsInProgress) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 8, 2) >= MaxProcsPerTurn) return;
        var context = choiceContext ?? new BlockingPlayerChoiceContext();
        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 8, MaxProcsPerTurn, null, width: 2);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, NpPerCritConsumed, null);
    }
}
