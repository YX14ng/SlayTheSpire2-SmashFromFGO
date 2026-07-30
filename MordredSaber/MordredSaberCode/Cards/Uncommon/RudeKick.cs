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
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new PowerVar<VulnerablePower>("Vulnerable", 1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    private bool WillCrit => Criticals.WillCrit(Owner.Creature, this);

    protected override bool ShouldGlowGoldInternal => WillCrit;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        // Rider RELATIVO (+1) en vez de "2 fijo" (audit 2026-07-05): con la carta mejorada
        // (Vulnerable base 2) el max(2, 2) dejaba el rider muerto pero la carta seguia brillando.
        var vulnerable = DynamicVars["Vulnerable"].IntValue + (Criticals.IsCritical(cardPlay) ? 1 : 0);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, vulnerable, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
    }
}
