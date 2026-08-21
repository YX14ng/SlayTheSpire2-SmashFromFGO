using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Yelmo Abollado (凹陷头盔) — DESIGN-MORDRED §5.2. 1⚡ Hab: 11 de Bloqueo + <see cref="BaseStars"/> Estrellas;
/// en Enmascarado, +10 Estrellas en su lugar (up +4 Bloqueo), glow. Payoff de la forma defensiva (las
/// abolladuras del casco saltan en chispas → ★). El bloqueo entra bajo el Baluarte si estás Enmascarada.
///
/// BI-CONDICIONAL SUAVE (DESIGN-REVIEW-2): antes daba 0 ★ fuera de Enmascarado. Ahora un PISO
/// (<see cref="BaseStars"/>) garantizado en cualquier forma, y Enmascarado lo sube al total <c>Stars</c>.
/// El ★ NO sube con el up. Patrón SparksOfTheHelm con ★ en lugar de NP.
/// </summary>
public sealed class DentedHelm() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int BaseStars = 10; // piso normalizado en cualquier forma

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(11m, ValueProp.Move), new DynamicVar("Stars", 20), new DynamicVar("BaseStars", BaseStars)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        var stars = Forms.InMaskedForm(Owner.Creature) ? DynamicVars["Stars"].IntValue : DynamicVars["BaseStars"].IntValue;
        await CritStars.Gain(choiceContext, Owner.Creature, stars, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
