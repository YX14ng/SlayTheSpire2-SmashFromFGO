using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Cobertura — until Mash's next turn, any damage that pierces an allied player's
/// defenses is taken by Mash instead (her own Block applies). Multiplayer mechanic:
/// the ally's HP loss is negated at the HP-loss stage and re-dealt to Mash right after.
/// </summary>
public sealed class CoverPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    private readonly Stack<PendingDamage> _candidates = [];
    private readonly Dictionary<Creature, Stack<PendingDamage>> _pendingTransfers = [];

    // Guard de re-entrancia (audit 2026-07-04): dos Mash en co-op con Cobertura MUTUA (A cubre a B y
    // B cubre a A) recursionaban hasta colgar el juego — el traspaso de A es un Damage que el Cover de
    // B vuelve a anular y traspasar, y así al infinito. Mientras un traspaso está en vuelo, NINGÚN
    // CoverPower cubre (estático per-cliente: el traspaso corre inline en el flujo sincronizado).
    private static bool _transferring;

    private bool Covers(Creature target, Creature? dealer) =>
        !_transferring &&
        target != Owner && target.IsPlayer && !target.IsDead &&
        dealer != null && dealer.IsMonster && !Owner.IsDead;

    // BeforeDamageReceived sólo corre durante una resolución real (no en previews). Guardamos allí
    // el daño entrante; ModifyHpLost queda puro y AfterModifying confirma únicamente al Cover que
    // efectivamente cambió el monto. Los stacks toleran daño reentrante.
    public override Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Covers(target, dealer) && amount > 0)
        {
            _candidates.Push(new PendingDamage(target, amount));
        }
        return Task.CompletedTask;
    }

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return Covers(target, dealer) && amount > 0 ? 0m : amount;
    }

    public override Task AfterModifyingHpLostBeforeOsty()
    {
        if (_candidates.TryPop(out var candidate))
        {
            if (!_pendingTransfers.TryGetValue(candidate.Target, out var transfers))
            {
                transfers = [];
                _pendingTransfers[candidate.Target] = transfers;
            }
            transfers.Push(candidate);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // Si este Cover no fue el que modificó el daño (otro Cover/protección llegó antes), su
        // candidato real sigue sin confirmar y se descarta al cerrar esta resolución.
        if (_candidates.TryPeek(out var candidate) && candidate.Target == target)
        {
            _candidates.Pop();
        }

        if (!_pendingTransfers.TryGetValue(target, out var transfers) || !transfers.TryPop(out var pending)) return;
        if (transfers.Count == 0) _pendingTransfers.Remove(target);
        var dmg = Math.Max(pending.Amount - result.BlockedDamage, 0m);
        if (dmg <= 0 || result.UnblockedDamage > 0 || !Covers(target, dealer)) return;

        Flash();
        _transferring = true;
        try
        {
            await CreatureCmdCompatibility.Damage(
                choiceContext,
                Owner,
                dmg,
                ValueProp.Move,
                dealer,
                cardSource,
                null);
        }
        finally
        {
            _transferring = false;
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Side != side)
        {
            _candidates.Clear();
            _pendingTransfers.Clear();
            await PowerCmd.Remove(this);
        }
    }

    private readonly record struct PendingDamage(Creature Target, decimal Amount);
}
