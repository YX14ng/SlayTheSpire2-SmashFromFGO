using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Abrigo de Plumas (羽衣 / Feather Coat) — DESIGN-OBERON §6.2. 1⚡ Habilidad: 5 de Bloqueo; en El
/// Príncipe del Invierno: +<see cref="WinterBonus"/> Bloqueo (up 8 base / +5 al rider). El payoff
/// defensivo de la forma Invierno (capa blanca de plumas): la estafa larga se cubre mejor. Lee
/// <c>HasPower<WinterPrincePower>()</c> directo. Glow en Invierno.
/// </summary>
public sealed class FeatherCoat() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int WinterBonus = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new DynamicVar("Bonus", WinterBonus)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<WinterPrincePower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = Owner.Creature.HasPower<WinterPrincePower>() ? DynamicVars["Bonus"].IntValue : 0;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + bonus, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
