using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Cleanse;

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

    /// <summary>Costo estándar de la keyword "Crítico" (gasto manual) en una carta.</summary>
    public const int CritCost = 50;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _isProcessing;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (power != this || _isProcessing) return;

        // Auto-proc por defecto (Mash): una sola carta puede cruzar 200+ (ej. +100 de golpe).
        while (Amount >= Threshold)
        {
            _isProcessing = true;
            Flash();
            await PowerCmd.ModifyAmount(choiceContext, this, -Threshold, Owner, null);
            await PowerCmd.Apply<CritReadyPower>(choiceContext, Owner, 1m, Owner, null);
            _isProcessing = false;
        }
    }
}

/// <summary>Helper de la economía de estrellas (espejo de NpCharge.Gain).</summary>
public static class CritStars
{
    public static int Of(Creature creature) => (int)creature.GetPowerAmount<CritStarsPower>();

    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        if (amount == 0) return;
        if (amount > 0)
        {
            await PowerCmd.Apply<CritStarsPower>(new BlockingPlayerChoiceContext(), creature, amount, creature, source);
            return;
        }
        var power = creature.GetPowerInstances<CritStarsPower>().FirstOrDefault();
        if (power == null) return;
        await PowerCmd.ModifyAmount(new BlockingPlayerChoiceContext(), power, Math.Max(amount, -power.Amount), creature, source);
    }

    /// <summary>¿Puede pagar un coste de estrellas (conversores estilo 等价交换)?</summary>
    public static bool CanPay(Creature creature, int cost) => Of(creature) >= cost;
}
