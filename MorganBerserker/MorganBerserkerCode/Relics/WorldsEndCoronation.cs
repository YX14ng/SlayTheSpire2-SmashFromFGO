using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Coronación del Confín (止境的加冕) — Ancient relic: every time you change form,
/// gain 1 Energy (max once per turn). The oscillation engine at full throttle.
/// </summary>
public sealed class WorldsEndCoronation : MorganRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeCombatStartLate()
    {
        await FormSwitch.Enter<FairyQueenFormPower>(null, Owner.Creature, null);
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner || FgoCombatState.GetCombat(Owner.Creature, 1) != 0) return;
        await FgoCombatState.SetCombat(choiceContext, Owner.Creature, 1, 1);
        await FGOCore.FGOCoreCode.Combat.ManifestCards
            .ManifestToHand<Cards.Special.QueensMetamorphosis>(Owner.Creature, 1.0f);
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (FgoCombatState.GetTurn(Owner.Creature, 4) != 0) return;
        await FgoCombatState.SetTurn(
            choiceContext ?? new BlockingPlayerChoiceContext(), Owner.Creature, 4, 1);
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }
}
