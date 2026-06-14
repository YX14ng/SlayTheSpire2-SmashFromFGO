using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers.Sleep;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Canción de Cuna del Fin (终焉的摇篮曲 / Lullaby of the End) — DESIGN-OBERON §6.4. 2⚡ Habilidad ·
/// Exhaust: un enemigo se DUERME (el stun disfrazado; respeta Insomnio vía <see cref="Sleep.TryApply"/>).
/// El up sube la duración 1 → 2.
/// </summary>
public sealed class LullabyOfTheEnd() : OberonCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Duration", Sleep.DefaultDuration)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Sleep.TryApply(cardPlay.Target, DynamicVars["Duration"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Duration"].UpgradeValueBy(1m);
}
