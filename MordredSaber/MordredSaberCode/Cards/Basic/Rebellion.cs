using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Basic;

/// <summary>
/// Rebelión (叛逆) — FIRMA básica de la forma OFENSIVA (DESIGN-MORDRED §5.0). 6 de daño; LUEGO te
/// quitás el yelmo (entrás en Rebelión). El daño se resuelve ANTES del switch, así NO se
/// auto-buffea con el +2 de la forma (precedente Watcher/Artoria: cada forma cambia la DECISIÓN,
/// no el número de la firma que la activa). La escena de Apocrypha ante Sisigou, jugable desde
/// el combate 1.
/// </summary>
public sealed class Rebellion() : MordredCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RebellionFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Daño PRIMERO (sin el +2 de Rebelión), después el yelmo cae.
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await Forms.Enter<RebellionFormPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
