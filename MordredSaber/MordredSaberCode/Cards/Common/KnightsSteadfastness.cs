using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Firmeza del Caballero (骑士坚毅) — DESIGN-MORDRED §5.1. 2⚡ Hab: 13 de Bloqueo; si Enmascarado +20 NP
/// (up +4/+20), glow. El muro grande de la forma defensiva (el bloqueo entra bajo el Baluarte de 10) que
/// además carga el medidor cuando estás tras el casco. Patrón SparksOfTheHelm escalado a 2⚡ ([Formas→NP
/// defensivo]). El up sube el Bloqueo y el +NP condicional.
/// </summary>
public sealed class KnightsSteadfastness() : MordredCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(13m, ValueProp.Move), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Forms.InMaskedForm(Owner.Creature))
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
