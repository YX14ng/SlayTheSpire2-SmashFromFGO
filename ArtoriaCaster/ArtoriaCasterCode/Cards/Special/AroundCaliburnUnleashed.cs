using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Special;

/// <summary>
/// The ult manifested at 100 NP in Caster/Avalon form: AROUND CALIBURN unleashed —
/// «La Estrella de Esperanza que te Abraza». Cleanse + 3 Anti-Purga + 2 Strength
/// (first time per combat) + 12 Block. Overcharge: +1 AP per 30 over 100 (cap 5 =
/// the real NP's verified Count). Free, Exhaust.
/// </summary>
public sealed class AroundCaliburnUnleashed() : ArtoriaCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IArtoriaNpCard
{
    public const int ChargeCost = 100;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new DynamicVar("AntiPurge", 3),
        new PowerVar<StrengthPower>("Strength", 2m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerThirty", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    // La Fuerza solo la primera vez por combate (tope del panel de jueces).
    private bool _strengthGrantedThisCombat;

    public override Task BeforeCombatStart()
    {
        _strengthGrantedThisCombat = false;
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var extraAp = (tier - ChargeCost) / 30 * DynamicVars["PerThirty"].IntValue;
        var ap = Math.Min(AntiPurgePower.Max, DynamicVars["AntiPurge"].IntValue + extraAp);

        await RemoveOwnDebuffs();
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, ap, Owner.Creature, this);
        if (!_strengthGrantedThisCombat)
        {
            _strengthGrantedThisCombat = true;
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        }
        var block = NpLevels.Scale(Owner, DynamicVars.Block.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);

        // Co-op: aliados remueven debuffs y ganan 1 de Anti-Purga.
        foreach (var player in Owner.RunState.Players)
        {
            if (player == Owner || player.Creature.IsDead) continue;
            foreach (var debuff in player.Creature.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>().Where(p => p.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Debuff).ToList())
            {
                await PowerCmd.Remove(debuff);
            }
            await PowerCmd.Apply<AntiPurgePower>(player.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
