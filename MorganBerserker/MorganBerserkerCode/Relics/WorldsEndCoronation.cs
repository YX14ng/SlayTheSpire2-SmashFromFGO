using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Coronación del Confín (止境的加冕) — Ancient relic: every time you change form,
/// gain 1 Energy (max once per turn). The oscillation engine at full throttle.
/// </summary>
public sealed class WorldsEndCoronation : MorganRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool _usedThisTurn;

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player) _usedThisTurn = false;
        return Task.CompletedTask;
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisTurn) return;
        _usedThisTurn = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }
}
