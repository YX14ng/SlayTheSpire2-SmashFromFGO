using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Limited/Zero Over (无限剑制) — ESTE turno, TODOS tus Ataques con poder hacen ×1.3 (+30%).
/// Se auto-remueve al final de tu turno (patrón <c>CommandBusterStrengthPower</c>), a
/// diferencia de Heaven's Feel que solo cubre el próximo. Single: no apila el multiplicador.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class ZeroOverPower : FGOCorePower
{
    public const decimal DamageMultiplier = 1.3m;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyDamageMultiplicativeFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 1m;
        return DamageMultiplier;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
