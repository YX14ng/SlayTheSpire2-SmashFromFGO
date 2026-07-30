using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FGOCore.FGOCoreCode.Stars;

/// <summary>
/// Crítico Listo: la próxima carta elegible critica sin gastar estrellas. Tiene prioridad
/// sobre el banco y un máximo global de tres cargas. La resolución vive en
/// <see cref="CriticalResolverPower"/>.
/// </summary>
public sealed class CritReadyPower : FGOCorePower
{
    public const int MaxStacks = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _isClamping;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(context, power, amount, applier, cardSource);
        if (power != this || _isClamping) return;

        if (Amount > MaxStacks)
        {
            _isClamping = true;
            await PowerCmd.ModifyAmount(context, this, MaxStacks - Amount, Owner, cardSource);
            _isClamping = false;
        }

        await Criticals.EnsureInstalled(context, Owner);
    }
}
