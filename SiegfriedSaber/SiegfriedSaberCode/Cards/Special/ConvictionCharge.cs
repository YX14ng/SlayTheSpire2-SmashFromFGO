using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

/// <summary>Opción "Carga" de Por Convicción Propia: +50 Carga NP. Token (no se draftea; sólo aparece en la
/// pantalla de elección). Nunca se juega: su efecto va por <see cref="ApplyConviction"/>.</summary>
public sealed class ConvictionCharge() : SiegfriedCard(0, CardType.Skill, CardRarity.Token, TargetType.Self), IConvictionOption
{
    public const int NpGain = 50;

    public async Task ApplyConviction() => await NpCharge.Gain(Owner.Creature, NpGain, this);

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
