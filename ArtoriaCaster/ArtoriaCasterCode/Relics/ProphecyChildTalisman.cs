using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Talismán de la Niña de la Profecía — starter relic, the FGO dupe mechanic: at
/// card rewards, give up the card to roll for a dupe (50% +25% pity). A dupe raises
/// Castoria's NP level (counter, 1-5; 6 with the Chalice), boosting her NP cards by
/// +15% per level. (Patrón SummonTicket de Mash / Sello de Morgan.)
/// </summary>
public sealed class ProphecyChildTalisman : ArtoriaRelic, INpLevelStore
{
    public const string DupeOptionId = "ARTORIA_DUPE";

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
        return NpDupeAlternative.TryAdd(
            Owner, player, alternatives, DupeOptionId, OnDupeRoll);
    }

    private async Task OnDupeRoll()
    {
        if (await NpLevels.TryRollDupeWithConsolation(Owner))
        {
            Flash();
        }
    }
}
