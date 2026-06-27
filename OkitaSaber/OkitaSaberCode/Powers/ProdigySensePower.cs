using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Sentido del Prodigio (天才之感) — cada vez que se activa tu umbral de 100★ (ganás un *Crítico
/// Listo): +<see cref="NpGain"/> Carga NP y robá 1 (DESIGN-OKITA §5.3). Engorda el hilo ★→NP.
/// Misma detección que Makoto (CritReadyPower ganado), patrón CombatAnalysisPower de Mash.
/// </summary>
public sealed class ProdigySensePower : OkitaPower
{
    public int NpGain = 10; // up: 20 (la carta lo setea desde su DynamicVar)
    public const int Draw = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
        if (amount <= 0m || power is not CritReadyPower || power.Owner != Owner || Owner.Player == null) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await NpCharge.Gain(Owner, NpGain, null);
            // Capeá el robo al mazo restante para no reshufflear mid-play (saltá si está vacío).
            var inDeck = Owner.Player.PlayerCombatState.AllPiles.FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = Math.Min(Draw, inDeck);
            if (toDraw > 0)
                await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), toDraw, Owner.Player);
        }
    }
}
