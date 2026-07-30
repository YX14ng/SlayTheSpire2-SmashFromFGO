using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Carisma de la Adversidad (逆境的魅力) — Poder: tus ataques hacen +1/+2/+3/+4
/// de daño según tu HP faltante (1%/25%/50%/75%).
/// Co-op: el bono por-HP es intrínseco a Morgan (no compartible), pero el carisma se contagia:
/// al jugarla, cada aliado gana !AllyStrength! de Fuerza.
/// </summary>
public sealed class CharismaOfAdversity() : MorganCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CharismaOfAdversityPower>("Stacks", 1m), new DynamicVar("AllyStrength", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CharismaOfAdversityPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue,
            Owner.Creature, this);
        // Co-op (el bono por-HP es inseparable de Morgan; lo que se comparte es el carisma en sí):
        // cada aliado vivo gana algo de Fuerza. En 1 jugador el foreach queda vacío (fiel a 1 jugador).
        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
            await PowerCmd.Apply<StrengthPower>(choiceContext, ally, DynamicVars["AllyStrength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(1m);
    }
}
