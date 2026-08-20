using MashShielder.MashShielderCode.Powers;
using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP while in PALADIN form (the fully-realized stance) —
/// Mash's true Noble Phantasm from FGO: LORD CHALDEAS (罗德·迦勒底亚斯), the wall that
/// protects everything. A direct upgrade over Lord Camelot: more Bulwark while preserving
/// its Strength, Intercept and co-op protection.
/// </summary>
public sealed class LordChaldeasUnleashed() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IMashNpCard, ICommandTyped, Cards.IBulwarkCard
{
    // Tipo de comando FGO de la ulti (audit 2026-07-05): el bono reforzado de CommandBonusPower
    // solo existia en LordCamelotUnleashed; el resto de las 7 cartas NP no lo recibia.
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;

    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(35m, ValueProp.Move),
        new PowerVar<StrengthPower>("Strength", 3m),
        new PowerVar<ProvokePower>("Provoke", 12m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 3),
        new DynamicVar("AllyBlock", 12),
        new DynamicVar("AllyProvoke", 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<ProvokePower>()];

    // Pasar la carta: el waiver de Pioneer NO cubre Event (parche P3) — sin él,
    // CanPay daría glow/playable falsos con el medidor vacío y un waiver activo.
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(choiceContext, Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;
        // NP level (dupes): +15% per level over the full amount, added as flat extra.
        var total = DynamicVars.Block.BaseValue + bonus;
        var extra = bonus + NpLevels.Scale(Owner, total) - total;

        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (extra > 0)
        {
            await BlockRetention.GainBulwarkBlock(this, Owner.Creature, extra);
        }
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<ProvokePower>(choiceContext, Owner.Creature, DynamicVars["Provoke"].BaseValue, Owner.Creature, this);

        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await BlockRetention.GainBulwarkBlock(this, ally, DynamicVars["AllyBlock"].BaseValue);
            await PowerCmd.Apply<ProvokePower>(choiceContext, ally, DynamicVars["AllyProvoke"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
