using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Espíritu Pionero de las Estrellas — Power. Rediseño v2: la primera carta NP de cada
/// combate no consume Carga y se resuelve con Sobrecarga MÍNIMA (waiver P3, hecho en
/// FGOCore: filtra Event y resuelve a tier mínimo — sin doble-dip ni marker stuck).
/// Además, cada carta NP que juegas: +30 Estrellas de Crítico (up +50) — los NP generan
/// estrellas como en FGO y el nombre por fin es literal.
/// </summary>
public sealed class PioneerOfTheStars() : MashShielderCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PioneerOfTheStarsPower>("Stars", 30m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<FGOCore.FGOCoreCode.Stars.CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PioneerOfTheStarsPower>(Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(20m);
    }
}
