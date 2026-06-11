using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Pacto de Sangre Feérica (妖精血之契约) — Poder: al inicio de cada turno,
/// pierdes 2 HP y ganas Carga NP +12.
/// </summary>
public sealed class FaeBloodPact() : MorganCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<FaeBloodPactPower>("Stacks", 12m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FaeBloodPactPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(6m);
    }
}
