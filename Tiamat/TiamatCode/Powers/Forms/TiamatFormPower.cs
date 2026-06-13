using BaseLib.Extensions;
using Tiamat.TiamatCode.Extensions;

namespace Tiamat.TiamatCode.Powers.Forms;

/// <summary>Base de las formas de Tiamat sobre el FormPower de FGOCore. Íconos en este mod.</summary>
public abstract class TiamatFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
