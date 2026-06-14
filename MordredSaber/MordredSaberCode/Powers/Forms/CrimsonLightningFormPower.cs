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

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames_crimson.tres";
}
