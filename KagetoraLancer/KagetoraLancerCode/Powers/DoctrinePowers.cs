using FGOCore.FGOCoreCode.Block;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public interface IAscensionListener
{
    Task AfterAscendingToKenshin(PlayerChoiceContext context, CardModel source);
}

/// <summary>Base compartida para las dos formas visuales de Kagetora.</summary>
public abstract class KagetoraFormPower : FormPower
{
    public override bool ShouldScaleInMultiplayer => false;
}

/// <summary>
/// Forma inicial de Nagao Kagetora — orden fijo Cielo → Pecho → Pies (§5).
///
/// Pasiva: <b>válvula anti-atasco</b>. Al inicio de tu turno, si en la mano no hay NINGUNA carta que
/// pueda avanzar la Doctrina ahora mismo, robás 1. Ataca exactamente el modo de fallo del orden fijo
/// (el diagnóstico midió que la tasa de ciclo cae de 75 % a 52,8 % al engordar el mazo), es
/// condicional, está autolimitada a 1/turno por el propio hook y es <i>legible</i>: el juego te
/// demuestra que sabe qué precepto espera.
/// </summary>
public sealed class NagaoKagetoraFormPower : KagetoraFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/kagetora_frames.tres";

    // AfterPlayerTurnStart y no AfterSideTurnStart: el hook de lado corre ANTES del robo de mano
    // (CombatManager.cs:672-675 dibuja la mano y recién después llama Hook.AfterPlayerTurnStart), así
    // que mirar la mano desde ahí leería la mano del turno ANTERIOR. Además éste entrega el
    // PlayerChoiceContext del turno, que CardPileCmd.Draw necesita.
    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext context, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner.Player is not { } owner || player != owner || Owner.IsDead) return;
        if (Owner.GetPower<DoctrinePower>() is not { } doctrine) return;

        // «El precepto esperado» se pregunta a través de WouldAdvance y no reimplementando el orden:
        // así la válvula respeta el tope de avances del turno y seguiría siendo correcta si el motor
        // cambiara. Una carta sin precepto devuelve false y no cuenta como salida.
        foreach (var card in PileType.Hand.GetPile(owner).Cards)
        {
            if (card is IPreceptCard tagged && doctrine.WouldAdvance(tagged.Precept)) return;
        }

        // Mismo guard anti-soft-lock que la Pagoda: robar con el mazo vacío gatilla el reshuffle que
        // puede corromper la carta en curso.
        if (PileType.Draw.GetPile(owner).Cards.Count <= 0) return;
        Flash();
        await CardPileCmd.Draw(context, 1, owner);
    }
}

/// <summary>
/// Transformación irreversible a Uesugi Kenshin durante el combate (§5).
///
/// Tres deltas permanentes, todos energéticamente neutros — <b>Kenshin NO recibe +1⚡</b> (§12.3-3:
/// era la fuente aritmética del pico de 257-267 y el mismo regalo incondicional que el diagnóstico
/// midió como inefectivo):
/// <list type="number">
/// <item>orden libre, sin repetir dentro del ciclo (lo resuelve <c>DoctrinePower.IsKenshin</c>);</item>
/// <item>completar un ciclo da <see cref="CycleNpBonus"/> de Carga NP ADEMÁS del +10 innato de Cielo
/// ⇒ 20 NP por ciclo ⇒ 5 ciclos hasta los 100 en vez de 10;</item>
/// <item>conservás hasta <see cref="RetainedBlock"/> de Bloqueo al final del turno.</item>
/// </list>
/// </summary>
public sealed class KenshinFormPower : KagetoraFormPower, IBlockRetentionSource, IDoctrineCycleListener
{
    /// <summary>§5 / J-07: la retención vive en la FORMA, no en una carta. Knob §15.4-6 (3 … 8).</summary>
    public const int RetainedBlock = 5;

    /// <summary>Carga NP EXTRA por ciclo completo (el +10 de Cielo es innato del motor).</summary>
    public const int CycleNpBonus = 10;

    public override string FramesPath => $"{MainFile.ResPath}/character/kenshin_frames.tres";
    public override bool IsPermanent => true;

    // CONTRATO de IBlockRetentionSource (FGOCore/Block/IBlockRetentionSource.cs): la interfaz sólo
    // alimenta el CÁLCULO del cap; alguien tiene que responder `ShouldClearBlock` = false. Para una
    // FORMA ya está cubierto: la base `FormPower` (FGOCore/Forms/FormPower.cs:60-75) responde por
    // cualquier subclase que implemente la interfaz con cap > 0 y encadena
    // `AfterPreventingBlockClear` → `BlockRetention.Enforce`. Por eso la retención va acá y NO en una
    // reliquia ni en un power suelto, que tendrían que overridear los dos hooks a mano.
    public decimal RetentionCap(Creature creature) => creature == Owner ? RetainedBlock : 0m;

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        Flash();
        await NpCharge.Gain(context, Owner, CycleNpBonus, result.CardPlay.Card);
    }
}

/// <summary>Encarnación: cada ciclo prepara, como máximo, una Bendición.</summary>
public sealed class IncarnationPower : KagetoraPower, IDoctrineCycleListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        Flash();
        await BishamontenBlessingPower.Grant(context, Owner, result.CardPlay.Card);
    }
}

