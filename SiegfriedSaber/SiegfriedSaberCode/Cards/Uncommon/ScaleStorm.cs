using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Tormenta de Escamas (鳞片风暴 / Scale Storm) — el golpe AoE de escamas que NO consume la armadura
/// (DESIGN-REVIEW-2: profundidad sobre el motor SdD; complementa a ScaleEruption, que sí la descarga).
/// 1⚡ Ataque AoE: 7 de daño a TODOS + 3 más mientras tu *Sangre de Dragón sea !Threshold! o más. El
/// barrido de las escamas eriza ado: limpieza de adds steady que premia tener la piel gruesa sin
/// gastarla. Patrón DragonSlayerStrike (bonus por umbral SdD≥3) aplicado a AoE. No genera SdD (no se
/// auto-alimenta). El up sube SÓLO el daño base (el bonus queda en su denominación, techo §1.bis).
/// </summary>
public sealed class ScaleStorm() : SiegfriedCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private const int ScalesThreshold = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("Bonus", 3), new DynamicVar("Threshold", ScalesThreshold)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override bool ShouldGlowGoldInternal => GlowAtScales(ScalesThreshold);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = ScaleThresholdBonus(ScalesThreshold, DynamicVars["Bonus"].IntValue);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardFgoCompatibility(this, cardPlay).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
