using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Pluma del Contrato (契约之羽 / Feather of the Contract) — reliquia RARA (DESIGN-OBERON §7 #9): cuando
/// jugas una carta-NP (<see cref="IOberonNpCard"/>), removes 2 puntos de Deuda (el sueño desplegado salda
/// cuentas). Sinergia con el banco: el finisher que normalmente vacia el medidor (-> impago) ahora
/// limpia parte de la cuenta. Reuso de <see cref="DebtPower.Forgive"/>; sin ×global.
/// </summary>
public sealed class FeatherOfTheContract : OberonRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int DebtForgiven = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card is not IOberonNpCard) return;
        if (DebtPower.Of(Owner.Creature) <= 0) return;
        Flash();
        await DebtPower.Forgive(Owner.Creature, DebtForgiven);
    }
}
