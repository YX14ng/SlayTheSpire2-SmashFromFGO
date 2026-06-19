using System.Linq;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>Castillo de la Utopía Lejana — Power: ALL your Block persists between turns.</summary>
public sealed class DistantUtopiaCastle() : MashShielderCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DistantUtopiaCastlePower>(1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DistantUtopiaCastlePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);

        // Co-op (la party entera se atrinchera tras la muralla): cada aliado vivo conserva TODO su
        // Bloqueo entre turnos. El power opera sobre su propio Owner (ShouldClearBlock/RetentionCap
        // referencian su portador). En 1 jugador el foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await PowerCmd.Apply<DistantUtopiaCastlePower>(choiceContext, ally, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
