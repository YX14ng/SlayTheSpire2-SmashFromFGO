using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Munición Conceptual — re-perfilado P2 2026-06-25. El power viejo (BeforeCardPlayed: quitaba
/// 1 buff antes del daño, X veces/turno) casi nunca procaba (competía con las cartas que ya
/// nacen Black Barrel y stripeaban ellas mismas → power muerto). Ahora es un PAYOFF pasivo del
/// eje de strip: cada vez que QUITÁS un buff enemigo con CUALQUIER carta, ganás <see cref="Amount"/>
/// Estrellas de Crítico (base 6). El disparo vive en el chokepoint <see cref="BlackBarrel.RemoveBuffs"/>
/// — la única vía por la que pasan todas las remociones de buff del jugador. Counter (el monto
/// es las ★ por buff, no una pila de cargas).
/// </summary>
public sealed class ConceptualAmmoPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// Otorga las Estrellas de Crítico por <paramref name="strippedBuffs"/> buffs removidos
    /// (Amount★ por buff). Llamado por <see cref="BlackBarrel.RemoveBuffs"/> tras el strip.
    /// El Flash vive acá porque PowerModel.Flash es protected (no accesible desde BlackBarrel).
    /// </summary>
    public async Task OnBuffsStripped(int strippedBuffs, CardModel? source)
    {
        if (strippedBuffs <= 0 || Amount <= 0) return;
        Flash();
        await CritStars.Gain(Owner, Amount * strippedBuffs, source);
    }
}
