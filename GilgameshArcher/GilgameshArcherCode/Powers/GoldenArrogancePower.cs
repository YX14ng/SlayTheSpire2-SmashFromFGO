using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Arrogancia Dorada (黄金傲慢) — DESIGN-GILGAMESH §5.3. El plan B contra la presión (lección 焰刑地狱):
/// la 1ª vez que perdés Vida cada turno, +<see cref="Stars"/> Estrellas de Crítico (tope 1 activación
/// por turno). Sangrar alimenta el juicio. Campo público seteable <see cref="Stars"/>: la carta lo fija
/// desde su DynamicVar al aplicar (patrón WeightOfExpectationsPower).
///
/// Detección por daño no bloqueado al dueño (<see cref="AfterDamageReceived"/>, <c>result.UnblockedDamage
/// &gt; 0</c>), con flag <see cref="_triggeredThisTurn"/> reseteado al inicio de tu turno
/// (<see cref="AfterSideTurnStart"/>) — mismo patrón que QueensScepter. Buff visible, Single, personal
/// (no escala en MP).
/// </summary>
public sealed class GoldenArrogancePower : GilgameshPower
{
    public int Stars => Math.Max(20, FgoCombatState.GetCombat(Owner, 7, 6));

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public Task Configure(PlayerChoiceContext context, int stars, CardModel source)
    {
        return FgoCombatState.SetCombat(context, Owner, 7, Math.Max(Stars, stars), source, width: 6);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0 || FgoCombatState.GetTurn(Owner, 0) != 0) return;
        await FgoCombatState.SetTurn(choiceContext, Owner, 0, 1, cardSource);
        Flash();
        await CritStars.Gain(choiceContext, Owner, Stars, null);
    }
}
