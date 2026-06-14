using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Espada del Padre (父亲之剑) — DESIGN-MORDRED §5.2. 2⚡ At: 14 de daño; si tenés ≥50 de Carga NP
/// (SIN gastarla): +6 (up +4 daño / +8 bonus), glow. Lee el banco sin consumirlo (patrón UtopianFortress;
/// su deseo del Grial: la espada que quiso del padre). Threshold y lectura vía
/// <see cref="NpCharge.Current"/>. El umbral 50 NO sube con el up.
/// </summary>
public sealed class FathersSword() : MordredCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int NpThreshold = 50;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(14m, ValueProp.Move), new DynamicVar("Bonus", 6), new DynamicVar("Threshold", NpThreshold)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => NpCharge.Current(Owner.Creature) >= NpThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = NpCharge.Current(Owner.Creature) >= NpThreshold ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Bonus"].UpgradeValueBy(8m);
    }
}
