using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Divinidad B / 神性 B (DESIGN-GILGAMESH §2 carta «KIT Divinity B», §5.3) — la pasiva real de Gil
/// (Damage Plus +175 plano por carta en FGO porque es 2/3 dios). Acá: tus ATAQUES hacen +
/// <see cref="Amount"/> de daño, INCLUIDAS las Armas del Tesoro (el flat-plus brilla sobre el
/// volumen de la tribu y el multi-hit). Patrón <c>MashFormPower.ModifyDamageAdditive</c>: aditivo,
/// así que entra ANTES del ×2 del Crítico Listo (el plano se duplica con el juicio del Rey, fiel).
///
/// Sólo golpes reales (<see cref="ValuePropExtensions.IsPoweredAttack"/>): Maldición/Veneno/coste de
/// HP de Dáinsleif NO son «golpes» y no reciben el plus. Capa ofensiva personal: no escala en MP.
/// </summary>
public sealed class DivinityPower : GilgameshPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return base.Amount;
    }
}
