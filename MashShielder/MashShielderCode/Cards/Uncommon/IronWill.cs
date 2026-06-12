using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Voluntad de Acero — Power: gain BULWARK Block at the end of every turn.
/// Rediseño v2: +4 Bloqueo CON BALUARTE al final de tu turno (up +2). Antes Bloqueo
/// suelto borderline; ahora apila sobre la retención y enciende los riders de Baluarte.</summary>
public sealed class IronWill() : MashShielderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IronWillPower>("IronWill", 4m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IronWillPower>(Owner.Creature, DynamicVars["IronWill"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["IronWill"].UpgradeValueBy(2m);
    }
}
