using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Basic;

/// <summary>Caparazón Larval — Baluarte básico (Bulwark, retenido entre turnos): 6 de Baluarte,
/// más 1 de Bloqueo NORMAL por cada Laḫmu en campo (hasta +3). Rebalance 2026-08-15: el bono por
/// Laḫmu era Baluarte también y componía el piso retenido con las 3 copias del mazo inicial
/// (docs/REBALANCE-TIAMAT-ARTORIA.md); los cuerpos de la cría amortiguan el golpe, no se
/// endurecen — solo el caparazón propio se retiene.</summary>
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
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue);
        var bonus = Math.Min(Lahmu.Count(Owner.Creature) * DynamicVars["PerLahmu"].IntValue, DynamicVars["MaxBonus"].IntValue);
        if (bonus > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, bonus, ValueProp.Unpowered, null);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
