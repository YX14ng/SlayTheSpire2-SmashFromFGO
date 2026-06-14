using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Shukuchi B+ (缩地B+, KIT) — DESIGN-OKITA §5.3. 1⚡ Hab, Exhaust: +2 *Aliento; robá 2; este turno
/// tus Ataques hacen +3 (<see cref="SwiftStancePower"/>) (up: +3 Aliento; robá 2; +4). El Quick Up +
/// estado pre-ataque ATK+20% del skill real (fidelidad §2).
/// </summary>
public sealed class ShukuchiBPlus() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Breath", 2), new CardsVar(2), new PowerVar<SwiftStancePower>("SwiftStance", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Aliento.Gain(Owner.Creature, DynamicVars["Breath"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<SwiftStancePower>(Owner.Creature, DynamicVars["SwiftStance"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Breath"].UpgradeValueBy(1m);       // +2 -> +3
        DynamicVars["SwiftStance"].UpgradeValueBy(1m);  // +3 -> +4
    }
}
