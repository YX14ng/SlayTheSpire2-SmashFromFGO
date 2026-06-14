using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Mariposas Negras (黑蝶 / Black Butterflies) — DESIGN-OBERON §6.3. 1⚡ Ataque: 3 de daño ×3 a enemigos
/// aleatorios; +5 Carga NP por cada golpe que dañe Vida (up 4×3). Multi-hit disperso con feed de NP
/// (lee attack.Results, patrón Andanada de Réplicas).
/// </summary>
public sealed class BlackButterflies() : OberonCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, ValueProp.Move), new DynamicVar("ChargePerHit", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(Hits).FromCard(this)
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var hpHits = attack.Results.Count(r => r.UnblockedDamage > 0);
        if (hpHits > 0)
        {
            await NpCharge.Gain(Owner.Creature, hpHits * DynamicVars["ChargePerHit"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
