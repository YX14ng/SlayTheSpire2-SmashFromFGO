using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Esquirla del Oro del Rin (莱茵黄金碎片) — al iniciar combate, tu próxima carta-NP gana +20 al nivel de
/// Sobrecarga (=2 stacks de OverchargeBlessingPower). Se consume al jugar el NP (ConsumeAllForNpCard lo
/// lee y remueve). 1 carga/combate (no AFK). Sobre Balmung con SdD≥3 = +4 AoE a todos. Reúso puro.
/// </summary>
public sealed class RhinegoldShard : SiegfriedRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int BlessingStacks = 2; // TierPerStack=10 ⇒ +20 al tier

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<OverchargeBlessingPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner.Creature, BlessingStacks, Owner.Creature, null);
    }
}
