using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>
/// Soborno Imposible / 不可能的贿赂 (DESIGN-GILGAMESH §5) — el oro del Rey compra el golpe que ningún
/// mestizo puede parar. 1⚡ At: 8 de daño base; si gastás <see cref="TreasureCost"/> (3) de *Tesoro,
/// inflige <see cref="Bonus"/> (10) más. Glow cuando podés pagar el bonus. up: base 8→11.
///
/// A diferencia de la Inversión Real, el Tesoro acá es OPCIONAL: la carta SIEMPRE pega su base, y el
/// Tesoro sólo amplifica (no es gate de jugabilidad). Usa <see cref="TreasurePower"/> (contador
/// mod-local, no el oro de la run).
/// </summary>
public sealed class ImpossibleBribe() : GilgameshCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int TreasureCost = 3;
    private const int Bonus = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Treasure", TreasureCost),
        new DynamicVar("Bonus", Bonus)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TreasurePower>()];

    protected override bool ShouldGlowGoldInternal => TreasurePower.CanAfford(Owner.Creature, DynamicVars["Treasure"].IntValue);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = await TreasurePower.TrySpend(Owner.Creature, DynamicVars["Treasure"].IntValue)
            ? DynamicVars["Bonus"].IntValue
            : 0;
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue + bonus);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
