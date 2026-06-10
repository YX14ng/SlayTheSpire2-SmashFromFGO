using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>Lentes de repuesto — draw 1 additional card at the start of each combat.</summary>
public sealed class SpareGlasses : MashShielderRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner || player.Creature.CombatState.RoundNumber > 1) return count;
        return count + DynamicVars.Cards.BaseValue;
    }
}
