using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #11 Velo de Niebla (雾之帷幕) — rediseño 2026-06-15 (swap Estrellas→Maldición): espejo A del
/// par NP↔Maldición: 0⚡, si tenés ≥50 de Carga NP, perdé 50 y aplicá 5 de Maldición a TODOS los
/// enemigos (el maná se condensa en niebla maldita). Convierte el medidor en bomba sembrada.
/// Glow cuando pagable. (up: consume 40 — neto +1 de Maldición eficaz; NO bajar a 30 sin playtest)
/// </summary>
public sealed class MistVeil() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ChargeCost", 50),
        new DynamicVar("Curse", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CursePower>()
    ];

    // Carga REAL, sin waiver: los espejos no son cartas NP y no deben quemar el comodín.
    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["ChargeCost"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!await NpCharge.Spend(Owner.Creature, DynamicVars["ChargeCost"].IntValue, this)) return;
        foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).Where(e => !e.IsDead).ToList())
        {
            await Curses.Apply(enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ChargeCost"].UpgradeValueBy(-10m);
    }
}
