using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Sentido del Prodigio (天才之感, KIT) — DESIGN-OKITA §5.3. 1⚡ Poder: aplica
/// <see cref="ProdigySensePower"/> (cada vez que se activa tu umbral de 100★: +10 Carga NP y robá 1)
/// (up: +20 NP). Engorda el hilo ★→NP. Fija el NpGain del power desde el DynamicVar.
/// </summary>
public sealed class ProdigysSense() : OkitaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ProdigySensePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.NpGain = DynamicVars["NpCharge"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m); // +10 -> +20
}
