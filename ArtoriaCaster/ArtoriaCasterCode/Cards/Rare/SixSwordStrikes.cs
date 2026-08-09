using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Seis Golpes de la Espada — Ataque 2⚡: 3 de daño ×6 al mismo objetivo (los seis
/// golpes del NP real). Mejora: 4×6. Con Critical v2 global el ×1.5 multiplica los
/// seis golpes de la carta que pagó el crítico.
/// </summary>
public sealed class SixSwordStrikes() : ArtoriaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int Hits = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var perHit = DynamicVars.Damage.BaseValue;

        for (var i = 0; i < Hits; i++)
        {
            if (cardPlay.Target.IsDead) break;
            await DamageCmd.Attack(perHit).FromCardFgoCompatibility(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
