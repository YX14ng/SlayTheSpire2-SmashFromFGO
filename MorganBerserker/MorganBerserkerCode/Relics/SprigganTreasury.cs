using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>Tesoro de Spriggan (斯普里根的宝库) — start each combat with 30 NP Charge.</summary>
public sealed class SprigganTreasury : MorganRelic
{
    public const int StartingCharge = 30;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", StartingCharge)];

    public override async Task BeforeCombatStartLate()
    {
        Flash();
        await NpCharge.Gain(Owner.Creature, StartingCharge, null);
    }
}
