using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Cáliz del Mar Interior del Planeta — Castoria's Holy Grail (she was sent from
/// Avalon, the Inner Sea of the Planet): +15 Max HP on pickup; while held, limits
/// break: Bond can reach level 12 and NP level can reach 6 (FGOCore ILimitBreaker).
/// </summary>
public sealed class InnerSeaChalice : ArtoriaRelic, ILimitBreaker
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
