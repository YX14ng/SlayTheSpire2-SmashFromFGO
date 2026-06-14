using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Absorción del Coleccionista (收藏家之汲取) — DESIGN-GILGAMESH §5.4, patrón 黑闪 (dispara el
/// umbral en la misma jugada). 0⚡ Hab Exhaust: +100 Estrellas de Crítico (auto-proc del umbral 100 →
/// Crítico Listo inmediato vía FGOCore). up: además +20 Carga NP. Las Estrellas NO escalan con el up;
/// el upgrade añade el +20 NP (gateado por IsUpgraded).</summary>
public sealed class CollectorsAbsorption() : GilgameshCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 100), new DynamicVar("Np", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        if (IsUpgraded)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        }
    }
}
