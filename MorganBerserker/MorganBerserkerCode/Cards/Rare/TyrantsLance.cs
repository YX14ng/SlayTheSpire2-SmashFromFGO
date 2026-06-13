using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Lanza de la Tirana (暴君之枪) — 24 de daño; pierdes 4 HP (→ +10★ vía el Cetro);
/// si tienes Alzarse: +10 de daño. "Crítico": el blanco ideal del ×2 — gastá 50★ y
/// este golpazo hace ×2 (el pico Buster de Morgan). Rediseño 2026-06-13.
/// </summary>
public sealed class TyrantsLance() : MorganCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new HpLossVar(4m),
        new DynamicVar("Bonus", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<GutsPower>(), HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<GutsPower>() || Crit.CanCrit(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await Crit.TrySpend(Owner.Creature, this);

        var damage = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.HasPower<GutsPower>())
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
