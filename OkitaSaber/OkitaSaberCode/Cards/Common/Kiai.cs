using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Kiai (气合) — DESIGN-OKITA §5.2. 0⚡ Hab, Exhaust: +20 Carga NP (up: +30). Batería con Exhaust.
/// </summary>
public sealed class Kiai() : OkitaCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m); // +20 -> +30
}
