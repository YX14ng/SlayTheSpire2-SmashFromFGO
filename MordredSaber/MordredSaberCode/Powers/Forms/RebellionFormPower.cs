namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// Forma ofensiva: aumenta el dano de los ataques a cambio de recibir dano adicional.
/// Comparte el set animado oficial de Mordred con las otras formas para evitar duplicarlo.
/// </summary>
public sealed class RebellionFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => AttackBonus;

    protected override bool TakesExtraDamage => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames.tres";
}
