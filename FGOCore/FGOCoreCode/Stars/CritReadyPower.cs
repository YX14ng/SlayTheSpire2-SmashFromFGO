using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>
/// Crítico Listo (暴击) — tu próximo ATAQUE (la carta entera, todos sus golpes)
/// hace daño ×2 y consume 1 stack. Un solo stack por carta: con varios en cola,
/// cada carta de Ataque consume el suyo (parche P8 del panel: sin ×4 en multi-hit).
/// Patrón NextAttackDoubleDamage de JeanneAlter, mejorado.
/// </summary>
public sealed class CritReadyPower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private CardModel? _activeCard;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || Amount <= 0 || cardSource == null) return 1m;
        // La primera carta de Ataque que pega reclama el crítico; sus demás golpes
        // también salen ×2, pero una segunda carta necesita su propio stack.
        _activeCard ??= cardSource;
        return cardSource == _activeCard ? 2m : 1m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activeCard == null || cardPlay.Card != _activeCard) return;
        _activeCard = null;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
