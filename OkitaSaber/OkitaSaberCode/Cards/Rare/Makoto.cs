using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER «Makoto (誠)» (诚) — DESIGN-OKITA §5.4. 2⚡ Poder: aplica <see cref="MakotoPower"/> (acumulador,
/// Amount 0) con Cap 10 (up: 16). Cada activación de tu umbral de 100★ suma +2 a tus Ataques este
/// combate, capeado al Cap. El estandarte: escalado capeado.
/// </summary>
public sealed class Makoto() : OkitaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Amount = 1 (marcador Single): PowerCmd.Apply retorna temprano con 0m y un Counter en 0
        // se auto-removería. El bono real arranca en 0 y vive en MakotoPower.Bonus (+2 por umbral
        // de 100★, capeado al Cap).
        var power = await PowerCmd.Apply<MakotoPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Cap = IsUpgraded ? MakotoPower.MaxBonusUpgraded : MakotoPower.MaxBonus;
    }
}
