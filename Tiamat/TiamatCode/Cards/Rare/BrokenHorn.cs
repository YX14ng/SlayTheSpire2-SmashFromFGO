using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Tiamat.TiamatCode.Cards.Rare;

/// <summary>Cuerno Roto — devorás TODA la cria de un golpe: daño AoE por cada Laḫmu devorado,
/// escalado por la Crianza, y cargás NP por cabeza. El boton de cosecha. 0 maná, Exhaust.</summary>
public sealed class BrokenHorn() : TiamatCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("PerHorn", 4),
        new DynamicVar("PerNurture", 2),
        new DynamicVar("NpPerLahmu", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Lahmu.Count(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nurture = Lahmu.NurtureOf(Owner.Creature);
        var eaten = await Lahmu.Devour(Owner.Creature, Lahmu.Count(Owner.Creature));
        if (eaten <= 0) return;

        var dmg = eaten * (DynamicVars["PerHorn"].IntValue + DynamicVars["PerNurture"].IntValue * nurture);
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).ToList())
        {
            if (enemy.IsDead) continue;
            await DamageCmd.Attack(dmg).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_bloody_impact")
                .Execute(choiceContext);
        }
        await NpCharge.Gain(Owner.Creature, eaten * DynamicVars["NpPerLahmu"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["PerNurture"].UpgradeValueBy(1m);
}
