using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Biblioteca de Chaldea — Power: draw 1 additional card each turn.
/// REEMPLAZADA por CombatAnalysis (que hereda su slot de arte). No hay mecanismo limpio
/// de exclusión de pool en BaseLib (sin CanDrop/IsInPool), así que se usa el mismo truco
/// que las Unleashed: CardRarity.Event no aparece en recompensas pero la clase sigue
/// registrada y las runs guardadas la resuelven.
/// Removida del pool en rediseño v2; borrar en la próxima versión.</summary>
public sealed class ChaldeaLibrary() : MashShielderCard(2, CardType.Power, CardRarity.Event, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ChaldeaLibraryPower>(1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ChaldeaLibraryPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
