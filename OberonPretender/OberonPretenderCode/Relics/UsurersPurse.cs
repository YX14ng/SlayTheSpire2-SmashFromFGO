using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Monedero del Prestamista (放贷人的钱袋 / Usurer's Purse) — reliquia de TIENDA (DESIGN-OBERON §7 #5):
/// tus cartas de *Prestamo (<see cref="ILoanCard"/>) otorgan ademas +10 Estrellas de Critico. Premia el
/// arquetipo Banca al Dia. Reuso puro del hilo de Estrellas — sube SOLO la sub-familia Prestamo, sin
/// ×global. Patron RedBikeOfTrifas.
/// </summary>
public sealed class UsurersPurse : OberonRelic
{
    private const int ExtraStars = 10;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not ILoanCard) return;
        Flash();
        await CritStars.Gain(Owner.Creature, ExtraStars, null);
    }
}
