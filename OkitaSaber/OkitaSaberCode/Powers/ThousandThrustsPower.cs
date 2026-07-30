using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Kata de las Mil Estocadas (千突之形) — el TERCER Ataque que jugás cada turno: +<see cref="StarsGain"/>
/// ★ y +<see cref="NpGainValue"/> Carga NP (DESIGN-OKITA §5.3; arquetipo Shukuchi N injertado de B,
/// sin energía gratis). Cuenta los Ataques propios del turno con un flag privado (CombatState no
/// expone "ataques jugados"), patrón WeightOfExpectationsPower. Counter (cada copia sube los valores).
/// </summary>
public sealed class ThousandThrustsPower : OkitaPower
{
    public int StarsGain => Math.Max(10, FgoCombatState.GetCombat(Owner, 0, 5));
    public int NpGainValue = 10;
    public const int ThrustNumber = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public Task Configure(PlayerChoiceContext context, int stars, int np, CardModel source)
    {
        NpGainValue = Math.Max(NpGainValue, np);
        return FgoCombatState.SetCombat(
            context, Owner, 0, Math.Max(StarsGain, stars), source, width: 5);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner?.Creature != Owner) return;
        var attacks = await FgoCombatState.IncrementTurn(
            context, Owner, 0, ThrustNumber, cardPlay.Card, width: 2);
        if (attacks != ThrustNumber) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await CritStars.Gain(context, Owner, StarsGain, null);
            await NpCharge.Gain(context, Owner, NpGainValue, null);
        }
    }
}
