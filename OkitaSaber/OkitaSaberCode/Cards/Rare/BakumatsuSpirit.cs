using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER «Espíritu del Bakumatsu» (幕末之魂) — DESIGN-OKITA §5.4. 3⚡ Poder: aplica
/// <see cref="BakumatsuSpiritPower"/> con Amount 1 (al inicio de cada turno: +1 *Aliento, +10★ y
/// +10 Carga NP) (up: coste 2⚡). Engorda los TRES hilos (patrón 龙之魔女).
/// </summary>
public sealed class BakumatsuSpirit() : OkitaCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AlientoPower>(),
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BakumatsuSpiritPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1); // 3 -> 2
}
