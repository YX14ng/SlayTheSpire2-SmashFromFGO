using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Cleanse;
using FGOCore.FGOCoreCode.Ritsu;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>
/// Estrellas de Crítico (Critical Stars / 暴击星) — recurso compartido FGO, patrón
/// JeanneAlter: al llegar a <see cref="Threshold"/> (100) se descuentan 100 y otorgan 1 de
/// <see cref="CritReadyPower"/> (próximo Ataque ×2) — auto-payoff. El gasto MANUAL
/// (keyword "Crítico", 50★) coexiste con el auto-proc: las cartas gastan vía
/// <see cref="CritStars.Gain"/> con monto negativo. (El "banco" IBanksCritStars que
/// desactivaba el auto-proc se eliminó en el audit 2026-07-04: Morgan lo abandonó con el
/// swap Estrellas→Maldición y no quedaba ningún implementador.)
/// NO confundir con el contador chico con candado de forma de ArtoriaCaster (mod-local).
/// </summary>
public sealed class CritStarsPower : FGOCorePower, IResourcePower
{
    /// <summary>Umbral del auto-proc (★ → próximo Ataque ×2) para quien NO banca.</summary>
    public const int Threshold = 100;

    /// <summary>Máximo del banco global. Threshold se conserva como alias compatible.</summary>
    public const int Max = 100;

    /// <summary>Costo estándar de la keyword "Crítico" (gasto manual) en una carta.</summary>
    public const int CritCost = 50;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    // El medidor visible lo aporta RitsuLib. El power conserva el ID y el estado de saves viejos.
    protected override bool IsVisibleInternal => false;

    private bool _isClamping;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (power != this || _isClamping) return;

        // Auto-proc por defecto (Mash): una sola carta puede cruzar 200+ (ej. +100 de golpe).
        if (Amount > Max)
        {
            _isClamping = true;
            await PowerCmd.ModifyAmount(choiceContext, this, Max - Amount, Owner, cardSource);
            _isClamping = false;
        }

        await Criticals.EnsureInstalled(choiceContext, Owner);
    }
}

/// <summary>Helper de la economía de estrellas (espejo de NpCharge.Gain).</summary>
public static class CritStars
{
    public static int Of(Creature creature) => (int)creature.GetPowerAmount<CritStarsPower>();

    public static Task Gain(Creature creature, int amount, CardModel? source) =>
        Gain(new BlockingPlayerChoiceContext(), creature, amount, source);

    /// <summary>Context-preserving star change for card and hook resolution paths.</summary>
    public static async Task Gain(
        PlayerChoiceContext choiceContext, Creature creature, int amount, CardModel? source)
    {
        if (amount == 0) return;
        if (amount > 0)
        {
            var room = CritStarsPower.Max - Of(creature);
            if (room <= 0) return;
            await PowerCmd.Apply<CritStarsPower>(choiceContext, creature, Math.Min(amount, room), creature, source);
            await FgoSecondaryResources.SynchronizeStarsFromLegacy(creature, source);
            return;
        }
        var power = creature.GetPower<CritStarsPower>();
        if (power == null) return;
        await PowerCmd.ModifyAmount(choiceContext, power, Math.Max(amount, -power.Amount), creature, source);
        await FgoSecondaryResources.SynchronizeStarsFromLegacy(creature, source);
    }

    /// <summary>¿Puede pagar un coste de estrellas (conversores estilo 等价交换)?</summary>
    public static bool CanPay(Creature creature, int cost) => cost >= 0 && Of(creature) >= cost;

    /// <summary>Gasta exactamente el coste pedido. Devuelve false sin mutar si no alcanza.</summary>
    public static async Task<bool> Spend(
        PlayerChoiceContext choiceContext, Creature creature, int cost, CardModel? source)
    {
        if (cost < 0) return false;
        if (cost == 0) return true;
        if (!CanPay(creature, cost)) return false;
        var power = creature.GetPower<CritStarsPower>();
        if (power == null) return false;
        await PowerCmd.ModifyAmount(choiceContext, power, -cost, creature, source);
        await FgoSecondaryResources.SynchronizeStarsFromLegacy(creature, source);
        return true;
    }
}
