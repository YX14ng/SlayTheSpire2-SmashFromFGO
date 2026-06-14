using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Embate de Odio (憎恶猛击) — DESIGN-MORDRED §5.1. 2⚡ At: 14 de daño; vs Élites/Jefes +4 (up +4/+2),
/// glow. El golpe-blanco grande del ×2; el rider anti-autoridad (la traducción del special anti-[Arthur])
/// se lee con <see cref="MordredExtensions.VersusAuthority"/>. Patrón GuardiansExecution/DragonSlayerStrike.
/// </summary>
public sealed class OnslaughtOfHatred() : MordredCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(14m, ValueProp.Move), new DynamicVar("Authority", 4)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.VersusAuthority();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = Owner.Creature.VersusAuthority() ? DynamicVars["Authority"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Authority"].UpgradeValueBy(2m);
    }
}
