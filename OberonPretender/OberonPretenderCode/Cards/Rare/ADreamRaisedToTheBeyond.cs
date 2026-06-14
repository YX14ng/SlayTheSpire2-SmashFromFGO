using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Cuento de un Sueño Alzado hacia el Más Allá (向彼方高举的梦之话 / A Dream Raised to the Beyond) —
/// DESIGN-OBERON §6.4 (el nombre real del NP). 1⚡ Habilidad: +30 Carga NP; si hay un enemigo Dormido,
/// +30 más y robá 1 (up base +10). El payoff del arquetipo Sueño del Mundo. Glow si hay Dormido.
/// </summary>
public sealed class ADreamRaisedToTheBeyond() : OberonCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 30), new DynamicVar("SleepBonus", 30)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        if (Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature) > 0)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["SleepBonus"].IntValue, this);
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Charge"].UpgradeValueBy(10m);
}
