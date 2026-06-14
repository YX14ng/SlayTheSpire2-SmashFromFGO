using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using FGOCore.FGOCoreCode.Np;
using FGOCore.FGOCoreCode.Stars;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Trono del Observador (旁观者之座) — DESIGN-GILGAMESH §5.3. La arrogancia como motor; la presión lo
/// interrumpe. Al final de tu turno, si NO defendiste (Owner.Block == 0): +<see cref="Stars"/> Estrellas
/// de Crítico y +<see cref="Np"/> Carga NP. Defender (jugar Bloqueo) apaga el dividendo de ese turno.
///
/// Campos públicos seteables <see cref="Stars"/>/<see cref="Np"/>: la carta los fija desde sus
/// DynamicVars al aplicar, para que el up 10→15 se refleje sin chocar con el conteo de stacks (patrón
/// WeightOfExpectationsPower de Siegfried). Buff visible, Single, personal (no escala en MP).
/// </summary>
public sealed class ThroneOfTheOnlookerPower : GilgameshPower
{
    public int Stars = 10;
    public int Np = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side && Owner.Block == 0)
        {
            Flash();
            await CritStars.Gain(Owner, Stars, null);
            await NpCharge.Gain(Owner, Np, null);
        }
    }
}
