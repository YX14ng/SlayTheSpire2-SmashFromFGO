using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Special;

/// <summary>
/// Vajra, el Rayo / 金刚杵 (DESIGN-GILGAMESH §5.5, Arma del Tesoro #4) — el rayo de Indra, el único Arma
/// AoE del arsenal. Token 0⚡ Exhaust generado (<c>CardRarity.Event</c>). 4 de daño a TODOS los enemigos
/// (builder fluido <c>TargetingAllOpponents</c>, patrón ChainedLightning).
/// </summary>
public sealed class Vajra() : GilgameshCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await ArmsPlayedPower.Record(Owner.Creature);
    }
}
