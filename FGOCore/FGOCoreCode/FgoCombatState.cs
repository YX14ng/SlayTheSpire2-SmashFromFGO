using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Cleanse;

namespace FGOCore.FGOCoreCode;

/// <summary>
/// Hidden bit field for character effects limited per player turn. Amount is state+1 so an empty
/// marker survives serialization; the marker resets itself only when its owner participates.
/// Character mods may reuse bit offsets because each creature belongs to a single character.
/// </summary>
public sealed class FgoTurnStatePower : FGOCorePower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner) && Amount != 1m)
            await PowerCmd.ModifyAmount(
                context, this, 1m - Amount, Owner, null, silent: true);
    }
}

/// <summary>Hidden bit field for once-per-combat character effects.</summary>
public sealed class FgoCombatStatePower : FGOCorePower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

/// <summary>
/// Serializable combat-state slots for character mechanics. Fields are non-overlapping bit ranges
/// selected by each character mod, all of which must fit within bits 0..28 (see TotalBits).
/// Values are committed through PowerCmd so they participate in combat snapshots and multiplayer
/// checksums.
/// </summary>
public static class FgoCombatState
{
    public static int GetTurn(Creature owner, int offset, int width = 1) =>
        Get<FgoTurnStatePower>(owner, offset, width);

    public static int GetCombat(Creature owner, int offset, int width = 1) =>
        Get<FgoCombatStatePower>(owner, offset, width);

    public static Task SetTurn(
        PlayerChoiceContext context, Creature owner, int offset, int value,
        CardModel? source = null, int width = 1) =>
        Set<FgoTurnStatePower>(context, owner, offset, width, value, source);

    public static Task SetCombat(
        PlayerChoiceContext context, Creature owner, int offset, int value,
        CardModel? source = null, int width = 1) =>
        Set<FgoCombatStatePower>(context, owner, offset, width, value, source);

    public static async Task<int> IncrementTurn(
        PlayerChoiceContext context, Creature owner, int offset, int maximum,
        CardModel? source = null, int width = 1)
    {
        var next = Math.Min(maximum, GetTurn(owner, offset, width) + 1);
        await SetTurn(context, owner, offset, next, source, width);
        return next;
    }

    public static async Task<int> IncrementCombat(
        PlayerChoiceContext context, Creature owner, int offset, int maximum,
        CardModel? source = null, int width = 1)
    {
        var next = Math.Min(maximum, GetCombat(owner, offset, width) + 1);
        await SetCombat(context, owner, offset, next, source, width);
        return next;
    }

    private static int Get<TPower>(Creature owner, int offset, int width)
        where TPower : PowerModel
    {
        ValidateField(offset, width);
        var encoded = owner.GetPower<TPower>()?.Amount ?? 1m;
        var state = Math.Max(0L, (long)encoded - 1L);
        var mask = (1L << width) - 1L;
        return (int)((state >> offset) & mask);
    }

    private static async Task Set<TPower>(
        PlayerChoiceContext context, Creature owner, int offset, int width, int value,
        CardModel? source)
        where TPower : PowerModel
    {
        ValidateField(offset, width);
        var valueMask = (1L << width) - 1L;
        if (value < 0 || value > valueMask)
            throw new ArgumentOutOfRangeException(nameof(value));

        var power = owner.GetPower<TPower>();
        var state = Math.Max(0L, (long)(power?.Amount ?? 1m) - 1L);
        var fieldMask = valueMask << offset;
        var nextState = (state & ~fieldMask) | ((long)value << offset);
        var desired = nextState + 1m;
        if (power == null)
        {
            if (value == 0) return;
            await PowerCmd.Apply<TPower>(context, owner, desired, owner, source, silent: true);
        }
        else if (power.Amount != desired)
        {
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
        }
    }

    // Techo REAL de la clase. El estado vive en PowerModel.Amount, y todo camino de commit termina
    // en PowerModel.SetAmount, que CLAMPEA en silencio: `Math.Clamp(amount, -999999999, 999999999)`
    // (PowerModel.cs:545, idéntico en MAIN y BETA). Pasarse no tira: deja el Amount pegado al tope
    // y todos los campos leen basura por el resto del combate. Como lo commiteado es state+1, el
    // estado entero tiene que entrar bajo ese clamp: 29 bits (máx 536.870.911, commit 536.870.912)
    // entran; 30 no (commit 1.073.741.824 > 999.999.999). El bit más alto que usan los 12 mods hoy
    // es el 13, así que sobra margen.
    private const int TotalBits = 29;

    private static void ValidateField(int offset, int width)
    {
        // `offset > TotalBits - width` y no `offset + width > TotalBits`: la suma desborda con
        // widths absurdos y se saltearía la guarda entera.
        if (offset < 0 || width < 1 || width > TotalBits || offset > TotalBits - width)
            throw new ArgumentOutOfRangeException(nameof(offset));
    }
}
