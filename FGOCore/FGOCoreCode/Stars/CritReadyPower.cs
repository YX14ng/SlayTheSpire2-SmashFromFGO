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

    // Lectura PURA: ModifyDamage* lo invoca el juego también en PREVIEWS de daño (hover/targeting
    // de cartas en mano, sin previewMode en la firma), así que NO se puede mutar estado acá — el
    // latch anterior (_activeCard ??= cardSource) se enganchaba a la carta equivocada en preview.
    // Patrón vanilla DoubleDamagePower: ×2 mientras Amount>0; cada carta de Ataque jugada consume
    // 1 stack en AfterCardPlayed (multi-hit = todos los golpes ×2 por una sola carga; sin ×4).
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        => (dealer == Owner && props.IsPoweredAttack() && Amount > 0) ? 2m : 1m;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (Amount <= 0 || cardPlay.Card.Type != CardType.Attack) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
