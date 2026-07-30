using System.Collections.Generic;
using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Marea Estancada — mientras está activa, las Maldiciones sobre tus enemigos NO decaen tras
/// pegar (implementa <see cref="ICursePreserver"/>, leído por <c>CursePower</c>). Es de UN turno:
/// se auto-remueve al inicio de tu próximo turno (espeja <c>NpResolvedThisTurnPower</c>). La marea
/// se estanca: el campo de Maldición queda congelado un turno antes de seguir corriendo.
/// </summary>
public sealed class TidalStasisPower : TiamatPower, ICursePreserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // El decaimiento de la Maldición ocurre en AfterSideTurnStart del lado enemigo; congelarlo
        // dura "este turno" del jugador, así que removemos al empezar el SIGUIENTE turno del dueño.
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
