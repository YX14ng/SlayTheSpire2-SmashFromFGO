using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Saberface (同脸, §5.3) — defensa→★ con sabor meme. La PRIMERA vez por turno que un enemigo te
/// golpea (alcance pleno), ganás +<see cref="StarsPerHit"/> Estrellas (up: y +<see cref="NpPerHit"/>
/// NP). El cap 1/turno sale de un flag propio reseteado en <see cref="BeforeSideTurnStart"/> (patrón
/// MadnessEnhancementPower). Los valores por activación son campos settables que fija la carta desde
/// sus DynamicVars; <see cref="Upgraded"/> habilita el +NP. Counter: las copias suman las ★.
/// </summary>
public sealed class SaberfacePower : MordredPower
{
    public int StarsPerHit = 10;
    public int NpPerHit = 10;

    public bool Upgraded;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _firedThisTurn;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _firedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_firedThisTurn || target != Owner || dealer == null || dealer == Owner || result.UnblockedDamage <= 0) return;
        _firedThisTurn = true;
        Flash();
        await CritStars.Gain(Owner, StarsPerHit * (int)Amount, null);
        if (Upgraded)
        {
            await NpCharge.Gain(Owner, NpPerHit, null);
        }
    }
}
