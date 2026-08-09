using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Common;

/// <summary>
/// Tajo de la Espada Sagrada (圣剑斩击) — THE critical-pattern exemplar:
/// 9 damage; Critical 2★: 16 (consume 2★ in Berserker/Avalon to use the crit value).
/// Rebalance 2026-08-09 (reporte Steam «pega como un Strike»): base 6→9 = piso de común 1⚡ pura
/// (baseline 9-10); el crítico no es rider gratis — se paga con 2★ (1★ ≈ ½⚡), delta +7 ≈ 3,5/★
/// dentro de la banda 3-5 de la tasa de Estrellas.
/// </summary>
public sealed class SacredSwordSlash() : ArtoriaCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public const int CritCost = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Crit", 16),
        new DynamicVar("CritCost", CritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = await ResolveCritDamage(CritCost);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);   // 9 -> 12 (upgrade estándar de común)
        DynamicVars["Crit"].UpgradeValueBy(5m);  // 16 -> 21
    }
}
