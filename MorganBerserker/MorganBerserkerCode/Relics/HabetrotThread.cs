using MegaCrit.Sts2.Core.Entities.Relics;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Hilo de Habetrot (哈贝特洛特的纺线) — when Guts (Alzarse) fires: you survive
/// at 10 HP instead of 1. The little fairy always leaves a thread to hold on to.
/// </summary>
public sealed class HabetrotThread : MorganRelic, IGutsFloorBooster
{
    public const int ImprovedFloor = 10;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public int ImproveFloor(int baseFloor) => ImprovedFloor;
}
