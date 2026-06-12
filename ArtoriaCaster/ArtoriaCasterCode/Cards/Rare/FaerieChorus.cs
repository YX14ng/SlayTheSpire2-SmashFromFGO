using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Coro de las Hadas — Habilidad 1⚡: ganás 1★ por cada carta jugada este turno
/// ANTES de esta (máximo 4). Cuenta vía CombatManager.Instance.History
/// (CardPlaysStarted, patrón EmotionChip vanilla), excluyendo esta misma jugada.
/// Mejora: máximo 5 y robás 1.
/// </summary>
public sealed class FaerieChorus() : ArtoriaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Max", 4),
        new CardsVar(0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var played = CombatManager.Instance.History.CardPlaysStarted.Count(e =>
            e.Actor == Owner.Creature &&
            e.HappenedThisTurn(Owner.Creature.CombatState) &&
            e.CardPlay != cardPlay);

        var stars = Math.Min(played, DynamicVars["Max"].IntValue);
        if (stars > 0)
        {
            await Stars.Gain(Owner.Creature, stars, this);
        }
        if (DynamicVars.Cards.IntValue > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Max"].UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
