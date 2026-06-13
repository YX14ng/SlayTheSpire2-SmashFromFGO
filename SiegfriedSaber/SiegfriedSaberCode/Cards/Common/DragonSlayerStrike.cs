using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Tajo Cazadragones (屠龙斩) — Buster-payoff puro. SKILL §2: 1⚡ puro 9-10; base 9 + 4 GATEADO
/// por SdD≥3 (condición de estado que puede fallar; §3 paga +20-30%). "Cazadragones" leído como SdD
/// propio (él ES el dragón, §4), NO trait de enemigo (no existe en vanilla). Sin NP (rol distinto del
/// Tajo básico DragonbloodCut). El bonus condicional y el umbral no suben con el up (no inflar el techo).</summary>
public sealed class DragonSlayerStrike() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int ScalesThreshold = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("Bonus", 4), new DynamicVar("Threshold", ScalesThreshold)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.GetPowerAmount<DragonScalesPower>() >= ScalesThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = Owner.Creature.GetPowerAmount<DragonScalesPower>() >= ScalesThreshold
            ? DynamicVars["Bonus"].IntValue
            : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
