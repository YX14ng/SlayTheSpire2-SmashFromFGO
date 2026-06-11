using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MorganBerserker.MorganBerserkerCode.Character;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Juramento a la Reina (向女王宣誓效忠) — Morgan's bond gauge on FGOCore's BondRelic.
/// Default numeric spine (Max HP at 1/3/6/9, starting NP at 2/5/8) with two Morgan
/// twists: the Block gifts become Curse-at-combat-start (Lv4: 1 to ALL, Lv7: 2 — the
/// Queen is no tank), and the Lv10 capstone «Un hogar con Morgan» (想和摩根组建家庭):
/// start every combat with Guts (Alzarse) — she finally has something to live for.
/// </summary>
[Pool(typeof(MorganRelicPool))]
public sealed class MorganBond : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Art lives in MorganBerserker's resources, not FGOCore's.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    // Morgan no es tanque: los regalos de Bloqueo default se reemplazan por Maldición.
    protected override int StartingBlock(int lv) => 0;

    private int StartingCurse(int lv) => lv >= 7 ? 2 : lv >= 4 ? 1 : 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();

        var curse = StartingCurse(Level);
        if (curse <= 0) return;
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(enemy, curse, Owner.Creature, null);
            }
        }
    }

    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<GutsPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}
