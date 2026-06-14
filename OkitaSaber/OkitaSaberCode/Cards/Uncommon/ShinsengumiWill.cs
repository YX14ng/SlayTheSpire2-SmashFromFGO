using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Voluntad del Shinsengumi (新选组之志, KIT) — DESIGN-OKITA §5.3. 2⚡ Poder: aplica
/// <see cref="ShinsengumiWillPower"/> (al final de tu turno, si jugaste 3+ Ataques: 5 Bloqueo y
/// +20 Carga NP) (up: coste 1⚡). Arquetipo Shukuchi defensivo.
/// </summary>
public sealed class ShinsengumiWill() : OkitaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ShinsengumiWillPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1); // 2 -> 1
}
