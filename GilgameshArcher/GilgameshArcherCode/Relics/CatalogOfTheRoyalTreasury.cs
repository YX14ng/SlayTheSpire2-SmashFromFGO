using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Catálogo de la Tesorería Real (王室宝库目录) — STARTER OCULTA (DESIGN-GILGAMESH §6), la mecánica de dupes
/// FGO sobre <see cref="INpLevelStore"/>: en las recompensas de carta podés ceder la carta para tirar por
/// un dupe (50% base +25% de pity, reset al acertar). Un dupe sube el nivel de NP de Gilgamesh (1-5; 6 con
/// el Cáliz Original), reforzando sus cartas-NP +15%/nivel vía <c>NpLevels.Scale</c>. Patrón
/// MorganSummonSeal / SummoningSealSaberOfRed / SummonTicket. NpLevel/DupePity con [SavedProperty].
/// </summary>
public sealed class CatalogOfTheRoyalTreasury : GilgameshRelic, INpLevelStore
{
    public const string DupeOptionId = "GILGAMESH_DUPE";

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
        // La pantalla soporta como máximo 2 alternativas (Saltar + una más).
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
