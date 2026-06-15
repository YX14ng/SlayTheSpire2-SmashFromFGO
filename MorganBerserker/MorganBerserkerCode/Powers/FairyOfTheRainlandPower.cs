using System.Linq;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>Hada del País de la Lluvia (雨之国的妖精) — at the start of your turn: NP Charge +Amount.
/// Co-op: además, cada aliado vivo gana <see cref="AllyCharge"/> de Carga NP al inicio de cada turno
/// (el reparto vive en el power, no en el OnPlay — patrón de tick por turno tipo Oberon). En 1 jugador
/// PlayerCreatures es solo el Owner -> el foreach queda vacío (fiel a 1 jugador).</summary>
public sealed class FairyOfTheRainlandPower : MorganPower
{
    /// <summary>Carga NP que se reparte a cada aliado por turno en co-op (fijo; no escala con el counter).</summary>
    public const int AllyCharge = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await NpCharge.Gain(Owner, Amount, null);
        foreach (var ally in Owner.CombatState.PlayerCreatures.Where(c => c != Owner && !c.IsDead))
            await NpCharge.Gain(ally, AllyCharge, null);
    }
}
