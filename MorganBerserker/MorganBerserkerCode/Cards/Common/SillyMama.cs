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
        new DynamicVar("NpCharge", 10),
        new DynamicVar("Curse", 0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CursePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (DynamicVars["Curse"].IntValue > 0)
        {
            foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).Where(e => !e.IsDead).ToList())
            {
                await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(2m);
    }
}
