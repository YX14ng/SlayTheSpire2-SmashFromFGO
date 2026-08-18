using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Bendición de Avalon — Poder 2⚡: al inicio de cada turno: Carga NP +20.
/// Mejora: +30. (v0.1.21: 8/12 quedaba fuera de las denominaciones 10/20/30/50 y, tras subir el
/// resto de las fuentes de NP, era la PEOR carta de NP del pool siendo rara de 2⚡.)
/// </summary>
public sealed class AvalonBenediction() : ArtoriaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<AvalonBenedictionPower>("NpCharge", 20m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AvalonBenedictionPower>(choiceContext, Owner.Creature, DynamicVars["NpCharge"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
