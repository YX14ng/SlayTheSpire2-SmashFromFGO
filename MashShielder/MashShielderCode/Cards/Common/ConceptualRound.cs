using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Bala Conceptual — Black Barrel round: ignores enemy Block and strips one buff.
/// "The bullets that kill the immortal."
/// Retoque v2: 6 daño (up +3); si eliminó un buff: +20 de Carga NP (up +10). La bala
/// que desarma alimenta el cañón — primer payoff de strip de la familia Black Barrel.
/// </summary>
public sealed class ConceptualRound() : MashShielderCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("NpCharge", 20)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal =>
        CombatState != null && CombatState.GetOpponentsOf(Owner.Creature)
            .Any(e => !e.IsDead && e.GetPowerInstances<PowerModel>().Any(p => p.Type == PowerType.Buff));

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_dramatic_stab");
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue,
            ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);

        var buff = cardPlay.Target.GetPowerInstances<PowerModel>().FirstOrDefault(p => p.Type == PowerType.Buff);
        if (buff != null)
        {
            await PowerCmd.Remove(buff);
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
