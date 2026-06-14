using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Kata de las Mil Estocadas (千突之形) — el TERCER Ataque que jugás cada turno: +<see cref="StarsGain"/>
/// ★ y +<see cref="NpGainValue"/> Carga NP (DESIGN-OKITA §5.3; arquetipo Shukuchi N injertado de B,
/// sin energía gratis). Cuenta los Ataques propios del turno con un flag privado (CombatState no
/// expone "ataques jugados"), patrón WeightOfExpectationsPower. Counter (cada copia sube los valores).
/// </summary>
public sealed class ThousandThrustsPower : OkitaPower
{
    public int StarsGain = 10;       // up 20 (la carta lo setea)
    public int NpGainValue = 10;
    public const int ThrustNumber = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    private int _attacksThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side) _attacksThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;
        _attacksThisTurn++;
        if (_attacksThisTurn != ThrustNumber) return;
        Flash();
        await CritStars.Gain(Owner, StarsGain, null);
        await NpCharge.Gain(Owner, NpGainValue, null);
    }
}
