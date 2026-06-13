using MegaCrit.Sts2.Core.Entities.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Tarnkappe / Manto de las Sombras (隐身斗篷) — flag resto-de-combate "la espalda ya no está expuesta".
/// Implementa <see cref="IDragonScalePierceSuppressor"/>: la Hoja de Tilo deja de dejar pasar el primer
/// golpe (las escamas lo reducen) y corta todas sus vías de NP. Single, SIN auto-remove (persiste el combate).
/// </summary>
public sealed class TarnkappePower : SiegfriedPower, IDragonScalePierceSuppressor
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    public bool SuppressPierce => true;
}
