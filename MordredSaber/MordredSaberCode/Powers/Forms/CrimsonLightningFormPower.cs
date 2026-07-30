namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// Forma permanente de climax: combina ataque, retencion de Bloqueo y generacion de NP.
/// Comparte el set animado oficial de Mordred con las otras formas para evitar duplicarlo.
/// </summary>
public sealed class CrimsonLightningFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => AttackBonus;

    protected override int BlockRetentionCap => MaskedRetention;

    protected override int NpPerTurnStart => NpPerTurn;

    public override bool IsPermanent => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames.tres";
}
