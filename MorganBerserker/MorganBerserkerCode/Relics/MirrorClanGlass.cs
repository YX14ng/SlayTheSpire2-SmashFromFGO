using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>Espejo del Clan (镜之氏族的魔镜) — every time you change form: draw 1 card.</summary>
public sealed class MirrorClanGlass : MorganRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (choiceContext == null) return;
        Flash();
        // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
        // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
        // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
        // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
        var inDeck = Owner.Creature.Player?.PlayerCombatState.AllPiles
            .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
        if (inDeck > 0)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
