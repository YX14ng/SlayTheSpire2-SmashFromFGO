using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Character;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Juramento del Peregrinaje — Castoria's bond gauge on FGOCore's BondRelic.
/// Default numeric spine (Max HP at 1/3/6/9, starting NP at 2/5/8) with her twist:
/// the Block gifts become starting Critical Stars (Lv4: 1★, Lv7: 2★ — her flavor is
/// the star engine), and the Lv10 capstone «La Estrella de la Esperanza»: start
/// every combat with 1 Anti-Purga.
/// </summary>
[Pool(typeof(ArtoriaRelicPool))]
public sealed class ArtoriaBond : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Art lives in ArtoriaCaster's resources, not FGOCore's.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<AntiPurgePower>()];

    // Castoria reparte estrellas, no Bloqueo plano.
    protected override int StartingBlock(int lv) => 0;

    private static int StartingStars(int lv) => lv >= 7 ? 2 : lv >= 4 ? 1 : 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();

        var stars = StartingStars(Level);
        if (stars > 0)
        {
            await Stars.Gain(Owner.Creature, stars, null);
        }
    }

    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, 1m, Owner.Creature, null);
    }
}
