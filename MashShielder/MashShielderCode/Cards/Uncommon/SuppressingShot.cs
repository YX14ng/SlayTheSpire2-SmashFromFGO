using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Tiro de Supresión — Black Barrel round + Weak.
/// Rediseño v2: 9 daño de Black Barrel (up +3) + 2 Débil (up +1). La supresión ahora es
/// munición conceptual: tercer miembro de la familia BB en común/poco común
/// (conserva clase y arte — el disparo).</summary>
public sealed class SuppressingShot() : MashShielderCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<WeakPower>("Weak", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await BlackBarrel.Hit(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, Owner.Creature, this);
        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}
