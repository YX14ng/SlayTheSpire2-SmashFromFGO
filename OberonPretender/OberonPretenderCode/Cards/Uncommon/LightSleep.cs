using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Sueño Ligero (浅眠 / Light Sleep) — DESIGN-OBERON §6.3. 1⚡ Habilidad: 8 de Bloqueo; si hay un
/// enemigo Dormido, robá 2 (up 11 Bloqueo). El payoff del arquetipo Sueño del Mundo. Glow si hay Dormido.
/// </summary>
public sealed class LightSleep() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new DynamicVar("Draw", 2)];

    protected override bool ShouldGlowGoldInternal =>
        Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature) > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
