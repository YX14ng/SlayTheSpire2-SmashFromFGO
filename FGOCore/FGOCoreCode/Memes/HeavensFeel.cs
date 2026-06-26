using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Heaven's Feel (天之杯) — tu PRÓXIMO Ataque (incluye tu próximo NP, que resuelve como Ataque
/// con poder) hace +40% de daño, SIN coste de HP (a diferencia del Grial Negro). Buff de un
/// solo uso (<see cref="HeavensFeelPower"/>): se consume al primer Ataque o al fin de turno.
/// </summary>
public sealed class HeavensFeel() : MemeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    // Sin var numérica: el +40% es fijo (HeavensFeelPower.DamageMultiplier) y va literal en la loc.
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HeavensFeelPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HeavensFeelPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Mejora: deja de gastar el slot — ya no Exhaust, así repite el +40% cada combate.
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
