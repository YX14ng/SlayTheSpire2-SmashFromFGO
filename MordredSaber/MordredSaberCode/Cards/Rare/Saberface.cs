using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Saberface (同脸) — DESIGN-MORDRED §5.3. 1⚡ Poder: la 1ª vez por turno que un enemigo te golpea, +10
/// Estrellas (up: y +10 NP). Defensa→★ con sabor meme. Aplica <see cref="SaberfacePower"/> y fija
/// StarsPerHit/NpPerHit + Upgraded desde la carta. El up habilita el +NP (no toca las ★).
/// </summary>
public sealed class Saberface() : MordredCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<SaberfacePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.StarsPerHit = DynamicVars["Stars"].IntValue;
            power.NpPerHit = DynamicVars["NpCharge"].IntValue;
            power.Upgraded = IsUpgraded;
        }
    }
}
