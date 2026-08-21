using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Basic;

/// <summary>
/// Comando Buster (Buster カード, la roja) — la carta de comando de ATAQUE PESADO del mazo inicial
/// QAABB (DESIGN-REVIEW-2: el deck base de Mordred era genérico sin motor, acto 1 roto). Modelada
/// EXACTO sobre las cartas de comando de Okita (Strike + feed del recurso): 8 de daño + 5 de Carga NP.
/// El Buster en FGO es el golpe fuerte que también empuja el medidor por sobrecarga; aquí enseña desde
/// el turno 1 que ATACAR carga el NP. Basic (deck-only, no drafteable). Patrón Strike + NpCharge.Gain.
/// </summary>
public sealed class BusterCommand() : MordredCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new DynamicVar("NpCharge", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
