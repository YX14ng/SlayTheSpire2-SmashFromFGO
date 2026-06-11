using MegaCrit.Sts2.Core.Entities.Powers;

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
}
