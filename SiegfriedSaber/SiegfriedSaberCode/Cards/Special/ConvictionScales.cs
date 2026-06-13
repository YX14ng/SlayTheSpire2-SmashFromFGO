using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

/// <summary>Opción "Endurecer" de Por Convicción Propia: +3 Sangre de Dragón. Token (no drafteable; sólo en
/// la pantalla de elección). Nunca se juega: su efecto va por <see cref="ApplyConviction"/>.</summary>
public sealed class ConvictionScales() : SiegfriedCard(0, CardType.Skill, CardRarity.Token, TargetType.Self), IConvictionOption
{
    public const int ScalesGain = 3;

    public async Task ApplyConviction() => await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, ScalesGain, Owner.Creature, this);

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
