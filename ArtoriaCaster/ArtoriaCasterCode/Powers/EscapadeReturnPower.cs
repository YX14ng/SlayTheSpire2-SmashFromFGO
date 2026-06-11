using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Retorno de la Escapada — power interno de «Escapada de Verano»: al final de tu
/// turno volvés a la forma anterior y el power se remueve. El retorno usa source
/// null, así que NO cuenta como "cambiar de forma" (sin FormShiftedPower ni
/// IFormChangeListener) — anti-loop deliberado.
/// </summary>
public sealed class EscapadeReturnPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Forma a la que volver: true = Caster, false = Berserker.</summary>
    public bool ReturnToCaster { get; set; }

    /// <summary>★ extra al volver (carta mejorada).</summary>
    public int StarsOnReturn { get; set; }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;

        Flash();
        if (ReturnToCaster)
        {
            await FormSwitch.Enter<ProphecyCasterFormPower>(choiceContext, Owner, null);
        }
        else
        {
            await FormSwitch.Enter<SummerBerserkerFormPower>(choiceContext, Owner, null);
        }
        if (StarsOnReturn > 0)
        {
            await Stars.Gain(Owner, StarsOnReturn, null);
        }
        await PowerCmd.Remove(this);
    }
}
