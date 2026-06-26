using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>
/// Lluvia de Tesoros / 财宝之雨 (DESIGN-GILGAMESH §5) — el Rey vuelca su bóveda entera sobre la sala. 2⚡
/// At: 5 de daño a TODOS los enemigos, más <see cref="PerTreasure"/> (3) por cada *Tesoro que gastes
/// (hasta <see cref="MaxSpend"/>, 6). Gasta TODO el Tesoro disponible (hasta el tope). Glow cuando tenés
/// Tesoro. up: base 5→8.
///
/// El sumidero de Tesoro de fin de acto: convierte un medidor lleno en un AoE devastador. Usa
/// <see cref="TreasurePower"/> (contador mod-local, no el oro de la run).
/// </summary>
public sealed class RainOfTreasures() : GilgameshCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const int PerTreasure = 3;
    private const int MaxSpend = 6;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("PerTreasure", PerTreasure)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TreasurePower>()];

    protected override bool ShouldGlowGoldInternal => TreasurePower.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var spend = Math.Min(TreasurePower.Of(Owner.Creature), MaxSpend);
        var bonus = await TreasurePower.TrySpend(Owner.Creature, spend)
            ? spend * DynamicVars["PerTreasure"].IntValue
            : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this)
            .TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
