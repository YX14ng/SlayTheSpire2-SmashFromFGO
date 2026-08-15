using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Common;

/// <summary>
/// Tajo de la Espada Sagrada (圣剑斩击) — Ataque común 1⚡: 9 de daño; con un Crítico Listo: +4.
/// Rebalance 2026-08-09: base 6→9 (reporte «pega como un Strike»). Rebalance 2026-08-15
/// (REBALANCE-TIAMAT-ARTORIA.md A6): ese buff la dejó duplicada con Proyección de Caliburn
/// (1⚡: 9 plano, señalado por el reporte chino); el rider de Crítico Listo la diferencia
/// devolviéndole su intent original de payoff crítico común (diseño: 6 / Crítico 2★: 13).
/// Nota: el A6 aprobado decía «+10★ en Berserker», pero eso calcaba a Estocada de la Pradera
/// (8 + 10★ en Berserker) — se usó el rider crítico para no crear otro duplicado.
/// </summary>
public sealed class SacredSwordSlash() : ArtoriaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Bonus", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool ShouldGlowGoldInternal => Owner?.Creature?.HasPower<CritReadyPower>() == true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.HasPower<CritReadyPower>())
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);   // 9 -> 12 (upgrade estándar de común)
        DynamicVars["Bonus"].UpgradeValueBy(1m); // rider 4 -> 5
    }
}
