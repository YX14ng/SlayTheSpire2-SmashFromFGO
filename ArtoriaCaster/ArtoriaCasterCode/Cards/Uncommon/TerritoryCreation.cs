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
    // "Boosted" es el valor MOSTRADO del bono "2+ Habilidades" — debe ser SIEMPRE
    // Stacks×2, que es lo que TerritoryCreationPower.BeforeTurnEnd calcula (Amount*2).
    // Lo derivamos de "Stacks" (no es un var independiente) para que tooltip y efecto
    // tengan una sola fuente de verdad y el ×2 no pueda driftar en futuros upgrades.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TerritoryCreationPower>("Stacks", 3m),
        new DynamicVar("Boosted", 3m * 2m),
        new DynamicVar("Skills", TerritoryCreationPower.SkillThreshold)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TerritoryCreationPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(1m);
        // Mantener Boosted == Stacks×2 (espejo de Amount*2 en el poder), sin literal manual.
        DynamicVars["Boosted"].BaseValue = DynamicVars["Stacks"].BaseValue * 2m;
    }
}
