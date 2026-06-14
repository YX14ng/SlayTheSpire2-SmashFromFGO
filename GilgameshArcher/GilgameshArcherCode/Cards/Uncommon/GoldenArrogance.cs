using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Arrogancia Dorada (黄金傲慢) — plan B contra presión. 1⚡ Poder: aplica
/// <see cref="GoldenArrogancePower"/>. La 1ª vez que perdés Vida cada turno: +20 Estrellas (tope
/// 1/turno) (up 30). Fija el campo Stars del power desde el DynamicVar al aplicar (patrón
/// WeightOfExpectations).</summary>
public sealed class GoldenArrogance() : GilgameshCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<GoldenArrogancePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Stars = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
