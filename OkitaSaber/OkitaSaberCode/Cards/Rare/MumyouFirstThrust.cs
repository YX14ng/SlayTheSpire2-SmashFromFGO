using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// NP «Mumyou: Primera Estocada» (无明·一之突) — DESIGN-OKITA §5.4. 1⚡ At, consume TODA la Carga NP
/// (mín. 50): 14 daño IGNORANDO Bloqueo (Unblockable) a UN enemigo. SOBRECARGA: +2 por cada 10 sobre
/// 50. Escala +15%/nivel (NpLevels). El piso spameable de la economía. Daño directo Unblockable
/// (patrón <see cref="Special.MumyouUnleashed"/>). up: 18. Glow cuando es pagable.
/// </summary>
public sealed class MumyouFirstThrust() : OkitaCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int ChargeCost = 50;
    private const int OverchargePerTen = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move | ValueProp.Unblockable),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / 10 * OverchargePerTen;
        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge);

        VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_dramatic_stab");
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage,
            ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m); // 14 -> 18
}
