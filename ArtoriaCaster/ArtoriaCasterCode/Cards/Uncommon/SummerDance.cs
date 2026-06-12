using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Danza de Verano — 9 de daño; ganás 1★; en forma Berserker: 2★ en vez de 1
/// (generación que premia pegar desde la forma agresiva).
/// </summary>
public sealed class SummerDance() : ArtoriaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new DynamicVar("Stars", 1),
        new DynamicVar("BerserkerStars", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<SummerBerserkerFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var stars = Owner.Creature.HasPower<SummerBerserkerFormPower>() || Owner.Creature.HasPower<AvalonFormPower>()
            ? DynamicVars["BerserkerStars"].IntValue
            : DynamicVars["Stars"].IntValue;
        await Stars.Gain(Owner.Creature, stars, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
