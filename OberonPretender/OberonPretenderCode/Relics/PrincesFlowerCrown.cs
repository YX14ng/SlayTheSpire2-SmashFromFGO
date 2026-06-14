using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Cards;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Corona de Flores del Principe (王子的花冠 / Prince's Flower Crown) — reliquia POCO COMUN
/// (DESIGN-OBERON §7 #7): la PRIMERA carta de *Prestamo (<see cref="ILoanCard"/>) de cada COMBATE NO
/// genera Deuda (la primera dosis es gratis — el anzuelo del prestamista). Tope ESTRUCTURAL 1/combate.
///
/// La carta-prestamo aplica su Deuda en su propio OnPlay (y el endulzante del Rey del Cuento puede sumar
/// +1); por eso esta reliquia toma una foto de la Deuda en <see cref="BeforeCardPlayed"/> y condona TODO
/// lo que ese primer prestamo añadio en <see cref="AfterCardPlayed"/> (incluido el endulzante: el
/// anzuelo cubre la dosis entera). Un solo proc por combate (flag de codigo, no "vigilar").
/// </summary>
public sealed class PrincesFlowerCrown : OberonRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _usedThisCombat;
    private bool _pending;
    private int _debtBefore;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        _pending = false;
        return base.BeforeCombatStartLate();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!_usedThisCombat && cardPlay.Card.Owner == Owner && cardPlay.Card is ILoanCard)
        {
            _pending = true;
            _debtBefore = DebtPower.Of(Owner.Creature);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!_pending) return;
        _pending = false;
        _usedThisCombat = true;

        // Condona todo lo que este primer prestamo añadio (base + endulzante): la primera dosis es gratis.
        var added = DebtPower.Of(Owner.Creature) - _debtBefore;
        if (added > 0)
        {
            Flash();
            await DebtPower.Forgive(Owner.Creature, added);
        }
    }
}
