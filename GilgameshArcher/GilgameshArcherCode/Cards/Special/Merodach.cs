using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Merodach, la Espada Original / 始源之剑·玛尔杜克 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #1) — el
/// prototipo de Caliburn, la vanilla del arsenal. Token 0⚡ Exhaust generado a la mano por la Puerta de
/// Babilonia / Bab-ilu (no drafteable, <c>CardRarity.Event</c> — la rareza de los tokens generados, igual
/// que <c>KnightsArm</c> de Morgan y la propia Enuma; «Special» del diseño es la etiqueta, el enum real
/// es Event). 6 de daño.
///
/// Cada Arma del Tesoro registra su jugada en <see cref="ArmsPlayedPower"/> (los riders «si jugaste un
/// Arma» y el Botín del Conquistador lo leen) — hasta que exista el evento <c>Arsenal.WeaponPlayed</c> de
/// FGOCore, el registro lo hace la propia carta en OnPlay (regla §10).
/// </summary>
public sealed class Merodach() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
