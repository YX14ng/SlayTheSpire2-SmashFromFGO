using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// Fachada de Mordred sobre el FormSwitch genérico de FGOCore, más sus consultas de forma.
/// Relámpago Carmesí es permanente: una vez dentro, ninguna otra forma la reemplaza (IsPermanent).
/// </summary>
public static class Forms
{
    /// <summary>¿Tiene el casco puesto? (sella parámetros, atacar hace −2). Relámpago Carmesí NO lo es.</summary>
    public static bool InMaskedForm(Creature creature) => creature.HasPower<MaskedKnightFormPower>();

    /// <summary>¿Está sin casco (Rebelión)? La ventana de cobro all-in.</summary>
    public static bool InRebellion(Creature creature) => creature.HasPower<RebellionFormPower>();

    /// <summary>¿Está en una forma ofensiva (Rebelión o Clímax: Ataques +2)?</summary>
    public static bool InOffensiveForm(Creature creature) =>
        creature.HasPower<RebellionFormPower>() || creature.HasPower<CrimsonLightningFormPower>();

    public static Task Enter<T>(PlayerChoiceContext? choiceContext, Creature creature, CardModel? source) where T : FormPower =>
        FormSwitch.Enter<T>(choiceContext, creature, source);

    /// <summary>
    /// REGLA DE ORO LORE (§3.bis): con el casco puesto NO puede gritar su rebelión → jugar la
    /// ulti estando Enmascarada PRIMERO le arranca el yelmo (entra en Rebelión). No-op si ya
    /// está sin casco o en Clímax permanente. Lo llama la carta-NP «Desatado» antes de pegar.
    /// </summary>
    public static async Task UnmaskForUlt(PlayerChoiceContext? choiceContext, Creature creature, CardModel? source)
    {
        if (InMaskedForm(creature))
        {
            await FormSwitch.Enter<RebellionFormPower>(choiceContext, creature, source);
        }
    }
}
