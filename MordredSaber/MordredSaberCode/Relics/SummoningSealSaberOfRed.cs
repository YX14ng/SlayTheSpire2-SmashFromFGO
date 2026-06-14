using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Sello de Invocación: Saber of Red (召唤刻印·红Saber) — STARTER OCULTA (DESIGN-MORDRED §6), la
/// mecánica de dupes FGO sobre <see cref="INpLevelStore"/>: en las recompensas de carta podés ceder
/// la carta para tirar por un dupe (50% base +25% de pity, reset al acertar). Un dupe sube el nivel
/// de NP de Mordred (contador, 1-5; 6 con el Santo Grial de la Selección), reforzando sus cartas-NP
/// +15%/nivel vía <c>NpLevels.Scale</c>. Patrón MorganSummonSeal / ProphecyChildTalisman / SummonTicket.
/// </summary>
public sealed class SummoningSealSaberOfRed : MordredRelic, INpLevelStore
{
    public const string DupeOptionId = "MORDRED_DUPE";

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
