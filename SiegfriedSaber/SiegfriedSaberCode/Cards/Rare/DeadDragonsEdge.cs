using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Afilado en Escamas Muertas (亡鳞砺刃, §rares) — setup de Sobrecarga para la próxima ult: +Bendición de
/// Sobrecarga (tu próxima carta-NP suma +10 al nivel de Overcharge por acumulación) + algo de NP. Exhaust
/// como cooldown. Reusa OverchargeBlessingPower + NpCharge de FGOCore.
/// </summary>
public sealed class DeadDragonsEdge() : SiegfriedCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Blessing", 3), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<OverchargeBlessingPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner.Creature, DynamicVars["Blessing"].IntValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Blessing"].UpgradeValueBy(1m);
        DynamicVars["Np"].UpgradeValueBy(5m);
    }
}
