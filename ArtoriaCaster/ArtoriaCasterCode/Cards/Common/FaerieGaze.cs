using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Common;

/// <summary>
/// Mirada Feérica — Habilidad 0⚡: aplica 1 de Débil. Mejorada: además ganás 1★
/// (el var "Stars" arranca en 0 y sube con el upgrade).
/// </summary>
public sealed class FaerieGaze() : ArtoriaCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>("Weak", 1m),
        new DynamicVar("Stars", 10),
        new CardsVar(0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        if (DynamicVars["Stars"].IntValue > 0)
        {
            await Stars.Gain(choiceContext, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
        // v0.1.21: el reporter señaló que el problema era la EFICIENCIA DE CARTA (gastar un slot de
        // mano por 1 Débil no paga), no el número. El robo de la mejora lo ataca de frente; Agotar
        // —la otra vía vanilla— sería peor acá porque Débil es un efecto que querés re-aplicar.
        if (DynamicVars.Cards.IntValue > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
