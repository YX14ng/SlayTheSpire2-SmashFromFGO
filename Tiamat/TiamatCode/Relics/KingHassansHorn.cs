using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Relics;

/// <summary>Cuerno de King Hassan — al iniciar el combate ganás Agallas (Guts): la 1ª vez que
/// caerías, revivís a 1 HP pariendo 3 Laḫmu. 1/combate. (La inmortalidad del Rey de la Montaña
/// girada a favor de la madre: solo "otorgándole la muerte" se la vence — y renace en cría.)</summary>
public sealed class KingHassansHorn : TiamatRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        Flash();
        await PowerCmd.Apply<MotherGutsPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}
