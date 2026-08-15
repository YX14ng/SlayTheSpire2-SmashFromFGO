using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Golpe del Espejo (镜之一击) — re-efecto RE-POOL V2 (carta-trampa cazada por el panel): la
/// condición pasa de «este combate» (trivial: siempre cumplida tras el turno 1) a «este TURNO»
/// — premia bailar HOY. 5 de daño ×2; si cambiaste de forma este turno: ×3 (mejora 7).
/// El flag vive en el bit 9 del estado de turno (lo levantan cetro/Ancient en OnFormChanged).
/// </summary>
public sealed class MirrorStrike() : MorganCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];

    private bool ShiftedThisTurn => FgoCombatState.GetTurn(Owner.Creature, 9) != 0;

    protected override bool ShouldGlowGoldInternal => ShiftedThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hits = ShiftedThisTurn ? 3 : 2;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(hits).FromCardFgoCompatibility(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
