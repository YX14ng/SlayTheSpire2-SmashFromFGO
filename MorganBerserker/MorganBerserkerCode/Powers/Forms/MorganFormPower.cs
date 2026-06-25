using BaseLib.Extensions;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Base for Morgan's forms on top of FGOCore's generic FormPower.
/// Icons live in MorganBerserker's resources, not FGOCore's.
/// Rediseño 2026-06-15: Morgan dejó el motor de Estrellas (queda en FGOCore para Mash/Castoria)
/// y volvió a su divisa temática, la Maldición. Por eso ya NO marca <c>IBanksCritStars</c>.
/// </summary>
public abstract class MorganFormPower : FormPower
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
