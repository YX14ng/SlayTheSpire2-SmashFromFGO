using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Verdugo de Fafnir (法夫纳的处刑者, §7) — Buster con escalado ACOTADO al techo ×1.5: +1 por cada 4 de
/// Sangre de Dragón (máx +6, no número plano repetible; lección TyrantsSweep P3). Cazadragones: +6 plano
/// contra una Élite o un Jefe (los "dragones" del Spire, gloss §7 — condición que puede fallar, §3).
/// </summary>
public sealed class FafnirsExecutioner() : SiegfriedCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDragonSlayerCard
{
    private const int ScaleCap = 6;   // tope duro del escalado por SdD
    private const int TribeBonus = 6; // bonus plano vs [Dragón] = Élite/Jefe

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    // Glow por SdD≥4 (hay escalado activo) — lectura pura del owner (sin Target, igual que Balmung).
    protected override bool ShouldGlowGoldInternal => Owner.Creature.GetPowerAmount<DragonScalesPower>() >= 4;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var scaleBonus = System.Math.Min(Owner.Creature.GetPowerAmount<DragonScalesPower>() / 4, ScaleCap);
        var tribeBonus = DragonTrait.IsDragon(cardPlay.Target) ? TribeBonus : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + scaleBonus + tribeBonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
