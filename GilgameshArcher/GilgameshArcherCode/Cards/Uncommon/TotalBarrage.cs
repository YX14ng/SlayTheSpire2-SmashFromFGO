using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Andanada Total (全弹齐射) — payoff de tribu escalado. 2⚡ At: 4 de daño ×4 a enemigos
/// aleatorios; +1 golpe por cada Arma del Tesoro jugada este turno (máx +2). Glow cuando jugaste un Arma.
/// up: daño por golpe 4→5. Lee <see cref="ArmsPlayedPower"/> (0 hasta que el Arsenal lo alimente).</summary>
public sealed class TotalBarrage() : GilgameshCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    private const int BaseHits = 4;
    private const int MaxExtraHits = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    protected override bool ShouldGlowGoldInternal => ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var extraHits = Math.Min(ArmsPlayedPower.PlayedThisTurn(Owner.Creature), MaxExtraHits);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(BaseHits + extraHits).FromCard(this)
            .TargetingRandomOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
