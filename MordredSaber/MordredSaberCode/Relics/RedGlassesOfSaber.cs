using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Gafas Rojas de Saber (红Saber的眼镜) — reliquia POCO COMÚN (DESIGN-MORDRED §6): al iniciar cada
/// combate, +<see cref="StartingStars"/> Estrellas de Crítico (el costume Glasses Spiritron). Arranque
/// de munición ★ para el primer Crítico Listo. Reúso puro del hilo de Estrellas, sin ×global.
/// </summary>
public sealed class RedGlassesOfSaber : MordredRelic
{
    private const int StartingStars = 20;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await CritStars.Gain(Owner.Creature, StartingStars, null);
    }
}
