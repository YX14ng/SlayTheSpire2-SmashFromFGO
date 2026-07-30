using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.SureHit;

/// <summary>
/// Certero de FGO: cada carga hace que el siguiente Ataque de carta del dueño ignore Bloqueo.
/// Se arma una vez por <see cref="AttackCommand"/> y se consume después de todos sus impactos.
/// </summary>
public sealed class SureHitPower : FGOCorePower
{
    private AttackCommand? _activeCommand;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner ||
            command.ModelSource is not CardModel ||
            !command.DamageProps.HasFlag(ValueProp.Move) ||
            command.DamageProps.HasFlag(ValueProp.Unpowered))
        {
            return Task.CompletedTask;
        }

        _activeCommand = command;
        command.WithValueProp(command.DamageProps | ValueProp.Unblockable);
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (_activeCommand != command) return;
        _activeCommand = null;
        await PowerCmd.Decrement(this);
    }
}

public static class SureHit
{
    public static Task Grant(
        PlayerChoiceContext context,
        Creature owner,
        int amount = 1,
        CardModel? source = null) =>
        amount <= 0
            ? Task.CompletedTask
            : PowerCmd.Apply<SureHitPower>(context, owner, amount, owner, source);
}
