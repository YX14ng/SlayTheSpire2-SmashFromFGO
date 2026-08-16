using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public sealed class DoctrineTurnStatePower : KagetoraPower, IResourcePower
{
    private int State => Math.Max(0, (int)Amount - 1);

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    // §16.4 — CAMPO DE DOS BITS. `DoctrineTurnState.Set` guarda `advances & 3`, así que con
    // `DoctrinePower.MaxAdvancesPerTurn = 4` el cuarto avance guardaría 0, el contador wrappearía,
    // `WouldAdvance` volvería a devolver true y el tope desaparecería: avances ilimitados ⇒ ciclos
    // ilimitados ⇒ refund de energía ilimitado (E1). Es un loop determinista, no un ajuste de
    // número. Si alguna vez hiciera falta subirlo: primero se ENSANCHA este campo (y el corrimiento
    // de AdvancedMask), después se re-verifica WouldAdvanceAfter, y recién entonces se escribe un
    // cap explícito de 1 refund/turno. Mismo comentario, a propósito, en Doctrine.cs.
    public int Advances => State & 3;
    public int AdvancedMask => State >> 2;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Amount != 1m)
            await PowerCmd.ModifyAmount(
                context, this, 1m - Amount, Owner, null, silent: true);
    }
}

public static class DoctrineTurnState
{
    public static async Task Set(
        PlayerChoiceContext context, Creature owner, int advances, int advancedMask, CardModel? source)
    {
        var desired = 1m + (advances & 3) + (advancedMask << 2);
        var power = owner.GetPower<DoctrineTurnStatePower>();
        if (power == null)
        {
            await PowerCmd.Apply<DoctrineTurnStatePower>(
                context, owner, desired, owner, source, silent: true);
        }
        else if (power.Amount != desired)
        {
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
        }
    }
}

[Flags]
public enum KagetoraUsage
{
    Riding = 1,
    GeneralsDoctrine = 2,
    WhiteFlame = 4,
    EightFormations = 8,
    FieldJudge = 16,
    VictoryInTheFeet = 32,
    EightPetalBanner = 64,
    HoushoutsukigeReins = 128,
    SixPlateArmour = 256,
    ShiranuiTachi = 512,
    SakeCup = 1024,
    WhiteFlameBrazier = 2048,

    // REDESIGN-KAGETORA-V2 §16.3 — bits libres a partir de 4096.
    /// <summary>E6: ya se gastó el crítico de este turno (lo marca DoctrinePower).</summary>
    CriticalThisTurn = 4096,

    /// <summary>
    /// P-5 / §14.1-2: la Divinidad ya bonificó un Ataque este turno. <c>DivinityPower</c> se re-arma
    /// por <c>CardPlay</c>, así que sin este bit el +3 (+5 como Kenshin) entraba en el primer impacto
    /// de CADA Ataque y la auditoría de pico no cerraba. La ligadura de <c>CardPlay</c> que decide
    /// QUÉ impacto es el primero sigue siendo privada y local al hook de cálculo (excepción
    /// documentada en §11.3): lo único que se hace visible es el flag POR TURNO, que es lo que
    /// DECISIONS:79-82 exige.
    /// </summary>
    Divinity = 8192,

    /// <summary>
    /// Reservado por §16.3 para E8 («armada» vs. «gastada»). NO se usa: el tope de 4 impactos de la
    /// Bendición es estado de una sola jugada y vive en <c>BishamontenBlessingActivePower</c>, que
    /// se remueve al terminar la carta. Se deja declarado para que nadie reasigne el valor.
    /// </summary>
    BlessingArmedThisTurn = 16384,

    /// <summary>
    /// Deuda de §11.3: <c>TreasureWindowPower._prevented</c> era un flag «una vez por turno» en un
    /// campo privado (viola DECISIONS:79-82). Acá es visible y se limpia con el resto del mask. La
    /// semántica del texto no cambia: «el PRÓXIMO debuff DEL TURNO se evita», una sola vez, aunque
    /// varios avances de Pecho vuelvan a abrir la Ventana en el mismo turno.
    /// </summary>
    TreasureWindow = 32768
}

public sealed class KagetoraUsagePower : KagetoraPower, IResourcePower
{
    private const int PerTurnMask =
        (int)(KagetoraUsage.Riding | KagetoraUsage.GeneralsDoctrine |
              KagetoraUsage.WhiteFlame | KagetoraUsage.EightFormations |
              KagetoraUsage.FieldJudge | KagetoraUsage.VictoryInTheFeet |
              KagetoraUsage.EightPetalBanner | KagetoraUsage.HoushoutsukigeReins |
              KagetoraUsage.SixPlateArmour | KagetoraUsage.ShiranuiTachi |
              KagetoraUsage.CriticalThisTurn | KagetoraUsage.Divinity |
              KagetoraUsage.TreasureWindow);
    // Fuera del PerTurnMask a propósito: SakeCup y WhiteFlameBrazier son «una vez por COMBATE».
    // BlessingArmedThisTurn está declarado pero sin usar (ver el enum).

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Mask => Math.Max(0, (int)Amount - 1);

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        var desired = (Mask & ~PerTurnMask) + 1m;
        if (desired != Amount)
            await PowerCmd.ModifyAmount(
                context, this, desired - Amount, Owner, null, silent: true);
    }
}

public static class KagetoraUsages
{
    // BUGFIX 2026-08-16 (diagnóstico del reporte de Steam): `int? & int` es una comparación
    // LEVANTADA — sin el power, `?.Mask` es null, `null & N` es null y `null != 0` es TRUE en C#.
    // Como el power SOLO se crea dentro de Mark y los 12 call-sites de Mark están detrás de un
    // guard WasUsed, el estado era absorbente: el power nunca nacía y los 12 efectos por turno
    // (Llama Blanca, Ocho Formaciones, Juez del Campo, Victoria en los Pies, Cabalgata, Doctrina
    // del General y 6 reliquias) NUNCA se ejecutaban. El `?? 0` restaura la semántica.
    public static bool WasUsed(Creature owner, KagetoraUsage usage) =>
        ((owner.GetPower<KagetoraUsagePower>()?.Mask ?? 0) & (int)usage) != 0;

    public static async Task Mark(
        PlayerChoiceContext context, Creature owner, KagetoraUsage usage, CardModel? source)
    {
        var power = owner.GetPower<KagetoraUsagePower>();
        if (power == null)
        {
            await PowerCmd.Apply<KagetoraUsagePower>(
                context, owner, (int)usage + 1m, owner, source, silent: true);
            return;
        }

        var desired = (power.Mask | (int)usage) + 1m;
        if (desired != power.Amount)
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
    }
}