/// <summary>
/// Reserva de Bendición. Antes de un Ataque se transforma en un efecto activo atado al CardPlay;
/// así una Bendición creada por el ciclo de ese mismo Ataque queda intacta para el siguiente.
///
/// <b>E8 (§3.2, parche J2-K1):</b> la Bendición sólo se arma en <b>Ataques de Pies o sin precepto</b>.
/// Antes se armaba en el primer Ataque de CUALQUIER tipo, y con orden fijo Cielo→Pecho→Pies ese
/// primer Ataque es casi siempre <c>Arts</c> (Cielo, 1 impacto de 6): el «+2 por impacto» valía «+2
/// total» y encima quemaba la reserva antes de que llegara el multi-impacto de Pies. Rota por abajo.
/// Con las dos comunes nuevas de Pecho (<c>SpearWall</c>, <c>EchigoRampart</c>) la trampa dejaba de
/// ser latente, por eso el guard mira el PRECEPTO además del tipo.
/// </summary>
public sealed class BishamontenBlessingPower : KagetoraPower
{
    public const int PerHitDamage = 2;

    /// <summary>
    /// E8 — tope de impactos bonificados. Sin él, un NP de 8 impactos cobraba +16 y el pico de §14.2
    /// se iba de 217,5 a ~225: fuera del techo de 180-220. Contingencia §14.4-1: bajar a 3.
    /// </summary>
    public const int MaxBoostedHits = 4;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public static async Task Grant(PlayerChoiceContext context, Creature owner, CardModel? source)
    {
        if (owner.HasPower<BishamontenBlessingPower>()) return;
        await PowerCmd.Apply<BishamontenBlessingPower>(context, owner, 1m, owner, source);
    }

    /// <summary>E8 — ¿este Ataque puede armar la Bendición? Puro: lo consulta el guard y el glow.</summary>
    public static bool CanArm(CardModel card)
    {
        if (card.Type != CardType.Attack) return false;
        var precept = card is IPreceptCard tagged ? tagged.Precept : Precept.None;
        // Los dos NP son IPreceptCard con Precept.None (KagetoraNpCard), así que entran por la rama
        // «sin precepto» — que es lo que §3.2 pide explícitamente («en los NP también +2, tope 4»).
        return precept is Precept.None or Precept.Feet;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || !CanArm(cardPlay.Card)) return;

        var context = new BlockingPlayerChoiceContext();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<BishamontenBlessingActivePower>(
            context, Owner, 1m, Owner, cardPlay.Card, silent: true);
        Owner.GetPower<BishamontenBlessingActivePower>()?.Arm(cardPlay);
    }
}

/// <summary>
/// Efecto oculto que aplica +2 a los primeros <see cref="BishamontenBlessingPower.MaxBoostedHits"/>
/// impactos de una sola jugada (E8).
/// </summary>
public sealed class BishamontenBlessingActivePower : KagetoraPower
{
    private CardPlay? _activePlay;
    private int _hitsBoosted;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    internal void Arm(CardPlay cardPlay)
    {
        _activePlay = cardPlay;
        _hitsBoosted = 0;
    }

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        // El conteo NO se hace acá: este hook corre varias veces por impacto (previews de daño) y
        // DECISIONS:83-84 exige que los hooks de cálculo sean puros. Leer el contador es puro;
        // incrementarlo va en AfterDamageGiven, que sólo corre sobre daño real.
        if (_hitsBoosted >= BishamontenBlessingPower.MaxBoostedHits) return 0m;
        return _activePlay != null &&
               (_activePlay == cardPlay || cardPlay == null) &&
               _activePlay.Card == cardSource
            ? BishamontenBlessingPower.PerHitDamage
            : 0m;
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result,
        ValueProp props, Creature target, CardModel? cardSource)
    {
        if (dealer == Owner && props.IsPoweredAttack() && _activePlay != null &&
            _activePlay.Card == cardSource &&
            _hitsBoosted < BishamontenBlessingPower.MaxBoostedHits)
        {
            _hitsBoosted++;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay != cardPlay) return;
        _activePlay = null;
        _hitsBoosted = 0;
        await PowerCmd.Remove(this);
    }
}

/// <summary>Marcador de compatibilidad para la manifestación del NP.</summary>
public sealed class NpManifestedPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

/// <summary>Preparación local de Overcharge: cada carga suma un nivel de OC al próximo NP.</summary>
public sealed class OverchargePreparationPower : KagetoraPower, INpOverchargePreparation
{
    public const int MaxStacks = 1;
    public const int ExtraTier = 200;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public static async Task<int> Consume(PlayerChoiceContext context, Creature owner)
    {
        if (owner.GetPower<OverchargePreparationPower>() is not { } power) return 0;
        var extra = (int)power.Amount * ExtraTier;
        await PowerCmd.Remove(power);
        return extra;
    }

    int INpOverchargePreparation.ExtraTier => ExtraTier;

    public async Task ConsumeOverchargePreparation(PlayerChoiceContext context) =>
        await PowerCmd.Remove(this);
}
