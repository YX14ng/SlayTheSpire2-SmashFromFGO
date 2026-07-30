using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using ShutenDouji.ShutenDoujiCode.Styles;

namespace ShutenDouji.ShutenDoujiCode.Cards.Special;

public enum AntiquityChoice
{
    Np,
    Sake,
    Stars
}

public interface IAntiquityChoice
{
    AntiquityChoice Choice { get; }
}

public abstract class AntiquityChoiceCard(AntiquityChoice choice) :
    ShutenCard(0, CardType.Skill, CardRarity.Event, TargetType.None, ShutenStyle.Assassin),
    IAntiquityChoice
{
    public AntiquityChoice Choice { get; } = choice;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class ChooseAntiquityNp() : AntiquityChoiceCard(AntiquityChoice.Np);
public sealed class ChooseAntiquitySake() : AntiquityChoiceCard(AntiquityChoice.Sake);
public sealed class ChooseAntiquityStars() : AntiquityChoiceCard(AntiquityChoice.Stars);
