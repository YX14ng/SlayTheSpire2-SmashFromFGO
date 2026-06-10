using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>Núcleo del Ortinax — start each combat with 40 NP Charge.</summary>
public sealed class OrtinaxCore : MashShielderRelic
{
    public const int StartingCharge = 40;

    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", StartingCharge)];

    public override async Task BeforeCombatStartLate()
    {
        Flash();
        await NpCharge.Gain(Owner.Creature, StartingCharge, null);
    }
}
