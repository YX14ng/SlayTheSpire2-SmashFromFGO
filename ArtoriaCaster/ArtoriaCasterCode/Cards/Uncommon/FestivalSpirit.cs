using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Espíritu del Festival — Poder: cada vez que cambiás de forma: ganás 1★ y 3 de
/// Bloqueo. Mejorada: 1★ y 5.
/// </summary>
public sealed class FestivalSpirit() : ArtoriaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FestivalSpiritPower>("Power", 1m),
        new DynamicVar("Stars", FestivalSpiritPower.StarsPerSwitch),
        new DynamicVar("Block", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FestivalSpiritPower>(Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
        var power = Owner.Creature.GetPowerInstances<FestivalSpiritPower>().FirstOrDefault();
        if (power != null)
        {
            power.BlockPerSwitch = DynamicVars["Block"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(2m);
    }
}
