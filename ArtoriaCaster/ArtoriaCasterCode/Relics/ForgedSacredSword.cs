using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// Espada Sagrada Forjada — boss relic (replaces the starter): every time you change
/// form: gain 2 Critical Stars (max once per turn — the per-turn cap kills any loop).
/// </summary>
public sealed class ForgedSacredSword : ArtoriaRelic, IFormChangeListener
{
    public const int StarsPerSwitch = 2;

    // "Boss relic" del diseño: Ancient es la rareza que usa el slot de jefe (patrón Coronación de Morgan).
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    private int _lastRound = -1;

    public override Task BeforeCombatStartLate()
    {
        _lastRound = -1;
        return Task.CompletedTask;
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        var round = Owner.Creature.CombatState.RoundNumber;
        if (round == _lastRound) return;
        _lastRound = round;
        Flash();
        await Stars.Gain(Owner.Creature, StarsPerSwitch, null);
    }
}
