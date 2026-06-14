using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Caballero del Relámpago Rojo A+ (赤雷骑士A+) — el Rank-Up de Estallido de Maná como PODER
/// (DESIGN-MORDRED §5.2): el multiplicador real de la forma ofensiva vive ACÁ (§5), no en la pasiva.
///   - tus Ataques hacen +<see cref="Amount"/> (aditivo);
///   - tus CRÍTICOS hacen +<see cref="CritBonus"/> ADICIONAL (el +N entra al golpe que va a
///     consumir el *Crítico Listo, ANTES del ×2 — se dobla con él, patrón SwordGeniusPower de Okita).
/// El <see cref="Amount"/> guarda el +Ataque (2; up 3); CritBonus es campo settable que la carta fija
/// desde su DynamicVar (6; up 8) para no chocar con el conteo de stacks. Counter: copias suman.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class KnightOfRedLightningPower : MordredPower
{
    public int CritBonus = 6; // up 8 (la carta lo setea desde su DynamicVar)

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // +Ataque plano siempre; +Crítico extra solo cuando hay un Crítico Listo en cola.
        var critExtra = Owner.GetPowerAmount<CritReadyPower>() > 0 ? CritBonus : 0;
        return Amount + critExtra;
    }
}
