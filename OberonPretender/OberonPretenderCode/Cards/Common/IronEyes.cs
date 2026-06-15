using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Ojos Férricos (铁色之眼 / Iron Eyes, Fae Eyes) — DESIGN-OBERON §6.2. 0⚡ Habilidad: 1 Vulnerable a
/// un enemigo; +5 NP (up +10 NP). El debuff-feeder que alimenta «Construcción de Ítems A+» (+1 stack)
/// y «???»: con ItemConstructionPower activo el Vulnerable aplica un stack extra (lectura pura del
/// owner). El up sube el NP; el Vulnerable base queda en 1 (anti-apilado). Patrón Mongrel.
/// </summary>
public sealed class IronEyes() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>("Vulnerable", 1m),
        new DynamicVar("Np", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var vulnerable = DynamicVars["Vulnerable"].BaseValue + ItemConstructionPower.ExtraDebuffStacks(Owner.Creature);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, vulnerable, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);

        // Construcción de Ítems A+ mejorado: +5 NP por el debuff aplicado (paridad con Rocío Tricolor).
        var construction = Owner.Creature.GetPower<ItemConstructionPower>();
        if (construction is { RefundsCharge: true })
        {
            await NpCharge.Gain(Owner.Creature, ItemConstructionPower.ChargePerApply, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(5m);
}
