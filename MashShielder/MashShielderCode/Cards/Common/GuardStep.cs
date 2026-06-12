using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Paso de Guardia — rediseño v2: 1E Habilidad; si tenés 30+ Estrellas de Crítico,
/// consumí 30 → robá 2 y obtené 4 de Bloqueo (up: exige y consume solo 20). El
/// 忘却补正 de Mash con sabor defensivo: leer trayectorias convierte estrellas en
/// cartas y postura.
/// </summary>
public sealed class GuardStep() : MashShielderCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StarCost", 30),
        new CardsVar(2),
        new BlockVar(4m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!CritStars.CanPay(Owner.Creature, DynamicVars["StarCost"].IntValue)) return;
        await CritStars.Gain(Owner.Creature, -DynamicVars["StarCost"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StarCost"].UpgradeValueBy(-10m);
    }
}
