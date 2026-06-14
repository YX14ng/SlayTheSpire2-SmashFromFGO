using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Cards;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// El Asiento Vacío de la Mesa Redonda (圆桌的空席) — reliquia RARA (DESIGN-MORDRED §6): cada carta-NP
/// que jugás (<see cref="IMordredNpCard"/>: Clarent Blood Arthur en sus variantes) te da
/// +<see cref="StarsPerNp"/> Estrellas de Crítico — la silla que nunca le dieron, recobrada golpe a
/// golpe. Lee el marcador de carta-NP igual que DasRheingold de Siegfried; reúso puro del hilo de ★.
/// </summary>
public sealed class TheEmptySeatOfTheRoundTable : MordredRelic
{
    private const int StarsPerNp = 20;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not IMordredNpCard) return;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerNp, null);
    }
}
