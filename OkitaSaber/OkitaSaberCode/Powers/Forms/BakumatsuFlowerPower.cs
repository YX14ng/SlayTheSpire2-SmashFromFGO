using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Cards;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Powers.Forms;

/// <summary>
/// Forma permanente de climax: las Rafagas dejan de gastar Aliento y agrega Tos al mazo.
/// Comparte el set animado oficial de Okita para no cargar un recurso inexistente.
/// </summary>
public sealed class BakumatsuFlowerPower : OkitaFormPower, IRafagaCostModifier
{
    public override bool IsPermanent => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/okita_frames.tres";

    public bool WaivesBreathCost => true;

    public int HpPerBreathPoint => 0;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        Flash();
        await Tos.ShuffleIntoDraw(choiceContext, Owner, null);
    }
}
