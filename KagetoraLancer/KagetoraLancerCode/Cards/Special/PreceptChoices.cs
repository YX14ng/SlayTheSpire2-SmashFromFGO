using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Special;

public interface IPreceptChoice { Precept ChosenPrecept { get; } }

public abstract class PreceptChoiceCard(Precept chosen) :
    KagetoraCard(0, CardType.Skill, CardRarity.Event, TargetType.None), IPreceptChoice
{
    public Precept ChosenPrecept { get; } = chosen;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class ChooseHeaven() : PreceptChoiceCard(Precept.Heaven);
public sealed class ChooseChest() : PreceptChoiceCard(Precept.Chest);
public sealed class ChooseFeet() : PreceptChoiceCard(Precept.Feet);
