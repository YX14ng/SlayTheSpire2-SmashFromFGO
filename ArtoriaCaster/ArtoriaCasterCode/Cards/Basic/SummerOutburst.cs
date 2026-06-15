using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Basic;

/// <summary>
/// Arrebato de Verano («¡Intentémoslo! ¡Si fallo, que así sea!») — signature basic 1:
/// 6 damage, THEN enter Berserker form (damage resolves before the switch so it never
/// self-buffs). Critical 2★: 12 (only crits if you were ALREADY in Berserker/Avalon).
/// </summary>
public sealed class SummerOutburst() : ArtoriaCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    public const int CritCost = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Crit", 12),
        new DynamicVar("CritCost", CritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<SummerBerserkerFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = await ResolveCritDamage(CritCost);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // El daño se resuelve ANTES del cambio: la carta no se auto-buffea.
        await FormSwitch.Enter<SummerBerserkerFormPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Crit"].UpgradeValueBy(4m);
    }
}
