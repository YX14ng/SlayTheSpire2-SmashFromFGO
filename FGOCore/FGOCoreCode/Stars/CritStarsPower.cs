using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>
/// Estrellas de Crítico (Critical Stars / 暴击星) — recurso compartido FGO, patrón
/// JeanneAlter: por defecto, al llegar a <see cref="Threshold"/> (100) se descuentan 100
/// y otorgan 1 de <see cref="CritReadyPower"/> (próximo Ataque ×2) — auto-payoff (Mash).
/// PERO si el owner tiene un power con <see cref="IBanksCritStars"/> (Morgan, vía sus
/// formas), las Estrellas son un BANCO de gasto MANUAL (sin auto-proc): las cartas con
/// "Crítico" las gastan (50★) para aplicar CritReady — la decisión es del jugador.
/// NO confundir con el contador chico con candado de forma de ArtoriaCaster (mod-local).
/// </summary>
public sealed class CritStarsPower : FGOCorePower
{
    /// <summary>Umbral del auto-proc (★ → próximo Ataque ×2) para quien NO banca.</summary>
    public const int Threshold = 100;

    /// <summary>Costo estándar de la keyword "Crítico" (gasto manual) en una carta.</summary>
    public const int CritCost = 50;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _isProcessing;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        if (power != this || _isProcessing) return;

        // Banco manual (Morgan): sin auto-proc — el gasto va por la keyword "Crítico".
        foreach (var p in Owner.GetPowerInstances<PowerModel>())
        {
            if (p is IBanksCritStars) return;
        }

        // Auto-proc por defecto (Mash): una sola carta puede cruzar 200+ (ej. +100 de golpe).
        while (Amount >= Threshold)
        {
            _isProcessing = true;
            Flash();
            await PowerCmd.ModifyAmount(this, -Threshold, Owner, null);
            await PowerCmd.Apply<CritReadyPower>(Owner, 1m, Owner, null);
            _isProcessing = false;
        }
    }
}

/// <summary>
/// Marker en un power del owner: trata las Estrellas de Crítico como un BANCO de gasto
/// manual (NO se auto-consumen a 100). Morgan lo usa vía su <c>MorganFormPower</c> base
/// (siempre está en una forma) para que el banco sea su decisión de "Crítico". El resto
/// (Mash) NO lo tiene y conserva el auto-payoff a 100.
/// </summary>
public interface IBanksCritStars;

/// <summary>Helper de la economía de estrellas (espejo de NpCharge.Gain).</summary>
public static class CritStars
{
    public static int Of(Creature creature) => (int)creature.GetPowerAmount<CritStarsPower>();

    public static async Task Gain(Creature creature, int amount, CardModel? source)
    {
        if (amount == 0) return;
        if (amount > 0)
        {
            await PowerCmd.Apply<CritStarsPower>(creature, amount, creature, source);
            return;
        }
        var power = creature.GetPowerInstances<CritStarsPower>().FirstOrDefault();
        if (power == null) return;
        await PowerCmd.ModifyAmount(power, Math.Max(amount, -power.Amount), creature, source);
    }

    /// <summary>¿Puede pagar un coste de estrellas (conversores estilo 等价交换)?</summary>
    public static bool CanPay(Creature creature, int cost) => Of(creature) >= cost;
}
