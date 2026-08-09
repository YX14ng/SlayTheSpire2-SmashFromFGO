using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Basic;

/// <summary>Engendrar — firma del mazo inicial: parí 1 Laḫmu y cargá NP (motor de arranque).
/// PARIR sube el techo del enjambre; la carga acerca la primera ventana Bestia.</summary>
public sealed class SpawnLahmu() : TiamatCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self), BaseLib.Abstracts.ITranscendenceCard
{
    public MegaCrit.Sts2.Core.Models.CardModel GetTranscendenceTransformedCard() =>
        MegaCrit.Sts2.Core.Models.ModelDb.Card<Rare.ElevenBelLahmu>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 1),
        new DynamicVar("NpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Lahmu.Spawn(choiceContext, Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(6m);
}
