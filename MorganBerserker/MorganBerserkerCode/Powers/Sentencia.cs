using FGOCore.FGOCoreCode.Curses;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.addons.mega_text;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// La Detonación de la Sentencia, compartida por <see cref="Forms.FairyQueenFormPower"/> y
/// <see cref="Forms.WinterQueenFormPower"/> (REDESIGN-MORGAN-V2 §3.2): consumir la Maldición del
/// objetivo, aplicar el re-efecto de Cernunnos (consume solo la MITAD, redondeo arriba — parche
/// M5/J1-4) y mostrar el flotante «¡Sentencia! +X» para que el jugador VEA la Maldición volverse
/// daño (reporte de Steam 2026-08-14: «se limpia a cero, ¿bug?» — no era bug, era ilegible).
/// </summary>
public static class Sentencia
{
    private static readonly LocString FloatLoc = new("powers", "MORGANBERSERKER-SENTENCE_FLOAT.text");

    /// <summary>
    /// Consume la Maldición del objetivo para Detonar y devuelve el BONO de daño (siempre la
    /// Maldición COMPLETA; lo que cambia es cuánta se consume).
    ///
    /// <para>2026-08-16, feedback de Sac2Loo2Sac en Steam: «la forma combinada no debería consumir
    /// los stacks, no podés salir de ella y se supone que es lo mejor de ambas». Tiene razón a
    /// medias — la Reina del Invierno es el clímax PERMANENTE y merece retener campo, pero no
    /// consumir NADA convierte la Maldición en un multiplicador que solo sube y rompe el techo de
    /// saturación de DECISIONS. Solución: en Reina del Invierno la Detonación consume solo la MITAD
    /// (redondeo arriba), que era el efecto de <see cref="CurseOfCernunnosPower"/>. Cernunnos se
    /// reconvirtió al knob que el panel dejó aprobado (§15.4-4 de REDESIGN-MORGAN-V2): +NP por
    /// Detonación. No se acumulan: son efectos distintos.</para>
    ///
    /// <para>Llamar UNA vez por carta (anti doble-dip, patrón _pendingSentence).</para>
    /// </summary>
    public static async Task<int> Detonar(Creature owner, Creature target)
    {
        var curse = Curses.Of(target);
        if (curse <= 0) return 0;

        var consume = owner.HasPower<Forms.WinterQueenFormPower>() ? (curse + 1) / 2 : curse;
        await Curses.Consume(target, consume);

        // Cernunnos: la Detonación alimenta el medidor (puente danza → Sobrecarga).
        var cernunnos = owner.GetPowerAmount<CurseOfCernunnosPower>();
        if (cernunnos > 0)
        {
            await NpCharge.Gain(owner, (int)cernunnos, null);
        }
        return curse;
    }

    /// <summary>
    /// Flotante «¡Sentencia! +X» sobre el objetivo. Reusa la escena VANILLA de "Blocked"
    /// (<see cref="NDamageBlockedVfx"/>, clase del JUEGO con node factory) y le reescribe el
    /// label tras el AddChild — así no se instancia ningún Node C# del assembly del mod, que en
    /// el build nativo Linux rompe el bridge engine→script (saga FgoSpriteMotion, FGOCore v0.1.22).
    /// </summary>
    public static void ShowFloat(Creature target, int amount)
    {
        if (amount <= 0) return;
        var node = NDamageBlockedVfx.Create(target);
        if (node == null) return;
        target.GetVfxContainer()?.AddChildSafely(node);
        // _Ready ya corrió sincrónicamente en AddChild y dejó "Blocked"; lo pisamos con el nuestro.
        node.GetNodeOrNull<MegaLabel>("Label")?.SetTextAutoSize($"{FloatLoc.GetRawText()} +{amount}");
    }
}
