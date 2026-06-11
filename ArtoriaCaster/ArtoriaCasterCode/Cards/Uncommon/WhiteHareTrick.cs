using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Truco de la Liebre Blanca — cambiá a tu forma opuesta; robás 1. Mejorada: robás 2.
/// (Espejo del Truco del Clan del Espejo de Morgan; en Avalon —forma permanente—
/// FormSwitch.Enter es no-op y la carta solo roba.)
/// </summary>
public sealed class WhiteHareTrick() : ArtoriaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ProphecyCasterFormPower>(), HoverTipFactory.FromPower<SummerBerserkerFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<ProphecyCasterFormPower>())
        {
            await FormSwitch.Enter<SummerBerserkerFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await FormSwitch.Enter<ProphecyCasterFormPower>(choiceContext, Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
