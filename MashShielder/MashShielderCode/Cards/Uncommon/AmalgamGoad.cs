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
        new DynamicVar("NpCharge", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<InterceptPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InterceptPower>(Owner.Creature, DynamicVars["Intercept"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Intercept"].UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
