using MashShielder.MashShielderCode.Extensions;
using MashShielder.MashShielderCode.Powers.Forms;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.addons.mega_text;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// La Descarga: el muro convertido en efecto (REDESIGN-MASH-V2 §3 CANDADO 2). Camino ÚNICO para
/// «consumí tu Bloqueo y usalo», con el multiplicador de forma ya horneado y el flotante
/// «¡Descarga! X» para que el jugador VEA que el Bloqueo se convirtió y no que se perdió (misma
/// lección de legibilidad que la Sentencia de Morgan).
/// </summary>
public static class Descarga
{
    private static readonly LocString FloatLoc = new("powers", "MASHSHIELDER-DISCHARGE_FLOAT.text");

    /// <summary>Ortinax/Paladín convierten más: la forma artillera es la que dispara el muro.</summary>
    public const decimal OrtinaxMultiplier = 1.5m;

    /// <summary>Multiplicador de conversión vigente según la forma activa.</summary>
    public static decimal Multiplier(Creature creature) =>
        creature.GetPowerInstances<MashFormPower>().Any(f => f.BoostsDischarge) ? OrtinaxMultiplier : 1m;

    /// <summary>
    /// Consume TODO el Bloqueo y devuelve el monto CONVERTIDO (Bloqueo × multiplicador de forma ×
    /// <paramref name="cardMultiplier"/>), redondeado hacia abajo. Devuelve 0 si no había Bloqueo.
    /// </summary>
    public static async Task<int> All(PlayerChoiceContext choiceContext, Creature owner, decimal cardMultiplier = 1m)
    {
        var block = await owner.ConsumeAllBlock(choiceContext, owner);
        return Convert(owner, block, cardMultiplier);
    }

    /// <summary>Consume hasta <paramref name="cap"/> de Bloqueo y devuelve el monto convertido.</summary>
    public static async Task<int> UpTo(PlayerChoiceContext choiceContext, Creature owner, int cap, decimal cardMultiplier = 1m)
    {
        var block = await owner.ConsumeBlockUpTo(cap, choiceContext, owner);
        return Convert(owner, block, cardMultiplier);
    }

    private static int Convert(Creature owner, int block, decimal cardMultiplier) =>
        block <= 0 ? 0 : (int)(block * Multiplier(owner) * cardMultiplier);

    /// <summary>
    /// Flotante «¡Descarga! X» sobre Mash. Reusa la escena VANILLA de "Blocked"
    /// (<see cref="NDamageBlockedVfx"/>, clase del JUEGO con node factory) y le reescribe el label
    /// tras el AddChild — así no se instancia ningún Node C# del assembly del mod, que en el build
    /// nativo Linux rompe el bridge engine→script (saga FgoSpriteMotion, FGOCore v0.1.22).
    /// </summary>
    public static void ShowFloat(Creature owner, int amount)
    {
        if (amount <= 0) return;
        var node = NDamageBlockedVfx.Create(owner);
        if (node == null) return;
        owner.GetVfxContainer()?.AddChildSafely(node);
        node.GetNodeOrNull<MegaLabel>("Label")?.SetTextAutoSize($"{FloatLoc.GetRawText()} {amount}");
    }
}
