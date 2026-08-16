using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace AstolfoRider.AstolfoRiderCode.Powers;

[Flags]
public enum AstolfoTurnUsage
{
    Riding = 1,
    IndependentAction = 2,
    ImpossibleExistence = 4,
    DifferentAdventure = 8,
    WorldReverse = 16,
    AdventureCritical = 32,
    AdventureEvasion = 64
}

public sealed class AstolfoTurnUsagePower : AstolfoPower, IResourcePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
    public int Mask => Math.Max(0, (int)Amount - 1);

    // BUGFIX 2026-08-16: faltaba el reset. Los SIETE flags de AstolfoTurnUsage son «una vez por
    // TURNO» según su propia localización ("each turn"), pero la máscara no se limpiaba nunca —
    // tras arreglar WasUsed habrían quedado como «una vez por combate». Patrón calcado de
    // KagetoraUsagePower: limpiar en BeforeSideTurnStart y solo cuando el dueño participa
    // (regla de DECISIONS sobre estado efímero por turno).
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext context, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Mask == 0) return;
        await PowerCmd.ModifyAmount(context, this, 1m - Amount, Owner, null, silent: true);
    }
}

public static class AstolfoTurnUsages
{
    // BUGFIX 2026-08-16 (mismo defecto encontrado en Kagetora): `int? & int` es una comparación
    // LEVANTADA — sin el power, `?.Mask` es null, `null & N` es null y `null != 0` es TRUE en C#.
    // El power solo se crea dentro de Mark y los 7 call-sites de Mark están detrás de un guard
    // WasUsed → estado absorbente: Cabalgata A+, Acción Independiente B, Existencia Imposible,
    // Aventura Distinta, Reverso del Mundo y los dos riders de La Aventura Continúa NUNCA se
    // ejecutaban. El `?? 0` restaura la semántica.
    public static bool WasUsed(Creature owner, AstolfoTurnUsage usage) =>
        ((owner.GetPower<AstolfoTurnUsagePower>()?.Mask ?? 0) & (int)usage) != 0;

    public static async Task Mark(
        PlayerChoiceContext context, Creature owner, AstolfoTurnUsage usage, CardModel? source)
    {
        var power = owner.GetPower<AstolfoTurnUsagePower>();
        if (power == null)
        {
            await PowerCmd.Apply<AstolfoTurnUsagePower>(
                context, owner, (int)usage + 1m, owner, source, silent: true);
            return;
        }

        var desired = (power.Mask | (int)usage) + 1m;
        if (desired != power.Amount)
            await PowerCmd.ModifyAmount(
                context, power, desired - power.Amount, owner, source, silent: true);
    }
}

public sealed class HippogriffManifestedPower : AstolfoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}
