using BaseLib.Abstracts;
using FGOCore.FGOCoreCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;

namespace FGOCore.FGOCoreCode.Events;

/// <summary>
/// Act 2 event shared by every FGO character. The character relic pool supplies
/// its own ILimitBreaker, so FGOCore never needs to reference character assemblies.
/// </summary>
public sealed class HolyGrailRitual : CustomEventModel
{
    private const int GrailCost = 200;

    public override ActModel[] Acts => [ModelDb.Act<Hive>()];

    public override string? CustomInitialPortraitPath =>
        $"{MainFile.ResPath}/images/card_portraits/big/palingenesis.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(GrailCost)];

    public override bool IsAllowed(IRunState runState) =>
        runState.Players.All(player =>
            FindCharacterGrail(player) is not null &&
            player.Relics.All(relic => relic is not ILimitBreaker));

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        Player owner = Owner ?? throw new InvalidOperationException("Holy Grail event has no owner.");
        RelicModel? canonicalGrail = FindCharacterGrail(owner);

        if (canonicalGrail is null)
        {
            return
            [
                new EventOption(this, Decline,
                    $"{Id.Entry}.pages.INITIAL.options.DECLINE")
            ];
        }

        RelicModel grail = canonicalGrail.ToMutable();
        grail.Owner = owner;

        EventOption grailOption = owner.Gold >= GrailCost
            ? EventOption.FromRelic(
                grail,
                this,
                ApplyGrail,
                $"{Id.Entry}.pages.INITIAL.options.APPLY_GRAIL")
            : EventOption.FromRelic(
                grail,
                this,
                null,
                $"{Id.Entry}.pages.INITIAL.options.APPLY_GRAIL_LOCKED");

        return
        [
            grailOption,
            new EventOption(this, Decline,
                $"{Id.Entry}.pages.INITIAL.options.DECLINE")
        ];
    }

    private async Task ApplyGrail()
    {
        Player owner = Owner ?? throw new InvalidOperationException("Holy Grail event has no owner.");
        RelicModel? canonicalGrail = FindCharacterGrail(owner);

        // Defensive guards for resumed saves and unexpected external state changes.
        if (canonicalGrail is null || owner.Gold < GrailCost || owner.Relics.Any(relic => relic is ILimitBreaker))
        {
            SetEventFinished(PageDescription("UNAVAILABLE"));
            return;
        }

        await PlayerCmd.LoseGold(GrailCost, owner, GoldLossType.Spent);
        await RelicCmd.Obtain(canonicalGrail.ToMutable(), owner);
        SetEventFinished(PageDescription("APPLIED"));
    }

    private Task Decline()
    {
        SetEventFinished(PageDescription("DECLINED"));
        return Task.CompletedTask;
    }

    private static RelicModel? FindCharacterGrail(Player player) =>
        player.Character.RelicPool.AllRelics.FirstOrDefault(relic => relic is ILimitBreaker);
}
