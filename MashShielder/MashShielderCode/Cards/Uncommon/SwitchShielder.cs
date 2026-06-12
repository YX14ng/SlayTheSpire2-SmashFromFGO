using MashShielder.MashShielderCode.Powers;
using MashShielder.MashShielderCode.Powers.Forms;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Cambio: ¡Shielder! — return to the defensive form, recovering NP Charge.
/// Rediseño v2: 20 NP (up +10; antes 15/25 — denominación fija).</summary>
public sealed class SwitchShielder() : MashShielderCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShielderFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Forms.Enter<ShielderFormPower>(choiceContext, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
