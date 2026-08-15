using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// El Precio de la Corona (王冠的代价) — RE-POOL V2 [NUEVA] (§5.2, versión P3 OBLIGATORIA
/// J1-6/J3-8; la versión poder de P1 quedó prohibida): Habilidad 0⚡: perdés 3 HP; ganás 1
/// Energía. Agotar (mejora: perdés 2). El Seeing Red a sangre de la línea D — energía con un
/// costo real que además siembra vía el cetro.
/// </summary>
public sealed class PriceOfTheCrown() : MorganCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(3m),
        new DynamicVar("Energy", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmdCompatibility.DamageFromCard(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        if (Owner.Creature.IsDead) return;
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.HpLoss.UpgradeValueBy(-1m);
    }
}
