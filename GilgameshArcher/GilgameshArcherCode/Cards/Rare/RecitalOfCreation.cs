using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Recital de la Creación (开辟之咏唱) — DESIGN-GILGAMESH §5.4, cose NP→crítico en el clímax. 2⚡
/// Poder: aplica <see cref="RecitalOfCreationPower"/> (cada vez que se manifiesta tu Enuma Elish —
/// cruzás 100 de Carga NP — +1 Crítico Listo y robá 1). up: cuesta 1⚡ (EnergyCost.UpgradeBy(-1)).
/// CanonicalVars vacío (patrón Siegfried GoldenRule).</summary>
public sealed class RecitalOfCreation() : GilgameshCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RecitalOfCreationPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
