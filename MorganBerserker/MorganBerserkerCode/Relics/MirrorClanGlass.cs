using MegaCrit.Sts2.Core.Commands;
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
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
}
