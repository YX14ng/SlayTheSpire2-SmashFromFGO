namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// Forma inicial defensiva: reduce el dano de los ataques, retiene Bloqueo y genera NP.
/// Comparte el set animado oficial de Mordred con las otras formas para evitar duplicarlo.
/// </summary>
public sealed class MaskedKnightFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => -AttackBonus;

    protected override int BlockRetentionCap => MaskedRetention;

    protected override int NpPerTurnStart => NpPerTurn;

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames.tres";
}
