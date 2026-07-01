using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>
/// Nido de Cría — el MOTOR de población persistente (DESIGN-REVIEW-2 §2). Concede
/// <see cref="BroodMotherPower"/>: al inicio de cada turno tuyo parís 1 Laḫmu por acumulación. El
/// enjambre se rellena solo, así la mordida de Lily y la cosecha Bestia nunca se quedan sin cría.
/// Carta de PODER (2026-07-01): concede un power persistente, así que es un Poder, no una Habilidad
/// (además, Tiamat necesitaba cartas tipo Poder — sin ellas la TIENDA crasheaba). Las cartas de Poder
/// ya salen de juego al aplicarse: el Exhaust explícito era redundante y se quitó.
/// </summary>
public sealed class BroodNest() : TiamatCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BroodMotherPower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BroodMotherPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(1m);
}
