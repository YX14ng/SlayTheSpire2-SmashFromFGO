using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Relámpago Encadenado (连锁闪电) — DESIGN-MORDRED §5.2. 1⚡ At AoE: 6 de daño a TODOS; si CONSUMISTE
/// un Crítico este turno, +6 a todos (up +2/+2), glow. El crítico se encadena entre rivales. Leído con
/// el marcador <see cref="CritConsumedThisTurnPower"/> (lo aplica RedLightningChannelPower). Patrón
/// ResidualLightning (AoE) + InsolentStrike (rider crítico-consumido).
/// </summary>
public sealed class ChainedLightning() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Bonus", 6)];

    private bool CritConsumed => Owner.Creature.HasPower<CritConsumedThisTurnPower>();

    protected override bool ShouldGlowGoldInternal => CritConsumed;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = CritConsumed ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
