using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Copa de Vino del Rey (王之酒杯) — reliquia COMÚN (DESIGN-GILGAMESH §6). Al final de cada combate
/// (victoria): ganás 3 de Oro. El ingreso pasivo del Rey (~135/run ≈ Golden Idol; la inflación del Rey).
/// Patrón BowlerHat vanilla: <c>PlayerCmd.GainGold</c> en <see cref="AfterCombatVictory"/>.
/// </summary>
public sealed class KingsWineCup : GilgameshRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(3)];

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        Flash();
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
    }
}
