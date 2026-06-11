using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Rhongomyniad, el Cetro de la Reina (止境之枪·王笏) — starter relic: the first
/// time you change form in each combat: gain 1 Energy, draw 1 card and NP +10.
/// Defines the identity (oscillating) and makes the first switch tempo-positive.
/// Also sets the starting form (Fairy Queen) at combat start — without it Morgan
/// fought FORMLESS until her first switch (queen passive inactive, latent bug) —
/// which additionally kicks off FormVisuals' background preload so the first
/// real switch doesn't hitch loading the other forms' textures.
/// </summary>
public sealed class QueensScepter : MorganRelic, IFormChangeListener
{
    public const int NpOnFirstSwitch = 10;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    private bool _usedThisCombat;

    public override async Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        // Forma inicial: Reina. source == null -> no cuenta como "cambio de forma".
        await FormSwitch.Enter<Powers.Forms.FairyQueenFormPower>(null, Owner.Creature, null);
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisCombat) return;
        _usedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
        await NpCharge.Gain(Owner.Creature, NpOnFirstSwitch, null);
        if (choiceContext != null)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
