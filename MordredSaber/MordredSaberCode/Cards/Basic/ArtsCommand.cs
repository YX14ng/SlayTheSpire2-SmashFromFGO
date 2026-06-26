using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Basic;

/// <summary>
/// Comando Arts (Arts カード, la azul) — la carta de comando de CARGA del mazo inicial QAABB
/// (DESIGN-REVIEW-2). Modelada EXACTO sobre las cartas de comando de Okita (golpe modesto + fuerte feed
/// del recurso de motor): 5 de daño + 10 de Carga NP. El Arts en FGO es el comando que más carga el
/// medidor; aquí es el feeder de NP del deck base. Basic (deck-only, no drafteable). Patrón
/// BalmungSwing/InsolentGuard (daño + NpCharge.Gain), con el NP de peso.
/// </summary>
public sealed class ArtsCommand() : MordredCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(5m);
}
