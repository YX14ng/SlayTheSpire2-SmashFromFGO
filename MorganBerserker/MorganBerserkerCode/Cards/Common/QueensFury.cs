using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Furia de la Reina (女王之怒) — re-efecto RE-POOL V2 (§3.3): el interruptor COMÚN hacia la
/// forma detonadora que pedía el reporte 1. Entrá en Reina Hada Y pegá en la misma carta: 9 daño
/// que Detona (mejora 13). Análogo Eruption de la Watcher — acá el auto-buff ES el punto.
/// Implementación: IUsesTargetCurse para que la pasiva de forma NO detone por su cuenta (su
/// _pendingSentence se cachea en BeforeCardPlayed, ANTES de que esta carta entre a la forma);
/// la carta entra, Detona ella misma (respetando Cernunnos) y suma el bono a su único golpe.
/// </summary>
public sealed class QueensFury() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IUsesTargetCurse
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(MorganKeywords.Detonar), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await FormSwitch.Enter<FairyQueenFormPower>(choiceContext, Owner.Creature, this);

        var damage = DynamicVars.Damage.BaseValue;
        var bonus = await Sentencia.Detonar(Owner.Creature, cardPlay.Target);
        damage += bonus;

        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        Sentencia.ShowFloat(cardPlay.Target, bonus);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
