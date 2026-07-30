using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Marea de Laḫmu — carta del MAZO ESPECIAL Bestia (<see cref="IBeastEphemeral"/>, Exhaust). 2⚡:
/// la marea de cría: parí hasta 6 Laḫmu y ganás 1 Crianza por cada Laḫmu YA en campo (se mide
/// antes de parir, así premia haber montado el enjambre en Lily). El motor de la ventana llena.
/// </summary>
public sealed class LahmuTide() : TiamatCard(2, CardType.Skill, CardRarity.Event, TargetType.Self), IBeastEphemeral
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 6),
        new DynamicVar("NurturePerLahmu", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var inPlay = Lahmu.Count(Owner.Creature);
        await Lahmu.Spawn(choiceContext, Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        if (inPlay > 0)
        {
            await Lahmu.Feed(choiceContext, Owner.Creature, DynamicVars["NurturePerLahmu"].IntValue * inPlay, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["NurturePerLahmu"].UpgradeValueBy(1m);
}
