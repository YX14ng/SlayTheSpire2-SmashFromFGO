using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Black Barrel — the Atlas Institute's conceptual cannon. Black Barrel damage
/// ignores enemy Block and strips one buff from the target ("kills the immortal").
/// </summary>
public static class BlackBarrel
{
    public static async Task Hit(PlayerChoiceContext choiceContext, Creature target, decimal amount, Creature dealer, CardModel source)
    {
        VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
        await CreatureCmd.Damage(choiceContext, target, amount, ValueProp.Move | ValueProp.Unblockable, dealer, source);
        await RemoveBuffs(target, 1);
    }

    public static async Task RemoveBuffs(Creature target, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var buff = target.GetPowerInstances<PowerModel>().FirstOrDefault(p => p.Type == PowerType.Buff);
            if (buff == null) return;
            await PowerCmd.Remove(buff);
        }
    }

    public static async Task RemoveAllBuffs(Creature target)
    {
        await RemoveBuffs(target, int.MaxValue);
    }
}
