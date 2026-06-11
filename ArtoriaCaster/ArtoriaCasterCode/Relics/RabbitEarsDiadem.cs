using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Diadema de Orejas de Conejo — uncommon: the first time you enter Berserker form
/// each combat: gain 1 Energy (makes opening the cash-out window tempo-neutral;
/// once per combat = no loops).
/// </summary>
public sealed class RabbitEarsDiadem : ArtoriaRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _usedThisCombat;

    public override Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        return Task.CompletedTask;
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisCombat) return;
        if (!Owner.Creature.HasPower<SummerBerserkerFormPower>()) return;
        _usedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }
}
