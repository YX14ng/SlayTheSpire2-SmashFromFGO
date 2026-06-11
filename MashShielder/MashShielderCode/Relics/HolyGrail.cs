using FGOCore.FGOCoreCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// 圣杯 (Santo Grial) — Palingenesis: surpass the maximum. On pickup, a large permanent
/// Max HP boost; while held, breaks the FGO-system caps: Bond can reach level 12
/// (+5 Max HP per extra level) and NP level can reach 6 (via FGOCore's ILimitBreaker).
/// </summary>
public sealed class HolyGrail : MashShielderRelic, ILimitBreaker
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(15m)];

    public int ExtraBondLevels => 2;

    public int ExtraNpLevels => 1;

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
}
