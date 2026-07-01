using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Common;

/// <summary>
/// Marea Corrosiva — Poder común, motor de campo: concede <see cref="CorrosiveTidePower"/> (al inicio de
/// cada turno tuyo, 1 Maldición a TODOS los enemigos por acumulación). Siembra el campo de Maldición que
/// el enjambre cosecha (muerde al más maldito) y que las cartas de Maldición cobran. Mejora: empieza con
/// 1 acumulación extra (2 Maldición a todos por turno).
/// </summary>
public sealed class CorrosiveTide() : TiamatCard(1, CardType.Power, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CorrosiveTidePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CorrosiveTidePower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(1m);
}
