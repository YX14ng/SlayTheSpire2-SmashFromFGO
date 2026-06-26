using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Finta (虚招) — DESIGN-OKITA §5.2. 0⚡ At: 4 daño y +5★ (up: 6 daño; +10★). Cantrip de relleno que
/// conecta vía el Haori (atacar = ★). Piso P2 2026-06-25: el rider de ★ pasó a BASE (era +0★ base,
/// solo daba ★ mejorada → carta floja sin upgrade); ahora ya alimenta el banco de Crítico de entrada.
/// </summary>
public sealed class Feint() : OkitaCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Stars", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var stars = DynamicVars["Stars"].IntValue;
        if (stars > 0) await CritStars.Gain(Owner.Creature, stars, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);  // 4 -> 6
        DynamicVars["Stars"].UpgradeValueBy(5m); // +5 -> +10★
    }
}
