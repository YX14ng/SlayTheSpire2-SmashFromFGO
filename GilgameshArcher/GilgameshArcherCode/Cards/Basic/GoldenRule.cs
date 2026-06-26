using GilgameshArcher.GilgameshArcherCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Basic;

/// <summary>
/// Regla de Oro / 黄金律 (DESIGN-GILGAMESH §5.1/§6) — la FIRMA del mazo inicial QAABB que ENSEÑA la
/// economía del Rey: la fortuna se acumula sola. 1⚡ Poder: aplica <see cref="GoldenRulePower"/> de
/// FGOCore (cada ganancia de *Carga NP se amplifica +50% por stack). El segundo hilo del medidor:
/// acelera el camino a la primera Enuma del run. Patrón EXACTO de la <c>GoldenRule</c> de Siegfried.
///
/// Token de comando del deck base (<c>CardRarity.Basic</c>, no drafteable). CanonicalVars vacío
/// (el efecto lo describe el power, patrón Siegfried/RecitalOfCreation).
/// </summary>
public sealed class GoldenRule() : GilgameshCard(1, CardType.Power, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoldenRulePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GoldenRulePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
