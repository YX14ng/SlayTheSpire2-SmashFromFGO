using MashShielder.MashShielderCode.Powers;
using MashShielder.MashShielderCode.Powers.Forms;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Protocolo Bunker — adapts to your form: Shielder charges NP, Ortinax/Paladin builds the wall.
/// Rediseño v2: en forma Shielder +30 NP (up +10; antes 25/35 — denominación fija); si no, 14 Bloqueo (up +4).
/// Glow dorado en Shielder (el modo NP es el titular de la carta).</summary>
public sealed class BunkerProtocol() : MashShielderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 30),
        new BlockVar(14m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => !Forms.InOffensiveForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Forms.InOffensiveForm(Owner.Creature))
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
        else
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
