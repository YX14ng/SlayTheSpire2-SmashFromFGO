using BaseLib.Extensions;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Powers.Forms;

/// <summary>Base de las formas de Oberon sobre el FormPower de FGOCore. Iconos en este mod
/// (espejo de TiamatFormPower).</summary>
public abstract class OberonFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
