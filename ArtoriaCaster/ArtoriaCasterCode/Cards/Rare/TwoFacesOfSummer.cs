using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Dos Caras del Verano — Poder 1⚡: cada vez que cambiás de forma: robás 1, ganás
/// 1★ y Carga NP +5. Mejora: robás 2. (El robo lo gobierna TwoFacesOfSummerPower.Draws,
/// patrón Ojos Feéricos para fijar la propiedad tras aplicar.)
/// </summary>
public sealed class TwoFacesOfSummer() : ArtoriaCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TwoFacesOfSummerPower>("Power", 1m),
        new CardsVar(1),
        new DynamicVar("Stars", TwoFacesOfSummerPower.StarsGain),
        new DynamicVar("NpCharge", TwoFacesOfSummerPower.NpGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TwoFacesOfSummerPower>(Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
        var power = Owner.Creature.GetPowerInstances<TwoFacesOfSummerPower>().FirstOrDefault();
        if (power != null)
        {
            power.Draws = DynamicVars.Cards.IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
