using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Cuerno Roto — carta del MAZO ESPECIAL Bestia (<see cref="IBeastEphemeral"/>, Exhaust). 0⚡:
/// el botón de cosecha de la ventana. Devorás TODA la cría de un golpe: AoE por cada Laḫmu devorado
/// escalado por la Crianza (× la forma Bestia = +50%, vía IDevourAmplifier) y recargás NP por cabeza
/// (clave para recargar dentro de la ventana y armar el cierre con Pluma de la Bestia).
/// </summary>
public sealed class BrokenHornBeast() : TiamatCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IBeastEphemeral
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
        dmg = dmg * Lahmu.DevourBonusMultiplierPct(Owner.Creature) / 100; // forma Bestia: Devorar +50%
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
