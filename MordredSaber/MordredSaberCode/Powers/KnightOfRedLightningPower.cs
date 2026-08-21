using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Caballero del Relámpago Rojo A+ (赤雷骑士A+) — el Rank-Up de Estallido de Maná como PODER
/// (DESIGN-MORDRED §5.2): el multiplicador real de la forma ofensiva vive ACÁ (§5), no en la pasiva.
///   - tus Ataques hacen +<see cref="Amount"/> (aditivo);
///   - tus CRÍTICOS hacen +<see cref="CritBonus"/> ADICIONAL (el +N entra al golpe que va a
///     consumir el *Crítico Listo, ANTES del ×2 — se dobla con él, patrón SwordGeniusPower de Okita).
/// El <see cref="Amount"/> guarda el +Ataque (2; up 3); CritBonus es campo settable que la carta fija
/// desde su DynamicVar (6; up 8) para no chocar con el conteo de stacks. Counter: copias suman.
/// Personal: no escala en multijugador.
/// </summary>
public sealed class KnightOfRedLightningPower : MordredPower
{
    public int CritBonus = 6; // up 8 (la carta lo setea con Math.Max). NOTA: a diferencia del +Ataque
    // (Amount, que suma por copia), el +Critico NO apila con copias — es un valor unico compartido.

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // +Ataque plano siempre; +Crítico extra solo cuando hay un Crítico Listo en cola.
        var critExtra = Criticals.WillCrit(Owner, cardSource) && IsFirstCritHit(cardSource, cardPlay) ? CritBonus : 0;
        return Amount + critExtra;
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
