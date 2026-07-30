using AstolfoRider.AstolfoRiderCode.Caprice;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace AstolfoRider.AstolfoRiderCode.Cards.Special;

public interface ICapriceChoice { CommandType ChosenType { get; } }

public abstract class CapriceChoiceCard(CommandType chosen) :
    AstolfoCard(0, CardType.Skill, CardRarity.Event, TargetType.None), ICapriceChoice
{
    public CommandType ChosenType { get; } = chosen;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class ChooseQuickCaprice() : CapriceChoiceCard(CommandType.Quick);
public sealed class ChooseArtsCaprice() : CapriceChoiceCard(CommandType.Arts);
public sealed class ChooseBusterCaprice() : CapriceChoiceCard(CommandType.Buster);
