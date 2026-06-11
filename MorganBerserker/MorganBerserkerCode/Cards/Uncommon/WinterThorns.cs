using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Espinas del Invierno (寒冬荆棘) — 7 de Bloqueo; los enemigos que te ataquen este turno ganan 2 de Maldición.</summary>
public sealed class WinterThorns() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new PowerVar<WinterThornsPower>("Thorns", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<WinterThornsPower>(Owner.Creature, DynamicVars["Thorns"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Thorns"].UpgradeValueBy(1m);
    }
}
