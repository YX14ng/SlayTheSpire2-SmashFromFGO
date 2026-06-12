using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Refuerzo de Locura (狂化) — cada vez que pierdas HP durante tu turno: Carga NP +10
/// (denominación), máx. 2 activaciones/turno. Rediseño v2 (lección 焰刑地狱): monto
/// 6→10 fijo; la mejora sube el tope 2→3 activaciones/turno en vez del monto.
/// </summary>
public sealed class MadnessEnhancement() : MorganCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MadnessEnhancementPower>("Triggers", 2m),
        new DynamicVar("NpCharge", MadnessEnhancementPower.NpPerTrigger)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MadnessEnhancementPower>(Owner.Creature, DynamicVars["Triggers"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Triggers"].UpgradeValueBy(1m);
    }
}
