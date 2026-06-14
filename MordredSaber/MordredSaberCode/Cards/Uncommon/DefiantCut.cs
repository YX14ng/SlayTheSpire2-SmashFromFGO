using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Corte Desafiante (挑衅斩) — DESIGN-MORDRED §5.2. 1⚡ At: 12 de daño; vs Élites/Jefes +10 Estrellas
/// (up +4 daño), glow. El anti-autoridad reciclado a ★ (la traducción del special anti-[Arthur]); leído
/// con <see cref="MordredExtensions.VersusAuthority"/>. El ★ NO sube con el up. Patrón OnslaughtOfHatred.
/// </summary>
public sealed class DefiantCut() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.VersusAuthority();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (Owner.Creature.VersusAuthority())
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
