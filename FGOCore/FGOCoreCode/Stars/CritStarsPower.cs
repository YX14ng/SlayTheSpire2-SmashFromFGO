using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>
/// Estrellas de Crítico (Critical Stars / 暴击星) — recurso compartido FGO, patrón
/// JeanneAlter (la referencia de diseño del usuario): contador que al llegar a
/// <see cref="Threshold"/> se descuenta 100 automáticamente y otorga 1 de
/// <see cref="CritReadyPower"/> (próximo Ataque ×2). El auto-payoff convierte cada
/// «+X estrellas» en progreso tangible — la lección central del análisis de Jeanne.
/// NO confundir con el contador chico con candado de forma de ArtoriaCaster
/// (mecánica distinta, mod-local).
/// </summary>
public sealed class CritStarsPower : FGOCorePower
{
    public const int Threshold = 100;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _isProcessing;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        if (power != this || _isProcessing) return;

        // while: una sola carta puede cruzar 200+ (p. ej. +100 de golpe).
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
