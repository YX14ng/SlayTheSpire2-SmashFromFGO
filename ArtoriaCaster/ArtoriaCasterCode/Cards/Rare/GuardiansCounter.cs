using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Contraataque de la Guardiana — Poder 2⚡: cada golpe enemigo anulado por completo:
/// el atacante recibe 4 de daño (máximo 3 veces por turno). Mejora: 6.
/// </summary>
public sealed class GuardiansCounter() : ArtoriaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<GuardiansCounterPower>("Damage", 4m),
        new DynamicVar("MaxTimes", GuardiansCounterPower.MaxPerTurn)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AntiPurgePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GuardiansCounterPower>(Owner.Creature, DynamicVars["Damage"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(2m);
    }
}
