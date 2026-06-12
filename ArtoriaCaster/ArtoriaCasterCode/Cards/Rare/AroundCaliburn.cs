using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Around Caliburn («La Estrella de Esperanza que te Abraza») — Habilidad NP 2⚡
/// (mínimo 70 NP, consume TODA la carga): removés tus debuffs; 2 Anti-Purga;
/// 2 Fuerza; 8 Bloqueo. Co-op: aliados remueven debuffs y ganan 1 Fuerza.
/// SOBRECARGA: +1 Anti-Purga por cada 20 sobre el mínimo (tope 5).
/// Mejora: 3 Fuerza; 12 Bloqueo.
/// </summary>
public sealed class AroundCaliburn() : ArtoriaCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self), IArtoriaNpCard
{
    public const int ChargeCost = 70;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new DynamicVar("AntiPurge", 2),
        new PowerVar<StrengthPower>("Strength", 2m),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTwenty", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var extraAp = (tier - ChargeCost) / 20 * DynamicVars["PerTwenty"].IntValue;
        var ap = Math.Min(AntiPurgePower.Max, DynamicVars["AntiPurge"].IntValue + extraAp);

        await RemoveOwnDebuffs();
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, ap, Owner.Creature, this);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        var block = NpLevels.Scale(Owner, DynamicVars.Block.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);

        // Co-op: aliados remueven debuffs y ganan 1 de Fuerza.
        foreach (var player in Owner.RunState.Players)
        {
            if (player == Owner || player.Creature.IsDead) continue;
            foreach (var debuff in player.Creature.GetPowerInstances<MegaCrit.Sts2.Core.Models.PowerModel>().Where(p => p.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Debuff).ToList())
            {
                await PowerCmd.Remove(debuff);
            }
            await PowerCmd.Apply<StrengthPower>(player.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Strength"].UpgradeValueBy(1m);
    }
}
