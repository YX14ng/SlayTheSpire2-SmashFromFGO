using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Genio de la Espada (剑之天才) — PERMANENTE: tus CRÍTICOS hacen +<see cref="Amount"/> de daño
/// (DESIGN-OKITA §5.4). Un crítico = un Ataque que consume *Crítico Listo (el ×2 de FGOCore).
/// Mientras tengas un Crítico Listo en cola, el Ataque que lo va a consumir recibe +Amount ADITIVO
/// (se suma ANTES del ×2 — el bono también se dobla, igual que el daño base, fiel a "el crítico
/// pega más fuerte"). Counter: copias suman. Personal: no escala en multijugador.
///
/// Implementación con hooks verificados (no requiere API nueva de FGOCore): lee CritReadyPower del
/// owner. El cálculo de daño de FGOCore aplica primero los aditivos y luego el ×2 multiplicativo de
/// CritReadyPower, así que el +Amount entra al golpe que va a criticar y se dobla con él.
/// </summary>
public sealed class SwordGeniusPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // Solo el golpe que va a criticar (hay un Crítico Listo en cola) recibe el bono.
        return Owner.GetPowerAmount<CritReadyPower>() > 0 ? Amount : 0m;
    }
}
