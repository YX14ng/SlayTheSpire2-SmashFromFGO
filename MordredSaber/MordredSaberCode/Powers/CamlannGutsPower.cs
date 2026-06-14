using BaseLib.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Camlann (卡姆兰, §5.3) — la subclase de <see cref="GutsPower"/> que aplica la carta homónima: la
/// primera vez del combate que un golpe te mataría, sobrevivís a 1 de Vida; AL ACTIVARSE, +100 NP
/// (up: y +50 Estrellas). El lore beat (la batalla final de Mordred): caer es el power-spike, el odio
/// estalla en Clarent. Reusa GutsPower de FGOCore (override de OnTriggered, patrón documentado).
/// <see cref="Upgraded"/> habilita el +★; lo fija la carta al aplicar.
/// </summary>
public sealed class CamlannGutsPower : GutsPower
{
    public const int NpOnTrigger = 100;
    public const int StarsOnTrigger = 50;

    public bool Upgraded;

    // Íconos en los recursos de MordredSaber (espejo de MordredPower).
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    protected override async Task OnTriggered(PlayerChoiceContext choiceContext)
    {
        await NpCharge.Gain(Owner, NpOnTrigger, null);
        if (Upgraded)
        {
            await CritStars.Gain(Owner, StarsOnTrigger, null);
        }
    }
}
