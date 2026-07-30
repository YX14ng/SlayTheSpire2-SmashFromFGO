using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TiamatBeast.TiamatCode.Cards.Basic;

/// <summary>Ojo de la Estrella Azul — firma del mazo inicial (azur, la mirada serena de la Diosa):
/// acelera la Carga NP y NIEGA a la presa más maldita. La Estrella Azul abre la ventana; la Roja
/// (poco común) acuña el puente. Dirige el castigo a <see cref="Curses.MostCursed"/> — el mismo
/// imán que sigue el enjambre.
/// NOTA DE PORTABILIDAD: el redesign pedía "Bloqueo de Curación + −25% crit enemigo"; FGOCore aún
/// NO expone esos powers (ni el juego tiene crit enemigo), así que la negación se entrega como
/// Debilidad (−daño) 1 turno sobre el más maldito. Sustituir por un HealBlockPower/CritDownPower de
/// FGOCore cuando existan (no tocar FGOCore desde acá).</summary>
public sealed class EyeOfTheBlueStar() : TiamatCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 14),
        new DynamicVar("Weak", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CursePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);

        var prey = Curses.MostCursed(Owner.Creature);
        if (prey != null)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, prey, DynamicVars["Weak"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(6m);
}
