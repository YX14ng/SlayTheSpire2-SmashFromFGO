using MegaCrit.Sts2.Core.Entities.Relics;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Fragmento de la Mesa Redonda — starter relic, rediseño v2: de retención pasiva a
/// MOTOR (estilo la reliquia de Jeanne). (1) Al final de tu turno conserva hasta 10
/// de Bloqueo y entra en forma Shielder. (2) Golpe enemigo totalmente bloqueado →
/// +10 Estrellas de Crítico (tanquear ES generar; mismo trigger que Intercepción/
/// SenpaiPromise). (3) Perder Vida → +10 de Carga NP (costo = recurso). Parche P1
/// del juez: cada conversión proca máximo 3 veces por turno (reset al inicio del
/// turno del jugador) — candado anti multi-hit. Si el dueño tiene LordCamelotRestored,
/// esa reliquia toma el relevo (+20 por proc, mismos caps) y esta no convierte.
/// El motor compartido vive en <see cref="BulwarkEngineRelic"/>.
/// </summary>
public sealed class RoundTableFragment : BulwarkEngineRelic
{
    public const int MaxRetained = 10;
    public const int StarsPerBlock = 10;
    public const int NpPerHp = 10;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override int MaxRetainedBlock => MaxRetained;
    protected override int StarsPerFullBlock => StarsPerBlock;
    protected override int NpPerHpLoss => NpPerHp;

    /// <summary>LordCamelotRestored supersedes both the retention cap and the conversions.</summary>
    protected override bool IsActive => Owner.GetRelic<LordCamelotRestored>() == null;
}
