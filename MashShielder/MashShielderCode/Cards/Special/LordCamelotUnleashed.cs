using FGOCore.FGOCoreCode.CardTypes;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP while in SHIELDER form (the default stance) — LORD
/// CAMELOT (理想之城), the iconic Noble Phantasm of Mash's shield. Generated for free
/// (into hand, cost 0) the first time the NP gauge reaches 100 in a combat. Playing
/// it consumes the full gauge.
/// </summary>
public sealed class LordCamelotUnleashed() : MashShielderCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IMashNpCard, ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Lord Camelot = Arts) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(30m, ValueProp.Move),
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
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
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

        // Co-op: misma fantasía/NP que LORD CAMELOT -> espejo exacto de su reparto a aliados.
        // Cada aliado vivo recibe Baluarte e Intercepción-por-provocación. En 1 jugador el foreach
        // queda vacío (fiel a 1 jugador).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
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
