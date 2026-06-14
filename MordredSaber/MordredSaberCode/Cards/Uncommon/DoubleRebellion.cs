using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Doble Rebelión (双重叛逆) — DESIGN-MORDRED §5.2. 1⚡ At: 7 de daño; si cambiaste de forma este
/// combate (<see cref="FormShiftedPower"/>): +10 Estrellas y +10 de Carga NP (up +3 daño), glow. Payoff
/// de la danza del casco (haber bailado el switch al menos una vez). Los riders NO suben con el up.
/// Patrón Double Rebellion (lee FormShiftedPower, el marcador de cambio-de-forma de FGOCore).
/// </summary>
public sealed class DoubleRebellion() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    private bool Shifted => Owner.Creature.HasPower<FormShiftedPower>();

    protected override bool ShouldGlowGoldInternal => Shifted;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var shifted = Shifted;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (shifted)
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
