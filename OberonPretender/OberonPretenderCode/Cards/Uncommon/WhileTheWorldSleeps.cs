using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Mientras el Mundo Duerme (当世界沉睡 / While the World Sleeps) — DESIGN-OBERON §6.3. 1⚡ Habilidad:
/// +20 Carga NP; +20 más por cada enemigo Dormido (up base +10). El turno de sueño es para armar los
/// libros. Glow si hay Dormido.
/// </summary>
public sealed class WhileTheWorldSleeps() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Charge", 20), new DynamicVar("PerAsleep", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal =>
        Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var asleep = Sleep.SleepingEnemies(Owner.Creature.CombatState, Owner.Creature);
        var charge = DynamicVars["Charge"].IntValue + asleep * DynamicVars["PerAsleep"].IntValue;
        await NpCharge.Gain(Owner.Creature, charge, this);
    }

    protected override void OnUpgrade() => DynamicVars["Charge"].UpgradeValueBy(10m);
}
