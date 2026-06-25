using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Cards.Special;

/// <summary>
/// Chispa de Clarent (克拉伦特火花) — token EFÍMERO manifestado a la mano cada vez que Mordred
/// CONSUME un *Crítico Listo* (cap 1/turno, lo gobierna <see cref="Powers.RedLightningSparkPower"/>).
/// 0⚡, At AoE, Exhaust, <c>CardRarity.Event</c> (la rareza de los tokens generados del ecosistema —
/// igual que el Desatado y las Armas de Gil). 4 de daño a TODOS.
///
/// FIX HOMOGENEIZACIÓN (P1 Mordred): el ×2 nativo de FGOCore es genérico (idéntico a Okita/Gil).
/// Esta chispa hace que el Crítico de Mordred MANIFIESTE relámpago rojo que EXPRESA su identidad de
/// forma — el daño no se hornea acá: el +2 de la forma activa (Rebelión/Clímax) cae solo sobre este
/// Ataque potenciado vía <c>MordredFormPower.ModifyDamageAdditive</c>, así que la chispa pega más
/// fuerte sin casco (la rabia desatada) que Enmascarada. No toca FGOCore: reusa el broadcast
/// <c>ICritConsumedListener</c> que ya existe (RedLightningChannelPower).
/// </summary>
public sealed class ChispaDeClarent() : MordredCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies)
{
    public const int Damage = 4;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(Damage, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // El +2 de la forma (Rebelión/Clímax) se aplica solo: es un Ataque potenciado y la forma
        // modifica todo Ataque potenciado del owner (espejo del Desatado, que tampoco lo hornea).
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }
}
