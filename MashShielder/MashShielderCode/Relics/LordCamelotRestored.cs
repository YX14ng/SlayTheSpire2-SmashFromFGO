using MegaCrit.Sts2.Core.Entities.Relics;

namespace MashShielder.MashShielderCode.Relics;

/// <summary>
/// Lord Camelot (restaurado) — Ancient relic, rediseño v2: retiene hasta 25 de
/// Bloqueo entre turnos y SUBE LOS MONTOS del motor de la starter (arco Hellup de
/// Jeanne): golpe enemigo totalmente bloqueado → +20 Estrellas de Crítico; perder
/// Vida → +20 de Carga NP. Parche P1 del juez: mantiene el cap de 3 procs por turno
/// y por conversión (techo 60/turno) — sube el monto, NO duplica los procs.
/// Supersedes the Round Table Fragment (caps y conversiones no se apilan; esta gana).
/// El motor compartido vive en <see cref="BulwarkEngineRelic"/>.
/// </summary>
public sealed class LordCamelotRestored : BulwarkEngineRelic
{
    public const int MaxRetained = 25;
    public const int StarsPerBlock = 20;
    public const int NpPerHp = 20;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int MaxRetainedBlock => MaxRetained;
    protected override int StarsPerFullBlock => StarsPerBlock;
    protected override int NpPerHpLoss => NpPerHp;
}
