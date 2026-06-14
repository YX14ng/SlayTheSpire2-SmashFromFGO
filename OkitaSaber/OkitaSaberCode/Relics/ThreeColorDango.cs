using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Dango de Tres Colores (三色团子) — reliquia de TIENDA (DESIGN-OKITA §6.2): +1 al tope del *Aliento
/// y empezás cada combate con +2 *Aliento. (Adora los dulces y a los niños.)
///
/// El bono al tope NO se aplica con un comando: AlientoPower.Cap recorre las reliquias del owner que
/// implementan <see cref="IBreathCapBooster"/> y suma <see cref="ExtraBreathCap"/> dinámicamente —
/// por eso basta con tener la reliquia (sin tocar el power). El +2 inicial sí se otorga al iniciar
/// combate (vía el helper Aliento.Gain, que respeta el tope ya elevado por esta misma reliquia).
/// </summary>
public sealed class ThreeColorDango : OkitaRelic, IBreathCapBooster
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    private const int StartingBreath = 2;

    public int ExtraBreathCap => 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await Aliento.Gain(Owner.Creature, StartingBreath, null);
    }
}
