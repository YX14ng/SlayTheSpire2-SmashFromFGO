using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Relics;

/// <summary>
/// El Bastón de Selección (選定の杖 — Merlin's staff, it talks to her in his voice) —
/// starter relic. Sets the starting form (Caster) at combat start, and the first time
/// you change form in each combat: gain 2 Critical Stars and 4 Block.
/// </summary>
public sealed class SelectionStaff : ArtoriaRelic, IFormChangeListener
{
    public const int Stars = 2;
    public const int Block = 4;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    private bool _usedThisCombat;

    public override async Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        // Initial form: Caster. source == null -> doesn't count as "changing form".
        await FormSwitch.Enter<ProphecyCasterFormPower>(null, Owner.Creature, null);
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisCombat) return;
        _usedThisCombat = true;
        Flash();
        await Powers.Stars.Gain(Owner.Creature, Stars, null);
        await CreatureCmd.GainBlock(Owner.Creature, Block, ValueProp.Unpowered, null);
    }
}
