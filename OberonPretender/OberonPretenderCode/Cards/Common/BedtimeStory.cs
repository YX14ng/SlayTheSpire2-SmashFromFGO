using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Cuento Antes de Dormir (睡前故事 / Bedtime Story) — DESIGN-OBERON §6.2. 1⚡ Habilidad: robá 2;
/// +10 Carga NP (up +20 NP). El motor de velocidad de mazo + batería: alimenta el banco a la vez que
/// cava más profundo. El up sube SOLO el NP (el robo 2 queda fijo, P10).
/// </summary>
public sealed class BedtimeStory() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Draw", 2), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(20m);
}
