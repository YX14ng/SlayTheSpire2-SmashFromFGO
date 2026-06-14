using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// NP «Mumyou Sandanzuki» (manual) (无明三段突) — DESIGN-OKITA §5.4. 2⚡ At, consume TODA la Carga NP
/// (mín. 70): 10 daño ×3 a UN enemigo IGNORANDO Bloqueo (Unblockable); tras el daño: 2 Vulnerable.
/// SOBRECARGA: +1/golpe por cada 10 sobre 70. Escala +15%/nivel (NpLevels). Dispararla ANTES del
/// auto-ulti (regla Morgan). Daño directo Unblockable (patrón <see cref="Special.MumyouUnleashed"/>).
/// up: 12×3 y Vulnerable 4. Glow cuando es pagable.
/// </summary>
public sealed class MumyouManual() : OkitaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 70;
    private const int Hits = 3;
    private const int OverchargePerTen = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<VulnerablePower>("Vulnerable", 2m),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CritReadyPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var perHit = (tier - ChargeCost) / 10 * OverchargePerTen;
        var perHitDamage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + perHit);

        for (var i = 0; i < Hits; i++)
        {
            if (cardPlay.Target.IsDead) break;
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_dramatic_stab");
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, perHitDamage,
                ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
        }

        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);        // 10 -> 12
        DynamicVars["Vulnerable"].UpgradeValueBy(2m); // 2 -> 4
    }
}
