using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using ShutenDouji.ShutenDoujiCode.Styles;

namespace ShutenDouji.ShutenDoujiCode.Cards.Special;

public interface IMountOoeRecipeChoice
{
    bool BrewSake { get; }
}

public abstract class MountOoeRecipeChoice(bool brewSake) :
    ShutenCard(0, CardType.Skill, CardRarity.Event, TargetType.None, ShutenStyle.Caster),
    IMountOoeRecipeChoice
{
    public bool BrewSake { get; } = brewSake;
    protected override Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) => Task.CompletedTask;
}

public sealed class BrewMountOoeSake() : MountOoeRecipeChoice(true);
public sealed class DistillMountOoePoison() : MountOoeRecipeChoice(false);
