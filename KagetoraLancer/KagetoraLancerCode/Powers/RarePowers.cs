using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

/// <summary>
/// §7.3 — `WhiteFlameA` pasa a 1⚡ y su mejora deja de rebajar el coste (a 0⚡ un Poder que paga
/// todos los turnos rompe E5): la mejora ahora sube LAS ESTRELLAS de 10 a 20. Por eso
/// <c>Amount</c> pasa a ser las estrellas por turno — antes era la Carga NP, que ya no se mejora —
/// y el +10 de Carga NP del primer avance de Cielo queda fijo en <see cref="HeavenAdvanceNp"/>.
/// </summary>
public sealed class WhiteFlamePower : KagetoraPower, IDoctrineAdvanceListener
{
    public const int HeavenAdvanceNp = 10;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        await CritStars.Gain(Owner, (int)Amount, null);
    }

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.WhiteFlame) ||
            !result.Advanced || result.Attempted != Precept.Heaven) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.WhiteFlame, result.CardPlay.Card);
        Flash();
        await NpCharge.Gain(context, Owner, HeavenAdvanceNp, result.CardPlay.Card);
    }
}

public sealed class EightFormationsPower : KagetoraPower, IDoctrineFailureOverride
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted) =>
        !KagetoraUsages.WasUsed(Owner, KagetoraUsage.EightFormations) &&
        cardPlay.Card.Owner?.Creature == Owner && attempted != Precept.None;

    public async Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted)
    {
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.EightFormations, cardPlay.Card);
        Flash();
    }
}

/// <summary>
/// Override de un solo uso, armado por <c>FortuneArmourAndMeritA</c>: la carta a la que quedó atado
/// avanza el precepto elegido ignorando el orden.
///
/// <para><b>Deuda de §11.3 — el precepto elegido se guarda ACÁ, no en la carta.</b>
/// <c>FortuneArmourAndMeritA</c> mutaba <c>KagetoraCard.Precept</c> en runtime, y esa mutación
/// PERSISTÍA: una elección cancelada o un combate anterior dejaban la carta convertida en «de Pecho»
/// para el resto de la run (glow mentiroso + avance espurio). <see cref="ArmedPrecept"/> y
/// <see cref="ArmedPreceptFor"/> son el almacén correcto — por jugada, se va con el power.</para>
///
/// <para><b>Lo que falta para cerrarla del todo, y por qué no se puede desde este archivo:</b> el
/// motor lee <c>tagged.Precept</c> y su rama de <c>IDoctrineFailureOverride</c> exige
/// <c>precept != Precept.None</c> (guard de <c>DoctrinePower.AfterCardPlayed</c>), así que una carta
/// con <c>Precept.None</c> sale del motor ANTES de consultar ningún override. El cierre son DOS
/// líneas, ninguna en <c>Doctrine.cs</c> (que §16.9 protege):
/// <list type="number">
/// <item><c>KagetoraCard.Precept</c> pasa de <c>public Precept Precept { get; protected set; }</c> a
/// <c>public virtual Precept Precept =&gt; precept;</c> (hoy no es virtual, por eso ninguna carta puede
/// derivarlo);</item>
/// <item><c>FortuneArmourAndMeritA</c> deja de asignarlo y lo deriva de acá:
/// <c>public override Precept Precept =&gt; IsMutable ? ArmedPreceptFor(Owner?.Creature, this) : Precept.None;</c>,
/// armando con <c>Arm(this, choice.ChosenPrecept)</c> en vez de <c>Arm(this)</c>.</item>
/// </list>
/// La guarda <c>IsMutable</c> es la misma de <c>KagetoraCard.DoctrineEngine</c>: el getter se
/// consulta sobre modelos canónicos, donde <c>Owner</c> tira <c>CanonicalModelException</c>.</para>
/// </summary>
public sealed class ForcedDoctrineAdvancePower : KagetoraPower,
    IDoctrineFailureOverride, IDoctrineAdvanceListener
{
    private CardModel? _card;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int DoctrineOverridePriority => 100;

    /// <summary>Precepto elegido para la carta armada (None si el power no está armado).</summary>
    public Precept ArmedPrecept { get; private set; } = Precept.None;

    /// <summary>
    /// El precepto que <paramref name="card"/> tiene que declarar mientras esté armada. Puro y
    /// null-safe: pensado para llamarse desde el getter <c>Precept</c> de la carta, que el compendio
    /// y la pantalla de recompensa consultan fuera de combate.
    /// </summary>
    public static Precept ArmedPreceptFor(Creature? owner, CardModel card) =>
        owner?.GetPower<ForcedDoctrineAdvancePower>() is { } power && power._card == card
            ? power.ArmedPrecept
            : Precept.None;

    public void Arm(CardModel card) => Arm(card, Precept.None);

    public void Arm(CardModel card, Precept precept)
    {
        _card = card;
        ArmedPrecept = precept;
    }

    public bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted) =>
        _card == cardPlay.Card && attempted != Precept.None;

    public async Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted) =>
        await PowerCmd.Remove(this);

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (_card == result.CardPlay.Card && Owner.HasPower<ForcedDoctrineAdvancePower>())
            await PowerCmd.Remove(this);
    }
}

