using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>
/// El Arsenal del Rey / 王之宝库·齐开 (DESIGN-GILGAMESH §5.5) — el clímax del eje de tribu. 2⚡ Hab:
/// manifiesta a tu mano <see cref="Arms"/> (3) Armas del Tesoro al azar y ganás <see cref="NpGain"/> (15)
/// de Carga NP. Una mano llena de Armas 0⚡ encadena los riders «por cada Arma jugada este turno»
/// (Asalto del Tesoro Abierto, Andanada Total, El Tesoro Protege al Rey) para un turno explosivo. up:
/// +1 Arma.
///
/// Usa el mismo motor que la Puerta (<see cref="TreasureDeck.ManifestRandom"/>); el plus de NP empuja
/// hacia el siguiente pico de Enuma (que, además, exhausta las Armas sobrantes como Sobrecarga extra).
/// </summary>
public sealed class KingsArsenal() : GilgameshCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const int Arms = 3;
    private const int NpGain = 15;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Arms", Arms), new DynamicVar("Np", NpGain)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await TreasureDeck.ManifestRandom(Owner.Creature, DynamicVars["Arms"].IntValue);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Arms"].UpgradeValueBy(1m);
}
