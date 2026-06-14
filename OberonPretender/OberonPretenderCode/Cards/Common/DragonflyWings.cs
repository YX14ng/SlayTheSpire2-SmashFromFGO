using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Stars;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Alas de Libélula (蜻蜓之翼 / Dragonfly Wings) — DESIGN-OBERON §6.2. 1⚡ Ataque: 8 de daño; en El
/// Príncipe del Invierno: +<see cref="WinterStars"/> Estrellas (up 11). El payoff ofensivo de Invierno
/// (las alas venosas que serán Vortigern): mientras diferís la Deuda, el golpe cosecha estrellas. Lee
/// <c>HasPower<WinterPrincePower>()</c> directo. El up sube el daño; las ★ del rider quedan fijas.
/// Glow en Invierno.
/// </summary>
public sealed class DragonflyWings() : OberonCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int WinterStars = 10;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Stars", WinterStars)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<WinterPrincePower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (Owner.Creature.HasPower<WinterPrincePower>())
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
