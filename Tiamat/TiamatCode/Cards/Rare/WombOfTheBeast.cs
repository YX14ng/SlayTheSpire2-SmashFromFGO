using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>
/// Vientre de la Bestia — Poder raro, motor de escalado del enjambre: concede <see cref="BroodNurturePower"/>
/// (al inicio de cada turno tuyo, +1 Crianza por acumulación). Cada Crianza hace que TODAS las mordidas de
/// Laḫmu peguen más fuerte, así el daño del enjambre escala solo turno a turno (el "DemonForm" de la cría).
/// Mejora: empieza con 1 acumulación extra (+2 Crianza por turno).
/// </summary>
public sealed class WombOfTheBeast() : TiamatCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BroodNurturePower>(),
        HoverTipFactory.FromPower<LahmuNurturePower>(),
        HoverTipFactory.FromPower<LahmuSwarmPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BroodNurturePower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(1m);
}
