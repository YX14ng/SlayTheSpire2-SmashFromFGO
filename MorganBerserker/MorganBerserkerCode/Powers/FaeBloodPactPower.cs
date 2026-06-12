using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Pacto de Sangre Feérica (妖精血之契约) — at the start of your turn: lose 2 HP
/// and gain Amount NP Charge. Rediseño v2, parche P4: the tick's HP loss is marked
/// via <see cref="TickInProgress"/> so it does NOT trigger MadnessEnhancement nor
/// the QueensScepter star grant (the Pact already pays for itself with its own NP;
/// without the filter it triple-dipped and ate one of Madness' 2 triggers/turn).
/// </summary>
public sealed class FaeBloodPactPower : MorganPower
{
    public const int HpCost = 2;

    /// <summary>
    /// P4 — true mientras se resuelve el daño del tick de inicio de turno.
    /// AfterDamageReceived de los listeners corre DENTRO del await de Damage
    /// (resolución secuencial de comandos), así que el flag es determinista.
    /// </summary>
    internal static bool TickInProgress { get; private set; }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        TickInProgress = true;
        try
        {
            await CreatureCmd.Damage(choiceContext, Owner, HpCost,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, null, null);
        }
        finally
        {
            TickInProgress = false;
        }
        if (Owner.IsAlive)
        {
            await NpCharge.Gain(Owner, Amount, null);
        }
    }
}
