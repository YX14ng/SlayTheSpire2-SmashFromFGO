using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Lluvia de Rhongomyniad (伦戈米尼亚德之雨) — the spammable mini-NP (min 50,
/// consumes ALL): the floor of the NP economy. Rediseño v2: min 40→50 (gate de
/// denominación — evita el spam con carga trivial en la economía rápida nueva).
/// Auditoría (P9, números corregidos): Rhongo+ a cap 300 = (300−50)/10×4 + 24 = 124.
/// </summary>
public sealed class RhongomyniadRain() : MorganCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(18m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + bonus);

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
