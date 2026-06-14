using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Acumular Nombres (收集名号 / Collecting Names) — DESIGN-OBERON §6.4. 2⚡ Habilidad: robá 3; +20 Carga
/// NP (Robin Goodfellow, Príncipe del Invierno…) (up robá 4). La cava grande + batería.
/// </summary>
public sealed class CollectingNames() : OberonCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Draw", 3), new DynamicVar("Charge", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
