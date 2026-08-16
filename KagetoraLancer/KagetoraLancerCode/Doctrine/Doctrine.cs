using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Doctrine;

public enum Precept
{
    None = 0,
    Heaven = 1,
    Chest = 2,
    Feet = 4
}

public interface IPreceptCard
{
    Precept Precept { get; }
}

public readonly record struct DoctrineAdvance(
    CardPlay CardPlay,
    Precept Attempted,
    bool Advanced,
    int BeforeMask,
    int AfterMask,
    bool CycleCompleted,
    int AdvancesThisTurn);

public interface IDoctrineAdvanceListener
{
    Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result);
}

public interface IDoctrineCycleListener
{
    Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result);
}

public interface IDoctrineFailureOverride
{
    int DoctrineOverridePriority => 0;
    bool CanOverrideDoctrineFailure(CardPlay cardPlay, Precept attempted);
    Task AfterOverridingDoctrineFailure(PlayerChoiceContext context, CardPlay cardPlay, Precept attempted);
}

/// <summary>
/// Motor único de la Doctrina. Amount guarda mask+1 para que el estado vacío siga existiendo y
/// sobreviva guardado/carga. Kagetora exige Heaven→Chest→Feet; Kenshin acepta cualquier orden sin
/// repetir. Una carta sólo puede producir un intento y nunca borra progreso al fallar.
///
/// REDESIGN-KAGETORA-V2 §3.2: además de avanzar, el motor ahora paga el ciclo (E1), se declara
/// recurso (E7) y arbitra el acceso al crítico (E6). Es <see cref="IResourcePower"/> porque
/// Cleanse.RemoveBuffs/RemoveAllBuffs excluye explícitamente los recursos: sin eso una limpieza
/// enemiga borraba el motor entero mientras sus dos satélites (DoctrineTurnStatePower,
/// KagetoraUsagePower) sobrevivían — asimetría invertida, DESIGN §4.4 promete lo contrario.
/// </summary>
public sealed class DoctrinePower :
    KagetoraPower, IResourcePower, IDoctrineCycleListener, ICriticalAccessRule, ICriticalConsumedListener
{
    // §16.4 — PROHIBIDO subir esto a 4. `DoctrineTurnStatePower.Advances` es un campo de DOS BITS
    // (`State & 3`, y `DoctrineTurnState.Set` guarda `advances & 3`): con 4 el contador wrappea a 0,
    // WouldAdvance vuelve a devolver true y el tope desaparece ⇒ avances ilimitados ⇒ ciclos
    // ilimitados ⇒ refund de energía ilimitado (loop determinista, no un ajuste de número).
    // Si alguna vez hiciera falta: primero se ensancha el campo, se re-verifica WouldAdvanceAfter y
    // se escribe un cap explícito de 1 refund/turno ANTES de tocar esta constante.
    public const int MaxAdvancesPerTurn = 3;
    public const int HeavenNp = 10;
    public const int ChestBlock = 4;
    public const int FeetStars = 20;

    /// <summary>E1 — cerrar un ciclo devuelve 1⚡. Es el único retorno de energía del kit (§17).</summary>
    public const int CycleEnergyRefund = 1;

    public override PowerType Type => PowerType.Buff;

    // Canal 1 de UI (§4.1). Verificado contra el decompilado: `NPower.RefreshAmount`
    // (decompiled/MegaCrit.Sts2.Core.Nodes.Combat/NPower.cs:234) escribe el label
    // `(Model.StackType == PowerStackType.Counter) ? Model.DisplayAmount.ToString() : string.Empty`.
    // Con Single el número NUNCA se dibujaba. `PowerModel.SetAmount` dispara DisplayAmountChanged y
    // `NPower.OnDisplayAmountChanged` repinta, así que basta con derivar DisplayAmount de Amount.
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>
    /// Preceptos ya avanzados del ciclo en curso (0-2). NO es <c>Amount</c>: Amount es mask+1, un
    /// número de estado interno que al jugador no le dice nada. Nunca vale 3 porque al completarse
    /// (mask == 7) el ciclo se vacía dentro del mismo <see cref="AfterCardPlayed"/>.
    /// </summary>
    public override int DisplayAmount => PreceptCount(ProgressMask);

    public int ProgressMask => Math.Max(0, (int)Amount - 1);
    public int AdvancesThisTurn => Owner.GetPower<DoctrineTurnStatePower>()?.Advances ?? 0;
    public int AdvancedMaskThisTurn => Owner.GetPower<DoctrineTurnStatePower>()?.AdvancedMask ?? 0;
    public bool IsKenshin => Owner.HasPower<KenshinFormPower>();

    /// <summary>E6 — bit por turno de «ya gasté mi crítico», en el mask visible de KagetoraUsage.</summary>
    public bool CriticalUsedThisTurn => KagetoraUsages.WasUsed(Owner, KagetoraUsage.CriticalThisTurn);

    /// <summary>Popcount de los tres bits de precepto de un mask (0-3). Sin System.Numerics: son tres.</summary>
    public static int PreceptCount(int mask) =>
        ((mask & (int)Precept.Heaven) != 0 ? 1 : 0) +
        ((mask & (int)Precept.Chest) != 0 ? 1 : 0) +
        ((mask & (int)Precept.Feet) != 0 ? 1 : 0);

    /// <summary>¿Ese precepto ya avanzó este turno? («¿es mi primera Pecho del turno?» = !esto).</summary>
    public bool AdvancedThisTurn(Precept precept) =>
        precept != Precept.None && (AdvancedMaskThisTurn & (int)precept) != 0;

    /// <summary>
    /// Mask de preceptos avanzados este turno CONTANDO el avance que la carta en resolución va a
    /// producir. `AfterCardPlayed` corre después del texto de la carta, así que durante `OnPlay` el
    /// propio avance todavía no está en `AdvancedMaskThisTurn` (ése era el bug de
    /// `EightWeaponsOneWarrior`: techo real +10 contra los +30 declarados, §11.3).
    /// </summary>
    public int AdvancedMaskIncludingThisPlay(Precept precept) =>
        AdvancedMaskThisTurn | (WouldAdvance(precept) ? (int)precept : 0);

    public static async Task EnsureInstalled(Creature owner)
    {
        if (!owner.HasPower<DoctrinePower>())
        {
            await PowerCmd.Apply<DoctrinePower>(
                new BlockingPlayerChoiceContext(), owner, 1m, owner, null);
        }
    }

    /// <summary>
    /// E2 — la primera mitad de la prueba de «≤1 refund por turno POR CONSTRUCCIÓN»: el tope de
    /// avances se evalúa acá, en la línea 1, ANTES de cualquier override. La otra mitad está en el
    /// guard `AdvancesThisTurn &lt; MaxAdvancesPerTurn` de <see cref="AfterCardPlayed"/>, que también
    /// corre antes de consultar <see cref="IDoctrineFailureOverride"/>: ni EightFormationsPower ni
    /// ForcedDoctrineAdvancePower pueden saltarlo.
    /// </summary>
    public bool WouldAdvance(Precept precept)
    {
        if (precept == Precept.None || AdvancesThisTurn >= MaxAdvancesPerTurn) return false;
        var mask = ProgressMask;
        if (IsKenshin) return (mask & (int)precept) == 0;

        var expected = (mask & (int)Precept.Heaven) == 0 ? Precept.Heaven
            : (mask & (int)Precept.Chest) == 0 ? Precept.Chest
            : (mask & (int)Precept.Feet) == 0 ? Precept.Feet
            : Precept.None;
        return precept == expected;
    }

    /// <summary>Predice qué etiqueta podría avanzar después de resolver la carta actual.</summary>
    public bool WouldAdvanceAfter(Precept current, Precept next)
    {
        if (next == Precept.None || AdvancesThisTurn >= MaxAdvancesPerTurn) return false;
        var mask = ProgressMask;
        var advances = AdvancesThisTurn;
        if (WouldAdvance(current))
        {
            mask |= (int)current;
            advances++;
            if (mask == 7) mask = 0;
        }
        if (advances >= MaxAdvancesPerTurn) return false;
        if (IsKenshin) return (mask & (int)next) == 0;
        var expected = (mask & (int)Precept.Heaven) == 0 ? Precept.Heaven
            : (mask & (int)Precept.Chest) == 0 ? Precept.Chest
            : (mask & (int)Precept.Feet) == 0 ? Precept.Feet
            : Precept.None;
        return next == expected;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        await MainFile.EnsureNpInCombat(Owner);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card is not IPreceptCard tagged) return;

        var precept = tagged.Precept;
        var before = ProgressMask;
        var advanced = WouldAdvance(precept);
        IDoctrineFailureOverride? overrideRule = null;
        if (!advanced && precept != Precept.None && AdvancesThisTurn < MaxAdvancesPerTurn)
        {
            overrideRule = Listeners.Of<IDoctrineFailureOverride>(Owner)
                .OrderByDescending(rule => rule.DoctrineOverridePriority)
                .FirstOrDefault(rule => rule.CanOverrideDoctrineFailure(cardPlay, precept));
            advanced = overrideRule != null;
        }
        if (!advanced)
        {
            var miss = new DoctrineAdvance(cardPlay, precept, false, before, before, false, AdvancesThisTurn);
            await Listeners.ForEachListener<IDoctrineAdvanceListener>(
                Owner, listener => listener.AfterDoctrineAdvance(context, miss));
            return;
        }

        // La recompensa se concede antes de cambiar el estado y de emitir eventos.
        if (overrideRule != null)
            await overrideRule.AfterOverridingDoctrineFailure(context, cardPlay, precept);
        await GrantInnateReward(context, precept, cardPlay.Card);

        var advancesThisTurn = AdvancesThisTurn + 1;
        var advancedMaskThisTurn = AdvancedMaskThisTurn | (int)precept;
        await DoctrineTurnState.Set(
            context, Owner, advancesThisTurn, advancedMaskThisTurn, cardPlay.Card);
        var advanceBit = (int)precept;
        if (overrideRule != null && (before & advanceBit) != 0)
        {
            advanceBit = new[] { 1, 2, 4 }.FirstOrDefault(bit => (before & bit) == 0);
        }
        var after = before | advanceBit;
        var completed = after == (int)(Precept.Heaven | Precept.Chest | Precept.Feet);
        await SetProgress(context, after, cardPlay.Card);

        var result = new DoctrineAdvance(
            cardPlay, precept, true, before, after, completed, advancesThisTurn);
        await Listeners.ForEachListener<IDoctrineAdvanceListener>(
            Owner, listener => listener.AfterDoctrineAdvance(context, result));

        if (!completed) return;

        // Vaciar antes del evento permite que sus listeners consulten el siguiente estado correcto.
        await SetProgress(context, 0, cardPlay.Card);
        await Listeners.ForEachListener<IDoctrineCycleListener>(
            Owner, listener => listener.AfterDoctrineCycle(context, result));
    }

    /// <summary>
    /// E1 — el ciclo devuelve 1⚡. Vive en el motor y no en la Pagoda a propósito: la reliquia se
    /// pierde o se reemplaza, y el ⚡ tiene que estar en el evento que el diseño premia (§3.3).
    ///
    /// E2, la prueba de que el refund es ≤1 por turno SIN contador nuevo: un ciclo se cierra cuando
    /// el mask llega a 7, y el mask se vacía en el mismo AfterCardPlayed. Cada avance enciende
    /// exactamente un bit (el override remapea a un bit libre, nunca agrega dos), y hay como mucho
    /// `MaxAdvancesPerTurn` = 3 avances por turno. Después del primer cierre el mask queda en 0, así
    /// que un segundo cierre exigiría 3 avances más — imposible, porque al menos uno ya se gastó en
    /// el primero. Ergo: ≤1 ciclo por turno ⇒ ≤1⚡ de refund por turno, por construcción.
    /// (Se auto-escucha: `Listeners.ForEachListener` recorre `GetPowerInstances`, que incluye a este
    /// power. Es el camino que pide §16.2: el refund sale del motor, no de la reliquia.)
    /// </summary>
    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (Owner.Player is not { } player) return;
        await PlayerCmd.GainEnergy(CycleEnergyRefund, player);
    }

    /// <summary>
    /// E6 — un crítico por turno, y sólo donde vale. Predicado PURO: `Criticals.CanSpend` también se
    /// consulta desde `WillCrit`, que es predicción para el glow, el hover y el preview de daño
    /// (`CriticalResolverPower.ModifyDamageMultiplicativeFgo`). Leer el bit por turno es puro;
    /// MARCARLO va en <see cref="AfterCriticalConsumed"/>, nunca acá.
    ///
    /// Sin este cap, `CriticalResolverPower.BeforeCardPlayed` se re-arma en cada carta y con
    /// `CritStarsPower.Max = 100` y `CritCost = 50` el banco compraba DOS críticos por turno: el
    /// breakpoint E4 («un ciclo = 50 estrellas exactas = un crítico») sería mentira.
    ///
    /// Las cartas sin precepto (incoloras de FGOCore, Llaves Negras, cualquier carta ajena)
    /// conservan el crítico, y el Crítico Listo también: `Criticals.CanSpend` corre ANTES de mirar
    /// `CritReadyPower`, así que sin esta válvula `Enfoque del Cielo` sería una trampa (§12.3-6).
    /// </summary>
    public bool CanSpendCritical(CardModel card)
    {
        if (CriticalUsedThisTurn) return false;
        var precept = card is IPreceptCard tagged ? tagged.Precept : Precept.None;
        if (precept is Precept.None or Precept.Feet) return true;
        return Owner.GetPowerAmount<CritReadyPower>() > 0m;
    }

    /// <summary>E6 — el marcado del bit por turno, en el listener de consumo (§16.2).</summary>
    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (critical.Owner != Owner) return;
        await KagetoraUsages.Mark(context, Owner, KagetoraUsage.CriticalThisTurn, critical.Card);
    }

    private async Task SetProgress(PlayerChoiceContext context, int mask, CardModel source)
    {
        var desired = mask + 1m;
        var delta = desired - Amount;
        if (delta != 0m)
        {
            await PowerCmd.ModifyAmount(context, this, delta, Owner, source);
        }
    }

    private async Task GrantInnateReward(
        PlayerChoiceContext context, Precept precept, CardModel source)
    {
        Flash();
        switch (precept)
        {
            case Precept.Heaven:
                await NpCharge.Gain(context, Owner, HeavenNp, source);
                break;
            case Precept.Chest:
                await CreatureCmd.GainBlock(Owner, ChestBlock, ValueProp.Unpowered, null);
                break;
            case Precept.Feet:
                await CritStars.Gain(context, Owner, FeetStars, source);
                break;
        }
    }
}
