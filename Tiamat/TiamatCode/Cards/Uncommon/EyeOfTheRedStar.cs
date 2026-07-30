using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Ojo de la Estrella Roja — EL puente explícito de Maldición (REDESIGN-TIAMAT §36). Sembrás
/// !Curse! de Maldición y bajás !Strength! de Fuerza al objetivo; si YA estaba bien maldito
/// (!Threshold!+), la Maldición se desborda en !Overflow! extra (total 7). La carta que decide
/// "este enemigo es la presa que la Bestia cosechará dos veces".
/// </summary>
public sealed class EyeOfTheRedStar() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 4),
        new DynamicVar("Strength", 2),
        new DynamicVar("Threshold", 6),
        new DynamicVar("Overflow", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>(), HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var amount = DynamicVars["Curse"].IntValue;
        // El puente: si la presa ya cargaba ≥Threshold de Maldición, la marea la desborda.
        if (Curses.Of(cardPlay.Target) >= DynamicVars["Threshold"].IntValue)
        {
            amount += DynamicVars["Overflow"].IntValue;
        }
        await Curses.Apply(choiceContext, cardPlay.Target, amount, Owner.Creature, this);
        // −Fuerza con el patrón vanilla Malaise (StrengthPower negativo).
        await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -DynamicVars["Strength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Curse"].UpgradeValueBy(2m);
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
