using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Chapa de la Primera Unidad (一番队队牌) — reliquia RARA (DESIGN-OKITA §6.2): al inicio de combates
/// contra Élites y Jefes, +20 *Estrellas y +2 *Aliento. (~el empujón para las peleas donde el pico de
/// gloria de verdad importa; honesta en hordas — no proca ahí.)
///
/// Detección del tipo de encuentro vía CombatState.Encounter.RoomType (patrón GuardiansExecution /
/// DragonTrait), leída en BeforeCombatStartLate (la encounter ya está montada al iniciar combate).
/// </summary>
public sealed class FirstUnitBadge : OkitaRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int Stars = 20;
    private const int Breath = 2;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<AlientoPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        if (Owner.Creature.CombatState?.Encounter?.RoomType is not (RoomType.Elite or RoomType.Boss)) return;
        Flash();
        await CritStars.Gain(Owner.Creature, Stars, null);
        await Aliento.Gain(Owner.Creature, Breath, null);
    }
}
