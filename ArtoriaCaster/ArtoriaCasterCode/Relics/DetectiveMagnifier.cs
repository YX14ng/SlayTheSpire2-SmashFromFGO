using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Lupa de la Detective del Verano — rare: your Criticals deal +2 damage and grant
/// NP Charge +3 when the stars are consumed (sews ★→NP: cashing stars also charges
/// the ult, fixing the «always cross 100 in Caster» bias).
/// </summary>
public sealed class DetectiveMagnifier : ArtoriaRelic, ICritListener, ICritDamageBoost
{
    public const int Bonus = 2;
    public const int NpPerCrit = 3;

    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public int CritDamageBonus => Bonus;

    public async Task AfterCritConsumed(int starsSpent)
    {
        Flash();
        await NpCharge.Gain(Owner.Creature, NpPerCrit, null);
    }
}
