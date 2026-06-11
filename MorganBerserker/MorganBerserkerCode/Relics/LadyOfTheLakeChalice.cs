using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Cáliz de la Dama del Lago (水妃的圣杯) — Morgan's Holy Grail (Vivian's vessel):
/// +15 Max HP on pickup; while held, limits break: Bond can reach level 12 and
/// NP level can reach 6 (FGOCore ILimitBreaker).
/// </summary>
public sealed class LadyOfTheLakeChalice : MorganRelic, ILimitBreaker
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
