using FGOCore.FGOCoreCode.Stars;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Ráfaga del Black Barrel — multiple Black Barrel rounds.
/// Rediseño v2: 5 daño BB ×2 (up ×3); por cada buff eliminado: +10 Estrellas de Crítico —
/// generador-payoff de la familia BB (strip → estrellas).</summary>
public sealed class BlackBarrelBurst() : MashShielderCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("Hits", 2),
        new DynamicVar("Stars", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hits = DynamicVars["Hits"].IntValue;
        for (var i = 0; i < hits && !cardPlay.Target.IsDead; i++)
        {
            // BlackBarrel.Hit elimina exactamente 1 buff si lo hay: si el objetivo tenía
            // buffs antes del impacto, este golpe eliminó uno → +10 estrellas.
            var hadBuff = cardPlay.Target.GetPowerInstances<PowerModel>().Any(p => p.Type == PowerType.Buff);
            await BlackBarrel.Hit(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, Owner.Creature, this);
            if (hadBuff)
            {
                await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1m);
    }
}
