using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Cien Espadas Astilladas (百剑碎裂) — DESIGN-MORDRED §5.3. 0⚡ At: solo jugable con ≥50 Estrellas;
/// consumí 50 y hacé 26 de daño (up 32), glow. Slot Cometa: las ★ como munición de un golpe enorme a
/// 0⚡. Patrón Comet/Cien Espadas: gate de banco + gasto manual de CritStars. El up sube SOLO el daño.
/// </summary>
public sealed class HundredShatteredSwords() : MordredCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int StarCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(26m, ValueProp.Move), new DynamicVar("StarCost", StarCost)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, StarCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (!CritStars.CanPay(Owner.Creature, StarCost)) return;
        await CritStars.Gain(Owner.Creature, -StarCost, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
