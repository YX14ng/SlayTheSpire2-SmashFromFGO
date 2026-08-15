using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Truco del Clan del Espejo (镜之氏族的把戏) — re-pool V2 (M2/§3.3): baja a COMÚN —
/// el toggle anti-atrapado debe aparecer en cada run. 1⚡: cambiá a tu forma opuesta, robá 2.
/// Mejora (parche J1-1: la mejora a 0⚡ queda PROHIBIDA): además +10 NP.</summary>
public sealed class MirrorClansTrick() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new DynamicVar("NpCharge", 0)
    ];

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
        if (DynamicVars["NpCharge"].IntValue > 0)
        {
            await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
