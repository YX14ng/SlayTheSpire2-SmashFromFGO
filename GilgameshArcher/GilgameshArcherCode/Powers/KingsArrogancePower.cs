using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Arrogancia del Rey (王之傲慢) — el RIESGO ESTRUCTURAL de Gilgamesh (P1 de la revisión Togawa §6):
/// un Rey no se digna a defender. Aplicado al iniciar cada combate por la starter Bab-ilu.
///   - Mientras NO hayas defendido este combate, tus Ataques hacen +<see cref="Bonus"/> de daño
///     (la arrogancia funciona — el desprecio pega).
///   - La PRIMERA vez que terminás un turno con Bloqueo (te rebajaste a defender como un mestizo),
///     la arrogancia se ROMPE para el resto del combate: el bonus desaparece y no vuelve.
/// Rankea la identidad (Presión/Ofensa &gt; Defensa, fiel al lore) y convierte «¿defiendo o no?» en
/// el eje de tensión del personaje, sin tocar el techo de daño (el bonus es plano y chico).
///
/// Aditivo (patrón <see cref="DivinityPower"/>): entra ANTES del ×2 del Crítico (el plano se duplica
/// con el juicio del Rey, fiel). Capa ofensiva personal: no escala en MP. Visible: el jugador debe
/// VER que tiene la ventaja y que defender la quema.
/// </summary>
public sealed class KingsArrogancePower : GilgameshPower
{
    public const int Bonus = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>¿Sigue arrogante? (no defendió aún este combate). Se apaga para siempre al defender.</summary>
    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (FgoCombatState.GetCombat(Owner, 0) != 0 || dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return Bonus;
    }

    // Si terminás un turno con Bloqueo, te rebajaste a defender → la arrogancia se rompe para siempre.
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner) && FgoCombatState.GetCombat(Owner, 0) == 0 && Owner.Block > 0)
        {
            await FgoCombatState.SetCombat(choiceContext, Owner, 0, 1);
            Flash();
        }
    }
}
