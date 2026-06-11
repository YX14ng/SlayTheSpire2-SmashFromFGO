using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers.Forms;

/// <summary>
/// La Reina (Berserker, 妖精女王) — Morgan's starting form.
/// (a) The first time you damage an enemy's HP each turn: NP Charge +5.
/// (b) Your cards that apply Curse apply +1 (ICurseAmplifier).
/// </summary>
public sealed class FairyQueenFormPower : MorganFormPower, ICurseAmplifier
{
    // 5 -> 8 (primer pase) -> 10 (re-balance v3 al entorno Hextech+BetterCharacters).
    public const int NpOnDamage = 10;

    public override string FramesPath => $"{MainFile.ResPath}/character/morgan_frames_queen.tres";

    // 1 -> 2 en el re-balance v3 (estilo BetterCharacters: duplicar el escalador del
    // arquetipo; acompaña el tope de Maldicion 15 -> 25).
    public int ExtraCurse => 2;

    private bool _npThisTurn;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == CombatSide.Player)
        {
            _npThisTurn = false;
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (_npThisTurn || dealer != Owner || target.IsPlayer) return;
        if (!props.IsPoweredAttack() || result.UnblockedDamage <= 0) return;

        _npThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, NpOnDamage, null);
    }
}
