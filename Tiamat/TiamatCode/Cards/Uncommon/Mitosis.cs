using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Mitosis — DEVORAR: sacrificás 1 Laḫmu, infligís 4 + 3 por Crianza, y parís 1 nuevo
/// (la Unit Mitosis canónica: desgarrar y rehacer). El pico gateado por haber criado.</summary>
public sealed class Mitosis() : TiamatCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("PerNurture", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    // Glow solo con cria para devorar (el dano exige el Devorar — audit 2026-07-05).
    protected override bool ShouldGlowGoldInternal => Lahmu.Count(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // El dano EXIGE haber devorado (audit 2026-07-05): con 0 Lahmu pegaba el dano completo
        // igual y encima quedaba +1 Lahmu neto gratis. Sin cria: solo pare (sin dano).
        var eaten = await Lahmu.Devour(choiceContext, Owner.Creature, 1);
        if (eaten > 0)
        {
            var dmg = DynamicVars.Damage.BaseValue + DynamicVars["PerNurture"].IntValue * Lahmu.NurtureOf(Owner.Creature);
            dmg = dmg * Lahmu.DevourBonusMultiplierPct(Owner.Creature) / 100m; // forma Bestia: Devorar +50%
            await DamageCmd.Attack(dmg).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_bloody_impact")
                .Execute(choiceContext);
        }
        await Lahmu.Spawn(choiceContext, Owner.Creature, 1, this);
    }

    protected override void OnUpgrade() => DynamicVars["PerNurture"].UpgradeValueBy(1m);
}
