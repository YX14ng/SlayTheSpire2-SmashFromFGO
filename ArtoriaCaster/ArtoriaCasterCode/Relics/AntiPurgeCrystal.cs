using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Cristal de Anti-Purga — rare: entering an Elite or Boss combat: gain 1 Anti-Purga.
/// (~1⚡ only in the fights where the big hit exists; honest in swarms.)
/// </summary>
public sealed class AntiPurgeCrystal : ArtoriaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AntiPurgePower>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is RoomType.Elite or RoomType.Boss)
        {
            Flash();
            await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, 1m, Owner.Creature, null);
        }
    }
}
