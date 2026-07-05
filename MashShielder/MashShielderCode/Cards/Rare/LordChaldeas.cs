using MashShielder.MashShielderCode.Powers;
using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// LORD CHALDEAS — NP card (min 50 charge, consumes ALL): a fortress of Bulwark Block.
/// FGO Overcharge: +Block per 10 charge consumed beyond the minimum.
/// </summary>
public sealed class LordChaldeas() : MashShielderCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self), IMashNpCard, ICommandTyped
{
    // Tipo de comando FGO de la ulti (audit 2026-07-05): el bono reforzado de CommandBonusPower
    // solo existia en LordCamelotUnleashed; el resto de las 7 cartas NP no lo recibia.
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;

    public const int ChargeCost = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(24m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3),
        new DynamicVar("AllyBlock", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        // NP level (dupes): +15% per level over the full amount, added as flat extra.
        var total = DynamicVars.Block.BaseValue + bonus;
        var extra = bonus + NpLevels.Scale(Owner, total) - total;

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature,
            (BlockVar)DynamicVars.Block, cardPlay);
        if (extra > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, extra);
        }

        // Co-op (NP propio de Mash, la muralla de Chaldeas que ampara a TODOS): cada aliado vivo
        // recibe una porción fija de Baluarte. En 1 jugador PlayerCreatures es solo el Owner -> el
        // foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await BlockRetention.GainBulwarkBlock(this, ally, DynamicVars["AllyBlock"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
