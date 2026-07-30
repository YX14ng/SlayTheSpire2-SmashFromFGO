using BaseLib.Extensions;
using FGOCore.FGOCoreCode;
using MashShielder.MashShielderCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// El Milagro de Fou — the first time this combat that damage would kill you,
/// you survive at this power's Amount HP, gain a burst of Block and NP Charge
/// (rediseño v2: +50 NP — el milagro arma la ult de la remontada).
///
/// Subclasea <see cref="GutsPower"/> (FGOCore) en vez de reimplementar el patrón Alzarse:
/// hereda el rescate al morir + los hooks ABSOLUTOS de ModifyHpLost* + el soporte de
/// <see cref="IGutsFloorBooster"/> (así el Holy Grail eleva su Vida de rescate). Solo
/// queda local: el override de <see cref="Floor"/> (la Vida de rescate la fija el Amount
/// del power, variable 1..12 por la carta/capstone — no el 1 fijo de Guts) y el bonus
/// específico del milagro (Block + NP) en <see cref="OnTriggered"/>.
/// </summary>
public sealed class FouMiraclePower : GutsPower
{
    public const int RescueBlock = 25;

    public const int RescueNpCharge = 50;

    // Carga el icono desde el .pck de MASHSHIELDER (no el de FGOCore, que es donde apuntaría
    // la base heredada). Replica el patrón de MashShielderPower con las extensiones de la mod.
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    // La Vida de rescate la fija el Amount del power (la carta FousMiracle / el capstone de MashBond
    // aplican >=1; con StackType.Single «el mayor gana», heredado de GutsPower). El base.Floor aporta
    // los IGutsFloorBooster (Holy Grail), así que tomamos el máximo entre ambos.
    protected override int Floor => Math.Max(base.Floor, Amount);

    // Bonus del milagro al gatillarse Alzarse: Baluarte de emergencia + carga para la ult de la remontada.
    protected override async Task OnTriggered(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.GainBlock(Owner, RescueBlock, ValueProp.Unpowered, null);
        await NpCharge.Gain(choiceContext, Owner, RescueNpCharge, null);
    }
}
