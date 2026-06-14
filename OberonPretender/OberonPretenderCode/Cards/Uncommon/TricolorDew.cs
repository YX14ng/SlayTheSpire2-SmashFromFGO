using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Rocío Tricolor (三色之露 / Tricolor Dew, love-in-idleness) — DESIGN-OBERON §6.3 (KIT Item
/// Construction A+). 1⚡ Habilidad · Exhaust: 2 Débil y 2 Vulnerable a un enemigo; +10 Carga NP (el rocío
/// que maldijo a Titania) (up 3/3). Con «Construcción de Ítems A+» activo cada debuff aplica un stack
/// extra (<see cref="ItemConstructionPower.ExtraDebuffStacks"/>); si su <see cref="ItemConstructionPower.RefundsCharge"/>
/// está encendido, devuelve +5 NP por cada uno de los dos debuffs aplicados.
/// </summary>
public sealed class TricolorDew() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>("Weak", 2m),
        new PowerVar<VulnerablePower>("Vulnerable", 2m),
        new DynamicVar("Charge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var extra = ItemConstructionPower.ExtraDebuffStacks(Owner.Creature);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue + extra, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue + extra, Owner.Creature, this);

        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);

        // Construcción de Ítems A+ mejorado: +5 NP por cada uno de los dos debuffs aplicados.
        var construction = Owner.Creature.GetPower<ItemConstructionPower>();
        if (construction is { RefundsCharge: true })
        {
            await NpCharge.Gain(Owner.Creature, 2 * ItemConstructionPower.ChargePerApply, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
