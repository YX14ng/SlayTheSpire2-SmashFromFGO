using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Amamantar Laḫmu — carta del MAZO ESPECIAL Bestia (<see cref="IBeastEphemeral"/>, Exhaust). 1⚡:
/// +3 Crianza (escala TODA la cría a la vez, así infla la mordida del enjambre y los Devorar de la
/// ventana). Si tenés 4+ Laḫmu, robá 1 para encadenar más cosecha dentro de los pocos turnos Bestia.
/// </summary>
public sealed class FeedLahmuBeast() : TiamatCard(1, CardType.Skill, CardRarity.Event, TargetType.Self), IBeastEphemeral
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Nurture", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Lahmu.Feed(choiceContext, Owner.Creature, DynamicVars["Nurture"].IntValue, this);
        if (Lahmu.Count(Owner.Creature) >= 4 && Owner.Creature.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner.Creature.Player);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Nurture"].UpgradeValueBy(1m);
}
