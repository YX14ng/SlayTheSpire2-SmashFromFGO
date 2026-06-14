using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Vísperas del Fin (终焉的晚祷 / Vespers of the End) — DESIGN-OBERON §6.4. 1⚡ Poder: SOLO en VORTIGERN,
/// tus Ataques hacen +3 (up +5). Fuera de Vortigern no hace nada. Aplica <see cref="VespersOfTheEndPower"/>
/// y le fija el bonus desde el DynamicVar. Glow en Vortigern.
/// </summary>
public sealed class VespersOfTheEnd() : OberonCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bonus", 3)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<VortigernPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<VespersOfTheEndPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Bonus = DynamicVars["Bonus"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["Bonus"].UpgradeValueBy(2m);
}
