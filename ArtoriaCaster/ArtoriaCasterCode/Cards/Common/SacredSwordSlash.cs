using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Common;

/// <summary>
/// Tajo de la Espada Sagrada (圣剑斩击) — Ataque común 1⚡: 9 de daño (mejora 12).
/// Rebalance 2026-08-09 (reporte Steam «pega como un Strike»): base 6→9 = piso de común 1⚡ pura
/// (baseline 9-10). El crítico lo aporta Critical v2 de FGOCore (×1.5 automático pagando Crítico
/// Listo o 50★ globales) — esta carta ya no lleva valores de crítico propios.
/// </summary>
public sealed class SacredSwordSlash() : ArtoriaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);   // 9 -> 12 (upgrade estándar de común)
    }
}
