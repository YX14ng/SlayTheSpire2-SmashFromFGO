using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Acción Independiente A+ / 单独行动 A+ (DESIGN-GILGAMESH §5.3 «KIT Independent Action A+») — la
/// pasiva real (Crit Damage +11%). Acá: mientras tengas un *Crítico Listo armado, tus Ataques hacen
/// +<see cref="Amount"/> de daño ANTES del ×2 (el plano entra en el multiplicador del juicio: con
/// CritReady ×2, +6 plano se vuelve +12 efectivos — exactamente «crit damage up»). Sin Crítico Listo
/// no hace nada (la pasiva sólo brilla cuando el Rey desprecia).
///
/// Aditivo (patrón Mash/Divinidad), gateado por <see cref="CritReadyPower"/> presente en el dueño:
/// <see cref="CritReadyPower.ModifyDamageMultiplicative"/> aplica el ×2 a TODO el daño aditivo de la
/// carta, así que el orden aditivo→multiplicativo de StS hace el resto sin tocar la lógica del crit.
/// Sólo golpes reales; capa ofensiva personal (no escala en MP).
/// </summary>
public sealed class IndependentActionPower : GilgameshPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        if (!Owner.HasPower<CritReadyPower>()) return 0m;
        return base.Amount;
    }
}
