using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using Tiamat.TiamatCode.Powers.Forms;

namespace Tiamat.TiamatCode.Relics;

/// <summary>
/// Útero del Mar de Vida — reliquia starter (el motor de Tiamat). Al iniciar cada combate
/// entrás en la forma Femme Fatale (la criadora) y parís 1 Laḫmu. (source=null en el
/// FormSwitch: fija la forma inicial sin contar como "cambio de forma" ni disparar bonus.)
/// </summary>
public sealed class SeaOfLifeWomb : TiamatRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LahmuSwarmPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await FormSwitch.Enter<TiamatFemmeFatalePower>(null, Owner.Creature, null);
        await Lahmu.Spawn(Owner.Creature, 1, null);
    }
}
