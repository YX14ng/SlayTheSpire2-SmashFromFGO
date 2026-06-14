using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Character;
using MordredSaber.MordredSaberCode.Extensions;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Juramento del Caballero de la Traición (叛逆骑士的誓约) — STARTER 2, el medidor de Vínculo de
/// Mordred sobre <see cref="BondRelic"/> de FGOCore (regalos por umbral: HP/NP/Bloqueo + capstone,
/// SIN ×daño global, DESIGN-MORDRED §6). Dos giros propios:
///   - los regalos de Bloqueo default (Nv 4/7) se reemplazan por ESTRELLAS al iniciar combate
///     (+10/+20★ — Mordred banca el relámpago, no es tanque puro);
///   - capstone Nv 10 «Reconocido como Hijo» (被认可为子嗣): la 1ª vez por combate que te quitás el
///     yelmo (entrás en Rebelión): +1⚡ — capeado 1/combate, sin loop ⚡-positivo (patrón
///     RabbitEarsDiadem/QueensScepter). El reconocimiento que su padre le negó, ganado en batalla.
/// </summary>
[Pool(typeof(MordredRelicPool))]
public sealed class OathOfTheKnightOfTreachery : BondRelic, IFormChangeListener
{
    private const int CapstoneLevel = 10;

    public override RelicRarity Rarity => RelicRarity.Starter;

    // El arte vive en los recursos de MordredSaber, no en los de FGOCore.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    private bool _capstoneUsedThisCombat;

    // Mordred no es tanque puro: los regalos de Bloqueo default se reemplazan por Estrellas.
    protected override int StartingBlock(int lv) => 0;

    private int StartingStars(int lv) => lv >= 7 ? 20 : lv >= 4 ? 10 : 0;

    public override async Task BeforeCombatStartLate()
    {
        _capstoneUsedThisCombat = false;
        await base.BeforeCombatStartLate();

        var stars = StartingStars(Level);
        if (stars > 0)
        {
            await CritStars.Gain(Owner.Creature, stars, null);
        }
    }

    /// <summary>
    /// Capstone Nv 10: +1⚡ la 1ª vez por combate que te quitás el yelmo (entrás en Rebelión).
    /// Otras transiciones (ponerse el yelmo, entrar en Clímax) NO lo gatillan.
    /// </summary>
    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_capstoneUsedThisCombat || Level < CapstoneLevel) return;
        if (!Forms.InRebellion(Owner.Creature)) return;
        _capstoneUsedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }
}
