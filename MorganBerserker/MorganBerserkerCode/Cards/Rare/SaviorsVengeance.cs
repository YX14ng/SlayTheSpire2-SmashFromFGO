using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Venganza de la Salvadora (救世妖精的复仇) — 6 de daño + tu HP perdida ÷ 5
/// (máx. 20 total). Mejora: ÷4 (máx. 24).
/// </summary>
public sealed class SaviorsVengeance() : MorganCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Divisor", 3),
        new DynamicVar("Max", 30)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var missing = (decimal)(Owner.Creature.MaxHp - Owner.Creature.CurrentHp);
        var bonus = Math.Floor(missing / DynamicVars["Divisor"].IntValue);
        var damage = Math.Min(DynamicVars.Damage.BaseValue + bonus, DynamicVars["Max"].BaseValue);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Max"].UpgradeValueBy(10m);
    }
}
