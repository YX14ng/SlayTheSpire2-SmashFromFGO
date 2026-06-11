using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Último Recurso (最后的度假胜地) — Aesc's S3 with its real FGO mechanic: cannot
/// be played before your 5th turn. NP Charge +50 and Guts (1 HP). Exhaust.
/// </summary>
public sealed class LastResort() : MorganCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 50),
        new DynamicVar("Turn", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<GutsPower>()];

    protected override bool IsPlayable =>
        Owner.Creature.CombatState.RoundNumber >= DynamicVars["Turn"].IntValue;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await PowerCmd.Apply<GutsPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Turn"].UpgradeValueBy(-1m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
