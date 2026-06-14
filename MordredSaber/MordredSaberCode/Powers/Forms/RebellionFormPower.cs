namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// CABALLERO DE LA REBELIÓN (sin casco, asc 3) — la ventana de cobro (DESIGN-MORDRED §3.bis,
/// forma 2): entrar con Crítico armado, reventar, volver a ponerse el yelmo.
///   - tus Ataques hacen +2;
///   - recibís +2 de daño por golpe enemigo (la tasa Berserker; cada golpe recibido = +10★ vía
///     la starter — parkear acá es un all-in consciente, no un error).
/// Sin Baluarte ni +NP/turno: la armadura está arrancada. Las cartas «en Rebelión: +X»
/// (Velocidad del Relámpago, Doble Filo del Odio) leen esta forma.
/// </summary>
public sealed class RebellionFormPower : MordredFormPower
{
    protected override int AttackDamageDelta => AttackBonus;

    protected override bool TakesExtraDamage => true;

    public override string FramesPath => $"{MainFile.ResPath}/character/mordred_frames_unmasked.tres";
}
