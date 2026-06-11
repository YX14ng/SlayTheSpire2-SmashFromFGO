using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Contraataque de la Guardiana — cada golpe enemigo anulado por completo: el
/// atacante recibe Amount de daño (máximo <see cref="MaxPerTurn"/> veces por turno).
/// Implementación DUAL por el quirk de Anti-Purga (ver AntiPurgePower): los golpes
/// detenidos por Bloqueo llegan con result.WasFullyBlocked en AfterDamageReceived;
/// las anulaciones de Anti-Purga que vanilla no contó llegan por
/// <see cref="IHitAnnulledListener"/> — sin doble conteo.
/// </summary>
public sealed class GuardiansCounterPower : ArtoriaPower, IHitAnnulledListener
{
    public const int MaxPerTurn = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private int _countersThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _countersThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack() || !result.WasFullyBlocked) return;
        await Counter(choiceContext, dealer);
    }

    public async Task AfterHitAnnulled(Creature attacker)
    {
        await Counter(new ThrowingPlayerChoiceContext(), attacker);
    }

    private async Task Counter(PlayerChoiceContext choiceContext, Creature attacker)
    {
        if (_countersThisTurn >= MaxPerTurn || Amount <= 0 || attacker.IsDead) return;
        _countersThisTurn++;
        Flash();
        await CreatureCmd.Damage(choiceContext, attacker, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
    }
}
