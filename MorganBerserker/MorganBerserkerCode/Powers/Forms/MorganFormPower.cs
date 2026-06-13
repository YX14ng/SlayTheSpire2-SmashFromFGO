using BaseLib.Extensions;
using FGOCore.FGOCoreCode.Stars;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Base for Morgan's forms on top of FGOCore's generic FormPower.
/// Icons live in MorganBerserker's resources, not FGOCore's.
/// Implementa <see cref="IBanksCritStars"/>: Morgan SIEMPRE está en una forma, así que
/// sus Estrellas son un banco de gasto manual ("Crítico") en vez de auto-procar a 100.
/// </summary>
public abstract class MorganFormPower : FormPower, IBanksCritStars
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
