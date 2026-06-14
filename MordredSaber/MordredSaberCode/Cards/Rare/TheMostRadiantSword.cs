using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// La Espada Más Resplandeciente (最耀眼之剑) — DESIGN-MORDRED §5.3. 2⚡ Poder: tus CRÍTICOS hacen +8
/// adicional (up +12) y devuelven +10 NP extra al consumirse. Clarent cose ★→NP (precedente Lupa).
/// Aplica <see cref="TheMostRadiantSwordPower"/> y fija CritBonus desde el DynamicVar (el +Crítico se
/// dobla con el ×2; el +NP/consumo va por ICritConsumedListener). El up sube SOLO el daño-crítico.
/// </summary>
public sealed class TheMostRadiantSword() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("CritBonus", 8), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<TheMostRadiantSwordPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.CritBonus = DynamicVars["CritBonus"].IntValue;
            power.NpOnConsume = DynamicVars["NpCharge"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["CritBonus"].UpgradeValueBy(4m);
}
