using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Cobro Final (最后的清算) — the Tyranny detonator: consumes ALL the target's
/// Curse and deals 2 damage per point (forfeiting the deferred damage).
/// Implementa <see cref="IUsesTargetCurse"/>: en forma Reina/Invierno la pasiva "Sentencia"
/// consumía la Maldición del objetivo en BeforeCardPlayed (antes de este OnPlay), dejando esta carta
/// en 0 → no hacía daño (reporte de player). El marcador hace que la Sentencia la saltee.
/// </summary>
public sealed class FinalCollection() : MorganCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy), IUsesTargetCurse
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Move),
        new DynamicVar("PerPoint", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override bool IsPlayable => true;

    protected override bool ShouldGlowGoldInternal =>
        Curses.MostCursed(Owner.Creature) != null;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var consumed = await Curses.Consume(choiceContext, cardPlay.Target, CursePower.MaxPerEnemy);
        if (consumed <= 0) return;

        // Re-tipada a Habilidad (resolución §9.3-2, 2-1): daño Unpowered — el cash-out total de
        // la Maldición no debe escalar ADEMÁS con Fuerza/tipo Buster. Como Habilidad, la Sentencia
        // ya ni la considera (los form powers cortan en Type != Attack antes de detonar); el
        // marcador IUsesTargetCurse queda por robustez ante refactors futuros.
        await CreatureCmd.Damage(choiceContext, cardPlay.Target,
            consumed * DynamicVars["PerPoint"].IntValue, ValueProp.Unpowered, Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerPoint"].UpgradeValueBy(1m);
    }
}
