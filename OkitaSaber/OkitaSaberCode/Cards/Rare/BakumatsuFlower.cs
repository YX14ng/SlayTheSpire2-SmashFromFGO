using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;
using OkitaSaber.OkitaSaberCode.Powers.Forms;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER CLÍMAX «Flor del Bakumatsu» (幕末之华) — DESIGN-OKITA §5.4/§3.4. 2⚡ Poder, Exhaust: entra a la
/// forma final PERMANENTE (<see cref="BakumatsuFlowerPower"/>, modelo 102720 haori asagi): tus
/// RÁFAGAS dejan de costar *Aliento, pero al final de cada turno ganás 1 *Tos (up: coste 1⚡). El
/// único FormPower (§3.4): la enfermedad ya ganó, pelea igual. Entra vía <see cref="FormSwitch.Enter"/>.
/// </summary>
public sealed class BakumatsuFlower() : OkitaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FormSwitch.Enter<BakumatsuFlowerPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1); // 2 -> 1
}
