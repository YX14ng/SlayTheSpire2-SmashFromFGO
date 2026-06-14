using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Menkyo Kaiden del Tennen Rishin-ryū (天然理心流·免许皆传) — reliquia STARTER, la mecánica de
/// duplicados de FGO (DESIGN-OKITA §6.1). En las recompensas de carta podés renunciar a la carta para
/// tirar por un dupe (50% +25% de pity acumulado). Un dupe sube el nivel de NP de Okita (contador 1-5;
/// 6 con el Santo Grial), que potencia sus cartas-NP en +15%/nivel (NpLevels.Scale). Estándar del roster
/// (patrón MorganSummonSeal / SummonTicket de Mash).
/// </summary>
public sealed class MenkyoKaiden : OkitaRelic, INpLevelStore
{
    public const string DupeOptionId = "OKITA_DUPE";

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
        // La pantalla soporta como mucho 2 alternativas (Saltar + una más).
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
