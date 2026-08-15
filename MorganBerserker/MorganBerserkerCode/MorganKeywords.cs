using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MorganBerserker.MorganBerserkerCode;

/// <summary>
/// Keywords custom de Morgan (BaseLib <see cref="CustomEnumAttribute"/>: el valor del enum se
/// genera al iniciar y la loc vive en <c>localization/&lt;lang&gt;/card_keywords.json</c> con el
/// prefijo del mod — <c>MORGANBERSERKER-DETONAR.title/.description</c>).
/// </summary>
public static class MorganKeywords
{
    /// <summary>
    /// Detonar (REDESIGN-MORGAN-V2 §3.2) — la Sentencia como keyword legible: «tus Ataques
    /// infligen daño adicional igual a la Maldición del objetivo y la consumen. Multi-golpe:
    /// solo el primer golpe. Ataques a TODOS: detona a cada objetivo golpeado.» El tooltip se
    /// engancha con <c>HoverTipFactory.FromKeyword(MorganKeywords.Detonar)</c>.
    /// </summary>
    [CustomEnum]
    public static CardKeyword Detonar;
}
