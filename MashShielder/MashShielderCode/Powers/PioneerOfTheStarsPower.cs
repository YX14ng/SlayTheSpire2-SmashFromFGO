using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Espíritu Pionero de las Estrellas — the first MANUAL NP card each combat costs no
/// NP Charge. Parche P3 (implementado en FGOCore NpCharge.GetWaiver/ConsumeAllForNpCard):
/// el waiver NUNCA cubre las ults auto-manifestadas (CardRarity.Event) — así el marker
/// CamelotManifestedPower no queda atascado — y la carta cubierta resuelve a Sobrecarga
/// MÍNIMA (sin doble-dip de banco lleno + medidor intacto).
/// Rediseño v2: además, cada carta NP que juega la dueña (manual o Unleashed, marker
/// IMashNpCard) otorga Amount Estrellas de Crítico (30, up 50; las copias apilan).
/// </summary>
public sealed class PioneerOfTheStarsPower : MashShielderPower, INpCostWaiver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public bool Used { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not Cards.IMashNpCard || cardPlay.Card.Owner?.Creature != Owner) return;
        Flash();
        await FGOCore.FGOCoreCode.Stars.CritStars.Gain(Owner, (int)Amount, null);
    }
}
