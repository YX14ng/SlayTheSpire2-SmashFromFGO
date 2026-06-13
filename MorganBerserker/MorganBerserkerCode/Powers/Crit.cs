using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// "Crítico" de Morgan (rediseño 2026-06-13): gasto MANUAL del banco de Estrellas.
/// Una carta de Ataque Buster, al jugarse, consume <see cref="CritStarsPower.CritCost"/>
/// (50) Estrellas para aplicar 1 de <see cref="CritReadyPower"/> — esa carta hace ×2.
/// Es la decisión activa que reemplaza al viejo auto-proc a 100: bancás estrellas (Quick,
/// Cetro al sangrar, cambios de forma) y elegís EN QUÉ Buster volcarlas. Llamar en OnPlay
/// ANTES de infligir el daño (CritReady reclama la carta que dispara el golpe).
/// </summary>
public static class Crit
{
    public static bool CanCrit(Creature c) => CritStars.CanPay(c, CritStarsPower.CritCost);

    /// <summary>Gasta 50★ y aplica Crítico Listo a esta carta. Devuelve true si crikeó.</summary>
    public static async Task<bool> TrySpend(Creature c, CardModel card)
    {
        if (!CritStars.CanPay(c, CritStarsPower.CritCost)) return false;
        await CritStars.Gain(c, -CritStarsPower.CritCost, card);
        await PowerCmd.Apply<CritReadyPower>(c, 1m, c, card);
        return true;
    }
}
