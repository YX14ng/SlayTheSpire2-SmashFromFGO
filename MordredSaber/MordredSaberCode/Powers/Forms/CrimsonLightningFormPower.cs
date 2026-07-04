namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// RELÁMPAGO CARMESÍ (绯红闪电, clímax) — forma PERMANENTE de fin de run, vía la rara «Poder
/// Clímax» (DESIGN-MORDRED §3.bis, forma 3). Lo bueno de las dos sin penalización:
///   - tus Ataques hacen +2;
///   - al final del turno conservás hasta 10 de Bloqueo (Baluarte);
///   - al inicio de tu turno: +5 NP;
///   - NO recibís daño extra (ya no hay tensión del casco).
/// Una vez dentro, ninguna otra forma la reemplaza (<see cref="FormPower.IsPermanent"/>); la ulti
/// pasa a «Interludio» (lo decide el GaugeFilled de MainFile leyendo esta forma).
/// </summary>
public sealed class CrimsonLightningFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => AttackBonus;

    protected override int BlockRetentionCap => MaskedRetention;

    protected override int NpPerTurnStart => NpPerTurn;

    public override bool IsPermanent => true;

    // FramesPath = null (audit 2026-07-04): el .tres "mordred_frames_crimson" no existe en el repo — el
    // swap declaraba un recurso inexistente (no-op con log de error). null = mantener el sprite
    // actual. TODO pase de arte: generar el .tres y restaurar el path.
    public override string? FramesPath => null;
}
