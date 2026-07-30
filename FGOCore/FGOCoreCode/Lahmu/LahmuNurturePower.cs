using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;

namespace FGOCore.FGOCoreCode.Lahmu;

/// <summary>
/// Crianza (養育 / Nurture) — el nivel GLOBAL de crianza del enjambre de Laḫmu (ver
/// <see cref="LahmuSwarmPower"/>). Un solo número que escala a TODAS las larvas por igual:
/// cada Laḫmu da bloque <c>2 + Crianza</c> y muerde por <c>1 + Crianza</c>. Las cartas de
/// ALIMENTAR la suben; es el motor de escalado real (parir sube el techo, alimentar el piso).
/// </summary>
public sealed class LahmuNurturePower : FGOCorePower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override string SmartDescriptionLocKey => $"{Id.Entry}.runtimeDescription";

    public override LocString Description
    {
        get
        {
            if (!IsMutable) return base.Description;

            var description = new LocString("powers", $"{Id.Entry}.smartDescription");
            description.Add("BlockPerLahmu", LahmuSwarmPower.BlockPerLahmu + Amount);
            description.Add("BitePerLahmu", LahmuSwarmPower.BitePerLahmu + Amount);
            return description;
        }
    }
}
