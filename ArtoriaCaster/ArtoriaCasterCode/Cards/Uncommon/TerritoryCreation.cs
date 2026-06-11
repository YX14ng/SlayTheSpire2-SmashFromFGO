using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Creación de Territorio EX (陣地作成, pasiva real de Castoria) — Poder: al final
/// de tu turno ganás 3 de Bloqueo; si jugaste 2+ Habilidades este turno: 6.
/// Mejorada: 4 / 8.
/// </summary>
public sealed class TerritoryCreation() : ArtoriaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TerritoryCreationPower>("Stacks", 3m),
        new DynamicVar("Boosted", 6),
        new DynamicVar("Skills", TerritoryCreationPower.SkillThreshold)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TerritoryCreationPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(1m);
        DynamicVars["Boosted"].UpgradeValueBy(2m);
    }
}
