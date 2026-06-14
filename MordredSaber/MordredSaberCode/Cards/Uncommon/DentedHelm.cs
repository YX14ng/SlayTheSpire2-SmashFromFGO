using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Yelmo Abollado (凹陷头盔) — DESIGN-MORDRED §5.2. 1⚡ Hab: 11 de Bloqueo; en Enmascarado +10 Estrellas
/// (up +4 Bloqueo), glow. Payoff de la forma defensiva (las abolladuras del casco saltan en chispas →
/// ★). Leído con <see cref="Forms.InMaskedForm"/>. El bloqueo entra bajo el Baluarte si estás
/// Enmascarada. El ★ NO sube con el up. Patrón SparksOfTheHelm con ★ en lugar de NP.
/// </summary>
public sealed class DentedHelm() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(11m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Forms.InMaskedForm(Owner.Creature))
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}
