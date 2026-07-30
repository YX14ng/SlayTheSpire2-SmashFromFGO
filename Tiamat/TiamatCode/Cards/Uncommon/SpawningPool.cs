using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Poza de Desove — parí 2 Laḫmu y +1 Crianza. El motor de población intermedio entre
/// «Engendrar» (1, básica) y «Once Bel Laḫmu» (tope, rara): puebla el enjambre rápido para que la
/// mordida de Lily (Marea Voraz) y la cosecha Bestia tengan de dónde comer.</summary>
public sealed class SpawningPool() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 2),
        new DynamicVar("Nurture", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Lahmu.Spawn(choiceContext, Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        await Lahmu.Feed(choiceContext, Owner.Creature, DynamicVars["Nurture"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Lahmu"].UpgradeValueBy(1m);
}
