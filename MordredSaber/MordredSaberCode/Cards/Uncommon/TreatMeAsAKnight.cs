using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// «Trátame como caballero» (把我当骑士对待) — DESIGN-MORDRED §5.2. 1⚡ Hab: robá 2; si Enmascarado
/// robá 1 más (up: robá 3 / +1 extra), glow. Ciclo + regla de trato (la trata como caballero, no como
/// niña ni como mujer). Leído con <see cref="Forms.InMaskedForm"/>. Patrón TreatMeAsAKnight (robo con
/// gate de forma).
/// </summary>
public sealed class TreatMeAsAKnight() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("MaskedDraw", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<MaskedKnightFormPower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var draws = DynamicVars.Cards.IntValue;
        if (Forms.InMaskedForm(Owner.Creature))
        {
            draws += DynamicVars["MaskedDraw"].IntValue;
        }
        await CardPileCmd.Draw(choiceContext, draws, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
        DynamicVars["MaskedDraw"].UpgradeValueBy(1m);
    }
}
