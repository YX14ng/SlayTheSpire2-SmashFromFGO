using System.Linq;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Guardiana Incansable — Power: permanent Intercept.
/// Rediseño v2: +5 Intercepción permanente (up +3; antes 4/6) — más masa sostenida
/// para los payoffs de golpe detenido.</summary>
public sealed class TirelessGuardian() : MashShielderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<InterceptPower>("Intercept", 5m), new DynamicVar("AllyIntercept", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InterceptPower>(Owner.Creature, DynamicVars["Intercept"].BaseValue, Owner.Creature, this);

        // Co-op (la Mesa Redonda contraataca en conjunto): cada aliado vivo gana algo de Intercepción
        // permanente; InterceptPower es agnóstico al Owner (dispara cuando SU portador bloquea un golpe).
        // En 1 jugador PlayerCreatures es solo el Owner -> el foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await PowerCmd.Apply<InterceptPower>(ally, DynamicVars["AllyIntercept"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Intercept"].UpgradeValueBy(3m);
    }
}
