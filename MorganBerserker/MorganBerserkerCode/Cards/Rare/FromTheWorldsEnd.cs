using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Desde el Confín del Mundo (来自止境) — Morgan's S3: gain Guts (when it fires:
/// survive at 1 HP, +3 Strength, NP +50) and apply 1 Weak to ALL enemies.
/// </summary>
public sealed class FromTheWorldsEnd() : MorganCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>("Weak", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WorldsEndGutsPower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WorldsEndGutsPower>(Owner.Creature, 1m, Owner.Creature, this);
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await PowerCmd.Apply<WeakPower>(enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}
