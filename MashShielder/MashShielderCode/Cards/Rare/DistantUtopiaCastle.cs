using System.Linq;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Castillo de la Utopía Lejana — Power: conservás Bloqueo entre turnos hasta la altura del castillo
/// (40, mejora 60). REDESIGN-MASH-V2 §6.3: antes era retención INFINITA. La mejora ya NO abarata
/// (precedente «Corte del Invierno» de REDESIGN-MORGAN-V2): sube la altura, que es el efecto.
/// </summary>
public sealed class DistantUtopiaCastle() : MashShielderCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DistantUtopiaCastlePower>("DistantUtopiaCastle", DistantUtopiaCastlePower.BaseHeight)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var height = DynamicVars["DistantUtopiaCastle"].BaseValue;
        await PowerCmd.Apply<DistantUtopiaCastlePower>(choiceContext, Owner.Creature, height, Owner.Creature, this);

        // Co-op (la party entera se atrinchera tras la muralla): cada aliado vivo conserva TODO su
        // Bloqueo entre turnos. El power opera sobre su propio Owner (ShouldClearBlock/RetentionCap
        // referencian su portador). En 1 jugador el foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await PowerCmd.Apply<DistantUtopiaCastlePower>(choiceContext, ally, height, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DistantUtopiaCastle"].UpgradeValueBy(
            DistantUtopiaCastlePower.UpgradedHeight - DistantUtopiaCastlePower.BaseHeight);
    }
}
