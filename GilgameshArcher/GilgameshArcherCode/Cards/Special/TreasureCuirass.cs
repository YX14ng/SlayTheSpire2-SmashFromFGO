using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Coraza del Tesoro / 宝库护甲 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #8) — hasta defender es disparar el
/// tesoro: la única Arma de Habilidad del arsenal, el plan B defensivo de la tribu. Token 0⚡ Exhaust
/// generado (<c>CardRarity.Event</c>). 5 de Bloqueo. Sigue contando como Arma jugada
/// (<see cref="ArmsPlayedPower"/>) para los riders y el Botín.
/// </summary>
public sealed class TreasureCuirass() : GilgameshCard(0, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
