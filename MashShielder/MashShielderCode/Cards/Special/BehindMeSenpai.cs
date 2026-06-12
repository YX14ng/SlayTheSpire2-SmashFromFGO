using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// ¡Detrás de mí! — generated each turn in multiplayer (free, Ethereal):
/// until your next turn, Mash takes the damage that pierces her allies' defenses.
/// Rediseño v2: cubrir es contraatacar — además +3 de Intercepción este turno y
/// 5 de Bloqueo; cada golpe que detenga también paga estrellas vía reliquia.
/// De isla co-op a pieza del hilo de Intercepción.
/// </summary>
public sealed class BehindMeSenpai() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<CoverPower>(1m),
        new PowerVar<ProvokePower>("Provoke", 3m),
        new BlockVar(5m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CoverPower>(), HoverTipFactory.FromPower<ProvokePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CoverPower>(Owner.Creature, 1m, Owner.Creature, this);
        await PowerCmd.Apply<ProvokePower>(Owner.Creature, DynamicVars["Provoke"].BaseValue, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
    }
}
