using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Engendrar Laḫmu — carta del MAZO ESPECIAL Bestia (<see cref="IBeastEphemeral"/>, Exhaust). 0⚡:
/// parí 2 Laḫmu para reponer la cría que la ventana va a devorar. Si el enjambre ya está al tope,
/// no se desperdicia: +1 Crianza (el Mar de Vida engorda lo que no puede parir).
/// </summary>
public sealed class SpawnLahmuBeast() : TiamatCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IBeastEphemeral
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 2),
        new DynamicVar("Nurture", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var spawned = await Lahmu.Spawn(Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        if (spawned < DynamicVars["Lahmu"].IntValue)
        {
            await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Lahmu"].UpgradeValueBy(1m);
}
