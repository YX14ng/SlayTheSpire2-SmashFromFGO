using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Common;

/// <summary>Charco de Marea — sembradora defensiva: Baluarte (retenido entre turnos) + Maldición a un
/// enemigo. El doble rol de la fase Lily: sobrevivir mientras acuñás la divisa. Bloquea Y siembra el
/// puente en la misma carta común.</summary>
public sealed class TidePool() : TiamatCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Curse", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue);
        await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
