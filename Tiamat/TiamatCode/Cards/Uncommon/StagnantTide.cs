using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Marea Estancada — el congelador del campo de Maldición (REDESIGN-TIAMAT §48). Sembrás !Curse!
/// de Maldición a un enemigo y, hasta tu próximo turno, tus Maldiciones NO decaen tras pegar
/// (<see cref="TidalStasisPower"/> / <c>ICursePreserver</c>). La marea se estanca para que el DoT
/// se acumule en vez de drenarse.
/// </summary>
public sealed class StagnantTide() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Curse", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>(), HoverTipFactory.FromPower<TidalStasisPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await Curses.Apply(cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        if (!Owner.Creature.HasPower<TidalStasisPower>())
        {
            await PowerCmd.Apply<TidalStasisPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Curse"].UpgradeValueBy(2m);
}
