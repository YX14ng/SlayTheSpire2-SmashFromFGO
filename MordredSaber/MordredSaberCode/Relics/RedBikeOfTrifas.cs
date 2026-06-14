using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Cards;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Moto Roja de Trifas (特里法斯的红摩托) — reliquia de TIENDA (DESIGN-MORDRED §6): tus cartas *Quick
/// (<see cref="IQuickCard"/>, la verde de comando) otorgan +<see cref="ExtraStars"/> Estrellas de
/// Crítico adicionales. Es la pasiva Riding B mecanizada (sabe montar de todo). Reúso puro del hilo
/// de Estrellas — sube SOLO la sub-familia Quick, sin ×global ni motor nuevo.
/// </summary>
public sealed class RedBikeOfTrifas : MordredRelic
{
    private const int ExtraStars = 10;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not IQuickCard) return;
        Flash();
        await CritStars.Gain(Owner.Creature, ExtraStars, null);
    }
}
