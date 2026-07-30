using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>
/// Inversión Real / 王之投资 (DESIGN-GILGAMESH §5) — convierte el Tesoro del Rey en poder bélico. 1⚡ Hab:
/// jugable SÓLO con al menos <see cref="TreasureCost"/> (2) de *Tesoro; consume ese Tesoro y ganás
/// <see cref="NpGain"/> (30) de Carga NP. Glow cuando podés pagar. up: +10 NP.
///
/// El Tesoro es el contador MOD-LOCAL de Gilgamesh (<see cref="TreasurePower"/>), sembrado por Bab-ilu
/// y recargado por la Apertura del Tesoro — NO el oro de la run. Patrón CollectorsGreed (IsPlayable =
/// CanAfford, ShouldGlowGoldInternal, early-return, TrySpend).
/// </summary>
public sealed class RoyalInvestment() : GilgameshCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const int TreasureCost = 2;
    private const int NpGain = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Treasure", TreasureCost), new DynamicVar("Np", NpGain)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<TreasurePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool IsPlayable => TreasurePower.CanAfford(Owner.Creature, DynamicVars["Treasure"].IntValue);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await TreasurePower.TrySpend(choiceContext, Owner.Creature, DynamicVars["Treasure"].IntValue)) return;
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(10m);
}
