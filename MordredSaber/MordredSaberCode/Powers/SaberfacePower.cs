using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Saberface (同脸, §5.3) — defensa→★ con sabor meme. La PRIMERA vez por turno que un enemigo te
/// golpea (alcance pleno), ganás +<see cref="StarsPerHit"/> Estrellas (up: y +<see cref="NpPerHit"/>
/// NP). El cap 1/turno sale de un flag propio reseteado en <see cref="BeforeSideTurnStart"/> (patrón
/// MadnessEnhancementPower). Los valores por activación son campos settables que fija la carta desde
/// sus DynamicVars; <see cref="Upgraded"/> habilita el +NP. Counter: las copias suman las ★.
///
/// TECHO DE COMBATE (revisión DESIGN-REVIEW-2): el escalado de ★ NO es infinito. Cada activación
/// suma a un contador de ★ ya generadas por este power este combate (<see cref="StarsThisCombat"/>);
/// una vez alcanzado <see cref="StarsCap"/>, las activaciones siguientes ya no dan ★ (el +NP del up
/// SÍ sigue — es un techo a la munición de Crítico, no a la carga). Patrón del Cap de MakotoPower de
/// Okita: el bono acumulado vive en un campo, no en Amount.
/// </summary>
public sealed class SaberfacePower : MordredPower
{
    /// <summary>Tope de ★ que este power puede generar en TODO el combate (anti-escalado-infinito).</summary>
    public const int StarsCap = 100;

    public int StarsPerHit = 10;
    public int NpPerHit = 10;

    public bool Upgraded;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private bool _firedThisTurn;

    /// <summary>★ ya generadas por Saberface este combate (techo en <see cref="StarsCap"/>).</summary>
    public int StarsThisCombat { get; private set; }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side) _firedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (_firedThisTurn || target != Owner || dealer == null || dealer == Owner || result.UnblockedDamage <= 0) return;
        _firedThisTurn = true;
        Flash();

        // ★ acotadas al techo de combate: lo pedido (StarsPerHit × copias) recortado a lo que falta para el cap.
        var wanted = StarsPerHit * (int)Amount;
        var room = StarsCap - StarsThisCombat;
        var stars = Math.Max(0, Math.Min(wanted, room));
        if (stars > 0)
        {
            StarsThisCombat += stars;
            await CritStars.Gain(Owner, stars, null);
        }
        if (Upgraded)
        {
            await NpCharge.Gain(Owner, NpPerHit, null);
        }
    }
}
