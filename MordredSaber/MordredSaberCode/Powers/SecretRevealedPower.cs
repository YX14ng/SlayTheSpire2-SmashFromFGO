using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Powers.Forms;
using FormsHelper = MordredSaber.MordredSaberCode.Powers.Forms.Forms;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Secreto Revelado (秘密揭露, §5.3) — el motor de la danza: cada vez que TE QUITÁS EL YELMO (entrás
/// en Rebelión, un cambio de forma iniciado por el jugador), ganás +<see cref="Stars"/> Estrellas y
/// robás <see cref="Cards"/>. Implementa <see cref="IFormChangeListener"/> (lo notifica FormSwitch.Enter,
/// igual que el Corazón de Homúnculo de Mash) y solo proca al pasar a Rebelión (la revelación), no en
/// cualquier swap. El <see cref="Stars"/> es campo settable que fija la carta; Amount = conteo de
/// stacks (Counter, las copias suman las ★). Personal.
/// </summary>
public sealed class SecretRevealedPower : MordredPower, IFormChangeListener
{
    public int Stars = 20;
    public int Cards = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebellionFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        // Solo cuenta la REVELACIÓN: arrancarse el yelmo (entrar en Rebelión).
        if (!FormsHelper.InRebellion(Owner)) return;
        Flash();
        await CritStars.Gain(Owner, Stars * (int)Amount, null);
        if (choiceContext != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, Cards, Owner.Player);
        }
    }
}
