using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Firmeza del Caballero (骑士坚毅) — DESIGN-MORDRED §5.1. 2⚡ Hab: 13 de Bloqueo + <see cref="BaseNp"/> NP;
/// si Enmascarado, +10 NP en su lugar (up +4 Bloqueo / +20 al NP Enmascarado), glow. El muro grande de la
/// forma defensiva (el bloqueo entra bajo el Baluarte de 10) que además carga el medidor.
///
/// BI-CONDICIONAL SUAVE (DESIGN-REVIEW-2): antes daba 0 NP fuera de Enmascarado. Ahora un PISO
/// (<see cref="BaseNp"/>) garantizado en cualquier forma, y Enmascarado lo sube al total <c>NpCharge</c>.
/// El up sube el Bloqueo y el +NP de Enmascarado (el piso queda fijo). Patrón SparksOfTheHelm a 2⚡.
/// </summary>
public sealed class KnightsSteadfastness() : MordredCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int BaseNp = 10; // piso de NP en cualquier forma

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(13m, ValueProp.Move), new DynamicVar("NpCharge", 20), new DynamicVar("BaseNp", BaseNp)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        var np = Forms.InMaskedForm(Owner.Creature) ? DynamicVars["NpCharge"].IntValue : DynamicVars["BaseNp"].IntValue;
        await NpCharge.Gain(choiceContext, Owner.Creature, np, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
