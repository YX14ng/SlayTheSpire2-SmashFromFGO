using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// La Espada Más Resplandeciente (最耀眼之剑, §5.3) — Clarent que cose ★→NP: el premio del motor.
///   - tus CRÍTICOS hacen +<see cref="CritBonus"/> ADICIONAL (el +N entra al golpe que va a
///     consumir el *Crítico Listo, ANTES del ×2 — se dobla con él, igual que KnightOfRedLightningPower);
///   - cada vez que CONSUMÍS un Crítico, ganás +<see cref="NpOnConsume"/> NP EXTRA
///     (vía <see cref="ICritConsumedListener"/>, lo dispara RedLightningChannelPower — encima del
///     +10 NP de la starter).
/// El <see cref="CritBonus"/> y el <see cref="NpOnConsume"/> son campos settables que fija la carta
/// desde sus DynamicVars (para que el up se refleje sin chocar con el conteo de stacks). Counter:
/// copias suman el daño-crit; el +NP/consumo no apila (un solo broadcast por consumo). Personal.
/// </summary>
public sealed class TheMostRadiantSwordPower : MordredPower, ICritConsumedListener
{
    public int CritBonus = 8;     // up 12 (la carta lo setea desde su DynamicVar)
    public int NpOnConsume = 10;  // +NP extra al consumir un crítico

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // +Crítico extra solo cuando hay un Crítico Listo en cola (el golpe que se va a doblar).
        return Criticals.WillCrit(Owner, cardSource) && IsFirstCritHit(cardSource, cardPlay)
            ? CritBonus * (int)Amount
            : 0m;
    }

    public async Task OnCritConsumed(PlayerChoiceContext? choiceContext)
    {
        Flash();
        await NpCharge.Gain(choiceContext ?? new BlockingPlayerChoiceContext(), Owner, NpOnConsume, null);
    }

    // ---- REDESIGN-MORDRED-V2 D7 / Candado 4 -------------------------------------------------
    // El bonus de CRÍTICO se cobra UNA vez por carta, en el primer impacto que pega de verdad.
    // Antes salía de `ModifyDamageAdditiveFgo`, que corre una vez POR IMPACTO: con una multi-hit
    // mejorada eso multiplicaba el bonus por la cantidad de golpes y encima el x1,5 del crítico
    // (contrato de Criticos v2: el multiplicador aplica a TODOS los impactos de la carta) lo volvia
    // a escalar. La auditoria de pico rehecha da ~302 de daño en un turno de 3⚡, ~40% POR ENCIMA de
    // la banda 180-220 de DECISIONS.md. Con este gate: ~212.
    //
    // El proyecto ya fallo este mismo caso en Kagetora (`DivinityPower`, UncommonPowers.cs): la
    // ligadura va al `CardPlay`, el marcado va en `AfterDamageGiven` -que solo corre sobre dano
    // REAL, no sobre previews- y `ModifyDamageAdditiveFgo` queda PURO (DECISIONS: los hooks de
    // preview no mutan estado). Un Ataque que no llega a pegar no quema el bonus.
    private CardPlay? _critPlay;
    private bool _critHitUsed;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature == Owner)
        {
            _critPlay = cardPlay;
            _critHitUsed = false;
        }
        return Task.CompletedTask;
    }

    /// <summary>Lectura PURA: ¿este impacto es el primero de la carta que esta criticando?</summary>
    private bool IsFirstCritHit(CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_critPlay == null) return true;   // fuera de una jugada real (preview): se muestra el bonus
        if (_critHitUsed) return false;
        if (_critPlay.Card != cardSource) return false;
        return cardPlay == null || cardPlay == _critPlay;
    }

    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (!_critHitUsed && _critPlay != null && dealer == Owner &&
            _critPlay.Card == cardSource && props.IsPoweredAttack())
        {
            _critHitUsed = true;
        }
        return Task.CompletedTask;
    }

}
