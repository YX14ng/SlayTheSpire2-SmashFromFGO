using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Filo de Gloria (绝刀之锋) — ESTE TURNO tus CRÍTICOS hacen +<see cref="Amount"/> de daño.
/// Versión temporal del Genio de la Espada (mismo mecanismo: +Amount aditivo al golpe que
/// consume el *Crítico Listo, antes del ×2). La aplica «Zettou A» — el turno de gloria
/// embotellado. Se auto-remueve al terminar tu turno (patrón ExposedBackPower / TemporaryStrength).
/// Counter: aplicaciones del mismo turno suman.
/// </summary>
public sealed class GloryEdgePower : AttackDamageAdditivePower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    // Solo el golpe que va a criticar (hay un Crítico Listo en cola) recibe el bono.
    protected override bool BonusApplies() => Owner.GetPowerAmount<CritReadyPower>() > 0;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side) await PowerCmd.Remove(this);
    }
}
