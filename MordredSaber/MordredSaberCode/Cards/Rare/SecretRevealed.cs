using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Secreto Revelado (秘密揭露) — DESIGN-MORDRED §5.3. 2⚡ Poder: cada vez que te quitás el yelmo (entrás
/// en Rebelión), +20 Estrellas y robá 1 (up: 1⚡). El motor de la danza del casco. Aplica
/// <see cref="SecretRevealedPower"/> (IFormChangeListener) y fija Stars desde el DynamicVar. El up baja
/// el coste (no toca las ★).
/// </summary>
public sealed class SecretRevealed() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebellionFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<SecretRevealedPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.Stars = DynamicVars["Stars"].IntValue;
            power.Cards = DynamicVars.Cards.IntValue;
        }
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
