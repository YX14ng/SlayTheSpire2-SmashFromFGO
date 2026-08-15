using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Respiro Feérico (妖精的喘息) — nuevo común P2 2026-06-25: 0⚡, cura 3 HP, Exhaust (up: cura 5).
/// El eje "Sangre de la Reina" gastaba Vida sin forma de recuperarla salvo en raras; este es el
/// piso de cura barato que sostiene el plan de autodaño con HP base 72. Modelado sobre SaviorsTears.
/// </summary>
public sealed class FaeRespite() : MorganCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(4m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2m);
    }
}
