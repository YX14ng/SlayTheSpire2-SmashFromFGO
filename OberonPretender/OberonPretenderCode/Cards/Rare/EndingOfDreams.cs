using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// El Fin de los Sueños (梦之终结EX / Ending of Dreams EX) — DESIGN-OBERON §6.4, KIT S3 CLÍMAX. 2⚡
/// Habilidad · Exhaust: este turno tus Ataques hacen +50% y tu próxima carta NP hace ×2; al final del
/// turno perdés TODOS tus Poderes positivos y NO robás cartas el próximo turno (caés en el Sueño Eterno).
/// Aplica <see cref="EndingOfDreamsPower"/> (que gobierna el ×, el strip de Buff y el no-robo). NO carga
/// NP — demérito irrenunciable. Glow si ≥70 NP. El up baja el coste a 1⚡.
/// </summary>
public sealed class EndingOfDreams() : OberonCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const int ChargeGate = 70;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<EndingOfDreamsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => NpCharge.Current(Owner.Creature) >= ChargeGate;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EndingOfDreamsPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
