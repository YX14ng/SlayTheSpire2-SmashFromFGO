using System.Linq;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Amalgam Goad — permanent Intercept + NP Charge.
/// Rediseño v2: +3 Intercepción PERMANENTE (up +2; antes 6 «este turno») + 20 NP
/// (up +10; antes 15) — segunda fuente sostenida de Intercepción (hueco del checklist).</summary>
public sealed class AmalgamGoad() : MashShielderCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<InterceptPower>("Intercept", 3m),
        new DynamicVar("NpCharge", 20),
        new DynamicVar("AllyIntercept", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<InterceptPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InterceptPower>(Owner.Creature, DynamicVars["Intercept"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);

        // Co-op (la provocación convierte al grupo en una línea de contraataque): cada aliado vivo gana
        // Intercepción. La Carga NP se queda en Mash (es su recurso). En 1 jugador PlayerCreatures es
        // solo el Owner -> el foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await PowerCmd.Apply<InterceptPower>(ally, DynamicVars["AllyIntercept"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Intercept"].UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
