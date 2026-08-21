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
        // REDESIGN-MORDRED-V2 D8 — es el ÚNICO Ataque del pool que además gasta Estrellas a mano, así
        // que compite con el crítico por el mismo banco: `CriticalResolverPower.BeforeCardPlayed`
        // (FGOCore/.../Stars/Criticals.cs) corre ANTES de este OnPlay y, si no había Crítico Listo, ya
        // se llevó 50★ solo. Con 50-99★ el cobro doble dejaba la carta en CERO de daño después de
        // haber quemado el banco, y su `IsPlayable` se había evaluado antes del cobro. Si el crítico
        // ya pagó, el coste está cumplido con creces y la carta pega igual.
        // Patrón textual de `StarlitCharge` (KagetoraLancer/.../Cards/Common/CommonCards.cs).
        var paid = await CritStars.Spend(choiceContext, Owner.Creature, StarCost, this);
        if (!paid && !Criticals.IsCritical(cardPlay)) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
