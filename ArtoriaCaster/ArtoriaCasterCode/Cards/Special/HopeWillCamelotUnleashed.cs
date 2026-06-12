using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP in Berserker form: HOPE WILL CAMELOT unleashed —
/// «La Espada de Esperanza que Hereda el Anhelo». 40 ST damage (Critical 5★: 60),
/// 1 Weak to the target, every ally gains 1 Anti-Purga. Overcharge: +4 damage per
/// 10 over 100. Free, Exhaust.
/// </summary>
public sealed class HopeWillCamelotUnleashed() : ArtoriaCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy), IArtoriaNpCard
{
    public const int ChargeCost = 100;
    public const int CritCost = 5;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(40m, ValueProp.Move),
        new DynamicVar("Crit", 60),
        new DynamicVar("CritCost", CritCost),
        new PowerVar<WeakPower>("Weak", 1m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        var damage = DynamicVars.Damage.BaseValue;
        if (Stars.CanCrit(Owner.Creature, CritCost))
        {
            await Stars.ConsumeForCrit(Owner.Creature, CritCost, this);
            damage = DynamicVars["Crit"].BaseValue + Stars.CritBonus(Owner.Creature);
        }
        damage = NpLevels.Scale(Owner, damage + overcharge);

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);

        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        }
        foreach (var player in Owner.RunState.Players)
        {
            if (player.Creature.IsDead) continue;
            await PowerCmd.Apply<AntiPurgePower>(player.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
        DynamicVars["Crit"].UpgradeValueBy(15m);
    }
}
