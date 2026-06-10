using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>Diario de la A-Team — every room you enter, gain 1 Max HP. Memories make her stronger.</summary>
public sealed class ATeamDiary : MashShielderRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(1m)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        Flash();
        await CreatureCmd.GainMaxHp(Owner.Creature, 1m);
    }
}
