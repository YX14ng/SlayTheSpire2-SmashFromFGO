using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Cabalgar E (骑乘E) — al inicio de cada turno: +<see cref="Amount"/> *Estrellas de Crítico
/// (DESIGN-OKITA §5.3, "sabe montar… apenas": +5; up +10). El goteo mínimo del rango E.
/// Counter (guarda las estrellas/turno). Personal: no escala en multijugador.
/// </summary>
public sealed class RidingEPower : OkitaPower
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
