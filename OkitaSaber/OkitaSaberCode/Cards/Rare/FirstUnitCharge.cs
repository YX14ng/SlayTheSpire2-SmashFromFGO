using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Carga de la Primera Unidad (一番队突击) — DESIGN-OKITA §5.4. 2⚡ At: +100★ PRIMERO (cruza el umbral,
/// el auto-payoff arma un CRÍTICO LISTO), LUEGO 12 daño (sale ×2 con el Crítico Listo recién armado)
/// (up: 16). Patrón 黑闪/Black: dispara el umbral y se cobra a sí misma.
/// </summary>
public sealed class FirstUnitCharge() : OkitaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int Stars = 100;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // PRIMERO las ★: el auto-payoff de CritStarsPower a 100 arma el Crítico Listo...
        await CritStars.Gain(Owner.Creature, Stars, this);
        // ...que el golpe siguiente consume (×2).
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m); // 12 -> 16
}
