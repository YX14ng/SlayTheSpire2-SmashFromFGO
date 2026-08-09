using GilgameshArcher.GilgameshArcherCode.Cards;
using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Basic;

/// <summary>
/// Puerta de Babilonia / 王之财宝·门 (DESIGN-GILGAMESH §5.5) — la FIRMA del mazo inicial QAABB: la llave que
/// abre el arsenal. 1⚡ Hab: manifiesta a tu mano <see cref="Arms"/> (2) Armas del Tesoro AL AZAR (0⚡,
/// Exhaust — se esfuman al jugarse). Es el MOTOR de la tribu: sin ella, los riders «si jugaste un Arma»
/// y el Botín del Conquistador no tienen con qué disparar.
///
/// Token de comando del deck base (<c>CardRarity.Basic</c>, no drafteable, lleva el tag de comando
/// igual que Arts/Quick). El up sube a 3 Armas. Usa <see cref="TreasureDeck.ManifestRandom"/> (espejo
/// de <c>BeastDeck</c> de Tiamat + el RNG determinista de combate).
/// </summary>
public sealed class GateOfBabylon() : GilgameshCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self), BaseLib.Abstracts.ITranscendenceCard
{
    public MegaCrit.Sts2.Core.Models.CardModel GetTranscendenceTransformedCard() =>
        MegaCrit.Sts2.Core.Models.ModelDb.Card<Rare.KingsArsenal>();

    private const int Arms = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Arms", Arms)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await TreasureDeck.ManifestRandom(Owner.Creature, DynamicVars["Arms"].IntValue);
    }

    protected override void OnUpgrade() => DynamicVars["Arms"].UpgradeValueBy(1m);
}
