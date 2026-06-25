using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Lanza de la Tirana (暴君之枪) — 24 de daño; pierdes 4 HP (→ siembra 3 de Maldición a un
/// enemigo al azar vía el Cetro); si tienes Alzarse: +10 de daño. En forma Reina/Invierno la
/// pasiva "Sentencia" le suma la Maldición del objetivo y la consume. Rediseño 2026-06-15
/// (swap Estrellas→Maldición).
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
        [HoverTipFactory.FromPower<GutsPower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<GutsPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

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
