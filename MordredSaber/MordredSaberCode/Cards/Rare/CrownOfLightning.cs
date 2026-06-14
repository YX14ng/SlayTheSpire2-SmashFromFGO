using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Corona del Relámpago (雷之冠) — DESIGN-MORDRED §5.3. 2⚡ Poder: al inicio de cada turno +10 Estrellas
/// (up +20). Per-turn ★, slot Ángel. Aplica <see cref="CrownOfLightningPower"/> y fija su StarsPerTurn
/// desde el DynamicVar (para que el up se refleje sin chocar con el conteo de stacks, patrón
/// WeightOfExpectations). Sube SOLO el valor por activación.
/// </summary>
public sealed class CrownOfLightning() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<CrownOfLightningPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.StarsPerTurn = DynamicVars["Stars"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
