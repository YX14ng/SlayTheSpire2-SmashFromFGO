using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Chispas del Yelmo (头盔火花) — DESIGN-MORDRED §5.1. 1⚡ Hab: 8 de Bloqueo + <see cref="BaseNp"/> NP;
/// si Enmascarado, +20 NP en total en su lugar (up +3 Bloqueo), glow. Payoff de la forma defensiva (las
/// chispas del casco cargan el medidor).
///
/// BI-CONDICIONAL SUAVE (DESIGN-REVIEW-2): antes daba 0 NP fuera de Enmascarado → carta muerta si caía
/// en la forma equivocada y fragmentaba la mano. Ahora siempre carga un PISO (<see cref="BaseNp"/>), y
/// Enmascarado lo SUBE al total <c>NpCharge</c>. El up sube SOLO el Bloqueo (los NP quedan en su
/// denominación). Patrón [Formas→NP] con piso garantizado.
/// </summary>
public sealed class SparksOfTheHelm() : MordredCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int BaseNp = 10; // piso de NP en cualquier forma (la chispa salta igual)

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("BaseNp", BaseNp)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        var np = Forms.InMaskedForm(Owner.Creature) ? DynamicVars["NpCharge"].IntValue : DynamicVars["BaseNp"].IntValue;
        await NpCharge.Gain(choiceContext, Owner.Creature, np, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
