using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>
/// Apertura del Tesoro / 宝库开门 (DESIGN-GILGAMESH §5.5) — el generador de Armas DRAFTEABLE de medio
/// juego, el que sostiene la tribu fuera de las 2 Puertas iniciales. 1⚡ Hab: manifiesta a tu mano
/// <see cref="Arms"/> (2) Armas del Tesoro al azar y ganás <see cref="TreasureGain"/> (3) de Tesoro
/// (recarga el medidor de los riders «Pagá X de Oro»). up: +1 Arma.
///
/// Mismo motor que la Puerta de Babilonia (<see cref="TreasureDeck.ManifestRandom"/>); además alimenta
/// el módulo Tesoro, así que cierra el loop Armas ↔ Tesoro de Gil.
/// </summary>
public sealed class OpeningOfTheTreasury() : GilgameshCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int Arms = 2;
    private const int TreasureGain = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Arms", Arms), new DynamicVar("Treasure", TreasureGain)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await TreasureDeck.ManifestRandom(Owner.Creature, DynamicVars["Arms"].IntValue);
        await TreasurePower.Add(choiceContext, Owner.Creature, DynamicVars["Treasure"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Arms"].UpgradeValueBy(1m);
}
