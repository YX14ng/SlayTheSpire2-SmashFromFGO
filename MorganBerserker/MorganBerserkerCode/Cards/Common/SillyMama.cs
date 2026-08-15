using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Meme: Mamá Boba (笨蛋妈妈) — retocada 2026-06-15 (swap Estrellas→Maldición): 0⚡, roba 1
/// + Carga NP 10; mejorada: además aplica 2 de Maldición a TODOS (la nena reparte maldiciones).
/// </summary>
public sealed class SillyMama() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new DynamicVar("NpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    // RE-POOL V2 (parche J2-16): la mejora NO muta de rol — antes agregaba Maldición AoE, ahora
    // solo engorda el cantrip (+10 NP). El meme queda como pegamento flex de la línea C.
    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
