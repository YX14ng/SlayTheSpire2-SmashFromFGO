using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Basic;

/// <summary>Caparazón Larval — Baluarte básico (Bulwark, retenido entre turnos): 6 + 1 por cada
/// Laḫmu en campo (hasta +3). La cría no solo muerde: blinda a la madre mientras cría el enjambre.</summary>
public sealed class Carapace() : TiamatCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // Tag Defend vanilla: es el Bloqueo básico de Tiamat. Sin un Basic+Defend, LargeCapsule (巨大扭蛋)
    // hace CardPool.AllCards.First(Basic && Defend) y CRASHEA al obtenerse (InvalidOperationException).
    // También habilita sinergias que cuentan "Defensas".
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("PerLahmu", 1),
        new DynamicVar("MaxBonus", 3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BulwarkPower>(),
        HoverTipFactory.FromPower<LahmuSwarmPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = Math.Min(Lahmu.Count(Owner.Creature) * DynamicVars["PerLahmu"].IntValue, DynamicVars["MaxBonus"].IntValue);
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue + bonus);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
