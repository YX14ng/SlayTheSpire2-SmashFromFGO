using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// MEMORY OF LONDINIUM (圣剑遥远梦之遗痕) — Aesc's real NP (min 70, consumes ALL):
/// AoE damage + Knight's Arms in hand. At 100+ consumed: Intangible 1 y +1 Arm.
/// Parche del juez P1 (rediseño v2): el Intangible se mueve al overcharge (tier
/// >= 100) — sin gate, ciclaba cada ~1.5 turnos como WraithForm gratis.
/// </summary>
public sealed class MemoryOfLondinium() : MorganCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public const int ChargeCost = 70;

    /// <summary>Tier de sobrecarga: con esta carga consumida o más, también da Intangible y +1 Arma.</summary>
    public const int OverchargeTier = 100;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(28m, ValueProp.Move),
        new CardsVar(2),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + bonus);

        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        if (tier >= OverchargeTier)
        {
            await PowerCmd.Apply<IntangiblePower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        var arms = DynamicVars.Cards.IntValue + (tier >= OverchargeTier ? 1 : 0);
        await Special.KnightsArm.AddToHand(Owner.Creature, arms);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
