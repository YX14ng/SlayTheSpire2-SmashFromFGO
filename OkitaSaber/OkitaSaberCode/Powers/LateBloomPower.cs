using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Florecer Tardío (迟开之花) — cada vez que ganás una *Tos: +<see cref="Amount"/> *Estrellas de
/// Crítico (DESIGN-OKITA §5.3: +20; up +30). La enfermedad alimenta la gloria. Lo dispara
/// <see cref="Tos.ShuffleIntoDraw"/> (único punto de generación de Tos) vía <see cref="ILateBloomListener"/>.
/// Counter (cada copia suma sus ★). Personal: no escala en multijugador.
/// </summary>
public sealed class LateBloomPower : OkitaPower, ILateBloomListenerWithContext
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public async Task OnTosGained(Creature creature, CardModel? source)
    {
        await OnTosGained(new BlockingPlayerChoiceContext(), creature, source);
    }

    public async Task OnTosGained(
        PlayerChoiceContext choiceContext, Creature creature, CardModel? source)
    {
        if (creature != Owner) return;
        Flash();
        await CritStars.Gain(choiceContext, Owner, Amount, source);
    }
}
