using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Cards;

namespace MordredSaber.MordredSaberCode.Cards.Basic;

/// <summary>
/// Comando Quick (Quick カード, la verde) — la carta de comando RÁPIDA del mazo inicial QAABB
/// (DESIGN-REVIEW-2). Modelada EXACTO sobre Shukuchi/SlashOfClarent (golpe ligero + generación de ★):
/// 5 de daño + 10 Estrellas de Crítico. El Quick en FGO es el comando que más Estrellas produce; aquí
/// es la munición del Crítico ×2 desde el turno 1. Marcada con <see cref="IQuickCard"/> para que la
/// reliquia «Moto Roja de Trifas» (Riding B) le sume +10★. Basic (deck-only, no drafteable).
/// </summary>
public sealed class QuickCommand() : MordredCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), IQuickCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
