using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>
/// Instinto Depredador — Poder raro: concede <see cref="PredatoryInstinctPower"/> (el enjambre muerde
/// una vez más por acumulación). El payoff persistente del plan Laḫmu en Lily, sin esperar la ventana
/// Bestia — y con la ventana abierta se apilan (3 mordidas). Mejora: cuesta 1 menos (patrón GoldenRule).
/// También suma al hueco Rara/Poder=2 que rompía el evento de pociones.
/// </summary>
public sealed class PredatoryInstinct() : TiamatCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<PredatoryInstinctPower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PredatoryInstinctPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
