using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Desde el Confín del Mundo (来自止境) — Morgan's S3 Guts: when it fires, she rises
/// stronger — gain 3 Strength and NP Charge +50. ("Murió incontables veces y volvió.")
/// </summary>
public sealed class WorldsEndGutsPower : GutsPower
{
    public const int StrengthOnRise = 3;
    public const int NpOnRise = 50;

    // Icons live in MorganBerserker's resources, not FGOCore's.
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    protected override async Task OnTriggered(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<StrengthPower>(Owner, StrengthOnRise, Owner, null);
        await NpCharge.Gain(Owner, NpOnRise, null);
    }
}
