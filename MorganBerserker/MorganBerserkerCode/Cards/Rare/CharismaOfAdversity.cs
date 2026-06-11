using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Carisma de la Adversidad (逆境的魅力) — Poder: tus ataques hacen +1/+2/+3/+4
/// de daño según tu HP faltante (1%/25%/50%/75%).
/// </summary>
public sealed class CharismaOfAdversity() : MorganCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CharismaOfAdversityPower>("Stacks", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CharismaOfAdversityPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(1m);
    }
}
