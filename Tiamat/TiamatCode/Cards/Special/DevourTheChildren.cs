using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Devorar a los Hijos — carta del MAZO ESPECIAL Bestia (<see cref="IBeastEphemeral"/>, Exhaust).
/// 1⚡: la Madre se come hasta 3 de su propia cría para un pico dirigido. Daño = devorados ×
/// (Base + PerNurture × Crianza) + un tercio de la Maldición del objetivo (cosecha el puente), todo
/// ×1.5 por la forma Bestia (IDevourAmplifier). Luego parí 1 para no vaciar el enjambre de un golpe.
/// </summary>
public sealed class DevourTheChildren() : TiamatCard(1, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy), IBeastEphemeral
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("PerNurture", 2),
        new DynamicVar("Devour", 3),
        new DynamicVar("CurseFraction", 3) // suma 1/CurseFraction de la Maldición del objetivo
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override bool ShouldGlowGoldInternal => Lahmu.Count(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var nurture = Lahmu.NurtureOf(Owner.Creature);
        var eaten = await Lahmu.Devour(Owner.Creature, DynamicVars["Devour"].IntValue);
        if (eaten > 0)
        {
            var curseBonus = Curses.Of(cardPlay.Target) / DynamicVars["CurseFraction"].IntValue;
            var dmg = eaten * (DynamicVars.Damage.BaseValue + DynamicVars["PerNurture"].IntValue * nurture) + curseBonus;
            dmg = dmg * Lahmu.DevourBonusMultiplierPct(Owner.Creature) / 100m; // forma Bestia: Devorar +50%
            await DamageCmd.Attack(dmg).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_bloody_impact")
                .Execute(choiceContext);
        }
        await Lahmu.Spawn(Owner.Creature, 1, this);
    }

    protected override void OnUpgrade() => DynamicVars["PerNurture"].UpgradeValueBy(1m);
}
