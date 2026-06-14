using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Vuelo de la Polilla (飞蛾之翼 / Moth Flight) — DESIGN-OBERON §6.2. 1⚡ Ataque: 5 de daño ×2;
/// +10 Estrellas (up 7×2). Multi-hit + ★: con Crítico Listo, el ×2 dobla SOLO el primer golpe (lo
/// maneja CritReadyPower). Patrón LightningSplinters. El up sube el daño por golpe; las ★ quedan fijas.
/// </summary>
public sealed class MothFlight() : OberonCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int Hits = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
