using MegaCrit.Sts2.Core.Entities.Creatures;

namespace OberonPretender.OberonPretenderCode.Powers.Sleep;

/// <summary>
/// Marcador en un power del ATACANTE que IGNORA el Sueno del objetivo: sus Ataques danan a los
/// Dormidos sin despertarlos (el abismo los devora en el sueno). Lo lleva la forma Vortigern
/// (VortigernPower). <see cref="SleepPower"/> lo consulta como lectura pura por golpe entrante:
/// si el dealer lo tiene, el golpe NO se anula y NO despierta.
/// </summary>
public interface ISleepIgnorer
{
    bool IgnoresSleep(Creature target);
}
