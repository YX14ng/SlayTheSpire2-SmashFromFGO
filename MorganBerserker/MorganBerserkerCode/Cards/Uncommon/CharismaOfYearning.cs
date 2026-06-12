using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Carisma del Anhelo (渴望的魅力) — Morgan S1: 2 de Fuerza, 1 de Vulnerable a TODOS,
/// Carga NP +20. Rediseño v2: NP 10→20 (denominación "paquete de skill"). (up +1/+10)
/// </summary>
public sealed class CharismaOfYearning() : MorganCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>("Strength", 2m),
        new PowerVar<VulnerablePower>("Vulnerable", 1m),
        new DynamicVar("NpCharge", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await PowerCmd.Apply<VulnerablePower>(enemy, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
            }
        }
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
