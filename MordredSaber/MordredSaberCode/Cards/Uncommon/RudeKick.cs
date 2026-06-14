using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Patada Descortés (无礼飞踢) — DESIGN-MORDRED §5.2. 1⚡ At: 8 de daño + 1 Vulnerable; si tenés un
/// *Crítico Listo*, 2 Vulnerable en vez de 1 (up +3 daño / +1 Vulnerable base), glow. Setup de crítico
/// (patrón ShieldRam): el ×2 en cola refuerza la apertura. Lee el conteo de CritReadyPower. El bonus de
/// Vulnerable condicional NO sube con el up (sólo la base).
/// </summary>
public sealed class RudeKick() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int CritVulnerable = 2; // Vulnerable total si hay Crítico Listo (no escala con el up)

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new PowerVar<VulnerablePower>("Vulnerable", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    private bool HasCritReady => Owner.Creature.GetPowerAmount<CritReadyPower>() > 0;

    protected override bool ShouldGlowGoldInternal => HasCritReady;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        var vulnerable = HasCritReady
            ? System.Math.Max(CritVulnerable, DynamicVars["Vulnerable"].IntValue)
            : DynamicVars["Vulnerable"].IntValue;
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, vulnerable, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
