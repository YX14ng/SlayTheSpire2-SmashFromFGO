using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Pared Absoluta — until your next turn, your HP cannot be reduced. Exhaust.
/// Rediseño v2 + parche P8b: los golpes detenidos por este efecto cuentan como
/// totalmente bloqueados — el power invoca directamente Intercepción, las estrellas
/// de la reliquia y SenpaiPromise (con guard anti doble-disparo si el golpe ya fue
/// bloqueado de verdad). Una línea y la isla queda cosida a tres hilos.
/// </summary>
public sealed class AbsoluteWall() : MashShielderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AbsoluteWallPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AbsoluteWallPower>(), HoverTipFactory.FromPower<InterceptPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AbsoluteWallPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
