using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Corona del Sin Par (无双之冠, §9) — la SEGUNDA carta de cleanse permitida (P8). Power 3⚡: el sobrecosto
/// ES la identidad (repetible pero antieconómico, no Exhaust). Aplica PeerlessCrownPower: el primer golpe que
/// atraviesa tu Sangre de Dragón cada turno no inflige daño y limpia 1 Debuff (up: +5 NP al activarse).
/// </summary>
public sealed class CrownOfThePeerless() : SiegfriedCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<PeerlessCrownPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Upgraded = IsUpgraded;
    }
}
