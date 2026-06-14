using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Velo de la Noche (Evening Shroud EX) -- DESIGN-OBERON 6.3, el único skill limpio del kit. Tus
/// cartas-NP (<see cref="IOberonNpCard"/>) hacen +<see cref="BonusPct"/>% de daño por
/// <see cref="Amount"/> turnos (el Counter son los turnos restantes; decrece en AfterTurnEnd). Patrón
/// CritReadyPower (% en vez de ×2, sin consumir por golpe). <see cref="BonusPct"/> lo fija la carta.
/// </summary>
public sealed class EveningShroudPower : OberonPower
{
    public int BonusPct = 30;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || Amount <= 0 || cardSource is not IOberonNpCard) return 1m;
        return 1m + BonusPct / 100m;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        await PowerCmd.Decrement(this);
    }
}
