using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Hada del País de la Lluvia (雨之国的妖精) — al jugarla: Carga NP +20; al inicio
/// de cada turno: +5. Rediseño v2: burst 15→20 (denominación "paquete"). (up +5/+3)
/// Co-op: al inicio de cada turno cada aliado también gana !AllyCharge! de Carga NP
/// (el reparto vive en <see cref="FairyOfTheRainlandPower"/>; aquí solo se documenta para la loc).
/// </summary>
public sealed class FairyOfTheRainland() : MorganCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 0),
        new PowerVar<FairyOfTheRainlandPower>("Stacks", 10m),
        new DynamicVar("AllyCharge", FairyOfTheRainlandPower.AllyCharge)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (DynamicVars["NpCharge"].IntValue > 0)
        {
            await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
        await PowerCmd.Apply<FairyOfTheRainlandPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
