using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// 呼符 (Boletos de Invocación) — starter relic, the FGO dupe mechanic: at card rewards,
/// an extra option lets Mash give up the card to roll the gacha for a dupe (50% +25%
/// pity per fail). A dupe raises her NP level (the counter, 1-5; 6 with the Holy Grail),
/// boosting her Noble Phantasm cards by +15% per level. Stores the level for the run.
/// </summary>
public sealed class SummonTicket : MashShielderRelic, INpLevelStore
{
    public const string DupeOptionId = "MASH_DUPE";

    private int _npLevel = 1;

    private int _dupePity;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool ShowCounter => true;

    public override int DisplayAmount => NpLevel;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    [SavedProperty]
    public int NpLevel
    {
        get => _npLevel;
        set
        {
            AssertMutable();
            _npLevel = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int DupePity
    {
        get => _dupePity;
        set
        {
            AssertMutable();
            _dupePity = value;
        }
    }

    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        if (Owner != player) return false;
        // The screen supports at most 2 alternatives (Skip + one more).
        if (alternatives.Count >= 2) return false;
        if (!NpLevels.CanLevelUp(Owner)) return false;

        alternatives.Add(new CardRewardAlternative(DupeOptionId, OnDupeRoll, PostAlternateCardRewardAction.DismissScreenAndRemoveReward));
        return true;
    }

    private Task OnDupeRoll()
    {
        if (NpLevels.TryRollDupe(Owner))
        {
            Flash();
        }
        return Task.CompletedTask;
    }
}
