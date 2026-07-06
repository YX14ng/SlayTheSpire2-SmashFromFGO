using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Instinto Depredador — payoff raro del enjambre: tus Laḫmu muerden una vez MÁS por acumulación
/// (<see cref="ISwarmBiteAmplifier"/>, el mismo gancho con que la forma Bestia muerde dos veces —
/// se SUMAN: Instinto + Bestia = 3 mordidas). No toca la tasa por mordida (nº × (1 + Crianza)):
/// duplica el motor YA construido, así el poder es proporcional al setup (§1.4). La concede la
/// carta rara <c>PredatoryInstinct</c>. Apilable (Counter = mordidas extra).
/// </summary>
public sealed class PredatoryInstinctPower : TiamatPower, ISwarmBiteAmplifier
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public int ExtraBites => Amount;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LahmuSwarmPower>()];
}
