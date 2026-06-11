using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Creación de Objetos (道具作成) — your cards that apply Curse apply +Amount.</summary>
public sealed class ItemConstructionPower : MorganPower, ICurseAmplifier
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public int ExtraCurse => Amount;
}
