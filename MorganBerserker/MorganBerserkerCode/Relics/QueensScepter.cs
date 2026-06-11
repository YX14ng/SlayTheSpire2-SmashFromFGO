using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Rhongomyniad, el Cetro de la Reina (止境之枪·王笏) — starter relic: the first
/// time you change form in each combat: gain 1 Energy and draw 1 card.
/// Defines the identity (oscillating) and makes the first switch tempo-positive.
/// </summary>
public sealed class QueensScepter : MorganRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    private bool _usedThisCombat;

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        return Task.CompletedTask;
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisCombat) return;
        _usedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
        if (choiceContext != null)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
