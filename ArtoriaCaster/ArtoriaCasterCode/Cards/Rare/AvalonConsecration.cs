using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Consagración de Avalon — la carta CLÍMAX 2⚡, Exhaust: entrás en ARTORIA AVALON
/// (forma permanente: ambas pasivas, sin penalización; la ulti manifestada pasa a
/// ser Around Caliburn: Desatado). Espejo de Coronación del Invierno de Morgan.
/// Mejora: coste 1⚡.
/// </summary>
public sealed class AvalonConsecration() : ArtoriaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AvalonFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FormSwitch.Enter<AvalonFormPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
