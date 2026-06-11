using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Maldición de Cernunnos (科尔努诺斯的诅咒) — Poder: tus Maldiciones ya no se
/// reducen al activarse. Mejora: coste 1⚡.
/// </summary>
public sealed class CurseOfCernunnos() : MorganCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CurseOfCernunnosPower>("Stacks", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CurseOfCernunnosPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
