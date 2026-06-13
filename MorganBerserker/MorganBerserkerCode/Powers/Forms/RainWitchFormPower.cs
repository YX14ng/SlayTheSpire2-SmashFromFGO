using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Bruja de la Lluvia (Caster, 雨之魔女·梣) — Aesc, el pasado de Morgan. El GENERADOR de
/// Estrellas del motor Buster (rediseño 2026-06-13): es donde ATESORÁS el banco para
/// después volcarlo en críticos. Recurso cruzado = Estrellas (Caster junta / Berserker gasta).
/// (a) Al inicio de tu turno: +12 Estrellas de Crítico.
/// (b) Tus Ataques hacen -2 daño (floja pegando; su rol es juntar, no golpear).
/// </summary>
public sealed class RainWitchFormPower : MorganFormPower
{
    public const int StarsPerTurn = 12;
    public const int AttackPenalty = 2;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_aesc.tres";

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await CritStars.Gain(Owner, StarsPerTurn, null);
    }

    // ModifyDamageAdditive es DELTA (default 0): devolver -2 para la penalidad.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return -AttackPenalty;
    }
}
