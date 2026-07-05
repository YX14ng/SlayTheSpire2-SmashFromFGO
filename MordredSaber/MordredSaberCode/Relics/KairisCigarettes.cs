using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Cigarrillos de Kairi (凯利的香烟) — reliquia POCO COMÚN (DESIGN-MORDRED §6): la 1ª vez por combate
/// que obtenés un *Crítico Listo, robás <see cref="DrawOnFirstCrit"/> (guiño a Kairi Sisigou, su Master).
/// Detección SIN API nueva (esta fase NO toca FGOCore): el auto-payoff de Estrellas a 100 (o cualquier
/// fuente) aplica <c>CritReadyPower</c>; lo leemos en <see cref="AfterPowerAmountChanged"/> con amount > 0
/// (un Crítico Listo GANADO), patrón MakotoPower/HaoriAsagi. Tope 1/combate (flag de código).
/// </summary>
public sealed class KairisCigarettes : MordredRelic
{
    private const int DrawOnFirstCrit = 2;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _usedThisCombat;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        return Task.CompletedTask;
    }

    // amount > 0 sobre CritReadyPower = un Crítico Listo GANADO → robá 2 (1ª vez por combate).
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_usedThisCombat || amount <= 0m || power is not CritReadyPower || power.Owner != Owner.Creature) return;
        _usedThisCombat = true;
        Flash();
        // BUGFIX (soft-lock): este robo proca a MITAD de la resolución de la carta que disparó el
        // Crítico Listo. Si RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
        // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
        // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
        var inDeck = Owner.Creature.Player?.PlayerCombatState.AllPiles
            .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
        var toDraw = System.Math.Min(DrawOnFirstCrit, inDeck);
        if (toDraw > 0)
        {
            await CardPileCmd.Draw(choiceContext ?? new BlockingPlayerChoiceContext(), toDraw, Owner);
        }
    }
}
