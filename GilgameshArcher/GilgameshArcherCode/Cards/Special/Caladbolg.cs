using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Caladbolg, la Espada Espiral / 螺旋剑·光剑 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #2) — el original
/// perforante. Token 0⚡ Exhaust generado (<c>CardRarity.Event</c>). 5 de daño que IGNORA el Bloqueo
/// (Unblockable). El builder fluido no expone Unblockable, así que el golpe va por
/// <c>CreatureCmd.Damage</c> con la ValueProp explícita (patrón ConceptualRound / MumyouUnleashed).
/// </summary>
public sealed class Caladbolg() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move | ValueProp.Unblockable)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue,
            ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
