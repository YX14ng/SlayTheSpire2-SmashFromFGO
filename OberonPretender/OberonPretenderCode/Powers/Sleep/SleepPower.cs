using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OberonPretender.OberonPretenderCode.Powers.Sleep;

/// <summary>
/// Dormido (Sleep) -- DESIGN-OBERON 3.d. El demerito de Invencible del NP real fundido en un
/// estado: el enemigo NO actua en su proximo turno y TODO el dano que recibiria se anula (intocable,
/// en el mundo de los suenos); despierta al final de ese turno y gana Insomnio 2.
///
/// Arquitectura (los gotchas de 10.8):
/// - El "no actua" se aplica con <c>CreatureCmd.Stun</c> al DORMIRLO (helper <see cref="Sleep"/>),
///   no aca: este power solo gobierna la inmunidad y el despertar.
/// - La anulacion de dano va en <see cref="ModifyHpLostBeforeOsty"/> (hook ABSOLUTO, devuelve el monto
///   resultante, NO un delta -- patron GutsPower/PeerlessCrownPower) y SOLO sobre Ataques (Maldicion/
///   auto-dano pasan). Es lectura pura -> 0 (preview-safe).
/// - EXCEPCION Vortigern: si el atacante tiene un <see cref="ISleepIgnorer"/> que ignora a este
///   objetivo, el golpe NO se anula y NO despierta (el abismo devora en el sueno).
/// - Despierta al final del turno del dueno (<see cref="AfterTurnEnd"/>), decrementa y, al llegar a 0,
///   aplica Insomnio 2 (no re-dormible). Counter por si dos fuentes lo apilan.
/// </summary>
public sealed class SleepPower : OberonPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || amount <= 0m || !props.IsPoweredAttack()) return amount;
        if (dealer != null && IgnoredBy(dealer)) return amount; // Vortigern devora sin despertar
        Flash();
        return 0m; // intocable en el mundo de los suenos
    }

    // Lectura pura: el atacante tiene un power que ignora el Sueno de este objetivo?
    private bool IgnoredBy(Creature dealer)
    {
        foreach (var ignorer in Listeners.PowersOf<ISleepIgnorer>(dealer))
        {
            if (ignorer.IgnoresSleep(Owner)) return true;
        }
        return false;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        await PowerCmd.Decrement(this);
        if (Amount <= 0 && Owner.IsAlive)
        {
            Flash();
            await PowerCmd.Apply<InsomniaPower>(Owner, InsomniaPower.Duration, Owner, null);
        }
    }
}
