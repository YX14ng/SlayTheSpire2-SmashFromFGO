using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Velo de la Noche (夜之帷幕 / Evening Shroud EX) — DESIGN-OBERON §6.3, KIT S1, el ÚNICO skill limpio
/// del kit (sin Exhaust ni demérito). 1⚡ Habilidad: +20 Carga NP; durante 3 turnos tus cartas-NP hacen
/// +30% de daño (up +10 NP / +10%). Aplica <see cref="EveningShroudPower"/> (Counter = turnos restantes)
/// y le fija el porcentaje desde el DynamicVar.
/// </summary>
public sealed class EveningShroud() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int Duration = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Charge", 20),
        new DynamicVar("BonusPct", 30),
        new DynamicVar("Duration", Duration)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<EveningShroudPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        var power = await PowerCmd.Apply<EveningShroudPower>(Owner.Creature, DynamicVars["Duration"].BaseValue, Owner.Creature, this);
        if (power != null) power.BonusPct = DynamicVars["BonusPct"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Charge"].UpgradeValueBy(10m);
        DynamicVars["BonusPct"].UpgradeValueBy(10m);
    }
}
