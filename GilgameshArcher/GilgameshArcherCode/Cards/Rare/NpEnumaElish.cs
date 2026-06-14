using GilgameshArcher.GilgameshArcherCode.Cards;
using GilgameshArcher.GilgameshArcherCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>
/// NP Enuma Elish (天地乖离开辟之星) — DESIGN-GILGAMESH §5.4. La carta-NP DRAFTEABLE: disparar Enuma ANTES
/// del auto-ulti. 2⚡, Exhaust; mín. 70, consume TODA la carga (<c>ConsumeAllForNpCard</c>); marca
/// <see cref="IGilgameshNpCard"/>.
///
/// 24 de daño a TODOS los enemigos, +2 por cada 10 de carga consumida sobre 70 (la Sobrecarga, a TODOS);
/// contra Élites/Jefes (<see cref="RoyalTrait.IsDivine"/>) +12 adicional. El daño base + sobrecarga escala
/// con dupes (<see cref="NpLevels.Scale"/>). La <c>OverchargeBlessingPower</c> ya está horneada en
/// <c>ConsumeAllForNpCard</c> (sube el tier consumido antes de calcular la sobrecarga). up: 30 base /
/// +15 anti-divino. Glow cuando es pagable.
/// </summary>
public sealed class NpEnumaElish() : GilgameshCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IGilgameshNpCard
{
    public const int ChargeCost = 70;

    private const int PerTen = 2; // +2 a TODOS por cada 10 de carga consumida sobre 70

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new DynamicVar("Divine", 12),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1) Consume TODA la carga; tier = lo realmente consumido (>= 70, + OverchargeBlessing).
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * PerTen; // sobrecarga a TODOS

        // 2) base + sobrecarga es plano para todos; sólo Élites/Jefes reciben +Divine.
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            var divineBonus = RoyalTrait.IsDivine(enemy) ? DynamicVars["Divine"].IntValue : 0;
            var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + divineBonus);
            await DamageCmd.Attack(damage).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_starry_impact")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars["Divine"].UpgradeValueBy(3m);
    }
}
