using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;
using TiamatBeast.TiamatCode.Powers.Seal;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Sello de las Mareas — el eco Lily del NP (REDESIGN-TIAMAT §46). Aplicás !Seal! de Sello de
/// Habilidad a un enemigo (la marea le ahoga las técnicas) y le sembrás !Curse! de Maldición. La
/// negación a la carta, fuera de la ventana Génesis.
/// </summary>
public sealed class TidalSeal() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Seal", 1),
        new DynamicVar("Curse", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SkillSealPower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Sello.Apply(choiceContext, cardPlay.Target, DynamicVars["Seal"].IntValue, Owner.Creature, this);
        await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
