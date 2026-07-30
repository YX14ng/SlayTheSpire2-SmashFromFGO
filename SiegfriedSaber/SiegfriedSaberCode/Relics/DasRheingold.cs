using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Das Rheingold / El Oro del Rin (莱茵的黄金) — CE-vínculo deduplicada (P4): la PRIMERA carta-NP de cada
/// turno te da 20 Estrellas de Crítico y robás 1. Tope ESTRUCTURAL 1/turno (flag de código, no "vigilar").
/// Reusa CritStars (auto-proc a 100) + el robo que el pool no tiene. NO clona el dúo +40-50★/NP que rompía el loop.
/// </summary>
public sealed class DasRheingold : SiegfriedRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int StarsPerNp = 20;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (FgoCombatState.GetTurn(Owner.Creature, 5) != 0 ||
            cardPlay.Card.Owner != Owner || cardPlay.Card is not ISiegfriedNpCard) return;
        await FgoCombatState.SetTurn(context, Owner.Creature, 5, 1, cardPlay.Card);
        Flash();
        await CritStars.Gain(context, Owner.Creature, StarsPerNp, null);
        await CardPileCmd.Draw(context, 1, Owner);
    }
}
