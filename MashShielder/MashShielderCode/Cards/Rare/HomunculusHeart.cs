using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Corazón del Homúnculo — Power: changing form draws cards and charges the NP.
/// Rediseño v2: cada cambio de forma → roba 2 (up 3) y +10 NP (up +20; antes 15 —
/// denominación). Parche P2: el power proca máximo 2 veces por turno (sin motor de
/// robo 0E con FormDrill). Con FormDrill por fin tiene gasolina para el ping-pong.
/// </summary>
public sealed class HomunculusHeart() : MashShielderCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<HomunculusHeartPower>("HomunculusHeart", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HomunculusHeartPower>(Owner.Creature, DynamicVars["HomunculusHeart"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HomunculusHeart"].UpgradeValueBy(1m);
    }
}
