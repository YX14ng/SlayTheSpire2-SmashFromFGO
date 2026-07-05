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

    private decimal _pendingTransfer;

    // Guard de re-entrancia (audit 2026-07-04): dos Mash en co-op con Cobertura MUTUA (A cubre a B y
    // B cubre a A) recursionaban hasta colgar el juego — el traspaso de A es un Damage que el Cover de
    // B vuelve a anular y traspasar, y así al infinito. Mientras un traspaso está en vuelo, NINGÚN
    // CoverPower cubre (estático per-cliente: el traspaso corre inline en el flujo sincronizado).
    private static bool _transferring;

    private bool Covers(Creature target, Creature? dealer) =>
        !_transferring &&
        target != Owner && target.IsPlayer && !target.IsDead &&
        dealer != null && dealer.IsMonster && !Owner.IsDead;

    // OJO: los hooks ModifyHpLost* devuelven el monto ABSOLUTO resultante (no un delta).
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!Covers(target, dealer) || amount <= 0) return amount;
        // Asignar, no acumular (audit 2026-07-05): AfterDamageReceivedLate corre por evento de dano;
        // si algun evento no la consumio, un += arrastraba monto viciado al proximo traspaso.
        _pendingTransfer = amount;
        return 0m;
    }

    public override async Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_pendingTransfer <= 0 || !Covers(target, dealer)) return;

        var dmg = _pendingTransfer;
        _pendingTransfer = 0;
        Flash();
        _transferring = true;
        try
        {
            await CreatureCmd.Damage(choiceContext, Owner, dmg, ValueProp.Move, dealer, null);
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
            _pendingTransfer = 0; // red de seguridad: nada viciado sobrevive a la expiracion
            await PowerCmd.Remove(this);
        }
    }
}
