using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Filo de Gloria (绝刀之锋) — ESTE TURNO tus CRÍTICOS hacen +<see cref="Amount"/> de daño.
/// Versión temporal del Genio de la Espada (mismo mecanismo: +Amount aditivo al golpe que
/// consume el *Crítico Listo, antes del ×2). La aplica «Zettou A» — el turno de gloria
/// embotellado. Se auto-remueve al terminar tu turno (patrón ExposedBackPower / TemporaryStrength).
/// Counter: aplicaciones del mismo turno suman.
/// </summary>
public sealed class GloryEdgePower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return Owner.GetPowerAmount<CritReadyPower>() > 0 ? Amount : 0m;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
