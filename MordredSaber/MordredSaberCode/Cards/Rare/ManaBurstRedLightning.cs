using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Cards;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Estallido de Maná: Relámpago Rojo (魔力放出·赤雷) — DESIGN-MORDRED §5.3. 1⚡ At NP single-target: el
/// piso spameable de la economía NP. Mín 50, consume TODA la carga: 16 a UN enemigo, +4 por cada 10 de
/// carga sobre el mínimo (up: base 22). Glow al poder pagarse. Mini-NP barato que cierra el ciclo de
/// ultis. Implementa IMordredNpCard + ConsumeAllForNpCard + NpLevels.Scale como las tokens.
/// </summary>
public sealed class ManaBurstRedLightning() : MordredCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IMordredNpCard
{
    public const int ChargeCost = 50;
    private const int OverchargePer = 10; // +4 por cada 10 sobre el mínimo

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move),
        new DynamicVar("OverchargeDamage", 4),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / OverchargePer * DynamicVars["OverchargeDamage"].IntValue;

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
