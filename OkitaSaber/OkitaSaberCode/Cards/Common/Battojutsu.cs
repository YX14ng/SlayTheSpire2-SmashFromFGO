using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Battōjutsu (拔刀术) — DESIGN-OKITA §5.2. 1⚡ At: 6 daño; si es tu PRIMER Ataque del turno: +10★
/// (up: 9 daño / +20★). Glow cuando es el primer Ataque. Tribu iai (injerto B); lee
/// <see cref="AttacksThisTurnPower"/> (0 ⇒ este es el primero).
/// </summary>
public sealed class Battojutsu() : OkitaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    private bool IsFirstAttack => AttacksThisTurnPower.PlayedBefore(Owner.Creature) == 0;

    protected override bool ShouldGlowGoldInternal => IsFirstAttack;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var first = IsFirstAttack;
        await AttacksThisTurnPower.EnsureInstalled(Owner.Creature);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (first) await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
