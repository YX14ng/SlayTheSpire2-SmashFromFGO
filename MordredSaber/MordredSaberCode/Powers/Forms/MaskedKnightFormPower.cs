namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// CABALLERO ENMASCARADO (不贞隐藏之兜, asc 1-2) — forma inicial: identidad y parámetros sellados
/// tras el casco (DESIGN-MORDRED §3.bis, forma 1). Tanquear y bancar:
///   - tus Ataques hacen −2 (atacar enmascarada es desperdicio);
///   - al final del turno conservás hasta 10 de Bloqueo (Baluarte, IBlockRetentionSource);
///   - al inicio de tu turno: +5 NP.
/// Las cartas «si Enmascarado: +X» (Chispas del Yelmo, Yelmo Abollado, etc.) leen esta forma.
/// </summary>
public sealed class MaskedKnightFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => -AttackBonus;

    protected override int BlockRetentionCap => MaskedRetention;

    protected override int NpPerTurnStart => NpPerTurn;

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames_masked.tres";
}
