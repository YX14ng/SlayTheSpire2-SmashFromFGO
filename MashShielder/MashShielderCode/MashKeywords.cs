using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MashShielder.MashShielderCode;

/// <summary>
/// Keywords custom de Mash (BaseLib <see cref="CustomEnumAttribute"/>: el valor del enum se genera
/// al iniciar y la loc vive en <c>localization/&lt;lang&gt;/card_keywords.json</c> con el prefijo del
/// mod — <c>MASHSHIELDER-DESCARGAR.title/.description</c>).
/// </summary>
public static class MashKeywords
{
    /// <summary>
    /// Descargar (REDESIGN-MASH-V2 §3 CANDADO 2) — el pago del muro hecho keyword legible:
    /// «Consume tu Bloqueo (hasta la cantidad indicada; si no se indica, TODO) y lo convierte en el
    /// efecto de la carta.» El tooltip se engancha con
    /// <c>HoverTipFactory.FromKeyword(MashKeywords.Descargar)</c>.
    /// </summary>
    [CustomEnum]
    public static CardKeyword Descargar;
}
