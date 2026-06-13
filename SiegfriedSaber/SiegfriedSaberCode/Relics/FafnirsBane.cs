using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Perdición de Fafnir (法夫纳之灾) — mientras tu Sangre de Dragón sea ≥3, tus cartas Cazadragones
/// (IDragonSlayerCard) infligen +5 de daño. Aditivo plano gateado por umbral, SÓLO a esa sub-familia
/// (no ×global). La condición puede fallar (si te bajan/suprimen la SdD). Sinergia con DragonHeartScale
/// (arranca en SdD 3) y el umbral de Balmung. Patrón ModifyDamageAdditive de StrikeDummy (relics vanilla).
/// </summary>
public sealed class FafnirsBane : SiegfriedRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int Bonus = 5;
    private const int ScalesThreshold = 3;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (props.IsPoweredAttack()
            && dealer == Owner.Creature
            && cardSource is IDragonSlayerCard
            && Owner.Creature.GetPowerAmount<DragonScalesPower>() >= ScalesThreshold)
        {
            return Bonus;
        }
        return 0m;
    }
}
