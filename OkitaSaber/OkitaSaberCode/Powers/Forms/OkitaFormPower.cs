using BaseLib.Extensions;
using OkitaSaber.OkitaSaberCode.Extensions;

namespace OkitaSaber.OkitaSaberCode.Powers.Forms;

/// <summary>Base de las formas de Okita sobre el FormPower de FGOCore. Íconos en este mod
/// (espejo de TiamatFormPower). Okita tiene UNA sola forma: el clímax permanente «Flor del Bakumatsu».</summary>
public abstract class OkitaFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
