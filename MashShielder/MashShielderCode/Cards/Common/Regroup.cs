using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Reagrupar — retoque v2: 1E Habilidad, 5 Bloqueo (up +2); si no tenías Bloqueo:
/// +20 de Carga NP (up +10; antes 12/18 — denominación fija). El arranque tras un
/// turno con el muro caído.
/// </summary>
public sealed class Regroup() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("NpCharge", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block == 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hadNoBlock = Owner.Creature.Block == 0;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (hadNoBlock)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
