using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Golpe de Estado (政变) — DESIGN-MORDRED §5.3. 3⚡ At, Exhaust: 36 de daño (up +10). El clímax
/// single-hit: EL blanco natural del ×2 (un golpe enorme que el Crítico Listo dobla entero). El Exhaust
/// paga la sobre-tasa (§9: 36/3⚡ + Exhaust = +30-50%). Patrón Bludgeon/Golpe de Estado vanilla.
/// </summary>
public sealed class CoupDEtat() : MordredCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(36m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(10m);
}
