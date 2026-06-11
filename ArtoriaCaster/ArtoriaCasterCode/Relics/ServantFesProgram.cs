using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Cards;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Programa del ServantFes — rare: every time you play an NP card: gain 3 Critical
/// Stars (spending the whole gauge re-seeds the critical window — sews the two
/// engines together, 1-2 times per combat).
/// </summary>
public sealed class ServantFesProgram : ArtoriaRelic
{
    public const int StarsPerNp = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not IArtoriaNpCard) return;
        Flash();
        await Stars.Gain(Owner.Creature, StarsPerNp, null);
    }
}
