using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>Voluntad de Acero — at the end of your turn, gain this much BULWARK Block.
/// Rediseño v2: el Bloqueo ahora es con Baluarte — apila sobre la retención de la
/// reliquia inicial y enciende los riders de Baluarte (FrontalCharge).</summary>
public sealed class IronWillPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player || Owner.Side != side) return;
        Flash();
        await BlockRetention.GainBulwarkBlock(null, Owner, Amount);
    }
}
