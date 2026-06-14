using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Recuerdo de la Última Primavera (最后春日之忆) — al inicio de cada turno: +<see cref="Amount"/>
/// *Estrellas de Crítico (DESIGN-OKITA §5.4: +20; up +30). El motor ★ per-turn (la paz del engawa).
/// </summary>
public sealed class LastSpringMemoryPower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        await CritStars.Gain(Owner, Amount, null);
    }
}
