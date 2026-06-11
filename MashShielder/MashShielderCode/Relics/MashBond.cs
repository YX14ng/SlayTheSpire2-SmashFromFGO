using BaseLib.Extensions;
using BaseLib.Utils;
using MashShielder.MashShielderCode.Character;
using MashShielder.MashShielderCode.Extensions;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// 好感度 (Vínculo) — Mash's bond gauge, on FGOCore's BondRelic (default point sources,
/// thresholds and gift curves). Her Lv 10 capstone is her devotion: survive lethal
/// damage at 1 HP once per combat (Fou's Miracle).
/// </summary>
[Pool(typeof(MashShielderRelicPool))]
public sealed class MashBond : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Art lives in MashShielder's resources, not FGOCore's.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<FouMiraclePower>(Owner.Creature, 1, Owner.Creature, null);
    }
}
