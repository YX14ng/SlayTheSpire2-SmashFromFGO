using MashShielder.MashShielderCode.Powers;
using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// LORD CAMELOT — NP card (min 100 charge, consumes ALL): the Castle of the Distant Utopia.
/// FGO Overcharge: +Block per 10 extra charge. Strength and Intercept always included.
/// Rediseño v2 (AUDITORÍA + fidelidad FGO: NP al 100%): mínimo 70 → 100.
/// </summary>
public sealed class LordCamelot() : MashShielderCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self), IMashNpCard, ICommandTyped, IBulwarkCard
{
    // Tipo de comando FGO de la ulti (audit 2026-07-05): el bono reforzado de CommandBonusPower
    // solo existia en LordCamelotUnleashed; el resto de las 7 cartas NP no lo recibia.
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;

    public const int ChargeCost = 100;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(23m, ValueProp.Move),
        new PowerVar<StrengthPower>("Strength", 3m),
        new PowerVar<ProvokePower>("Provoke", 12m),
        new PowerVar<Powers.InterceptPower>("Intercept", 3m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 4),
        new DynamicVar("AllyBlock", 12),
        new DynamicVar("AllyProvoke", 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<ProvokePower>()];

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
        // REDESIGN-MASH-V2 §6.3: el escudo de Camelot ES el contraataque. Con Baluarte de un solo
        // turno, una NP de 3⚡ + medidor lleno que sólo diera muro efímero quedaba floja; la
        // Intercepción PERMANENTE es la compensación temática y el puente arquetipo C → A. Apila
        // entre casteos (declarado en el diseño, no accidental).
        await PowerCmd.Apply<Powers.InterceptPower>(choiceContext, Owner.Creature, DynamicVars["Intercept"].BaseValue, Owner.Creature, this);

        // Co-op (Lord Camelot = «la fortaleza que escuda a TODA la Mesa Redonda»): cada aliado vivo
        // recibe una porción de Baluarte y de Intercepción-por-provocación, de modo que también
        // contraataque los golpes que bloquee. En 1 jugador PlayerCreatures es solo el Owner -> el
        // foreach queda vacío (idéntico a hoy).
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
        DynamicVars["Intercept"].UpgradeValueBy(2m);
    }
}
