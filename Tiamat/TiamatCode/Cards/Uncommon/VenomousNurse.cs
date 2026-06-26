using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Nodriza Venenosa — golpe que ALIMENTA: pega y sube la Crianza global (escala a TODO el
/// enjambre y sus mordidas). Daño Lily que además engorda la mordida del enjambre y la cosecha
/// Bestia: ataca y cría al mismo tiempo.</summary>
public sealed class VenomousNurse() : TiamatCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Nurture", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuNurturePower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Nurture"].UpgradeValueBy(1m);
}
