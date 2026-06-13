using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Bruja de la Lluvia (Caster, 雨之魔女·梣) — Aesc, Morgan's past. EL GENERADOR del
/// motor de dos tiempos (rediseño 2026-06-12): es donde SIEMBRAS la maldición.
/// (a) Tus cartas que aplican Maldición aplican +2 (ICurseAmplifier — movido desde
///     Berserker; la siembra vive acá).
/// (b) Mientras estés en esta forma, la Maldición NO decae (ICursePreserver): apilás
///     la bomba sin perderla. Al cambiar a Berserker, el decay vuelve → urgencia de
///     detonar antes de que se achique.
/// (c) Al inicio de tu turno: +8 NP (bajado de 12 — el NP es UN eje, no el óptimo).
/// (d) Tus Ataques hacen -2 daño (la Bruja es floja en combate cuerpo a cuerpo; su
///     daño viene del tic de Maldición sostenido, no de pegar).
/// </summary>
public sealed class RainWitchFormPower : MorganFormPower, ICurseAmplifier, ICursePreserver
{
    public const int NpPerTurn = 8;
    public const int AttackPenalty = 2;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_aesc.tres";

    public int ExtraCurse => 2;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await NpCharge.Gain(Owner, NpPerTurn, null);
    }

    // ModifyDamageAdditive es DELTA (default 0): devolver -2 para la penalidad.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return -AttackPenalty;
    }
}
