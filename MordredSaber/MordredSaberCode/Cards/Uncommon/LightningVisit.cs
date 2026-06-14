using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Visita Relámpago (闪电造访) — DESIGN-MORDRED §5.2. 0⚡ Hab: cambiá a tu forma opuesta; al final del
/// turno volvés a la forma anterior (up: al volver, +10 Estrellas). La ventana de 1 turno (arrancarse
/// el yelmo, rugir, volver). El switch de ida lo hace <see cref="Forms.Enter"/>; el de vuelta lo
/// gestiona <see cref="LightningVisitReturnPower"/> (BeforeTurnEnd). En Relámpago Carmesí (permanente)
/// el switch es no-op. <see cref="LightningVisitReturnPower.ReturnToMasked"/> recuerda la forma previa.
/// </summary>
public sealed class LightningVisit() : MordredCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<RebellionFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var wasMasked = Forms.InMaskedForm(Owner.Creature);
        if (wasMasked)
        {
            await Forms.Enter<RebellionFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await Forms.Enter<MaskedKnightFormPower>(choiceContext, Owner.Creature, this);
        }
        var ret = await PowerCmd.Apply<LightningVisitReturnPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (ret != null)
        {
            ret.ReturnToMasked = wasMasked;
            ret.StarsOnReturn = IsUpgraded ? DynamicVars["Stars"].IntValue : 0;
        }
    }
}
