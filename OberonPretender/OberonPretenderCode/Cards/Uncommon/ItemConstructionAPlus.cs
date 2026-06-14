using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Construcción de Ítems A+ (道具作成A+ / Item Construction A+) — DESIGN-OBERON §6.3. 1⚡ Poder: tus
/// Débil y Vulnerable aplican +1 stack (el rocío tricolor con que maldijo a Titania). Aplica
/// <see cref="ItemConstructionPower"/>; el up enciende <see cref="ItemConstructionPower.RefundsCharge"/>
/// (+5 NP por cada Débil/Vulnerable aplicado, lo gestiona la carta que aplica el debuff).
/// </summary>
public sealed class ItemConstructionAPlus() : OberonCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<VulnerablePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<ItemConstructionPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.RefundsCharge = IsUpgraded;
    }
}
