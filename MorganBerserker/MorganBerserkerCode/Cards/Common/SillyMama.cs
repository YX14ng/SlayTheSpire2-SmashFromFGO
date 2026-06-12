using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Meme: Mamá Boba (笨蛋妈妈) — retocada v2: 0⚡, roba 1 + Carga NP 10; mejorada:
/// además +10 Estrellas de Crítico (la nena te tira estrellitas).
/// </summary>
public sealed class SillyMama() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("NpCharge", 10),
        new DynamicVar("Stars", 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<FGOCore.FGOCoreCode.Stars.CritStarsPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (DynamicVars["Stars"].IntValue > 0)
        {
            await FGOCore.FGOCoreCode.Stars.CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
