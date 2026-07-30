using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>La primera vez por turno que realizas un Crítico, recupera 10 estrellas.</summary>
public sealed class SwordInstinctPower : ArtoriaPower, ICritDiscount, ICriticalConsumedListener
{
    public const int StarsRefund = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public int CritCostReduction => 0;

    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (FgoCombatState.GetTurn(Owner, 2) != 0 || critical.Owner != Owner) return;
        await FgoCombatState.SetTurn(context, Owner, 2, 1, critical.Card);
        Flash();
        await CritStars.Gain(context, Owner, StarsRefund, critical.Card);
    }
}
