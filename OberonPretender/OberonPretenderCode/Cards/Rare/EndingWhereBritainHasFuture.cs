using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// El Final donde Britania Tiene un Futuro (不列颠拥有未来的结局 / The Ending Where Britain Has a Future)
/// — DESIGN-OBERON §6.4, el reset. 2⚡ Habilidad · Exhaust: 25 de Bloqueo; remové hasta 3 puntos de Deuda
/// (condonación pura, no NP) (up 32 Bloqueo / hasta 5 Deuda). Aún traicionando, ejecuta el final bueno.
/// </summary>
public sealed class EndingWhereBritainHasFuture() : OberonCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(25m, ValueProp.Move), new DynamicVar("Debt", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override bool ShouldGlowGoldInternal => DebtPower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await DebtPower.Forgive(Owner.Creature, DynamicVars["Debt"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(7m);
        DynamicVars["Debt"].UpgradeValueBy(2m);
    }
}
