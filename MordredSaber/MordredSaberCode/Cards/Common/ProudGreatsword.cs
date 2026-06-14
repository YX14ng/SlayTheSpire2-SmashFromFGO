using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Mandoble Orgulloso (傲慢大剑) — DESIGN-MORDRED §5.1. 1⚡ At: 12 de daño; perdés 2 de Vida (up +4).
/// La pérdida de Vida NO da ★ directamente: la cobra la starter (Clarent → +10★ por pérdida de Vida),
/// por eso la carta no menciona el ★ (el rider es de la reliquia). Eje [Vida→★]. Patrón MadLunge: el
/// HpLoss es Unblockable|Unpowered. El up sube SOLO el daño; el HpLoss queda fijo (no inflar el costo
/// de Vida, que es lo que paga el motor).
/// </summary>
public sealed class ProudGreatsword() : MordredCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, ValueProp.Move), new HpLossVar(2m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
