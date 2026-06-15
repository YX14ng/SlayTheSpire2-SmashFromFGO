using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Munición Conceptual — parche P8a del juez: hasta <c>Amount</c> veces por turno, cuando
/// juegas un Ataque contra un enemigo con buffs, elimina 1 buff ANTES de resolver el daño
/// (BeforeCardPlayed — sin cirugía de ValueProps; el «ignora Bloqueo» queda solo en las
/// cartas que ya nacen Black Barrel). Es el GENERADOR de la familia: dispara los payoffs
/// de strip (ConceptualRound/BlackBarrelBurst).
/// </summary>
public sealed class ConceptualAmmoPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int _usesThisTurn;

    protected override void OnPlayerTurnStartReset() => _usesThisTurn = 0;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (_usesThisTurn >= Amount) return;
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;

        var target = cardPlay.Target;
        if (target == null || target.IsDead || target.Side == Owner.Side) return;
        if (!target.GetPowerInstances<PowerModel>().Any(p => p.Type == PowerType.Buff)) return;

        _usesThisTurn++;
        Flash();
        await BlackBarrel.RemoveBuffs(target, 1);
    }
}
