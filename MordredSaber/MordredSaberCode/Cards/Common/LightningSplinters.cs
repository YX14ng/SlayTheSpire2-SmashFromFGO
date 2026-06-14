using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// Astillas de Relámpago (雷霆碎片) — DESIGN-MORDRED §5.1. 1⚡ At: 3 de daño ×3 a enemigos aleatorios
/// + 10 Estrellas (up: 4×3). Multi-hit disperso que deja chispas (★). El Crítico ×2 dobla solo el
/// PRIMER golpe (lo maneja CritReadyPower). Patrón FamiliarRain (TargetType.RandomEnemy +
/// TargetingRandomOpponents) con feed de ★. El up sube el daño por golpe; el ★ queda en su denominación.
/// </summary>
public sealed class LightningSplinters() : MordredCard(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
{
    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this)
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
