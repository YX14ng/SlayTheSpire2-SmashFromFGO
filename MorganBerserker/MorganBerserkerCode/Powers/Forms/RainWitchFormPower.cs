using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// Bruja de la Lluvia (Caster, 雨之魔女·梣) — Aesc, Morgan's past.
/// (a) At the start of your turn: NP Charge +5.
/// (b) Your Attacks deal 2 less damage (staying to brawl in Caster bites).
/// </summary>
public sealed class RainWitchFormPower : MorganFormPower
{
    public const int NpPerTurn = 5;
    public const int AttackPenalty = 2;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_aesc.tres";

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
