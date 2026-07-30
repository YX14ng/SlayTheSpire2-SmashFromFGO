using FGOCore.FGOCoreCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Relics;

/// <summary>
/// Tiamat's event-only Holy Grail. It performs Palingenesis on pickup and
/// unlocks the extended Bond and NP caps used by the shared FGO systems.
/// </summary>
public sealed class HolyGrailOfTheSeaOfLife : TiamatRelic, ILimitBreaker
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool IsAllowed(MegaCrit.Sts2.Core.Runs.IRunState runState) => false;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(15m)];

    public int ExtraBondLevels => 2;

    public int ExtraNpLevels => 1;

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
}
