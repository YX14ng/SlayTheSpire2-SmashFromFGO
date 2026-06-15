using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Maldición de Cernunnos (科尔努诺斯的诅咒) — your Curses no longer decay after
/// dealing their damage (the per-enemy cap <see cref="FGOCore.FGOCoreCode.Curses.CursePower.MaxPerEnemy"/>
/// is the brake; ICursePreserver).
/// </summary>
public sealed class CurseOfCernunnosPower : MorganPower, ICursePreserver
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
