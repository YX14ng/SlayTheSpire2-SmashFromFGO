using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Servomotores del Ortinax — Power: aggression feeds the wall AND the gauge.
/// Rediseño v2: al final de tu turno, si jugaste 2+ Ataques: +6 Bloqueo (up +2) y +10 NP
/// (fijo). Gastás Bloqueo atacando y los servos lo reponen — deja de ser borderline.</summary>
public sealed class OrtinaxServos() : MashShielderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<OrtinaxServosPower>("OrtinaxServos", 6m),
        new DynamicVar("NpCharge", OrtinaxServosPower.NpPerProc)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OrtinaxServosPower>(Owner.Creature, DynamicVars["OrtinaxServos"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["OrtinaxServos"].UpgradeValueBy(2m);
    }
}
