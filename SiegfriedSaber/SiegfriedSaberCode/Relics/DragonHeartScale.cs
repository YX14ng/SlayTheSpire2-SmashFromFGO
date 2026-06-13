using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Escama del Corazón de Dragón (龙心之鳞) — al iniciar combate, +1 Sangre de Dragón. Se SUMA a la SdD 2
/// de la Hoja de Tilo → arrancás en 3, justo el umbral SdD≥3 que afila a Balmung y activa los riders
/// condicionales. NO genera NP (P2). Reúso puro de DragonScalesPower.
/// </summary>
public sealed class DragonHeartScale : SiegfriedRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    private const int ScalesGain = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, ScalesGain, Owner.Creature, null);
    }
}
