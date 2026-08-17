using MegaCrit.Sts2.Core.Entities.Powers;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Maldición de Cernunnos (科尔努诺斯的诅咒) — re-efecto 2026-08-16 (feedback de Sac2Loo2Sac):
/// <b>cada Detonación tuya te da <see cref="PowerModel.Amount"/> de Carga NP</b>. Es el knob que el
/// panel dejó aprobado como alternativa (REDESIGN-MORGAN-V2 §15.4-4: «mejora alternativa +10 NP por
/// Detonación, nunca exención de consumo»).
///
/// <para>Su efecto anterior — «las Detonaciones consumen solo la mitad» — se mudó a la
/// <see cref="Forms.WinterQueenFormPower"/>: el jugador pidió que el clímax permanente retuviera
/// campo, y ese es su lugar natural (una forma irreversible que se vende como «lo mejor de ambas»).
/// Así Cernunnos deja de ser redundante con el clímax y pasa a ser el puente danza → Sobrecarga.</para>
///
/// <para>La lectura la hace <see cref="Sentencia.Detonar"/>; este power es el marcador.
/// PROHIBIDO por el panel: «la primera detonación no consume» (loop con Cosecha de Maldición).</para>
/// </summary>
public sealed class CurseOfCernunnosPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    // Counter (antes Single): ahora el Amount ES el número que le importa al jugador (la Carga NP
    // por Detonación), y con Single el motor nunca dibuja el número — NPower.cs:234 solo imprime
    // DisplayAmount con Counter. Dos copias apilan a 20/40, que es el comportamiento esperado.
    public override PowerStackType StackType => PowerStackType.Counter;
}
