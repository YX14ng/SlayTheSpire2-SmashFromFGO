using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #8 Castigo Real (女王的惩罚) — rediseño v2 (parche P7 del juez: un rider y medio,
/// no dos): 2⚡, 12 de daño + 1 Vulnerable; si el objetivo tiene Maldición: +10 NP
/// (la Corona cobra el castigo). Glow con enemigo maldito. (up: +4 daño, Vulnerable 1→2)
/// </summary>
public sealed class RoyalPunishment() : MorganCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new PowerVar<VulnerablePower>("Vulnerable", 1m),
        new DynamicVar("NpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CursePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed(Owner.Creature.CombatState, Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var cursed = Curses.Of(cardPlay.Target) > 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        if (cursed)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
