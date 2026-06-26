using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Maquinaria Mística / Formal Craft (魔术礼装) — versión TURNO de la Regla de Oro: la Carga
/// NP que ganás ESTE turno se incrementa en <see cref="Ratio"/> (+30%). Espejo EXACTO de
/// <see cref="GoldenRulePower"/> (mismo guard <c>_amplifying</c>, mismo <c>ModifyAmount</c>
/// directo que NO re-dispara GaugeFilled ni re-amplifica, mismo respeto del tope dinámico),
/// salvo que se auto-remueve al final de tu turno (patrón <c>CommandBusterStrengthPower</c>).
/// Single: no apila el ratio (es un buff de turno, no un contador de combate).
/// Personal: no escala en multijugador.
/// </summary>
public sealed class FormalCraftPower : FGOCorePower
{
    public const decimal Ratio = 0.3m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _amplifying;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (_amplifying) return;
        if (power is not NpChargePower np || power.Owner != Owner) return;
        if (amount <= 0m) return;

        var extra = (int)System.Math.Floor(amount * Ratio);
        extra = System.Math.Min(extra, NpCharge.Max(Owner) - (int)np.Amount);
        if (extra <= 0) return;

        _amplifying = true;
        Flash();
        await PowerCmd.ModifyAmount(choiceContext, np, extra, Owner, cardSource);
        _amplifying = false;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side) await PowerCmd.Remove(this);
    }
}
