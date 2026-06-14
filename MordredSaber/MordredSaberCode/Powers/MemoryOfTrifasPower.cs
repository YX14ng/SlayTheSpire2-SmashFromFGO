using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Memoria de Trifas (特里法斯的记忆, §5.3) — el epílogo: al inicio de cada turno curás
/// <see cref="HealPerTurn"/> (2, up 3) y ganás <see cref="NpPerTurn"/> de Carga NP (5).
/// Sustain conectado al hilo NP. Los valores por activación son campos settables que fija la
/// carta desde sus DynamicVars; Amount es el conteo de stacks (Counter).
/// </summary>
public sealed class MemoryOfTrifasPower : MordredPower
{
    public int HealPerTurn = 2;
    public int NpPerTurn = 5;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CreatureCmd.Heal(Owner, HealPerTurn * (int)Amount);
        await NpCharge.Gain(Owner, NpPerTurn * (int)Amount, null);
    }
}
