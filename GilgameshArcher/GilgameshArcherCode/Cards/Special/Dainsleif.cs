using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Dáinsleif, la Espada Maldita / 咒剑·达因斯莱布 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #7) — la espada que
/// una vez desenvainada exige sangre. Token 0⚡ Exhaust generado (<c>CardRarity.Event</c>). 8 de daño (el
/// pegador más fuerte del arsenal) a cambio de perder 2 HP — autodaño SIEMPRE con
/// <c>Unblockable | Unpowered</c> (regla del autodaño, patrón MapoTofu / Black Barrel).
/// </summary>
public sealed class Dainsleif() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new HpLossVar(2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
