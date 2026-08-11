using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Sello de Invocación de la Reina (女王的呼符) — starter relic, the FGO dupe
/// mechanic: at card rewards, give up the card to roll for a dupe (50% +25% pity).
/// A dupe raises Morgan's NP level (counter, 1-5; 6 with the Chalice), boosting her
/// NP cards by +15% per level. (Patrón SummonTicket de Mash.)
/// </summary>
public sealed class MorganSummonSeal : MorganRelic, INpLevelStore
{
    public const string DupeOptionId = "MORGAN_DUPE";

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
        // CardRewardAlternative.Generate TIRA con más de 2 alternativas (MAIN y BETA); con
        // Driftwood (Skip + Reroll) no entramos. No se pierde el gacha: al usar el reroll la
        // pantalla regenera las alternativas con CanReroll=false y la opción aparece ahí.
        if (alternatives.Count >= 2) return false;
        if (!NpLevels.CanLevelUp(Owner)) return false;

        alternatives.Add(new CardRewardAlternative(DupeOptionId, OnDupeRoll, PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private async Task OnDupeRoll()
    {
        if (await NpLevels.TryRollDupeWithConsolation(Owner))
        {
            Flash();
        }
    }
}
