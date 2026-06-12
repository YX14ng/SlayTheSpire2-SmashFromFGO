using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Munición Conceptual — Power: generador de la familia Black Barrel.
/// Rediseño v2 (parche P8a): una vez por turno, cuando juegas un Ataque contra un enemigo
/// con buffs, elimina 1 buff ANTES de resolver el daño (up: dos veces por turno; el costo
/// queda en 2).</summary>
public sealed class ConceptualAmmo() : MashShielderCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ConceptualAmmoPower>("ConceptualAmmo", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ConceptualAmmoPower>(Owner.Creature, DynamicVars["ConceptualAmmo"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ConceptualAmmo"].UpgradeValueBy(1m);
    }
}
