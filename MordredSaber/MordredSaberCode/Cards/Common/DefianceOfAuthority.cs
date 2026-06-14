using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Desafío a la Autoridad (蔑视权威) — DESIGN-MORDRED §5.1. 1⚡ At: 7 de daño; vs Élites/Jefes +20 NP
/// (up +3/+20), glow. El rider anti-autoridad (la traducción del special anti-[Arthur]) canalizado a
/// Carga NP — escupir en la cara del poder llena el medidor. El check vive en
/// <see cref="MordredExtensions.VersusAuthority"/>. Patrón OnslaughtOfHatred, pero el rider va a NP en
/// vez de daño. El up sube daño base y el +NP condicional.
/// </summary>
public sealed class DefianceOfAuthority() : MordredCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.VersusAuthority();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var versus = Owner.Creature.VersusAuthority();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (versus)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
