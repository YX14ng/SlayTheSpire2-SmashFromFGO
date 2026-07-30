using MegaCrit.Sts2.Core.Combat;
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
// IUsesTargetCurse: exime de la Sentencia (que consumía la Maldición del objetivo antes del OnPlay),
// para que el +NP condicional a "objetivo maldito" dispare en forma Reina/Invierno.
public sealed class RoyalPunishment() : MorganCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IUsesTargetCurse
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
        Curses.MostCursed(Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var cursed = Curses.Of(cardPlay.Target) > 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        if (cursed)
        {
            await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
