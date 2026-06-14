using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Doble Filo del Odio (憎恶双刃) — DESIGN-MORDRED §5.3. 2⚡ Poder: en Rebelión, tus Ataques hacen +3
/// adicional (up +4). Stack del arquetipo de forma ofensiva (redundante adrede). Aplica
/// <see cref="DoubleEdgeOfHatredPower"/> y fija AttackBonus desde el DynamicVar. El up sube el bonus.
/// </summary>
public sealed class DoubleEdgeOfHatred() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("AttackBonus", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RebellionFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<DoubleEdgeOfHatredPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.AttackBonus = DynamicVars["AttackBonus"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["AttackBonus"].UpgradeValueBy(1m);
}
