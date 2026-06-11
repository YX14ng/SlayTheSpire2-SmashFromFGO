using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Truco del Clan del Espejo (镜之氏族的把戏) — switch to your opposite form, draw 2.</summary>
public sealed class MirrorClansTrick() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<RainWitchFormPower>())
        {
            await FormSwitch.Enter<FairyQueenFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await FormSwitch.Enter<RainWitchFormPower>(choiceContext, Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
