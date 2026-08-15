using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Maldición de Cernunnos (科尔努诺斯的诅咒) — re-efecto 2026-08-15 (REDESIGN-MORGAN-V2 M5/J1-4):
/// tus Detonaciones consumen solo la MITAD de la Maldición del objetivo (redondeo hacia arriba);
/// el daño de la Detonación sigue siendo la Maldición completa. Es el puente rara entre la danza
/// (arquetipo A) y el invierno perpetuo (B): detonás sin vaciar el campo. El efecto viejo («tu
/// Maldición no baja tras hacer su daño», ICursePreserver) era redundante con el no-decay de
/// Bruja/Invierno. La lectura la hace <see cref="Sentencia.Detonar"/>; este power es el marcador.
/// PROHIBIDO por el panel: «la primera detonación no consume» (loop con Cosecha).
/// </summary>
public sealed class CurseOfCernunnosPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}
