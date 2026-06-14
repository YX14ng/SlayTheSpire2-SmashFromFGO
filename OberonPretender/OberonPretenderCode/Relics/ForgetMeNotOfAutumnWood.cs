using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Nomeolvides del Bosque de Otoño (秋之森的勿忘草 / Forget-me-not of Autumn Wood) — reliquia STARTER
/// OCULTA (DESIGN-OBERON §7 #3), la mecanica de dupes FGO: en las recompensas de carta, renuncias a la
/// carta para tirar un dupe (50% +25% de piedad). Un dupe sube el nivel de NP de Oberon (contador, 1-5;
/// 6 con el Santo Grial), reforzando sus cartas-NP en +15% por nivel (<see cref="NpLevels.Scale"/>, que
/// las Desatadas ya leen). Patron MorganSummonSeal / SummonTicket de Mash.
/// </summary>
public sealed class ForgetMeNotOfAutumnWood : OberonRelic, INpLevelStore
{
    public const string DupeOptionId = "OBERON_DUPE";

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
