using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Basic;

/// <summary>
/// Bajar la Visera (落下面甲) — FIRMA básica de la forma DEFENSIVA (DESIGN-MORDRED §5.0): el retiro
/// táctico que carga. Te ponés el yelmo (entrás en Enmascarado), 4 de Bloqueo y +5 NP. El bloqueo
/// se da YA enmascarada, así lo conserva el Baluarte de la forma (hasta 10). Up: 7 Bloqueo / +10 NP.
/// </summary>
public sealed class LowerTheVisor() : MordredCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("NpCharge", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MaskedKnightFormPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // El yelmo PRIMERO: el bloqueo entra ya bajo el Baluarte de Enmascarado.
        await Forms.Enter<MaskedKnightFormPower>(choiceContext, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(5m);
    }
}
