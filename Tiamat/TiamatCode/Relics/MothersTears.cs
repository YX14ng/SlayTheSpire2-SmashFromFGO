using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace TiamatBeast.TiamatCode.Relics;

/// <summary>Lágrimas de la Madre — al Devorar Laḫmu, curás HP por cada larva sacrificada (la
/// madre llora a sus hijos y esa pena la sostiene). Engancha por <see cref="ILahmuDevourListener"/>.</summary>
public sealed class MothersTears : TiamatRelic, ILahmuDevourListener
{
    public const int HealPerLahmu = 2;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task OnLahmuDevoured(Creature devourer, int eaten)
    {
        Flash();
        await CreatureCmd.Heal(devourer, HealPerLahmu * eaten);
    }
}
