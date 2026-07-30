using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AstolfoRider.AstolfoRiderCode.Cards.Special;

public enum AdventureBagChoice { Np, Stars, Caprice }
public interface IAdventureBagChoice { AdventureBagChoice Choice { get; } }

public abstract class AdventureBagChoiceCard(AdventureBagChoice choice) :
    AstolfoCard(0, CardType.Skill, CardRarity.Event, TargetType.None), IAdventureBagChoice
{
    public AdventureBagChoice Choice { get; } = choice;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class ChooseAdventureNp() : AdventureBagChoiceCard(AdventureBagChoice.Np);
public sealed class ChooseAdventureStars() : AdventureBagChoiceCard(AdventureBagChoice.Stars);
public sealed class ChooseAdventureCaprice() : AdventureBagChoiceCard(AdventureBagChoice.Caprice);
