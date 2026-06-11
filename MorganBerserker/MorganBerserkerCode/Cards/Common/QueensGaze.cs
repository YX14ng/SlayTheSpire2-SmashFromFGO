using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#17 Mirada de la Reina (女王的凝视) — 0⚡: aplica 1 de Débil + Carga NP 5.</summary>
public sealed class QueensGaze() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>("Weak", 1m),
        new DynamicVar("NpCharge", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars["NpCharge"].UpgradeValueBy(3m);
    }
}
