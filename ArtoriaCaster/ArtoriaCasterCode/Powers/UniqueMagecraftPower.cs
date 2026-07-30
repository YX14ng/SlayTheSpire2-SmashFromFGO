using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Magia Única B (固有魔術) — tus CRÍTICOS hacen +Amount de daño (vía
/// <see cref="ICritDamageBoost"/>, sumado por Stars.CritBonus al valor crítico).
/// </summary>
public sealed class UniqueMagecraftPower : ArtoriaPower, ICritDamageBoost
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public int CritDamageBonus => Amount;

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return Criticals.WillCrit(Owner, cardSource) ? Amount : 0m;
    }
}
