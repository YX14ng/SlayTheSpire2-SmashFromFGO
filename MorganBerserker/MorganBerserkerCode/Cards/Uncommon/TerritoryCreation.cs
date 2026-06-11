using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Creación de Territorio (阵地作成) — al final de tu turno: gana 4 de Bloqueo.</summary>
public sealed class TerritoryCreation() : MorganCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<TerritoryCreationPower>("Stacks", 4m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TerritoryCreationPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(2m);
    }
}
