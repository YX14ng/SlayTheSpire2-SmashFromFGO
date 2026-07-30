using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Velocidad del Relámpago (雷速) — DESIGN-MORDRED §5.2. 1⚡ At: 9 de daño + <see cref="BaseStars"/> Estrellas;
/// en Rebelión, +10 Estrellas en su lugar (up +3 daño), glow. Payoff de la forma ofensiva (sin casco, el
/// relámpago fluye → ★). Leído con <see cref="Forms.InRebellion"/>.
///
/// BI-CONDICIONAL SUAVE (DESIGN-REVIEW-2): antes daba 0 ★ fuera de Rebelión. Ahora un PISO
/// (<see cref="BaseStars"/>) garantizado en cualquier forma, y Rebelión lo sube al total <c>Stars</c>.
/// El ★ NO sube con el up. Patrón DefiantCut con piso de forma.
/// </summary>
public sealed class LightningSpeed() : MordredCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int BaseStars = 10; // piso normalizado en cualquier forma

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("Stars", 10), new DynamicVar("BaseStars", BaseStars)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebellionFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InRebellion(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var stars = Forms.InRebellion(Owner.Creature) ? DynamicVars["Stars"].IntValue : DynamicVars["BaseStars"].IntValue;
        await CritStars.Gain(choiceContext, Owner.Creature, stars, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
