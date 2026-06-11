using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Furia de la Adversidad (逆境之怒) — 8 de daño; +4 si HP ≤75%, +4 más si ≤50%.</summary>
public sealed class AdversitysFury() : MorganCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Bonus", 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var ratio = (double)Owner.Creature.CurrentHp / (double)Owner.Creature.MaxHp;
        var bonus = 0;
        if (ratio <= 0.75) bonus += DynamicVars["Bonus"].IntValue;
        if (ratio <= 0.50) bonus += DynamicVars["Bonus"].IntValue;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
