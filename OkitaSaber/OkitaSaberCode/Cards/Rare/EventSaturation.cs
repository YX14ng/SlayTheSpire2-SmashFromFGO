using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Saturación de Eventos (事象饱和) — DESIGN-OKITA §5.4. 2⚡ Hab, Exhaust: aplica
/// <see cref="EventSaturationPower"/> (este turno tus Ataques IGNORAN Bloqueo) (up: coste 1⚡).
/// La paradoja como skill; anti torre-de-bloqueo.
/// </summary>
public sealed class EventSaturation() : OkitaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EventSaturationPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1); // 2 -> 1
}
