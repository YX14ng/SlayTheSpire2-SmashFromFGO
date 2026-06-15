using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Base LOCAL de los powers "al inicio de cada turno: +<see cref="PowerModel.Amount"/> *Estrellas
/// de Crítico" (Cabalgar E §5.3, Recuerdo de la Última Primavera §5.4). Factoriza el cuerpo
/// idéntico (Counter/Buff personal + el hover tip de CritStars + el goteo en AfterPlayerTurnStart);
/// las subclases sólo aportan su class name (= model ID / loc / ícono) y su monto de diseño.
/// Counter, personal: no escala en multijugador.
/// </summary>
public abstract class PerTurnStarsPower : OkitaPower
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
