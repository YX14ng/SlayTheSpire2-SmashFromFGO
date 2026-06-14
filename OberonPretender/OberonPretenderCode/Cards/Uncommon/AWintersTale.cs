using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Cuento del Invierno (冬天的故事 / A Winter's Tale) — DESIGN-OBERON §6.3 (loc key A_WINTERS_TALE).
/// 2⚡ Habilidad: 13 de Bloqueo; si tenés Deuda, +4 (up 17 / +5). El defensivo con lector de Deuda. Glow
/// si tenés Deuda.
/// </summary>
public sealed class AWintersTale() : OberonCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int DebtBonus = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(13m, ValueProp.Move), new DynamicVar("Bonus", DebtBonus)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = DebtPower.Of(Owner.Creature) > 0 ? DynamicVars["Bonus"].IntValue : 0;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + bonus, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
