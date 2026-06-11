using BaseLib.Extensions;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Base for Morgan's forms on top of FGOCore's generic FormPower.
/// Icons live in MorganBerserker's resources, not FGOCore's.
/// </summary>
public abstract class MorganFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
