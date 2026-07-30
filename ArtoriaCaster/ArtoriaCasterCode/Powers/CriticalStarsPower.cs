using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Adaptador de migración con el ID histórico de Estrellas de Caliburn. Está oculto y convierte
/// cualquier cantidad guardada en el banco global (1/2/3 → 10/20/30; 4–5 → 50; más → cap 100).
/// </summary>
public sealed class CriticalStarsPower : ArtoriaPower
{
    public const int Max = 12;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    private bool _migrating;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext context, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(context, power, amount, applier, cardSource);
        if (power == this && Amount > 0) await Migrate(context, cardSource);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner)
        {
            await Migrate(new BlockingPlayerChoiceContext(), cardPlay.Card);
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            await Migrate(new BlockingPlayerChoiceContext(), null);
        }
    }

    private async Task Migrate(PlayerChoiceContext context, CardModel? source)
    {
        if (_migrating || Amount <= 0) return;
        _migrating = true;
        var oldAmount = Amount;
        var converted = oldAmount switch
        {
            <= 0 => 0,
            <= 3 => oldAmount * 10,
            <= 5 => 50,
            _ => Math.Min(CritStarsPower.Max, oldAmount * 10)
        };
        await PowerCmd.Remove(this);
        if (converted > 0) await CritStars.Gain(context, Owner, converted, source);
        _migrating = false;
    }
}