public sealed class TreasureInHeartPower : KagetoraPower, IDoctrineAdvanceListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (!result.Advanced || result.Attempted != Precept.Chest) return;
        // La Ventana es «el próximo debuff DEL TURNO»: si ya se gastó, un segundo avance de Pecho no
        // abre una segunda. Antes se aplicaba igual y quedaba un icono visible que no hacía nada.
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.TreasureWindow)) return;
        Flash();
        await PowerCmd.Apply<TreasureWindowPower>(context, Owner, Amount, Owner, result.CardPlay.Card);
    }
}

/// <summary>
/// Ventana del Tesoro: evita un debuff y paga Carga NP.
/// <para><b>Deuda de §11.3 cerrada:</b> el flag «una vez por turno» era el campo privado
/// <c>_prevented</c>, que viola DECISIONS:79-82 (estado por turno en powers visibles) y no sobrevivía
/// guardado/carga. Ahora vive en <c>KagetoraUsage.TreasureWindow</c>, dentro del <c>PerTurnMask</c>
/// de <c>KagetoraUsagePower</c>, que se limpia en <c>BeforeSideTurnStart</c> con
/// <c>participants.Contains(Owner)</c>. Leer el bit desde <see cref="TryModifyPowerAmountReceived"/>
/// es PURO (es un hook de cálculo, DECISIONS:83-84); marcarlo va en
/// <see cref="AfterModifyingPowerAmountReceived"/>, el hook de efecto.</para>
/// </summary>
public sealed class TreasureWindowPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target,
        decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        // BUGFIX 2026-08-16: faltaba el guard `amount <= 0` del precedente vanilla
        // (RuinedHelmet.cs:42-45). Hook.ModifyPowerAmountReceived (Hook.cs:1916-1928) encadena los
        // listeners propagando el valor YA modificado y acumula TODOS los que devuelven true; si
        // Artifact corrió antes y dejó amount=0, GetTypeForAmount(0) sigue devolviendo Debuff
        // (PowerModel.cs:460-470: los dos early-return exigen < 0) → esta Ventana también se
        // consumía. Un solo debuff bloqueado gastaba las DOS defensas.
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.TreasureWindow) ||
            amount <= 0m || target != Owner || applier == null || applier.Side == target.Side ||
            canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff || !canonicalPower.IsVisible) return false;
        modifiedAmount = 0m;
        return true;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.TreasureWindow)) return;
        var context = new BlockingPlayerChoiceContext();
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.TreasureWindow, null);
        Flash();
        await NpCharge.Gain(context, Owner, (int)Amount, null);
        await PowerCmd.Remove(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext context, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class FieldJudgePower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(context, power, amount, applier, cardSource);
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.FieldJudge) ||
            amount <= 0 || power.Owner.Side == Owner.Side ||
            power.GetTypeForAmount(amount) != PowerType.Buff || !power.IsVisible) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.FieldJudge, cardSource);
        Flash();
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        await NpCharge.Gain(context, Owner, Amount >= 12m ? 20 : 10, cardSource);
    }
}

public sealed class VictoryIsInTheFeetPower : KagetoraPower, ICriticalConsumedListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (KagetoraUsages.WasUsed(Owner, KagetoraUsage.VictoryInTheFeet) ||
            critical.Owner != Owner) return;
        await KagetoraUsages.Mark(
            context, Owner, KagetoraUsage.VictoryInTheFeet, critical.Card);
        Flash();
        await CritStars.Gain(context, Owner, 20, critical.Card);
        if (Amount >= 2m) await NpCharge.Gain(context, Owner, 10, critical.Card);
    }
}

/// <summary>
/// Manifestación de Bishamonten: los próximos <see cref="MaxCycles"/> ciclos completos dan +1 Fuerza.
/// <c>Amount</c> son los ciclos que quedan y se decrementa por ciclo, así que el tope de UNA copia
/// sale gratis.
///
/// <para><b>El clamp de §12.3-5 sí hace falta:</b> <c>PowerCmd.Apply</c> SUMA stacks, así que dos
/// copias de la carta dejaban <c>Amount = 6</c> y +6 de Fuerza. La Fuerza multiplica ~20 impactos por
/// turno: es el motor del pico roto que los tres jueces cazaron, y la auditoría de §14.2 está hecha
/// con +3. El clamp es reactivo y no recursivo: al bajar hasta <see cref="MaxCycles"/> el hook vuelve
/// a correr y la condición ya es falsa. Contingencia §14.4-3: bajar el tope a 2.</para>
/// </summary>
public sealed class BishamontenManifestationPower : KagetoraPower, IDoctrineCycleListener
{
    public const int MaxCycles = 3;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext context, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(context, power, amount, applier, cardSource);
        if (power != this || Amount <= MaxCycles) return;
        await PowerCmd.ModifyAmount(context, this, MaxCycles - Amount, Owner, cardSource, silent: true);
    }

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (Amount <= 0) return;
        Flash();
        await PowerCmd.Apply<StrengthPower>(context, Owner, 1m, Owner, result.CardPlay.Card);
        await PowerCmd.Decrement(this);
    }
}
