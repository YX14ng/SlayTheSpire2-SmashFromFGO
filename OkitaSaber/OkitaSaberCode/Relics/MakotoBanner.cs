using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Estandarte Makoto (诚之旗) — reliquia RARA (DESIGN-OKITA §6.2): cada activación de tu umbral de 100★
/// (ganás un *Crítico Listo por auto-payoff): +<see cref="NpGain"/> Carga NP y robá 1.
///
/// Detección sin API nueva (idéntica a ProdigySensePower / MakotoPower): el auto-payoff de
/// CritStarsPower a 100 aplica CritReadyPower; lo leemos en <see cref="AfterPowerAmountChanged"/> con
/// amount > 0 = un Crítico Listo GANADO = un umbral cumplido. Engorda el hilo ★→NP, igual que la
/// carta-Poder Sentido del Prodigio pero como reliquia.
/// </summary>
public sealed class MakotoBanner : OkitaRelic, ICriticalConsumedListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    private const int NpGain = 10;
    private const int Draw = 1;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public async Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (critical.Owner != Owner.Creature || Owner.Creature.Player == null) return;
        Flash();
        await NpCharge.Gain(choiceContext, Owner.Creature, NpGain, null);
        // Guard de mazo vacio (audit 2026-07-04, espejo de ProdigySensePower): robar aca dispara un
        // reshuffle a mitad de un play — se saltea si no hay cartas en el mazo de robo.
        if (MegaCrit.Sts2.Core.Entities.Cards.PileType.Draw.GetPile(Owner.Creature.Player).Cards.Count > 0)
        {
            await CardPileCmd.Draw(choiceContext, Draw, Owner.Creature.Player);
        }
    }
}
