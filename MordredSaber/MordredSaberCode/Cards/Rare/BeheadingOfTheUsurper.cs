using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Decapitación del Usurpador (篡位者斩首) — DESIGN-MORDRED §5.3. 3⚡ At: 24 de daño; vs Élites/Jefes +12
/// (up: 30/+15), glow. El anti-autoridad clímax (slot Ejecución de la Guardiana): el golpe que cobra la
/// traición contra quien se sienta más alto. Rider con <see cref="MordredExtensions.VersusAuthority"/>.
/// </summary>
public sealed class BeheadingOfTheUsurper() : MordredCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(24m, ValueProp.Move), new DynamicVar("Authority", 12)];

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
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars["Authority"].UpgradeValueBy(3m);
    }
}
