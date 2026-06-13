using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Cazador A → A++ (猎手, §6) — Rank-Up-como-upgrade. Buster anti-dragón: base 11, +6 mientras SdD≥3
/// ("Cazadragones" leído como SdD propio, §4). El Rank-Up A++ añade +2 de Fuerza TEMPORAL este turno
/// (el "+Buster 1 turno" del §6, vía SiegfriedTemporaryStrengthPower). IsUpgraded gatea el extra.
/// </summary>
public sealed class DragonHunterStrike() : SiegfriedCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDragonSlayerCard
{
    private const int ScalesThreshold = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11m, ValueProp.Move), new DynamicVar("Bonus", 6), new DynamicVar("Threshold", ScalesThreshold)];

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
        if (IsUpgraded)
        {
            await PowerCmd.Apply<SiegfriedTemporaryStrengthPower>(Owner.Creature, 2m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
