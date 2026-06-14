using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Berrinche Real (王室任性) — DESIGN-MORDRED §5.2. 1⚡ At: 10 de daño; perdés 1 de Vida (up +4 daño).
/// La pérdida de Vida la cobra la starter «Clarent, la Espada Robada» como +10★ (el odio se recicla:
/// sangrar carga el relámpago). El auto-daño es Unblockable+Unpowered (no lo toca el ±2 de forma ni el
/// Bloqueo). Patrón MapoTofu (HpLossVar). La pérdida de Vida NO sube con el up.
/// </summary>
public sealed class RoyalTantrum() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new HpLossVar(1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
