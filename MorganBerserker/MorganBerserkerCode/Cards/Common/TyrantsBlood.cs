using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>Sangre de Tirana (暴君之血) — re-spec RE-POOL V2 (parche J3-9, injerto de P2): 1⚡,
/// 8 de daño; perdés 2 HP; aplicá 3 de Maldición (mejora 11; 4 Mald.). El puente D→B en común:
/// la sangre de la tirana siembra la bomba (y dispara el cetro).</summary>
public sealed class TyrantsBlood() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new HpLossVar(2m),
        new DynamicVar("Curse", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        await CreatureCmdCompatibility.DamageFromCard(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        if (!cardPlay.Target.IsDead)
        {
            await Curses.Apply(choiceContext, cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
