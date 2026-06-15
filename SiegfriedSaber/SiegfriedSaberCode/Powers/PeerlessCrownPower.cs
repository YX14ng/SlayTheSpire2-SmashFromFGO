using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Cleanse;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Corona del Sin Par (无双之冠) — el primer golpe que ATRAVIESA tu Sangre de Dragón cada turno (la espalda
/// expuesta) NO inflige daño y te limpia 1 Debuff (+5 NP si está mejorada). Arquitectura preview-safe en dos
/// partes: (a) <see cref="ModifyHpLostBeforeOsty"/> = LECTURA PURA que anula el golpe que VA a piercear
/// (espejo del predicado de DragonScalesPower, sin mutar); (b) <see cref="OnScalesPierced"/> (camino real del
/// daño, ≤1/turno) consume el cupo, limpia y da NP. P8: el cleanse es 1 Debuff, nunca Artifact.
/// </summary>
public sealed class PeerlessCrownPower : SiegfriedPower, IDragonScalePierceListener
{
    private const int NpOnTrigger = 5;

    public bool Upgraded; // lo fija la carta desde IsUpgraded

    private bool _consumedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || amount <= 0m || !props.IsPoweredAttack() || _consumedThisTurn) return amount;
        if (PierceWouldHappen(props, dealer))
        {
            Flash();
            return 0m; // la corona desvía el golpe fatal de la espalda expuesta
        }
        return amount;
    }

    public async Task OnScalesPierced(PlayerChoiceContext choiceContext)
    {
        if (_consumedThisTurn) return;
        _consumedThisTurn = true;
        Flash();
        await Cleanse.RemoveDebuffs(Owner, 1);
        if (Upgraded) await NpCharge.Gain(Owner, NpOnTrigger, null);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _consumedThisTurn = false;
        return Task.CompletedTask;
    }

    // Espejo PURO de DragonScalesPower.ShouldPierce (consulta los IDragonScalePiercer del dueño), sin mutar.
    private bool PierceWouldHappen(ValueProp props, Creature? dealer)
    {
        var relics = Owner.Player?.Relics;
        if (relics == null) return false;
        foreach (var relic in relics)
        {
            if (relic is IDragonScalePiercer piercer && piercer.ShouldPierceScales(props, dealer)) return true;
        }
        return false;
    }
}
