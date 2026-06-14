using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Estandarte de la Rebelión (叛逆旗帜, §5.2) — el motor de FORMAS: cada cambio de forma ganás
/// <see cref="StarsPerSwitch"/> Estrellas (10) y <see cref="NpPerSwitch"/> de Carga NP (5); el up
/// añade robar 1 carta (<see cref="DrawsPerSwitch"/>). Implementa <see cref="IFormChangeListener"/>
/// (lo notifica FormSwitch.Enter en los cambios iniciados por el jugador — el setup de combate con
/// source==null NO cuenta). Los valores por activación son campos settables que fija la carta desde
/// sus DynamicVars; Amount es el conteo de stacks (Counter). Patrón FestivalSpiritPower/HomunculusHeart.
/// </summary>
public sealed class BannerOfRebellionPower : MordredPower, IFormChangeListener
{
    public int StarsPerSwitch = 10;
    public int NpPerSwitch = 5;
    public int DrawsPerSwitch; // 0 base; 1 tras el up (lo fija la carta)

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CritStars.Gain(Owner, StarsPerSwitch * (int)Amount, null);
        await NpCharge.Gain(Owner, NpPerSwitch * (int)Amount, null);
        if (DrawsPerSwitch > 0)
        {
            await MegaCrit.Sts2.Core.Commands.CardPileCmd.Draw(
                choiceContext ?? new BlockingPlayerChoiceContext(), DrawsPerSwitch * (int)Amount, Owner.Player);
        }
    }
}
