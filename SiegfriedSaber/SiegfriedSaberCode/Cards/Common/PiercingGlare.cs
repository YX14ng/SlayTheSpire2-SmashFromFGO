using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Mirada Perforante (洞穿之瞳 / Piercing Glare) — el PRIMER payoff de Estrellas de Siegfried
/// (DESIGN-REVIEW-2: generaba ★ con GraniStalk/Das Rheingold pero casi no tenía dónde gastarlas; las ★
/// quedaban vestigiales). 1⚡ At: 8 de daño; si tenés al menos <see cref="StarThreshold"/> ★, +6 de daño
/// más (sin gastarlas — es lectura de banca, igual que EyesOnTheTip de Okita). El ojo del cazador que
/// halla el punto débil cuando hay luz de estrellas para apuntar. Glow cuando ≥30★.
/// </summary>
public sealed class PiercingGlare() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const int StarThreshold = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Bonus", 6), new DynamicVar("Threshold", StarThreshold)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => CritStars.Of(Owner.Creature) >= StarThreshold;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = CritStars.Of(Owner.Creature) >= StarThreshold ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
