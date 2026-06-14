using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER «Respiración del Tennen Rishin-ryū» (天然理心流之息) — DESIGN-OKITA §5.4. 2⚡ Poder: aplica
/// <see cref="TennenRishinBreathPower"/> (+2 al tope de *Aliento; recuperás 3 Aliento/turno en vez
/// de 2 — los números viven en el power) (up: coste 1⚡). El motor del arquetipo Ráfaga.
/// </summary>
public sealed class TennenRishinBreath() : OkitaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TennenRishinBreathPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1); // 2 -> 1
}
